//---------------------------------------------------------------------
// <copyright file="ODataBatchWriterTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataBatchWriter which allows for transparent monitoring and changing
    /// of the writer behavior.
    /// </summary>
    public sealed class ODataBatchWriterTestWrapper
    {
        /// <summary>
        /// The underlying writer to wrap.
        /// </summary>
        private readonly ODataBatchWriter batchWriter;

        /// <summary>
        /// Test configuration to use.
        /// </summary>
        private readonly WriterTestConfiguration testConfiguration;

        /// <summary>
        /// Content id used in request/response.
        /// </summary>
        private int contentId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchWriter">The writer to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataBatchWriterTestWrapper(ODataBatchWriter batchWriter, WriterTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(batchWriter, "batchWriter");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.batchWriter = batchWriter;
            this.testConfiguration = testConfiguration;
            this.contentId = 100;
        }

        /// <summary>
        /// The underlying batch writer.
        /// </summary>
        public ODataBatchWriter BatchWriter
        {
            get { return this.batchWriter; }
        }

        /// <summary>
        /// Starts a new batch; can be only called once and as first call.
        /// </summary>
        public void WriteStartBatch()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.batchWriter.WriteStartBatch();
            }
            else
            {
                this.batchWriter.WriteStartBatchAsync().Wait();
            }
        }

        /// <summary>
        /// Ends a batch; can only be called after WriteStartBatch has been called and if no other active changeset or operation exist.
        /// </summary>
        public void WriteEndBatch()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.batchWriter.WriteEndBatch();
            }
            else
            {
                this.batchWriter.WriteEndBatchAsync().Wait();
            }
        }

        /// <summary>
        /// Starts a new changeset; can only be called after WriteStartBatch and if no other active operation or changeset exists.
        /// </summary>
        public void WriteStartChangeset()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.batchWriter.WriteStartChangeset();
            }
            else
            {
                this.batchWriter.WriteStartChangesetAsync().Wait();
            }
        }

        /// <summary>
        /// Ends an active changeset; this can only be called after WriteStartChangeset and only once for each changeset.
        /// </summary>
        public void WriteEndChangeset()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.batchWriter.WriteEndChangeset();
            }
            else
            {
                this.batchWriter.WriteEndChangesetAsync().Wait();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentIdOverrride">The contentId to be used for this request operation.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(string method, Uri uri, string contentIdOverride = null)
        {
            string contentIdToWrite = contentIdOverride ?? (++contentId).ToString();

            if (this.testConfiguration.Synchronous)
            {
                return this.batchWriter.CreateOperationRequestMessage(method, uri, contentIdToWrite);
            }
            else
            {
                return this.batchWriter.CreateOperationRequestMessageAsync(method, uri, contentIdToWrite).WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response.
        /// </summary>
        /// <returns>The message that can be used to write the response operation.</returns>
        public ODataBatchOperationResponseMessage CreateOperationResponseMessage(bool isQuery = false)
        {
            string contentId = isQuery ? null : "1";

            if (this.testConfiguration.Synchronous)
            {
                return this.batchWriter.CreateOperationResponseMessage(contentId);
            }
            else
            {
                return this.batchWriter.CreateOperationResponseMessageAsync(contentId).WaitForResult();
            }
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public void Flush()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.batchWriter.Flush();
            }
            else
            {
                this.batchWriter.FlushAsync().Wait();
            }
        }
    }
}
