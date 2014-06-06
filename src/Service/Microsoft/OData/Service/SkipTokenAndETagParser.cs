//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Core;
    using Microsoft.OData.Service.Parsing;

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
