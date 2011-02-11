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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    #endregion Namespaces.

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
        internal static void WriteStartBoundary(StreamWriter writer, string boundary)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(boundary != null, "boundary != null");

            // write the CRLF that belongs to the boundary (see RFC 2046, Section 5.1.1)
            writer.WriteLine();

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

            writer.WriteLine("--{0}--", boundary);
        }

        /// <summary>
        /// Writes the headers, (optional) Content-ID and the request line
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The (optional) content ID to be included in this request operation.</param>
        internal static void WriteRequestPreamble(StreamWriter writer, HttpMethod method, Uri uri, string contentId)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(uri != null, "uri != null");
            Debug.Assert(uri.IsAbsoluteUri, "uri.IsAbsoluteUri");

            // write the headers
            writer.WriteLine("{0}: {1}", ODataHttpHeaders.ContentType, MimeConstants.MimeApplicationHttp);
            writer.WriteLine("{0}: {1}", HttpConstants.ContentTransferEncoding, HttpConstants.BatchRequestContentTransferEncoding);

            // write the optional content ID
            if (contentId != null)
            {
                writer.WriteLine("{0}: {1}", HttpConstants.ContentId, contentId);
            }

            // write separator line between headers and the request line
            writer.WriteLine();

            writer.WriteLine("{0} {1} {2}", method.ToText(), uri.AbsoluteUri, HttpConstants.HttpVersionInBatching);
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
        /// Converts the specified URI into an absolute URI; if the URI is already absolute and the <paramref name="baseUri"/>
        /// is not null it also verifies that <paramref name="baseUri"/> is the base of <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <param name="baseUri">The base Uri to use.</param>
        /// <returns>An absolute URI which is either the specified <paramref name="uri"/> if it was absolute,
        /// or it's a combination of the <paramref name="baseUri"/> and the relative <paramref name="uri"/>.</returns>
        /// <remarks>
        /// This method will fail if the specified <paramref name="uri"/> is relative and there's no base URI available
        /// or if the <paramref name="baseUri"/> is not the base of <paramref name="uri"/>.
        /// </remarks>
        internal static Uri BuildAbsoluteUri(Uri uri, Uri baseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            Uri resultUri;
            if (uri.IsAbsoluteUri)
            {
                resultUri = uri;
            }
            else
            {
                if (baseUri == null)
                {
                    throw new ODataException(Strings.ODataBatchWriterUtils_RelativeUriUsedWithoutBaseUriSpecified(UriUtils.UriToString(uri)));
                }

                resultUri = UriUtils.UriToAbsoluteUri(baseUri, uri);
            }

            return resultUri;
        }
    }
}
