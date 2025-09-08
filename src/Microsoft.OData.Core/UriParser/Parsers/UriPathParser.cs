//---------------------------------------------------------------------
// <copyright file="UriPathParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using Microsoft.OData.Core;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Parser which consumes the URI path and produces the lexical object model.
    /// </summary>
    public class UriPathParser
    {
        /// <summary>
        /// The maximum number of segments allowed.
        /// </summary>
        private readonly int maxSegments;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">The Uri parser settings.</param>
        public UriPathParser(ODataUriParserSettings settings)
        {
            this.maxSegments = settings.PathLimit;
        }

        /// <summary>
        /// Returns list of segments in the specified path (eg: /abc/pqr -&gt; abc, pqr).
        /// </summary>
        /// <param name="fullUri">The full URI of the request.</param>
        /// <param name="serviceBaseUri">The service base URI for the request.</param>
        /// <returns>List of unescaped segments.</returns>
        public virtual ICollection<string> ParsePathIntoSegments(Uri fullUri, Uri serviceBaseUri)
        {
            // Necessary: UriUtils.UriInvariantInsensitiveIsBaseOf relies on absolute/non-null; Debug.Assert is no-op in release.
            ExceptionUtils.CheckArgumentNotNull(fullUri, nameof(fullUri));

            if (serviceBaseUri == null)
            {
                Debug.Assert(!fullUri.IsAbsoluteUri, "fullUri must be relative Uri");
                serviceBaseUri = UriUtils.CreateMockAbsoluteUri(); // -> "http://host/"
                fullUri = UriUtils.CreateMockAbsoluteUri(fullUri); // -> absolute
            }
            else
            {
                // Ensure absolute URIs before base-of validation; otherwise
                // UriUtils.UriInvariantInsensitiveIsBaseOf can throw when given relative URIs.
                if (!serviceBaseUri.IsAbsoluteUri)
                {
                    serviceBaseUri = UriUtils.CreateMockAbsoluteUri(serviceBaseUri);
                }

                if (!fullUri.IsAbsoluteUri)
                {
                    fullUri = UriUtils.CreateMockAbsoluteUri(fullUri);
                }
            }

            if (!UriUtils.UriInvariantInsensitiveIsBaseOf(serviceBaseUri, fullUri))
            {
                throw new ODataException(Error.Format(SRResources.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri, fullUri, serviceBaseUri));
            }

            try
            {
                // Work with the encoded path so we can detect "%27" reliably
                // AbsolutePath is the path component (percent-encoded where applicable)
                string fullPath = fullUri.AbsolutePath;
                string basePath = serviceBaseUri.AbsolutePath;

                // Compute the starting index in fullPath right after the basePath
                // The base-of check above guarantees this relation
                int startIndex = 0;
                // base-of check normalizes to uppercase and effectively does an OrdinalIgnoreCase comparison
                if (fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                {
                    startIndex = basePath.Length;
                }
                else
                {
                    // Should not happen if base-of passed and both are absolute
                    throw new ODataException(SRResources.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri);
                }

                List<string> segments = new List<string>(4);
                // Seed capacity: typical paths are short; cap to avoid large upfront allocation.
                // Increase if profiling shows frequent growth.
                int estimatedCapacity = fullPath.Length - startIndex;
                StringBuilder sb = new StringBuilder(estimatedCapacity > 256 ? 256 : estimatedCapacity);
                bool hasEncodedChars = false;

                // Local helpers
                // Matches "%27" (single quote encoded) at position i
                static bool IsPercent27(string s, int i) => i + 2 < s.Length && s[i] == '%' && s[i + 1] == '2' && s[i + 2] == '7';
                // Matches "%28" (left paren encoded) at position i
                static bool IsPercent28(string s, int i) => i + 2 < s.Length && s[i] == '%' && s[i + 1] == '2' && s[i + 2] == '8';
                // Matches "%29" (right paren encoded) at position i
                static bool IsPercent29(string s, int i) => i + 2 < s.Length && s[i] == '%' && s[i + 1] == '2' && s[i + 2] == '9';
                // Matches "%XX" escape sequence at position i
                // Returns true and sets nextIndex to index after the escape if found
                static bool TryConsumeEncodedChar(string s, int i, out int nextIndex)
                {
                    if (i < s.Length && s[i] == '%')
                    {
                        if (i + 2 < s.Length)
                        {
                            nextIndex = i + 3; // %XX
                        }
                        else
                        {
                            // Malformed tail like "%" or "%X": append as-is, let UnescapeDataString handle that later
                            nextIndex = i + 1;
                        }

                        return true;
                    }

                    nextIndex = i;

                    return false;
                }

                // Consume a *balanced* parenthetical block that begins at a raw '(' or encoded '%28'.
                // - Honors single-quoted strings inside the block so that '/' within quotes is treated as data
                //   and does not split path segments.
                // - Supports raw/encoded parentheses and quotes: '(' / ')' / %28 / %29, and quotes '\'' / %27.
                // - Recognizes escaped quotes: '' (raw), %27%27 (encoded), and mixed forms ('%27 and %27').
                // - Allows commas, equals, whitespace, and nested parentheses.
                // - Returns true and sets `exclusiveEnd` to the index *after* the matching ')' or '%29'.
                // - Sets `hasEncodedChars` to true if any percent-escape (%XX) occurs anywhere in the block.
                // - Returns false for unbalanced parentheses or unterminated quotes; caller should then
                //   treat characters literally
                static bool TryConsumeParenBlock(string s, int i, out int exclusiveEnd, out bool hasEncodedChars)
                {
                    exclusiveEnd = i;
                    hasEncodedChars = false;

                    int j = i;

                    // Opening paren: raw "(" or encoded "%28"
                    if (j < s.Length && s[j] == '(')
                    {
                        j++;
                    }
                    else if (IsPercent28(s, j))
                    {
                        hasEncodedChars = true;
                        j += 3;
                    }
                    else
                    {
                        return false;
                    }

                    int parenDepth = 1;
                    bool inQuote = false;

                    while (j < s.Length && parenDepth > 0)
                    {
                        char ch = s[j];

                        if (inQuote)
                        {
                            // Raw quote (escape or close)
                            if (ch == '\'')
                            {
                                if (j + 1 < s.Length && s[j + 1] == '\'')
                                {
                                    j += 2; // Raw escape: ''
                                    continue;
                                }
                                else if (j + 1 < s.Length && IsPercent27(s, j + 1))
                                {
                                    j += 4; // Mixed escaped pair: '%27
                                    hasEncodedChars = true;
                                    continue;
                                }

                                // Closing raw quote
                                inQuote = false;
                                j++;
                                continue;
                            }

                            // Encoded quote (escape or close)
                            if (IsPercent27(s, j))
                            {
                                hasEncodedChars = true;

                                if (j + 3 < s.Length && IsPercent27(s, j + 3))
                                {
                                    j += 6; // Encoded escape: %27%27
                                    continue;
                                }
                                else if (j + 3 < s.Length && s[j + 3] == '\'')
                                {
                                    j += 4; // Mixed escaped pair: %27'
                                    continue;
                                }

                                // Closing encoded quote
                                inQuote = false;
                                j += 3;
                                continue;
                            }

                            // Any other escape sequence
                            if (TryConsumeEncodedChar(s, j, out int nextIndex))
                            {
                                hasEncodedChars = true;
                                j = nextIndex;
                                continue;
                            }

                            j++;
                            continue;
                        }
                        else
                        {
                            // Slash outside quote invalidates the block (cannot protect it)
                            if (ch == '/')
                            {
                                return false; // Slash becomes a segment separator
                            }

                            // Enter quote
                            if (ch == '\'')
                            {
                                inQuote = true;
                                j++;
                                continue;
                            }

                            if (IsPercent27(s, j))
                            {
                                hasEncodedChars = true;
                                inQuote = true;
                                j += 3;
                                continue;
                            }

                            // Nested paren (rare but legal)
                            if (ch == '(')
                            {
                                parenDepth++;
                                j++;
                                continue;
                            }

                            if (ch == ')')
                            {
                                parenDepth--;
                                j++;
                                continue;
                            }

                            if (IsPercent28(s, j))
                            {
                                hasEncodedChars = true;
                                parenDepth++;
                                j += 3;
                                continue;
                            }

                            if (IsPercent29(s, j))
                            {
                                hasEncodedChars = true;
                                parenDepth--;
                                j += 3;
                                continue;
                            }

                            // Any other escape sequence
                            if (TryConsumeEncodedChar(s, j, out int nextIndex))
                            {
                                hasEncodedChars = true;
                                j = nextIndex;
                                continue;
                            }

                            // Any other char (including '/', ',', '=', spaces, ect)
                            j++;
                        }
                    }

                    if (parenDepth == 0 && !inQuote)
                    {
                        exclusiveEnd = j;
                        return true; // Found the matching closing paren
                    }

                    // Unbalanced or unterminated quote -> fail (caller will treat chars normally)
                    return false;
                }

                void AddSegmentIfAny()
                {
                    if (sb.Length == 0)
                    {
                        return;
                    }

                    // Depending on internal implementation, Uri.UnescapeDataString when there's no
                    // escaping might result into two strings so we optimize for that case, i.e.,
                    // one allocation for sb.ToString() and for Uri.UnescapeDataString
                    string raw = sb.ToString();
                    // Unescape percent-encoded characters for the final segment if necessary
                    string segment = hasEncodedChars ? Uri.UnescapeDataString(raw) : raw;

                    // Skip empties and any stray "/" segment
                    if (segment.Length > 0 && segment != "/")
                    {
                        if (segments.Count >= this.maxSegments)
                        {
                            throw new ODataException(SRResources.UriQueryPathParser_TooManySegments);
                        }

                        segments.Add(segment);
                    }

                    sb.Clear();
                    hasEncodedChars = false; // Reset for next segment
                }

                // Walk the path after the basePath and split on unquoted '/'
                // We don't want something like api/a'/b'/c to give us a'/b' and c instead of a', b' and c
                for (int i = startIndex; i < fullPath.Length;)
                {
                    char c = fullPath[i];

                    // Split on '/' unless inside a "protected" region (detected via lookahead)
                    if (c == '/')
                    {
                        // Path segment separator
                        AddSegmentIfAny();
                        i++; // Consume path segment separator
                        continue;
                    }

                    if (c == '(' || IsPercent28(fullPath, i))
                    {
                        // Attempt to consume a *balanced* parenthetical (key/function args). If successful,
                        // append it as a whole so that any '/' within quoted literals does not split the segment.
                        // If not successful (unbalanced/unterminated), fall back to literal scanning.
                        if (TryConsumeParenBlock(fullPath, i, out int endParen, out bool blockHasEncodedChars))
                        {
                            sb.Append(fullPath, i, endParen - i);
                            if (blockHasEncodedChars)
                            {
                                hasEncodedChars = true;
                            }

                            i = endParen;
                            continue;
                        }

                        // Fall through: treat as normal characters (unbalanced -> do not protect '/' enclosed in single quote)
                    }

                    // Copy escaped sequences as a unit, mark for unescape
                    if (c == '%')
                    {
                        hasEncodedChars = true; // Mark that we saw an escape sequence

                        if (i + 2 < fullPath.Length)
                        {
                            sb.Append(fullPath, i, 3);
                            i += 3; // %XX - consume entire escape sequence
                        }
                        else
                        {
                            // Malformed tail like "%" or "%X": append as-is, let UnescapeDataString handle that later
                            sb.Append(c);
                            i++;
                        }

                        continue;
                    }

                    // Regular character - just append
                    sb.Append(c);
                    i++; // Consume character
                }

                // Flush last segment
                AddSegmentIfAny();

                return segments;
            }
            catch (FormatException uriFormatException)
            {
                throw new ODataException(SRResources.UriQueryPathParser_SyntaxError, uriFormatException);
            }
        }
    }
}