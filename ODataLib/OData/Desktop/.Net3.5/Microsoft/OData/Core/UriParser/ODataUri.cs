//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;

    #endregion Namespaces

    /// <summary>
    /// The root node of a query. Holds the query itself plus additional metadata about the query.
    /// </summary>
    internal sealed class ODataUri
    {
        /// <summary>
        /// The top level path for this Uri.
        /// </summary>
        private readonly ODataPath path;

        /// <summary>
        /// Any custom query options for this Uri.
        /// </summary>
        private readonly IEnumerable<QueryNode> customQueryOptions;

        /// <summary>
        /// Any select or expand options in this uri. Can be null.
        /// </summary>
        private readonly SelectExpandClause selectAndExpand;

        /// <summary>
        /// Any filter option in this uri. Can be null.
        /// </summary>
        private readonly FilterClause filter;

        /// <summary>
        /// Any order by option in this uri. Can be null.
        /// </summary>
        private readonly OrderByClause orderBy;

        /// <summary>
        /// Any skip option in this uri. Can be null.
        /// </summary>
        private readonly long? skip;

        /// <summary>
        /// Any top option in this uri. Can be null.
        /// </summary>
        private readonly long? top;

        /// <summary>
        /// Query count option in this uri. true or false. Can be null.
        /// </summary>
        private readonly bool? queryCount;

        /// <summary>
        /// Create a new ODataUri. This contains the semantic meaning of the 
        /// entire uri.
        /// </summary>
        /// <param name="path">The top level path for this uri.</param>
        /// <param name="customQueryOptions">Any custom query options for this uri. Can be null.</param>
        /// <param name="selectAndExpand">Any $select or $expand option for this uri. Can be null.</param>
        /// <param name="filter">Any $filter option for this uri. Can be null.</param>
        /// <param name="orderby">Any $orderby option for this uri. Can be null</param>
        /// <param name="skip">Any $skip option for this uri. Can be null.</param>
        /// <param name="top">Any $top option for this uri. Can be null.</param>
        /// <param name="queryCount">Any query $count option for this uri. Can be null.</param>
        public ODataUri(
            ODataPath path,
            IEnumerable<QueryNode> customQueryOptions,
            SelectExpandClause selectAndExpand,
            FilterClause filter,
            OrderByClause orderby,
            long? skip,
            long? top,
            bool? queryCount)
        {
            this.path = path;
            this.customQueryOptions = new ReadOnlyCollection<QueryNode>(customQueryOptions.ToList());
            this.selectAndExpand = selectAndExpand;
            this.filter = filter;
            this.orderBy = orderby;
            this.skip = skip;
            this.top = top;
            this.queryCount = queryCount;
        }

        /// <summary>
        /// Gets the top level path for this uri.
        /// </summary>
        public ODataPath Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// Gets any custom query options for this uri. 
        /// </summary>
        public IEnumerable<QueryNode> CustomQueryOptions
        {
            get { return this.customQueryOptions; }
        }

        /// <summary>
        /// Gets any $select or $expand option for this uri.
        /// </summary>
        public SelectExpandClause SelectAndExpand
        {
            get { return this.selectAndExpand; }
        }

        /// <summary>
        /// Gets any $filter option for this uri.
        /// </summary>
        public FilterClause Filter
        {
            get { return this.filter; }
        }

        /// <summary>
        /// Gets any $orderby option for this uri.
        /// </summary>
        public OrderByClause OrderBy
        {
            get { return this.orderBy; }
        }

        /// <summary>
        /// Gets any $skip option for this uri.
        /// </summary>
        public long? Skip
        {
            get { return this.skip; }
        }

        /// <summary>
        /// Gets any $top option for this uri.
        /// </summary>
        public long? Top
        {
            get { return this.top; }
        }

        /// <summary>
        /// Get any query $count option for this uri.
        /// </summary>
        public bool? QueryCount
        {
            get { return this.queryCount; }
        }
    }
}
