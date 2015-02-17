//---------------------------------------------------------------------
// <copyright file="WebConvert.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;

    /// <summary>Utilities for converting literals to/from strings.</summary>
    internal static class WebConvert
    {
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
        /// <param name="prefix">Prefix to remove; case insensitive.</param>
        /// <param name="text">Text to attempt to remove prefix from.</param>
        /// <returns>true if the prefix was found and removed; false otherwise.</returns>
        internal static bool TryRemovePrefix(string prefix, ref string text)
        {
            Debug.Assert(prefix != null, "prefix != null");
            if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                text = text.Remove(0, prefix.Length);
                return true;
            }

            return false;
        }

        /// <summary>Removes quotes from the single-quotes text.</summary>
        /// <param name="text">Text to remove quotes from.</param>
        /// <returns>Whether quotes were successfully removed.</returns>
        internal static bool TryRemoveQuotes(ref string text)
        {
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
    }
}
