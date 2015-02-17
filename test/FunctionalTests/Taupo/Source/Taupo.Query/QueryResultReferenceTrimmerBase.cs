//---------------------------------------------------------------------
// <copyright file="QueryResultReferenceTrimmerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Building blocks for classes that would trim query result references in EntityFramework and Astoria.
    /// </summary>
    /// <remarks>
    /// Note that we keep the state from previously trimmed queries, to mimic the behavior of ObjectStateManager.
    /// Process has beed divided into several steps:
    /// 1. Extracting all new entities that should be materialized in this query (and adding them to the structure resembling OSM)
    /// 2. Fixup of references based on span paths (including Many-Many) 
    /// Note: steps 1 and 2 could probably be combined, but kept them separate so that it's more clear whats going on.
    /// 3. Fixup 1-1 and 1-Many references between all entities in the OSM-structure. 
    /// Note: We always have to go through entire set of entities, not only ones that have been brought up in this current query. 
    /// This is because some of the navigations are only one way.
    /// 4. Create a result by taking a structure of the initial QueryValue, and replacing all entities with the ones from our OSM structure 
    /// 5. Copying the result to keep the current state of OSM. 
    /// Note: If first query materializes Customers, and second query materializes Orders, we should not know about Orders when we verify the first query.
    /// However, because first we generate all the baselines, without performing the copy, all the baselines would have full relationship graph, 
    /// which would be incorrect.
    /// .....
    /// VisibleEntitiesGraph is a structure that represents current state of ObjectStateManager. Keys are original values we got from Evaluator
    /// (which come from QueryDataSet and have complete relationship graph), values are entities that are visible currently
    /// with relationship graph only to themselves. From these entities we construct final result.
    /// </remarks>
    public abstract class QueryResultReferenceTrimmerBase
    {
        /// <summary>
        /// Initializes a new instance of the QueryResultReferenceTrimmerBase class.
        /// </summary>
        protected QueryResultReferenceTrimmerBase()
        {
            this.VisibleEntitiesGraph = new Dictionary<QueryStructuralValue, QueryStructuralValue>();
        }

        /// <summary>
        /// Gets the graph of currently visible entities.
        /// </summary>
        /// <remarks>This is exposed mostly as testability hook.</remarks>
        protected Dictionary<QueryStructuralValue, QueryStructuralValue> VisibleEntitiesGraph { get; private set; }

        /// <summary>
        /// Adds entities that are materialized in the given QueryValue into list of currently visible entities.
        /// </summary>
        /// <param name="value">QueryValue representing result of a query.</param>
        /// <param name="spanPaths">Span paths for the query.</param>
        protected void UpdateVisibleEntitiesGraph(QueryValue value, string[] spanPaths)
        {
            this.UpdateVisibleEntitiesGraphForQueryValue(value, string.Empty, spanPaths);
        }

        /// <summary>
        /// Fixes up references added via Full span.
        /// </summary>
        /// <param name="value">QueryValue representing result of a query.</param>
        /// <param name="spanPaths">Span paths for the query.</param>
        protected void FixupReferencesBasedOnSpanPaths(QueryValue value, string[] spanPaths)
        {
            this.FixupReferencesBasedOnSpanPaths(value, null, null, string.Empty, spanPaths);
        }

        /// <summary>
        /// Fixes up references between entities in the visible entities graph.
        /// </summary>
        protected void FixupReferencesOfVisibleEntities()
        {
            foreach (var entry in this.VisibleEntitiesGraph)
            {
                QueryStructuralValue originalEntity = entry.Key;
                QueryStructuralValue visibleEntity = entry.Value;

                if (!visibleEntity.IsNull)
                {
                    this.FixupReferencesOfVisibleEntity(originalEntity, visibleEntity);
                }
            }
        }

        /// <summary>
        /// Creates a result based on currently visible entities.
        /// </summary>
        /// <param name="queryResult">Initial query result, containing full relationship graph.</param>
        /// <returns>Query result that has the same shape as the given, but with relationship graph trimmed only to visible entities.</returns>
        protected QueryValue CreateResultOnlyWithVisibleEntities(QueryValue queryResult)
        {
            var resultCreatingVisitor = new ResultOnlyWithSpannedEntitiesCreatingVisitor(this.VisibleEntitiesGraph);
            var result = resultCreatingVisitor.ProcessResult(queryResult);

            return result;
        }

        private void UpdateVisibleEntitiesGraphForQueryValue(QueryValue value, string path, string[] spanPaths)
        {
            var queryStructuralValue = value as QueryStructuralValue;
            var queryCollectionValue = value as QueryCollectionValue;
            var queryRecordValue = value as QueryRecordValue;

            if (queryStructuralValue != null)
            {
                if (queryStructuralValue.Type is QueryEntityType)
                {
                    this.UpdateVisibleEntitiesGraphForEntity(queryStructuralValue, path, spanPaths);
                }
                else if (queryStructuralValue.Type is QueryAnonymousStructuralType || queryStructuralValue.Type is QueryGroupingType)
                {
                    this.UpdateVisibleEntitiesGraphForAnonymousAndGrouping(queryStructuralValue, spanPaths);
                }
            }
            else if (queryCollectionValue != null)
            {
                this.UpdateVisibleEntitiesGraphForCollection(queryCollectionValue, path, spanPaths);
            }
            else if (queryRecordValue != null)
            {
                this.UpdateVisibleEntitiesGraphForRecord(queryRecordValue, spanPaths);
            }
        }

        private void UpdateVisibleEntitiesGraphForEntity(QueryStructuralValue entityValue, string path, string[] spanPaths)
        {
            if (!this.VisibleEntitiesGraph.ContainsKey(entityValue))
            {
                if (entityValue.IsNull)
                {
                    this.VisibleEntitiesGraph[entityValue] = entityValue;

                    return;
                }
                else
                {
                    var visibleEntity = entityValue.Type.CreateNewInstance();
                    this.PopulateNonNavigationPropertiesOfVisibleEntity(entityValue, visibleEntity);
                    this.VisibleEntitiesGraph[entityValue] = visibleEntity;
                }
            }

            if (!entityValue.IsNull)
            {
                foreach (QueryProperty property in entityValue.Type.Properties)
                {
                    string newPath = this.CreateNewPath(path, property.Name);
                    if (this.IsValidSpanPath(newPath, spanPaths))
                    {
                        var originalValue = entityValue.GetValue(property.Name);
                        this.UpdateVisibleEntitiesGraphForQueryValue(originalValue, newPath, spanPaths);
                    }
                }
            }
        }

        private void UpdateVisibleEntitiesGraphForCollection(QueryCollectionValue collectionValue, string path, string[] spanPaths)
        {
            if (!collectionValue.IsNull)
            {
                foreach (var element in collectionValue.Elements)
                {
                    this.UpdateVisibleEntitiesGraphForQueryValue(element, path, spanPaths);
                }
            }
        }

        private void UpdateVisibleEntitiesGraphForAnonymousAndGrouping(QueryStructuralValue anonymousOrGroupingValue, string[] spanPaths)
        {
            // we never take Includes into account for anonymous and grouping properties, hence we can terminate/corrupt the current span path
            string newPath = "[anonymous or grouping]";
            if (!anonymousOrGroupingValue.IsNull)
            {
                foreach (QueryProperty property in anonymousOrGroupingValue.Type.Properties)
                {
                    var originalValue = anonymousOrGroupingValue.GetValue(property.Name);
                    this.UpdateVisibleEntitiesGraphForQueryValue(originalValue, newPath, spanPaths);
                }
            }
        }

        private void UpdateVisibleEntitiesGraphForRecord(QueryRecordValue recordValue, string[] spanPaths)
        {
            // we never take Includes into account for record properties, hence we can terminate/corrupt the current span path
            string newPath = "[record]";
            if (!recordValue.IsNull)
            {
                for (int i = 0; i < recordValue.Type.Properties.Count; i++)
                {
                    var originalValue = recordValue.GetMemberValue(i);
                    this.UpdateVisibleEntitiesGraphForQueryValue(originalValue, newPath, spanPaths);
                }
            }
        }

        private string CreateNewPath(string currentPath, string memberName)
        {
            return string.IsNullOrEmpty(currentPath) ? memberName : currentPath + "." + memberName;
        }

        private void PopulateNonNavigationPropertiesOfVisibleEntity(QueryStructuralValue originalEntity, QueryStructuralValue visibleEntity)
        {
            foreach (var property in originalEntity.Type.Properties)
            {
                bool isCollectionOfNonEntities = property.PropertyType is QueryCollectionType && !(((QueryCollectionType)property.PropertyType).ElementType is QueryEntityType);
                if (property.PropertyType is QueryScalarType || property.PropertyType is QueryComplexType || isCollectionOfNonEntities)
                {
                    visibleEntity.SetValue(property.Name, originalEntity.GetValue(property.Name));
                }
            }
        }

        private void FixupReferencesBasedOnSpanPaths(QueryValue value, QueryStructuralValue parent, NavigationProperty navigationProperty, string path, string[] spanPaths)
        {
            if (value.IsNull)
            {
                return;
            }

            var collectionValue = value as QueryCollectionValue;
            var structuralValue = value as QueryStructuralValue;
            var entityType = value.Type as QueryEntityType;

            if (collectionValue != null && collectionValue.Type.ElementType is QueryEntityType)
            {
                foreach (var element in collectionValue.Elements)
                {
                    this.FixupReferencesBasedOnSpanPaths(element, parent, navigationProperty, path, spanPaths);
                }
            }

            if (structuralValue != null && entityType != null)
            {
                if (parent != null)
                {
                    var visibleParent = this.VisibleEntitiesGraph[parent];
                    var visibleChild = this.VisibleEntitiesGraph[structuralValue];
                    this.FixupBothEndsOfNavigationProperty(visibleParent, visibleChild, navigationProperty);
                }

                foreach (var property in structuralValue.Type.Properties.Where(p => p.PropertyType is QueryEntityType || p.PropertyType is QueryCollectionType))
                {
                    var newPath = this.CreateNewPath(path, property.Name);
                    if (this.IsValidSpanPath(newPath, spanPaths))
                    {
                        var propertyValue = structuralValue.GetValue(property.Name);
                        var navigationProp = entityType.EntityType.AllNavigationProperties.Where(p => p.Name == property.Name).Single();
                        this.FixupReferencesBasedOnSpanPaths(propertyValue, structuralValue, navigationProp, newPath, spanPaths);
                    }
                }
            }
        }

        private bool IsValidSpanPath(string path, string[] spanPaths)
        {
            foreach (var spanPath in spanPaths)
            {
                if (path == spanPath)
                {
                    return true;
                }

                if (spanPath.StartsWith(path, StringComparison.Ordinal) && spanPath[path.Length] == '.')
                {
                    return true;
                }
            }

            return false;
        }

        private void FixupReferencesOfVisibleEntity(QueryStructuralValue originalEntity, QueryStructuralValue visibleEntity)
        {
            foreach (var property in originalEntity.Type.Properties)
            {
                var entityProperty = property.PropertyType as QueryEntityType;
                if (entityProperty != null)
                {
                    var originalProperty = originalEntity.GetStructuralValue(property.Name);
                    QueryStructuralValue fixedUpProperty;
                    if (!this.VisibleEntitiesGraph.TryGetValue(originalProperty, out fixedUpProperty))
                    {
                        fixedUpProperty = entityProperty.NullValue;
                    }

                    var navigationProperty = ((QueryEntityType)visibleEntity.Type).EntityType.AllNavigationProperties.Where(p => p.Name == property.Name).SingleOrDefault();
                    ExceptionUtilities.CheckObjectNotNull(navigationProperty, "Could not find navigation property '" + property.Name + "' on entity.");

                    this.FixupBothEndsOfNavigationProperty(visibleEntity, fixedUpProperty, navigationProperty);
                }
            }
        }

        private void FixupBothEndsOfNavigationProperty(QueryStructuralValue fromValue, QueryStructuralValue toValue, NavigationProperty navigationProperty)
        {
            this.FixupNavigationProperty(fromValue, toValue, navigationProperty.Name);
            var otherSideNavigation = navigationProperty.ToAssociationEnd.EntityType.AllNavigationProperties.Where(p => p.Association == navigationProperty.Association && p.FromAssociationEnd == navigationProperty.ToAssociationEnd && p.ToAssociationEnd == navigationProperty.FromAssociationEnd).SingleOrDefault();
            if (otherSideNavigation != null && !toValue.IsNull)
            {
                this.FixupNavigationProperty(toValue, fromValue, otherSideNavigation.Name);
            }
        }

        private void FixupNavigationProperty(QueryStructuralValue fromValue, QueryStructuralValue toValue, string propertyName)
        {
            var propertyType = fromValue.Type.Properties.Where(p => p.Name == propertyName).Single().PropertyType;
            if (propertyType is QueryEntityType)
            {
                fromValue.SetValue(propertyName, toValue);
            }
            else
            {
                ExceptionUtilities.Assert(propertyType is QueryCollectionType, "Expecting collection type.");
                var currentCollection = fromValue.GetCollectionValue(propertyName);
                if (!currentCollection.Elements.Contains(toValue))
                {
                    currentCollection.Elements.Add(toValue);
                }

                fromValue.SetValue(propertyName, currentCollection);
            }
        }

        private class ResultOnlyWithSpannedEntitiesCreatingVisitor : IQueryValueVisitor<QueryValue>
        {
            private readonly Dictionary<QueryStructuralValue, QueryStructuralValue> visibleEntitiesGraph;

            /// <summary>
            /// Initializes a new instance of the ResultOnlyWithSpannedEntitiesCreatingVisitor class.
            /// </summary>
            /// <param name="visibleEntitiesGraph">Structure describing all entities that are visible at the moment.</param>
            public ResultOnlyWithSpannedEntitiesCreatingVisitor(Dictionary<QueryStructuralValue, QueryStructuralValue> visibleEntitiesGraph)
            {
                this.visibleEntitiesGraph = visibleEntitiesGraph;
            }

            /// <summary>
            /// Replaces entities with ones that only have appropriate references (i.e. only to entities that are currently visible)
            /// </summary>
            /// <param name="value">QueryValue to process.</param>
            /// <returns>Processed QueryValue.</returns>
            public QueryValue ProcessResult(QueryValue value)
            {
                if (value.IsNull)
                {
                    return value;
                }
                else
                {
                    return value.Accept(this);
                }
            }

            /// <summary>
            /// Visits the given QueryValue
            /// </summary>
            /// <param name="value">QueryValue being visited.</param>
            /// <returns>The result of visiting this QueryValue.</returns>
            public QueryValue Visit(QueryCollectionValue value)
            {
                var resultElements = new List<QueryValue>();

                // special case the collection of entities, we want to avoid adding entities that are not visible, rather than returning them as nulls.
                var entityElementType = value.Type.ElementType as QueryEntityType;
                if (entityElementType != null)
                {
                    foreach (var entityElement in value.Elements.Cast<QueryStructuralValue>())
                    {
                        QueryStructuralValue resultEntityElement;
                        if (this.visibleEntitiesGraph.TryGetValue(entityElement, out resultEntityElement))
                        {
                            resultElements.Add(resultEntityElement);
                        }
                    }
                }
                else
                {
                    foreach (var element in value.Elements)
                    {
                        var resultElement = this.ProcessResult(element);
                        resultElements.Add(resultElement);
                    }
                }

                return value.Type.CreateCollectionWithValues(resultElements);
            }

            /// <summary>
            /// Visits the given QueryValue
            /// </summary>
            /// <param name="value">QueryValue being visited.</param>
            /// <returns>The result of visiting this QueryValue.</returns>
            public QueryValue Visit(QueryRecordValue value)
            {
                var result = value.Type.CreateNewInstance();
                for (int i = 0; i < value.Type.Properties.Count; i++)
                {
                    var propertyValue = value.GetMemberValue(i);
                    var resultPropertyValue = this.ProcessResult(propertyValue);
                    result.SetMemberValue(i, resultPropertyValue);
                }

                return result;
            }

            /// <summary>
            /// Visits the given QueryValue
            /// </summary>
            /// <param name="value">QueryValue being visited.</param>
            /// <returns>The result of visiting this QueryValue.</returns>
            public QueryValue Visit(QueryReferenceValue value)
            {
                return value;
            }

            /// <summary>
            /// Visits the given QueryValue
            /// </summary>
            /// <param name="value">QueryValue being visited.</param>
            /// <returns>The result of visiting this QueryValue.</returns>
            public QueryValue Visit(QueryScalarValue value)
            {
                return value;
            }

            /// <summary>
            /// Visits the given QueryValue
            /// </summary>
            /// <param name="value">QueryValue being visited.</param>
            /// <returns>The result of visiting this QueryValue.</returns>
            public QueryValue Visit(QueryStructuralValue value)
            {
                if (value.Type is QueryComplexType)
                {
                    return value;
                }

                if (value.Type is QueryEntityType)
                {
                    return this.visibleEntitiesGraph[value];
                }

                var result = value.Type.CreateNewInstance();
                foreach (var property in value.Type.Properties)
                {
                    var propertyValue = value.GetValue(property.Name);
                    var resultPropertyValue = this.ProcessResult(propertyValue);
                    result.SetValue(property.Name, resultPropertyValue);
                }

                return result;
            }
        }
    }
}