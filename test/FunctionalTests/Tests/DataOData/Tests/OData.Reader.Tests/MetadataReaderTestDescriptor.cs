//---------------------------------------------------------------------
// <copyright file="MetadataReaderTestDescriptor.cs" company="Microsoft">
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
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Reader test descriptor for metadata tests.
    /// </summary>
    public class MetadataReaderTestDescriptor : ReaderTestDescriptor
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class Settings
        {
            [InjectDependency(IsRequired = true)]
            public AssertionHandler Assert { get; set; }

            [InjectDependency(IsRequired = true)]
            public MetadataReaderTestExpectedResult.MetadataReaderTestExpectedResultSettings ExpectedResultSettings { get; set; }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        protected readonly Settings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public MetadataReaderTestDescriptor(Settings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The other metadata test descriptor to copy</param>
        public MetadataReaderTestDescriptor(MetadataReaderTestDescriptor other)
            : base(other)
        {
            this.settings = other.settings;
            this.PayloadEdmModel = other.PayloadEdmModel;
            this.ExpectedPayloadEdmModel = other.ExpectedPayloadEdmModel;
            this.ExpectedException = other.ExpectedException;
            this.ContentType = other.ContentType;
        }

        /// <summary>
        /// The payload kind which is being tested.
        /// </summary>
        public override ODataPayloadKind PayloadKind
        {
            get
            {
                return ODataPayloadKind.MetadataDocument;
            }
            set
            {
                ExceptionUtilities.Assert(value == ODataPayloadKind.MetadataDocument, "Only ODataPayloadKind.MetadataDocument is supported.");
            }
        }

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

        /// <summary>
        /// The expected edm model. If null the payload edm model will be used.
        /// </summary>
        public IEdmModel ExpectedPayloadEdmModel { get; set; }

        /// <summary>
        /// The expected exception if the test is expected to fail.
        /// </summary>
        public ExpectedException ExpectedException { get; set; }

        /// <summary>
        /// Transformation to apply to the metadata document payload.
        /// </summary>
        public IPayloadTransform<XElement> MetadataDocumentTransform { get; set; }

        /// <summary>
        /// The content type header to use, if set overrides the content type derived from the test configuration.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Creates a copy of this MetadataReaderTestDescriptor.
        /// </summary>
        public override object Clone()
        {
            return new MetadataReaderTestDescriptor(this);
        }

        /// <summary>
        /// Returns description of the test case.
        /// </summary>
        /// <returns>Humanly readable description of the test. Used for debugging.</returns>
        public override string ToString()
        {
            string result = base.ToString();

            if (this.PayloadEdmModel != null)
            {
                // There's no short way of serializing the payload model. Instead if the test fails the model
                // will be serialized as CSDL and dumped to the test output.
                result += "\r\nPayload model present";
            }

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

            TestMessage testMessage = TestReaderUtils.CreateInputMessageFromStream(
                messageStream, 
                testConfiguration, 
                ODataPayloadKind.MetadataDocument,
                this.ContentType,
                /*urlResolver*/ null);
            return testMessage;
        }

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected override ReaderTestExpectedResult GetExpectedResult(ReaderTestConfiguration testConfiguration)
        {
            ReaderTestExpectedResult expectedResult = base.GetExpectedResult(testConfiguration);
            if (expectedResult == null)
            {
                return new MetadataReaderTestExpectedResult(this.settings.ExpectedResultSettings)
                {
                    ExpectedException = this.ExpectedException,
                };
            }
            else
            {
                return expectedResult;
            }
        }

        /// <summary>
        /// Called before the test is actually executed for the specified test configuration to determine if the test should be skipped.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>true if the test should be skipped for the <paramref name="testConfiguration"/> or false to run the test.</returns>
        /// <remarks>Derived classes should always call the base class and return true if the base class returned true.</remarks>
        protected override bool ShouldSkipForTestConfiguration(ReaderTestConfiguration testConfiguration)
        {
            if (this.SkipTestConfiguration != null)
            {
                return this.SkipTestConfiguration(testConfiguration);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the model to use for the specified test configuration.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The model to use for the test.</returns>
        protected override IEdmModel GetMetadataProvider(ReaderTestConfiguration testConfiguration)
        {
            // for reading metadata we should not require a model when creating the ODataMessageReader
            return null;
        }

        /// <summary>
        /// If overridden dumps the content of an input message which would be created for the specified test configuration
        /// into a string and returns it. This is used only for debugging purposes.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The string content of the input message.</returns>
        protected override string DumpInputMessageContent(ReaderTestConfiguration testConfiguration)
        {
            byte[] payload = this.GetPayload(testConfiguration);
            return Encoding.UTF8.GetString(payload, 0, payload.Length);
        }

        /// <summary>
        /// Returns the payload to be used for this test case and the specified test configuration.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The payload to use for testing.</returns>
        private byte[] GetPayload(ReaderTestConfiguration testConfiguration)
        {
            if (testConfiguration.Format != null && testConfiguration.Format != ODataFormat.Metadata)
            {
                // NOTE: metadata reading is not supported in other formats; return an empty payload for error tests
                return new byte[0];
            }

            TestStream testStream = new TestStream(new MemoryStream(), ignoreDispose: true);

            if (this.PayloadEdmModel != null)
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(testStream))
                {
                    IEnumerable<EdmError> errors;

                    if (!CsdlWriter.TryWriteCsdl(this.PayloadEdmModel, xmlWriter, CsdlTarget.OData, out errors))
                    {
                        var errorBuilder = new StringBuilder();

                        foreach (var error in errors)
                        {
                            errorBuilder.AppendLine(error.ToString());
                        }

                        throw new Exception("TryWriteEdmx Error:" + errorBuilder);
                    }
                }
            }

            byte[] payload = new byte[testStream.Length];
            testStream.Seek(0, SeekOrigin.Begin);
            testStream.Read(payload, 0, (int)testStream.Length);
            return payload;
        }
    }
}
