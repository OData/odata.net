//---------------------------------------------------------------------
// <copyright file="InStreamErrorReaderJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces.
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces.

    /// <summary>
    /// Tests for correct handling of in-stream errors.
    /// </summary>
    [TestClass, TestCase]
    public class InStreamErrorReaderJsonTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings PayloadTestDescriptorSettings { get; set; }

        [InjectDependency]
        public PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings PayloadExpectedResultSettings { get; set; }

        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Validates correct behavior of the ODataJsonReader for in-stream errors in entries.")]
        public void InStreamErrorInEntriesTest()
        {
            EdmModel edmModel = new EdmModel();

            EdmEntityContainer defaultContainer = new EdmEntityContainer("TestModel", "DefaultContainer");
            edmModel.AddElement(defaultContainer);

            var dummyType = new EdmEntityType("TestModel", "DummyEntryType");
            dummyType.AddKeys(dummyType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false)));
            edmModel.AddElement(dummyType);

            var dummySet = defaultContainer.AddEntitySet("DummySet", dummyType);
            
            var errorCases = new []
            {
                new
                {
                    Json = "{{ \"{0}\": {{ \"code\": \"my-error-code\" }} }}",
                    ExpectedException = ODataExpectedExceptions.ODataErrorException(new ODataError { ErrorCode = "my-error-code" }) 
                },
                new
                {
                    Json = "{{ \"{0}\": {{ \"message\": \"my-error-message\" }} }}",
                    ExpectedException = ODataExpectedExceptions.ODataErrorException(new ODataError { Message = "my-error-message" }) 
                },
                new
                {
                    Json = "{{ \"{0}\": {{ \"code\": \"my-error-code\", \"message\": \"my-error-message\" }} }}",
                    ExpectedException = ODataExpectedExceptions.ODataErrorException(new ODataError { ErrorCode = "my-error-code", Message = "my-error-message" }) 
                },
                new
                {
                    Json = "{{ \"{0}\": {{ \"code\": \"my-error-code\", \"innererror\": {{ \"message\": \"my-inner-error\" }} }} }}",
                    ExpectedException = ODataExpectedExceptions.ODataErrorException(new ODataError { ErrorCode = "my-error-code", InnerError = new ODataInnerError { Message = "my-inner-error" }}) 
                },
                new
                {
                    Json = "{{ \"{0}\": {{ \"innererror\": {{ \"message\": \"my-inner-error\" }} }} }}",
                    ExpectedException = ODataExpectedExceptions.ODataErrorException(new ODataError {InnerError = new ODataInnerError { Message = "my-inner-error" }}) 
                },
                new
                {
                    Json = "{{ \"{0}\": {{ \"message\":  \"my-error-message\", \"innererror\": {{ \"message\": \"my-inner-error\" }} }} }}",
                    ExpectedException = ODataExpectedExceptions.ODataErrorException(new ODataError { Message = "my-error-message", InnerError = new ODataInnerError { Message = "my-inner-error" }}) 
                },
                new
                {
                    Json = "{{ \"{0}\": {{ \"code\": \"my-error-code\", \"message\": \"my-error-message\", \"innererror\": {{ \"message\": \"my-inner-error\" }} }} }}",
                    ExpectedException = ODataExpectedExceptions.ODataErrorException(new ODataError { ErrorCode = "my-error-code", Message = "my-error-message", InnerError = new ODataInnerError { Message = "my-inner-error" }}) 
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                errorCases,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (errorCase, testConfiguration) =>
                {
                    string errorElementName = JsonLightConstants.ODataErrorPropertyName;

                    var testCase = new JsonPayloadErrorTestCase
                    {
                        Json = PayloadBuilder.Entity("TestModel.DummyEntryType")
                            .JsonRepresentation(string.Format(errorCase.Json, errorElementName))
                            .ExpectedEntityType(dummyType, dummySet),
                        EdmModel = edmModel,
                        ExpectedExceptionFunc = (tCase, tConfig) => errorCase.ExpectedException,
                    };

                    var descriptor = testCase.ToEdmPayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings, this.PayloadExpectedResultSettings);

                    // NOTE: payload generation is not supported for JSON Light yet
                    var descriptors = testConfiguration.Format == ODataFormat.Json
                        ? new PayloadReaderTestDescriptor[] { descriptor }
                        : this.PayloadGenerator.GenerateReaderPayloads(descriptor);

                    this.CombinatorialEngineProvider.RunCombinations(
                        descriptors,
                        testDescriptor => testDescriptor.RunTest(testConfiguration));
                });
        }

        // TODO: add more tests for in-stream errors in feeds, top-level properties, etc.
    }
}
