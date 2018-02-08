//---------------------------------------------------------------------
// <copyright file="ExpandTermToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    #region Namespaces
    using System.Collections.Generic;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing an expand operation.
    /// </summary>
    public sealed class ExpandTermToken : QueryToken
    {
        /// <summary>
        /// The nav prop path for this ExpandTerm
        /// </summary>
        private readonly PathSegmentToken pathToNavigationProp;

        /// <summary>
        /// the filter option for this expand term
        /// </summary>
        private readonly QueryToken filterOption;

        /// <summary>
        /// the order by options for this expand term
        /// </summary>
        private readonly IEnumerable<OrderByToken> orderByOptions;

        /// <summary>
        /// the top option for this expand term
        /// </summary>
        private readonly long? topOption;

        /// <summary>
        /// the skip option for this expand term.
        /// </summary>
        private readonly long? skipOption;

        /// <summary>
        /// the query count option for this expand term.
        /// </summary>
        private readonly bool? countQueryOption;

        /// <summary>
        /// the levels option for this expand term
        /// </summary>
        private readonly long? levelsOption;

        /// <summary>
        /// the search option for this expand term
        /// </summary>
        private readonly QueryToken searchOption;

        /// <summary>
        /// the select option for this expand term.
        /// </summary>
        private readonly SelectToken selectOption;

        /// <summary>
        /// the compute option for this expand term.
        /// </summary>
        private readonly ComputeToken computeOption;

        /// <summary>
        /// the expand option for this expand term.
        /// </summary>
        private readonly ExpandToken expandOption;

        /// <summary>
        /// Create an expand term token using only a property
        /// </summary>
        /// <param name="pathToNavigationProp">the path to the navigation property</param>
        public ExpandTermToken(PathSegmentToken pathToNavigationProp)
            : this(pathToNavigationProp, null, null)
        {
        }

        /// <summary>
        /// Create an expand term using only the property and its subexpand/select
        /// </summary>
        /// <param name="pathToNavigationProp">the path to the navigation property for this expand term</param>
        /// <param name="selectOption">the sub select for this token</param>
        /// <param name="expandOption">the sub expand for this token</param>
        public ExpandTermToken(PathSegmentToken pathToNavigationProp, SelectToken selectOption, ExpandToken expandOption)
            : this(pathToNavigationProp, null, null, null, null, null, null, null, selectOption, expandOption)
        {
        }

        /// <summary>
        /// Create an expand term token
        /// </summary>
        /// <param name="pathToNavigationProp">the nav prop for this expand term</param>
        /// <param name="filterOption">the filter option for this expand term</param>
        /// <param name="orderByOptions">the orderby options for this expand term</param>
        /// <param name="topOption">the top option for this expand term</param>
        /// <param name="skipOption">the skip option for this expand term</param>
        /// <param name="countQueryOption">the query count option for this expand term</param>
        /// <param name="levelsOption">the levels option for this expand term</param>
        /// <param name="searchOption">the search option for this expand term</param>
        /// <param name="selectOption">the select option for this expand term</param>
        /// <param name="expandOption">the expand option for this expand term</param>
        public ExpandTermToken(PathSegmentToken pathToNavigationProp, QueryToken filterOption, IEnumerable<OrderByToken> orderByOptions, long? topOption, long? skipOption, bool? countQueryOption, long? levelsOption, QueryToken searchOption, SelectToken selectOption, ExpandToken expandOption)
            : this(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, levelsOption, searchOption, selectOption, expandOption, null)
        {
        }

        /// <summary>
        /// Create an expand term token
        /// </summary>
        /// <param name="pathToNavigationProp">the nav prop for this expand term</param>
        /// <param name="filterOption">the filter option for this expand term</param>
        /// <param name="orderByOptions">the orderby options for this expand term</param>
        /// <param name="topOption">the top option for this expand term</param>
        /// <param name="skipOption">the skip option for this expand term</param>
        /// <param name="countQueryOption">the query count option for this expand term</param>
        /// <param name="levelsOption">the levels option for this expand term</param>
        /// <param name="searchOption">the search option for this expand term</param>
        /// <param name="selectOption">the select option for this expand term</param>
        /// <param name="expandOption">the expand option for this expand term</param>
        /// <param name="computeOption">the compute option for this expand term.</param>
        public ExpandTermToken(
            PathSegmentToken pathToNavigationProp,
            QueryToken filterOption,
            IEnumerable<OrderByToken> orderByOptions,
            long? topOption,
            long? skipOption,
            bool? countQueryOption,
            long? levelsOption,
            QueryToken searchOption,
            SelectToken selectOption,
            ExpandToken expandOption,
            ComputeToken computeOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavigationProp, "property");

            this.pathToNavigationProp = pathToNavigationProp;
            this.filterOption = filterOption;
            this.orderByOptions = orderByOptions;
            this.topOption = topOption;
            this.skipOption = skipOption;
            this.countQueryOption = countQueryOption;
            this.levelsOption = levelsOption;
            this.searchOption = searchOption;
            this.selectOption = selectOption;
            this.computeOption = computeOption;
            this.expandOption = expandOption;
        }

        /// <summary>
        /// the nav property for this expand term
        /// </summary>
        public PathSegmentToken PathToNavigationProp
        {
            get { return this.pathToNavigationProp; }
        }

        /// <summary>
        /// The filter option for this expand term.
        /// </summary>
        public QueryToken FilterOption
        {
            get { return this.filterOption; }
        }

        /// <summary>
        /// the orderby options for this expand term.
        /// </summary>
        public IEnumerable<OrderByToken> OrderByOptions
        {
            get { return this.orderByOptions; }
        }

        /// <summary>
        /// the top option for this expand term.
        /// </summary>
        public long? TopOption
        {
            get { return this.topOption; }
        }

        /// <summary>
        /// the skip option for this expand term.
        /// </summary>
        public long? SkipOption
        {
            get { return this.skipOption; }
        }

        /// <summary>
        /// the query count option for this expand term.
        /// </summary>
        public bool? CountQueryOption
        {
            get { return this.countQueryOption; }
        }

        /// <summary>
        /// the levels option for this expand term.
        /// </summary>
        public long? LevelsOption
        {
            get { return this.levelsOption; }
        }

        /// <summary>
        /// the search option for this expand term.
        /// </summary>
        public QueryToken SearchOption
        {
            get { return this.searchOption; }
        }

        /// <summary>
        /// the select option for this expand term.
        /// </summary>
        public SelectToken SelectOption
        {
            get { return this.selectOption; }
        }

        /// <summary>
        /// the compute option for this expand term.
        /// </summary>
        public ComputeToken ComputeOption
        {
            get { return this.computeOption; }
        }

        /// <summary>
        /// the expand option for this expand term.
        /// </summary>
        public ExpandToken ExpandOption
        {
            get { return this.expandOption; }
        }

        /// <summary>
        /// the kind of this expand term.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.ExpandTerm; }
        }

        /// <summary>
        /// Implement the visitor for this Token
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="visitor">A tree visitor that will visit this node.</param>
        /// <returns>Determined by the return type of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
