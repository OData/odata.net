//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
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
