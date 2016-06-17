//---------------------------------------------------------------------
// <copyright file="StreamingPayloadReaderTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System;
    using System.IO;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;

    public class StreamingPayloadReaderTestDescriptor : PayloadReaderTestDescriptor
    {
        /// <summary>
        /// A func which returns expected result for the test based on the test configuration.
        /// </summary>
        public new Func<WriterTestConfiguration, WriterTestExpectedResults> ExpectedResultCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public StreamingPayloadReaderTestDescriptor(Settings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The other payload test descriptor to copy</param>
        public StreamingPayloadReaderTestDescriptor(StreamingPayloadReaderTestDescriptor other)
            : base(other)
        {
        }

        public override void RunTest(ReaderTestConfiguration testConfiguration)
        {
            //TODO: Use Logger to verify result, right now this change is only to unblock writer testcase checkin
            BaselineLogger logger = null;

            if (this.ShouldSkipForTestConfiguration(testConfiguration))
            {
                return;
            }

            var originalPayload = this.PayloadElement;
            this.PayloadElement = this.PayloadElement.DeepCopy();

            // Create messages (payload gets serialized in createInputMessage)
            TestMessage readerMessage = this.CreateInputMessage(testConfiguration);
            var settings = new ODataMessageWriterSettings()
            {
                Version = testConfiguration.Version,
                BaseUri = testConfiguration.MessageReaderSettings.BaseUri,
                EnableMessageStreamDisposal = testConfiguration.MessageReaderSettings.EnableMessageStreamDisposal,
            };

            settings.SetContentType(testConfiguration.Format);

            WriterTestConfiguration writerConfig = new WriterTestConfiguration(testConfiguration.Format, settings, testConfiguration.IsRequest, testConfiguration.Synchronous);
            TestMessage writerMessage = TestWriterUtils.CreateOutputMessageFromStream(new TestStream(new MemoryStream()), writerConfig, this.PayloadKind, String.Empty, this.UrlResolver);

            IEdmModel model = this.GetMetadataProvider(testConfiguration);
            WriterTestExpectedResults expectedResult = this.GetExpectedResult(writerConfig);

            ExceptionUtilities.Assert(expectedResult != null, "The expected result could not be determined for the test. Did you specify it?");

            Exception exception = TestExceptionUtils.RunCatching(() =>
            {
                using (ODataMessageReaderTestWrapper messageReaderWrapper = TestReaderUtils.CreateMessageReader(readerMessage, model, testConfiguration))
                using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(writerMessage, model, writerConfig, this.settings.Assert))
                {
                    var streamer = new ObjectModelReadWriteStreamer();
                    streamer.StreamMessage(messageReaderWrapper, messageWriterWrapper, this.PayloadKind, writerConfig);
                    expectedResult.VerifyResult(writerMessage, this.PayloadKind, writerConfig, logger);
                }
            });

            this.PayloadElement = originalPayload;

            try
            {
                expectedResult.VerifyException(exception);
            }
            catch (Exception)
            {
                this.TraceFailureInformation(testConfiguration);
                throw;
            }
        }

        protected WriterTestExpectedResults GetExpectedResult(WriterTestConfiguration testConfiguration)
        {
            if (this.ExpectedResultCallback != null)
            {
                return this.ExpectedResultCallback(testConfiguration);
            }
            else
            {
                return null;
            }
        }
    }
}
