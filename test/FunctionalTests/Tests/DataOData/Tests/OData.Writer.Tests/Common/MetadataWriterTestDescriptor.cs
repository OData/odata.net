//---------------------------------------------------------------------
// <copyright file="MetadataWriterTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    #endregion Namespaces

    /// <summary>
    /// Writer test descriptor for tests which specify the input as ODataPayloadElement
    /// </summary>
    public class MetadataWriterTestDescriptor : WriterTestDescriptor
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public new class Settings : WriterTestDescriptor.Settings
        {
            [InjectDependency(IsRequired = true)]
            public new WriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }
        }

        /// <summary>
        /// The settings for the test descriptor.
        /// </summary>
        protected readonly Settings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public MetadataWriterTestDescriptor(Settings settings)
            : base(settings)
        {
            this.settings = settings;
            this.EdmVersion = EdmVersion.Latest;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="MetadataWriterTestDescriptor"/> payload test descriptor to copy</param>
        public MetadataWriterTestDescriptor(MetadataWriterTestDescriptor other)
            : base(other)
        {
            this.settings = other.settings;
            this.EdmVersion = other.EdmVersion;
        }

        /// <summary>
        /// The metadata in the form of entity model for the payload.
        /// </summary>
        public EdmVersion EdmVersion { get; set; }

        /// <summary>
        /// The expected non-OData exception if the test is expected to fail.
        /// </summary>
        public Exception ExpectedException { get; set; }

        /// <summary>
        /// The expected ODataExceptionMessage if the test is expected to fail.
        /// </summary>
        public string ExpectedODataExceptionMessage { get; set; }

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
                ExceptionUtilities.Assert(value != ODataPayloadKind.MetadataDocument, "Only ODataPayloadKind.MetadataDocument is supported by MetadataWriterTestDescriptor.");
            }
        }

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected override WriterTestExpectedResults GetExpectedResult(WriterTestConfiguration testConfiguration)
        {
            WriterTestExpectedResults expectedResult = base.GetExpectedResult(testConfiguration);
            if (expectedResult == null)
            {
                ODataErrorException errorException = null;
                Exception regularException = null;
                if (this.ExpectedException != null)
                {
                    errorException = this.ExpectedException as ODataErrorException;
                    regularException = errorException == null ? this.ExpectedException : null;
                }

                return new MetadataWriterTestExpectedResult(this.settings.ExpectedResultSettings)
                {
                    EdmVersion = this.EdmVersion,
                    ExpectedException = regularException,
                    ExpectedODataErrorException = errorException,
                    ExpectedODataExceptionMessage = this.ExpectedODataExceptionMessage,
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
        protected override bool ShouldSkipForTestConfiguration(WriterTestConfiguration testConfiguration)
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
        /// Called to write the payload to the specified <paramref name="messageWriter"/>.
        /// </summary>
        /// <param name="messageWriter">The <see cref="ODataMessageWriterTestWrapper"/> to use for writing the payload.</param>
        protected override void WritePayload(ODataMessageWriterTestWrapper messageWriter, WriterTestConfiguration config)
        {
            Debug.Assert(messageWriter != null, "messageWriter != null");

            messageWriter.WriteMetadataDocument();
        }
    }
}
