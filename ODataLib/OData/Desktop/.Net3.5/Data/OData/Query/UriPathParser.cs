//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Collections.Generic;

    #endregion Namespaces

    /// <summary>
    /// Parser which consumes the URI path and produces the lexical object model.
    /// </summary>
    internal sealed class UriPathParser
    {
        /// <summary>
        /// The maximum number of segments allowed.
        /// </summary>
        private readonly int maxSegments;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxSegments">The maximum number of segments for each part of the query.</param>
        internal UriPathParser(int maxSegments)
        {
            DebugUtils.CheckNoExternalCallers();

            this.maxSegments = maxSegments;
        }

        /// <summary>
        /// Parses the <paramref name="escapedRelativePathUri"/> and returns a list of strings for each segment.
        /// </summary>
        /// <param name="escapedRelativePathUri">The relative URI which holds the query to parse.</param>
        /// <returns>a list of strings for each segment in the uri.</returns>
        internal string[] ParsePath(string escapedRelativePathUri)
        {
            DebugUtils.CheckNoExternalCallers();

            if (escapedRelativePathUri == null || String.IsNullOrEmpty(escapedRelativePathUri.Trim()))
            {
                return new string[0];
            }

            // TODO: The code below has a bug that / in the named values will be considered a segment separator
            //   so for example /Customers('abc/pqr') is treated as two segments, which is wrong.
            string[] segments = escapedRelativePathUri.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // TODO lookup astoria max segments...
            if (segments.Length >= this.maxSegments)
            {
                throw new ODataException(Strings.UriQueryPathParser_TooManySegments);
            }

            return segments;
        }

        /// <summary>
        /// Returns list of segments in the specified path (eg: /abc/pqr -&gt; abc, pqr).
        /// </summary>
        /// <param name="absoluteUri">The absolute URI of the request.</param>
        /// <param name="serviceBaseUri">The service base URI for the request.</param>
        /// <returns>List of unescaped segments.</returns>
        internal ICollection<string> ParsePathIntoSegments(Uri absoluteUri, Uri serviceBaseUri)
        {
            DebugUtils.CheckNoExternalCallers(); 

            if (!UriUtils.UriInvariantInsensitiveIsBaseOf(serviceBaseUri, absoluteUri))
            {
                throw new ODataException(Strings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(absoluteUri, serviceBaseUri));
            }

            try
            {
                Uri uri = absoluteUri;
                int numberOfSegmentsToSkip = 0;

                // Skip over the base URI segments
#if SILVERLIGHT || PORTABLELIB
                numberOfSegmentsToSkip = serviceBaseUri.AbsolutePath.Split('/').Length;
                string[] uriSegments = uri.AbsolutePath.Split('/');
#else
                numberOfSegmentsToSkip = serviceBaseUri.Segments.Length;
                string[] uriSegments = uri.Segments;
#endif

                List<string> segments = new List<string>();
                for (int i = numberOfSegmentsToSkip; i < uriSegments.Length; i++)
                {
                    string segment = uriSegments[i];
                    if (segment.Length != 0 && segment != "/")
                    {
                        if (segment[segment.Length - 1] == '/')
                        {
                            segment = segment.Substring(0, segment.Length - 1);
                        }

                        if (segments.Count == this.maxSegments)
                        {
                            throw new ODataException(Strings.UriQueryPathParser_TooManySegments);
                        }

                        segments.Add(Uri.UnescapeDataString(segment));
                    }
                }

                return segments.ToArray();
            }
#if PORTABLELIB
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
