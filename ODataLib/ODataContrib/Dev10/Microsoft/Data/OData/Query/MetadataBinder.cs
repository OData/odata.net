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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Binder which applies metadata to a lexical QueryToken tree and produces a bound semantic QueryNode tree.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Keeping the visitor in one place makes sense.")]
#if INTERNAL_DROP
    internal class MetadataBinder
#else
    public class MetadataBinder
#endif
    {
        /// <summary>
        /// The model used for binding.
        /// </summary>
        private readonly IEdmModel model;

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
        /// <param name="model">The model to use for binding.</param>
        public MetadataBinder(IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            this.model = model;
        }

        /// <summary>
        /// Gets the EDM model.
        /// </summary>
        protected IEdmModel Model
        {
            get { return this.model; }
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
            this.queryOptions = new List<QueryOptionQueryToken>(queryDescriptorToken.QueryOptions);
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
            queryDescriptorNode.CustomQueryOptions = new ReadOnlyCollection<QueryNode>(boundQueryOptions);
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
            ExceptionUtils.CheckArgumentNotNull(segmentToken.Name, "segmentToken.Name");

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

            IEdmTypeReference leftType = left.TypeReference;
            IEdmTypeReference rightType = right.TypeReference;
            if (!TypePromotionUtils.PromoteOperandTypes(binaryOperatorToken.OperatorKind, ref leftType, ref rightType))
            {
                string leftTypeName = left.TypeReference == null ? "<null>" : left.TypeReference.ODataFullName();
                string rightTypeName = right.TypeReference == null ? "<null>" : right.TypeReference.ODataFullName();
                throw new ODataException(Strings.MetadataBinder_IncompatibleOperandsError(leftTypeName, rightTypeName, binaryOperatorToken.OperatorKind));
            }

            if (leftType != null)
            {
                left = ConvertToType(left, leftType);
            }

            if (rightType != null)
            {
                right = ConvertToType(right, rightType);
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

            IEdmTypeReference typeReference = operand.TypeReference;
            if (!TypePromotionUtils.PromoteOperandType(unaryOperatorToken.OperatorKind, ref typeReference))
            {
                string typeName = operand.TypeReference == null ? "<null>" : operand.TypeReference.ODataFullName();
                throw new ODataException(Strings.MetadataBinder_IncompatibleOperandError(typeName, unaryOperatorToken.OperatorKind));
            }

            if (typeReference != null)
            {
                operand = ConvertToType(operand, typeReference);
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

            IEdmStructuredTypeReference structuredParentType = 
                parentNode.TypeReference == null ? null : parentNode.TypeReference.AsStructuredOrNull();
            IEdmProperty property = 
                structuredParentType == null ? null : structuredParentType.FindProperty(propertyAccessToken.Name);

            if (property != null)
            {
                // TODO: Check that we have entity set once we add the entity set propagation into the bound tree
                // See RequestQueryParser.ExpressionParser.ParseMemberAccess
                if (property.Type.IsNonEntityODataCollectionTypeKind())
                {
                    throw new ODataException(Strings.MetadataBinder_MultiValuePropertyNotSupportedInExpression(property.Name));
                }

                if (property.PropertyKind == EdmPropertyKind.Navigation)
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
                if (parentNode.TypeReference != null && !parentNode.TypeReference.Definition.IsOpenType())
                {
                    throw new ODataException(Strings.MetadataBinder_PropertyNotDeclared(parentNode.TypeReference.ODataFullName(), propertyAccessToken.Name));
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
            List<QueryNode> argumentNodes = new List<QueryNode>(functionCallToken.Arguments.Select(ar => this.Bind(ar)));

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
                IEdmTypeReference[] argumentTypes = new IEdmTypeReference[argumentNodes.Count];
                for (int i = 0; i < argumentNodes.Count; i++)
                {
                    SingleValueQueryNode argumentNode = argumentNodes[i] as SingleValueQueryNode;
                    if (argumentNode == null)
                    {
                        throw new ODataException(Strings.MetadataBinder_FunctionArgumentNotSingleValue(functionCallToken.Name));
                    }

                    argumentTypes[i] = argumentNode.TypeReference;
                }

                BuiltInFunctionSignature signature = (BuiltInFunctionSignature)
                    TypePromotionUtils.FindBestFunctionSignature(signatures, argumentTypes);
                if (signature == null)
                {
                    throw new ODataException(Strings.MetadataBinder_NoApplicableFunctionFound(
                        functionCallToken.Name,
                        BuiltInFunctions.BuildFunctionSignatureListDescription(functionCallToken.Name, signatures)));
                }

                // Convert all argument nodes to the best signature argument type
                Debug.Assert(signature.ArgumentTypes.Length == argumentNodes.Count, "The best signature match doesn't have the same number of arguments.");
                for (int i = 0; i < argumentNodes.Count; i++)
                {
                    Debug.Assert(argumentNodes[i] is SingleValueQueryNode, "We should have already verified that all arguments are single values.");
                    SingleValueQueryNode argumentNode = (SingleValueQueryNode)argumentNodes[i];
                    IEdmTypeReference signatureArgumentType = signature.ArgumentTypes[i];
                    if (signatureArgumentType != argumentNode.TypeReference)
                    {
                        argumentNodes[i] = ConvertToType(argumentNode, signatureArgumentType);
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
        /// Checks if the source is of the specified type and if not tries to inject a convert.
        /// </summary>
        /// <param name="source">The source node to apply the convertion to.</param>
        /// <param name="targetTypeReference">The target primitive type.</param>
        /// <returns>The converted query node.</returns>
        private static SingleValueQueryNode ConvertToType(SingleValueQueryNode source, IEdmTypeReference targetTypeReference)
        {
            Debug.Assert(source != null, "source != null");
            Debug.Assert(targetTypeReference != null, "targetType != null");
            Debug.Assert(targetTypeReference.IsODataPrimitiveTypeKind(), "Can only convert primitive types.");

            if (source.TypeReference != null)
            {
                // TODO: Do we check this here or in the caller?
                Debug.Assert(source.TypeReference.IsODataPrimitiveTypeKind(), "Can only work on primitive types.");

                if (source.TypeReference.IsEquivalentTo(targetTypeReference))
                {
                    return source;
                }
                else
                {
                    if (!TypePromotionUtils.CanConvertTo(source.TypeReference, targetTypeReference))
                    {
                        throw new ODataException(Strings.MetadataBinder_CannotConvertToType(source.TypeReference.ODataFullName(), targetTypeReference.ODataFullName()));
                    }
                }
            }

            // If the source doesn't have a type (possibly an open property), then it's possible to convert it
            // cause we don't know for sure.
            return new ConvertQueryNode()
            {
                Source = source,
                TargetType = targetTypeReference
            };
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

                this.parameter = new ParameterQueryNode() { ParameterType = entityCollection.ItemType };

                QueryNode expressionNode = this.Bind(filter);

                SingleValueQueryNode expressionResultNode = expressionNode as SingleValueQueryNode;
                if (expressionResultNode == null ||
                    (expressionResultNode.TypeReference != null && !expressionResultNode.TypeReference.IsODataPrimitiveTypeKind()))
                {
                    throw new ODataException(Strings.MetadataBinder_FilterExpressionNotSingleValue);
                }

                // The type may be null here if the query statically represents the null literal
                // TODO: once we support open types/properties a 'null' type will mean 'we don't know the type'. Review.
                IEdmTypeReference expressionResultType = expressionResultNode.TypeReference;
                if (expressionResultType != null)
                {
                    IEdmPrimitiveTypeReference primitiveExpressionResultType = expressionResultType.AsPrimitiveOrNull();
                    if (primitiveExpressionResultType == null || primitiveExpressionResultType.PrimitiveKind() != EdmPrimitiveTypeKind.Boolean)
                    {
                        throw new ODataException(Strings.MetadataBinder_FilterExpressionNotSingleValue);
                    }
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

            foreach (OrderByQueryToken orderByToken in orderByTokens)
            {
                query = this.ProcessSingleOrderBy(query, orderByToken);
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
            List<QueryNode> customQueryOptionNodes = new List<QueryNode>();
            foreach (QueryOptionQueryToken queryToken in this.queryOptions)
            {
                QueryNode customQueryOptionNode = this.Bind(queryToken);
                if (customQueryOptionNode != null)
                {
                    customQueryOptionNodes.Add(customQueryOptionNode);
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

            this.parameter = new ParameterQueryNode() { ParameterType = entityCollection.ItemType };

            QueryNode expressionNode = this.Bind(orderByToken.Expression);

            // TODO: shall we really restrict order-by expressions to primitive types?
            SingleValueQueryNode expressionResultNode = expressionNode as SingleValueQueryNode;
            if (expressionResultNode == null ||
                (expressionResultNode.TypeReference != null && !expressionResultNode.TypeReference.IsODataPrimitiveTypeKind()))
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
            IEdmFunctionImport serviceOperation = this.model.TryResolveServiceOperation(segmentToken.Name);
            if (serviceOperation != null)
            {
                return this.BindServiceOperation(segmentToken, serviceOperation);
            }

            // TODO: Content-ID reference resolution. Do we actually do anything here or do we perform this through extending the metadata binder?

            // Look for an entity set.
            IEdmEntitySet entitySet = this.model.TryResolveEntitySet(segmentToken.Name);
            if (entitySet == null)
            {
                throw new ODataException(Strings.MetadataBinder_RootSegmentResourceNotFound(segmentToken.Name));
            }

            EntitySetQueryNode entitySetQueryNode = new EntitySetQueryNode()
            {
                EntitySet = entitySet
            };

            if (segmentToken.NamedValues != null)
            {
                return this.BindKeyValues(entitySetQueryNode, segmentToken.NamedValues);
            }

            return entitySetQueryNode;
        }

        /// <summary>
        /// Binds a service operation segment.
        /// </summary>
        /// <param name="segmentToken">The segment which represents a service operation.</param>
        /// <param name="serviceOperation">The service operation to bind.</param>
        /// <returns>The bound node.</returns>
        private QueryNode BindServiceOperation(SegmentQueryToken segmentToken, IEdmFunctionImport serviceOperation)
        {
            Debug.Assert(segmentToken != null, "segmentToken != null");
            Debug.Assert(serviceOperation != null, "serviceOperation != null");
            Debug.Assert(segmentToken.Name == serviceOperation.Name, "The segment represents a different service operation.");

            //// This is a metadata copy of the RequestUriProcessor.CreateSegmentForServiceOperation

            //// The WCF DS checks the verb in this place, we can't do that here

            // All service operations other than those returning IQueryable MUST NOT have () appended to the URI
            // V1/V2 behavior: if it's IEnumerable<T>, we do not allow () either, because it's not further composable.
            ODataServiceOperationResultKind? resultKind = serviceOperation.GetServiceOperationResultKind(this.model);
            if (resultKind != ODataServiceOperationResultKind.QueryWithMultipleResults && segmentToken.NamedValues != null)
            {
                throw new ODataException(Strings.MetadataBinder_NonQueryableServiceOperationWithKeyLookup(segmentToken.Name));
            }

            if (!resultKind.HasValue)
            {
                throw new ODataException(Strings.MetadataBinder_ServiceOperationWithoutResultKind(serviceOperation.Name));
            }

            IEnumerable<QueryNode> serviceOperationParameters = this.BindServiceOperationParameters(serviceOperation);
            switch (resultKind.Value)
            {
                case ODataServiceOperationResultKind.QueryWithMultipleResults:
                    if (serviceOperation.ReturnType.TypeKind() != EdmTypeKind.Entity)
                    {
                        throw new ODataException(Strings.MetadataBinder_QueryServiceOperationOfNonEntityType(serviceOperation.Name, resultKind.Value.ToString(), serviceOperation.ReturnType.ODataFullName()));
                    }

                    CollectionServiceOperationQueryNode collectionServiceOperationQueryNode = new CollectionServiceOperationQueryNode()
                    {
                        ServiceOperation = serviceOperation,
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

                case ODataServiceOperationResultKind.QueryWithSingleResult:
                    if (!serviceOperation.ReturnType.IsODataEntityTypeKind())
                    {
                        throw new ODataException(Strings.MetadataBinder_QueryServiceOperationOfNonEntityType(serviceOperation.Name, resultKind.Value.ToString(), serviceOperation.ReturnType.ODataFullName()));
                    }

                    return new SingleValueServiceOperationQueryNode()
                    {
                        ServiceOperation = serviceOperation,
                        Parameters = serviceOperationParameters
                    };

                case ODataServiceOperationResultKind.DirectValue:
                    if (serviceOperation.ReturnType.IsODataPrimitiveTypeKind())
                    {
                        // Direct primitive values are composable, $value is allowed on them.
                        return new SingleValueServiceOperationQueryNode()
                        {
                            ServiceOperation = serviceOperation,
                            Parameters = serviceOperationParameters
                        };
                    }
                    else
                    {
                        // Direct non-primitive values are not composable at all
                        return new UncomposableServiceOperationQueryNode()
                        {
                            ServiceOperation = serviceOperation,
                            Parameters = serviceOperationParameters
                        };
                    }

                case ODataServiceOperationResultKind.Enumeration:
                case ODataServiceOperationResultKind.Void:
                    // Enumeration and void service operations are not composable
                    return new UncomposableServiceOperationQueryNode()
                    {
                        ServiceOperation = serviceOperation,
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
        private IEnumerable<QueryNode> BindServiceOperationParameters(IEdmFunctionImport serviceOperation)
        {
            Debug.Assert(serviceOperation != null, "serviceOperation != null");

            //// This is a copy of RequestUriProcessor.ReadOperationParameters

            List<QueryNode> parameters = new List<QueryNode>();
            foreach (IEdmFunctionParameter serviceOperationParameter in serviceOperation.Parameters)
            {
                IEdmTypeReference parameterType = serviceOperationParameter.Type;
                string parameterTextValue = this.ConsumeQueryOption(serviceOperationParameter.Name);
                object parameterValue;
                if (string.IsNullOrEmpty(parameterTextValue))
                {
                    if (!parameterType.IsNullable)
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
                        throw new ODataException(Strings.MetadataBinder_ServiceOperationParameterInvalidType(serviceOperationParameter.Name, parameterTextValue, serviceOperation.Name, serviceOperationParameter.Type.ODataFullName()));
                    }
                }

                parameters.Add(new ConstantQueryNode() { Value = parameterValue });
            }

            if (parameters.Count == 0)
            {
                return null;
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
            Debug.Assert(collectionQueryNode.ItemType != null, "collectionQueryNode.ItemType != null");

            IEdmTypeReference collectionItemType = collectionQueryNode.ItemType;
            List<KeyPropertyValue> keyPropertyValues = new List<KeyPropertyValue>();

            if (!collectionItemType.IsODataEntityTypeKind())
            {
                throw new ODataException(Strings.MetadataBinder_KeyValueApplicableOnlyToEntityType(collectionItemType.ODataFullName()));
            }

            IEdmEntityTypeReference collectionItemEntityTypeReference = collectionItemType.AsEntityOrNull();
            Debug.Assert(collectionItemEntityTypeReference != null, "collectionItemEntityTypeReference != null");
            IEdmEntityType collectionItemEntityType = collectionItemEntityTypeReference.EntityDefinition();

            HashSet<string> keyPropertyNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (NamedValue namedValue in namedValues)
            {
                KeyPropertyValue keyPropertyValue = this.BindKeyPropertyValue(namedValue, collectionItemEntityType);
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
            else if (keyPropertyValues.Count != collectionItemEntityType.Key().Count())
            {
                throw new ODataException(Strings.MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(collectionQueryNode.ItemType.ODataFullName()));
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
        /// <param name="collectionItemEntityType">The type of a single item in a collection to apply the key value to.</param>
        /// <returns>The bound key property value node.</returns>
        private KeyPropertyValue BindKeyPropertyValue(NamedValue namedValue, IEdmEntityType collectionItemEntityType)
        {
            // These are exception checks because the data comes directly from the potentially user specified tree.
            ExceptionUtils.CheckArgumentNotNull(namedValue, "namedValue");
            ExceptionUtils.CheckArgumentNotNull(namedValue.Value, "namedValue.Value");
            Debug.Assert(collectionItemEntityType != null, "collectionItemType != null");

            IEdmProperty keyProperty = null;
            if (namedValue.Name == null)
            {
                foreach (IEdmProperty p in collectionItemEntityType.Key())
                {
                    if (keyProperty == null)
                    {
                        keyProperty = p;
                    }
                    else
                    {
                        throw new ODataException(Strings.MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(collectionItemEntityType.ODataFullName()));
                    }
                }
            }
            else
            {
                keyProperty = collectionItemEntityType.Key().Where(k => string.CompareOrdinal(k.Name, namedValue.Name) == 0).SingleOrDefault();

                if (keyProperty == null)
                {
                    throw new ODataException(Strings.MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue(namedValue.Name, collectionItemEntityType.ODataFullName()));
                }
            }

            IEdmTypeReference keyPropertyType = keyProperty.Type;

            SingleValueQueryNode value = (SingleValueQueryNode)this.Bind(namedValue.Value);

            // TODO: Check that the value is of primitive type
            value = ConvertToType(value, keyPropertyType);

            Debug.Assert(keyProperty != null, "keyProperty != null");
            return new KeyPropertyValue()
            {
                KeyProperty = keyProperty,
                KeyValue = value
            };
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
