//---------------------------------------------------------------------
// <copyright file="UriParserHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text.Json;

    #endregion

    internal static class UriParserHelper
    {
        private static readonly char[] XmlWhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

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
        internal static bool TryRemovePrefix(ReadOnlySpan<char> prefix, ref ReadOnlySpan<char> text)
        {
            return TryRemoveLiteralPrefix(prefix, ref text);
        }

        /// <summary>
        /// Remove single or double quoted string literal delimiters from the specified <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to remove quotes from.</param>
        /// <param name="value">The resulting string after removing quotes. If true, this will contain the unquoted string.</param>
        /// <returns>True if quotes were successfully removed; otherwise, false.</returns>
        internal static bool TryRemoveQuotes(ref ReadOnlySpan<char> text, out string value)
        {
            value = null;
            if (text.Length < 2)
            {
                return false;
            }

            // double quoted string
            if (text[0] == '"' && text[^1] == '"')
            {
                value = JsonSerializer.Deserialize<string>(text);
                text = value.AsSpan();
                return true;
            }

            if (TryRemoveSingleQuotes(ref text, out value))
            {
                if (value == null)
                {
                    // The value is null meaning the text doesn't have escaped single quotes within the string.
                    value = text.ToString();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes quotes from the single-quotes text.
        /// </summary>
        /// <param name="text">Text to remove quotes from.</param>
        /// <returns>Whether quotes were successfully removed.</returns>
        internal static bool TryRemoveSingleQuotes(ref ReadOnlySpan<char> text)
            => TryRemoveSingleQuotes(ref text, out _);

        /// <summary>
        /// Removes quotes from the single-quotes text.
        /// </summary>
        /// <param name="text">Text to remove quotes from.</param>
        /// <param name="value">The new string created if there's any escaped single quotes.</param>
        /// <returns>Whether quotes were successfully removed.</returns>
        internal static bool TryRemoveSingleQuotes(ref ReadOnlySpan<char> text, out string value)
        {
            Debug.Assert(!text.IsEmpty, "!text.IsEmpty");

            value = null;
            if (text.Length < 2)
            {
                return false;
            }

            char quote = text[0];
            if (quote != '\'' || text[^1] != quote)
            {
                return false;
            }

            // Work with the inner content only by removing the leading and ending single quotes
            text = text.Slice(1, text.Length - 2);

            int escapedCount = 0;
            for (int k = 0; k < text.Length; k++)
            {
                if (text[k] == quote)
                {
                    if (k + 1 >= text.Length || text[k + 1] != quote)
                    {
                        // found a single quote within the content so it's not a valid single quoted string.
                        return false;
                    }

                    escapedCount++;
                    k++; // Skip the second quote of the pair
                }
            }

            // If no escaped quotes found, return the un-quoted (leading and ending single quotes removed) immediately
            if (escapedCount == 0)
            {
                return true;
            }

            int finalLength = text.Length - escapedCount;

            // Now, it's valid single quoted string.
            value = string.Create(finalLength, text, (dest, src) =>
            {
                int destIdx = 0;
                for (int i = 0; i < src.Length; i++)
                {
                    dest[destIdx++] = src[i];

                    // If we hit a quote, check if the next char is also a quote to skip it
                    if (src[i] == quote && i + 1 < src.Length && src[i + 1] == quote)
                    {
                        i++;
                    }
                }
            });

            text = value.AsSpan();
            return true;
        }

        /// <summary>
        /// Check and strip the input <paramref name="text"/> for literal <paramref name="suffix"/>
        /// </summary>
        /// <param name="suffix">The suffix value</param>
        /// <param name="text">The string to check</param>
        /// <returns>A string that has been striped of the suffix</returns>
        internal static bool TryRemoveLiteralSuffix(ReadOnlySpan<char> suffix, ref ReadOnlySpan<char> text)
        {
            Debug.Assert(!text.IsEmpty, "!text.IsEmpty");
            Debug.Assert(!suffix.IsEmpty, "!suffix.IsEmpty");

            text = text.Trim(XmlWhitespaceChars);
            if (text.Length <= suffix.Length || IsValidNumericConstant(text) || !text.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                text = text.Slice(0, text.Length - suffix.Length);
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
        internal static bool TryRemoveLiteralPrefix(ReadOnlySpan<char> prefix, ref ReadOnlySpan<char> text)
        {
            Debug.Assert(!prefix.IsEmpty, "!prefix.IsEmpty");

            if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                text = text.Slice(prefix.Length);
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
                    string.Format(CultureInfo.InvariantCulture, Error.Format(SRResources.UriParserHelper_InvalidPrefixLiteral, typePrefixLiteralName)));
            }
        }

        /// <summary>
        /// Checks whether the specified text is a correctly formatted single quoted value.
        /// </summary>
        /// <param name='text'>Text to check.</param>
        /// <returns>true if the text is correctly formatted, false otherwise.</returns>
        internal static bool IsUriValueSingleQuoted(ReadOnlySpan<char> text)
        {
            Debug.Assert(!text.IsEmpty, "!text.IsEmpty");

            if (text.Length < 2 || text[0] != '\'' || text[^1] != '\'')
            {
                return false;
            }
            else
            {
                ReadOnlySpan<char> s = text.Slice(1, text.Length - 2); // skip the leading and ending single quote
                while (!s.IsEmpty)
                {
                    // Check whether the Uri contains a valid escaped single quote.
                    // single quote is escaped within single quoted string using two consecutive single quotes. Example: 'aaa''bbb'.
                    int match = s.IndexOf('\'');
                    if (match < 0)
                    {
                        break;
                    }
                    else if (match == s.Length - 1 || s[match + 1] != '\'')
                    {
                        return false;
                    }
                    else
                    {
                        s = s.Slice(match + 2);
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
                    return EdmCoreModel.Instance.GetBinary(false);
                case ExpressionTokenKind.DateOnlyLiteral:
                    return EdmCoreModel.Instance.GetDateOnly(false);
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
                case ExpressionTokenKind.TimeOnlyLiteral:
                    return EdmCoreModel.Instance.GetTimeOnly(false);
            }

            return null;
        }

        /// <summary>
        /// Determines whether or not an identifier is an annotation term name
        /// </summary>
        /// <param name="identifier">The identifier that may be an annotation term name</param>
        /// <returns>True if the identifier is an annotation term, otherwise false</returns>
        internal static bool IsAnnotation(ReadOnlySpan<char> identifier)
        {
            return !identifier.IsEmpty && identifier[0] == UriQueryConstants.AnnotationPrefix && identifier.Contains(".".AsSpan(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Read a query option from the lexer.
        /// </summary>
        /// <returns>The query option as a ReadOnlyMemory of characters.</returns>
        internal static ReadOnlyMemory<char> ReadQueryOption(ExpressionLexer lexer)
        {
            if (lexer.CurrentToken.Kind != ExpressionTokenKind.Equal)
            {
                throw new ODataException(Error.Format(SRResources.UriSelectParser_TermIsNotValid, lexer.ExpressionText));
            }

            // get the full text from the current location onward
            // there could be literals like 'A string literal; tricky!' in there, so we need to be careful.
            // Also there could be more nested (...) expressions that we ignore until we recurse enough times to get there.
            ReadOnlyMemory<char> expressionText = lexer.AdvanceThroughExpandOption();

            if (lexer.CurrentToken.Kind == ExpressionTokenKind.SemiColon)
            {
                // Move over the ';' separator
                lexer.NextToken();
                return expressionText;
            }

            // If there wasn't a semicolon, it MUST be the last option. We must be at ')' in this case
            lexer.ValidateToken(ExpressionTokenKind.CloseParen);
            return expressionText;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if text is '-INF' or 'INF' or 'NaN'.
        /// </summary>
        /// <param name="text">numeric string</param>
        /// <returns>true or false</returns>
        internal static bool IsValidNumericConstant(ReadOnlySpan<char> text)
        {
            return text.Equals(ExpressionConstants.InfinityLiteral, StringComparison.OrdinalIgnoreCase) // INF
                || text.Equals(ExpressionConstants.NegativeInfinityLiteral, StringComparison.OrdinalIgnoreCase) // -INF
                || text.Equals(ExpressionConstants.NaNLiteral, StringComparison.OrdinalIgnoreCase); // NaN
        }

        #endregion
    }
}