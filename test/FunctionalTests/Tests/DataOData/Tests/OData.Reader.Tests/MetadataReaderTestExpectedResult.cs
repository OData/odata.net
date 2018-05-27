//---------------------------------------------------------------------
// <copyright file="MetadataReaderTestExpectedResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Reader test expected result which specified the expected metadata document.
    /// </summary>
    public class MetadataReaderTestExpectedResult : ReaderTestExpectedResult
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class MetadataReaderTestExpectedResultSettings : ReaderTestExpectedResultSettings
        {
            [InjectDependency(IsRequired = true)]
            public MessageToObjectModelReader MessageToObjectModelReader { get; set; }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        private readonly MetadataReaderTestExpectedResultSettings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public MetadataReaderTestExpectedResult(MetadataReaderTestExpectedResultSettings settings)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(settings, "settings");

            this.settings = settings;
        }

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
            this.settings.Assert.IsTrue(payloadKind == ODataPayloadKind.MetadataDocument, "Only metadata payload kind is supported.");

            object metadata = this.settings.MessageToObjectModelReader.ReadMessage(
                messageReader, 
                payloadKind,
                /*payloadModel*/ null,
                PayloadReaderTestDescriptor.ReaderMetadata.None,
                /*expectedBatchPayload*/ null,
                testConfiguration);
            IEdmModel actualModel = metadata as IEdmModel;
            this.settings.Assert.IsTrue(metadata != null && actualModel != null, "Expected a non-null model to be read.");
        }
    }
}
