//---------------------------------------------------------------------
// <copyright file="UriPathParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                StringBuilder sb = new StringBuilder(Math.Min(256, Math.Max(0, fullPath.Length - startIndex)));
                bool inQuotes = false;
                bool hasEscapedSequence = false;

                // Local helpers
                static bool IsPercent27(string s, int i) // %27 = single quote encoded
                {
                    // Matches "%27" at position i
                    return i + 2 < s.Length
                        && s[i] == '%'
                        && s[i + 1] == '2'
                        && s[i + 2] == '7';
                }

                static bool IsDoublePercent27(string s, int i)
                {
                    return IsPercent27(s, i) && IsPercent27(s, i + 3);
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
                    string rawString = sb.ToString();
                    // Unescape percent-encoded characters for the final segment if necessary
                    string segment = hasEscapedSequence ? Uri.UnescapeDataString(rawString) : rawString;

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
                    hasEscapedSequence = false; // Reset for next segment
                }

                // Walk the path after the basePath and split on unquoted '/'
                for (int i = startIndex; i < fullPath.Length;)
                {
                    char c = fullPath[i];

                    // Per the OData V4 spec, forward slashes that are not path separators must be percent-encoded,
                    // even when inside quoted literals. This would allow simple parsing by splitting on '/'.
                    // Spec reference: https://docs.oasis-open.org/odata/odata/v4.0/errata03/os/complete/part2-url-conventions/odata-v4.0-errata03-os-part2-url-conventions-complete.html#_Toc453752335
                    //
                    // In practice, some clients (e.g., Excel or other external integrations) emit unencoded slashes inside quoted literals.
                    // To ensure compatibility, we must handle both encoded and unencoded cases.

                    // Treat '/' as path segment separators only when not inside quotes
                    if (!inQuotes && c == '/')
                    {
                        AddSegmentIfAny();
                        i++; // Consume path segment separator
                        continue;
                    }

                    // Literal quote handling (e.g., People('foo/bar'))
                    if (c == '\'')
                    {
                        // OData escape inside quotes: double single-quote (e.g. People('O''Neal'))
                        if (inQuotes && i + 1 < fullPath.Length && fullPath[i + 1] == '\'')
                        {
                            sb.Append('\'').Append('\''); // Zero string allocations; Better than: sb.Append("''");
                            i += 2; // Consume both quotes
                            continue;
                        }

                        inQuotes = !inQuotes; // Toggle quote state
                        sb.Append(c); // Keep the quote in the segment
                        i++; // Consume quote
                        continue;
                    }

                    // Percent-encoded single quote handling (e.g., People(%27foo/bar%27))
                    if (c == '%')
                    {
                        // Double-encoded quote: %27%27 = '' (escaped single quote inside quoted literal - e.g. People(%27O%27%27Neil%27) or People('O%27%27Neil'))
                        if (IsDoublePercent27(fullPath, i))
                        {
                            // Preserve as-is; don't toggle quote state
                            sb.Append(fullPath, i, 6); // Copy %27%27 directly from source
                            i += 6; // Consume both %27
                            hasEscapedSequence = true; // Mark that we saw an escape sequence
                            continue;
                        }

                        // Single-encoded quote: %27 = ' (quote start/end - e.g. People(%27foo/bar%27))
                        if (IsPercent27(fullPath, i))
                        {
                            inQuotes = !inQuotes; // Toggle quote state
                            sb.Append(fullPath, i, 3); // Copy %27 directly from source
                            i += 3; // Consume %27
                            hasEscapedSequence = true; // Mark that we saw an escape sequence
                            continue;
                        }

                        // Any other percent-escape (e.g., %2F for '/') - just append as-is
                        if (i + 2 < fullPath.Length)
                        {
                            sb.Append(fullPath, i, 3);
                            i += 3; // Consume entire escape sequence
                            hasEscapedSequence = true; // Mark that we saw an escape sequence
                            continue;
                        }
                        else
                        {
                            // Malformed percent-escape - be conservative and append what remains. Let Uri.UnescapeDataString handle any errors.
                            sb.Append(c);
                            i++; // Consume '%'
                            hasEscapedSequence = true; // To ensure that UnescapeDataString runs and throws
                            continue;
                        }
                    }

                    // Regular character - just append
                    sb.Append(c);
                    i++; // Consume character
                }

                // Flush last segment
                AddSegmentIfAny();

                // If we ended while still in quotes, that's a syntax error
                if (inQuotes)
                {
                    throw new ODataException(SRResources.UriQueryPathParser_SyntaxError);
                }

                return segments;
            }
            catch (FormatException uriFormatException)
            {
                throw new ODataException(SRResources.UriQueryPathParser_SyntaxError, uriFormatException);
            }
        }
    }
}