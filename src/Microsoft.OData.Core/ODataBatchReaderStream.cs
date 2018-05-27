//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Class used by the <see cref="ODataBatchReader"/> to read the various pieces of a batch payload.
    /// </summary>
    /// <remarks>
    /// This is the base class for format-specific derived classes to process batch requests in different formats,
    /// such as multipart/mixed format and application/json format.
    /// </remarks>
    internal abstract class ODataBatchReaderStream
    {
        /// <summary>The buffer used by the batch reader stream to scan for boundary strings.</summary>
        protected readonly ODataBatchReaderStreamBuffer BatchBuffer;


        /// <summary>
        /// true if the underlying stream was exhausted during a read operation; we won't try to read from the
        /// underlying stream again once it was exhausted.
        /// </summary>
        protected bool underlyingStreamExhausted;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ODataBatchReaderStream()
        {
            this.BatchBuffer = new ODataBatchReaderStreamBuffer();
        }

        /// <summary>
        /// Reads from the batch stream while ensuring that we stop reading at each boundary.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        internal abstract int ReadWithDelimiter(byte[] userBuffer, int userBufferOffset, int count);


        /// <summary>
        /// Reads from the batch stream without checking for a boundary delimiter since we
        /// know the length of the stream.
        /// </summary>
        /// <param name="userBuffer">The byte array to read bytes into.</param>
        /// <param name="userBufferOffset">The offset in the buffer where to start reading bytes into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        internal abstract int ReadWithLength(byte[] userBuffer, int userBufferOffset, int count);
    }
}
