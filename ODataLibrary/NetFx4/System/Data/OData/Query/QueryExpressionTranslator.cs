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
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    #endregion Namespaces.

    /// <summary>
    /// Class which translates semantic tree fragments (QueryNode) into LINQ Expression.
    /// </summary>
#if INTERNAL_DROP
    internal abstract class QueryExpressionTranslator
#else
    public abstract class QueryExpressionTranslator
#endif
    {
        /// <summary>
        /// The name of the method to call on Queryable or Enumerable to perform Where operation.
        /// </summary>
        private const string WhereMethodName = "Where";

        /// <summary>
        /// The name of the method to call on Queryable or Enumerable to perform Skip operation.
        /// </summary>
        private const string SkipMethodName = "Skip";

        /// <summary>
        /// The name of the method to call on Queryable or Enumerable to perform Take operation.
        /// </summary>
        private const string TakeMethodName = "Take";

        /// <summary>
        /// The name of the method to call on Queryable or Enumerable to perform OrderBy operation.
        /// </summary>
        private const string OrderByMethodName = "OrderBy";

        /// <summary>
        /// The name of the method to call on Queryable or Enumerable to perform OrderByDescending operation.
        /// </summary>
        private const string OrderByDescendingMethodName = "OrderByDescending";

        /// <summary>
        /// The name of the method to call on Queryable or Enumerable to perform ThenBy operation.
        /// </summary>
        private const string ThenByMethodName = "ThenBy";

        /// <summary>
        /// The name of the method to call on Queryable or Enumerable to perform ThenByDescending operation.
        /// </summary>
        private const string ThenByDescendingMethodName = "ThenByDescending";

        /// <summary>
        /// Constant expression representing the (untyped) null literal.
        /// </summary>
        private static readonly Expression nullLiteralExpression = Expression.Constant(null);

        /// <summary>
        /// Literal constant true expression.
        /// </summary>
        private static readonly Expression trueLiteralExpression = Expression.Constant(true, typeof(bool));

        /// <summary>
        /// Literal constant false expression.
        /// </summary>
        private static readonly ConstantExpression falseLiteralExpression = Expression.Constant(false);

        /// <summary>
        /// Set to true if null propagation should be injected into the resulting expression tree.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Temporary until the field is actually used.")]
        private readonly bool nullPropagationRequired;

        /// <summary>
        /// Stack which holds definitions for parameter nodes.
        /// </summary>
        /// <remarks>The top of the stack contains the current parameter query node and its respective expression it should be translated to.</remarks>
        private Stack<KeyValuePair<ParameterQueryNode, Expression>> parameterNodeDefinitions;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nullPropagationRequired">true if null propagation should be injected or false otherwise.</param>
        protected QueryExpressionTranslator(bool nullPropagationRequired)
        {
            this.nullPropagationRequired = nullPropagationRequired;
            this.parameterNodeDefinitions = new Stack<KeyValuePair<ParameterQueryNode, Expression>>();
        }

        /// <summary>
        /// Translates a semantic node and all its children into a LINQ expression.
        /// </summary>
        /// <param name="node">The query node to translate.</param>
        /// <returns>The translated LINQ expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Once place to switch on all the node types.")]
        public Expression Translate(QueryNode node)
        {
            ExceptionUtils.CheckArgumentNotNull(node, "node");

            Expression result = null;
            Type requiredExpressionType = null;

            CollectionQueryNode collectionNode = node as CollectionQueryNode;
            SingleValueQueryNode singleValueNode = node as SingleValueQueryNode;
            if (collectionNode != null)
            {
                if (collectionNode.ItemType == null)
                {
                    throw new ODataException(Strings.QueryExpressionTranslator_CollectionQueryNodeWithoutItemType(node.Kind));
                }

                bool requiresIQueryable = false;
                switch (node.Kind)
                {
                    case QueryNodeKind.Extension:
                        result = this.TranslateExtension(node);
                        break;
                    case QueryNodeKind.ResourceSet:
                        requiresIQueryable = true;
                        result = this.TranslateResourceSet((ResourceSetQueryNode)node);
                        break;
                    case QueryNodeKind.CollectionServiceOperation:
                        requiresIQueryable = true;
                        result = this.TranslateCollectionServiceOperation((CollectionServiceOperationQueryNode)node);
                        break;
                    case QueryNodeKind.Skip:
                        // TODO: eventually we will have to support this on IEnumerable as well (as it can happen on expansions)
                        requiresIQueryable = true;
                        result = this.TranslateSkip((SkipQueryNode)node);
                        break;
                    case QueryNodeKind.Top:
                        // TODO: eventually we will have to support this on IEnumerable as well (as it can happen on expansions)
                        requiresIQueryable = true;
                        result = this.TranslateTop((TopQueryNode)node);
                        break;
                    case QueryNodeKind.Filter:
                        // TODO: eventually we will have to support this on IEnumerable as well (as it can happen on expansions)
                        requiresIQueryable = true;
                        result = this.TranslateFilter((FilterQueryNode)node);
                        break;
                    case QueryNodeKind.OrderBy:
                        // TODO: eventually we will have to support this on IEnumerable as well (as it can happen on expansions)
                        requiresIQueryable = true;
                        result = this.TranslateOrderBy((OrderByQueryNode)node);
                        break;
                    default:
                        throw new ODataException(Strings.QueryExpressionTranslator_UnsupportedQueryNodeKind(node.Kind));
                }

                if (requiresIQueryable)
                {
                    requiredExpressionType = typeof(IQueryable<>).MakeGenericType(collectionNode.ItemType.InstanceType);
                }
                else
                {
                    requiredExpressionType = typeof(IEnumerable<>).MakeGenericType(collectionNode.ItemType.InstanceType);
                }
            }
            else if (singleValueNode != null)
            {
                // we allow null resource types for the nodes that can take literal null values and
                // the operators combining them. We have separate checks in the corresponding translate methods
                // to ensure that a null resource type is only allowed if the result is statically known to be null
                ResourceType resourceType = singleValueNode.ResourceType;
                if (resourceType == null && 
                    singleValueNode.Kind != QueryNodeKind.Constant &&
                    singleValueNode.Kind != QueryNodeKind.UnaryOperator &&
                    singleValueNode.Kind != QueryNodeKind.BinaryOperator)
                {
                    throw new ODataException(Strings.QueryExpressionTranslator_SingleValueQueryNodeWithoutResourceType(node.Kind));
                }

                // TODO: Note that we don't always set the requiredExpressionType
                // The reason is that in some cases it can be IQueryable<T> and in other cases just T.
                // When we translate the query path, we need to keep everything IQueryable<T> (even if we know it should be a single value)
                // so that the LINQ providers can recognize the query as a join. Outside of the query path (in filters, expansions and so on)
                // we keep single values as simple T instead.
                switch (node.Kind)
                {
                    case QueryNodeKind.Extension:
                        Debug.Assert(resourceType != null, "resourceType != null");
                        result = this.TranslateExtension(node);
                        break;
                    case QueryNodeKind.SingleValueServiceOperation:
                        Debug.Assert(resourceType != null, "resourceType != null");
                        result = this.TranslateSingleValueServiceOperation((SingleValueServiceOperationQueryNode)node);
                        break;
                    case QueryNodeKind.Constant:
                        result = this.TranslateConstant((ConstantQueryNode)node);
                        requiredExpressionType = resourceType == null ? null : resourceType.InstanceType;
                        break;
                    case QueryNodeKind.Convert:
                        Debug.Assert(resourceType != null, "resourceType != null");
                        result = this.TranslateConvert((ConvertQueryNode)node);
                        requiredExpressionType = resourceType.InstanceType;
                        break;
                    case QueryNodeKind.KeyLookup:
                        Debug.Assert(resourceType != null, "resourceType != null");
                        result = this.TranslateKeyLookup((KeyLookupQueryNode)node);
                        break;
                    case QueryNodeKind.BinaryOperator:
                        result = this.TranslateBinaryOperator((BinaryOperatorQueryNode)node);
                        requiredExpressionType = resourceType == null ? null : resourceType.InstanceType;
                        break;
                    case QueryNodeKind.UnaryOperator:
                        result = this.TranslateUnaryOperator((UnaryOperatorQueryNode)node);
                        requiredExpressionType = resourceType == null ? null : resourceType.InstanceType;
                        break;
                    case QueryNodeKind.PropertyAccess:
                        Debug.Assert(resourceType != null, "resourceType != null");
                        result = this.TranslatePropertyAccess((PropertyAccessQueryNode)node);
                        requiredExpressionType = resourceType.InstanceType;
                        break;
                    case QueryNodeKind.Parameter:
                        Debug.Assert(resourceType != null, "resourceType != null");
                        result = this.TranslateParameter((ParameterQueryNode)node);
                        requiredExpressionType = resourceType.InstanceType;
                        break;
                    case QueryNodeKind.SingleValueFunctionCall:
                        result = this.TranslateSingleValueFunctionCall((SingleValueFunctionCallQueryNode)node);
                        requiredExpressionType = singleValueNode.ResourceType.InstanceType;
                        break;
                    default:
                        throw new ODataException(Strings.QueryExpressionTranslator_UnsupportedQueryNodeKind(node.Kind));
                }
            }
            else
            {
                switch (node.Kind)
                {
                    case QueryNodeKind.Extension:
                        result = this.TranslateExtension(node);
                        break;
                    default:
                        throw new ODataException(Strings.QueryExpressionTranslator_UnsupportedQueryNodeKind(node.Kind));
                }
            }

            if (result == null)
            {
                throw new ODataException(Strings.QueryExpressionTranslator_NodeTranslatedToNull(node.Kind));
            }

            // Check if the result type matches the required type. Note that we do allow to return nullable version of the required type
            // if null propagation is turned on, since null propagation may turn otherwise non-nullable expressions to nullable ones.
            if (requiredExpressionType != null && !requiredExpressionType.IsAssignableFrom(result.Type))
            {
                if (!this.nullPropagationRequired || !TypeUtils.AreTypesEquivalent(TypeUtils.GetNonNullableType(result.Type), requiredExpressionType))
                {
                    throw new ODataException(Strings.QueryExpressionTranslator_NodeTranslatedToWrongType(node.Kind, result.Type, requiredExpressionType));
                }
            }

            return result;
        }

        /// <summary>
        /// Translates an extension node.
        /// </summary>
        /// <param name="extensionNode">The extension node to translate.</param>
        /// <returns>The expression the node was translated to.</returns>
        protected virtual Expression TranslateExtension(QueryNode extensionNode)
        {
            ExceptionUtils.CheckArgumentNotNull(extensionNode, "extensionNode");

            throw new ODataException(Strings.QueryExpressionTranslator_UnsupportedExtensionNode);
        }

        /// <summary>
        /// Translates a resource set node.
        /// </summary>
        /// <param name="resourceSetNode">The resource set query node to translate.</param>
        /// <returns>Expression which evaluates to IQueryable&lt;T&gt; where T is the InstanceType of the ResourceType of the resource set.</returns>
        protected abstract Expression TranslateResourceSet(ResourceSetQueryNode resourceSetNode);

        /// <summary>
        /// Translates a service operation node which returns a collection of entities.
        /// </summary>
        /// <param name="serviceOperationNode">The collection service operation node to translate.</param>
        /// <returns>Expression which evaluates to IQueryable&lt;T&gt; where T is the InstanceType of the ResultType of the service operation.</returns>
        protected abstract Expression TranslateCollectionServiceOperation(CollectionServiceOperationQueryNode serviceOperationNode);

        /// <summary>
        /// Translates a service operation node which returns a single value.
        /// </summary>
        /// <param name="serviceOperationNode">The single value service operation node to translate.</param>
        /// <returns>Expression which evaluates to type which is the InstanceType of the ResultType of the service operation.</returns>
        protected abstract Expression TranslateSingleValueServiceOperation(SingleValueServiceOperationQueryNode serviceOperationNode);

        /// <summary>
        /// Translates a constant node.
        /// </summary>
        /// <param name="constantNode">The constant node to translate.</param>
        /// <returns>The translated expression which evaluates to the constant value.</returns>
        protected virtual Expression TranslateConstant(ConstantQueryNode constantNode)
        {
            ExceptionUtils.CheckArgumentNotNull(constantNode, "constantNode");

            if (constantNode.ResourceType == null)
            {
                Debug.Assert(constantNode.Value == null, "constantNode.Value == null");
                return nullLiteralExpression;
            }
            else
            {
                if (constantNode.ResourceType.ResourceTypeKind != ResourceTypeKind.Primitive)
                {
                    throw new ODataException(Strings.QueryExpressionTranslator_ConstantNonPrimitive(constantNode.ResourceType.FullName));
                }

                return Expression.Constant(constantNode.Value, constantNode.ResourceType.InstanceType);
            }
        }

        /// <summary>
        /// Translates a convert node.
        /// </summary>
        /// <param name="convertNode">The convert node to translate.</param>
        /// <returns>The translated expression which evaluates to the converted value.</returns>
        protected virtual Expression TranslateConvert(ConvertQueryNode convertNode)
        {
            ExceptionUtils.CheckArgumentNotNull(convertNode, "convertNode");
            ExceptionUtils.CheckArgumentNotNull(convertNode.ResourceType, "convertNode.ResourceType");
            
            // TODO: Should we verify that the Source doesn't translate to a wrong type?
            //   In theory if it's a KeyLookup for example it might translate to IQueryable<T> instead of T in which case the Convert should fail
            //   as it's not supported in the path.
            if (convertNode.ResourceType.CanReflectOnInstanceType)
            {
                Expression sourceExpression = this.Translate(convertNode.Source);

                // If we detect a null literal we replace the convert of the null literal with a typed null literal.
                return sourceExpression == nullLiteralExpression
                    ? (Expression)Expression.Constant(null, convertNode.ResourceType.InstanceType)
                    : (Expression)Expression.Convert(sourceExpression, convertNode.ResourceType.InstanceType);
            }
            else
            {
                // TODO: Support for untyped resource types in convert
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Translates a key lookup.
        /// </summary>
        /// <param name="keyLookupNode">The key lookup node to translate.</param>
        /// <returns>The translated expression which evaluates to the result of the key lookup.</returns>
        protected virtual Expression TranslateKeyLookup(KeyLookupQueryNode keyLookupNode)
        {
            ExceptionUtils.CheckArgumentNotNull(keyLookupNode, "keyLookupNode");
            ExceptionUtils.CheckArgumentNotNull(keyLookupNode.Collection, "keyLookupNode.Collection");
            ExceptionUtils.CheckArgumentNotNull(keyLookupNode.Collection.ItemType, "keyLookupNode.Collection.ItemType");
            ExceptionUtils.CheckArgumentNotNull(keyLookupNode.KeyPropertyValues, "keyLookupNode.KeyPropertyValues");

            ResourceType itemType = keyLookupNode.Collection.ItemType;
            if (keyLookupNode.Collection.ItemType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new ODataException(Strings.QueryExpressionTranslator_KeyLookupOnlyOnEntities(keyLookupNode.Collection.ItemType.FullName, keyLookupNode.Collection.ItemType.ResourceTypeKind));
            }

            Expression collectionExpression = this.Translate(keyLookupNode.Collection);
            Type expectedCollectionExpressionType = typeof(IQueryable<>).MakeGenericType(itemType.InstanceType);
            if (!expectedCollectionExpressionType.IsAssignableFrom(collectionExpression.Type))
            {
                throw new ODataException(Strings.QueryExpressionTranslator_KeyLookupOnlyOnQueryable(collectionExpression.Type, expectedCollectionExpressionType));
            }

            ParameterExpression parameter = Expression.Parameter(itemType.InstanceType, "it");
            Expression body = null;

            // We have to walk the key properties as declared on the type to get the declared order (rather than the order in the query)
            // So we need to cache the key properties in a list so that we can look them up. This is necessary to avoid enumerating the IEnumerable
            // specified on the Key lookup node multiple times (since we don't know how costly it is and if it's actually possible).
            List<KeyPropertyValue> keyPropertyValuesCache = new List<KeyPropertyValue>(keyLookupNode.KeyPropertyValues);
            foreach (ResourceProperty keyProperty in itemType.KeyProperties)
            {
                // Find the value for the key property and verify that it's specified exactly once.
                KeyPropertyValue keyPropertyValue = null;
                foreach (KeyPropertyValue candidateKeyPropertyValue in keyPropertyValuesCache.Where(kpv => kpv.KeyProperty == keyProperty))
                {
                    if (keyPropertyValue != null)
                    {
                        throw new ODataException(Strings.QueryExpressionTranslator_KeyLookupWithoutKeyProperty(keyProperty.Name, itemType.FullName));
                    }

                    keyPropertyValue = candidateKeyPropertyValue;
                }

                if (keyPropertyValue == null)
                {
                    throw new ODataException(Strings.QueryExpressionTranslator_KeyLookupWithoutKeyProperty(keyProperty.Name, itemType.FullName));
                }

                if (keyPropertyValue.KeyProperty == null || 
                    !keyPropertyValue.KeyProperty.IsOfKind(ResourcePropertyKind.Key) ||
                    !itemType.KeyProperties.Contains(keyPropertyValue.KeyProperty))
                {
                    throw new ODataException(Strings.QueryExpressionTranslator_KeyPropertyValueWithoutProperty);
                }

                if (keyPropertyValue.KeyValue == null || 
                    keyPropertyValue.KeyValue.ResourceType == null || 
                    keyPropertyValue.KeyValue.ResourceType != keyPropertyValue.KeyProperty.ResourceType)
                {
                    throw new ODataException(Strings.QueryExpressionTranslator_KeyPropertyValueWithWrongValue(keyPropertyValue.KeyProperty.Name));
                }

                Expression keyPropertyAccess = CreatePropertyAccessExpression(parameter, itemType, keyPropertyValue.KeyProperty);

                Expression keyValueExpression = this.Translate(keyPropertyValue.KeyValue);
#if DEBUG
                Debug.Assert(
                    TypeUtils.AreTypesEquivalent(keyPropertyAccess.Type, keyValueExpression.Type), 
                    "The types of the expression should have been checked against the metadata types which are equal.");
#endif

                Expression keyPredicate = Expression.Equal(
                    keyPropertyAccess,
                    keyValueExpression);

                if (body == null)
                {
                    body = keyPredicate;
                }
                else
                {
                    body = Expression.AndAlso(body, keyPredicate);
                }
            }

            if (body == null)
            {
                throw new ODataException(Strings.QueryExpressionTranslator_KeyLookupWithNoKeyValues);
            }

            return Expression.Call(
                typeof(Queryable),
                WhereMethodName,
                new Type[] { itemType.InstanceType },
                collectionExpression,
                Expression.Quote(Expression.Lambda(body, parameter)));
        }

        /// <summary>
        /// Translates a skip operation.
        /// </summary>
        /// <param name="skipNode">The skip node to translate.</param>
        /// <returns>The translated expression which evaluates to the result of the skip operation.</returns>
        protected virtual Expression TranslateSkip(SkipQueryNode skipNode)
        {
            ExceptionUtils.CheckArgumentNotNull(skipNode, "skipNode");
            ExceptionUtils.CheckArgumentNotNull(skipNode.Amount, "skipNode.Amount");

            Expression collectionExpression = this.Translate(skipNode.Collection);
            Expression amountExpression = this.Translate(skipNode.Amount);

            // TODO: eventually we will have to support this on Enumerable as well (as it can happen on expansions)
            return Expression.Call(
                typeof(Queryable),
                SkipMethodName,
                new Type[] { skipNode.Collection.ItemType.InstanceType },
                collectionExpression,
                amountExpression);
        }

        /// <summary>
        /// Translates a top/take operation.
        /// </summary>
        /// <param name="topNode">The top node to translate.</param>
        /// <returns>The translated expression which evaluates to the result of the top operation.</returns>
        protected virtual Expression TranslateTop(TopQueryNode topNode)
        {
            ExceptionUtils.CheckArgumentNotNull(topNode, "topNode");
            ExceptionUtils.CheckArgumentNotNull(topNode.Amount, "topNode.Amount");

            Expression collectionExpression = this.Translate(topNode.Collection);
            Expression amountExpression = this.Translate(topNode.Amount);

            // TODO: eventually we will have to support this on Enumerable as well (as it can happen on expansions)
            return Expression.Call(
                typeof(Queryable),
                TakeMethodName,
                new Type[] { topNode.Collection.ItemType.InstanceType },
                collectionExpression,
                amountExpression);
        }

        /// <summary>
        /// Translates a filter node.
        /// </summary>
        /// <param name="filterNode">The filter node to translate.</param>
        /// <returns>Expression which evaluates to the result after the filter operation.</returns>
        protected virtual Expression TranslateFilter(FilterQueryNode filterNode)
        {
            ExceptionUtils.CheckArgumentNotNull(filterNode, "filterNode");
            ExceptionUtils.CheckArgumentNotNull(filterNode.ItemType, "filterNode.ItemType");
            ExceptionUtils.CheckArgumentNotNull(filterNode.Collection, "filterNode.Collection");
            ExceptionUtils.CheckArgumentNotNull(filterNode.Collection.ItemType, "filterNode.Collection.ItemType");
            ExceptionUtils.CheckArgumentNotNull(filterNode.Expression, "filterNode.Expression");
            ExceptionUtils.CheckArgumentNotNull(filterNode.Parameter, "filterNode.Parameter");
            ExceptionUtils.CheckArgumentNotNull(filterNode.Parameter.ResourceType, "filterNode.Parameter.ResourceType");

            ParameterExpression parameter = Expression.Parameter(filterNode.Parameter.ResourceType.InstanceType, "it");
            Expression collectionExpression = this.Translate(filterNode.Collection);
            
            // TODO: If we should support Filter on IEnumerable, then we need to add support here
            Type expectedCollectionType = typeof(IQueryable<>).MakeGenericType(parameter.Type);
            if (!expectedCollectionType.IsAssignableFrom(collectionExpression.Type))
            {
                throw new ODataException(Strings.QueryExpressionTranslator_FilterCollectionOfWrongType(collectionExpression.Type, expectedCollectionType));
            }

            this.parameterNodeDefinitions.Push(new KeyValuePair<ParameterQueryNode, Expression>(filterNode.Parameter, parameter));
            Expression body = this.Translate(filterNode.Expression);
            Debug.Assert(this.parameterNodeDefinitions.Peek().Key == filterNode.Parameter, "The parameter definition stack was not balanced correctly.");
            Debug.Assert(this.parameterNodeDefinitions.Peek().Value == parameter, "The parameter definition stack was not balanced correctly.");
            this.parameterNodeDefinitions.Pop();

            // TODO: Deal with open expressions
            if (body == nullLiteralExpression)
            {
                // lifting rules say that a null literal is interpreted as 'false' when treated as boolean.
                body = falseLiteralExpression;
            }
            else if (body.Type == typeof(bool?))
            {
                Expression test = Expression.Equal(body, Expression.Constant(null, typeof(bool?)));
                body = Expression.Condition(test, falseLiteralExpression, Expression.Property(body, "Value"));
            }

            if (body.Type != typeof(bool))
            {
                throw new ODataException(Strings.QueryExpressionTranslator_FilterExpressionOfWrongType(body.Type));
            }

            return Expression.Call(
                typeof(Queryable),
                WhereMethodName,
                new Type[] { parameter.Type },
                collectionExpression,
                Expression.Quote(Expression.Lambda(body, parameter)));
        }

        /// <summary>
        /// Translates a binary operator node.
        /// </summary>
        /// <param name="binaryOperatorNode">The binary operator node to translate.</param>
        /// <returns>Expression which evaluates to the result of the binary operator.</returns>
        protected virtual Expression TranslateBinaryOperator(BinaryOperatorQueryNode binaryOperatorNode)
        {
            ExceptionUtils.CheckArgumentNotNull(binaryOperatorNode, "binaryOperatorNode");
            ExceptionUtils.CheckArgumentNotNull(binaryOperatorNode.Left, "binaryOperatorNode.Left");
            ExceptionUtils.CheckArgumentNotNull(binaryOperatorNode.Right, "binaryOperatorNode.Right");

            Expression left = this.Translate(binaryOperatorNode.Left);
            Expression right = this.Translate(binaryOperatorNode.Right);

            // If the operands both statically are null literals we optimize the operator to result in the 
            // null literal as well (per specification) because we otherwise cannot execute such an expression tree
            if (left == nullLiteralExpression && right == nullLiteralExpression)
            {
                // Equality operators have special rules if one (or both) operands are null
                if (binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal)
                {
                    return trueLiteralExpression;
                }
                else if (binaryOperatorNode.OperatorKind == BinaryOperatorKind.NotEqual)
                {
                    return falseLiteralExpression;
                }

                return nullLiteralExpression;
            }

            Debug.Assert(left != nullLiteralExpression && right != nullLiteralExpression, "None of the operands should be the untyped literal null expression at this point.");

            // throw if no resource type is available
            if (binaryOperatorNode.ResourceType == null)
            {
                throw new ODataException(Strings.QueryExpressionTranslator_SingleValueQueryNodeWithoutResourceType(binaryOperatorNode.Kind));
            }

            // Deal with null propagation; after translating the operands to expressions one (or both) can have changed from a non-nullable type to a
            // nullable type if null propagation is enabled; compensate for that.
            if (this.nullPropagationRequired)
            {
                HandleBinaryOperatorNullPropagation(ref left, ref right);
            }
            else
            {
                Debug.Assert(left.Type == right.Type, "left.Type == right.Type");
            }

            // TODO: Deal with open properties
            // See RequestQueryParser.ExpressionParser.GenerateComparisonExpression for the actual complexity required for comparison operators
            switch (binaryOperatorNode.OperatorKind)
            {
                case BinaryOperatorKind.Or:
                    return Expression.OrElse(left, right);
                case BinaryOperatorKind.And:
                    return Expression.AndAlso(left, right);

                case BinaryOperatorKind.Equal:
                    if (left.Type == typeof(byte[]))
                    {
                        return Expression.Equal(left, right, false, DataServiceProviderMethods.AreByteArraysEqualMethodInfo);
                    }
                    else
                    {
                        return Expression.Equal(left, right);
                    }

                case BinaryOperatorKind.NotEqual:
                    if (left.Type == typeof(byte[]))
                    {
                        return Expression.NotEqual(left, right, false, DataServiceProviderMethods.AreByteArraysNotEqualMethodInfo);
                    }
                    else
                    {
                        return Expression.NotEqual(left, right);
                    }

                case BinaryOperatorKind.GreaterThan:
                    return CreateGreaterThanExpression(left, right);
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return CreateGreaterThanOrEqualExpression(left, right);
                case BinaryOperatorKind.LessThan:
                    return CreateLessThanExpression(left, right);
                case BinaryOperatorKind.LessThanOrEqual:
                    return CreateLessThanOrEqualExpression(left, right);

                case BinaryOperatorKind.Add:
                    return Expression.Add(left, right);
                case BinaryOperatorKind.Subtract:
                    return Expression.Subtract(left, right);

                case BinaryOperatorKind.Multiply:
                    return Expression.Multiply(left, right);
                case BinaryOperatorKind.Divide:
                    return Expression.Divide(left, right);
                case BinaryOperatorKind.Modulo:
                    return Expression.Modulo(left, right);

                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.QueryExpressionTranslator_TranslateBinaryOperator_UnreachableCodepath));
            }
        }

        /// <summary>
        /// Translates a unary operator node.
        /// </summary>
        /// <param name="unaryOperatorNode">The unary operator node to translate.</param>
        /// <returns>Expression which evaluates to the result of the unary operator.</returns>
        protected virtual Expression TranslateUnaryOperator(UnaryOperatorQueryNode unaryOperatorNode)
        {
            ExceptionUtils.CheckArgumentNotNull(unaryOperatorNode, "unaryOperatorNode");
            ExceptionUtils.CheckArgumentNotNull(unaryOperatorNode.Operand, "unaryOperatorNode.Operand");

            Expression operand = this.Translate(unaryOperatorNode.Operand);

            // If the operand statically is the null literal we optimize the operator to result in the 
            // null literal as well (per specification) because we otherwise cannot execute such an expression tree
            if (operand == nullLiteralExpression)
            {
                return nullLiteralExpression;
            }

            // throw if no resource type is available
            if (unaryOperatorNode.ResourceType == null)
            {
                throw new ODataException(Strings.QueryExpressionTranslator_SingleValueQueryNodeWithoutResourceType(unaryOperatorNode.Kind));
            }

            // TODO: deal with null propagation if necessary
            // TODO: deal with open types
            switch (unaryOperatorNode.OperatorKind)
            {
                case UnaryOperatorKind.Negate:
                    return Expression.Negate(operand);

                case UnaryOperatorKind.Not:
                    if (operand.Type == typeof(bool) || operand.Type == typeof(bool?))
                    {
                        // Expression.Not will take numerics and apply '~' to them, thus the extra check here.
                        return Expression.Not(operand);
                    }
                    else
                    {
                        throw new ODataException(Strings.QueryExpressionTranslator_UnaryNotOperandNotBoolean(operand.Type));
                    }

                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.QueryExpressionTranslator_TranslateUnaryOperator_UnreachableCodepath));
            }
        }

        /// <summary>
        /// Translates a property access node.
        /// </summary>
        /// <param name="propertyAccessNode">The property access node to translate.</param>
        /// <returns>Expression which evaluates to the result of the property access.</returns>
        protected virtual Expression TranslatePropertyAccess(PropertyAccessQueryNode propertyAccessNode)
        {
            ExceptionUtils.CheckArgumentNotNull(propertyAccessNode, "propertyAccessNode");
            ExceptionUtils.CheckArgumentNotNull(propertyAccessNode.Source, "propertyAccessNode.Source");
            ExceptionUtils.CheckArgumentNotNull(propertyAccessNode.Source.ResourceType, "propertyAccessNode.Source.ResourceType");
            ExceptionUtils.CheckArgumentNotNull(propertyAccessNode.Property, "propertyAccessNode.Property");

            Expression source = this.Translate(propertyAccessNode.Source);
            if (!propertyAccessNode.Source.ResourceType.InstanceType.IsAssignableFrom(source.Type))
            {
                throw new ODataException(Strings.QueryExpressionTranslator_PropertyAccessSourceWrongType(source.Type, propertyAccessNode.Source.ResourceType.InstanceType));
            }

            if (propertyAccessNode.Property.CanReflectOnInstanceTypeProperty)
            {
                Debug.Assert(source != nullLiteralExpression, "source != nullLiteralExpression");

                Expression propertyAccessExpression = Expression.Property(source, propertyAccessNode.Source.ResourceType.GetPropertyInfo(propertyAccessNode.Property));

                // Add null propagation code if the parent is not the parameter node (which will never be null) and is nullable
                if (this.nullPropagationRequired && source.NodeType != ExpressionType.Parameter && TypeUtils.TypeAllowsNull(source.Type))
                {
                    Expression test = Expression.Equal(source, Expression.Constant(null, source.Type));
                    Type propagatedType = TypeUtils.GetNullableType(propertyAccessExpression.Type);

                    if (propagatedType != propertyAccessExpression.Type)
                    {
                        // we need to convert the property access result to the propagated type
                        propertyAccessExpression = Expression.Convert(propertyAccessExpression, propagatedType);
                    }

                    propertyAccessExpression = Expression.Condition(test, Expression.Constant(null, propagatedType), propertyAccessExpression);
                }

                return propertyAccessExpression;
            }
            else
            {
                // TODO: support for untyped and open properties
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Translates a parameter node.
        /// </summary>
        /// <param name="parameterNode">The parameter node to translate.</param>
        /// <returns>Expression which evaluates to the result of the parameter.</returns>
        protected virtual Expression TranslateParameter(ParameterQueryNode parameterNode)
        {
            ExceptionUtils.CheckArgumentNotNull(parameterNode, "parameterNode");

            if (this.parameterNodeDefinitions.Count == 0)
            {
                throw new ODataException(Strings.QueryExpressionTranslator_ParameterNotDefinedInScope);
            }

            KeyValuePair<ParameterQueryNode, Expression> currentParameter = this.parameterNodeDefinitions.Peek();
            if (!object.ReferenceEquals(currentParameter.Key, parameterNode))
            {
                throw new ODataException(Strings.QueryExpressionTranslator_ParameterNotDefinedInScope);
            }

            return currentParameter.Value;
        }

        /// <summary>
        /// Translates an orderby node.
        /// </summary>
        /// <param name="orderByNode">The orderby node to translate.</param>
        /// <returns>Expression which evaluates to the result after the order operation.</returns>
        protected virtual Expression TranslateOrderBy(OrderByQueryNode orderByNode)
        {
            ExceptionUtils.CheckArgumentNotNull(orderByNode, "orderByNode");
            ExceptionUtils.CheckArgumentNotNull(orderByNode.ItemType, "orderByNode.ItemType");
            ExceptionUtils.CheckArgumentNotNull(orderByNode.Collection, "orderByNode.Collection");
            ExceptionUtils.CheckArgumentNotNull(orderByNode.Collection.ItemType, "orderByNode.Collection.ItemType");
            ExceptionUtils.CheckArgumentNotNull(orderByNode.Expression, "orderByNode.Expression");
            ExceptionUtils.CheckArgumentNotNull(orderByNode.Parameter, "orderByNode.Parameter");
            ExceptionUtils.CheckArgumentNotNull(orderByNode.Parameter.ResourceType, "orderByNode.Parameter.ResourceType");

            ParameterExpression parameter = Expression.Parameter(orderByNode.Parameter.ResourceType.InstanceType, "element");
            Expression collectionExpression = this.Translate(orderByNode.Collection);

            // TODO: If we should support OrderBy on IEnumerable, then we need to add support here
            Type expectedCollectionType = typeof(IQueryable<>).MakeGenericType(parameter.Type);
            if (!expectedCollectionType.IsAssignableFrom(collectionExpression.Type))
            {
                throw new ODataException(Strings.QueryExpressionTranslator_OrderByCollectionOfWrongType(collectionExpression.Type, expectedCollectionType));
            }

            this.parameterNodeDefinitions.Push(new KeyValuePair<ParameterQueryNode, Expression>(orderByNode.Parameter, parameter));
            Expression body = this.Translate(orderByNode.Expression);
            Debug.Assert(this.parameterNodeDefinitions.Peek().Key == orderByNode.Parameter, "The parameter definition stack was not balanced correctly.");
            Debug.Assert(this.parameterNodeDefinitions.Peek().Value == parameter, "The parameter definition stack was not balanced correctly.");
            this.parameterNodeDefinitions.Pop();

            // If the collectionExpression is already another OrderBy or ThenBy we need to use a ThenBy
            string methodName = orderByNode.Direction == OrderByDirection.Ascending ? OrderByMethodName : OrderByDescendingMethodName;
            if (collectionExpression.NodeType == ExpressionType.Call)
            {
                MethodCallExpression collectionMethodCallExpression = (MethodCallExpression)collectionExpression;
                if (collectionMethodCallExpression.Method.DeclaringType == typeof(Queryable) &&
                    collectionMethodCallExpression.Method.Name == OrderByMethodName ||
                    collectionMethodCallExpression.Method.Name == OrderByDescendingMethodName ||
                    collectionMethodCallExpression.Method.Name == ThenByMethodName ||
                    collectionMethodCallExpression.Method.Name == ThenByDescendingMethodName)
                {
                    methodName = orderByNode.Direction == OrderByDirection.Ascending ? ThenByMethodName : ThenByDescendingMethodName;
                }
            }

            return Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { parameter.Type, body.Type },
                collectionExpression,
                Expression.Quote(Expression.Lambda(body, parameter)));
        }

        /// <summary>
        /// Translates a function call which returns a single value.
        /// </summary>
        /// <param name="functionCallNode">The function call node to translate.</param>
        /// <returns>The translated expression which evaluates to the result of the function call.</returns>
        protected virtual Expression TranslateSingleValueFunctionCall(SingleValueFunctionCallQueryNode functionCallNode)
        {
            ExceptionUtils.CheckArgumentNotNull(functionCallNode, "functionCallNode");
            ExceptionUtils.CheckArgumentNotNull(functionCallNode.Name, "functionCallNode.Name");

            // First check that all arguments are single nodes and build a list so that we can go over them more than once.
            List<SingleValueQueryNode> argumentNodes = new List<SingleValueQueryNode>();
            if (functionCallNode.Arguments != null)
            {
                foreach (QueryNode argumentNode in functionCallNode.Arguments)
                {
                    SingleValueQueryNode singleValueArgumentNode = argumentNode as SingleValueQueryNode;
                    if (singleValueArgumentNode == null)
                    {
                        throw new ODataException(Strings.QueryExpressionTranslator_FunctionArgumentNotSingleValue(functionCallNode.Name));
                    }

                    argumentNodes.Add(singleValueArgumentNode);
                }
            }

            // Find the exact matching signature
            BuiltInFunctionSignature[] signatures;
            if (!BuiltInFunctions.TryGetBuiltInFunction(functionCallNode.Name, out signatures))
            {
                throw new ODataException(Strings.QueryExpressionTranslator_UnknownFunction(functionCallNode.Name));
            }

            BuiltInFunctionSignature signature =
                (BuiltInFunctionSignature)TypePromotionUtils.FindExactFunctionSignature(
                    signatures,
                    argumentNodes.Select(argumentNode => argumentNode.ResourceType).ToArray());
            if (signature == null)
            {
                throw new ODataException(
                    Strings.QueryExpressionTranslator_NoApplicableFunctionFound(
                        functionCallNode.Name,
                        BuiltInFunctions.BuildFunctionSignatureListDescription(functionCallNode.Name, signatures)));
            }

            // Translate all arguments into expressions
            Expression[] argumentExpressions = new Expression[argumentNodes.Count];
            Expression[] nonNullableArgumentExpression = new Expression[argumentNodes.Count];
            for (int argumentIndex = 0; argumentIndex < argumentNodes.Count; argumentIndex++)
            {
                SingleValueQueryNode argumentNode = argumentNodes[argumentIndex];
                Expression argumentExpression = this.Translate(argumentNode);
                argumentExpressions[argumentIndex] = argumentExpression;

                // The below code relies on the fact that all built-in functions do not take nullable parameters (in CLR space)
                // so if we do see a nullable as one of the parameters, we cast it down to non-nullable
                // If null propagation is on, we may deal with the nullable aspect later on by injecting conditionals around the function call
                // which handle the null, or if null propagation is of, passing the potential null is OK since the LINQ provider is expected
                // to handle such cases then.
                if (TypeUtils.IsNullableType(argumentExpression.Type))
                {
                    nonNullableArgumentExpression[argumentIndex] = Expression.Convert(argumentExpression, TypeUtils.GetNonNullableType(argumentExpression.Type));
                }
                else
                {
                    nonNullableArgumentExpression[argumentIndex] = argumentExpression;
                }
            }

            // Build the function call expression (as if null propagation is not required and nulls can be handled anywhere)
            Expression functionCallExpression = signature.BuildExpression(nonNullableArgumentExpression);

            // Apply the null propagation
            // Note that we inject null propagation on all arguments, which means we assume that if any of the arguments passed to function
            // is null, the entire function is null. This is true for all the built-in functions for now. If this would be to change we would have to review
            // the following code.
            if (this.nullPropagationRequired)
            {
                for (int argumentIndex = 0; argumentIndex < argumentExpressions.Length; argumentIndex++)
                {
                    Expression argumentExpression = argumentExpressions[argumentIndex];

                    // Don't null propagate the lambda parameter since it will never be null
                    //   or if the argument expression can't be null due to its type.
                    if (argumentExpression == this.parameterNodeDefinitions.Peek().Value || !TypeUtils.TypeAllowsNull(argumentExpression.Type))
                    {
                        continue;
                    }

                    // Tiny optimization: remove the check on constants which are known not to be null.
                    // Otherwise every string literal propagates out, which is correct but unnecessarily messy.
                    ConstantExpression constantExpression = argumentExpression as ConstantExpression;
                    if (constantExpression != null && constantExpression.Value != null)
                    {
                        continue;
                    }

                    Expression test = Expression.Equal(argumentExpression, Expression.Constant(null, argumentExpression.Type));
                    Expression falseBranch = functionCallExpression;
                    if (!TypeUtils.TypeAllowsNull(falseBranch.Type))
                    {
                        falseBranch = Expression.Convert(falseBranch, typeof(Nullable<>).MakeGenericType(falseBranch.Type));
                    }

                    Expression trueBranch = Expression.Constant(null, falseBranch.Type);
                    functionCallExpression = Expression.Condition(test, trueBranch, falseBranch);
                }
            }

            return functionCallExpression;
        }

        /// <summary>
        /// Creates an expression to access a property.
        /// </summary>
        /// <param name="source">The source expression which evaluates to the instance to access the property on.</param>
        /// <param name="sourceResourceType">The resource type of the source expression.</param>
        /// <param name="resourceProperty">The resource property to access.</param>
        /// <returns>An expression which evaluates to the property value.</returns>
        private static Expression CreatePropertyAccessExpression(Expression source, ResourceType sourceResourceType, ResourceProperty resourceProperty)
        {
            Debug.Assert(source != null, "source != null");
            Debug.Assert(sourceResourceType != null, "sourceResourceType != null");
            Debug.Assert(resourceProperty != null, "resourceProperty != null");
            Debug.Assert(sourceResourceType.Properties.Contains(resourceProperty), "resourceProperty is not declared on sourceResourceType");
#if DEBUG
            Debug.Assert(TypeUtils.AreTypesEquivalent(source.Type, sourceResourceType.InstanceType), "source.Type != sourceResourceType.InstanceType");
#endif

            // TODO: Deal with null propagation???
            if (resourceProperty.CanReflectOnInstanceTypeProperty)
            {
                return Expression.Property(source, sourceResourceType.GetPropertyInfo(resourceProperty));
            }
            else
            {
                // TODO: Support for untyped and open properties
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Creates a greater then expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>The result expression.</returns>
        private static Expression CreateGreaterThanExpression(Expression left, Expression right)
        {
            Debug.Assert(left != null, "left != null");
            Debug.Assert(right != null, "right != null");
            
            // TODO: Deal with open types
            MethodInfo comparisonMethodInfo = GetComparisonMethodInfo(left.Type);
            if (comparisonMethodInfo != null)
            {
                left = Expression.Call(null, comparisonMethodInfo, left, right);
                right = Expression.Constant(0, typeof(int));
            }

            return Expression.GreaterThan(left, right);
        }

        /// <summary>
        /// Creates a greater then or equal expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>The result expression.</returns>
        private static Expression CreateGreaterThanOrEqualExpression(Expression left, Expression right)
        {
            Debug.Assert(left != null, "left != null");
            Debug.Assert(right != null, "right != null");

            // TODO: Deal with open types
            MethodInfo comparisonMethodInfo = GetComparisonMethodInfo(left.Type);
            if (comparisonMethodInfo != null)
            {
                left = Expression.Call(null, comparisonMethodInfo, left, right);
                right = Expression.Constant(0, typeof(int));
            }

            return Expression.GreaterThanOrEqual(left, right);
        }

        /// <summary>
        /// Creates a less then expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>The result expression.</returns>
        private static Expression CreateLessThanExpression(Expression left, Expression right)
        {
            Debug.Assert(left != null, "left != null");
            Debug.Assert(right != null, "right != null");

            // TODO: Deal with open types
            MethodInfo comparisonMethodInfo = GetComparisonMethodInfo(left.Type);
            if (comparisonMethodInfo != null)
            {
                left = Expression.Call(null, comparisonMethodInfo, left, right);
                right = Expression.Constant(0, typeof(int));
            }

            return Expression.LessThan(left, right);
        }

        /// <summary>
        /// Creates a less then or equal expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>The result expression.</returns>
        private static Expression CreateLessThanOrEqualExpression(Expression left, Expression right)
        {
            Debug.Assert(left != null, "left != null");
            Debug.Assert(right != null, "right != null");

            // TODO: Deal with open types
            MethodInfo comparisonMethodInfo = GetComparisonMethodInfo(left.Type);
            if (comparisonMethodInfo != null)
            {
                left = Expression.Call(null, comparisonMethodInfo, left, right);
                right = Expression.Constant(0, typeof(int));
            }

            return Expression.LessThanOrEqual(left, right);
        }

        /// <summary>
        /// Determine which method to use as the implementation for a comparison operator on a given type.
        /// </summary>
        /// <param name="type">The type the comparison is working on. This should be a primitive type.</param>
        /// <returns>A method info for the method to use as the implementation method for the comparison operator.</returns>
        private static MethodInfo GetComparisonMethodInfo(Type type)
        {
            Debug.Assert(type != null, "type != null");

            if (type == typeof(string))
            {
                return DataServiceProviderMethods.StringCompareMethodInfo;
            }
            else if (type == typeof(bool))
            {
                return DataServiceProviderMethods.BoolCompareMethodInfo;
            }
            else if (type == typeof(bool?))
            {
                return DataServiceProviderMethods.BoolCompareMethodInfoNullable;
            }
            else if (type == typeof(Guid))
            {
                return DataServiceProviderMethods.GuidCompareMethodInfo;
            }
            else if (type == typeof(Guid?))
            {
                return DataServiceProviderMethods.GuidCompareMethodInfoNullable;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// After processing the operands of a binary operator, one or both of them may have changed their
        /// type from a non-nullable type to a nullable type because of null propagation.
        /// This method lifts the operands to the nullable type as required.
        /// </summary>
        /// <param name="left">The left operand of the binary operator.</param>
        /// <param name="right">The right operand of the binary operator.</param>
        private static void HandleBinaryOperatorNullPropagation(ref Expression left, ref Expression right)
        {
            Debug.Assert(left != null, "left != null");
            Debug.Assert(right != null, "right != null");

            if (left.Type != right.Type)
            {
                if (TypeUtils.IsNullableType(left.Type))
                {
                    right = Expression.Convert(right, TypeUtils.GetNullableType(right.Type));
                }
                else if (TypeUtils.IsNullableType(right.Type))
                {
                    left = Expression.Convert(left, TypeUtils.GetNullableType(left.Type));
                }
                else
                {
                    Debug.Assert(!TypeUtils.IsNullableType(right.Type) && !TypeUtils.IsNullableType(left.Type), "None of the operands is expected to be nullable.");
                }
            }
        }
    }
}
