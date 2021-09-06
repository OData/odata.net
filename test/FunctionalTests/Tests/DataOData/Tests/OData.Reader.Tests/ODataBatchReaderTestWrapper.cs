//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataBatchReader which allows for transparent monitoring and changing
    /// of the reader behavior.
    /// </summary>
    public sealed class ODataBatchReaderTestWrapper
    {
        /// <summary>
        /// The underlying reader to wrap.
        /// </summary>
        private readonly ODataBatchReader batchReader;

        /// <summary>
        /// Test configuration to use.
        /// </summary>
        private readonly ReaderTestConfiguration testConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchReader">The reader to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataBatchReaderTestWrapper(ODataBatchReader batchReader, ReaderTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(batchReader, "batchReader");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.batchReader = batchReader;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// The underlying batch reader.
        /// </summary>
        public ODataBatchReader BatchReader
        {
            get { return this.batchReader; }
        }

        /// <summary>
        /// The current state of the batch reader.
        /// </summary>
        public ODataBatchReaderState State
        {
            get { return this.batchReader.State; }
        }

        /// <summary>
        /// Reads the next part from the batch message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public bool Read()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.batchReader.Read();
            }
            else
            {
                return this.batchReader.ReadAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public Task<bool> ReadAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Returns an <see cref="ODataBatchOperationRequestMessage"/> for reading the content of a batch operation.
        /// </summary>
        /// <returns>A request message for reading the content of a batch operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.batchReader.CreateOperationRequestMessage();
            }
            else
            {
                return this.batchReader.CreateOperationRequestMessageAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Asynchronously returns an <see cref="ODataBatchOperationRequestMessage"/> for reading the content of a batch operation.
        /// </summary>
        /// <returns>A task that when completed returns a request message for reading the content of a batch operation.</returns>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Returns an <see cref="ODataBatchOperationResponseMessage"/> for reading the content of a batch operation.
        /// </summary>
        /// <returns>A response message for reading the content of a batch operation.</returns>
        public ODataBatchOperationResponseMessage CreateOperationResponseMessage()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.batchReader.CreateOperationResponseMessage();
            }
            else
            {
                return this.batchReader.CreateOperationResponseMessageAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Asynchronously returns an <see cref="ODataBatchOperationResponseMessage"/> for reading the content of a batch operation.
        /// </summary>
        /// <returns>A task that when completed returns a response message for reading the content of a batch operation.</returns>
        public Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }
    }
}
