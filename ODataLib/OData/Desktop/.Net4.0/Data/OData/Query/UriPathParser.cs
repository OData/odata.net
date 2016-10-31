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
#if (SILVERLIGHT || PORTABLELIB) && !WINRT
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
