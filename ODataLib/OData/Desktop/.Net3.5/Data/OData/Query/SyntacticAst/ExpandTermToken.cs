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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Data.OData.Query.SemanticAst;

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
        /// the order by option for this expand term
        /// </summary>
        private readonly OrderByToken orderByOption;

        /// <summary>
        /// the top option for this expand term
        /// </summary>
        private readonly long? topOption;

        /// <summary>
        /// the skip option for this expand term.
        /// </summary>
        private readonly long? skipOption;

        /// <summary>
        /// the inlineCount option for this expand term.
        /// </summary>
        private readonly InlineCountKind? inlineCountOption;

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
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavProp, "pathToNavigationProperty");
            this.pathToNavProp = pathToNavProp;
            this.filterOption = null;
            this.orderByOption = null;
            this.topOption = null;
            this.skipOption = null;
            this.inlineCountOption = null;
            this.selectOption = null;
            this.expandOption = null;
        }

        /// <summary>
        /// Create an expand term using only the property and its subexpand/select
        /// </summary>
        /// <param name="pathToNavProp">the path to the navigation property for this expand term</param>
        /// <param name="selectOption">the sub select for this token</param>
        /// <param name="expandOption">the sub expand for this token</param>
        public ExpandTermToken(PathSegmentToken pathToNavProp, SelectToken selectOption, ExpandToken expandOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavProp, "pathToNavigationProperty");
            this.pathToNavProp = pathToNavProp;
            this.filterOption = null;
            this.orderByOption = null;
            this.topOption = null;
            this.skipOption = null;
            this.selectOption = selectOption;
            this.expandOption = expandOption;
        }

        /// <summary>
        /// Create an expand term token
        /// </summary>
        /// <param name="pathToNavProp">the nav prop for this expand term</param>
        /// <param name="filterOption">the filter option for this expand term</param>
        /// <param name="orderByOption">the orderby option for this expand term</param>
        /// <param name="topOption">the top option for this expand term</param>
        /// <param name="skipOption">the skip option for this expand term</param>
        /// <param name="inlineCountOption">the inlineCountOption for this expand term</param>
        /// <param name="selectOption">the select option for this expand term</param>
        /// <param name="expandOption">the expand option for this expand term</param>
        public ExpandTermToken(PathSegmentToken pathToNavProp, QueryToken filterOption, OrderByToken orderByOption, long? topOption, long? skipOption, InlineCountKind? inlineCountOption, SelectToken selectOption, ExpandToken expandOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavProp, "property");

            this.pathToNavProp = pathToNavProp;
            this.filterOption = filterOption;
            this.orderByOption = orderByOption;
            this.topOption = topOption;
            this.skipOption = skipOption;
            this.inlineCountOption = inlineCountOption;
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
        /// the orderby option for this expand term.
        /// </summary>
        public OrderByToken OrderByOption
        {
            get { return this.orderByOption; }
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
        /// the inline count option for this expand term.
        /// </summary>
        public InlineCountKind? InlineCountOption
        {
            get { return this.inlineCountOption; }
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
