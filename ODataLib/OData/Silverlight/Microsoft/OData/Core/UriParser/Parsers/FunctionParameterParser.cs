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
        /// <param name="lexer">The lexer to read from.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        internal static bool TrySplitFunctionParameters(this ExpressionLexer lexer, out ICollection<FunctionParameterToken> splitParameters)
        {
            return lexer.TrySplitOperationParameters(ExpressionTokenKind.CloseParen, out splitParameters);
        }

        /// <summary>
        /// Tries to parse a collection of function parameters for path.
        /// </summary>     
        /// <param name="functionName">The function name to use in error messages.</param>
        /// <param name="parenthesisExpression">The contents of the parentheses portion of the current path segment.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        internal static bool TrySplitOperationParameters(string functionName, string parenthesisExpression, out ICollection<FunctionParameterToken> splitParameters)
        {
            Debug.Assert(!string.IsNullOrEmpty(parenthesisExpression), "!string.IsNullOrEmpty(parenthesisExpression)");
            var lexer = new ExpressionLexer(parenthesisExpression, true /*moveToFirstToken*/, false /*useSemicolonDelimiter*/, true /*parsingFunctionParameters*/);

            if (lexer.CurrentToken.IsFunctionParameterToken)
            {
                splitParameters = null;
                return false;
            }

            string funcFullStr = string.Format(CultureInfo.InvariantCulture, "({0})", parenthesisExpression);
            lexer = new ExpressionLexer(funcFullStr, true /*moveToFirstToken*/, false /*useSemicolonDelimiter*/, true /*parsingFunctionParameters*/);
            UriQueryExpressionParser.Parser parseMethod = () =>
            {
                throw new ODataException(" ParseArgumentList");
            }; // ParseArgumentList method should never call this parseMethod
            FunctionCallParser functionCallBinder = new FunctionCallParser(lexer, parseMethod);
            splitParameters = functionCallBinder.ParseArgumentList();
            lexer.ValidateToken(ExpressionTokenKind.End);
            return true;
        }

        /// <summary>
        /// Tries to parse a collection of function parameters. Allows path and filter to share the core algorithm while representing parameters differently.
        /// </summary>
        /// <param name="lexer">The lexer to read from.</param>
        /// <param name="endTokenKind">The token kind that marks the end of the parameters.</param>
        /// <param name="splitParameters">The parameters if they were successfully split.</param>
        /// <returns>Whether the parameters could be split.</returns>
        private static bool TrySplitOperationParameters(this ExpressionLexer lexer, ExpressionTokenKind endTokenKind, out ICollection<FunctionParameterToken> splitParameters)
        {
            Debug.Assert(lexer != null, "lexer != null");

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

                // TODO challenh: 1. currently maxDepth=10 supports only simple value, 2. per ABNF, should support commeon expression
                //      functionExprParameter  = parameterName EQ ( parameterAlias / parameterValue )
                //      parameterValue = arrayOrObject
                //                       / commonExpr
                UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(10);
                QueryToken parameterValue = expressionParser.ParseExpressionText(lexer.CurrentToken.Text);

                // if (!TryCreateParameterValueToken(lexer, out parameterValue))
                // {
                //     throw new ODataException(ODataErrorStrings.ExpressionLexer_SyntaxError(lexer.Position, lexer.ExpressionText));
                // }
                parameters.Add(new FunctionParameterToken(identifier, parameterValue));

                // Read the next parameterToken. We should be at the end, or find
                // we have a comma followed by something.
                lexer.NextToken();
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
