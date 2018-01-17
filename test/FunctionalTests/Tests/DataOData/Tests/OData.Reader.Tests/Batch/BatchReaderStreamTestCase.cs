//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.MultipartMixed;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

#if !SILVERLIGHT && !WINDOWS_PHONE
    // Batch stream buffer tests use private reflection and thus cannot run on Silverlight or the phone.

    /// <summary>
    /// Base class for all unit tests for the ODataMultipartMixedBatchReaderStream.
    /// </summary>
    public abstract class BatchReaderStreamTestCase : BatchReaderStreamOrBufferTestCase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchBoundary">The boundary string to be used for the batch request; or null if a default boundary should be used.</param>
        /// <param name="encoding">The encoding to use to convert bytes to and from strings.</param>
        /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
        public BatchReaderStreamTestCase(string batchBoundary = null, Encoding encoding = null, char[] lineFeedChars = null)
            : base(encoding, lineFeedChars)
        {
            this.BatchBoundary = batchBoundary ?? "batchBoundary";
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The test case instance to copy.</param>
        public BatchReaderStreamTestCase(BatchReaderStreamTestCase other)
            : base(other)
        {
            this.IsResponse = other.IsResponse;
            this.BatchBoundary = other.BatchBoundary;
        }

        /// <summary>true if the test case is reading a response; otherwise false.</summary>
        public bool IsResponse { get; set; }

        /// <summary>The batch boundary to use for this test case.</summary>
        public string BatchBoundary { get; set; }

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

            // Create a message reader and then a batch reader for the message
            using (ODataMessageReader messageReader = this.CreateMessageReader(memoryStream))
            {
                ODataMultipartMixedBatchReader batchReader = messageReader.CreateODataBatchReader() as ODataMultipartMixedBatchReader;
                BatchReaderStreamWrapper streamWrapper = new BatchReaderStreamWrapper(batchReader);

                Exception exception = TestExceptionUtils.RunCatching(() =>
                {
                    this.RunTestAction(streamWrapper);
                });

                this.VerifyException(exception);
            }
        }

        /// <summary>
        /// Runs the test action of this test after setting up the batch reader stream.
        /// </summary>
        /// <param name="streamWrapper">The batch reader stream to test.</param>
        protected abstract void RunTestAction(BatchReaderStreamWrapper streamWrapper);

        /// <summary>
        /// Creates an <see cref="ODataMessageReader"/> from the specified <paramref name="memoryStream"/>.
        /// </summary>
        /// <param name="memoryStream">The <see cref="MemoryStream"/> used as input for the message reader.</param>
        /// <returns>An <see cref="ODataMessageReader"/> instance.</returns>
        private ODataMessageReader CreateMessageReader(MemoryStream memoryStream)
        {
            // Create a test message around the memory stream, set the content-type header and then create
            // the message reader
            if (this.IsResponse)
            {
                TestResponseMessage responseMessage = new TestResponseMessage(memoryStream, TestMessageFlags.NoAsynchronous);
                responseMessage.SetHeader(ODataConstants.ContentTypeHeader, "multipart/mixed; boundary=" + this.BatchBoundary);
                return new ODataMessageReader(responseMessage);
            }
            else
            {
                TestRequestMessage requestMessage = new TestRequestMessage(memoryStream, TestMessageFlags.NoAsynchronous);
                requestMessage.SetHeader(ODataConstants.ContentTypeHeader, "multipart/mixed; boundary=" + this.BatchBoundary);
                return new ODataMessageReader(requestMessage);
            }
        }
    }
#endif
}
