//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
