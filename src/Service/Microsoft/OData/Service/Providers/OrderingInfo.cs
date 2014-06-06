//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
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
