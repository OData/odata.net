//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Query
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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

            // TODO: shall we really restrict order-by expressions to primitive types?
            SingleValueNode expressionResultNode = expressionNode as SingleValueNode;
            if (expressionResultNode == null ||
                (expressionResultNode.TypeReference != null && !expressionResultNode.TypeReference.IsODataPrimitiveTypeKind()))
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
