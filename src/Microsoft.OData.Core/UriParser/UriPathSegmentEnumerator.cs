//---------------------------------------------------------------------
// <copyright file="UriPathSegmentEnumerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !PORTABLELIB
namespace Microsoft.OData.Core.Query
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// This class is responsible for turning an absolute Uri into a list of strings that represent the segments in the Uri path.
    /// It is also responsible for ensuring that the Uri has the correct base Uri.
    /// </summary>
    internal sealed class UriPathSegmentEnumerator
    {
        /// <summary>
        /// The maximum number of segments allowed.
        /// </summary>
        private readonly int maxSegments;

        /// <summary>
        /// Creates a UriPathSegmentEnumerator to enumerate segments of a Uri.
        /// </summary>
        /// <param name="maxSegments">The maximum number of segments allowed.</param>
        internal UriPathSegmentEnumerator(int maxSegments)
        {
            DebugUtils.CheckNoExternalCallers();
            this.maxSegments = maxSegments;
        }

        /// <summary>
        /// Returns list of segments in the specified path (eg: /abc/pqr -&gt; abc, pqr).
        /// </summary>
        /// <param name="absoluteUri">The absolute URI of the request.</param>
        /// <param name="serviceBaseUri">The service base URI for the request.</param>
        /// <returns>List of unescaped segments.</returns>
        internal List<string> EnumerateSegments(Uri absoluteUri, Uri serviceBaseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(absoluteUri != null, "absoluteUri != null");
            Debug.Assert(absoluteUri.IsAbsoluteUri, "absoluteRequestUri.IsAbsoluteUri(" + absoluteUri.IsAbsoluteUri + ")");
            Debug.Assert(serviceBaseUri != null, "serviceBaseUri != null");
            Debug.Assert(serviceBaseUri.IsAbsoluteUri, "serviceBaseUri.IsAbsoluteUri(" + serviceBaseUri + ")");

            //// This is a copy of the RequestUriProcessor.EnumerateSegments

            // COMPAT 29: Slash in key lookup breaks URI parser
            // TODO: The code below has a bug that / in the named values will be considered a segment separator
            //   so for example /Customers('abc/pqr') is treated as two segments, which is wrong.
            if (!UriUtils.UriInvariantInsensitiveIsBaseOf(serviceBaseUri, absoluteUri))
            {
                throw new ODataException(Strings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(absoluteUri, serviceBaseUri));
            }

            try
            {
                Uri uri = absoluteUri;
                int numberOfSegmentsToSkip = 0;

                // Skip over the base URI segments
#if SILVERLIGHT
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

                return segments;
            }
            catch (UriFormatException uriFormatException)
            {
                throw new ODataException(Strings.UriQueryPathParser_SyntaxError, uriFormatException);
            }
        }
    }
}
#endif