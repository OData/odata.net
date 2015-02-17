//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamBufferTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    #endregion Namespaces

    /// <summary>
    /// Base class for all unit tests for the ODataBatchReaderStreamBuffer.
    /// </summary>
    public abstract class BatchReaderStreamBufferTestCase : BatchReaderStreamOrBufferTestCase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="encoding">The encoding to use to convert bytes to and from strings.</param>
        /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
        public BatchReaderStreamBufferTestCase(Encoding encoding = null, char[] lineFeedChars = null)
            : base(encoding, lineFeedChars)
        {
        }

        /// <summary>
        /// Runs the test case.
        /// </summary>
        public override void Run()
        {
            BatchReaderStreamBufferWrapper streamBuffer = new BatchReaderStreamBufferWrapper();
            MemoryStreamBatchPayloadBuilder payloadBuilder = new MemoryStreamBatchPayloadBuilder(this.Encoding, this.LineFeedChars);

            // If no explicit payload func was specified, use a default payload
            MemoryStream memoryStream;
            if (this.PayloadFunc == null)
            {
                memoryStream = payloadBuilder.FillBytes(BatchReaderStreamBufferWrapper.BufferLength).ResetMemoryStream();
            }
            else
            {
                memoryStream = this.PayloadFunc(payloadBuilder);
            }

            Exception exception = TestExceptionUtils.RunCatching(() =>
            {
                this.RunTestAction(streamBuffer, memoryStream);
            });

            this.VerifyException(exception);
        }

        /// <summary>
        /// Runs the test action of this test after setting up the input memory stream.
        /// </summary>
        /// <param name="streamBuffer">The batch reader stream buffer to test.</param>
        /// <param name="memoryStream">The memory stream with the input payload.</param>
        protected abstract void RunTestAction(BatchReaderStreamBufferWrapper streamBuffer, MemoryStream memoryStream);
    }
}
