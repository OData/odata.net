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

namespace System.Data.Services
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
