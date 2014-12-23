//   OData .NET Libraries ver. 6.9
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
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Parser for Uri Template. See <see cref="UriTemplateExpression"/> class for detail.
    /// </summary>
    internal sealed class UriTemplateParser
    {
        /// <summary>
        /// Check whether given literal matches the uri template pattern {literals}, See <see cref="UriTemplateExpression"/> class for detail.
        /// </summary>
        /// <param name="literalText">The text to be evaluated</param>
        /// <returns>True if <paramref name="literalText"/> is valid for Uri template</returns>
        internal static bool IsValidTemplateLiteral(string literalText)
        {
            return (!string.IsNullOrEmpty(literalText)
                && literalText.StartsWith("{", StringComparison.Ordinal)
                && literalText.EndsWith("}", StringComparison.Ordinal));
        }

        /// <summary>
        /// Parse a literal as Uri template.
        /// </summary>
        /// <param name="literalText">The input text.</param>
        /// <param name="expectedType">The expected type of the object which the Uri template stands for.</param>
        /// <param name="expression">The <see cref="UriTemplateExpression"/> representing the Uri template.</param>
        /// <returns>True if successfully expression parsed, false otherwise. </returns>
        internal static bool TryParseLiteral(string literalText, IEdmTypeReference expectedType, out UriTemplateExpression expression)
        {
            if (IsValidTemplateLiteral(literalText))
            {
                expression = new UriTemplateExpression { LiteralText = literalText, ExpectedType = expectedType };
                return true;
            }

            expression = null;
            return false;
        }
    }
}
