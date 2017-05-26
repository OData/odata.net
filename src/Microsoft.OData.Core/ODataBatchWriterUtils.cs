//---------------------------------------------------------------------
// <copyright file="ODataBatchWriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
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
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}; {1}={2}",
                MimeConstants.MimeMultipartMixed,
                ODataConstants.HttpMultipartBoundary,
                boundary);
        }

        /// <summary>
        /// Write the start boundary.
        /// </summary>
        /// <param name="writer">Writer to which the boundary needs to be written.</param>
        /// <param name="boundary">Boundary string.</param>
        /// <param name="firstBoundary">true if this is the first start boundary.</param>
        internal static void WriteStartBoundary(TextWriter writer, string boundary, bool firstBoundary)
        {
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
        /// <param name="missingStartBoundary">true if there was no start boundary written before this end boundary.</param>
        internal static void WriteEndBoundary(TextWriter writer, string boundary, bool missingStartBoundary)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(boundary != null, "boundary != null");

            // write the CRLF that belongs to the boundary (see RFC 2046, Section 5.1.1)
            // but only if it's not the only boundary, the new line is not required by the fisrt boundary
            // and we don't want to write it unless necessary.
            if (!missingStartBoundary)
            {
                writer.WriteLine();
            }

            // Note that we don't write a newline AFTER the end boundary since there's no need to.
            writer.Write("--{0}--", boundary);
        }

        /// <summary>
        /// Writes the headers, (optional) Content-ID and the request line.
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        /// <param name="httpMethod">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="baseUri">The service root Uri to be used for this request operation.</param>
        /// <param name="inChangeSetBound">Whether we are in ChangeSetBound.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <param name="payloadUriOption">The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        internal static void WriteRequestPreamble(
            TextWriter writer,
            string httpMethod,
            Uri uri,
            Uri baseUri,
            bool inChangeSetBound,
            string contentId,
            BatchPayloadUriOption payloadUriOption)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(uri != null, "uri != null");
            Debug.Assert(uri.IsAbsoluteUri || UriUtils.UriToString(uri).StartsWith("$", StringComparison.Ordinal), "uri.IsAbsoluteUri || uri.OriginalString.StartsWith(\"$\")");

            // write the headers
            WriteHeaders(writer, inChangeSetBound, contentId);

            // write separator line between headers and the request line
            writer.WriteLine();

            // write request line
            WriteRequestUri(writer, httpMethod, uri, baseUri, payloadUriOption);
        }

        /// <summary>
        /// Writes the headers and response line.
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        /// <param name="inChangeSetBound">Whether we are in ChangeSetBound.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        internal static void WriteResponsePreamble(TextWriter writer, bool inChangeSetBound, string contentId)
        {
            Debug.Assert(writer != null, "writer != null");

            // write the headers
            WriteHeaders(writer, inChangeSetBound, contentId);

            // write separator line between headers and the response line
            writer.WriteLine();
        }

        /// <summary>
        /// Writes the preamble for a change set (e.g., the content-type header).
        /// </summary>
        /// <param name="writer">Writer to write to.</param>
        /// <param name="changeSetBoundary">The boundary string to use for the change set.</param>
        internal static void WriteChangeSetPreamble(TextWriter writer, string changeSetBoundary)
        {
            Debug.Assert(changeSetBoundary != null, "changeSetBoundary != null");

            string multipartContentType = CreateMultipartMixedContentType(changeSetBoundary);
            writer.WriteLine("{0}: {1}", ODataConstants.ContentTypeHeader, multipartContentType);

            // write separator line between headers and first change set operation
            writer.WriteLine();
        }

        /// <summary>
        /// Writes the headers.
        /// </summary>
        /// <param name="writer">Writer to write headers.</param>
        /// <param name="inChangeSetBound">Whether we are in ChangeSetBound.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        private static void WriteHeaders(TextWriter writer, bool inChangeSetBound, string contentId)
        {
            writer.WriteLine("{0}: {1}", ODataConstants.ContentTypeHeader, MimeConstants.MimeApplicationHttp);
            writer.WriteLine("{0}: {1}", ODataConstants.ContentTransferEncoding, ODataConstants.BatchContentTransferEncoding);
            if (inChangeSetBound && contentId != null)
            {
                writer.WriteLine("{0}: {1}", ODataConstants.ContentIdHeader, contentId);
            }
        }

        /// <summary>
        /// Writes the request line.
        /// </summary>
        /// <param name="writer">Writer to write request uri.</param>
        /// <param name="httpMethod">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="baseUri">The service root Uri to be used for this request operation.</param>
        /// <param name="payloadUriOption">The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        private static void WriteRequestUri(TextWriter writer, string httpMethod, Uri uri, Uri baseUri, BatchPayloadUriOption payloadUriOption)
        {
            if (uri.IsAbsoluteUri)
            {
                string absoluteUriString = uri.AbsoluteUri;

                switch (payloadUriOption)
                {
                    case BatchPayloadUriOption.AbsoluteUri:
                        writer.WriteLine("{0} {1} {2}", httpMethod, UriUtils.UriToString(uri), ODataConstants.HttpVersionInBatching);
                        break;

                    case BatchPayloadUriOption.AbsoluteUriUsingHostHeader:
                        string absoluteResourcePath = absoluteUriString.Substring(absoluteUriString.IndexOf('/', absoluteUriString.IndexOf("//", StringComparison.Ordinal) + 2));
                        writer.WriteLine("{0} {1} {2}", httpMethod, absoluteResourcePath, ODataConstants.HttpVersionInBatching);
                        writer.WriteLine("Host: {0}:{1}", uri.Host, uri.Port);
                        break;

                    case BatchPayloadUriOption.RelativeUri:
                        Debug.Assert(baseUri != null, "baseUri != null");
                        string baseUriString = UriUtils.UriToString(baseUri);
                        Debug.Assert(absoluteUriString.StartsWith(baseUriString, StringComparison.Ordinal), "absoluteUriString.StartsWith(baseUriString)");
                        string relativeResourcePath = absoluteUriString.Substring(baseUriString.Length);
                        writer.WriteLine("{0} {1} {2}", httpMethod, relativeResourcePath, ODataConstants.HttpVersionInBatching);
                        break;
                }
            }
            else
            {
                writer.WriteLine("{0} {1} {2}", httpMethod, UriUtils.UriToString(uri), ODataConstants.HttpVersionInBatching);
            }
        }
    }
}
