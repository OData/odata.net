//---------------------------------------------------------------------
// <copyright file="PayloadReaderTestExpectedResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Reader test expected result which specified the expected ODataPayloadElement.
    /// </summary>
    public class PayloadReaderTestExpectedResult : ReaderTestExpectedResult
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class PayloadReaderTestExpectedResultSettings : ReaderTestExpectedResultSettings
        {
            [InjectDependency(IsRequired = true)]
            public MessageToObjectModelReader MessageToObjectModelReader { get; set; }

            [InjectDependency(IsRequired = true)]
            public ObjectModelToPayloadElementConverter ObjectModelToPayloadElementConverter { get; set; }

            [InjectDependency(IsRequired = true)]
            public IODataPayloadElementComparer ODataPayloadElementComparer { get; set; }

            [InjectDependency(IsRequired = true)]
            public ODataOrderIgnoringPayloadElementComparer IgnorePropertyOrderODataPayloadElementComparer { get; set; }

            [InjectDependency(IsRequired = true)]
            public ODataJsonLightResponsePayloadElementComparer ODataJsonLightResponsePayloadElementComparer { get; set; }

            [InjectDependency(IsRequired = true)]
            public ODataJsonLightResponseOrderIgnoringPayloadElementComparer JsonLightResponseIgnorePropertyOrderODataPayloadElementComparer { get; set; }

            [InjectDependency(IsRequired = true)]
            public IODataObjectModelValidator DefaultODataObjectModelValidator { get; set; }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        private readonly PayloadReaderTestExpectedResultSettings settings;

        /// <summary>
        /// Expected payload element which will be compared to the actual payload element created by reading the input.
        /// </summary>
        public ODataPayloadElement ExpectedPayloadElement { get; set; }

        /// <summary>
        /// The metadata information to be used when reading the actual payload.
        /// </summary>
        public PayloadReaderTestDescriptor.ReaderMetadata ReaderMetadata { get; set; }

        /// <summary>
        /// The OData OM validator to use.
        /// </summary>
        public IODataObjectModelValidator ODataObjectModelValidator { get; set; }

        /// <summary>
        /// true to ignore the order of properties in the payload during comparison; otherwise false.
        /// </summary>
        public bool IgnorePropertyOrder { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public PayloadReaderTestExpectedResult(PayloadReaderTestExpectedResultSettings settings)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(settings, "settings");

            this.settings = settings;
            this.ODataObjectModelValidator = this.settings.DefaultODataObjectModelValidator;
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
            object odataObject = this.settings.MessageToObjectModelReader.ReadMessage(
                messageReader,
                payloadKind,
                /*payloadModel*/ null,
                this.ReaderMetadata,
                /*expectedBatchPayload*/ null,
                testConfiguration);
            if (this.ODataObjectModelValidator != null)
            {
                this.ODataObjectModelValidator.ValidateODataObjectModel(odataObject);
            }

            // only compare the payloads if the expected payload is not 'null'; null indicates to skip the comparison
            if (this.ExpectedPayloadElement != null)
            {
                ODataPayloadElement actualPayloadElement = this.settings.ObjectModelToPayloadElementConverter.Convert(odataObject, !testConfiguration.IsRequest);
                if (this.IgnorePropertyOrder)
                {
                    if (testConfiguration.Format == ODataFormat.Json && !testConfiguration.IsRequest)
                    {
                        this.settings.JsonLightResponseIgnorePropertyOrderODataPayloadElementComparer.Compare(this.ExpectedPayloadElement, actualPayloadElement);
                    }
                    else
                    {
                        this.settings.IgnorePropertyOrderODataPayloadElementComparer.Compare(this.ExpectedPayloadElement, actualPayloadElement);
                    }
                }
                else
                {
                    if (testConfiguration.Format == ODataFormat.Json && !testConfiguration.IsRequest)
                    {
                        this.settings.ODataJsonLightResponsePayloadElementComparer.Compare(this.ExpectedPayloadElement, actualPayloadElement);
                    }
                    else
                    {
                        this.settings.ODataPayloadElementComparer.Compare(this.ExpectedPayloadElement, actualPayloadElement);
                    }
                }
            }
        }
    }
}
