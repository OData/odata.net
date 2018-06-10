//---------------------------------------------------------------------
// <copyright file="ODataBatchUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.OData.JsonLight;

    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the ODataBatchWriter and ODataBatchReader (and related classes).
    /// </summary>
    internal static class ODataBatchUtils
    {
        /// <summary>
        /// Creates the URI for a batch request operation.
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <param name="baseUri">The base Uri to use.</param>
        /// <param name="payloadUriConverter">An optional custom URL converter to convert URLs for writing them into the payload.</param>
        /// <returns>An URI to be used in the request line of a batch request operation. It uses the <paramref name="payloadUriConverter"/>
        /// first and falls back to the default URI building schema if the no URL resolver is specified or the URL resolver
        /// returns null. In the default scheme, the method either returns the specified <paramref name="uri"/> if it was absolute,
        /// or it's combination with the <paramref name="baseUri"/> if it was relative.</returns>
        /// <remarks>
        /// This method will fail if no custom resolution is implemented and the specified <paramref name="uri"/> is
        /// relative and there's no base URI available.
        /// </remarks>
        internal static Uri CreateOperationRequestUri(Uri uri, Uri baseUri, IODataPayloadUriConverter payloadUriConverter)
        {
            Debug.Assert(uri != null, "uri != null");

            Uri resultUri;
            if (payloadUriConverter != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = payloadUriConverter.ConvertPayloadUri(baseUri, uri);
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
                    string errorMessage = UriUtils.UriToString(uri).StartsWith("$", StringComparison.Ordinal)
                        ? Strings.ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified(UriUtils.UriToString(uri))
                        : Strings.ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified(UriUtils.UriToString(uri));
                    throw new ODataException(errorMessage);
                }

                resultUri = UriUtils.UriToAbsoluteUri(baseUri, uri);
            }

            return resultUri;
        }

        /// <summary>
        /// Creates a batch operation stream from the specified batch stream.
        /// </summary>
        /// <param name="batchReaderStream">The batch stream to create the operation read stream for.</param>
        /// <param name="headers">The headers of the current part; based on the header we create different, optimized stream implementations.</param>
        /// <param name="operationListener">The operation listener to be passed to the newly created read stream.</param>
        /// <returns>A new <see cref="ODataBatchOperationReadStream"/> instance.</returns>
        internal static ODataBatchOperationReadStream CreateBatchOperationReadStream(
            ODataBatchReaderStream batchReaderStream,
            ODataBatchOperationHeaders headers,
            IODataBatchOperationListener operationListener)
        {
            Debug.Assert(batchReaderStream != null, "batchReaderStream != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            // See whether we have a Content-Length header
            string contentLengthValue;
            if (headers.TryGetValue(ODataConstants.ContentLengthHeader, out contentLengthValue))
            {
                int length = Int32.Parse(contentLengthValue, CultureInfo.InvariantCulture);
                if (length < 0)
                {
                    throw new ODataException(Strings.ODataBatchReaderStream_InvalidContentLengthSpecified(contentLengthValue));
                }

                return ODataBatchOperationReadStream.Create(batchReaderStream, operationListener, length);
            }

            return ODataBatchOperationReadStream.Create(batchReaderStream, operationListener);
        }

        /// <summary>
        /// Creates a batch operation write stream over the specified output stream.
        /// </summary>
        /// <param name="outputStream">The output stream to create the operation write stream over.</param>
        /// <param name="operationListener">The operation listener to be passed to the newly created write stream.</param>
        /// <returns>A new <see cref="ODataBatchOperationWriteStream"/> instance.</returns>
        internal static ODataBatchOperationWriteStream CreateBatchOperationWriteStream(
            Stream outputStream,
            IODataBatchOperationListener operationListener)
        {
            Debug.Assert(outputStream != null, "outputStream != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            return new ODataBatchOperationWriteStream(outputStream, operationListener);
        }

        /// <summary>
        /// Grows the specified byte array by the specified amount.
        /// </summary>
        /// <param name="buffer">The byte array to grow.</param>
        /// <param name="numberOfBytesInBuffer">The number of bytes currently in the buffer.</param>
        /// <param name="requiredByteCount">The number of bytes to be added to the array.</param>
        internal static void EnsureArraySize(ref byte[] buffer, int numberOfBytesInBuffer, int requiredByteCount)
        {
            Debug.Assert(buffer != null, "bytes != null");
            Debug.Assert(numberOfBytesInBuffer >= 0, "numberOfBytesInBuffer >= 0");
            Debug.Assert(requiredByteCount >= 0, "byteCount >= 0");

            int remainingUnusedBytesInBuffer = buffer.Length - numberOfBytesInBuffer;
            if (requiredByteCount <= remainingUnusedBytesInBuffer)
            {
                // Still enough room in the buffer to store all required bytes
                return;
            }

            int numberOfAdditionalBytesNeeded = requiredByteCount - remainingUnusedBytesInBuffer;
            Debug.Assert(numberOfAdditionalBytesNeeded > 0, "Expected a positive number of additional bytes.");

            // NOTE: grow the array only by the exact number of needed bytes; we expect the
            //       caller to specify a larger required byte count to grow the array more.
            byte[] oldBytes = buffer;
            buffer = new byte[buffer.Length + numberOfAdditionalBytesNeeded];
            Buffer.BlockCopy(oldBytes, 0, buffer, 0, numberOfBytesInBuffer);
        }

        /// <summary>
        /// Validates that the uri's reference of $requestId, if used, is one of the depends-on requests.
        /// The Uri can be either absolute or relative.
        /// Exception is thrown if the request Id reference is not found in the list of depends-on requests.
        /// </summary>
        /// <param name="uri">The Uri to validate the request Id reference.</param>
        /// <param name="dependsOnRequestIds">Enumeration of request Ids used to lookup the request Id reference.</param>
        /// <param name="baseUri">The baseUri used for validation.</param>
        internal static void ValidateReferenceUri(Uri uri, IEnumerable<string> dependsOnRequestIds, Uri baseUri)
        {
            Debug.Assert(uri != null, "uri != null");

            if (UriUtils.UriToString(uri).IndexOf('$') == -1)
            {
                // uri does not use $requestId,
                return;
            }

            string relativePath = null;
            if (uri.IsAbsoluteUri)
            {
                if (baseUri == null)
                {
                    // The absolution Uri can contain $ character followed by name of system resource
                    // such as $all, $metadata, etc, along with an unspecified baseUri. In such cases there are
                    // not reference Uri to validate.
                    return;
                }

                string baseUriString = UriUtils.UriToString(baseUri);
                if (!uri.AbsoluteUri.StartsWith(baseUriString, StringComparison.Ordinal))
                {
                    throw new ODataException(Strings.ODataBatchReader_AbsoluteURINotMatchingBaseUri(
                        uri.AbsoluteUri, baseUriString));
                }

                relativePath = uri.AbsoluteUri.Substring(baseUriString.Length);
            }
            else
            {
                relativePath = UriUtils.UriToString(uri);
            }

            // Trim the starting forward slashes as needed.
            while (relativePath.StartsWith("/", StringComparison.Ordinal))
            {
                relativePath = relativePath.Substring(1);
            }

            if (relativePath.Length > 0 && relativePath[0] == '$')
            {
                // If the relative path starts with '$', find the request id reference optionally ended by '/'.
                int idx = relativePath.IndexOf('/', 1);

                string referenceId = idx > 0
                    ? relativePath.Substring(1, idx - 1)
                    : relativePath.Substring(1);

                // Validate that dependsOn contains the Id reference.
                // Note that the dependsOnRequestIds here is already flattened and consists of individual request
                // ids, therefore it should contain the request id referenced by the Uri.
                if (dependsOnRequestIds == null || !dependsOnRequestIds.Contains(referenceId))
                {
                    throw new ODataException(Strings.ODataBatchReader_ReferenceIdNotIncludedInDependsOn(
                        referenceId, UriUtils.UriToString(uri),
                        dependsOnRequestIds != null ? string.Join(",", dependsOnRequestIds.ToArray()) : "null"));
                }
            }
        }
    }
}
