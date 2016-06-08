//---------------------------------------------------------------------
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