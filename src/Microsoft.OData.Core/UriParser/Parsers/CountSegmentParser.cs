//---------------------------------------------------------------------
// <copyright file="CountSegmentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Syntactic parser for the $count segment.
    /// </summary>
    internal sealed class CountSegmentParser
    {
        /// <summary>
        /// Reference to the lexer.
        /// </summary>
        private readonly ExpressionLexer lexer;

        /// <summary>
        /// Method used to parse arguments.
        /// </summary>
        private readonly UriQueryExpressionParser parser;

        /// <summary>
        /// Create a new CountSegmentParser.
        /// </summary>
        /// <param name="lexer">Lexer positioned at a $count identifier.</param>
        /// <param name="parser">The UriQueryExpressionParser.</param>
        public CountSegmentParser(ExpressionLexer lexer, UriQueryExpressionParser parser)
        {
            ExceptionUtils.CheckArgumentNotNull(lexer, "lexer");
            ExceptionUtils.CheckArgumentNotNull(parser, "parser");
            this.lexer = lexer;
            this.parser = parser;
        }

        /// <summary>
        /// Reference to the underlying UriQueryExpressionParser.
        /// </summary>
        public UriQueryExpressionParser UriQueryExpressionParser
        {
            get { return this.parser; }
        }

        /// <summary>
        /// Reference to the lexer.
        /// </summary>
        public ExpressionLexer Lexer
        {
            get { return this.lexer; }
        }

        /// <summary>
        /// Create a <see cref="CountSegmentToken"/>.
        /// </summary>
        /// <param name="countedInstance">The instance to count on.</param>
        public CountSegmentToken CreateCountSegmentToken(QueryToken countedInstance)
        {
            QueryToken filterToken = null;
            QueryToken searchToken = null;

            bool isFilterOption = false;
            bool isSearchOption = false;

            string filterText = string.Empty;
            string searchText = string.Empty;

            // We need to maintain the recursionDepth of the outer $filter query since calling
            // ParseFilter or ParseSearch below will reset it to 0.
            int outerRecursiveDepth = this.UriQueryExpressionParser.recursionDepth;
            ExpressionLexer outerLexer = this.UriQueryExpressionParser.Lexer;

            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                return new CountSegmentToken(countedInstance);
            }
            else
            {
                this.lexer.NextToken();
                isFilterOption = this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.QueryOptionFilter, this.UriQueryExpressionParser.enableCaseInsensitiveBuiltinIdentifier);
                isSearchOption = this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.QueryOptionSearch, this.UriQueryExpressionParser.enableCaseInsensitiveBuiltinIdentifier);

                this.lexer.NextToken();

                if (isFilterOption)
                {
                    filterText = this.lexer.AdvanceThroughExpandOption();
                    filterToken = filterText == null ? null : this.UriQueryExpressionParser.ParseFilter(filterText);
                }
                else if(isSearchOption)
                {
                    searchText = this.lexer.AdvanceThroughExpandOption();
                    SearchParser searchParser = new SearchParser(ODataUriParserSettings.DefaultSearchLimit);
                    searchToken = searchText == null ? null : searchParser.ParseSearch(searchText);
                }
                else
                {
                    // Only a $filter and $search can be inside a $count()
                    throw ParseError(ODataErrorStrings.UriQueryExpressionParser_IllegalQueryOptioninDollarCount());
                }
            }

            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.SemiColon)
            {
                this.lexer.NextToken();
                isFilterOption = this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.QueryOptionFilter, this.UriQueryExpressionParser.enableCaseInsensitiveBuiltinIdentifier);
                isSearchOption = this.lexer.CurrentToken.IdentifierIs(ExpressionConstants.QueryOptionSearch, this.UriQueryExpressionParser.enableCaseInsensitiveBuiltinIdentifier);

                this.lexer.NextToken();

                if (isFilterOption)
                {
                    filterText = this.lexer.AdvanceThroughExpandOption();
                    filterToken = filterText == null ? null : this.UriQueryExpressionParser.ParseFilter(filterText);
                }
                else if (isSearchOption)
                {
                    searchText = this.lexer.AdvanceThroughExpandOption();
                    SearchParser searchParser = new SearchParser(ODataUriParserSettings.DefaultSearchLimit);
                    searchToken = searchText == null ? null : searchParser.ParseSearch(searchText);
                }
                else
                {
                    // Only a $filter and $search can be inside a $count()
                    throw ParseError(ODataErrorStrings.UriQueryExpressionParser_IllegalQueryOptioninDollarCount());
                }
            }


            this.lexer.ValidateToken(ExpressionTokenKind.CloseParen);

            this.lexer.NextToken();
            this.UriQueryExpressionParser.recursionDepth = outerRecursiveDepth;
            this.UriQueryExpressionParser.Lexer = outerLexer;
            return new CountSegmentToken(countedInstance, filterToken, searchToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryOptionName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static string TryGetQueryOption(string queryOptionName, string query)
        {
            if (queryOptionName == null)
            {
                return null;
            }

            // Concat $filter or $search with an = symbol to get $filter= or $search=
            string queryOption = string.Concat(queryOptionName, ExpressionConstants.SymbolEqual);

            char[] trimmingChars = queryOption.ToCharArray();

            return query.TrimStart(trimmingChars);
        }

        /// <summary>Creates an exception for a parse error.</summary>
        /// <param name="message">Message text.</param>
        /// <returns>A new Exception.</returns>
        private static Exception ParseError(string message)
        {
            return new ODataException(message);
        }
    }
}
