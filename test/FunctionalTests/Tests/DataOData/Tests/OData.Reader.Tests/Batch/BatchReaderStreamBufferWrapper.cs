//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamBufferWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper of ODataBatchReaderStreamBuffer to expose the internal API.
    /// </summary>
    public sealed class BatchReaderStreamBufferWrapper
    {
        /// <summary>The size of the look-ahead buffer.</summary>
        public const int BufferLength = 8000;

        /// <summary>
        /// The type of the stream buffer class in the product code.
        /// </summary>
        private static readonly Type streamBufferType = typeof(ODataBatchReader).Assembly.GetType("Microsoft.OData.ODataBatchReaderStreamBuffer");

        /// <summary>
        /// The stream buffer instance from the product code.
        /// </summary>
        private readonly object batchReaderStreamBuffer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BatchReaderStreamBufferWrapper()
        {
            this.batchReaderStreamBuffer = ReflectionUtils.CreateInstance(streamBufferType);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchReaderStream">The batch reader stream instance to get the buffer from.</param>
        public BatchReaderStreamBufferWrapper(object batchReaderStream)
        {
            this.batchReaderStreamBuffer = ReflectionUtils.GetField(batchReaderStream, "BatchBuffer");
        }

        /// <summary>
        /// An enumeration representing the result of a scan operation through 
        /// the batch reader stream's buffer.
        /// </summary>
        public enum ODataBatchReaderStreamScanResult
        {
            // NOTE: important to keep the list and order of enum values in sync with the 
            //       enum in the product code!
            NoMatch,
            PartialMatch,
            Match,
        }

        /// <summary>
        /// The byte array that acts as the actual storage of the buffered data.
        /// </summary>
        public byte[] Bytes
        {
            get { return (byte[])ReflectionUtils.GetProperty(this.batchReaderStreamBuffer, "Bytes"); }
        }

        /// <summary>
        /// The current position inside the buffer.
        /// </summary>
        public int CurrentReadPosition
        {
            get { return (int)ReflectionUtils.GetProperty(this.batchReaderStreamBuffer, "CurrentReadPosition"); }
        }

        /// <summary>
        /// The number of (not yet consumed) bytes currently in the buffer.
        /// </summary>
        public int NumberOfBytesInBuffer
        {
            get { return (int)ReflectionUtils.GetProperty(this.batchReaderStreamBuffer, "NumberOfBytesInBuffer"); }
        }

        /// <summary>
        /// Skip to the specified position in the buffer. 
        /// Adjust the current position and the number of bytes in the buffer.
        /// </summary>
        /// <param name="newPosition">The position to skip to.</param>
        public void SkipTo(int newPosition)
        {
            ReflectionUtils.InvokeMethod(this.batchReaderStreamBuffer, "SkipTo", newPosition);
        }

        /// <summary>
        /// Refills the buffer from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to refill the buffer from.</param>
        /// <param name="preserveFrom">The index in the current buffer starting from which the 
        /// currently buffered data should be preserved.</param>
        /// <returns>true if the underlying stream got exhausted while refilling.</returns>
        /// <remarks>This method will first shift any data that is to be preserved to the beginning
        /// of the buffer and then refill the rest of the buffer from the <paramref name="stream"/>.</remarks>
        public bool RefillFrom(Stream stream, int preserveFrom)
        {
            return (bool)ReflectionUtils.InvokeMethod(this.batchReaderStreamBuffer, "RefillFrom", stream, preserveFrom);
        }

        /// <summary>
        /// Scans the current buffer for a line end.
        /// </summary>
        /// <param name="lineEndStartPosition">The start position of the line terminator or -1 if not found.</param>
        /// <param name="lineEndEndPosition">The end position of the line terminator or -1 if not found.</param>
        /// <returns>An enumeration value indicating whether the line termintor was found completely, partially or not at all.</returns>
        public ODataBatchReaderStreamScanResult ScanForLineEnd(out int lineEndStartPosition, out int lineEndEndPosition)
        {
            object[] args = new object[2];
            int result = (int)ReflectionUtils.InvokeMethod(this.batchReaderStreamBuffer, "ScanForLineEnd", args);
            lineEndStartPosition = (int)args[0];
            lineEndEndPosition = (int)args[1];
            return (ODataBatchReaderStreamScanResult)result;
        }

        /// <summary>
        /// Scans the current buffer for the specified boundary.
        /// </summary>
        /// <param name="boundaries">The boundary strings to search for; this enumerable is sorted from the inner-most boundary
        /// to the top-most boundary. The boundary strings don't include the leading line terminator or the leading dashes.</param>
        /// <param name="maxScanLength">The maximum number of data bytes to read searching for a boundary.</param>
        /// <param name="boundaryStartPosition">The start position of the boundary or -1 if not found.
        /// Note that the start position is the first byte of the leading line terminator.</param>
        /// <param name="boundaryEndPosition">The end position of the boundary or -1 if not found.
        /// Note that the end positoin is the last byte of the trailing line terminator.</param>
        /// <param name="isEndBoundary">true if the boundary is an end boundary (followed by two dashes); otherwise false.</param>
        /// <returns>An enumeration value indicating whether the boundary was completely, partially or not found in the buffer.</returns>
        public ODataBatchReaderStreamScanResult ScanForBoundary(
            IEnumerable<string> boundaries,
            int maxScanLength,
            out int boundaryStartPosition,
            out int boundaryEndPosition,
            out bool isEndBoundary,
            out bool isParentBoundary)
        {
            object[] args = new object[6];
            args[0] = boundaries;
            args[1] = maxScanLength;

            int result = (int)ReflectionUtils.InvokeMethod(this.batchReaderStreamBuffer, "ScanForBoundary", args);
            boundaryStartPosition = (int)args[2];
            boundaryEndPosition = (int)args[3];
            isEndBoundary = (bool)args[4];
            isParentBoundary = (bool)args[5];
            return (ODataBatchReaderStreamScanResult)result;
        }
    }
}
