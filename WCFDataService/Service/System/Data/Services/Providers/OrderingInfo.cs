//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
