//---------------------------------------------------------------------
// <copyright file="ClientResponseQueryValueBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Default implementation for evaluating query expression driven by payload.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Need to refactor client verification related classes in the future.")]
    [ImplementationName(typeof(IClientResponseQueryValueBuilder), "Default")]
    public class ClientResponseQueryValueBuilder : IClientResponseQueryValueBuilder
    {
        /// <summary>
        /// current evaluating expression
        /// </summary>
        private QueryExpression currentExpression;

        /// <summary>
        /// Gets or sets log
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets query evaluation strategy
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryEvaluationStrategy EvaluationStrategy { get; set; }

        /// <summary>
        /// Gets or sets strategy selector
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector StrategySelector { get; set; }

        /// <summary>
        /// Gets or sets the initial data set of current workspace.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryDataSet DefaultDataSet { get; set; }

        /// <summary>
        /// Gets or sets the query resolver.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryResolver QueryResolver { get; set; }

        /// <summary>
        /// Gets or sets the query type library.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public QueryTypeLibrary TypeLibrary { get; set; }

        /// <summary>
        /// Gets or sets the entity model of current workspace.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AstoriaWorkspace Workspace { get; set; }

        /// <summary>
        /// Gets or sets the entity model of current workspace.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public EntityModelSchema EntityModel { get; set; }

        /// <summary>
        /// Gets or sets the query repository
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public QueryRepository QueryRepository { get; set; }

        /// <summary>
        /// Gets or sets the PayloadElementToQueryValueConverter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToQueryValueConverter PayloadElementToQueryValueConverter { get; set; }

        /// <summary>
        /// Gets or sets the QueryToODataUriConverter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToODataUriConverter QueryToODataUriConverter { get; set; }

        /// <summary>
        /// Gets or sets the ODataPayloadElementMetadataResolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementMetadataResolver PayloadElementMetadataResolver { get; set; }

        /// <summary>
        /// Gets or sets the evaluator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaExpressionEvaluator Evaluator { get; set; }

        /// <summary>
        /// Gets or sets the client side projection replacer.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientSideProjectionReplacingVisitor ClientSideProjectionReplacer { get; set; }

        /// <summary>
        /// Build QueryValue from query expresion and server response.
        /// </summary>
        /// <param name="expression">The query expresion of client request.</param>
        /// <param name="response">The http response from the server.</param>
        /// <returns>The baseline QueryValue converted from payload.</returns>
        public QueryValue BuildQueryValue(QueryExpression expression, HttpResponseData response)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckArgumentNotNull(response, "response");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            // get type resolver and payload deserializer.
            var typeResolver = new LinqToAstoriaTypeResolutionVisitor(this.TypeLibrary);

            // if the response has an error
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return expression.ExpressionType.CreateErrorValue(new QueryError("Response from server has an error!"));
            }

            string contentType;
            ExceptionUtilities.Assert(response.Headers.TryGetValue(HttpHeaders.ContentType, out contentType), "Cannot get content type from response.");
            var deserializer = this.StrategySelector.GetStrategy(contentType, null).GetDeserializer();
            this.currentExpression = expression;

            var expressionForUri = this.ClientSideProjectionReplacer.ReplaceClientSideProjections(expression);
            ODataUri queryUri = this.QueryToODataUriConverter.ComputeUri(expressionForUri);
            queryUri.Segments.Insert(0, ODataUriBuilder.Root(this.Workspace.ServiceUri));

            // deserialize byte array payload to OData payload element.          
            ODataPayloadElement payload = this.DeserializePayloadData(deserializer, response, queryUri);

            this.PayloadElementMetadataResolver.ResolveMetadata(payload, queryUri);
            var normalizer = this.StrategySelector.GetStrategy(contentType, null).GetPayloadNormalizer();
            payload = normalizer.Normalize(payload);

            if (this.ShouldUsePayloadDrivenVerification(queryUri))
            {
                return this.BuildQueryValueForActionResponse(payload, expression.ExpressionType);
            }
            else
            {
                // build query data set from payload for evaluation. It's a different data set from the initial one of current workspace.
                IQueryDataSet dataSet = this.BuildQueryDataSet(payload);

                // filter the query and resolve types. Need to remove expressions such as $top, $skip, $orderby, $filter because the returned payload is already the result of performing these expressions.
                // need to keep root expression, key expression, $expand and $select to have correct anonymous type.
                var filteredQuery = this.FilterExpression(expression);
                filteredQuery = typeResolver.ResolveTypes(filteredQuery, this.EvaluationStrategy);

                // replace the evaluator's query-data-set with the one generated in the payload
                using (this.Evaluator.WithTemporaryDataSet(dataSet))
                {
                    return this.Evaluator.Evaluate(filteredQuery);
                }
            }
        }

        /// <summary>
        /// Filter the query expression using specific expression visistor.
        /// </summary>
        /// <param name="expression">The initial query expression.</param>
        /// <returns>The filtered query expression.</returns>
        protected QueryExpression FilterExpression(QueryExpression expression)
        {
            return new ClientQueryExpressionFilterVisitor().Filter(expression);
        }

        /// <summary>
        /// Find out whether to use payload for client verification
        /// </summary>
        /// <param name="queryUri">The request uri</param>
        /// <returns>Whether to use payload for client verification</returns>
        private bool ShouldUsePayloadDrivenVerification(ODataUri queryUri)
        {
            if (queryUri.IsAction())
            {
                return true;
            }

            if (queryUri.IsServiceOperation())
            {
                FunctionSegment functionSegment = queryUri.Segments.OfType<FunctionSegment>().Last();
                return !functionSegment.Function.Annotations.OfType<FunctionBodyAnnotation>().Single().IsRoot;
            }

            return false;
        }

        /// <summary> default xml deserialzier for converting byte array payload to ODataPayloadElement
        /// Deserializes payload data and builds ODataPayloadElement.
        /// </summary>
        /// <param name="deserializer">The default xml payload deserializer.</param>
        /// <param name="response">The http response from the server.</param>
        /// <param name="payloadUri">The payload uri</param>
        /// <returns>The ODataPayloadElement converted from the payload.</returns>
        private ODataPayloadElement DeserializePayloadData(IPayloadDeserializer deserializer, HttpResponseData response, ODataUri payloadUri)
        {
            ExceptionUtilities.CheckObjectNotNull(response.Body, "Response has no body content");

            // Only expects HttpStatusCode.OK for normal query tests. Error sceanarios are not included.
            ExceptionUtilities.Assert(response.StatusCode == HttpStatusCode.OK, "Expected status code: OK. Actual: {0}", response.StatusCode);

            try
            {
                string charset;
                if (!response.TryGetMimeCharset(out charset))
                {
                    charset = null;
                }

                string contentType = null;
                if (!response.TryGetHeaderValueIgnoreHeaderCase(HttpHeaders.ContentType, out contentType))
                {
                    contentType = null;
                }
                
                var payloadContext = ODataPayloadContext.BuildODataPayloadContextFromODataUri(payloadUri);
                payloadContext.EncodingName = charset;
                payloadContext.ContentType = contentType;
                return deserializer.DeserializeFromBinary(response.Body, payloadContext);
            }
            catch (Exception ex)
            {
                throw new TaupoInvalidOperationException("Failed to deserialize response body", ex);
            }
        }

        /// <summary>
        /// Build a sub query data set from payload, but having enough information and correct order for evaluation.
        /// </summary>
        /// <param name="payload">The ODataPayloadElement converted from payload.</param>
        /// <returns>A sub query data set containing converted query value.</returns>
        private IQueryDataSet BuildQueryDataSet(ODataPayloadElement payload)
        {
            var dataSet = new QueryDataSet();

            var xmlBaseAnnotations = payload.Annotations.OfType<XmlBaseAnnotation>();

            var entityInstance = payload as EntityInstance;
            if (entityInstance != null)
            {
                // get QueryEntityType for entity instance. Notice that even we are expecting an anonymous type from projection, we still need to find out the 'original' entity type, since we need to build a query data set.
                var elementType = this.currentExpression.ExpressionType as QueryEntityType;
                if (elementType == null)
                {
                    var collectionType = this.currentExpression.ExpressionType as QueryCollectionType;
                    ExceptionUtilities.CheckObjectNotNull(collectionType, "Cannot cast expression type to QueryCollectionType.");

                    elementType = collectionType.ElementType as QueryEntityType;
                    if (elementType == null)
                    {
                        var anonymousType = collectionType.ElementType as QueryAnonymousStructuralType;
                        ExceptionUtilities.CheckArgumentNotNull(anonymousType, "It must be an anonymous type if it is not an entity type.");

                        // TODO: It may have problems if client side's entity types have diferent names with those on server side. 
                        // A solution is implementing another query expression visitor, removing all the $select expression, and using the type from expression.ExpressionType.
                        elementType = this.GetQueryEntityTypeByFullTypeName(entityInstance.FullTypeName);
                    }
                }

                // build query value and store it in query data set.
                var entity = this.BuildFromEntityInstance(entityInstance, elementType, xmlBaseAnnotations);
                var collection = QueryCollectionValue.Create(this.DefaultDataSet[elementType.EntitySet.Name].Type.ElementType, new List<QueryValue>() { entity }, true);
                dataSet.RootQueryData.Add(elementType.EntitySet.Name, collection);
            }
            else
            {
                var entitySetInstance = payload as EntitySetInstance;
                ExceptionUtilities.CheckObjectNotNull(entitySetInstance, "Payload was neither a feed nor entity. Type was: {0}", payload.ElementType);

                var collectionType = this.currentExpression.ExpressionType as QueryCollectionType;
                ExceptionUtilities.CheckObjectNotNull(collectionType, "Cannot cast expression type to QueryCollectionType.");

                var elementType = collectionType.ElementType as QueryEntityType;
                if (elementType == null)
                {
                    var anonymousType = collectionType.ElementType as QueryAnonymousStructuralType;
                    ExceptionUtilities.CheckArgumentNotNull(anonymousType, "It must be an anonymous type if it is not an entity type.");

                    var title = entitySetInstance.Annotations.OfType<TitleAnnotation>().SingleOrDefault();
                    ExceptionUtilities.CheckObjectNotNull(title, "Cannot find title information from entity set instance.");

                    ExceptionUtilities.CheckObjectNotNull(title.Value, "Cannot get title value from entity set instance.");
                    QueryStructuralType rootDataType;
                    this.QueryRepository.RootDataTypes.TryGetValue(title.Value, out rootDataType);
                    if (rootDataType == null)
                    {
                        this.RootDataTypesDerivedTypes().TryGetValue(title.Value, out rootDataType);
                    }

                    ExceptionUtilities.CheckObjectNotNull(rootDataType, "Cannot find title value '{0}' in root datatypes", title.Value);
                    elementType = rootDataType as QueryEntityType;
                    ExceptionUtilities.CheckObjectNotNull(elementType, "Cannot find element type for entity set '{0}'.", title.Value);
                }

                var collection = this.BuildFromEntitySetInstance(entitySetInstance, elementType, xmlBaseAnnotations);
                dataSet.RootQueryData.Add(elementType.EntitySet.Name, collection);
            }

            return dataSet;
        }

        private IDictionary<string, QueryStructuralType> RootDataTypesDerivedTypes()
        {
            Dictionary<string, QueryStructuralType> types = new Dictionary<string, QueryStructuralType>();
            foreach (QueryStructuralType rootType in this.QueryRepository.RootDataTypes.Values.Where(t => t.DerivedTypes.Any()))
            {
                foreach (QueryEntityType derivedType in rootType.DerivedTypes.OfType<QueryEntityType>())
                {
                    types.Add(derivedType.EntityType.FullName, derivedType);
                }
            }

            return types;
        }

        /// <summary>
        /// Gets corresponding QueryEntityType for a given full type name.
        /// </summary>
        /// <param name="typeName">The string format of full type name.</param>
        /// <returns>The corresponding QueryEntityType. It might be the base type of the actual query entity type and this scenario is handled later.</returns>
        private QueryEntityType GetQueryEntityTypeByFullTypeName(string typeName)
        {
            var entityType = this.EntityModel.EntityTypes.SingleOrDefault(t => t.FullName == typeName);
            ExceptionUtilities.CheckObjectNotNull(entityType, "Cannot find entity type with name {0}.", typeName);

            var entitySet = this.EntityModel.EntityContainers.SelectMany(c => c.EntitySets).SingleOrDefault(s => entityType.GetBaseTypesAndSelf().Contains(s.EntityType));
            ExceptionUtilities.CheckObjectNotNull(entitySet, "Cannot find entity set which holds type {0}.", typeName);

            var elementType = this.DefaultDataSet[entitySet.Name].Type.ElementType as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(elementType, "Cannot find query entity type associated with entity set {0}.", entitySet.Name);

            return elementType;
        }

        /// <summary>
        /// Builds an instance of QueryStructureValue from an entity instance.
        /// </summary>
        /// <param name="instance">The entity instance</param>
        /// <param name="type">The QueryEntityType of the entity</param>
        /// <param name="xmlBaseAnnotations">The xml base annotations from parent elements, if any</param>
        /// <returns>The converted structural value of entity instance.</returns>
        private QueryStructuralValue BuildFromEntityInstance(EntityInstance instance, QueryEntityType type, IEnumerable<XmlBaseAnnotation> xmlBaseAnnotations)
        {
            QueryStructuralValue entity;
            
            // Helpful for debugging purposes to understand when things fail, on which entity they fail on
            this.Logger.WriteLine(LogLevel.Trace, "Build From Entity Instance with Id: {0}", instance.Id);

            // handle MEST.
            var queryEntityType = type;
            if (type.EntityType.FullName != instance.FullTypeName)
            {
                queryEntityType = type.DerivedTypes.Cast<QueryEntityType>().SingleOrDefault(t => t.EntityType.FullName == instance.FullTypeName);
                ExceptionUtilities.CheckObjectNotNull(queryEntityType, "Cannot find query entity type for entity type {0}.", instance.FullTypeName);
            }

            if (!instance.Annotations.OfType<DataTypeAnnotation>().Any())
            {
                instance.Annotations.Add(new DataTypeAnnotation() { DataType = DataTypes.EntityType.WithDefinition(queryEntityType.EntityType) });
            }

            // remove navigation properties so that they can be added after
            var navigationProperties = instance.Properties.OfType<NavigationPropertyInstance>().ToList();
            navigationProperties.ForEach(n => instance.Remove(n));

            // convert to query value
            entity = (QueryStructuralValue)this.PayloadElementToQueryValueConverter.Convert(instance, queryEntityType);
            
            // add expanded navigation properties.
            this.AddNavigationProperties(entity, navigationProperties.Where(p => p.IsExpanded), xmlBaseAnnotations);

            // add stream properties
            this.AddStreamProperties(entity, instance, xmlBaseAnnotations);
            
            return entity;
        }

        /// <summary>
        /// Builds an instance of QueryCollectionValue from an entity set instance.
        /// </summary>
        /// <param name="entitySetInstance">The entity set instance.</param>
        /// <param name="elementType">The QueryEntityType of its element.</param>
        /// <param name="xmlBaseAnnotations">The xml base annotations from parent elements, if any</param>
        /// <returns>The converted QueryCollectionValue of entity set instance.</returns>
        private QueryCollectionValue BuildFromEntitySetInstance(EntitySetInstance entitySetInstance, QueryEntityType elementType, IEnumerable<XmlBaseAnnotation> xmlBaseAnnotations)
        {
            ExceptionUtilities.CheckArgumentNotNull(elementType, "elementType");
            var entities = new List<QueryStructuralValue>();

            foreach (var entityInstance in entitySetInstance)
            {
                var value = this.BuildFromEntityInstance(entityInstance, elementType, xmlBaseAnnotations.Concat(entityInstance.Annotations.OfType<XmlBaseAnnotation>()));
                entities.Add(value);
            }

            return QueryCollectionValue.Create(elementType, entities.ToArray());
        }

        /// <summary>
        /// Adds navigational properties to structural value.
        /// </summary>
        /// <param name="entity">he target structural value.</param>
        /// <param name="properties">The expanded navigation properties.</param>
        /// <param name="xmlBaseAnnotations">The xml base annotations ancestors and local element</param>
        private void AddNavigationProperties(QueryStructuralValue entity, IEnumerable<NavigationPropertyInstance> properties, IEnumerable<XmlBaseAnnotation> xmlBaseAnnotations)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            foreach (var navigation in properties)
            {
                ExceptionUtilities.Assert(navigation.IsExpanded, "Do not handle deferred link in building query values.");

                // Get query entity type for the other end.
                var queryEntityType = entity.Type as QueryEntityType;
                var property = queryEntityType.EntityType.AllNavigationProperties.Single(p => p.Name == navigation.Name);
                var otherEndType = queryEntityType.GetRelatedEntityType(property.Name);

                var expandedLink = navigation.Value as ExpandedLink;
                if (expandedLink != null)
                {
                    if (expandedLink.ExpandedElement != null)
                    {
                        if (property.ToAssociationEnd.Multiplicity == EndMultiplicity.Many)
                        {
                            // handle collection navigation property.
                            var entitySetInstance = expandedLink.ExpandedElement as EntitySetInstance;
                            ExceptionUtilities.CheckObjectNotNull(entitySetInstance, "Cannot cast payload element to EntitySetInstance.");

                            var collection = this.BuildFromEntitySetInstance(entitySetInstance, otherEndType, xmlBaseAnnotations);
                            entity.SetValue(navigation.Name, collection);
                        }
                        else
                        {
                            // handle reference navigation property.
                            var entityInstance = expandedLink.ExpandedElement as EntityInstance;
                            ExceptionUtilities.CheckObjectNotNull(entityInstance, "Cannot cast payload element to EntityInstance.");

                            var instance = this.BuildFromEntityInstance(entityInstance, otherEndType, xmlBaseAnnotations);
                            entity.SetValue(navigation.Name, instance);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds stream properties to structural value.
        /// </summary>
        /// <param name="entity">The target structural value.</param>
        /// <param name="payload">The payload with the entity values.</param>
        /// <param name="xmlBaseAnnotations">The xml base annotations ancestors and local element</param>
        private void AddStreamProperties(QueryStructuralValue entity, EntityInstance payload, IEnumerable<XmlBaseAnnotation> xmlBaseAnnotations)
        {
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");

            var xmlBaseAnnotationsList = xmlBaseAnnotations.ToList();

            if (payload.IsMediaLinkEntry())
            {
                string editLink = this.NormalizeLinkFromPayload(payload.StreamEditLink, xmlBaseAnnotationsList);
                string selfLink = this.NormalizeLinkFromPayload(payload.StreamSourceLink, xmlBaseAnnotationsList);

                entity.SetStreamValue(null, payload.StreamContentType, payload.StreamETag, editLink, selfLink, new byte[0]);
            }

            foreach (var stream in payload.Properties.OfType<NamedStreamInstance>())
            {
                string contentType = stream.SourceLinkContentType;
                if (contentType == null)
                {
                    contentType = stream.EditLinkContentType;
                }

                string editLink = this.NormalizeLinkFromPayload(stream.EditLink, xmlBaseAnnotationsList);
                string selfLink = this.NormalizeLinkFromPayload(stream.SourceLink, xmlBaseAnnotationsList);

                entity.SetStreamValue(stream.Name, contentType, stream.ETag, editLink, selfLink, new byte[0]);
            }
        }

        private string NormalizeLinkFromPayload(string link, IEnumerable<XmlBaseAnnotation> xmlBaseAnnotations)
        {
            if (link == null)
            {
                return null;
            }

            var linkUri = new Uri(link, UriKind.RelativeOrAbsolute);
            if (!linkUri.IsAbsoluteUri)
            {
                linkUri = new Uri(UriHelpers.ConcatenateUriSegments(xmlBaseAnnotations.Select(b => b.Value).Concat(link).ToArray()), UriKind.RelativeOrAbsolute);
            }

            return linkUri.OriginalString;
        }

        /// <summary>
        /// Build QueryValue from action response payload
        /// </summary>
        /// <param name="payload">response payload element</param>
        /// <param name="queryType">query type to build</param>
        /// <returns>query value that represents the payload</returns>
        private QueryValue BuildQueryValueForActionResponse(ODataPayloadElement payload, QueryType queryType)
        {
            EntitySetInstance entitySetInstance = payload as EntitySetInstance;
            PrimitiveProperty primitiveProperty = payload as PrimitiveProperty;
            ComplexProperty complexProperty = payload as ComplexProperty;
            PrimitiveMultiValueProperty primitiveMultiValueProperty = payload as PrimitiveMultiValueProperty;
            ComplexMultiValueProperty complexMultiValueProperty = payload as ComplexMultiValueProperty;
            PrimitiveCollection primitiveCollection = payload as PrimitiveCollection;
            ComplexInstanceCollection complexInstanceCollection = payload as ComplexInstanceCollection;
            if (entitySetInstance != null)
            {
                var xmlBaseAnnotations = payload.Annotations.OfType<XmlBaseAnnotation>();
                var collectionType = this.currentExpression.ExpressionType as QueryCollectionType;
                ExceptionUtilities.CheckObjectNotNull(collectionType, "Cannot cast expression type to QueryCollectionType.");
                var elementType = collectionType.ElementType as QueryEntityType;
                return this.BuildFromEntitySetInstance(entitySetInstance, elementType, xmlBaseAnnotations);
            }
            else if (primitiveProperty != null)
            {
                return this.PayloadElementToQueryValueConverter.Convert(primitiveProperty.Value, queryType);
            }
            else if (complexProperty != null)
            {
                return this.PayloadElementToQueryValueConverter.Convert(complexProperty.Value, queryType);
            }
            else if (primitiveMultiValueProperty != null)
            {
                return this.PayloadElementToQueryValueConverter.Convert(primitiveMultiValueProperty.Value, queryType);
            }
            else if (complexMultiValueProperty != null)
            {
                return this.PayloadElementToQueryValueConverter.Convert(complexMultiValueProperty.Value, queryType);
            }
            else if (primitiveCollection != null)
            {
                return this.PayloadElementToQueryValueConverter.Convert(primitiveCollection, queryType);
            }
            else if (complexInstanceCollection != null)
            {
                return this.PayloadElementToQueryValueConverter.Convert(complexInstanceCollection, queryType);
            }
            else
            {
                ExceptionUtilities.CheckArgumentNotNull(payload as EntityInstance, "Unexpected response payload type: " + payload.ElementType + ".");
                return this.PayloadElementToQueryValueConverter.Convert(payload, queryType);
            }
        }

        /// <summary>
        /// Filters query expresssions to be applied after building initial data set from payload. 
        /// </summary>
        private class ClientQueryExpressionFilterVisitor : LinqToAstoriaExpressionReplacingVisitor, ILinqToAstoriaExpressionVisitor<QueryExpression>
        {
            /// <summary>
            /// Filters the specified expression. 
            /// </summary>
            /// <param name="expression">The expression to filter.</param>
            /// <returns>The filtered expression</returns>
            public QueryExpression Filter(QueryExpression expression)
            {
                return expression.Accept(this);
            }

            /// <summary>
            /// Filters the specified expression. Skip orderby expression.
            /// </summary>
            /// <param name="expression">The expression to filter.</param>
            /// <returns>The filtered expression</returns>
            public override QueryExpression Visit(LinqOrderByExpression expression)
            {
                return expression.Source.Accept(this);
            }

            /// <summary>
            /// Filters the specified expression. Skip skip expression.
            /// </summary>
            /// <param name="expression">The expression to filter.</param>
            /// <returns>The filtered expression</returns>
            public override QueryExpression Visit(LinqSkipExpression expression)
            {
                return expression.Source.Accept(this);
            }

            /// <summary>
            /// Filters the specified expression. Skip top expression.
            /// </summary>
            /// <param name="expression">The expression to filter.</param>
            /// <returns>The filtered expression</returns>
            public override QueryExpression Visit(LinqTakeExpression expression)
            {
                return expression.Source.Accept(this);
            }

            /// <summary>
            /// Filters the specified expression. Skip where expression.
            /// </summary>
            /// <param name="expression">The expression to filter.</param>
            /// <returns>The filtered expression</returns>
            public override QueryExpression Visit(LinqWhereExpression expression)
            {
                return expression.Source.Accept(this);
            }
        }
    }
}
