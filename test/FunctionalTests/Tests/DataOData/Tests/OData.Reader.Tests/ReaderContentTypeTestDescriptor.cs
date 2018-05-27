//---------------------------------------------------------------------
// <copyright file="ReaderContentTypeTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Reader test descriptor for tests which test the content type processing of the message reader.
    /// </summary>
    public sealed class ReaderContentTypeTestDescriptor : PayloadReaderTestDescriptor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public ReaderContentTypeTestDescriptor(Settings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="other"><see cref="PayloadReaderTestDescriptor"/> to initialize this instance.</param>
        public ReaderContentTypeTestDescriptor(PayloadReaderTestDescriptor other)
            : base(other)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="other"><see cref="ReaderContentTypeTestDescriptor"/> to initialize this instance.</param>
        public ReaderContentTypeTestDescriptor(ReaderContentTypeTestDescriptor other)
            : base(other)
        {
            this.ContentType = other.ContentType;
            this.ExpectedFormat = other.ExpectedFormat;
        }

        /// <summary>The content type to set for the Content-Type header field.</summary>
        public string ContentType { get; set; }

        /// <summary>The expected <see cref="ODataFormat"/> for the specified content type.</summary>
        public ODataFormat ExpectedFormat { get; set; }

        /// <summary>
        /// Called to create the input message for the reader test and sets the specified value for the Content-Type header.
        /// </summary>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The newly created test message to use.</returns>
        protected override TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration)
        {
            TestMessage testMessage = base.CreateInputMessage(testConfiguration);

            // overwrite the content-type header of the message
            testMessage.SetHeader(ODataConstants.ContentTypeHeader, this.ContentType);

            return testMessage;
        }

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected override ReaderTestExpectedResult GetExpectedResult(ReaderTestConfiguration testConfiguration)
        {
            ReaderTestExpectedResult innerExpectedResult = base.GetExpectedResult(testConfiguration);
            return new ContentTypeReaderTestExpectedResult(innerExpectedResult, this.ExpectedFormat, this.settings.ExpectedResultSettings);
        }

        /// <summary>
        /// Custom test expected result class for content type tests that also validates the <see cref="ODataFormat"/>
        /// reported by the message reader.
        /// </summary>
        private sealed class ContentTypeReaderTestExpectedResult : ReaderTestExpectedResult
        {
            /// <summary>The inner <see cref="ReaderTestExpectedResult"/> that is doing most of the work.</summary>
            private readonly ReaderTestExpectedResult innerExpectedResult;

            /// <summary>Settings to be used.</summary>
            private readonly ReaderTestExpectedResult.ReaderTestExpectedResultSettings settings;

            /// <summary>The expected <see cref="ODataFormat"/> for the specified content type.</summary>
            private readonly ODataFormat expectedFormat;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="innerExpectedResult">The inner <see cref="ReaderTestExpectedResult"/> that is doing most of the work.</param>
            /// <param name="expectedFormat">The expected <see cref="ODataFormat"/> for the specified content type.</param>
            /// <param name="settings">Settings to use.</param>
            public ContentTypeReaderTestExpectedResult(ReaderTestExpectedResult innerExpectedResult, ODataFormat expectedFormat, ReaderTestExpectedResult.ReaderTestExpectedResultSettings settings)
                : base(settings)
            {
                this.innerExpectedResult = innerExpectedResult;
                this.expectedFormat = expectedFormat;
                this.settings = settings;
            }

            /// <summary>
            /// Verifies that the result of the test is what is expected.
            /// It does so by delegating to the inner expected result provided in the constructor and additionally checking the reported <see cref="ODataFormat"/>.
            /// </summary>
            /// <param name="messageReader">The message reader which is the result of the test. This method should use it to read the results
            /// of the parsing and verify those.</param>
            /// <param name="payloadKind">The payload kind specified in the test descriptor.</param>
            /// <param name="testConfiguration">The test configuration to use.</param>
            public override void VerifyResult(ODataMessageReaderTestWrapper messageReader, ODataPayloadKind payloadKind, ReaderTestConfiguration testConfiguration)
            {
                if (this.innerExpectedResult != null)
                {
                    this.innerExpectedResult.VerifyResult(messageReader, payloadKind, testConfiguration);
                }

                ODataFormat actualFormat = ODataUtils.GetReadFormat(messageReader.MessageReader);
                this.settings.Assert.AreEqual(this.expectedFormat, actualFormat, "Formats don't match.");
            }

            /// <summary>
            /// Called to create the input message for the reader test.
            /// </summary>
            /// <param name="testConfiguration">The test configuration.</param>
            /// <returns>The newly created test message to use.</returns>
            public override void VerifyException(Exception exception)
            {
                if (this.innerExpectedResult != null)
                {
                    this.innerExpectedResult.VerifyException(exception);
                }
            }
        }
    }
}
