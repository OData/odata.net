//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
        /// <param name="functionName">The function name to use in error messages.</param>
        /// <param name="parenthesisExpression">The contents of the parentheses portion of the current path segment.</param>
        /// <param name="configuration">The ODataUriParserConfiguration to create a UriQueryExpressionParser.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        internal static bool TrySplitOperationParameters(string functionName, string parenthesisExpression, ODataUriParserConfiguration configuration, out ICollection<FunctionParameterToken> splitParameters)
        {
            ExpressionLexer lexer = new ExpressionLexer(parenthesisExpression, true /*moveToFirstToken*/, false /*useSemicolonDelimeter*/, true /*parsingFunctionParameters*/);
            UriQueryExpressionParser parser = new UriQueryExpressionParser(configuration.Settings.FilterLimit, lexer);
            var ret = parser.TrySplitOperationParameters(ExpressionTokenKind.End, out splitParameters);

            // check duplicate names
            if (splitParameters.Select(t => t.ParameterName).Distinct().Count() != splitParameters.Count)
            {
                throw new ODataException(ODataErrorStrings.FunctionCallParser_DuplicateParameterName);
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
