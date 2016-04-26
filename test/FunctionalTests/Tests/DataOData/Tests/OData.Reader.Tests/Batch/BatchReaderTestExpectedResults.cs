//---------------------------------------------------------------------
// <copyright file="BatchReaderTestExpectedResults.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Batch Reader test expected result which specified the expected ODataPayloadElement.
    /// </summary>
    public class BatchReaderTestExpectedResult : ReaderTestExpectedResult
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class BatchReaderTestExpectedResultSettings : ReaderTestExpectedResultSettings
        {
            [InjectDependency(IsRequired = true)]
            public MessageToObjectModelReader MessageToObjectModelReader { get; set; }

            [InjectDependency(IsRequired = true)]
            public ObjectModelToPayloadElementConverter ObjectModelToPayloadElementConverter { get; set; }

            //[InjectDependency(IsRequired = true)]
            //public IODataPayloadElementComparer ODataPayloadElementComparer { get; set; }

            //[InjectDependency(IsRequired = true)]
            //public ReaderDefaultODataObjectModelValidator DefaultODataObjectModelValidator { get; set; }

            //[InjectDependency(IsRequired = true)]
            //public IBatchPayloadDeserializer BatchDeSerializer { get; set; }

            [InjectDependency(IsRequired = true)]
            public IBatchPayloadComparer BatchComparer { get; set; }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        protected readonly BatchReaderTestExpectedResultSettings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public BatchReaderTestExpectedResult(BatchReaderTestExpectedResultSettings settings)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(settings, "settings");

            this.settings = settings;
        }

        /// <summary>
        /// Expected payload element which will be compared to the actual payload element created by reading the input.
        /// </summary>
        public ODataPayloadElement ExpectedBatchPayload { get; set; }

        /// <summary>
        /// Model for the payload of the various parts of the batch.
        /// </summary>
        public IEdmModel PayloadModel { get; set; }

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
            object odataObject = this.settings.MessageToObjectModelReader.ReadMessage(
                messageReader,
                ODataPayloadKind.Batch,
                this.PayloadModel,
                PayloadReaderTestDescriptor.ReaderMetadata.None,
                this.ExpectedBatchPayload,
                testConfiguration);

            // only compare the payloads if the expected payload is not 'null'; null indicates to skip the comparison
            if (this.ExpectedBatchPayload != null)
            {
                ODataPayloadElement actualPayloadElement = this.settings.ObjectModelToPayloadElementConverter.Convert(odataObject, !testConfiguration.IsRequest);
                this.settings.BatchComparer.CompareBatchPayload(this.ExpectedBatchPayload, actualPayloadElement);
            }
        }
    }
}
