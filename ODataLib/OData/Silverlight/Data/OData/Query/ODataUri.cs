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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Data.OData.Query.SemanticAst;
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
        /// Any inline count option in this uri. Can be null.
        /// </summary>
        private readonly InlineCountKind? inlineCount;

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
        /// <param name="inlineCount">Any $inlinecount option for this uri. Can be null.</param>
        public ODataUri(
            ODataPath path,
            IEnumerable<QueryNode> customQueryOptions,
            SelectExpandClause selectAndExpand,
            FilterClause filter,
            OrderByClause orderby,
            long? skip,
            long? top,
            InlineCountKind? inlineCount)
        {
            this.path = path;
            this.customQueryOptions = new ReadOnlyCollection<QueryNode>(customQueryOptions.ToList());
            this.selectAndExpand = selectAndExpand;
            this.filter = filter;
            this.orderBy = orderby;
            this.skip = skip;
            this.top = top;
            this.inlineCount = inlineCount;
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
        /// Get any $inlinecount option for this uri.
        /// </summary>
        public InlineCountKind? InlineCount
        {
            get { return this.inlineCount; }
        }
    }
}
