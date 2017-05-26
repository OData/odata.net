//---------------------------------------------------------------------
// <copyright file="StreamingPayloadWriterTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using System.Diagnostics;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;

    public class StreamingPayloadWriterTestDescriptor<T> : PayloadWriterTestDescriptor<T>
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public new class Settings : PayloadWriterTestDescriptor.Settings
        {
            [InjectDependency(IsRequired = true)]
            public ODataMessageReaderSettings MessageReaderSettings { get; set; }
        }

        /// <summary>
        /// The settings for the StreamingPayloadWriterTestDescriptor
        /// </summary>
        protected new Settings settings;

        /// <summary>
        /// The test message we are using for the tests.
        /// </summary>
        protected TestMessage testMessage;

        /// <summary>
        /// The result of the read aspect of the write-read (used to pass between writing and validation).
        /// </summary>
        protected ODataPayloadElement readObject;

        /// <summary>
        /// Create a new descriptor instance given a payload item, expected ATOM and JSON results
        /// and optional extractors
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">The OData payload item to write.</param>
        /// <param name="expectedResultCallback">The callback to use to determine the expected results.</param>
        internal StreamingPayloadWriterTestDescriptor(
            Settings settings,
            T payloadItem,
            WriterTestExpectedResultCallback expectedResultCallback)
            : this(settings, new T[] { payloadItem }, expectedResultCallback)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">payload item for the test</param>
        internal StreamingPayloadWriterTestDescriptor(Settings settings)
            : this(settings, new T[] { })
        {
            this.settings = settings;
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">payloadItems</param>
        internal StreamingPayloadWriterTestDescriptor(Settings settings, IEnumerable<T> payloadItems)
            : base(settings, payloadItems)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item for writing raw values
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">payload item for the test</param>
        /// <param name="rawValueAsString">The expected string value of the payload item.</param>
        /// <param name="rawBytes">The expected raw bytes for binary payload items.</param>
        /// <param name="expectedContentType">The expected content type of the raw value.</param>
        internal StreamingPayloadWriterTestDescriptor(Settings settings, T payloadItem, string rawValueAsString, byte[] rawBytes = null, string expectedContentType = null)
            : this(settings, new T[] { payloadItem }, rawValueAsString, rawBytes, expectedContentType)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item for writing raw values
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">payloadItems</param>
        /// <param name="rawValueAsString">The expected string value of the payload items.</param>
        /// <param name="rawBytes">The expected raw bytes for binary payload items.</param>
        /// <param name="expectedContentType">The expected content type of the raw value.</param>
        internal StreamingPayloadWriterTestDescriptor(Settings settings, IEnumerable<T> payloadItems, string rawValueAsString = null, byte[] rawBytes = null, string expectedContentType = null)
            : base(settings, payloadItems, rawValueAsString, rawBytes, expectedContentType)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item, expected ATOM and JSON results
        /// and optional extractors
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">The OData payload items to write.</param>
        /// <param name="expectedResultCallback">The callback to use to determine the expected results.</param>
        internal StreamingPayloadWriterTestDescriptor(
            Settings settings,
            IEnumerable<T> payloadItems,
            WriterTestExpectedResultCallback expectedResultCallback)
            : base(settings, payloadItems, expectedResultCallback)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="PayloadWriterTestDescriptor&lt;T&gt;"/> to clone.</param>
        internal StreamingPayloadWriterTestDescriptor(StreamingPayloadWriterTestDescriptor<T> other)
            : base(other)
        {
            this.settings = other.settings;
        }

        /// <summary>
        /// Runs the test specified by this test descriptor.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use for running the test.</param>
        public override void RunTest(WriterTestConfiguration testConfiguration, BaselineLogger logger = null)
        {
            //TODO: Use Logger to verify result, right now this change is only to unblock writer testcase checkin

            if (this.ShouldSkipForTestConfiguration(testConfiguration))
            {
                return;
            }

            // Generate a StreamingTestStream with a NonDisposingStream.
            this.messageStream = new StreamingTestStream(new NonDisposingStream(new MemoryStream()));
            TestMessage message = this.CreateOutputMessage(this.messageStream, testConfiguration);
            IEdmModel model = this.GetMetadataProvider();
            StreamingWriterTestExpectedResults expectedResult = (StreamingWriterTestExpectedResults)this.GetExpectedResult(testConfiguration);
            ExceptionUtilities.Assert(expectedResult != null, "The expected result could not be determined for the test. Did you specify it?");

            Exception exception = TestExceptionUtils.RunCatching(() =>
            {
                using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(message, model, testConfiguration, this.settings.Assert))
                {
                    this.WritePayload(messageWriterWrapper, testConfiguration);
                    expectedResult.ObservedElement = this.readObject;
                    expectedResult.VerifyResult(message, this.PayloadKind, testConfiguration);
                }
            });

            try
            {
                expectedResult.VerifyException(exception);
            }
            catch (Exception failureException)
            {
                this.TraceFailureInformation(message, this.messageStream, testConfiguration);
                throw failureException;
            }
        }

        /// <summary>
        /// Called to write the payload to the specified <paramref name="messageWriter"/>.
        /// </summary>
        /// <param name="messageWriter">The <see cref="ODataMessageWriterTestWrapper"/> to use for writing the payload.</param>
        /// <param name="testConfiguration">The test configuration to generate the payload for.</param>
        protected override void WritePayload(ODataMessageWriterTestWrapper messageWriter, WriterTestConfiguration testConfiguration)
        {
            Debug.Assert(this.messageStream != null, "Streaming test stream must have been created.");
            TestMessage testMessage = this.CreateInputMessageFromStream((TestStream)this.messageStream, testConfiguration, this.PayloadKind, string.Empty, this.UrlResolver);
            testMessage.SetContentType(testConfiguration.Format, this.PayloadKind);

            Exception exception = TestExceptionUtils.RunCatching(() =>
            {
                ODataMessageReaderSettings readerSettings = this.settings.MessageReaderSettings.Clone();
                readerSettings.EnableMessageStreamDisposal = testConfiguration.MessageWriterSettings.EnableMessageStreamDisposal;

                ReaderTestConfiguration readerConfig = new ReaderTestConfiguration(
                    testConfiguration.Format,
                    readerSettings,
                    testConfiguration.IsRequest,
                    testConfiguration.Synchronous);

                IEdmModel model = this.GetMetadataProvider();
                using (ODataMessageReaderTestWrapper messageReaderWrapper = TestReaderUtils.CreateMessageReader(testMessage, model, readerConfig))
                {
                    ODataPayloadElementToObjectModelConverter payloadElementToOMConverter = new ODataPayloadElementToObjectModelConverter(!testConfiguration.IsRequest);
                    ObjectModelToPayloadElementConverter reverseConverter = new ObjectModelToPayloadElementConverter();
                    ObjectModelWriteReadStreamer streamer = new ObjectModelWriteReadStreamer();

                    this.readObject = reverseConverter.Convert(streamer.WriteMessage(messageWriter, messageReaderWrapper, this.PayloadKind, payloadElementToOMConverter.Convert(this.PayloadElement)), !testConfiguration.IsRequest);
                }
            });
        }

        /// <summary>
        /// Helper method to create a test message from its content.
        /// </summary>
        /// <param name="messageContent">Stream with the content of the message.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <param name="payloadKind">The payload kind to use to compute and set the content type header; or null if no content type header should be set.</param>
        /// <param name="customContentTypeHeader">A custom content type header to be used in the message.</param>
        /// <param name="urlResolver">Url resolver to add to the test message created.</param>
        /// <returns>Newly created test message.</returns>
        protected TestMessage CreateInputMessageFromStream(
            TestStream messageContent,
            WriterTestConfiguration testConfiguration,
            ODataPayloadKind? payloadKind,
            string customContentTypeHeader,
            IODataPayloadUriConverter urlResolver)
        {
            TestMessage message;
            if (testConfiguration.IsRequest)
            {
                if (urlResolver != null)
                {
                    message = new TestRequestMessageWithUrlResolver(messageContent, urlResolver, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
                else
                {
                    message = new TestRequestMessage(messageContent, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
            }
            else
            {
                if (urlResolver != null)
                {
                    message = new TestResponseMessageWithUrlResolver(messageContent, urlResolver, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
                else
                {
                    message = new TestResponseMessage(messageContent, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
            }

            return message;
        }

        /// <summary>
        /// Called to create the input message for the reader test.
        /// </summary>
        /// <param name="innerStream">The <see cref="Stream"/> instance to be used as inner stream of the <see cref="TestStream"/>.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The newly created test message to use.</returns>
        protected override TestMessage CreateOutputMessage(Stream innerStream, WriterTestConfiguration testConfiguration)
        {
            Debug.Assert(innerStream is StreamingTestStream, "StreamingTestStream expected.");
            StreamingTestStream streamingTestStream = (StreamingTestStream)innerStream;

            if (testConfiguration.Synchronous)
            {
                streamingTestStream.FailAsynchronousCalls = true;
            }
            else
            {
                streamingTestStream.FailSynchronousCalls = true;
            }

            TestMessage testMessage = TestWriterUtils.CreateOutputMessageFromStream(
                streamingTestStream,
                testConfiguration,
                this.PayloadKind,
                this.PayloadElement.GetCustomContentTypeHeader(),
                this.UrlResolver);

            return testMessage;
        }
    }
}
