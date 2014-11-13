//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Component for parsing function parameters in both $filter/$orderby expressions and in paths.
    /// TODO: 1631831 - update code that is duplicate between operation and operation import, add more tests.
    /// </summary>
    internal static class FunctionParameterParser
    {
        /// <summary>
        /// Tries to parse a collection of function parameters. Allows path and filter to share the core algorithm while representing parameters differently.
        /// </summary>
        /// <param name="parser">The UriQueryExpressionParser to read from.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        internal static bool TrySplitFunctionParameters(this UriQueryExpressionParser parser, out ICollection<FunctionParameterToken> splitParameters)
        {
            return parser.TrySplitOperationParameters(ExpressionTokenKind.CloseParen, out splitParameters);
        }

        /// <summary>
        /// Tries to parse a collection of function parameters for path.
        /// </summary>     
        /// <param name="parenthesisExpression">The contents of the parentheses portion of the current path segment.</param>
        /// <param name="configuration">The ODataUriParserConfiguration to create a UriQueryExpressionParser.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        internal static bool TrySplitOperationParameters(string parenthesisExpression, ODataUriParserConfiguration configuration, out ICollection<FunctionParameterToken> splitParameters)
        {
            ExpressionLexer lexer = new ExpressionLexer(parenthesisExpression, true /*moveToFirstToken*/, false /*useSemicolonDelimeter*/, true /*parsingFunctionParameters*/);
            UriQueryExpressionParser parser = new UriQueryExpressionParser(configuration.Settings.FilterLimit, lexer);
            var ret = parser.TrySplitOperationParameters(ExpressionTokenKind.End, out splitParameters);

            // check duplicate names
            if (splitParameters.Select(t => t.ParameterName).Distinct().Count() != splitParameters.Count)
            {
                throw new ODataException(ODataErrorStrings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
            }

            return ret;
        }

        /// <summary>
        /// Tries to parse a collection of function parameters. Allows path and filter to share the core algorithm while representing parameters differently.
        /// </summary>
        /// <param name="parser">The UriQueryExpressionParser to read from.</param>
        /// <param name="endTokenKind">The token kind that marks the end of the parameters.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        private static bool TrySplitOperationParameters(this UriQueryExpressionParser parser, ExpressionTokenKind endTokenKind, out ICollection<FunctionParameterToken> splitParameters)
        {
            Debug.Assert(parser != null, "parser != null");
            var lexer = parser.Lexer;
            var parameters = new List<FunctionParameterToken>();
            splitParameters = parameters;

            ExpressionToken currentToken = lexer.CurrentToken;
            if (currentToken.Kind == endTokenKind)
            {
                return true;
            }

            if (currentToken.Kind != ExpressionTokenKind.Identifier || lexer.PeekNextToken().Kind != ExpressionTokenKind.Equal)
            {
                return false;
            }

            while (currentToken.Kind != endTokenKind)
            {
                lexer.ValidateToken(ExpressionTokenKind.Identifier);
                string identifier = lexer.CurrentToken.GetIdentifier();
                lexer.NextToken();

                lexer.ValidateToken(ExpressionTokenKind.Equal);
                lexer.NextToken();

                // the below UriQueryExpressionParser.ParseExpression() is able to parse common expression per ABNF:
                //      functionExprParameter  = parameterName EQ ( parameterAlias / parameterValue )
                //      parameterValue = arrayOrObject
                //                       / commonExpr
                QueryToken parameterValue = parser.ParseExpression();
                parameters.Add(new FunctionParameterToken(identifier, parameterValue));

                // the above parser.ParseExpression() already moves to the next token, now get CurrentToken checking a comma followed by something
                currentToken = lexer.CurrentToken;
                if (currentToken.Kind == ExpressionTokenKind.Comma)
                {
                    lexer.NextToken();
                    currentToken = lexer.CurrentToken;
                    if (currentToken.Kind == endTokenKind)
                    {
                        // Trailing comma.
                        throw new ODataException(ODataErrorStrings.ExpressionLexer_SyntaxError(lexer.Position, lexer.ExpressionText));
                    }
                }
            }

            return true;
        }
    }
}
