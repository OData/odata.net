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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Spatial;
    using System.Text;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    using ODataPlatformHelper = Microsoft.Data.OData.PlatformHelper;
    #endregion Namespaces

    /// <summary>
    /// Parser which consumes the URI format of primitive types and converts it to primitive types.
    /// </summary>
    internal static class UriPrimitiveTypeParser
    {
        /// <summary>Whitespace characters to trim around literals.</summary>
        private static char[] WhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

        /// <summary>Determines whether the specified character is a valid hexadecimal digit.</summary>
        /// <param name="c">Character to check.</param>
        /// <returns>true if <paramref name="c"/> is a valid hex digit; false otherwise.</returns>
        internal static bool IsCharHexDigit(char c)
        {
            DebugUtils.CheckNoExternalCallers();
            
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        /// <summary>Converts a string to a primitive value.</summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetType">Type to convert string to.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of the WebConvert.TryKeyStringToPrimitive</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is not too high; handling all the cases in one method is preferable to refactoring.")]
        internal static bool TryUriStringToPrimitive(string text, IEdmTypeReference targetType, out object targetValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(text != null, "text != null");
            Debug.Assert(targetType != null, "targetType != null");

            if (targetType.IsNullable)
            {
                if (text == ExpressionConstants.KeywordNull)
                {
                    targetValue = null;
                    return true;
                }
            }

            IEdmPrimitiveTypeReference primitiveTargetType = targetType.AsPrimitiveOrNull();
            if (primitiveTargetType == null)
            {
                targetValue = null;
                return false;
            }

            EdmPrimitiveTypeKind targetTypeKind = primitiveTargetType.PrimitiveKind();

            byte[] byteArrayValue;
            bool binaryResult = TryUriStringToByteArray(text, out byteArrayValue);
            if (targetTypeKind == EdmPrimitiveTypeKind.Binary)
            {
                targetValue = (object)byteArrayValue;
                return binaryResult;
            }
            else if (binaryResult)
            {
                string keyValue = Encoding.UTF8.GetString(byteArrayValue, 0, byteArrayValue.Length);
                return TryUriStringToPrimitive(keyValue, targetType, out targetValue);
            }
            else if (targetTypeKind == EdmPrimitiveTypeKind.Guid)
            {
                Guid guidValue;
                bool result = TryUriStringToGuid(text, out guidValue);
                targetValue = guidValue;
                return result;
            }
            else if (targetTypeKind == EdmPrimitiveTypeKind.DateTime)
            {
                DateTime dateTimeValue;
                bool result = TryUriStringToDateTime(text, out dateTimeValue);
                targetValue = dateTimeValue;
                return result;
            }
            else if (targetTypeKind == EdmPrimitiveTypeKind.DateTimeOffset)
            {
                DateTimeOffset dateTimeOffsetValue;
                bool result = TryUriStringToDateTimeOffset(text, out dateTimeOffsetValue);
                targetValue = dateTimeOffsetValue;
                return result;
            }
            else if (targetTypeKind == EdmPrimitiveTypeKind.Time)
            {
                TimeSpan timespanValue;
                bool result = TryUriStringToTime(text, out timespanValue);
                targetValue = timespanValue;
                return result;
            }
            else if (targetTypeKind == EdmPrimitiveTypeKind.Geography)
            {
                Geography geographyValue;
                bool result = TryUriStringToGeography(text, out geographyValue);
                targetValue = geographyValue;
                return result;
            }
            else if (targetTypeKind == EdmPrimitiveTypeKind.Geometry)
            {
                Geometry geometryValue;
                bool result = TryUriStringToGeometry(text, out geometryValue);
                targetValue = geometryValue;
                return result;
            }

            bool quoted = targetTypeKind == EdmPrimitiveTypeKind.String;
            if (quoted != IsUriValueQuoted(text))
            {
                targetValue = null;
                return false;
            }

            if (quoted)
            {
                text = RemoveQuotes(text);
            }

            try
            {
                switch (targetTypeKind)
                {
                    case EdmPrimitiveTypeKind.String:
                        targetValue = text;
                        break;
                    case EdmPrimitiveTypeKind.Boolean:
                        targetValue = XmlConvert.ToBoolean(text);
                        break;
                    case EdmPrimitiveTypeKind.Byte:
                        targetValue = XmlConvert.ToByte(text);
                        break;
                    case EdmPrimitiveTypeKind.SByte:
                        targetValue = XmlConvert.ToSByte(text);
                        break;
                    case EdmPrimitiveTypeKind.Int16:
                        targetValue = XmlConvert.ToInt16(text);
                        break;
                    case EdmPrimitiveTypeKind.Int32:
                        targetValue = XmlConvert.ToInt32(text);
                        break;
                    case EdmPrimitiveTypeKind.Int64:
                        if (TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixInt64, ref text))
                        {
                            targetValue = XmlConvert.ToInt64(text);
                        }
                        else
                        {
                            targetValue = default(Int64);
                            return false;
                        }

                        break;
                    case EdmPrimitiveTypeKind.Single:
                        if (TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixSingle, ref text))
                        {
                            targetValue = XmlConvert.ToSingle(text);
                        }
                        else
                        {
                            targetValue = default(Single);
                            return false;
                        }

                        break;
                    case EdmPrimitiveTypeKind.Double:
                        TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixDouble, ref text);
                        targetValue = XmlConvert.ToDouble(text);
                        break;
                    case EdmPrimitiveTypeKind.Decimal:
                        if (TryRemoveLiteralSuffix(ExpressionConstants.LiteralSuffixDecimal, ref text))
                        {
                            try
                            {
                                targetValue = XmlConvert.ToDecimal(text);
                            }
                            catch (FormatException)
                            {
                                // we need to support exponential format for decimals since we used to support them in V1
                                decimal result;
                                if (Decimal.TryParse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
                                {
                                    targetValue = result;
                                }
                                else
                                {
                                    targetValue = default(Decimal);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            targetValue = default(Decimal);
                            return false;
                        }

                        break;
                    default:
                        throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.UriPrimitiveTypeParser_TryUriStringToPrimitive));
                }

                return true;
            }
            catch (FormatException)
            {
                targetValue = null;
                return false;
            }
            catch (OverflowException)
            {
                targetValue = null;
                return false;
            }
        }

        /// <summary>
        /// Try to parse a string value into a non-negative integer.
        /// </summary>
        /// <param name="text">The string value to parse.</param>
        /// <param name="nonNegativeInteger">The non-negative integer value parsed from the <paramref name="text"/>.</param>
        /// <returns>True if <paramref name="text"/> could successfully be parsed into a non-negative integer; otherwise returns false.</returns>
        internal static bool TryUriStringToNonNegativeInteger(string text, out int nonNegativeInteger)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(text != null, "text != null");

            object valueAsObject;
            if (!UriPrimitiveTypeParser.TryUriStringToPrimitive(text, EdmCoreModel.Instance.GetInt32(false), out valueAsObject))
            {
                nonNegativeInteger = -1;
                return false;
            }

            nonNegativeInteger = (int)valueAsObject;

            if (nonNegativeInteger < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check and strip the input <paramref name="text"/> for literal <paramref name="suffix"/>
        /// </summary>
        /// <param name="suffix">The suffix value</param>
        /// <param name="text">The string to check</param>
        /// <returns>A string that has been striped of the suffix</returns>
        /// <remarks>Copy of WebConvert.TryRemoveLiteralSuffix.</remarks>
        internal static bool TryRemoveSuffix(string suffix, ref string text)
        {
            DebugUtils.CheckNoExternalCallers();
            return TryRemoveLiteralSuffix(suffix, ref text);
        }

        /// <summary>
        /// Tries to remove a literal <paramref name="prefix"/> from the specified <paramref name="text"/>.
        /// </summary>
        /// <param name="prefix">Prefix to remove; one-letter prefixes are case-sensitive, others insensitive.</param>
        /// <param name="text">Text to attempt to remove prefix from.</param>
        /// <returns>true if the prefix was found and removed; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryRemoveLiteralPrefix.</remarks>
        internal static bool TryRemovePrefix(string prefix, ref string text)
        {
            DebugUtils.CheckNoExternalCallers();
            return TryRemoveLiteralPrefix(prefix, ref text);
        }
        
        /// <summary>
        /// Removes quotes from the single-quotes text.
        /// </summary>
        /// <param name="text">Text to remove quotes from.</param>
        /// <returns>Whether quotes were successfully removed.</returns>
        /// <remarks>Copy of WebConvert.TryRemoveQuotes.</remarks>
        internal static bool TryRemoveQuotes(ref string text)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(text != null, "text != null");

            if (text.Length < 2)
            {
                return false;
            }

            char quote = text[0];
            if (quote != '\'' || text[text.Length - 1] != quote)
            {
                return false;
            }

            string s = text.Substring(1, text.Length - 2);
            int start = 0;
            while (true)
            {
                int i = s.IndexOf(quote, start);
                if (i < 0)
                {
                    break;
                }

                s = s.Remove(i, 1);
                if (s.Length < i + 1 || s[i] != quote)
                {
                    return false;
                }

                start = i + 1;
            }

            text = s;
            return true;
        }

        /// <summary>
        /// Converts a string to a byte[] value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToByteArray.</remarks>
        private static bool TryUriStringToByteArray(string text, out byte[] targetValue)
        {
            Debug.Assert(text != null, "text != null");

            if (!TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixBinary, ref text) &&
                !TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixShortBinary, ref text))
            {
                targetValue = null;
                return false;
            }

            if (!TryRemoveQuotes(ref text))
            {
                targetValue = null;
                return false;
            }

            if ((text.Length % 2) != 0)
            {
                targetValue = null;
                return false;
            }

            byte[] result = new byte[text.Length / 2];
            int resultIndex = 0;
            int textIndex = 0;
            while (resultIndex < result.Length)
            {
                char ch0 = text[textIndex];
                char ch1 = text[textIndex + 1];
                if (!IsCharHexDigit(ch0) || !IsCharHexDigit(ch1))
                {
                    targetValue = null;
                    return false;
                }

                result[resultIndex] = (byte)((byte)(HexCharToNibble(ch0) << 4) + HexCharToNibble(ch1));
                textIndex += 2;
                resultIndex++;
            }

            targetValue = result;
            return true;
        }

        /// <summary>
        /// Converts a string to a GUID value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToGuid.</remarks>
        private static bool TryUriStringToGuid(string text, out Guid targetValue)
        {
            if (!TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixGuid, ref text))
            {
                targetValue = default(Guid);
                return false;
            }

            if (!TryRemoveQuotes(ref text))
            {
                targetValue = default(Guid);
                return false;
            }

            try
            {
                targetValue = XmlConvert.ToGuid(text);
                return true;
            }
            catch (FormatException)
            {
                targetValue = default(Guid);
                return false;
            }
        }

        /// <summary>
        /// Converts a string to a DateTime value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToDateTime.</remarks>
        private static bool TryUriStringToDateTime(string text, out DateTime targetValue)
        {
            targetValue = default(DateTime);
            if (!TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixDateTime, ref text))
            {
                return false;
            }

            if (!TryRemoveQuotes(ref text))
            {
                return false;
            }

            try
            {
                targetValue = ODataPlatformHelper.ConvertStringToDateTime(text);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a string to a DateTimeOffset value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToDateTimeOffset.</remarks>
        private static bool TryUriStringToDateTimeOffset(string text, out DateTimeOffset targetValue)
        {
            targetValue = default(DateTimeOffset);
            if (!TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixDateTimeOffset, ref text))
            {
                return false;
            }

            if (!TryRemoveQuotes(ref text))
            {
                return false;
            }

            try
            {
                targetValue = ODataPlatformHelper.ConvertStringToDateTimeOffset(text);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a string to a Time value.
        /// </summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetValue">After invocation, converted value.</param>
        /// <returns>true if the value was converted; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryKeyStringToTime.</remarks>
        private static bool TryUriStringToTime(string text, out TimeSpan targetValue)
        {
            if (!TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixTime, ref text))
            {
                targetValue = default(TimeSpan);
                return false;
            }

            if (!TryRemoveQuotes(ref text))
            {
                targetValue = default(TimeSpan);
                return false;
            }

            try
            {
                targetValue = XmlConvert.ToTimeSpan(text);
                return true;
            }
            catch (FormatException)
            {
                targetValue = default(TimeSpan);
                return false;
            }
        }

        /// <summary>
        /// Try to parse the given text to a Geography object.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="targetValue">Geography to return.</param>
        /// <returns>True if succeeds, false if not.</returns>
        private static bool TryUriStringToGeography(string text, out Geography targetValue)
        {
            if (!TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixGeography, ref text))
            {
                targetValue = default(Geography);
                return false;
            }

            if (!TryRemoveQuotes(ref text))
            {
                targetValue = default(Geography);
                return false;
            }

            try
            {
                targetValue = LiteralUtils.ParseGeography(text);
                return true;
            }
            catch (ParseErrorException)
            {
                targetValue = default(Geography);
                return false;
            }
        }

        /// <summary>
        /// Try to parse the given text to a Geometry object.
        /// </summary>
        /// <param name="text">Text to parse.</param>
        /// <param name="targetValue">Geometry to return.</param>
        /// <returns>True if succeeds, false if not.</returns>
        private static bool TryUriStringToGeometry(string text, out Geometry targetValue)
        {
            if (!TryRemoveLiteralPrefix(ExpressionConstants.LiteralPrefixGeometry, ref text))
            {
                targetValue = default(Geometry);
                return false;
            }

            if (!TryRemoveQuotes(ref text))
            {
                targetValue = default(Geometry);
                return false;
            }

            try
            {
                targetValue = LiteralUtils.ParseGeometry(text);
                return true;
            }
            catch (ParseErrorException)
            {
                targetValue = default(Geometry);
                return false;
            }
        }

        /// <summary>
        /// Check and strip the input <paramref name="text"/> for literal <paramref name="suffix"/>
        /// </summary>
        /// <param name="suffix">The suffix value</param>
        /// <param name="text">The string to check</param>
        /// <returns>A string that has been striped of the suffix</returns>
        /// <remarks>Copy of WebConvert.TryRemoveLiteralSuffix.</remarks>
        private static bool TryRemoveLiteralSuffix(string suffix, ref string text)
        {
            Debug.Assert(text != null, "text != null");
            Debug.Assert(suffix != null, "suffix != null");

            text = text.Trim(WhitespaceChars);
            if (text.Length <= suffix.Length || !text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                text = text.Substring(0, text.Length - suffix.Length);
                return true;
            }
        }

        /// <summary>
        /// Tries to remove a literal <paramref name="prefix"/> from the specified <paramref name="text"/>.
        /// </summary>
        /// <param name="prefix">Prefix to remove; one-letter prefixes are case-sensitive, others insensitive.</param>
        /// <param name="text">Text to attempt to remove prefix from.</param>
        /// <returns>true if the prefix was found and removed; false otherwise.</returns>
        /// <remarks>Copy of WebConvert.TryRemoveLiteralPrefix.</remarks>
        private static bool TryRemoveLiteralPrefix(string prefix, ref string text)
        {
            Debug.Assert(prefix != null, "prefix != null");

            if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                text = text.Remove(0, prefix.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the specified text is a correctly formatted quoted value.
        /// </summary>
        /// <param name='text'>Text to check.</param>
        /// <returns>true if the text is correctly formatted, false otherwise.</returns>
        /// <remarks>Copy of WebConvert.IsKeyValueQuoted.</remarks>
        private static bool IsUriValueQuoted(string text)
        {
            Debug.Assert(text != null, "text != null");

            if (text.Length < 2 || text[0] != '\'' || text[text.Length - 1] != '\'')
            {
                return false;
            }
            else
            {
                int startIndex = 1;
                while (startIndex < text.Length - 1)
                {
                    int match = text.IndexOf('\'', startIndex, text.Length - startIndex - 1);
                    if (match == -1)
                    {
                        break;
                    }
                    else if (match == text.Length - 2 || text[match + 1] != '\'')
                    {
                        return false;
                    }
                    else
                    {
                        startIndex = match + 2;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Removes quotes from the single-quotes text.
        /// </summary>
        /// <param name="text">Text to remove quotes from.</param>
        /// <returns>The specified <paramref name="text"/> with single quotes removed.</returns>
        /// <remarks>Copy of WebConvert.RemoveQuotes.</remarks>
        private static string RemoveQuotes(string text)
        {
            Debug.Assert(!String.IsNullOrEmpty(text), "!String.IsNullOrEmpty(text)");

            char quote = text[0];
            Debug.Assert(quote == '\'', "quote == '\''");
            Debug.Assert(text[text.Length - 1] == '\'', "text should end with '\''.");

            string s = text.Substring(1, text.Length - 2);
            int start = 0;
            while (true)
            {
                int i = s.IndexOf(quote, start);
                if (i < 0)
                {
                    break;
                }

                Debug.Assert(i + 1 < s.Length && s[i + 1] == '\'', @"Each single quote should be propertly escaped with double single quotes.");
                s = s.Remove(i, 1);
                start = i + 1;
            }

            return s;
        }

        /// <summary>
        /// Returns the 4 bits that correspond to the specified character.
        /// </summary>
        /// <param name="c">Character in the 0-F range to be converted.</param>
        /// <returns>The 4 bits that correspond to the specified character.</returns>
        /// <exception cref="FormatException">Thrown when 'c' is not in the '0'-'9','a'-'f' range.</exception>
        /// <remarks>This is a copy of WebConvert.HexCharToNibble.</remarks>
        private static byte HexCharToNibble(char c)
        {
            Debug.Assert(IsCharHexDigit(c), string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} is not a hex digit.", c));

            switch (c)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
                default:
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.UriPrimitiveTypeParser_HexCharToNibble));
            }
        }
    }
}
