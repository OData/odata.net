//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData.Query
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Globalization;
    #endregion Namespaces.

    /// <summary>
    /// Binder which applies metadata to a lexical QueryToken tree and produces a bound semantic QueryNode tree.
    /// </summary>
#if INTERNAL_DROP
    internal class MetadataBinder
#else
    public class MetadataBinder
#endif
    {
        /// <summary>
        /// The metadata provider used for binding.
        /// </summary>
        private DataServiceMetadataProviderWrapper metadataProvider;

        /// <summary>
        /// Collection of query option tokens associated with the currect query being processed.
        /// If a given query option is bound it should be removed from this collection.
        /// </summary>
        private List<QueryOptionQueryToken> queryOptions;

        /// <summary>
        /// If the binder is binding an expression, like $filter or $orderby, this member holds
        /// the reference to the parameter node for the expression.
        /// </summary>
        private ParameterQueryNode parameter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="metadataProvider">The metadata provider to use for binding.</param>
        public MetadataBinder(IDataServiceMetadataProvider metadataProvider)
        {
            ExceptionUtils.CheckArgumentNotNull(metadataProvider, "metadataProvider");

            this.metadataProvider = new DataServiceMetadataProviderWrapper(metadataProvider);
        }

        /// <summary>
        /// Binds the specified <paramref name="queryDescriptorQueryToken"/> to the metadata and returns a bound query.
        /// </summary>
        /// <param name="queryDescriptorQueryToken">The lexical query descriptor for the query to process.</param>
        /// <returns>Metadata bound semantic query descriptor for the query.</returns>
        public QueryDescriptorQueryNode BindQuery(QueryDescriptorQueryToken queryDescriptorQueryToken)
        {
            ExceptionUtils.CheckArgumentNotNull(queryDescriptorQueryToken, "queryDescriptorQueryToken");

            return (QueryDescriptorQueryNode)this.Bind(queryDescriptorQueryToken);
        }

        /// <summary>
        /// Visits a <see cref="QueryToken"/> in the lexical tree and binds it to metadata producing a semantic <see cref="QueryNode"/>.
        /// </summary>
        /// <param name="token">The query token on the input.</param>
        /// <returns>The bound query node output.</returns>
        protected QueryNode Bind(QueryToken token)
        {
            ExceptionUtils.CheckArgumentNotNull(token, "token");

            QueryNode result;
            switch (token.Kind)
            {
                case QueryTokenKind.Extension:
                    result = this.BindExtension(token);
                    break;
                case QueryTokenKind.QueryDescriptor:
                    result = this.BindQueryDescriptor((QueryDescriptorQueryToken)token);
                    break;
                case QueryTokenKind.Segment:
                    result = this.BindSegment((SegmentQueryToken)token);
                    break;
                case QueryTokenKind.Literal:
                    result = this.BindLiteral((LiteralQueryToken)token);
                    break;
                case QueryTokenKind.BinaryOperator:
                    result = this.BindBinaryOperator((BinaryOperatorQueryToken)token);
                    break;
                case QueryTokenKind.UnaryOperator:
                    result = this.BindUnaryOperator((UnaryOperatorQueryToken)token);
                    break;
                case QueryTokenKind.PropertyAccess:
                    result = this.BindPropertyAccess((PropertyAccessQueryToken)token);
                    break;
                case QueryTokenKind.FunctionCall:
                    result = this.BindFunctionCall((FunctionCallQueryToken)token);
                    break;
                case QueryTokenKind.QueryOption:
                    result = this.BindQueryOption((QueryOptionQueryToken)token);
                    break;
                default:
                    throw new ODataException(Strings.MetadataBinder_UnsupportedQueryTokenKind(token.Kind));
            }

            if (result == null)
            {
                throw new ODataException(Strings.MetadataBinder_BoundNodeCannotBeNull(token.Kind));
            }

            return result;
        }

        /// <summary>
        /// Binds an extension token.
        /// </summary>
        /// <param name="extensionToken">The extension token to bind.</param>
        /// <returns>The bound query node.</returns>
        protected virtual QueryNode BindExtension(QueryToken extensionToken)
        {
            ExceptionUtils.CheckArgumentNotNull(extensionToken, "extensionToken");

            throw new ODataException(Strings.MetadataBinder_UnsupportedExtensionToken);
        }

        /// <summary>
        /// Binds a <see cref="QueryDescriptorQueryToken"/>.
        /// </summary>
        /// <param name="queryDescriptorToken">The query descriptor token to bind.</param>
        /// <returns>The bound query descriptor.</returns>
        protected virtual QueryDescriptorQueryNode BindQueryDescriptor(QueryDescriptorQueryToken queryDescriptorToken)
        {
            ExceptionUtils.CheckArgumentNotNull(queryDescriptorToken, "queryDescriptorToken");
            ExceptionUtils.CheckArgumentNotNull(queryDescriptorToken.Path, "queryDescriptorToken.Path");

            // Make a copy of query options since we may consume some of them as we bind the query
            if (queryDescriptorToken.QueryOptions != null)
            {
                this.queryOptions = new List<QueryOptionQueryToken>(queryDescriptorToken.QueryOptions);
            }

            //// TODO: check whether there is a $count segment at the top; if so strip it and first process everything else and
            ////       then add it to the bound query tree at the end

            // First bind the path
            QueryNode query = this.Bind(queryDescriptorToken.Path);

            // Apply filter first, then order-by, skip, top, select and expand
            query = this.ProcessFilter(query, queryDescriptorToken.Filter);
            query = this.ProcessOrderBy(query, queryDescriptorToken.OrderByTokens);
            query = ProcessSkip(query, queryDescriptorToken.Skip);
            query = ProcessTop(query, queryDescriptorToken.Top);

            //// TODO: if we found a $count segment process it now and add it to the query

            QueryDescriptorQueryNode queryDescriptorNode = new QueryDescriptorQueryNode();
            queryDescriptorNode.Query = query;

            // Add the remaining query options to the query descriptor.
            List<QueryNode> boundQueryOptions = this.ProcessQueryOptions();
            queryDescriptorNode.CustomQueryOptions = boundQueryOptions == null ? null : new ReadOnlyCollection<QueryNode>(boundQueryOptions);
            Debug.Assert(this.queryOptions == null, "this.queryOptions == null");

            return queryDescriptorNode;
        }

        /// <summary>
        /// Binds a <see cref="SegmentQueryToken"/>.
        /// </summary>
        /// <param name="segmentToken">The segment token to bind.</param>
        /// <returns>The bound node.</returns>
        protected virtual QueryNode BindSegment(SegmentQueryToken segmentToken)
        {
            ExceptionUtils.CheckArgumentNotNull(segmentToken, "segmentToken");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(segmentToken.Name, "segmentToken.Name");

            if (segmentToken.Parent == null)
            {
                return this.BindRootSegment(segmentToken);
            }
            else
            {
                // TODO: return this.BindNonRootSegment(segmentToken);
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Binds a literal token.
        /// </summary>
        /// <param name="literalToken">The literal token to bind.</param>
        /// <returns>The bound literal token.</returns>
        protected virtual QueryNode BindLiteral(LiteralQueryToken literalToken)
        {
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");

            return new ConstantQueryNode()
            {
                Value = literalToken.Value
            };
        }

        /// <summary>
        /// Binds a binary operator token.
        /// </summary>
        /// <param name="binaryOperatorToken">The binary operator token to bind.</param>
        /// <returns>The bound binary operator token.</returns>
        protected virtual QueryNode BindBinaryOperator(BinaryOperatorQueryToken binaryOperatorToken)
        {
            ExceptionUtils.CheckArgumentNotNull(binaryOperatorToken, "binaryOperatorToken");

            SingleValueQueryNode left = this.Bind(binaryOperatorToken.Left) as SingleValueQueryNode;
            if (left == null)
            {
                throw new ODataException(Strings.MetadataBinder_BinaryOperatorOperandNotSingleValue(binaryOperatorToken.OperatorKind.ToString()));
            }

            SingleValueQueryNode right = this.Bind(binaryOperatorToken.Right) as SingleValueQueryNode;
            if (right == null)
            {
                throw new ODataException(Strings.MetadataBinder_BinaryOperatorOperandNotSingleValue(binaryOperatorToken.OperatorKind.ToString()));
            }

            ResourceType leftType = left.ResourceType;
            ResourceType rightType = right.ResourceType;
            if (!TypePromotionUtils.PromoteOperandTypes(binaryOperatorToken.OperatorKind, ref leftType, ref rightType))
            {
                string leftTypeName = left.ResourceType == null ? "<null>" : left.ResourceType.FullName;
                string rightTypeName = right.ResourceType == null ? "<null>" : right.ResourceType.FullName;
                throw new ODataException(Strings.MetadataBinder_IncompatibleOperandsError(leftTypeName, rightTypeName, binaryOperatorToken.OperatorKind));
            }

            if (leftType != null)
            {
                left = this.ConvertToType(left, leftType);
            }

            if (rightType != null)
            {
                right = this.ConvertToType(right, rightType);
            }

            return new BinaryOperatorQueryNode()
            {
                OperatorKind = binaryOperatorToken.OperatorKind,
                Left = left,
                Right = right
            };
        }

        /// <summary>
        /// Binds a unary operator token.
        /// </summary>
        /// <param name="unaryOperatorToken">The unary operator token to bind.</param>
        /// <returns>The bound unary operator token.</returns>
        protected virtual QueryNode BindUnaryOperator(UnaryOperatorQueryToken unaryOperatorToken)
        {
            ExceptionUtils.CheckArgumentNotNull(unaryOperatorToken, "unaryOperatorToken");

            SingleValueQueryNode operand = this.Bind(unaryOperatorToken.Operand) as SingleValueQueryNode;
            if (operand == null)
            {
                throw new ODataException(Strings.MetadataBinder_UnaryOperatorOperandNotSingleValue(unaryOperatorToken.OperatorKind.ToString()));
            }

            ResourceType resourceType = operand.ResourceType;
            if (!TypePromotionUtils.PromoteOperandType(unaryOperatorToken.OperatorKind, ref resourceType))
            {
                string typeName = operand.ResourceType == null ? "<null>" : operand.ResourceType.FullName;
                throw new ODataException(Strings.MetadataBinder_IncompatibleOperandError(typeName, unaryOperatorToken.OperatorKind));
            }

            if (resourceType != null)
            {
                operand = this.ConvertToType(operand, resourceType);
            }

            return new UnaryOperatorQueryNode()
            {
                OperatorKind = unaryOperatorToken.OperatorKind,
                Operand = operand
            };
        }

        /// <summary>
        /// Binds a property access token.
        /// </summary>
        /// <param name="propertyAccessToken">The property access token to bind.</param>
        /// <returns>The bound property access token.</returns>
        protected virtual QueryNode BindPropertyAccess(PropertyAccessQueryToken propertyAccessToken)
        {
            ExceptionUtils.CheckArgumentNotNull(propertyAccessToken, "propertyAccessToken");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(propertyAccessToken.Name, "propertyAccessToken.Name");

            SingleValueQueryNode parentNode;
            if (propertyAccessToken.Parent == null)
            {
                if (this.parameter == null)
                {
                    throw new ODataException(Strings.MetadataBinder_PropertyAccessWithoutParentParameter);
                }

                parentNode = this.parameter;
            }
            else
            {
                // TODO: Do we really want to fail with cast exception if the parent is a collection (for example)?
                parentNode = this.Bind(propertyAccessToken.Parent) as SingleValueQueryNode;
                if (parentNode == null)
                {
                    throw new ODataException(Strings.MetadataBinder_PropertyAccessSourceNotSingleValue(propertyAccessToken.Name));
                }
            }

            ResourceProperty property = parentNode.ResourceType == null ?
                null : parentNode.ResourceType.TryResolvePropertyName(propertyAccessToken.Name);

            if (property != null)
            {
                // TODO: Check that we have resourceset once we add the resource set propagation into the bound tree
                // See RequestQueryParser.ExpressionParser.ParseMemberAccess
                if (property.Kind == ResourcePropertyKind.MultiValue)
                {
                    throw new ODataException(Strings.MetadataBinder_MultiValuePropertyNotSupportedInExpression(property.Name));
                }

                if (property.Kind == ResourcePropertyKind.ResourceReference || property.Kind == ResourcePropertyKind.ResourceSetReference)
                {
                    // TODO: Implement navigations
                    throw new NotImplementedException();
                }

                return new PropertyAccessQueryNode()
                {
                    Source = parentNode,
                    Property = property
                };
            }
            else
            {
                if (parentNode.ResourceType != null && !parentNode.ResourceType.IsOpenType)
                {
                    throw new ODataException(Strings.MetadataBinder_PropertyNotDeclared(parentNode.ResourceType.FullName, propertyAccessToken.Name));
                }

                // TODO: Implement open property support
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Binds a function call token.
        /// </summary>
        /// <param name="functionCallToken">The function call token to bind.</param>
        /// <returns>The bound function call token.</returns>
        protected virtual QueryNode BindFunctionCall(FunctionCallQueryToken functionCallToken)
        {
            ExceptionUtils.CheckArgumentNotNull(functionCallToken, "functionCallToken");
            ExceptionUtils.CheckArgumentNotNull(functionCallToken.Name, "functionCallToken.Name");

            // Bind all arguments
            List<QueryNode> argumentNodes = new List<QueryNode>();
            if (functionCallToken.Arguments != null)
            {
                foreach (QueryToken argumentToken in functionCallToken.Arguments)
                {
                    argumentNodes.Add(this.Bind(argumentToken));
                }
            }

            // Special case the operators which look like function calls
            if (functionCallToken.Name == "cast")
            {
                // TODO: Implement cast operators
                throw new NotImplementedException();
            }
            else if (functionCallToken.Name == "isof")
            {
                // TODO: Implement cast operators
                throw new NotImplementedException();
            }
            else
            {
                // Try to find the function in our built-in functions
                BuiltInFunctionSignature[] signatures;
                if (!BuiltInFunctions.TryGetBuiltInFunction(functionCallToken.Name, out signatures))
                {
                    throw new ODataException(Strings.MetadataBinder_UnknownFunction(functionCallToken.Name));
                }

                // Right now all functions take a single value for all arguments
                ResourceType[] argumentTypes = new ResourceType[argumentNodes.Count];
                for (int i = 0; i < argumentNodes.Count; i++)
                {
                    SingleValueQueryNode argumentNode = argumentNodes[i] as SingleValueQueryNode;
                    if (argumentNode == null)
                    {
                        throw new ODataException(Strings.MetadataBinder_FunctionArgumentNotSingleValue(functionCallToken.Name));
                    }

                    argumentTypes[i] = argumentNode.ResourceType;
                }

                BuiltInFunctionSignature signature = (BuiltInFunctionSignature)
                    TypePromotionUtils.FindBestFunctionSignature(signatures, argumentTypes);
                if (signature == null)
                {
                    throw new ODataException(
                        Strings.MetadataBinder_NoApplicableFunctionFound(
                            functionCallToken.Name,
                            BuiltInFunctions.BuildFunctionSignatureListDescription(functionCallToken.Name, signatures)));
                }

                // Convert all argument nodes to the best signature argument type
                Debug.Assert(signature.ArgumentTypes.Length == argumentNodes.Count, "The best signature match doesn't have the same number of arguments.");
                for (int i = 0; i < argumentNodes.Count; i++)
                {
                    Debug.Assert(argumentNodes[i] is SingleValueQueryNode, "We should have already verified that all arguments are single values.");
                    SingleValueQueryNode argumentNode = (SingleValueQueryNode)argumentNodes[i];
                    ResourceType signatureArgumentType = signature.ArgumentTypes[i];
                    if (signatureArgumentType != argumentNode.ResourceType)
                    {
                        argumentNodes[i] = this.ConvertToType(argumentNode, signatureArgumentType);
                    }
                }

                return new SingleValueFunctionCallQueryNode
                {
                    Name = functionCallToken.Name,
                    Arguments = new ReadOnlyCollection<QueryNode>(argumentNodes),
                    ReturnType = signature.ReturnType
                };
            }
        }

        /// <summary>
        /// Binds a query option token.
        /// </summary>
        /// <param name="queryOptionToken">The query option token to bind.</param>
        /// <returns>The bound query option token.</returns>
        /// <remarks>The default implementation of this method will not allow any system query options 
        /// (query options starting with '$') that were not previously processed.</remarks>
        protected virtual QueryNode BindQueryOption(QueryOptionQueryToken queryOptionToken)
        {
            ExceptionUtils.CheckArgumentNotNull(queryOptionToken, "queryOptionToken");

            // If we find a system query option here we will throw
            string name = queryOptionToken.Name;
            if (!string.IsNullOrEmpty(name) && name[0] == '$')
            {
                throw new ODataException(Strings.MetadataBinder_UnsupportedSystemQueryOption(name));
            }

            return new CustomQueryOptionQueryNode
            {
                Name = name,
                Value = queryOptionToken.Value
            };
        }

        /// <summary>
        /// Processes the skip operator (if any) and returns the combined query.
        /// </summary>
        /// <param name="query">The query tree constructed so far.</param>
        /// <param name="skip">The skip amount or null if none was specified.</param>
        /// <returns>
        /// The unmodified <paramref name="query"/> if no skip is specified or the combined 
        /// query tree including the skip operator if it was specified.
        /// </returns>
        private static QueryNode ProcessSkip(QueryNode query, int? skip)
        {
            ExceptionUtils.CheckArgumentNotNull(query, "query");

            if (skip.HasValue)
            {
                CollectionQueryNode entityCollection = query.AsEntityCollectionNode();
                if (entityCollection == null)
                {
                    throw new ODataException(Strings.MetadataBinder_SkipNotApplicable);
                }

                int skipValue = skip.Value;
                if (skipValue < 0)
                {
                    throw new ODataException(Strings.MetadataBinder_SkipRequiresNonNegativeInteger(skipValue.ToString(CultureInfo.CurrentCulture)));
                }

                query = new SkipQueryNode()
                {
                    Collection = entityCollection,
                    Amount = new ConstantQueryNode { Value = skipValue }
                };
            }

            return query;
        }

        /// <summary>
        /// Processes the top operator (if any) and returns the combined query.
        /// </summary>
        /// <param name="query">The query tree constructed so far.</param>
        /// <param name="top">The top amount or null if none was specified.</param>
        /// <returns>
        /// The unmodified <paramref name="query"/> if no top is specified or the combined
        /// query tree including the top operator if it was specified.
        /// </returns>
        private static QueryNode ProcessTop(QueryNode query, int? top)
        {
            ExceptionUtils.CheckArgumentNotNull(query, "query");

            if (top.HasValue)
            {
                CollectionQueryNode entityCollection = query.AsEntityCollectionNode();
                if (entityCollection == null)
                {
                    throw new ODataException(Strings.MetadataBinder_TopNotApplicable);
                }

                int topValue = top.Value;
                if (topValue < 0)
                {
                    throw new ODataException(Strings.MetadataBinder_TopRequiresNonNegativeInteger(topValue.ToString(CultureInfo.CurrentCulture)));
                }

                query = new TopQueryNode()
                {
                    Collection = entityCollection,
                    Amount = new ConstantQueryNode { Value = topValue }
                };
            }

            return query;
        }

        /// <summary>
        /// Processes the filter of the query (if any).
        /// </summary>
        /// <param name="query">The query tree constructed so far.</param>
        /// <param name="filter">The filter to bind.</param>
        /// <returns>If no filter is specified, returns the <paramref name="query"/> unchanged. If a filter is specified it returns the combined query including the filter.</returns>
        private QueryNode ProcessFilter(QueryNode query, QueryToken filter)
        {
            ExceptionUtils.CheckArgumentNotNull(query, "query");

            if (filter != null)
            {
                CollectionQueryNode entityCollection = query.AsEntityCollectionNode();
                if (entityCollection == null)
                {
                    throw new ODataException(Strings.MetadataBinder_FilterNotApplicable);
                }

                this.parameter = new ParameterQueryNode() { ParameterResourceType = entityCollection.ItemType };

                QueryNode expressionNode = this.Bind(filter);

                SingleValueQueryNode expressionResultNode = expressionNode as SingleValueQueryNode;
                if (expressionResultNode == null ||
                    (expressionResultNode.ResourceType != null && expressionResultNode.ResourceType.ResourceTypeKind != ResourceTypeKind.Primitive))
                {
                    throw new ODataException(Strings.MetadataBinder_FilterExpressionNotSingleValue);
                }

                // The resource type may be null here if the query statically represents the null literal
                // TODO: once we support open types/properties a 'null' resource type will mean 'we don't know the type'. Review.
                if (expressionResultNode.ResourceType != null &&
                    !this.ResourceTypesEqual(expressionResultNode.ResourceType, PrimitiveTypeUtils.BoolResourceType) &&
                    !this.ResourceTypesEqual(expressionResultNode.ResourceType, PrimitiveTypeUtils.NullableBoolResourceType))
                {
                    throw new ODataException(Strings.MetadataBinder_FilterExpressionNotSingleValue);
                }

                query = new FilterQueryNode()
                {
                    Collection = entityCollection,
                    Parameter = this.parameter,
                    Expression = expressionResultNode
                };

                this.parameter = null;
            }

            return query;
        }

        /// <summary>
        /// Processes the order-by tokens of a query (if any).
        /// </summary>
        /// <param name="query">The query tree constructed so far.</param>
        /// <param name="orderByTokens">The order-by tokens to bind.</param>
        /// <returns>If no order-by tokens are specified, returns the <paramref name="query"/> unchanged. If order-by tokens are specified it returns the combined query including the ordering.</returns>
        private QueryNode ProcessOrderBy(QueryNode query, IEnumerable<OrderByQueryToken> orderByTokens)
        {
            ExceptionUtils.CheckArgumentNotNull(query, "query");

            if (orderByTokens != null)
            {
                foreach (OrderByQueryToken orderByToken in orderByTokens)
                {
                    query = this.ProcessSingleOrderBy(query, orderByToken);
                }
            }

            return query;
        }

        /// <summary>
        /// Process the remaining query options (represent the set of custom query options after
        /// service operation parameters and system query options have been removed).
        /// </summary>
        /// <returns>The list of <see cref="QueryNode"/> instances after binding.</returns>
        private List<QueryNode> ProcessQueryOptions()
        {
            List<QueryNode> customQueryOptionNodes = null;
            
            if (this.queryOptions != null && this.queryOptions.Count > 0)
            {
                customQueryOptionNodes = new List<QueryNode>();

                foreach (QueryOptionQueryToken queryToken in this.queryOptions)
                {
                    QueryNode customQueryOptionNode = this.Bind(queryToken);
                    if (customQueryOptionNode != null)
                    {
                        customQueryOptionNodes.Add(customQueryOptionNode);
                    }
                }
            }

            this.queryOptions = null;

            return customQueryOptionNodes;
        }

        /// <summary>
        /// Processes the specified order-by token.
        /// </summary>
        /// <param name="query">The query tree constructed so far.</param>
        /// <param name="orderByToken">The order-by token to bind.</param>
        /// <returns>Returns the combined query including the ordering.</returns>
        private QueryNode ProcessSingleOrderBy(QueryNode query, OrderByQueryToken orderByToken)
        {
            Debug.Assert(query != null, "query != null");
            ExceptionUtils.CheckArgumentNotNull(orderByToken, "orderByToken");

            CollectionQueryNode entityCollection = query.AsEntityCollectionNode();
            if (entityCollection == null)
            {
                throw new ODataException(Strings.MetadataBinder_OrderByNotApplicable);
            }

            this.parameter = new ParameterQueryNode() { ParameterResourceType = entityCollection.ItemType };

            QueryNode expressionNode = this.Bind(orderByToken.Expression);

            // TODO: shall we really restrict order-by expressions to primitive types?
            SingleValueQueryNode expressionResultNode = expressionNode as SingleValueQueryNode;
            if (expressionResultNode == null ||
                (expressionResultNode.ResourceType != null && expressionResultNode.ResourceType.ResourceTypeKind != ResourceTypeKind.Primitive))
            {
                throw new ODataException(Strings.MetadataBinder_OrderByExpressionNotSingleValue);
            }

            query = new OrderByQueryNode()
            {
                Collection = entityCollection,
                Direction = orderByToken.Direction,
                Parameter = this.parameter,
                Expression = expressionResultNode
            };

            this.parameter = null;

            return query;
        }

        /// <summary>
        /// Binds a root path segment.
        /// </summary>
        /// <param name="segmentToken">The segment to bind.</param>
        /// <returns>The bound node.</returns>
        private QueryNode BindRootSegment(SegmentQueryToken segmentToken)
        {
            Debug.Assert(segmentToken != null, "segmentToken != null");
            Debug.Assert(segmentToken.Parent == null, "Only root segments should be allowed here.");
            Debug.Assert(!string.IsNullOrEmpty(segmentToken.Name), "!string.IsNullOrEmpty(segmentToken.Name)");

            //// This is a metadata-only version of the RequestUriProcessor.CreateFirstSegment.

            if (segmentToken.Name == UriQueryConstants.MetadataSegment)
            {
                // TODO: $metadata segment parsing - no key values are allowed.
                throw new NotImplementedException();
            }

            if (segmentToken.Name == UriQueryConstants.BatchSegment)
            {
                // TODO: $batch segment parsing - no key values are allowed.
                throw new NotImplementedException();
            }

            // TODO: WCF DS checks for $count here first and fails. But not for the other $ segments.
            // which means other $segments get to SO resolution. On the other hand the WCF DS would eventually fail if an SO started with $.

            // Look for a service operation
            ServiceOperationWrapper serviceOperation = this.metadataProvider.TryResolveServiceOperation(segmentToken.Name);
            if (serviceOperation != null)
            {
                return this.BindServiceOperation(segmentToken, serviceOperation);
            }

            // TODO: Content-ID reference resolution. Do we actually do anything here or do we perform this through extending the metadata binder?

            // Look for an entity set.
            ResourceSetWrapper resourceSet = this.metadataProvider.TryResolveResourceSet(segmentToken.Name);
            if (resourceSet == null)
            {
                throw new ODataException(Strings.MetadataBinder_RootSegmentResourceNotFound(segmentToken.Name));
            }

            ResourceSetQueryNode resourceSetQueryNode = new ResourceSetQueryNode()
            {
                ResourceSet = resourceSet.ResourceSet
            };

            if (segmentToken.NamedValues != null)
            {
                return this.BindKeyValues(resourceSetQueryNode, segmentToken.NamedValues);
            }
            else
            {
                return resourceSetQueryNode;
            }
        }

        /// <summary>
        /// Binds a service operation segment.
        /// </summary>
        /// <param name="segmentToken">The segment which represents a service operation.</param>
        /// <param name="serviceOperation">The service operation to bind.</param>
        /// <returns>The bound node.</returns>
        private QueryNode BindServiceOperation(SegmentQueryToken segmentToken, ServiceOperationWrapper serviceOperation)
        {
            Debug.Assert(segmentToken != null, "segmentToken != null");
            Debug.Assert(serviceOperation != null, "serviceOperation != null");
            Debug.Assert(segmentToken.Name == serviceOperation.Name, "The segment represents a different service operation.");

            //// This is a metadata copy of the RequestUriProcessor.CreateSegmentForServiceOperation

            //// The WCF DS checks the verb in this place, we can't do that here

            // All service operations other than those returning IQueryable MUST NOT have () appended to the URI
            // V1/V2 behavior: if it's IEnumerable<T>, we do not allow () either, because it's not further composable.
            if (serviceOperation.ResultKind != ServiceOperationResultKind.QueryWithMultipleResults && 
                segmentToken.NamedValues != null)
            {
                throw new ODataException(Strings.MetadataBinder_NonQueryableServiceOperationWithKeyLookup(segmentToken.Name));
            }

            IEnumerable<QueryNode> serviceOperationParameters = this.BindServiceOperationParameters(serviceOperation);
            switch (serviceOperation.ResultKind)
            {
                case ServiceOperationResultKind.QueryWithMultipleResults:
                    if (serviceOperation.ResultType.ResourceTypeKind != ResourceTypeKind.EntityType)
                    {
                        throw new ODataException(Strings.MetadataBinder_QueryServiceOperationOfNonEntityType(serviceOperation.Name, serviceOperation.ResultKind.ToString(), serviceOperation.ResultType.FullName));
                    }

                    CollectionServiceOperationQueryNode collectionServiceOperationQueryNode = new CollectionServiceOperationQueryNode()
                    {
                        ServiceOperation = serviceOperation.ServiceOperation,
                        Parameters = serviceOperationParameters
                    };

                    if (segmentToken.NamedValues != null)
                    {
                        return this.BindKeyValues(collectionServiceOperationQueryNode, segmentToken.NamedValues);
                    }
                    else
                    {
                        return collectionServiceOperationQueryNode;
                    }

                case ServiceOperationResultKind.QueryWithSingleResult:
                    if (serviceOperation.ResultType.ResourceTypeKind != ResourceTypeKind.EntityType)
                    {
                        throw new ODataException(Strings.MetadataBinder_QueryServiceOperationOfNonEntityType(serviceOperation.Name, serviceOperation.ResultKind.ToString(), serviceOperation.ResultType.FullName));
                    }

                    return new SingleValueServiceOperationQueryNode()
                    {
                        ServiceOperation = serviceOperation.ServiceOperation,
                        Parameters = serviceOperationParameters
                    };

                case ServiceOperationResultKind.DirectValue:
                    if (serviceOperation.ResultType.ResourceTypeKind == ResourceTypeKind.Primitive)
                    {
                        // Direct primitive values are composable, $value is allowed on them.
                        return new SingleValueServiceOperationQueryNode()
                        {
                            ServiceOperation = serviceOperation.ServiceOperation,
                            Parameters = serviceOperationParameters
                        };
                    }
                    else
                    {
                        // Direct non-primitive values are not composable at all
                        return new UncomposableServiceOperationQueryNode()
                        {
                            ServiceOperation = serviceOperation.ServiceOperation,
                            Parameters = serviceOperationParameters
                        };
                    }

                case ServiceOperationResultKind.Enumeration:
                case ServiceOperationResultKind.Void:
                    // Enumeration and void service operations are not composable
                    return new UncomposableServiceOperationQueryNode()
                    {
                        ServiceOperation = serviceOperation.ServiceOperation,
                        Parameters = serviceOperationParameters
                    };

                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.MetadataBinder_BindServiceOperation));
            }
        }

        /// <summary>
        /// Binds the service operation parameters from query options.
        /// </summary>
        /// <param name="serviceOperation">The service operation to bind the parameters for.</param>
        /// <returns>Enumeration of parameter values for the service operation.</returns>
        private IEnumerable<QueryNode> BindServiceOperationParameters(ServiceOperationWrapper serviceOperation)
        {
            Debug.Assert(serviceOperation != null, "serviceOperation != null");

            //// This is a copy of RequestUriProcessor.ReadOperationParameters

            if (serviceOperation.Parameters.Count == 0)
            {
                return null;
            }

            List<QueryNode> parameters = new List<QueryNode>(serviceOperation.Parameters.Count);
            for (int i = 0; i < serviceOperation.Parameters.Count; i++)
            {
                ServiceOperationParameter serviceOperationParameter = serviceOperation.Parameters[i];
                Type parameterType = serviceOperationParameter.ParameterType.InstanceType;
                Type nonNullableParameterType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;
                string parameterTextValue = this.ConsumeQueryOption(serviceOperationParameter.Name);
                object parameterValue;
                if (string.IsNullOrEmpty(parameterTextValue))
                {
                    if (!parameterType.IsClass && (parameterType == nonNullableParameterType))
                    {
                        // The target parameter type is non-nullable, but we found null value, this usually means that the parameter is missing
                        throw new ODataException(Strings.MetadataBinder_ServiceOperationParameterMissing(serviceOperation.Name, serviceOperationParameter.Name));
                    }

                    parameterValue = null;
                }
                else
                {
                    // We choose to be a little more flexible than with keys and
                    // allow surrounding whitespace (which is never significant).
                    parameterTextValue = parameterTextValue.Trim();
                    if (!UriPrimitiveTypeParser.TryUriStringToPrimitive(parameterTextValue, parameterType, out parameterValue))
                    {
                        throw new ODataException(Strings.MetadataBinder_ServiceOperationParameterInvalidType(serviceOperationParameter.Name, parameterTextValue, serviceOperation.Name, serviceOperationParameter.ParameterType.FullName));
                    }
                }

                parameters.Add(new ConstantQueryNode() { Value = parameterValue });
            }

            return new ReadOnlyCollection<QueryNode>(parameters);
        }

        /// <summary>
        /// Binds key values to a key lookup on a collection.
        /// </summary>
        /// <param name="collectionQueryNode">Already bound collection node.</param>
        /// <param name="namedValues">The named value tokens to bind.</param>
        /// <returns>The bound key lookup.</returns>
        private QueryNode BindKeyValues(CollectionQueryNode collectionQueryNode, IEnumerable<NamedValue> namedValues)
        {
            Debug.Assert(namedValues != null, "namedValues != null");
            Debug.Assert(collectionQueryNode != null, "collectionQueryNode != null");
            ResourceType collectionItemType = this.CheckAndValidateResourceType(collectionQueryNode.ItemType, "collectionQueryNode.ItemType");

            List<KeyPropertyValue> keyPropertyValues = new List<KeyPropertyValue>();

            if (collectionItemType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new ODataException(Strings.MetadataBinder_KeyValueApplicableOnlyToEntityType(collectionItemType.FullName));
            }

            HashSet<string> keyPropertyNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (NamedValue namedValue in namedValues)
            {
                KeyPropertyValue keyPropertyValue = this.BindKeyPropertyValue(namedValue, collectionItemType);
                Debug.Assert(keyPropertyValue != null, "keyPropertyValue != null");
                Debug.Assert(keyPropertyValue.KeyProperty != null, "keyPropertyValue.KeyProperty != null");

                if (!keyPropertyNames.Add(keyPropertyValue.KeyProperty.Name))
                {
                    throw new ODataException(Strings.MetadataBinder_DuplicitKeyPropertyInKeyValues(keyPropertyValue.KeyProperty.Name));
                }

                keyPropertyValues.Add(keyPropertyValue);
            }

            if (keyPropertyValues.Count == 0)
            {
                // No key values specified, for example '/Customers()', do not include the key lookup at all
                return collectionQueryNode;
            }
            else if (keyPropertyValues.Count != collectionQueryNode.ItemType.KeyProperties.Count)
            {
                throw new ODataException(Strings.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(collectionQueryNode.ItemType.FullName));
            }
            else
            {
                return new KeyLookupQueryNode()
                {
                    Collection = collectionQueryNode,
                    KeyPropertyValues = new ReadOnlyCollection<KeyPropertyValue>(keyPropertyValues)
                };
            }
        }

        /// <summary>
        /// Binds a key property value.
        /// </summary>
        /// <param name="namedValue">The named value to bind.</param>
        /// <param name="collectionItemType">The resource type of a single item in a collection to apply the key value to.</param>
        /// <returns>The bound key property value node.</returns>
        private KeyPropertyValue BindKeyPropertyValue(NamedValue namedValue, ResourceType collectionItemType)
        {
            // These are exception checks because the data comes directly from the potentially user specified tree.
            ExceptionUtils.CheckArgumentNotNull(namedValue, "namedValue");
            ExceptionUtils.CheckArgumentNotNull(namedValue.Value, "namedValue.Value");
            Debug.Assert(collectionItemType != null, "collectionItemType != null");
            Debug.Assert(collectionItemType.ResourceTypeKind == ResourceTypeKind.EntityType, "collectionItemType.ResourceTypeKind == ResourceTypeKind.EntityType");

            ResourceProperty keyProperty;
            if (namedValue.Name == null)
            {
                if (collectionItemType.KeyProperties.Count != 1)
                {
                    throw new ODataException(Strings.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(collectionItemType.FullName));
                }

                keyProperty = collectionItemType.KeyProperties[0];
            }
            else
            {
                keyProperty = collectionItemType.TryResolvePropertyName(namedValue.Name);
                if (keyProperty == null)
                {
                    throw new ODataException(Strings.MetadataBinder_PropertyNotDeclared(collectionItemType.FullName, namedValue.Name));
                }

                if (!keyProperty.IsOfKind(ResourcePropertyKind.Key))
                {
                    throw new ODataException(Strings.MetadataBinder_PropertyNotKeyInKeyValue(keyProperty.Name, collectionItemType.FullName));
                }
            }

            ResourceType keyPropertyType = this.metadataProvider.ValidateResourceType(keyProperty.ResourceType);

            SingleValueQueryNode value = (SingleValueQueryNode)this.Bind(namedValue.Value);

            // TODO: Check that the value is of primitive type
            value = this.ConvertToType(value, keyPropertyType);

            Debug.Assert(keyProperty != null, "keyProperty != null");
            return new KeyPropertyValue()
            {
                KeyProperty = keyProperty,
                KeyValue = value
            };
        }

        /// <summary>
        /// Checks if the source is of the specified type and if not tries to inject a convert.
        /// </summary>
        /// <param name="source">The source node to apply the convertion to.</param>
        /// <param name="targetType">The target primitive resource type.</param>
        /// <returns>The converted query node.</returns>
        private SingleValueQueryNode ConvertToType(SingleValueQueryNode source, ResourceType targetType)
        {
            Debug.Assert(source != null, "source != null");
            Debug.Assert(targetType != null, "targetType != null");
            Debug.Assert(targetType.ResourceTypeKind == ResourceTypeKind.Primitive, "Can only convert primitive types.");

            if (source.ResourceType != null)
            {
                // TODO: Do we check this here or in the caller?
                Debug.Assert(source.ResourceType.ResourceTypeKind == ResourceTypeKind.Primitive, "Can only work on primitive types.");

                ResourceType sourceResourceType = this.metadataProvider.ValidateResourceType(source.ResourceType);

                // NOTE we have to also allow different resource types with the same name if they are primitive resource
                //      types since the resource types for 'bool' and 'bool?' have the same name.
                Debug.Assert(
                    sourceResourceType == targetType || 
                    sourceResourceType.FullName != targetType.FullName || 
                    sourceResourceType.ResourceTypeKind == ResourceTypeKind.Primitive && targetType.ResourceTypeKind == ResourceTypeKind.Primitive,
                    "The types were not atomized correctly.");

                if (this.ResourceTypesEqual(sourceResourceType, targetType))
                {
                    return source;
                }
                else
                {
                    if (!TypePromotionUtils.CanConvertTo(sourceResourceType.InstanceType, targetType.InstanceType))
                    {
                        throw new ODataException(Strings.MetadataBinder_CannotConvertToType(sourceResourceType.FullName, targetType.FullName));
                    }
                }
            }

            // If the source doesn't have a type (possibly an open property), then it's possible to convert it
            // cause we don't know for sure.
            return new ConvertQueryNode()
            {
                Source = source,
                TargetResourceType = targetType
            };
        }

        /// <summary>
        /// Checks that the resource type is not null and validates it.
        /// </summary>
        /// <param name="resourceType">The resource type to validate.</param>
        /// <param name="parameterName">The name of the parameter to use in error messages.</param>
        /// <returns>The validated resource type (might be a different instance)</returns>
        private ResourceType CheckAndValidateResourceType(ResourceType resourceType, string parameterName)
        {
            ExceptionUtils.CheckArgumentNotNull(resourceType, parameterName);
            return this.metadataProvider.ValidateResourceType(resourceType);
        }

        /// <summary>
        /// Checks if two resource types are equal.
        /// </summary>
        /// <param name="resourceTypeA">One resource type.</param>
        /// <param name="resourceTypeB">The other resource type.</param>
        /// <returns>true if the types are the same, false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The this is used in debug only.")]
        private bool ResourceTypesEqual(ResourceType resourceTypeA, ResourceType resourceTypeB)
        {
            //// TODO: Can we really rely on reference comparison? If so, then we need to test this extensively
            //// Maybe even add some special code into the metadata wrapper to verify this runtime

#if DEBUG
            Debug.Assert(resourceTypeA != null, "resourceTypeA != null");
            Debug.Assert(resourceTypeB != null, "resourceTypeB != null");
            Debug.Assert(object.ReferenceEquals(resourceTypeA, this.metadataProvider.ValidateResourceType(resourceTypeA)), "Resource type not validated.");
            Debug.Assert(object.ReferenceEquals(resourceTypeB, this.metadataProvider.ValidateResourceType(resourceTypeB)), "Resource type not validated.");
#endif

            return object.ReferenceEquals(resourceTypeA, resourceTypeB);
        }

        /// <summary>
        /// Finds a query option by its name and consumes it (removes it from still available query options).
        /// </summary>
        /// <param name="queryOptionName">The query option name to get.</param>
        /// <returns>The value of the query option or null if it was not found.</returns>
        private string ConsumeQueryOption(string queryOptionName)
        {
            if (this.queryOptions != null)
            {
                return this.queryOptions.GetQueryOptionValueAndRemove(queryOptionName);
            }
            else
            {
                return null;
            }
        }
    }
}
