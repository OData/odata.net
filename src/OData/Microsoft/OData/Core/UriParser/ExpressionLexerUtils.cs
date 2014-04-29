//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ODATALIB
namespace Microsoft.OData.Core.UriParser
#else
namespace Microsoft.OData.Service.Parsing
#endif
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;

    #endregion Namespaces

    /// <summary>
    /// Utilities needed by <see cref="ExpressionLexer"/> which are relatively simple and standalone.
    /// </summary>
    internal sealed class ExpressionLexerUtils
    {
        /// <summary>Suffix for single literals.</summary>
        private const char SingleSuffixLower = 'f';

        /// <summary>Suffix for single literals.</summary>
        private const char SingleSuffixUpper = 'F';

        /// <summary>Whether the specified token identifier is a numeric literal.</summary>
        /// <param name="id">Token to check.</param>
        /// <returns>true if it's a numeric literal; false otherwise.</returns>
        internal static bool IsNumeric(ExpressionTokenKind id)
        {
            return
                id == ExpressionTokenKind.IntegerLiteral || id == ExpressionTokenKind.DecimalLiteral ||
                id == ExpressionTokenKind.DoubleLiteral || id == ExpressionTokenKind.Int64Literal ||
                id == ExpressionTokenKind.SingleLiteral;
        }

        /// <summary>
        /// Checks if the <paramref name="tokenText"/> is INF or NaN.
        /// Internal for testing only.
        /// </summary>
        /// <param name="tokenText">Input token.</param>
        /// <returns>true if match found, false otherwise.</returns>
        internal static bool IsInfinityOrNaNDouble(string tokenText)
        {
            Debug.Assert(tokenText != null, "tokenText != null");

            if (tokenText.Length == 3)
            {
                if (tokenText[0] == ExpressionConstants.InfinityLiteral[0])
                {
                    return IsInfinityLiteralDouble(tokenText);
                }
                else
                {
                    if (tokenText[0] == ExpressionConstants.NaNLiteral[0])
                    {
                        return String.CompareOrdinal(tokenText, 0, ExpressionConstants.NaNLiteral, 0, 3) == 0;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether <paramref name="text"/> equals to 'INF'
        /// Internal for testing only
        /// </summary>
        /// <param name="text">Text to look in.</param>
        /// <returns>true if the substring is equal using an ordinal comparison; false otherwise.</returns>
        internal static bool IsInfinityLiteralDouble(string text)
        {
            Debug.Assert(text != null, "text != null");

            return String.CompareOrdinal(text, 0, ExpressionConstants.InfinityLiteral, 0, text.Length) == 0;
        }

        /// <summary>
        /// Checks if the <paramref name="tokenText"/> is INFf/INFF or NaNf/NaNF.
        /// Internal for testing only.
        /// </summary>
        /// <param name="tokenText">Input token.</param>
        /// <returns>true if match found, false otherwise.</returns>
        internal static bool IsInfinityOrNanSingle(string tokenText)
        {
            Debug.Assert(tokenText != null, "tokenText != null");

            if (tokenText.Length == 4)
            {
                if (tokenText[0] == ExpressionConstants.InfinityLiteral[0])
                {
                    return IsInfinityLiteralSingle(tokenText);
                }
                else if (tokenText[0] == ExpressionConstants.NaNLiteral[0])
                {
                    return (tokenText[3] == SingleSuffixLower || tokenText[3] == SingleSuffixUpper) &&
                            String.CompareOrdinal(tokenText, 0, ExpressionConstants.NaNLiteral, 0, 3) == 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether <paramref name="text"/> EQUALS to 'INFf' or 'INFF'.
        /// Internal for testing only.
        /// </summary>
        /// <param name="text">Text to look in.</param>
        /// <returns>true if the substring is equal using an ordinal comparison; false otherwise.</returns>
        internal static bool IsInfinityLiteralSingle(string text)
        {
            Debug.Assert(text != null, "text != null");
            return text.Length == 4 &&
                   (text[3] == SingleSuffixLower || text[3] == SingleSuffixUpper) &&
                   String.CompareOrdinal(text, 0, ExpressionConstants.InfinityLiteral, 0, 3) == 0;
        }
    }
}
