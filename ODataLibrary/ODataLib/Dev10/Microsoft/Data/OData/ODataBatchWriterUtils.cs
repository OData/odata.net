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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the ODataBatchWriter.
    /// </summary>
    internal static class ODataBatchWriterUtils
    {
        /// <summary>
        /// Creates a new batch boundary string based on a randomly created GUID.
        /// </summary>
        /// <param name="isResponse">A flag indicating whether the boundary should be created for a request or a resonse.</param>
        /// <returns>The newly created batch boundary as string.</returns>
        internal static string CreateBatchBoundary(bool isResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            string template = isResponse ? ODataConstants.BatchResponseBoundaryTemplate : ODataConstants.BatchRequestBoundaryTemplate;
            return string.Format(CultureInfo.InvariantCulture, template, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Creates a new changeset boundary string based on a randomly created GUID.
        /// </summary>
        /// <param name="isResponse">A flag indicating whether the boundary should be created for a request or a resonse.</param>
        /// <returns>The newly created changeset boundary as string.</returns>
        internal static string CreateChangeSetBoundary(bool isResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            string template = isResponse ? ODataConstants.ResponseChangeSetBoundaryTemplate : ODataConstants.RequestChangeSetBoundaryTemplate;
            return string.Format(CultureInfo.InvariantCulture, template, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Creates the multipart/mixed content type with the specified boundary (if any).
        /// </summary>
        /// <param name="boundary">The boundary to be used for this operation or null if no boundary should be included.</param>
        /// <returns>The multipart/mixed content type with the specified boundary (if any).</returns>
        internal static string CreateMultipartMixedContentType(string boundary)
        {
            DebugUtils.CheckNoExternalCallers();
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}; {1}={2}",
                MimeConstants.MimeMultipartMixed,
                HttpConstants.HttpMultipartBoundary,
                boundary);
        }

        /// <summary>
        /// Write the start boundary.
        /// </summary>
        /// <param name="writer">Writer to which the boundary needs to be written.</param>
        /// <param name="boundary">Boundary string.</param>
        /// <param name="firstBoundary">true if this is the first start boundary.</param>
        internal static void WriteStartBoundary(StreamWriter writer, string boundary, bool firstBoundary)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(boundary != null, "boundary != null");

            // write the CRLF that belongs to the boundary (see RFC 2046, Section 5.1.1)
            // but only if it's not the first boundary, the new line is not required by the boundary
            // and we don't want to write it unless necessary.
            if (!firstBoundary)
            {
                writer.WriteLine();
            }

            writer.WriteLine("--{0}", boundary);
        }

        /// <summary>
        /// Write the end boundary.
        /// </summary>
        /// <param name="writer">Writer to which the end boundary needs to be written.</param>
        /// <param name="boundary">Boundary string.</param>
        internal static void WriteEndBoundary(StreamWriter writer, string boundary)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(boundary != null, "boundary != null");

            // write the CRLF that belongs to the boundary (see RFC 2046, Section 5.1.1)
            writer.WriteLine();

            // Note that we don't write a newline AFTER the end boundary since there's no need to.
            writer.Write("--{0}--", boundary);
        }

        /// <summary>
        /// Writes the headers, (optional) Content-ID and the request line
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        internal static void WriteRequestPreamble(StreamWriter writer, HttpMethod method, Uri uri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(uri != null, "uri != null");
            Debug.Assert(uri.IsAbsoluteUri || UriUtilsCommon.UriToString(uri).StartsWith("$", StringComparison.Ordinal), "uri.IsAbsoluteUri || uri.OriginalString.StartsWith(\"$\")");

            // write the headers
            writer.WriteLine("{0}: {1}", ODataHttpHeaders.ContentType, MimeConstants.MimeApplicationHttp);
            writer.WriteLine("{0}: {1}", HttpConstants.ContentTransferEncoding, HttpConstants.BatchRequestContentTransferEncoding);

            // write separator line between headers and the request line
            writer.WriteLine();

            writer.WriteLine("{0} {1} {2}", method.ToText(), UriUtilsCommon.UriToString(uri), HttpConstants.HttpVersionInBatching);
        }

        /// <summary>
        /// Writes the headers and response line.
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        internal static void WriteResponsePreamble(StreamWriter writer)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");

            // write the headers
            writer.WriteLine("{0}: {1}", ODataHttpHeaders.ContentType, MimeConstants.MimeApplicationHttp);
            writer.WriteLine("{0}: {1}", HttpConstants.ContentTransferEncoding, HttpConstants.BatchRequestContentTransferEncoding);

            // write separator line between headers and the response line
            writer.WriteLine();
        }

        /// <summary>
        /// Writes the preamble for a change set (e.g., the content-type header).
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        /// <param name="changeSetBoundary">The boundary string to use for the change set.</param>
        internal static void WriteChangeSetPreamble(StreamWriter writer, string changeSetBoundary)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(changeSetBoundary != null, "changeSetBoundary != null");

            string multipartContentType = CreateMultipartMixedContentType(changeSetBoundary);
            writer.WriteLine("{0}: {1}", ODataHttpHeaders.ContentType, multipartContentType);

            // write separator line between headers and first change set operation
            writer.WriteLine();
        }

        /// <summary>
        /// Creates the URI for a batch request operation.
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <param name="baseUri">The base Uri to use.</param>
        /// <param name="urlResolver">An optional custom URL resolver to resolve URLs for writing them into the payload.</param>
        /// <returns>An URI to be used in the request line of a batch request operation. It uses the <paramref name="urlResolver"/>
        /// first and falls back to the defaullt URI building schema if the no URL resolver is specified or the URL resolver
        /// returns null. In the default scheme, the method either returns the specified <paramref name="uri"/> if it was absolute,
        /// or it's combination with the <paramref name="baseUri"/> if it was relative.</returns>
        /// <remarks>
        /// This method will fail if no custom resolution is implemented and the specified <paramref name="uri"/> is 
        /// relative and there's no base URI available.
        /// </remarks>
        internal static Uri CreateOperationRequestUri(Uri uri, Uri baseUri, ODataBatchUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            Uri resultUri;
            if (urlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = urlResolver.ResolveUrl(uri);
                if (resultUri != null)
                {
                    return resultUri;
                }
            }

            if (uri.IsAbsoluteUri)
            {
                resultUri = uri;
            }
            else
            {
                if (baseUri == null)
                {
                    throw new ODataException(Strings.ODataBatchWriterUtils_RelativeUriUsedWithoutBaseUriSpecified(UriUtilsCommon.UriToString(uri)));
                }

                resultUri = UriUtils.UriToAbsoluteUri(baseUri, uri);
            }

            return resultUri;
        }
    }
}
