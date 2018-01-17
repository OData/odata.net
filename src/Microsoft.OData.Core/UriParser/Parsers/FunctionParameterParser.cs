//---------------------------------------------------------------------
// <copyright file="FunctionParameterParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Component for parsing function parameters in both $filter/$orderby expressions and in paths.
    /// TODO: update code that is duplicate between operation and operation import, add more tests.
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
