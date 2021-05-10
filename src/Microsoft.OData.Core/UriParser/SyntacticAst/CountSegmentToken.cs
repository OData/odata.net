//---------------------------------------------------------------------
// <copyright file="CountSegmentToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// Lexical token representing the $count segment in a path.
    /// </summary>
    public sealed class CountSegmentToken : PathToken
    {
        /// <summary>
        /// The instance to count on.
        /// </summary>
        private QueryToken nextToken;

        /// <summary>
        /// The token representing $filter.
        /// </summary>
        private QueryToken filterOption;

        /// <summary>
        /// The token representing $search.
        /// </summary>
        private QueryToken searchOption;

        /// <summary>
        /// Create a CountSegmentToken given the NextToken.
        /// </summary>
        /// <param name="nextToken">The instance to count on.</param>
        public CountSegmentToken(QueryToken nextToken)
            :this(nextToken, null, null)
        {
        }

        /// <summary>
        /// Create a CountSegmentToken given the NextToken, FilterOption (if any) and SearchOption (if any).
        /// </summary>
        /// <param name="nextToken">The instance to count on.</param>
        /// <param name="filterOption">The <see cref="QueryToken"/> representing $filter.</param>
        /// <param name="searchOption">The <see cref="QueryToken"/> representing $search. </param>
        public CountSegmentToken(QueryToken nextToken, QueryToken filterOption, QueryToken searchOption)
        {
            ExceptionUtils.CheckArgumentNotNull(nextToken, "nextToken");

            this.nextToken = nextToken;
            this.filterOption = filterOption;
            this.searchOption = searchOption;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.CountSegment; }
        }

        /// <summary>
        /// The name of this token, which in this case is always "$count".
        /// </summary>
        public override string Identifier
        {
#if ODATA_CLIENT
            get { return UriHelper.VIRTUALPROPERTYCOUNT; }
#else
            get { return ExpressionConstants.QueryOptionCount; }
#endif
        }

        /// <summary>
        /// The instance to count on.
        /// </summary>
        public override QueryToken NextToken
        {
            get { return this.nextToken; }
            set { this.nextToken = value; }
        }

        /// <summary>
        /// The <see cref="QueryToken"/> representing $filter.
        /// If this is null, then the $count segment does not have a filter query option.
        /// </summary>
        public QueryToken FilterOption
        {
            get { return this.filterOption; }
            set { this.filterOption = value; }
        }

        /// <summary>
        /// The <see cref="QueryToken"/> representing $search.
        /// If this is null, then the $count segment does not have a search query option.
        /// </summary>
        public QueryToken SearchOption
        {
            get { return this.searchOption; }
            set { this.searchOption = value; }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}