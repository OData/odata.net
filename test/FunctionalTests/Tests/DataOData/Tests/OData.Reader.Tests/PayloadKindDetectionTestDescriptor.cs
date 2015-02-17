//---------------------------------------------------------------------
// <copyright file="PayloadKindDetectionTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Reader test descriptor for metadata payload kind detection tests.
    /// </summary>
    public class PayloadKindDetectionTestDescriptor : ReaderTestDescriptor
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class Settings
        {
            [InjectDependency(IsRequired = true)]
            public AssertionHandler Assert { get; set; }

            [InjectDependency(IsRequired = true)]
            public PayloadKindDetectionTestExpectedResult.PayloadKindDetectionTestExpectedResultSettings ExpectedResultSettings { get; set; }

            [InjectDependency(IsRequired = true)]
            public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }
        }

        /// <summary>Settings for the test descriptor.</summary>
        private readonly Settings settings;

        /// <summary>Cache slot for the created test message.</summary>
        private TestMessage testMessage;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public PayloadKindDetectionTestDescriptor(Settings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">Test descriptor instance to copy.</param>
        public PayloadKindDetectionTestDescriptor(PayloadKindDetectionTestDescriptor other)
            : base(other)
        {
            this.settings = other.settings;
            this.ContentType = other.ContentType;
            this.PayloadString = other.PayloadString;
            this.ExpectedDetectionResults = other.ExpectedDetectionResults;
            this.ExpectedException = other.ExpectedException;
            this.TestMessageWrapper = other.TestMessageWrapper;
        }

        /// <summary>
        /// The content type header to use, if set overrides the content type derived from the test configuration.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The string representation of the payload.
        /// </summary>
        public string PayloadString { get; set; }

        /// <summary>
        /// The metadata in the form of entity model for the payload.
        /// Can be null in which case the payload will execute without metadata.
        /// </summary>
        public IEdmModel PayloadEdmModel
        {
            get
            {
                return this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        /// <summary>true to read the payload as the detected kind(s); otherwise false.</summary>
        public bool ReadDetectedPayloads { get; set; }

        /// <summary>
        /// The expected payload kind detection results.
        /// </summary>
        public Func<ReaderTestConfiguration, IEnumerable<PayloadKindDetectionResult>> ExpectedDetectionResults { get; set; }

        /// <summary>
        /// The expected exception if the test is expected to fail.
        /// </summary>
        public ExpectedException ExpectedException { get; set; }

        /// <summary>
        /// Func which is called to wrap a test message if needed.
        /// </summary>
        public Func<TestMessage, TestMessage> TestMessageWrapper { get; set; }

        /// <summary>
        /// Creates a copy of this MetadataReaderTestDescriptor.
        /// </summary>
        public override object Clone()
        {
            return new PayloadKindDetectionTestDescriptor(this);
        }

        /// <summary>
        /// Returns description of the test case.
        /// </summary>
        /// <returns>Humanly readable description of the test. Used for debugging.</returns>
        public override string ToString()
        {
            string result = base.ToString();
            result += "\r\nPayload:\r\n" + this.PayloadString;
            return result;
        }

        /// <summary>
        /// Called to create the input message for the reader test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The newly created test message to use.</returns>
        protected override TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration)
        {
            MemoryStream memoryStream = new MemoryStream(this.GetPayload(testConfiguration));
            TestStream messageStream = new TestStream(memoryStream);
            if (testConfiguration.Synchronous)
            {
                messageStream.FailAsynchronousCalls = true;
            }
            else
            {
                messageStream.FailSynchronousCalls = true;
            }

            this.testMessage = TestReaderUtils.CreateInputMessageFromStream(
                messageStream, 
                testConfiguration,
                /*payloadKind*/ null,
                this.ContentType,
                /*urlResolver*/ null);

            if (this.TestMessageWrapper != null)
            {
                this.testMessage = this.TestMessageWrapper(this.testMessage);
            }

            return this.testMessage;
        }

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected override ReaderTestExpectedResult GetExpectedResult(ReaderTestConfiguration testConfiguration)
        {
            return new PayloadKindDetectionTestExpectedResult(this.settings.ExpectedResultSettings)
            {
                TestMessage = this.testMessage,
                ReadDetectedPayloads = this.ReadDetectedPayloads,
                ExpectedDetectionResults = this.ExpectedDetectionResults == null ? null : this.ExpectedDetectionResults(testConfiguration),
                ExpectedException = this.ExpectedException,
            };
        }

        /// <summary>
        /// Gets The model to use for the specified test configuration.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The model to use for the test.</returns>
        protected override IEdmModel GetMetadataProvider(ReaderTestConfiguration testConfiguration)
        {
            IEdmModel model = base.GetMetadataProvider(testConfiguration);
            if (model != null)
            {
                return model;
            }

            // NOTE: get the model from the first result (if any) as the model for the message writer.
            //       This message writer will be used to validate the first result only.
            PayloadKindDetectionResult firstResult = this.ExpectedDetectionResults == null 
                ? null 
                : this.ExpectedDetectionResults(testConfiguration).FirstOrDefault();
            return firstResult == null ? null : firstResult.Model;
        }

        /// <summary>
        /// If overriden dumps the content of an input message which would be created for the specified test configuration
        /// into a string and returns it. This is used only for debugging purposes.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The string content of the input message.</returns>
        protected override string DumpInputMessageContent(ReaderTestConfiguration testConfiguration)
        {
            return this.PayloadString;
        }

        /// <summary>
        /// Returns the payload to be used for this test case and the specified test configuration.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The payload to use for testing.</returns>
        private byte[] GetPayload(ReaderTestConfiguration testConfiguration)
        {
            return Encoding.UTF8.GetBytes(this.PayloadString);
        }
    }
}
