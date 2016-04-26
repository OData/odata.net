//---------------------------------------------------------------------
// <copyright file="UriTemplateParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
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
