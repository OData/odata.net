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
