//---------------------------------------------------------------------
// <copyright file="OrderByBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class to handle the binding of orderby tokens.
    /// </summary>
    internal sealed class OrderByBinder
    {
        /// <summary>
        /// Method to use to visit the token tree and bind the tokens recursively.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// Creates an OrderByBinder
        /// </summary>
        /// <param name="bindMethod">Method to use to visit the token tree and bind the tokens recursively.</param>
        internal OrderByBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            ExceptionUtils.CheckArgumentNotNull(bindMethod, "bindMethod");
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Processes the order-by tokens of a entityCollection (if any).
        /// </summary>
        /// <param name="state">State to use for binding.</param>
        /// <param name="orderByTokens">The order-by tokens to bind.</param>
        /// <returns>An OrderByClause representing the orderby statements expressed in the tokens.</returns>
        internal OrderByClause BindOrderBy(BindingState state, IEnumerable<OrderByToken> orderByTokens)
        {
            ExceptionUtils.CheckArgumentNotNull(state, "state");
            ExceptionUtils.CheckArgumentNotNull(orderByTokens, "orderByTokens");

            OrderByClause orderByClause = null;

            // Go through the orderby tokens starting from the last one
            foreach (OrderByToken orderByToken in orderByTokens.Reverse())
            {
                orderByClause = this.ProcessSingleOrderBy(state, orderByClause, orderByToken);
            }

            return orderByClause;
        }

        /// <summary>
        /// Processes the specified order-by token.
        /// </summary>
        /// <param name="state">State to use for binding.</param>
        /// <param name="thenBy"> The next OrderBy node, or null if there is no orderby after this.</param>
        /// <param name="orderByToken">The order-by token to bind.</param>
        /// <returns>Returns the combined entityCollection including the ordering.</returns>
        private OrderByClause ProcessSingleOrderBy(BindingState state, OrderByClause thenBy, OrderByToken orderByToken)
        {
            ExceptionUtils.CheckArgumentNotNull(state, "state");
            ExceptionUtils.CheckArgumentNotNull(orderByToken, "orderByToken");

            QueryNode expressionNode = this.bindMethod(orderByToken.Expression);

            // The order-by expressions need to be primitive / enumeration types
            SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
            if (expressionResultNode == null ||
                (expressionResultNode.TypeReference != null && !expressionResultNode.TypeReference.IsODataPrimitiveTypeKind() && !expressionResultNode.TypeReference.IsODataEnumTypeKind() && !expressionResultNode.TypeReference.IsODataTypeDefinitionTypeKind()))
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_OrderByExpressionNotSingleValue);
            }

            OrderByClause orderByNode = new OrderByClause(
                thenBy,
                expressionResultNode,
                orderByToken.Direction,
                state.ImplicitRangeVariable);

            return orderByNode;
        }
    }
}