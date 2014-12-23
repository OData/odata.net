//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
