//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

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
            this.maxSegments = maxSegments;
        }

        /// <summary>
        /// Parses the <paramref name="escapedRelativePathUri"/> and returns a list of strings for each segment.
        /// </summary>
        /// <param name="escapedRelativePathUri">The relative URI which holds the query to parse.</param>
        /// <returns>a list of strings for each segment in the uri.</returns>
        internal string[] ParsePath(string escapedRelativePathUri)
        {
            if (escapedRelativePathUri == null || String.IsNullOrEmpty(escapedRelativePathUri.Trim()))
            {
                return new string[0];
            }

            // TODO: The code below has a bug that / in the named values will be considered a segment separator
            //   so for example /Customers('abc/pqr') is treated as two segments, which is wrong.
            string[] segments = escapedRelativePathUri.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // TODO: lookup astoria max segments...
            if (segments.Length >= this.maxSegments)
            {
                throw new ODataException(Strings.UriQueryPathParser_TooManySegments);
            }

            return segments;
        }

        /// <summary>
        /// Returns list of segments in the specified path (eg: /abc/pqr -&gt; abc, pqr).
        /// </summary>
        /// <param name="fullUri">The full URI of the request.</param>
        /// <param name="serviceBaseUri">The service base URI for the request.</param>
        /// <returns>List of unescaped segments.</returns>
        internal ICollection<string> ParsePathIntoSegments(Uri fullUri, Uri serviceBaseUri)
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
