//---------------------------------------------------------------------
// <copyright file="ODataMultipartMixedBatchOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.MultipartMixed
{
    #region Namespaces
    using System.Diagnostics;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// MultipartMixed batch format output context.
    /// </summary>
    internal sealed class ODataMultipartMixedBatchOutputContext : ODataRawOutputContext
    {
        /// <summary>The boundary for writing a batch.</summary>
        private readonly string batchBoundary;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        internal ODataMultipartMixedBatchOutputContext(
            ODataFormat format,
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
            : base(format, messageInfo, messageWriterSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");
            Debug.Assert(messageInfo.MediaType != null, "Media type should have been set in messageInfo prior to creating Raw Input Context for Batch");
            this.batchBoundary = ODataMultipartMixedBatchWriterUtils.GetBatchBoundaryFromMediaType(messageInfo.MediaType);
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>The created batch writer.</returns>
        /// <remarks>We don't plan to make this public!</remarks>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataBatchWriter CreateODataBatchWriter()
        {
            this.AssertSynchronous();

            return this.CreateODataBatchWriterImplementation();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>A running task for the created batch writer.</returns>
        /// <remarks>We don't plan to make this public!</remarks>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataBatchWriter> CreateODataBatchWriterAsync()
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataBatchWriterImplementation());
        }
#endif

        /// <summary>
        /// Creates a batch writer.
        /// </summary>
        /// <returns>The newly created batch writer.</returns>
        private ODataBatchWriter CreateODataBatchWriterImplementation()
        {
            // Batch writer needs the default encoding to not use the preamble.
            this.encoding = this.encoding ?? MediaTypeUtils.EncodingUtf8NoPreamble;
            ODataBatchWriter batchWriter = new ODataMultipartMixedBatchWriter(this, this.batchBoundary);
            this.outputInStreamErrorListener = batchWriter;
            return batchWriter;
        }
    }
}
