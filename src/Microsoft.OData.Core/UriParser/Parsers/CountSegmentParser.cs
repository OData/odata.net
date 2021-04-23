//---------------------------------------------------------------------
// <copyright file="CountSegmentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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
        /// <param name="identifier">The name of this token, which in this case is always "$count".The name of this token, which in this case is always "$count".</param>
        /// <param name="countedInstance">The instance to count on.</param>
        public CountSegmentToken CreateCountSegmentToken(string identifier, QueryToken countedInstance)
        {
            QueryToken filterToken = null;
            QueryToken searchToken = null;
            string textWithinParenthesis = null;

            // We need to maintain the recursionDepth of the outer $filter query since calling
            // ParseFilter or ParseSearch below will reset it to 0.
            int outerRecursiveDepth = this.UriQueryExpressionParser.recursionDepth;

            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                return new CountSegmentToken(identifier, countedInstance);
            }
            else
            {
                textWithinParenthesis = this.lexer.AdvanceThroughExpandOption();
            }

            if (textWithinParenthesis.StartsWith(ExpressionConstants.QueryOptionFilter, StringComparison.OrdinalIgnoreCase))
            {
                string filterQuery = TryGetQueryOption(ExpressionConstants.QueryOptionFilter, textWithinParenthesis);
                filterToken = filterQuery == null ? null: this.UriQueryExpressionParser.ParseFilter(filterQuery);
            }
            else if (textWithinParenthesis.StartsWith(ExpressionConstants.QueryOptionSearch, StringComparison.OrdinalIgnoreCase))
            {
                string searchQuery = TryGetQueryOption(ExpressionConstants.QueryOptionSearch, textWithinParenthesis);
                searchToken = searchQuery == null ? null : this.UriQueryExpressionParser.ParseSearch(searchQuery);
            }
            else
            {
                // Error
            }

            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
            {
                // Error
            }

            this.lexer.NextToken();
            this.UriQueryExpressionParser.recursionDepth = outerRecursiveDepth;
            return new CountSegmentToken(identifier, countedInstance, filterToken, searchToken);
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
    }
}
