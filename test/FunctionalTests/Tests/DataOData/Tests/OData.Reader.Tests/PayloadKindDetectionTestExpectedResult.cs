//---------------------------------------------------------------------
// <copyright file="PayloadKindDetectionTestExpectedResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Reader test expected result which specified the expected result for payload kind detection.
    /// </summary>
    public class PayloadKindDetectionTestExpectedResult : ReaderTestExpectedResult
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class PayloadKindDetectionTestExpectedResultSettings : ReaderTestExpectedResultSettings
        {
            [InjectDependency(IsRequired = true)]
            public MessageToObjectModelReader MessageToObjectModelReader { get; set; }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        private readonly PayloadKindDetectionTestExpectedResultSettings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public PayloadKindDetectionTestExpectedResult(PayloadKindDetectionTestExpectedResultSettings settings)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(settings, "settings");

            this.settings = settings;
        }

        /// <summary>
        /// The expected payload kind detection results.
        /// </summary>
        public IEnumerable<PayloadKindDetectionResult> ExpectedDetectionResults { get; set; }

        /// <summary>
        /// The test message from the test descriptor.
        /// </summary>
        public TestMessage TestMessage { get; set; }

        /// <summary>true to read the payload as the detected kind(s); otherwise false.</summary>
        public bool ReadDetectedPayloads { get; set; }

        /// <summary>
        /// Verifies that the result of the test (the message reader) is what the test expected.
        /// </summary>
        /// <param name="messageReader">The message reader which is the result of the test. This method should use it to read the results
        /// of the parsing and verify those.</param>
        /// <param name="payloadKind">The payload kind specified in the test descriptor.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public override void VerifyResult(
            ODataMessageReaderTestWrapper messageReader,
            ODataPayloadKind payloadKind,
            ReaderTestConfiguration testConfiguration)
        {
            // First compare the payload kind detection results.
            IEnumerable<ODataPayloadKindDetectionResult> actualDetectionResults = messageReader.DetectPayloadKind();
            this.VerifyPayloadKindDetectionResult(actualDetectionResults);

            // Then try to read the message as the detected kind if requested
            if (this.ReadDetectedPayloads)
            {
                bool firstResult = true;
                foreach (PayloadKindDetectionResult result in this.ExpectedDetectionResults)
                {
                    if (firstResult)
                    {
                        // For the first result use the existing message reader
                        firstResult = false;
                    }
                    else
                    {
                        // For all subsequent results we need to reset the test stream and create a new message reader
                        // over it.
                        this.TestMessage.Reset();
                        messageReader = TestReaderUtils.CreateMessageReader(this.TestMessage, result.Model, testConfiguration);

                        // Detect the payload kinds again and make sure we can also read the subsequent payload kinds
                        // immediately after detection.
                        actualDetectionResults = messageReader.DetectPayloadKind();
                        this.VerifyPayloadKindDetectionResult(actualDetectionResults);
                    }

                    TestExceptionUtils.ExpectedException(
                        this.settings.Assert,
                        () =>
                            {
                                using (messageReader)
                                {
                                    this.settings.MessageToObjectModelReader.ReadMessage(
                                        messageReader,
                                        result.PayloadKind,
                                        result.Model,
                                        new PayloadReaderTestDescriptor.ReaderMetadata(result.ExpectedType),
                                        /*expectedBatchPayload*/ null,
                                        testConfiguration);
                                }
                            },
                        result.ExpectedException,
                        this.settings.ExceptionVerifier);
                }
            }
        }

        private void VerifyPayloadKindDetectionResult(IEnumerable<ODataPayloadKindDetectionResult>  actualDetectionResults)
        {
            VerificationUtils.VerifyEnumerationsAreEqual(
                this.ExpectedDetectionResults,
                actualDetectionResults.Select(ar => new PayloadKindDetectionResult(ar.PayloadKind, ar.Format)),
                (first, second, assert) =>
                {
                    assert.AreEqual(first.Format, second.Format, "Formats don't match.");
                    assert.AreEqual(first.PayloadKind, second.PayloadKind, "Payload kinds don't match.");
                },
                result => "[" + result.PayloadKind.ToString() + ", " + result.Format.ToString() + "]",
                this.settings.Assert);
        }
    }
}
