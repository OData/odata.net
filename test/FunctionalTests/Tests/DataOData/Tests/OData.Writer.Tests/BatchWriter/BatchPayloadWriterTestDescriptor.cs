//---------------------------------------------------------------------
// <copyright file="BatchPayloadWriterTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;

    public class BatchPayloadWriterTestDescriptor<T> : PayloadWriterTestDescriptor<T>
    {
         /// <summary>
        /// Create a new descriptor instance given a payload item, expected ATOM and JSON results
        /// and optional extractors
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">The OData payload item to write.</param>
        /// <param name="expectedResultCallback">The callback to use to determine the expected results.</param>
        internal BatchPayloadWriterTestDescriptor(
            Settings settings,
            T payloadItem,
            WriterTestExpectedResultCallback expectedResultCallback)
            : this(settings, new T[] { payloadItem }, expectedResultCallback)
        {
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">payload item for the test</param>
        internal BatchPayloadWriterTestDescriptor(Settings settings, T payloadItem)
            : this(settings, new T[] { payloadItem })
        {
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">payloadItems</param>
        internal BatchPayloadWriterTestDescriptor(Settings settings, IEnumerable<T> payloadItems)
            : base(settings, payloadItems)
        {
            
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item for writing raw values
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">payload item for the test</param>
        /// <param name="rawValueAsString">The expected string value of the payload item.</param>
        /// <param name="rawBytes">The expected raw bytes for binary payload items.</param>
        /// <param name="expectedContentType">The expected content type of the raw value.</param>
        internal BatchPayloadWriterTestDescriptor(Settings settings, T payloadItem, string rawValueAsString = null, byte[] rawBytes = null, string expectedContentType = null)
            : this(settings, new T[] { payloadItem }, rawValueAsString, rawBytes, expectedContentType)
        {
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item for writing raw values
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">payloadItems</param>
        /// <param name="rawValueAsString">The expected string value of the payload items.</param>
        /// <param name="rawBytes">The expected raw bytes for binary payload items.</param>
        /// <param name="expectedContentType">The expected content type of the raw value.</param>
        internal BatchPayloadWriterTestDescriptor(Settings settings, IEnumerable<T> payloadItems, string rawValueAsString = null, byte[] rawBytes = null, string expectedContentType = null)
            : base(settings, payloadItems, rawValueAsString, rawBytes, expectedContentType)
        {
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item, expected ATOM and JSON results
        /// and optional extractors
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">The OData payload items to write.</param>
        /// <param name="expectedResultCallback">The callback to use to determine the expected results.</param>
        internal BatchPayloadWriterTestDescriptor(
            Settings settings,
            IEnumerable<T> payloadItems,
            WriterTestExpectedResultCallback expectedResultCallback)
            : base(settings,payloadItems, expectedResultCallback)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="PayloadWriterTestDescriptor&lt;T&gt;"/> to clone.</param>
        internal BatchPayloadWriterTestDescriptor(BatchPayloadWriterTestDescriptor<T> other)
            : base(other)
        {
        }

        /// <summary>
        /// Runs the test specified by this test descriptor.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use for running the test.</param>
        public override void RunTest(WriterTestConfiguration testConfiguration, BaselineLogger logger)
        {
            if (this.ShouldSkipForTestConfiguration(testConfiguration))
            {
                return;
            }

            // Wrap the memory stream in a non-disposing stream so we can dump the message content
            // even in the case of a failure where the message stream normally would get disposed.
            logger.LogConfiguration(testConfiguration);
            logger.LogModelPresence(this.Model);
            this.messageStream = new NonDisposingStream(new MemoryStream());
            TestMessage message = this.CreateOutputMessage(this.messageStream, testConfiguration, this.PayloadElement);
            IEdmModel model = this.GetMetadataProvider();
            WriterTestExpectedResults expectedResult = this.GetExpectedResult(testConfiguration);
            ExceptionUtilities.Assert(expectedResult != null, "The expected result could not be determined for the test. Did you specify it?");

            Exception exception = TestExceptionUtils.RunCatching(() =>
            {
                // We create a new test configuration for batch because the payload indicates whether we are dealing with a request or a response and the configuration won't know that in advance
                var newTestConfig = new WriterTestConfiguration(testConfiguration.Format, testConfiguration.MessageWriterSettings, this.PayloadElement is BatchRequestPayload, testConfiguration.Synchronous);
                using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(message, model, newTestConfig, this.settings.Assert, null))
                {
                    this.WritePayload(messageWriterWrapper, testConfiguration);
                    expectedResult.VerifyResult(message, this.PayloadKind, testConfiguration, logger);
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

        private TestMessage CreateOutputMessage(Stream innerStream, WriterTestConfiguration testConfiguration, ODataPayloadElement payload)
        {
            TestStream messageStream = new TestStream(innerStream);
            TestWriterUtils.SetFailAsynchronousCalls(messageStream, testConfiguration.Synchronous);

            var boundary = this.PayloadElement.GetAnnotation<BatchBoundaryAnnotation>();
            
            // We create a new test configuration for batch because the payload indicates whether we are dealing with a request or a response and the configuration won't know that in advance
            var newTestConfig = new WriterTestConfiguration(testConfiguration.Format, testConfiguration.MessageWriterSettings, this.PayloadElement is BatchRequestPayload, testConfiguration.Synchronous);
            TestMessage testMessage = TestWriterUtils.CreateOutputMessageFromStream(messageStream, newTestConfig, this.PayloadKind, boundary.BatchBoundaryInHeader, this.UrlResolver);
            return testMessage;
        }

       
    }
}
