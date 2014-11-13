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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Globalization;
    #endregion Namespaces

    /// <summary>
    /// Constants and utility methods for the OData URI builder.
    /// </summary>
    internal static class ODataUriBuilderUtils
    {
        /// <summary>
        /// The format for integer ToString output.
        /// </summary>
        internal const string IntegerFormat = "D";

        /// <summary>
        /// The format for float ToString output.
        /// </summary>
        internal const string FloatFormat = "F";

        /// <summary>
        /// The format for byte ToString output.
        /// </summary>
        internal const string BinaryFormat = "X2";

        /// <summary>
        /// The format for double ToString output.
        /// </summary>
        internal const string DoubleFormat = "R";

        /// <summary>
        /// The format for DateTime ToString output.
        /// </summary>
        internal const string DateTimeFormat = @"yyyy-MM-ddTHH:mm:ss.FFFFFFF";

        /// <summary>
        /// The format for DateTimeOffset ToString output.
        /// </summary>
        internal const string DateTimeOffsetFormat = @"yyyy-MM-ddTHH:mm:ss.FFFFFFFzzzzzzz";

        /// <summary>
        /// The format for Decimal ToString output.
        /// </summary>
        internal static readonly NumberFormatInfo DecimalFormatInfo = new NumberFormatInfo { NumberDecimalDigits = 0 };

        /// <summary>
        /// The format for Double ToString output.
        /// </summary>
        internal static readonly NumberFormatInfo DoubleFormatInfo = new NumberFormatInfo { NumberDecimalDigits = 0, PositiveSign = string.Empty };

        /// <summary>
        /// Escape a string literal by replacing single ' with ''.
        /// </summary>
        /// <param name="text">Text to escape.</param>
        /// <returns>A string where all ' is replaced by ''.</returns>
        internal static string Escape(string text)
        {
            DebugUtils.CheckNoExternalCallers(); 
            return text.Replace(ExpressionConstants.SymbolSingleQuote, ExpressionConstants.SymbolSingleQuoteEscaped);
        }

        /// <summary>
        /// Returns the string representation of the inline count kind.
        /// </summary>
        /// <param name="inlineCount">The inline count kind to convert to string.</param>
        /// <returns>The string representation of the <paramref name="inlineCount"/>.</returns>
        internal static string ToText(this InlineCountKind inlineCount)
        {
            DebugUtils.CheckNoExternalCallers(); 
            switch (inlineCount)
            {
                case InlineCountKind.None: return ExpressionConstants.InlineCountNone;
                case InlineCountKind.AllPages: return ExpressionConstants.InlineCountAllPages;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataUriBuilderUtils_ToText_InlineCountKind_UnreachableCodePath));
            }
        }

        /// <summary>
        /// Throw ODataException on the given QueryTokenKind as not supported for writing to Uri.
        /// </summary>
        /// <param name="queryTokenKind">QueryTokenKind that is not supported.</param>
        internal static void NotSupportedQueryTokenKind(QueryTokenKind queryTokenKind)
        {
            DebugUtils.CheckNoExternalCallers(); 
            throw new ODataException(Strings.UriBuilder_NotSupportedQueryToken(queryTokenKind));
        }

        /// <summary>
        /// Throw ODataException on the given CLR type as not supported for writing to Uri.
        /// </summary>
        /// <param name="type">CLR type that is not supported.</param>
        internal static void NotSupported(Type type)
        {
            DebugUtils.CheckNoExternalCallers(); 
            throw new ODataException(Strings.UriBuilder_NotSupportedClrLiteral(type.FullName));
        }
    }
}
