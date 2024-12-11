﻿//---------------------------------------------------------------------
// <copyright file="CountSegmentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using Microsoft.OData.Core;

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

            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.OpenParen)
            {
                return new CountSegmentToken(countedInstance);
            }
            else
            {
                // advance past the '('
                this.lexer.NextToken();

                // Check for (), which is not allowed.
                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
                {
                    throw new ODataException(SRResources.UriParser_EmptyParenthesis);
                }

                StringComparison comparison = this.UriQueryExpressionParser.EnableCaseInsensitiveBuiltinIdentifier ?
                        StringComparison.OrdinalIgnoreCase :
                        StringComparison.Ordinal;

                // Look for all the supported query options
                while (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
                {
                    ReadOnlySpan<char> textSpan = this.lexer.CurrentToken.Span;

                    if (textSpan.Equals(ExpressionConstants.QueryOptionFilter, comparison) ||
                        (this.UriQueryExpressionParser.EnableNoDollarQueryOptions && textSpan.Equals("filter", comparison)))
                    {
                        // $filter within $count segment
                        filterToken = ParseInnerFilter();
                    }
                    else if (textSpan.Equals(ExpressionConstants.QueryOptionSearch, comparison) ||
                        (this.UriQueryExpressionParser.EnableNoDollarQueryOptions && textSpan.Equals("search", comparison)))
                    {
                        // $search within $count segment
                        searchToken = ParseInnerSearch();
                    }
                    else
                    {
                        throw new ODataException(SRResources.UriQueryExpressionParser_IllegalQueryOptioninDollarCount);
                    }
                }
            }

            // Move past the ')'
            this.lexer.NextToken();
            return new CountSegmentToken(countedInstance, filterToken, searchToken);
        }

        /// <summary>
        /// Parse the filter option in the $count segment.
        /// </summary>
        /// <returns>The filter option for the $count segment.</returns>
        private QueryToken ParseInnerFilter()
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string filterText = UriParserHelper.ReadQueryOption(this.lexer);

            UriQueryExpressionParser filterParser = new UriQueryExpressionParser(ODataUriParserSettings.DefaultFilterLimit, this.UriQueryExpressionParser.EnableCaseInsensitiveBuiltinIdentifier);
            return filterParser.ParseFilter(filterText);
        }

        /// <summary>
        /// Parse the search option in the $count segment.
        /// </summary>
        /// <returns>The search option for the $count segment.</returns>
        private QueryToken ParseInnerSearch()
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string searchText = UriParserHelper.ReadQueryOption(this.lexer);

            SearchParser searchParser = new SearchParser(ODataUriParserSettings.DefaultSearchLimit);
            return searchParser.ParseSearch(searchText);
        }
    }
}
