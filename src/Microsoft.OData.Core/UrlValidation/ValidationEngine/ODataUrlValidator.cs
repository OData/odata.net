//---------------------------------------------------------------------
// <copyright file="ODataUrlValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Aggregation;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Validation engine for running validation rules
    /// </summary>
    public sealed class ODataUrlValidator
    {
        private Dictionary<Type, List<ODataUrlValidationRule>> ruleDictionary = new Dictionary<Type, List<ODataUrlValidationRule>>();
        private ConcurrentDictionary<Type, byte> unusedTypes = new ConcurrentDictionary<Type, byte>();
        private ConcurrentDictionary<Type, Type[]> implementedInterfaces = new ConcurrentDictionary<Type, Type[]>();
        internal IEdmModel Model { get; }

        /// <summary>
        /// Validates urls based on a provided model and collection of rules
        /// </summary>
        /// <param name="model">The model to validate a URL against</param>
        /// <param name="rules">The rules to use in validating a URL</param>
        public ODataUrlValidator(IEdmModel model, ODataUrlValidationRuleSet rules)
        {
            this.Model = model;

            // Collect rules for each type
            List<ODataUrlValidationRule> ruleList;
            foreach (ODataUrlValidationRule rule in rules)
            {
                Type ruleType = rule.GetRuleType();
                if (!ruleDictionary.TryGetValue(ruleType, out ruleList))
                {
                    ruleList = new List<ODataUrlValidationRule>();
                    ruleDictionary.Add(ruleType, ruleList);
                }

                ruleList.Add(rule);
            }
        }

        /// <summary>
        /// Validate the given <see cref="ODataUri"/>.
        /// </summary>
        /// <param name="odataUri">The <see cref="ODataUri"/> to validate.</param>
        /// <param name="validationMessages">An output parameter to return any validation messages associated with the oDataUri</param>
        /// <returns>True, if any errors are found, otherwise false.</returns>
        public bool ValidateUrl(ODataUri odataUri, out IEnumerable<ODataUrlValidationMessage> validationMessages)
        {
            ODataUrlValidationContext validationContext = new ODataUrlValidationContext(Model, this);

            // Validate ODataUri
            ValidateItem(odataUri, validationContext);

            // Validate Path
            foreach (ODataPathSegment segment in odataUri.Path)
            {
                ValidatePathSegment(segment, validationContext);
            }

            // Validate Select and Expand
            ValidateSelectExpandClause(odataUri.Path.LastSegment.EdmType.AsElementType(), odataUri.SelectAndExpand, validationContext);

            // Validate Filter Clause
            ValidateFilterClause(odataUri.Filter, validationContext);

            // Validate OrderBy Clause
            ValidateOrderByClause(odataUri.OrderBy, validationContext);

            // Validate Compute Clause
            ValidateComputeClause(odataUri.Compute, validationContext);

            // Validate Apply Clause
            ValidateApplyClause(odataUri.Apply, validationContext);

            // Validate Custom Query Options
            ValidateCustomQueryOptions(odataUri.CustomQueryOptions, validationContext);

            // Validate Search Clause
            ValidateSearchClause(odataUri.Search, validationContext);

            // Must be evaluated as part of the ODataUri (since the values are just primitive types):
            // -Count
            // -Top
            // -Skip
            // -SkipToken
            // -Delta
            validationMessages = validationContext.Messages;
            return !validationMessages.Any();
        }

        /// <summary>
        /// For each type and interface in the item's type heirarchy, calls any validation rules
        /// for the specified type
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <param name="validationContext">Validation Context.</param>
        /// <param name="impliedProperty">Whether the item being validated is an implied property.</param>
        internal void ValidateItem(object item, ODataUrlValidationContext validationContext, bool impliedProperty = false)
        {
            if (item == null)
            {
                return;
            }

            ValidateTypeHierarchy(item, item.GetType(), validationContext, new HashSet<Type>(), impliedProperty, true);
        }

        /// <summary>
        /// Walks the type hierarchy for the given type, checking for rules of each type and interface in the hierarchy
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <param name="itemType">The type to use in looking for rules for the item.</param>
        /// <param name="validationContext">Validation Context.</param>
        /// <param name="validatedTypes">HashSet of types already validated for this item.</param>
        /// <param name="impliedProperty">Whether the item being validated is an implied property.</param>
        /// <returns>True/False.</returns>
        private bool ValidateTypeHierarchy(object item, Type itemType, ODataUrlValidationContext validationContext, HashSet<Type> validatedTypes, bool impliedProperty, bool includeInterfaces)
        {
            bool foundRule = false;

            // if we are at the base type or already know there are no rules in the remaining hierarchy, return
            if (itemType == typeof(object) || unusedTypes.ContainsKey(itemType))
            {
                return false;
            }

            // find any rules defined on the type of the item
            if (ValidateByType(item, itemType, validationContext, impliedProperty))
            {
                foundRule = true;
            }

            // make sure we only validate the item only once per type/interface
            validatedTypes.Add(itemType);

            // find any rules defined on an interface defined on the type of the item
            // GetInterfaces() returns derived interfaces, so only do this at the top level
            if (includeInterfaces)
            {
                Type[] interfaces = GetInterfaces(itemType);
                int length = interfaces.Length;
                for (int i=0; i < length; i++)
                {
                    Type interfaceType = interfaces[i];
                    if (validatedTypes.Add(interfaceType) && !this.unusedTypes.ContainsKey(interfaceType))
                    {
                        if (ValidateByType(item, interfaceType, validationContext, impliedProperty))
                        {
                            foundRule = true;
                        }
                        else
                        {
                            // there were no rules for this interface, so don't try it again
                            this.unusedTypes.TryAdd(interfaceType, 0);
                        }
                    }
                }
            }

            // repeat for any base type
            foundRule = foundRule | ValidateTypeHierarchy(item, itemType.GetTypeInfo().BaseType, validationContext, validatedTypes, impliedProperty, false);

            if (!foundRule)
            {
                this.unusedTypes.TryAdd(itemType, 0);
            }

            return foundRule;
        }

        /// <summary>
        /// Given a type, returns the interfaces implemented by that type
        /// </summary>
        /// <param name="type">The type on which to check for interfaces.</param>
        /// <returns>The interfaces implemented on the type.</returns>
        private Type[] GetInterfaces(Type type)
        {
            Type[] interfaces;
            if(!implementedInterfaces.TryGetValue(type, out interfaces))
            {

#if NETSTANDARD1_1
                interfaces = type.GetInterfaces().ToArray();
#else
                interfaces = type.GetInterfaces();
#endif
                implementedInterfaces.TryAdd(type, interfaces);
            }

            return interfaces;
        }

        /// <summary>
        /// Given an item and a type, call all validation rules associated with the type
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="itemType">Type of rules to run against item.</param>
        /// <param name="validationContext">Validation Context.</param>
        /// <param name="impliedProperty">Whether the item being validated is an implied property.</param>
        /// <returns>True, if at least one rule was found for the type, otherwise false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031", Justification = "Catch user code exception and surface as a validation error.")]
        private bool ValidateByType(object item, Type itemType, ODataUrlValidationContext validationContext, bool impliedProperty)
        {
            List<ODataUrlValidationRule> ruleList;
            if (ruleDictionary.TryGetValue(itemType, out ruleList))
            {
                foreach (ODataUrlValidationRule rule in ruleList)
                {
                    if (!impliedProperty || rule.IncludeImpliedProperties)
                    {
                        try
                        {
                            rule.Validate(validationContext, item);
                        }
                        catch (Exception e)
                        {
                            validationContext.AddMessage(ODataUrlValidationMessageCodes.InvalidRule, Strings.ODataUrlValidationError_InvalidRule(rule.RuleName, e.Message), Severity.Warning);
                        }
                    }
                }

                return true;
            }

            return false;
        }

#region Validate Url Components

        private static void ValidatePathSegment(ODataPathSegment segment, ODataUrlValidationContext validationContext)
        {
            validationContext.PathValidator.ValidatePath(segment);
        }

        private void ValidateSelectExpandClause(IEdmType segmentType, SelectExpandClause selectExpand, ODataUrlValidationContext validationContext)
        {
            ValidateImpliedProperties(segmentType, selectExpand, validationContext);
            if (selectExpand != null)
            {
                ValidateItem(selectExpand, validationContext);
                foreach (SelectItem selectExpandItem in selectExpand.SelectedItems)
                {
                    ValidateSelectItem(selectExpandItem, validationContext);
                }
            }
        }

        private void ValidateFilterClause(FilterClause filter, ODataUrlValidationContext validationContext)
        {
            if (filter != null)
            {
                ValidateItem(filter, validationContext);
                validationContext.ExpressionValidator.ValidateNode(filter.Expression);
            }
        }

        private void ValidateOrderByClause(OrderByClause orderBy, ODataUrlValidationContext validationContext)
        {
            if (orderBy != null)
            {
                ValidateItem(orderBy, validationContext);
                validationContext.ExpressionValidator.ValidateNode(orderBy.Expression);
                ValidateOrderByClause(orderBy.ThenBy, validationContext);
            }
        }

        private void ValidateApplyClause(ApplyClause apply, ODataUrlValidationContext validationContext)
        {
            if (apply != null)
            {
                ValidateItem(apply, validationContext);
                foreach (TransformationNode transformation in apply.Transformations)
                {
                    ValidateTransformation(transformation, validationContext);
                }
            }
        }

        private void ValidateComputeClause(ComputeClause compute, ODataUrlValidationContext validationContext)
        {
            if (compute != null)
            {
                ValidateItem(compute, validationContext);
                foreach (ComputeExpression computeExpression in compute.ComputedItems)
                {
                    ValidateItem(computeExpression, validationContext);
                    validationContext.ExpressionValidator.ValidateNode(computeExpression.Expression);
                }
            }
        }

        private void ValidateCustomQueryOptions(IEnumerable<QueryNode> queryOptions, ODataUrlValidationContext validationContext)
        {
            if (queryOptions != null)
            {
                foreach (QueryNode option in queryOptions)
                {
                    ValidateItem(option, validationContext);
                }
            }
        }

        private void ValidateSearchClause(SearchClause search, ODataUrlValidationContext validationContext)
        {
            if (search != null)
            {
                ValidateItem(search, validationContext);
                validationContext.ExpressionValidator.ValidateNode(search.Expression);
            }
        }

        private void ValidateSelectItem(SelectItem selectExpandItem, ODataUrlValidationContext validationContext)
        {
            if (selectExpandItem != null)
            {
                ExpandedNavigationSelectItem expandItem;
                PathSelectItem pathItem;
                ExpandedReferenceSelectItem referenceSelectItem;
                if ((pathItem = selectExpandItem as PathSelectItem) != null)
                {
                    ValidatePathItem(pathItem, validationContext);
                }
                else if ((expandItem = selectExpandItem as ExpandedNavigationSelectItem) != null)
                {
                    ValidateExpandItem(expandItem, validationContext);
                }
                else if ((referenceSelectItem = selectExpandItem as ExpandedReferenceSelectItem) != null)
                {
                    ValidateExpandReferenceItem(referenceSelectItem, validationContext);
                }
            }
        }

        private void ValidatePathItem(PathSelectItem pathItem, ODataUrlValidationContext validationContext)
        {
            if (pathItem != null)
            {
                ValidateItem(pathItem, validationContext);

                foreach (ODataPathSegment segment in pathItem.SelectedPath)
                {
                    ValidatePathSegment(segment, validationContext);
                }

                ValidateSelectExpandClause(pathItem.SelectedPath.LastSegment.EdmType.AsElementType(), pathItem.SelectAndExpand, validationContext);
                ValidateFilterClause(pathItem.FilterOption, validationContext);
                ValidateOrderByClause(pathItem.OrderByOption, validationContext);
                ValidateComputeClause(pathItem.ComputeOption, validationContext);
                ValidateSearchClause(pathItem.SearchOption, validationContext);
            }
        }

        private void ValidateExpandItem(ExpandedNavigationSelectItem expandItem, ODataUrlValidationContext validationContext)
        {
            ValidateExpandReferenceItem(expandItem, validationContext);
            ValidateSelectExpandClause(expandItem.PathToNavigationProperty.LastSegment.EdmType.AsElementType(), expandItem.SelectAndExpand, validationContext);
            ValidateComputeClause(expandItem.ComputeOption, validationContext);
            ValidateApplyClause(expandItem.ApplyOption, validationContext);
        }

        private void ValidateExpandReferenceItem(ExpandedReferenceSelectItem expandItem, ODataUrlValidationContext validationContext)
        {
            ValidateItem(expandItem, validationContext);
            foreach (ODataPathSegment segment in expandItem.PathToNavigationProperty)
            {
                ValidatePathSegment(segment, validationContext);
            }

            ValidateFilterClause(expandItem.FilterOption, validationContext);
            ValidateOrderByClause(expandItem.OrderByOption, validationContext);
            ValidateSearchClause(expandItem.SearchOption, validationContext);

            // todo: apply and compute are also defined on ExpandReferenceItem, but should only be valid on ExpandItem
            // add rule to fail if these are found on ExpandReferenceItem that is not ExpandItem?
        }

        // If the last segment of a path is a structured type with no select, validate implied properties
        private void ValidateImpliedProperties(IEdmType segmentType, SelectExpandClause selectExpand, ODataUrlValidationContext validationContext)
        {
            if (selectExpand == null)
            {
                // Validate all properties for the type
                // ValdiateProperties handles non-structured types, so we don't have to check here
                ValidateProperties(segmentType, validationContext);
            }
            else
            {
                bool propertySelected = false;

                // Check each property in the SelectAndExpand to see if they terminate in a structured type with no select
                foreach (SelectItem selectItem in selectExpand.SelectedItems)
                {
                    if (selectItem is WildcardSelectItem)
                    {
                        // Validate all properties for the type
                        ValidateProperties(segmentType, validationContext);
                        
                        // Make sure we don't validate the type again at the end
                        propertySelected = true;
                    }
                    else
                    {
                        ExpandedNavigationSelectItem expandItem;
                        PathSelectItem pathSelectItem = selectItem as PathSelectItem;
                        if (pathSelectItem != null)
                        {
                            // SelectItem is a propety. See if it is structured with no select
                            ValidateImpliedProperties(pathSelectItem.SelectedPath.LastSegment.EdmType.AsElementType(), pathSelectItem.SelectAndExpand, validationContext);

                            // At least one non-navigation property was in the select list
                            propertySelected = true;
                        }
                        else if ((expandItem = selectItem as ExpandedNavigationSelectItem) != null)
                        {
                            // SelectItem is a navigation property. Make sure it has a select.
                            ValidateImpliedProperties(expandItem.PathToNavigationProperty.LastSegment.EdmType.AsElementType(), expandItem.SelectAndExpand, validationContext);
                        }
                    }

                    // If we didn't find any non-expand properties, and didn't already validate due to a WildCard, then validate type now
                    if (!propertySelected)
                    {
                        ValidateProperties(segmentType, validationContext);
                    }
                }
            }
        }

        // Validate all structured properties of a type, recursing through nested complex typed properties
        private void ValidateProperties(IEdmType edmType, ODataUrlValidationContext context)
        {
            // true if the element is added to the set; false if the element is already in the set.
            if (edmType.TypeKind != EdmTypeKind.Primitive && context.ValidatedTypes.Add(edmType))
            {
                IEdmStructuredType structuredType = edmType as IEdmStructuredType;
                if (structuredType != null)
                {
                    foreach (IEdmProperty property in structuredType.StructuralProperties())
                    {
                        ValidateItem(property, context, /* impliedProperty */ true);

                        IEdmType elementType = property.Type.Definition.AsElementType();
                        if (elementType.TypeKind != EdmTypeKind.Primitive && !context.ValidatedTypes.Contains(elementType))
                        {
                            ValidateItem(elementType, context, /* impliedProperty */ true);
                            ValidateProperties(elementType, context);
                        }
                    }
                }
            }
        }

#endregion

#region Validate Aggregation Transformations
        private void ValidateTransformation(TransformationNode transformation, ODataUrlValidationContext validationContext)
        {
            ValidateItem(transformation, validationContext);

            AggregateTransformationNode aggregate;
            GroupByTransformationNode groupBy;
            FilterTransformationNode filter;
            ComputeTransformationNode compute;
            ExpandTransformationNode expand;
            if ((aggregate = transformation as AggregateTransformationNode) != null)
            {
                foreach (AggregateExpression aggregateExpression in aggregate.AggregateExpressions)
                {
                    ValidateItem(aggregateExpression, validationContext);
                    ValidateItem(aggregateExpression.Method, validationContext);
                    ValidateItem(aggregateExpression.MethodDefinition, validationContext);
                    validationContext.ExpressionValidator.ValidateNode(aggregateExpression.Expression);
                }
            }
            else if ((groupBy = transformation as GroupByTransformationNode) != null)
            {
                ValidateTransformation(groupBy.ChildTransformations, validationContext);
                foreach (GroupByPropertyNode property in groupBy.GroupingProperties)
                {
                    ValidateGroupByPropertyNode(property, validationContext);
                }
            }
            else if ((filter = transformation as FilterTransformationNode) != null)
            {
                ValidateFilterClause(filter.FilterClause, validationContext);
            }
            else if ((compute = transformation as ComputeTransformationNode) != null)
            {
                foreach (ComputeExpression computeExpression in compute.Expressions)
                {
                    validationContext.ExpressionValidator.ValidateNode(computeExpression.Expression);
                }
            }
            else if ((expand = transformation as ExpandTransformationNode) != null)
            {
                // fine to pass null as type since the expand clause will not be null
                ValidateSelectExpandClause(null, expand.ExpandClause, validationContext);
            }
        }

        private void ValidateGroupByPropertyNode(GroupByPropertyNode groupByProperty, ODataUrlValidationContext validationContext)
        {
            ValidateItem(groupByProperty, validationContext);
            validationContext.ExpressionValidator.ValidateNode(groupByProperty.Expression);
            foreach (GroupByPropertyNode childTransformation in groupByProperty.ChildTransformations)
            {
                ValidateGroupByPropertyNode(childTransformation, validationContext);
            }
        }

#endregion
    }
}