//---------------------------------------------------------------------
// <copyright file="ODataUriEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Default implementation of the OData uri evaluator contract.
    /// </summary>
    [ImplementationName(typeof(IODataUriEvaluator), "Default")]
    public class ODataUriEvaluator : IODataUriEvaluator
    {
        private ODataUri currentUri;
        private EntitySet expectedEntitySet;
        private bool shouldLastSegmentBeSingleton;
        private int? expectedPageSize = null;
        private bool applyPagingInExpands = false;

        /// <summary>
        /// Gets or sets the query evaluator
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryExpressionEvaluator Evaluator { get; set; }

        /// <summary>
        /// Gets or sets the query repository
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public QueryRepository Repository { get; set; }
        
        /// <summary>
        /// Gets or sets the uri to string converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter UriConverter { get; set; }

        /// <summary>
        /// Gets or sets the formatter to use for key values
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Gets or sets the query resolver to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryResolver QueryResolver { get; set; }

        /// <summary>
        /// Processes the given uri and produces an expected value according to the conventions of an OData server implementation
        /// </summary>
        /// <param name="uri">The uri to process</param>
        /// <param name="applySelectAndExpand">A value indicating whether or not $select and $expand should be applied to the query values</param>
        /// <param name="applyPaging">A value indicating whether server-driven paging should be applied to the query values</param>
        /// <returns>The value resulting from processing the uri</returns>
        public QueryValue Evaluate(ODataUri uri, bool applySelectAndExpand, bool applyPaging)
        {
            this.applyPagingInExpands = applyPaging;
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            ExceptionUtilities.CheckCollectionNotEmpty(uri.Segments, "uri.Segments");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.currentUri = uri;
            this.shouldLastSegmentBeSingleton = false;

            applyPaging &= uri.TryGetExpectedEntitySet(out this.expectedEntitySet);

            if (applyPaging)
            {
                // compute the expected page size for this request based on the existence of a universal page size or set-specific page size
                this.expectedPageSize = this.expectedEntitySet.GetEffectivePageSize();
            }
            else
            {
                this.expectedPageSize = null;
            }

            var query = this.BuildQueryFromSegments();
            ExceptionUtilities.CheckObjectNotNull(query, "Could not build query from uri '{0}'", uri);

            // we explicitly do not process expand or select here because they do not affect the number of rows returned or their ordering
            query = this.ProcessFilter(query);
            ExceptionUtilities.CheckObjectNotNull(query, "Could not build query from uri '{0}'", uri);

            if (uri.IsEntitySet())
            {
                // TODO: skip-token processing should happen before ordering
                query = this.ProcessOrderBy(query);
                ExceptionUtilities.CheckObjectNotNull(query, "Could not build query from uri '{0}'", uri);

                query = this.ProcessSkipAndTop(query);
                ExceptionUtilities.CheckObjectNotNull(query, "Could not build query from uri '{0}'", uri);
            }

            // handle $count requests
            if (uri.IsCount())
            {
                query = query.LongCount();
            }

            // need to resolve types since we constructed new QueryExpression tree
            var resolvedQuery = this.QueryResolver.Resolve(query);

            var value = this.Evaluator.Evaluate(resolvedQuery);
            ExceptionUtilities.CheckObjectNotNull(value, "Could not evaluate query '{0}'", query);
            ExceptionUtilities.Assert(value.EvaluationError == null, "Query evaluation produced an error: {0}", value.EvaluationError);

            // post-process the result to defer properties that were not expanded
            if (applySelectAndExpand)
            {
                value = this.ApplySelectAndExpand(uri, value);
            }

            // fixup for key expressions that should return singletons
            if (this.shouldLastSegmentBeSingleton)
            {
                value = EnsureLastSegmentIsSingletonOrNull(value);
            }

            this.currentUri = null;
            this.expectedEntitySet = null;
            this.expectedPageSize = null;

            return value;
        }

        /// <summary>
        /// Gets the type in the type hierarchy where the navigation property with the given hane is defined
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="navigationName">The name of the navigation property to search</param>
        /// <returns>The series of properties for the path</returns>
        internal static EntityType GetTypeWhereNavigationPropertyIsDefined(EntityType entityType, string navigationName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(navigationName, "navigationName");

            if (!entityType.AllNavigationProperties.Select(p => p.Name).Contains(navigationName))
            {
                return null;
            }

            if (entityType.NavigationProperties.Any(np => np.Name == navigationName))
            {
                return entityType;
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(entityType.BaseType, "Parent of type '{0}' was not expected to be null.", entityType.FullName);
                return GetTypeWhereNavigationPropertyIsDefined(entityType.BaseType, navigationName);
            }
        }

        /// <summary>
        /// Tries to get a path from the given set of paths which matches the given navigation name.
        /// </summary>
        /// <param name="searchPaths">The set of possible paths to match with the navigation name</param>
        /// <param name="navigationName">The name of the navigation property to look for.</param>
        /// <param name="entityType">The entity type where the property is defined or a subtype of the entity type where the property is defined</param>
        /// <param name="foundPath">If found, the path that matches the given property; the given navigation name otherwise.</param>
        /// <returns>Whether or not a path matches the given property</returns>
        internal static bool TryGetExpandedPath(IEnumerable<string> searchPaths, string navigationName, EntityType entityType, out string foundPath)
        {
            ExceptionUtilities.CheckArgumentNotNull(searchPaths, "searchPaths");
            ExceptionUtilities.CheckArgumentNotNull(navigationName, "navigationName");
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            EntityType typeWhereNavigationIsDefined = GetTypeWhereNavigationPropertyIsDefined(entityType, navigationName);
            ExceptionUtilities.CheckObjectNotNull(typeWhereNavigationIsDefined, "Cannot find navigation in entity type");

            foreach (string typeName in DerivedTypesAndSelf(typeWhereNavigationIsDefined).Select(t => t.FullName))
            {
                foreach (string path in searchPaths)
                {
                    if (path.StartsWith(Uri.EscapeDataString(navigationName), StringComparison.Ordinal))
                    {
                        foundPath = Uri.EscapeDataString(navigationName);
                        return true;
                    }
                    else if (path.StartsWith(Uri.EscapeDataString(typeName) + "/" + Uri.EscapeDataString(navigationName), StringComparison.Ordinal))
                    {
                        foundPath = Uri.EscapeDataString(typeName) + "/" + Uri.EscapeDataString(navigationName);
                        return true;
                    }
                }
            }

            foundPath = navigationName;
            return false;
        }

        private static QueryValue EnsureLastSegmentIsSingletonOrNull(QueryValue value)
        {
            QueryCollectionValue collection = value as QueryCollectionValue;
            if (collection != null)
            {
                if (collection.IsNull || collection.Elements.Count == 0)
                {
                    value = collection.Type.ElementType.NullValue;
                }
                else
                {
                    value = collection.Elements.Single();
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the names and full names of the given type and its derived types.
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <returns>The names and full names of the given type and its derived types</returns>
        private static IEnumerable<EntityType> DerivedTypesAndSelf(EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "The given entity type should not be null");
            ExceptionUtilities.CheckObjectNotNull(entityType.Model, "The model for the given entity type should no be null. Entity type with null model: '{0}'.", entityType.FullName);

            List<EntityType> derivedTypesAndSelf = new List<EntityType>();
            derivedTypesAndSelf.Add(entityType);
            derivedTypesAndSelf.AddRange(entityType.Model.EntityTypes.Where(t => t.IsKindOf(entityType)));
            return derivedTypesAndSelf;
        }

        /// <summary>
        /// Recursively hides the properties, and any child properties.
        /// </summary>
        /// <param name="maskedQueryValue">The query value with properties to hide.</param>
        /// <param name="propertyName">The name of the property to hide.</param>
        private static void HidePropertyRecursive(MaskedQueryStructuralValue maskedQueryValue, string propertyName)
        {
            var propertyValue = maskedQueryValue.GetValue(propertyName) as QueryStructuralValue;
            if (propertyValue != null)
            {
                var maskedPropertyValue = propertyValue as MaskedQueryStructuralValue;
                if (maskedPropertyValue == null)
                {
                    maskedPropertyValue = MaskedQueryStructuralValue.Create(propertyValue);
                }

                var subPropertyNames = maskedPropertyValue.AllMemberNames.ToList();
                foreach (string subPropertyName in subPropertyNames)
                {
                    HidePropertyRecursive(maskedPropertyValue, subPropertyName);
                }

                maskedQueryValue.SetValue(propertyName, maskedPropertyValue);
            }

            maskedQueryValue.HideMember(propertyName);
        }

        private QueryValue ApplySelectAndExpand(ODataUri uri, QueryValue value)
        {
            // Note: this is applied on all URIs, and it is left up to the FixupPropertiesForExpand helper method
            // to skip cases where it is not an entity type
            var expandedPaths = uri.ExpandSegments.Select(s => this.UriConverter.ConcatenateSegments(s));
            value = this.VisitEntityValues(value, s => this.FixupPropertiesForExpand(s, expandedPaths));
            var collection = value as QueryCollectionValue;

            // since we remove expand expression earlier, we need explictly examine it here and set correct IsSorted value for collections.
            if (collection != null)
            {
                var strategy = collection.Type.ElementType.EvaluationStrategy as ILinqToAstoriaQueryEvaluationStrategy;
                ExceptionUtilities.CheckObjectNotNull(strategy, "Cannot get astoria-specific evaluation strategy from collection value.");

                if (uri.ExpandSegments.Any() && !strategy.IsCollectionOrderPredictable)
                {
                    value = QueryCollectionValue.Create(collection.Type.ElementType, collection.Elements, false);
                }
            }

            // if there are any select segments, then fixup the value
            if (uri.SelectSegments.Count > 0)
            {
                // post-process the result to omit properties that were not selected
                var selectedPaths = uri.SelectSegments.Select(s => this.UriConverter.ConcatenateSegments(s.Where(s2 => !(s2 is EntityTypeSegment))));
                value = this.VisitEntityValues(value, s => this.FixupPropertiesForSelect(s, selectedPaths));
            }

            return value;
        }

        /// <summary>
        /// Constructs a query expression based on the current uri's segments
        /// </summary>
        /// <returns>A query expression</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Temporarily allowing high coupling.")]
        private QueryExpression BuildQueryFromSegments()
        {
            var segments = this.currentUri.Segments;
            EntitySet currentEntitySet = null;
            QueryExpression query = null;
            foreach (var segment in segments)
            {
                if (segment.SegmentType == ODataUriSegmentType.ServiceRoot || segment == SystemSegment.Count || segment == SystemSegment.EntityReferenceLinks || segment == SystemSegment.Value)
                {
                    continue;
                }
                else if (segment.SegmentType == ODataUriSegmentType.EntitySet)
                {
                    // must be the root query
                    ExceptionUtilities.Assert(query == null, "Cannot have multiple entity set segments in the uri");

                    var entitySetSegment = (EntitySetSegment)segment;
                    query = CommonQueryBuilder.Root(entitySetSegment.EntitySet.Name, this.Repository.RootDataTypes[entitySetSegment.EntitySet.Name].CreateCollectionType());
                    currentEntitySet = entitySetSegment.EntitySet;
                }
                else if (segment.SegmentType == ODataUriSegmentType.Key)
                {
                    // build the key expression
                    ExceptionUtilities.CheckObjectNotNull(query, "Key expression present in uri before any root segment");
                    var keySegment = (KeyExpressionSegment)segment;
                    var values = keySegment.IncludedValues.Select(p => new NamedValue(p.Key.Name, p.Value));
                    query = query.Key(values);

                    this.shouldLastSegmentBeSingleton  = true;
                }
                else if (segment.SegmentType == ODataUriSegmentType.NavigationProperty)
                {
                    ExceptionUtilities.CheckObjectNotNull(query, "Navigation property present in uri before any root segment");

                    var navigationSegment = (NavigationSegment)segment;
                    query = query.Property(navigationSegment.NavigationProperty.Name);

                    ExceptionUtilities.CheckObjectNotNull(currentEntitySet, "Expected an EntitySet Segment prior to a NavigationSegment");
                    currentEntitySet = currentEntitySet.GetRelatedEntitySet(navigationSegment.NavigationProperty);

                    this.shouldLastSegmentBeSingleton = false;
                }
                else if (segment.SegmentType == ODataUriSegmentType.NamedStream)
                {
                    var namedStreamSegment = (NamedStreamSegment)segment;
                    query = query.Property(namedStreamSegment.Name);

                    this.shouldLastSegmentBeSingleton = false;
                }
                else if (segment.SegmentType == ODataUriSegmentType.EntityType)
                {
                    query = this.AddEntityTypeFilter(currentEntitySet, query, (EntityTypeSegment)segment);
                }
                else if (segment.SegmentType == ODataUriSegmentType.Function)
                {
                    ExceptionUtilities.Assert(query == null, "Cannot have multiple root query segments in the uri");
                    
                    var serviceOperationSegment = (FunctionSegment)segment;
                    Function serviceOperation = serviceOperationSegment.Function;
                    var bodyAnnotation = serviceOperation.Annotations.OfType<FunctionBodyAnnotation>().SingleOrDefault();
                    ExceptionUtilities.CheckObjectNotNull(bodyAnnotation, "Cannot evaluate function without body annotation");

                    List<QueryExpression> serviceOpArguments = new List<QueryExpression>();
                    foreach (var parameter in serviceOperation.Parameters)
                    {
                        var parameterPrimitiveType = parameter.DataType as PrimitiveDataType;
                        ExceptionUtilities.CheckObjectNotNull(parameterPrimitiveType, "Non primitive parameter types are unsupported");

                        string parameterValue;
                        if (this.currentUri.CustomQueryOptions.TryGetValue(parameter.Name, out parameterValue))
                        {
                            object argValue = this.LiteralConverter.DeserializePrimitive(parameterValue, parameterPrimitiveType.GetFacetValue<PrimitiveClrTypeFacet, Type>(typeof(object)));
                            serviceOpArguments.Add(CommonQueryBuilder.Constant(argValue));
                        }
                        else
                        {
                            serviceOpArguments.Add(CommonQueryBuilder.Null(this.Repository.TypeLibrary.GetDefaultQueryType(parameterPrimitiveType)));
                        }
                    }

                    query = new QueryCustomFunctionCallExpression(
                        this.Repository.TypeLibrary.GetDefaultQueryType(serviceOperation.ReturnType),
                        serviceOperation,
                        bodyAnnotation.FunctionBody,
                        false,
                        false,
                        serviceOpArguments.ToArray());

                    serviceOperation.TryGetExpectedServiceOperationEntitySet(out currentEntitySet);
                }
                else
                {
                    // must be a property segment
                    var propertySegment = segment as PropertySegment;
                    ExceptionUtilities.CheckObjectNotNull(propertySegment, "Uri segments contained unhandled segment type '{0}'", segment.SegmentType);
                    ExceptionUtilities.CheckObjectNotNull(query, "Property present in uri before any root segment");

                    query = query.Property(propertySegment.Property.Name);

                    this.shouldLastSegmentBeSingleton = false;
                }
            }

            return query;
        }

        private QueryExpression AddEntityTypeFilter(EntitySet currentEntitySet, QueryExpression query, EntityTypeSegment oftypeSegment)
        {
            ExceptionUtilities.CheckObjectNotNull(currentEntitySet, "Expected an EntitySet Segment prior to a an EntityTypeSegment");

            var queryEntityType = this.Repository.TypeLibrary.GetQueryEntityType(currentEntitySet, oftypeSegment.EntityType);
            ExceptionUtilities.CheckObjectNotNull(queryEntityType, "QueryEntityType must not be null");
            
            // have to resolve the query to determine if its a singleton or a collection so far
            query = this.QueryResolver.Resolve(query);
            if (query.ExpressionType is QueryCollectionType)
            {
                query = query.OfType(queryEntityType);
            }
            else
            {
                query = query.As(queryEntityType);
            }
            
            return query;
        }
        
        /// <summary>
        /// Processes the $filter portion of the uri. Note that only uri's that were originally built from queries can be handled here
        /// </summary>
        /// <param name="source">The source expression</param>
        /// <returns>The result of processing the filter</returns>
        private QueryExpression ProcessFilter(QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            var queryBasedUri = this.currentUri as QueryBasedODataUri;
            if (queryBasedUri != null)
            {
                // rebuild the filter expression using .Where
                var filteredQuery = source;
                foreach (var filter in queryBasedUri.FilterExpressions)
                {
                    filteredQuery = filteredQuery.Where(filter);
                }

                return filteredQuery;
            }
            else
            {
                // if this uri was not built from a query, then we cannot handle filter expressions
                var filterString = this.currentUri.Filter;
                ExceptionUtilities.Assert(string.IsNullOrEmpty(filterString), "Non-query-based uri had non-empty filter string '{0}' which cannot be processed", filterString);
                return source;
            }
        }
        
        /// <summary>
        /// Processes the $orderby portion of the uri. Will also add special ordering if the response would be paged.
        /// </summary>
        /// <param name="source">The source expression</param>
        /// <returns>The result of processing the orderby</returns>
        private QueryExpression ProcessOrderBy(QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            var queryBasedUri = this.currentUri as QueryBasedODataUri;
            if (queryBasedUri != null && queryBasedUri.OrderByExpressions.Count > 0)
            {
                LinqOrderByExpression orderedQuery = null;

                // rebuild the orderby expression using OrderBy and ThenBy
                foreach (var orderby in queryBasedUri.OrderByExpressions)
                {
                    ExceptionUtilities.Assert(orderby.AreDescending.Count == orderby.KeySelectors.Count, "Orderby expression had mismatched counts");
                    ExceptionUtilities.Assert(orderby.KeySelectors.Count > 0, "Orderby expression did not have any key selectors");

                    // handle the first key selector seperately, since the remainder will need to use ThenBy
                    int startIndex = 0;
                    if (orderedQuery == null)
                    {
                        var ascending = !orderby.AreDescending[0];
                        var lambda = orderby.KeySelectors[0];
                        startIndex = 1;

                        if (ascending)
                        {
                            orderedQuery = source.OrderBy(o => this.RewriteLambda(o, lambda));
                        }
                        else
                        {
                            orderedQuery = source.OrderByDescending(o => this.RewriteLambda(o, lambda));
                        }
                    }

                    // handle the remaining key selectors
                    for (int i = startIndex; i < orderby.AreDescending.Count; i++)
                    {
                        var ascending = !orderby.AreDescending[i];
                        var lambda = orderby.KeySelectors[i];

                        if (ascending)
                        {
                            orderedQuery = orderedQuery.ThenBy(o => this.RewriteLambda(o, lambda));
                        }
                        else
                        {
                            orderedQuery = orderedQuery.ThenByDescending(o => this.RewriteLambda(o, lambda));
                        }
                    }
                }

                source = orderedQuery;
            }
            else
            {
                // if this uri was not built from a query, then we cannot handle orderby expressions
                var orderByString = this.currentUri.OrderBy;
                ExceptionUtilities.Assert(string.IsNullOrEmpty(orderByString), "Non-query-based uri had non-empty orderby string '{0}' which cannot be processed", orderByString);
            }

            // add ordering if the response would be paged
            if (this.IsPaged())
            {
                source = this.AddOrderingForPaging(source);
            }

            return source;
        }

        /// <summary>
        /// Returns whether or not the response for the current uri would be paged, due to either client or server driven paging
        /// </summary>
        /// <returns>A value indicating whether the response would be paged</returns>
        private bool IsPaged()
        {
            return this.currentUri.Skip.HasValue || this.currentUri.Top.HasValue || this.expectedPageSize.HasValue;
        }

        /// <summary>
        /// Adds an ordering expression for each key property 
        /// </summary>
        /// <param name="source">The query to add ordering to</param>
        /// <returns>The result of adding the ordering expressions</returns>
        private LinqOrderByExpression AddOrderingForPaging(QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            var orderByExpression = source as LinqOrderByExpression;

            ExceptionUtilities.CheckObjectNotNull(this.expectedEntitySet, "No entity set expected");

            // the product will sort key properties alphabetically
            foreach (var key in this.expectedEntitySet.EntityType.AllKeyProperties.OrderBy(p => p.Name))
            {
                // if the expression already has other orderby clauses, use thenby
                if (orderByExpression == null)
                {
                    orderByExpression = source.OrderBy(o => o.Property(key.Name));
                }
                else
                {
                    orderByExpression = orderByExpression.ThenBy(o => o.Property(key.Name));
                }
            }

            ExceptionUtilities.CheckObjectNotNull(orderByExpression, "Failed to build orderby expression, entity type had no keys");
            return orderByExpression;
        }

        /// <summary>
        /// Adds an ordering expression for each key property 
        /// </summary>
        /// <param name="queryCollectionValue">QueryCollection Value to sort</param>
        /// <returns>The result of adding the ordering expressions</returns>
        private QueryCollectionValue AddOrderingForPagingToQueryCollectionValue(QueryCollectionValue queryCollectionValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryCollectionValue, "queryCollectionValue");

            ExceptionUtilities.CheckObjectNotNull(this.expectedEntitySet, "No entity set expected");
            var queryEntityType = (QueryEntityType)queryCollectionValue.Type.ElementType;

            var orderedElements = queryCollectionValue.Elements.OfType<QueryStructuralValue>();

            // the product will sort key properties alphabetically
            foreach (var key in queryEntityType.EntityType.AllKeyProperties.OrderBy(p => p.Name))
            {
                orderedElements = orderedElements.OrderBy(o => o.GetScalarValue(key.Name).Value);
            }

            return QueryCollectionValue.Create(queryCollectionValue.Type.ElementType, orderedElements.ToArray());
        }

        /// <summary>
        /// Adds the skip and take expressions to the given expression
        /// </summary>
        /// <param name="source">The source query expression</param>
        /// <returns>The expression with skip and top applied</returns>
        private QueryExpression ProcessSkipAndTop(QueryExpression source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            if (this.currentUri.Skip.HasValue)
            {
                source = source.Skip(this.currentUri.Skip.Value);
            }

            if (this.currentUri.Top.HasValue)
            {
                source = source.Take(this.currentUri.Top.Value);
            }

            if (this.expectedPageSize.HasValue)
            {
                source = source.Take(this.expectedPageSize.Value); 
            }

            return source;
        }

        /// <summary>
        /// Helper method to rewrite lambda expressions with different parameter expressions
        /// </summary>
        /// <param name="newParameter">The parameter to use as a replacement</param>
        /// <param name="lambda">The lambda to rewrite</param>
        /// <returns>The rewritten lambda expression</returns>
        private QueryExpression RewriteLambda(LinqParameterExpression newParameter, LinqLambdaExpression lambda)
        {
            ExceptionUtilities.CheckArgumentNotNull(newParameter, "newParameter");
            ExceptionUtilities.CheckArgumentNotNull(lambda, "lambda");
            var oldParameter = lambda.Parameters.Single();
            return new ParameterReplacingVisitor(oldParameter, newParameter).ReplaceExpression(lambda.Body);
        }

        /// <summary>
        /// Helper method for walking down a query value tree and invoking a callback on all structural values
        /// </summary>
        /// <param name="value">The value to visit</param>
        /// <param name="callback">The callback for structural values</param>
        /// <returns>The result of visiting the value</returns>
        private QueryValue VisitEntityValues(QueryValue value, Func<QueryStructuralValue, QueryStructuralValue> callback)
        {
            ExceptionUtilities.CheckArgumentNotNull(value, "value");
            ExceptionUtilities.CheckArgumentNotNull(callback, "callback");

            var collection = value as QueryCollectionValue;
            if (collection != null && collection.Type.ElementType is QueryEntityType)
            {
                return this.VisitEntityValues(collection, callback);  
            }

            var structural = value as QueryStructuralValue;
            if (structural != null && structural.Type is QueryEntityType && !structural.IsNull)
            {
                return callback(structural);
            }

            return value;
        }

        /// <summary>
        /// Helper method for visiting a collection of values invoking a callback on all structural values
        /// </summary>
        /// <param name="collection">The collection to visit</param>
        /// <param name="callback">The callback for structural values</param>
        /// <returns>The result of visiting the collection</returns>
        private QueryCollectionValue VisitEntityValues(QueryCollectionValue collection, Func<QueryStructuralValue, QueryStructuralValue> callback)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            ExceptionUtilities.CheckArgumentNotNull(callback, "callback");

            var type = collection.Type.ElementType as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(type, "Collection element type was not an entity type. Type was: {0}", collection.Type.ElementType);

            // in general, we want to return a different instance so that the value stored in the query data set are not affected
            return collection.Select(v => this.VisitEntityValues(v, callback)); 
        }
        
        /// <summary>
        /// Fixes up the properties of the structural value based on the given selected paths
        /// </summary>
        /// <param name="instance">The value to fix up</param>
        /// <param name="selectedPaths">The selected paths for the value's scope</param>
        /// <returns>The fixed-up value</returns>
        private QueryStructuralValue FixupPropertiesForSelect(QueryStructuralValue instance, IEnumerable<string> selectedPaths)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckCollectionNotEmpty(selectedPaths, "selectedPaths");

            var entityType = instance.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Select is not supported on non-entity types");

            var masked = MaskedQueryStructuralValue.Create(instance);

            bool wildCard = selectedPaths.Contains(Endpoints.SelectAll);

            var propertyNames = entityType.EntityType.AllProperties.Select(p => p.Name).Concat(entityType.Properties.Streams().Select(m => m.Name));
            foreach (string propertyName in propertyNames)
            {
                if (!selectedPaths.Contains(Uri.EscapeDataString(propertyName)) && !wildCard)
                {
                    // Primitive, bag and stream properties are either entirely present or entirely missing.
                    // However, complex properties can be partially present if a mapped sub-property has a
                    // null value. For this reason, we recursively hide the property and all sub-properties.
                    HidePropertyRecursive(masked, propertyName);
                }
            }

            foreach (string propertyName in entityType.EntityType.AllNavigationProperties.Select(p => p.Name))
            {
                string pathMarker = Uri.EscapeDataString(propertyName) + "/";
                var subpaths = selectedPaths.Where(p => p.StartsWith(pathMarker, StringComparison.Ordinal)).Select(p => p.Substring(pathMarker.Length));

                var value = instance.GetValue(propertyName);
                ExceptionUtilities.CheckObjectNotNull(value, "Value for property '{0}' was null", propertyName);

                if (selectedPaths.Contains(Uri.EscapeDataString(propertyName)))
                {
                    continue;
                }
                else if (subpaths.Any())
                {
                    var maskedValue = this.VisitEntityValues(value, s => this.FixupPropertiesForSelect(s, subpaths));
                    masked.SetValue(propertyName, maskedValue);
                }
                else if (wildCard)
                {
                    masked.SetValue(propertyName, value.Type.NullValue);
                }
                else
                {
                    masked.HideMember(propertyName);
                }
            }

            return masked;
        }

        /// <summary>
        /// Fixes up the properties of the structural value based on the given expanded paths
        /// </summary>
        /// <param name="instance">The value to fix up</param>
        /// <param name="expandedPaths">The expanded paths for the value's scope</param>
        /// <returns>The fixed-up value</returns>
        private QueryStructuralValue FixupPropertiesForExpand(QueryStructuralValue instance, IEnumerable<string> expandedPaths)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckArgumentNotNull(expandedPaths, "expandedPaths");

            var entityType = instance.Type as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Expand is not supported on non-entity types");

            var masked = MaskedQueryStructuralValue.Create(instance);

            foreach (var navigation in entityType.EntityType.AllNavigationProperties)
            {
                string expandedPath;
                string propertyName = navigation.Name;
                var value = instance.GetValue(propertyName);
                
                ExceptionUtilities.CheckObjectNotNull(value, "Value for property '{0}' was null", propertyName);

                bool match = TryGetExpandedPath(expandedPaths, propertyName, entityType.EntityType, out expandedPath);
                string pathMarker = expandedPath + "/";
                var subpaths = expandedPaths.Where(p => p.StartsWith(pathMarker, StringComparison.Ordinal)).Select(p => p.Substring(pathMarker.Length));

                // note that if there is match, we still need to fix up up any children, even if there are no subpaths
                if (subpaths.Any() || match)
                {
                    // recurse
                    var maskedValue = this.VisitEntityValues(value, s => this.FixupPropertiesForExpand(s, subpaths));

                    // handle page limit on expanded collections
                    if (maskedValue.Type is QueryCollectionType)
                    {
                        var collection = maskedValue as QueryCollectionValue;
                        ExceptionUtilities.CheckObjectNotNull(collection, "Value was a collection type, but not a collection value");

                        var relatedSet = entityType.EntitySet.GetRelatedEntitySet(navigation);
                        var relatedSetPageSize = relatedSet.GetEffectivePageSize();
                        if (relatedSetPageSize.HasValue && this.applyPagingInExpands)
                        {
                            collection = this.AddOrderingForPagingToQueryCollectionValue(collection);
                            maskedValue = collection.Take(relatedSetPageSize.Value);
                        }
                    }

                    masked.SetValue(propertyName, maskedValue);
                }
                else if (!match)
                {
                    // set any non-expanded navigation properties to null
                    masked.SetValue(propertyName, value.Type.NullValue);
                }
            }

            return masked;
        }
    }
}
