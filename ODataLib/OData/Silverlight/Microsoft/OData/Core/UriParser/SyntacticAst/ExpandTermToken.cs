//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing an expand operation.
    /// </summary>
    internal sealed class ExpandTermToken : QueryToken
    {
        /// <summary>
        /// The nav prop path for this ExpandTerm
        /// </summary>
        private readonly PathSegmentToken pathToNavProp;

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
        /// the expand option for this expand term.
        /// </summary>
        private readonly ExpandToken expandOption;

        /// <summary>
        /// Create an expand term token using only a property
        /// </summary>
        /// <param name="pathToNavProp">the path to the navigation property</param>
        public ExpandTermToken(PathSegmentToken pathToNavProp)
            : this(pathToNavProp, null, null)
        {
        }

        /// <summary>
        /// Create an expand term using only the property and its subexpand/select
        /// </summary>
        /// <param name="pathToNavProp">the path to the navigation property for this expand term</param>
        /// <param name="selectOption">the sub select for this token</param>
        /// <param name="expandOption">the sub expand for this token</param>
        public ExpandTermToken(PathSegmentToken pathToNavProp, SelectToken selectOption, ExpandToken expandOption)
            : this(pathToNavProp, null, null, null, null, null, null, null, selectOption, expandOption)
        {
        }

        /// <summary>
        /// Create an expand term token
        /// </summary>
        /// <param name="pathToNavProp">the nav prop for this expand term</param>
        /// <param name="filterOption">the filter option for this expand term</param>
        /// <param name="orderByOptions">the orderby options for this expand term</param>
        /// <param name="topOption">the top option for this expand term</param>
        /// <param name="skipOption">the skip option for this expand term</param>
        /// <param name="countQueryOption">the query count option for this expand term</param>
        /// <param name="levelsOption">the levels option for this expand term</param>
        /// <param name="searchOption">the search option for this expand term</param>
        /// <param name="selectOption">the select option for this expand term</param>
        /// <param name="expandOption">the expand option for this expand term</param>
        public ExpandTermToken(PathSegmentToken pathToNavProp, QueryToken filterOption, IEnumerable<OrderByToken> orderByOptions, long? topOption, long? skipOption, bool? countQueryOption, long? levelsOption, QueryToken searchOption, SelectToken selectOption, ExpandToken expandOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavProp, "property");

            this.pathToNavProp = pathToNavProp;
            this.filterOption = filterOption;
            this.orderByOptions = orderByOptions;
            this.topOption = topOption;
            this.skipOption = skipOption;
            this.countQueryOption = countQueryOption;
            this.levelsOption = levelsOption;
            this.searchOption = searchOption;
            this.selectOption = selectOption;
            this.expandOption = expandOption;
        }

        /// <summary>
        /// the nav property for this expand term
        /// </summary>
        public PathSegmentToken PathToNavProp
        {
            get { return this.pathToNavProp; }
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
