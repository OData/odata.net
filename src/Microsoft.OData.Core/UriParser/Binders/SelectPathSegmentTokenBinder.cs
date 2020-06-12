//---------------------------------------------------------------------
// <copyright file="SelectPathSegmentTokenBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Edm.Vocabularies;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Builds segments from tokens within $select.
    /// </summary>
    internal static class SelectPathSegmentTokenBinder
    {
        /// <summary>
        /// Build a segment from a token.
        /// </summary>
        /// <param name="tokenIn">the token to bind</param>
        /// <param name="model">The model.</param>
        /// <param name="edmType">the type of the current scope based on type segments.</param>
        /// <param name="resolver">Resolver for uri parser.</param>
        /// <param name="state">The binding state.</param>
        /// <returns>The segment created from the token.</returns>
        public static ODataPathSegment ConvertNonTypeTokenToSegment(PathSegmentToken tokenIn, IEdmModel model, IEdmStructuredType edmType, ODataUriResolver resolver, BindingState state = null)
        {
            ExceptionUtils.CheckArgumentNotNull(resolver, "resolver");

            ODataPathSegment nextSegment;
            if (UriParserHelper.IsAnnotation(tokenIn.Identifier))
            {
                if (TryBindAsDeclaredTerm(tokenIn, model, resolver, out nextSegment))
                {
                    return nextSegment;
                }

                string qualifiedTermName = tokenIn.Identifier.Remove(0, 1);
                int separator = qualifiedTermName.LastIndexOf(".", StringComparison.Ordinal);
                string namespaceName = qualifiedTermName.Substring(0, separator);
                string termName = qualifiedTermName.Substring(separator == 0 ? 0 : separator + 1);

                // Don't allow selecting odata control information
                if (String.Compare(namespaceName, ODataConstants.ODataPrefix, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(tokenIn.Identifier));
                }

                return new AnnotationSegment(new EdmTerm(namespaceName, termName, EdmCoreModel.Instance.GetUntyped()));
            }

            EndPathToken endPathToken = new EndPathToken(tokenIn.Identifier, null);

            if ((state?.IsCollapsed ?? false) && !(state?.AggregatedPropertyNames?.Contains(endPathToken) ?? false))
            {
                throw new ODataException(ODataErrorStrings.ApplyBinder_GroupByPropertyNotPropertyAccessValue(tokenIn.Identifier));
            }

            if (TryBindAsDeclaredProperty(tokenIn, edmType, resolver, out nextSegment))
            {
                return nextSegment;
            }

            // Operations must be container-qualified, and because the token type indicates it was not a .-separated identifier, we should not try to look up operations.
            if (tokenIn.IsNamespaceOrContainerQualified())
            {
                if (TryBindAsOperation(tokenIn, model, edmType, out nextSegment))
                {
                    return nextSegment;
                }

                // If an action or function is requested in a selectItem using a qualifiedActionName or a qualifiedFunctionName
                // and that operation cannot be bound to the entities requested, the service MUST ignore the selectItem.
                if (!edmType.IsOpen)
                {
                    return null;
                }
            }

            if (edmType.IsOpen  || (state?.AggregatedPropertyNames?.Contains(endPathToken) ?? false))
            {
                return new DynamicPathSegment(tokenIn.Identifier);
            }

            throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(edmType.FullTypeName(), tokenIn.Identifier));
        }

        /// <summary>
        /// Build a wildcard selection item
        /// </summary>
        /// <param name="tokenIn">the token to bind to a wildcard</param>
        /// <param name="model">the model to search for this wildcard</param>
        /// <param name="item">the new wildcard selection item, if we found one</param>
        /// <returns>true if we successfully bound to a wildcard, false otherwise</returns>
        public static bool TryBindAsWildcard(PathSegmentToken tokenIn, IEdmModel model, out SelectItem item)
        {
            bool isTypeToken = tokenIn.IsNamespaceOrContainerQualified();
            bool wildcard = tokenIn.Identifier.EndsWith("*", StringComparison.Ordinal);

            if (isTypeToken && wildcard)
            {
                string namespaceName = tokenIn.Identifier.Substring(0, tokenIn.Identifier.Length - 2);

                if (model.DeclaredNamespaces.Any(declaredNamespace => declaredNamespace.Equals(namespaceName, StringComparison.Ordinal)))
                {
                    item = new NamespaceQualifiedWildcardSelectItem(namespaceName);
                    return true;
                }
            }

            if (tokenIn.Identifier == "*")
            {
                item = new WildcardSelectItem();
                return true;
            }

            item = null;
            return false;
        }

        /// <summary>
        /// Tries to bind a given token as an Operation.
        /// </summary>
        /// <param name="pathToken">Token to bind.</param>
        /// <param name="model">The model.</param>
        /// <param name="entityType">the current entity type to use as the binding type when looking for operations.</param>
        /// <param name="segment">Bound segment if the token was bound to an operation successfully, or null.</param>
        /// <returns>True if the token was bound successfully, or false otherwise.</returns>
        internal static bool TryBindAsOperation(PathSegmentToken pathToken, IEdmModel model, IEdmStructuredType entityType, out ODataPathSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToken, "pathToken");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");

            IEnumerable<IEdmOperation> possibleFunctions = Enumerable.Empty<IEdmOperation>();
            IList<string> parameterNames = new List<string>();

            // Catch all catchable exceptions as FindDeclaredBoundOperations is implemented by anyone.
            // If an exception occurs it will be suppressed and the possible functions will be empty and return false.
            try
            {
                int wildCardPos = pathToken.Identifier.IndexOf("*", StringComparison.Ordinal);
                if (wildCardPos > -1)
                {
                    string namespaceName = pathToken.Identifier.Substring(0, wildCardPos - 1);
                    possibleFunctions = model.FindBoundOperations(entityType).Where(o => o.Namespace == namespaceName);
                }
                else
                {
                    NonSystemToken nonSystemToken = pathToken as NonSystemToken;
                    if (nonSystemToken != null && nonSystemToken.NamedValues != null)
                    {
                        parameterNames = nonSystemToken.NamedValues.Select(s => s.Name).ToList();
                    }

                    if (parameterNames.Count > 0)
                    {
                        // Always force to use fully qualified name when select operation
                        possibleFunctions = model.FindBoundOperations(entityType).FilterByName(true, pathToken.Identifier).FilterOperationsByParameterNames(parameterNames, false);
                    }
                    else
                    {
                        possibleFunctions = model.FindBoundOperations(entityType).FilterByName(true, pathToken.Identifier);
                    }
                }
            }
            catch (Exception exc)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(exc))
                {
                    throw;
                }
            }

            // Only filter if there is more than one and its needed.
            if (possibleFunctions.Count() > 1)
            {
                possibleFunctions = possibleFunctions.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(entityType);
            }

            // If more than one overload matches, try to select based on optional parameters
            if (possibleFunctions.Count() > 1 && parameterNames.Count > 0)
            {
                possibleFunctions = possibleFunctions.FilterOverloadsBasedOnParameterCount(parameterNames.Count);
            }

            if (!possibleFunctions.HasAny())
            {
                segment = null;
                return false;
            }

            possibleFunctions.EnsureOperationsBoundWithBindingParameter();

            segment = new OperationSegment(possibleFunctions, null /*entitySet*/);
            return true;
        }

        /// <summary>
        /// Tries to bind a given token as an a declared structural or navigation property.
        /// </summary>
        /// <param name="tokenIn">Token to bind.</param>
        /// <param name="edmType">the type to search for this property</param>
        /// <param name="resolver">Resolver for uri parser.</param>
        /// <param name="segment">Bound segment if the token was bound to a declared property successfully, or null.</param>
        /// <returns>True if the token was bound successfully, or false otherwise.</returns>
        private static bool TryBindAsDeclaredProperty(PathSegmentToken tokenIn, IEdmStructuredType edmType, ODataUriResolver resolver, out ODataPathSegment segment)
        {
            IEdmProperty prop = resolver.ResolveProperty(edmType, tokenIn.Identifier);
            if (prop == null)
            {
                segment = null;
                return false;
            }

            if (prop.PropertyKind == EdmPropertyKind.Structural)
            {
                segment = new PropertySegment((IEdmStructuralProperty)prop);
                return true;
            }

            if (prop.PropertyKind == EdmPropertyKind.Navigation)
            {
                segment = new NavigationPropertySegment((IEdmNavigationProperty)prop, null /*TODO: set*/);
                return true;
            }

            throw new ODataException(ODataErrorStrings.SelectExpandBinder_UnknownPropertyType(prop.Name));
        }

        /// <summary>
        /// Tries to bind a given token as a declared annotation term.
        /// </summary>
        /// <param name="tokenIn">Token to bind.</param>
        /// <param name="model">The model to search for this term</param>
        /// <param name="resolver">Resolver for uri parser.</param>
        /// <param name="segment">Bound segment if the token was bound to a declared term successfully, or null.</param>
        /// <returns>True if the token was bound successfully, or false otherwise.</returns>
        private static bool TryBindAsDeclaredTerm(PathSegmentToken tokenIn, IEdmModel model, ODataUriResolver resolver, out ODataPathSegment segment)
        {
            if (!UriParserHelper.IsAnnotation(tokenIn.Identifier))
            {
                segment = null;
                return false;
            }

            IEdmTerm term = resolver.ResolveTerm(model, tokenIn.Identifier.Remove(0, 1));
            if (term == null)
            {
                segment = null;
                return false;
            }

            segment = new AnnotationSegment(term);
            return true;
        }
    }
}