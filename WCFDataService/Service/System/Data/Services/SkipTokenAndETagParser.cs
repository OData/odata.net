//   OData .NET Libraries ver. 5.6.3
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
