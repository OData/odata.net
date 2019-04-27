﻿//---------------------------------------------------------------------
// <copyright file="UriPathParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            if (serviceBaseUri == null)
            {
                Debug.Assert(!fullUri.IsAbsoluteUri, "fullUri must be relative Uri");
                serviceBaseUri = UriUtils.CreateMockAbsoluteUri();
                fullUri = UriUtils.CreateMockAbsoluteUri(fullUri);
            }

            if (!UriUtils.UriInvariantInsensitiveIsBaseOf(serviceBaseUri, fullUri))
            {
                throw new ODataException(Strings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(fullUri, serviceBaseUri));
            }

            // COMPAT 29: Slash in key lookup breaks URI parser
            // TODO: The code below has a bug that / in the named values will be considered a segment separator
            //   so for example /Customers('abc/pqr') is treated as two segments, which is wrong.
            try
            {
                Uri uri = fullUri;
                int numberOfSegmentsToSkip = 0;

                // Skip over the base URI segments
#if !ORCAS
                // need to calculate the number of segments to skip in the full
                // uri (so that we can skip over http://blah.com/basePath for example,
                // get only the odata specific parts of the path).
                //
                // because of differences in system.uri between portable lib and
                // the desktop library, we need to handle this differently.
                // in this case we get the number of segments to skip as simply
                // then number of tokens in the serviceBaseUri split on slash, with
                // length - 1 since its a zero based array.
                numberOfSegmentsToSkip = serviceBaseUri.AbsolutePath.Split('/').Length - 1;
                string[] uriSegments = uri.AbsolutePath.Split('/');
#else
                numberOfSegmentsToSkip = serviceBaseUri.Segments.Length;
                string[] uriSegments = uri.Segments;
#endif

                int escapedStart = -1;
                List<string> segments = new List<string>();
                for (int i = numberOfSegmentsToSkip; i < uriSegments.Length; i++)
                {
                    string segment = uriSegments[i];

                    // Skip the empty segment or the "/" segment
                    if (segment.Length == 0 || segment == "/")
                    {
                        continue;
                    }

                    // When we use "uri.Segments" to get the segments,
                    // The segment element includes the "/", we should remove that.
                    if (segment[segment.Length - 1] == '/')
                    {
                        segment = segment.Substring(0, segment.Length - 1);
                    }

                    if (segments.Count == this.maxSegments)
                    {
                        throw new ODataException(Strings.UriQueryPathParser_TooManySegments);
                    }

                    // Handle the "root...::/{xyz}"
                    if (segment.Length >= 2 && segment.EndsWith("::", StringComparison.Ordinal))
                    {
                        // It should be the terminal of the provious escape segment and the start of next escape semgent.
                        // Otherwise, it's an invalid Uri.
                        if (escapedStart == -1)
                        {
                            throw new ODataException(Strings.UriQueryPathParser_InvalidEscapeUri(segment));
                        }
                        else
                        {
                            string value = String.Join("/", uriSegments, escapedStart, i - escapedStart + 1);
                            segments.Add(":" + value.Substring(0, value.Length - 1)); // because the last one has "::", remove one.
                            escapedStart = i + 1;
                        }
                    }
                    else if (segment.Length >= 1 && segment[segment.Length - 1] == ':')
                    {
                        // root:/{abc}....
                        if (escapedStart == -1)
                        {
                            if (segment != ":")
                            {
                                segments.Add(segment.Substring(0, segment.Length - 1)); // remove the last ':'
                            }

                            escapedStart = i + 1;
                        }
                        else
                        {
                            // root:/{abc}:....
                            string escapedSegment = ":" + String.Join("/", uriSegments, escapedStart, i - escapedStart + 1); // the last has one ":";
                            segments.Add(escapedSegment);
                            escapedStart = -1;
                        }
                    }
                    else
                    {
                        // if we didn't find a starting escape, the current segment is normal segment, accept it.
                        // otherwise, it's part of the escape, skip it and process it when we find the ending delimiter.
                        if (escapedStart == -1)
                        {
                            segments.Add(Uri.UnescapeDataString(segment));
                        }
                    }
                }

                if (escapedStart != -1 && escapedStart < uriSegments.Length)
                {
                    string escapedSegment = ":" + String.Join("/", uriSegments, escapedStart, uriSegments.Length - escapedStart);
                    segments.Add(escapedSegment); // We should not use "segments.Add(Uri.UnescapeDataString(escapedSegment));" to keep the orignal string.
                }

                return segments.ToArray();
            }
#if !ORCAS
            catch (FormatException uriFormatException)
#else
            catch (UriFormatException uriFormatException)
#endif
            {
                throw new ODataException(Strings.UriQueryPathParser_SyntaxError, uriFormatException);
            }
        }
    }
}