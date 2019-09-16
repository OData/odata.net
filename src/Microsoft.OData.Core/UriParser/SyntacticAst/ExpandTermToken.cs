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
    public sealed class ExpandTermToken : SelectExpandTermToken
    {
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
            : this(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, levelsOption, searchOption, selectOption, expandOption, computeOption, null)
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
        /// <param name="applyOptions">the apply options for this expand term.</param>
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
            ComputeToken computeOption,
            IEnumerable<QueryToken> applyOptions)
            : base(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, searchOption, selectOption, computeOption)
        {
            ExpandOption = expandOption;
            LevelsOption = levelsOption;
            ApplyOptions = applyOptions;
        }

        /// <summary>
        /// Gets the navigation property for this expand term.
        /// </summary>
        public PathSegmentToken PathToNavigationProp
        {
            get
            {
                return PathToProperty;
            }
        }

        /// <summary>
        /// Gets the expand option for this select or expand term.
        /// </summary>
        public ExpandToken ExpandOption { get; internal set; }

        /// <summary>
        /// Gets the levels option for this expand term.
        /// </summary>
        public long? LevelsOption { get; private set; }

        /// <summary>
        /// Gets the apply options for this expand term.
        /// </summary>
        public IEnumerable<QueryToken> ApplyOptions { get; private set; }

        /// <summary>
        /// Gets the kind of this expand term.
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
