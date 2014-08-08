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

namespace System.Data.Services
{
    using System.Collections.Generic;
    using System.Data.Services.Parsing;
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>Provides a class used to parse a skip-token or etag.</summary>
    internal static class SkipTokenAndETagParser
    {
        /// <summary>Attempts to parse nullable values (only positional values, no name-value pairs) from the specified text.</summary>
        /// <param name='text'>Text to parse (not null).</param>
        /// <param name='values'>After invocation, the parsed skiptoken/etag values as strings.</param>
        /// <returns>
        /// true if the given values were parsed; false if there was a 
        /// syntactic error.
        /// </returns>
        /// <remarks>
        /// The returned collection contains only string values. They must be converted later.
        /// </remarks>
        internal static bool TryParseNullableTokens(string text, out IList<object> values)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(text != null, "text != null");

            List<object> positionalValues = new List<object>();
            values = positionalValues;

#if ODATALIB
            ExpressionLexer lexer = new ExpressionLexer(text, true, false);
#else
            ExpressionLexer lexer = new ExpressionLexer(text);
#endif
            ExpressionToken currentToken = lexer.CurrentToken;
            if (currentToken.Kind == ExpressionTokenKind.End)
            {
                return true;
            }

            do
            {
                if (currentToken.IsLiteral)
                {
                    // Positional value.
                    positionalValues.Add(lexer.CurrentToken.Text);
                }
                else
                {
                    return false;
                }

                // Read the next token. We should be at the end, or find
                // we have a comma followed by something.
                lexer.NextToken();
                currentToken = lexer.CurrentToken;
                if (currentToken.Kind == ExpressionTokenKind.Comma)
                {
                    lexer.NextToken();
                    currentToken = lexer.CurrentToken;
                    if (currentToken.Kind == ExpressionTokenKind.End)
                    {
                        // Trailing comma.
                        return false;
                    }
                }
            }
            while (currentToken.Kind != ExpressionTokenKind.End);

            return true;
        }
    }
}
