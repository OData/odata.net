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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
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
        /// <param name="urlResolver">An optional custom URL resolver to resolve URLs for writing them into the payload.</param>
        /// <returns>An URI to be used in the request line of a batch request operation. It uses the <paramref name="urlResolver"/>
        /// first and falls back to the defaullt URI building schema if the no URL resolver is specified or the URL resolver
        /// returns null. In the default scheme, the method either returns the specified <paramref name="uri"/> if it was absolute,
        /// or it's combination with the <paramref name="baseUri"/> if it was relative.</returns>
        /// <remarks>
        /// This method will fail if no custom resolution is implemented and the specified <paramref name="uri"/> is 
        /// relative and there's no base URI available.
        /// </remarks>
        internal static Uri CreateOperationRequestUri(Uri uri, Uri baseUri, IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            Uri resultUri;
            if (urlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                resultUri = urlResolver.ResolveUrl(baseUri, uri);
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
                    string errorMessage = UriUtilsCommon.UriToString(uri).StartsWith("$", StringComparison.Ordinal)
                        ? Strings.ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified(UriUtilsCommon.UriToString(uri))
                        : Strings.ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified(UriUtilsCommon.UriToString(uri));
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
    }
}
