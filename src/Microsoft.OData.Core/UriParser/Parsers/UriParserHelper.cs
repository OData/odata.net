//---------------------------------------------------------------------
// <copyright file="UriParserHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers.UriParsers
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion

    internal static class UriParserHelper
    {
        #region Internal Methods

        /// <summary>Determines whether the specified character is a valid hexadecimal digit.</summary>
        /// <param name="c">Character to check.</param>
        /// <returns>true if <paramref name="c"/> is a valid hex digit; false otherwise.</returns>
        internal static bool IsCharHexDigit(char c)
        {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
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
        /// Removes quotes from the single-quotes text.
        /// </summary>
        /// <param name="text">Text to remove quotes from.</param>
        /// <returns>The specified <paramref name="text"/> with single quotes removed.</returns>
        /// <remarks>Copy of WebConvert.RemoveQuotes.</remarks>
        /// TODO: Consider combine this method with the method 'TryRemoveQuotes'
        internal static string RemoveQuotes(string text)
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
        /// Check and strip the input <paramref name="text"/> for literal <paramref name="suffix"/>
        /// </summary>
        /// <param name="suffix">The suffix value</param>
        /// <param name="text">The string to check</param>
        /// <returns>A string that has been striped of the suffix</returns>
        /// <remarks>Copy of WebConvert.TryRemoveLiteralSuffix.</remarks>
        internal static bool TryRemoveLiteralSuffix(string suffix, ref string text)
        {
            Debug.Assert(text != null, "text != null");
            Debug.Assert(suffix != null, "suffix != null");

            text = text.Trim();
            if (text.Length <= suffix.Length || IsValidNumericConstant(text) || !text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
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
        internal static bool TryRemoveLiteralPrefix(string prefix, ref string text)
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
        /// Make sure the given literal contains letters or '.' only.
        /// </summary>
        /// <param name="typePrefixLiteralName">typePrefixLiteralName</param>
        /// <exception cref="ArgumentException">Literal is not valid</exception>
        internal static void ValidatePrefixLiteral(string typePrefixLiteralName)
        {
            bool isLettersOnly = typePrefixLiteralName.ToCharArray().All(x => char.IsLetter(x) || x == '.');

            if (!isLettersOnly)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, ODataErrorStrings.UriParserHelper_InvalidPrefixLiteral(typePrefixLiteralName)));
            }
        }

        /// <summary>
        /// Checks whether the specified text is a correctly formatted quoted value.
        /// </summary>
        /// <param name='text'>Text to check.</param>
        /// <returns>true if the text is correctly formatted, false otherwise.</returns>
        /// <remarks>Copy of WebConvert.IsKeyValueQuoted.</remarks>
        internal static bool IsUriValueQuoted(string text)
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
                    // Check whether the Uri contains a valid escaped single quote.
                    // Example: 'aaa''bbb'.
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

        internal static IEdmTypeReference GetLiteralEdmTypeReference(ExpressionTokenKind tokenKind)
        {
            switch (tokenKind)
            {
                case ExpressionTokenKind.BooleanLiteral:
                    return EdmCoreModel.Instance.GetBoolean(false);
                case ExpressionTokenKind.DecimalLiteral:
                    return EdmCoreModel.Instance.GetDecimal(false);
                case ExpressionTokenKind.StringLiteral:
                    return EdmCoreModel.Instance.GetString(true);
                case ExpressionTokenKind.Int64Literal:
                    return EdmCoreModel.Instance.GetInt64(false);
                case ExpressionTokenKind.IntegerLiteral:
                    return EdmCoreModel.Instance.GetInt32(false);
                case ExpressionTokenKind.DoubleLiteral:
                    return EdmCoreModel.Instance.GetDouble(false);
                case ExpressionTokenKind.SingleLiteral:
                    return EdmCoreModel.Instance.GetSingle(false);
                case ExpressionTokenKind.GuidLiteral:
                    return EdmCoreModel.Instance.GetGuid(false);
                case ExpressionTokenKind.BinaryLiteral:
                    return EdmCoreModel.Instance.GetBinary(true);
                case ExpressionTokenKind.DateLiteral:
                    return EdmCoreModel.Instance.GetDate(false);
                case ExpressionTokenKind.DateTimeOffsetLiteral:
                    return EdmCoreModel.Instance.GetDateTimeOffset(false);
                case ExpressionTokenKind.DurationLiteral:
                    return EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false);
                case ExpressionTokenKind.GeographyLiteral:
                    return EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false);
                case ExpressionTokenKind.GeometryLiteral:
                    return EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, false);
                case ExpressionTokenKind.QuotedLiteral:
                    return EdmCoreModel.Instance.GetString(true);
                case ExpressionTokenKind.TimeOfDayLiteral:
                    return EdmCoreModel.Instance.GetTimeOfDay(false);
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if text is '-INF' or 'INF' or 'NaN'.
        /// </summary>
        /// <param name="text">numeric string</param>
        /// <returns>true or false</returns>
        private static bool IsValidNumericConstant(string text)
        {
            return string.Equals(text, ExpressionConstants.InfinityLiteral, StringComparison.OrdinalIgnoreCase)
                || string.Equals(text, "-" + ExpressionConstants.InfinityLiteral, StringComparison.OrdinalIgnoreCase)
                || string.Equals(text, ExpressionConstants.NaNLiteral, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}