//---------------------------------------------------------------------
// <copyright file="ODataMultipartMixedBatchInputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.MultipartMixed
{
    #region Namespaces
    using System;
    using System.Diagnostics;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// MultipartMixed batch format output context.
    /// </summary>
    internal sealed class ODataMultipartMixedBatchInputContext : ODataRawInputContext
    {
        /// <summary>The boundary for writing a batch.</summary>
        private string batchBoundary;

        /// <summary>Constructor.</summary>
        /// <param name="format">The format for this input context.</param>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        public ODataMultipartMixedBatchInputContext(
        ODataFormat format,
        ODataMessageInfo messageInfo,
        ODataMessageReaderSettings messageReaderSettings)
            : base(format, messageInfo, messageReaderSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");
            Debug.Assert(messageInfo.MediaType != null, "Media type should have been set in messageInfo prior to creating Raw Input Context for Batch");
            try
            {
                this.batchBoundary =
                    ODataMultipartMixedBatchWriterUtils.GetBatchBoundaryFromMediaType(messageInfo.MediaType);
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to get a batch boundary.
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    messageInfo.MessageStream.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// Create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <returns>The newly created <see cref="ODataCollectionReader"/>.</returns>
        internal override ODataBatchReader CreateBatchReader()
        {
            return this.CreateBatchReaderImplementation(/*synchronous*/ true);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <returns>Task which when completed returns the newly created <see cref="ODataCollectionReader"/>.</returns>
        internal override Task<ODataBatchReader> CreateBatchReaderAsync()
        {
            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateBatchReaderImplementation(/*synchronous*/ false));
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="synchronous">If the reader should be created for synchronous or asynchronous API.</param>
        /// <returns>The newly created <see cref="ODataCollectionReader"/>.</returns>
        private ODataBatchReader CreateBatchReaderImplementation(bool synchronous)
        {
            return new ODataMultipartMixedBatchReader(this, this.batchBoundary, this.Encoding, synchronous);
        }
    }
}
