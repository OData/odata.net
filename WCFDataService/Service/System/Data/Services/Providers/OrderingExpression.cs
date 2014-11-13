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

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Linq.Expressions;
    #endregion

    /// <summary>
    /// Describes a single ordering expression along with sort order
    /// </summary>
    internal sealed class OrderingExpression
    {
        /// <summary>Ordering expression</summary>
        private readonly Expression orderingExpression;

        /// <summary>Order is ascending or descending</summary>
        private readonly bool isAscending;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderingExpression">Expression for ordering</param>
        /// <param name="isAscending">Order by ascending or descending</param>
        public OrderingExpression(Expression orderingExpression, bool isAscending)
        {
            this.orderingExpression = orderingExpression;
            this.isAscending = isAscending;
        }

        /// <summary>Ordering expression</summary>
        public Expression Expression
        {
            get
            {
                return this.orderingExpression;
            }
        }

        /// <summary>Ascending or descending</summary>
        public bool IsAscending
        {
            get
            {
                return this.isAscending;
            }
        }
    }
}
