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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    #endregion

    /// <summary>
    /// Describes ordering information for each entity set
    /// for $expand request for a WCF Data Service.
    /// </summary>
    internal sealed class OrderingInfo
    {
        /// <summary>Is the expanded entity set paged</summary>
        private readonly bool paged;

        /// <summary>Collection of ordering expressions</summary>
        private readonly List<OrderingExpression> orderingExpressions;

        /// <summary>Constructor</summary>
        /// <param name="paged">Whether top level entity set is paged</param>
        internal OrderingInfo(bool paged)
        {
            this.paged = paged;
            this.orderingExpressions = new List<OrderingExpression>();
        }

        /// <summary>Is the expaded entity set paged</summary>
        public bool IsPaged
        {
            get
            {
                return this.paged;
            }
        }

        /// <summary>Gives the collection of ordering expressions for a request</summary>
        public ReadOnlyCollection<OrderingExpression> OrderingExpressions
        {
            get
            {
                return this.orderingExpressions.AsReadOnly();
            }
        }

        /// <summary>Adds a single OrderingExpression to the collection</summary>
        /// <param name="orderingExpression">Ordering expression to add</param>
        internal void Add(OrderingExpression orderingExpression)
        {
            this.orderingExpressions.Add(orderingExpression);
        }

        /// <summary>Adds multiple OrderingExpressions to the collection</summary>
        /// <param name="expressions">Ordering expressions to add</param>
        internal void AddRange(IEnumerable<OrderingExpression> expressions)
        {
            this.orderingExpressions.AddRange(expressions);
        }
    }
}
