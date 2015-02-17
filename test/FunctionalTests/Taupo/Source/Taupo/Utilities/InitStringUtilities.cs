//---------------------------------------------------------------------
// <copyright file="InitStringUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Very simple parser for extracting name/value pairs from a connection string-like text.
    /// </summary>
    public static class InitStringUtilities
    {
        private const char VerbatimStringPrefix = '@';

        /// <summary>
        /// Parses initialization string.
        /// </summary>
        /// <param name="value">string to be parsed</param>
        /// <returns>Sequence of (name,value) pairs as parsed from the string</returns>
        public static IEnumerable<KeyValuePair<string, string>> ParseInitString(string value)
        {
            int p = 0;

            value = value ?? string.Empty;

            while (p < value.Length)
            {
                // skip whitespace before keyword
                SkipWhile(value, ref p, c => char.IsWhiteSpace(c));
                if (ConditionalSkip(value, ref p, c => c == ';'))
                {
                    continue;
                }

                int keywordNameStart = p;

                // skip until '='
                SkipWhile(value, ref p, c => c != '=');
                int keywordNameEnd = p;

                // skip past '='
                if (!ConditionalSkip(value, ref p, c => c == '='))
                {
                    break;
                }

                // skip optional whitespace before value
                SkipWhile(value, ref p, c => char.IsWhiteSpace(c));

                string keywordValue;

                if (p >= value.Length)
                {
                    keywordValue = string.Empty;
                }
                else if (value[p] == '"' || value[p] == '\'')
                {
                    char endChar = value[p];

                    p++;
                    int valueStart = p;
                    SkipWhile(value, ref p, c => c != endChar);
                    int valueEnd = p;
                    p++;
                    keywordValue = value.Substring(valueStart, valueEnd - valueStart);
                }
                else if (p + 2 < value.Length && value[p] == VerbatimStringPrefix && value[p + 1] == '"')
                {
                    // skip initial quote
                    p += 2;

                    keywordValue = string.Empty;

                    bool again;

                    do
                    {
                        int valueStart = p;
                        while (p < value.Length && value[p] != '"')
                        {
                            p++;
                        }

                        keywordValue += value.Substring(valueStart, p - valueStart);
                        p++;

                        // if the character right after the closing quote is another quote - loop again
                        again = false;
                        if (p < value.Length && value[p] == '"')
                        {
                            keywordValue += '"';
                            p++;
                            again = true;
                        }
                    }
                    while (again);
                }
                else
                {
                    int valueStart = p;
                    SkipWhile(value, ref p, c => c != ';');
                    int valueEnd = p;
                    keywordValue = value.Substring(valueStart, valueEnd - valueStart).Trim();
                }

                string keyword = value.Substring(keywordNameStart, keywordNameEnd - keywordNameStart);

                yield return new KeyValuePair<string, string>(keyword.Trim(), keywordValue);
            }
        }

        /// <summary>
        /// Generates the initialization string from key-value pairs.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <returns>Initialization string.</returns>
        public static string GenerateInitializationString(IEnumerable<KeyValuePair<string, string>> elements)
        {
            ExceptionUtilities.CheckArgumentNotNull(elements, "elements");

            StringBuilder sb = new StringBuilder();
            string separator = string.Empty;

            foreach (var el in elements)
            {
                string key = el.Key;
                string value = el.Value;

                sb.Append(separator);
                separator = ";";
                sb.Append(key);
                sb.Append("=");

                bool hasSpaces = value.Any(c => char.IsWhiteSpace(c));
                bool hasSemicolon = value.IndexOf(';') >= 0;
                bool hasSingleQuote = value.IndexOf('\'') >= 0;
                bool hasDoubleQuote = value.IndexOf('"') >= 0;

                if (hasSingleQuote)
                {
                    if (!hasDoubleQuote)
                    {
                        sb.Append("\"");
                        sb.Append(value);
                        sb.Append("\"");
                    }
                    else
                    {
                        sb.Append(VerbatimStringPrefix);
                        sb.Append("\"");
                        sb.Append(value.Replace("\"", "\"\""));
                        sb.Append("\"");
                    }
                }
                else if (hasDoubleQuote || hasSpaces || hasSemicolon)
                {
                    // no single quotes but requires quoting
                    sb.Append("'");
                    sb.Append(value);
                    sb.Append("'");
                }
                else
                {
                    // no quoting required
                    sb.Append(value);
                }
            }

            return sb.ToString();
        }

        private static void SkipWhile(string s, ref int pos, Func<char, bool> predicate)
        {
            while (pos < s.Length && predicate(s[pos]))
            {
                pos++;
            }
        }

        private static bool ConditionalSkip(string s, ref int pos, Func<char, bool> predicate)
        {
            if (pos < s.Length && predicate(s[pos]))
            {
                pos++;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
