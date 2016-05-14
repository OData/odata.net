//---------------------------------------------------------------------
// <copyright file="ParameterReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Reader;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of parameter values in JSON Light.
    /// </summary>
    [TestClass, TestCase]
    public class ParameterReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies JSON Light specific error cases.")]
        public void ParameterJsonLightErrorTests()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Primitive").First();

            this.CombinatorialEngineProvider.RunCombinations(
            this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest),
            (testConfiguration) =>
            {
                // Reading a Json parameter payload with an empty name should fail.
                ODataParameterReader reader = ParameterReaderTests.CreateODataParameterReader(model, functionImport, testConfiguration, "{ \"\" : \"foo\" }");
                this.Assert.ExpectedException(
                    () => reader.Read(),
                    ODataExpectedExceptions.ODataExceptionContains("JsonReader_InvalidPropertyNameOrUnexpectedComma", ""), this.ExceptionVerifier);
                this.Assert.AreEqual(ODataParameterReaderState.Exception, reader.State, "Reader should be in 'Exception' state.");

                reader = ParameterReaderTests.CreateODataParameterReader(model, functionImport, testConfiguration, "{ : \"foo\" }");
                this.Assert.ExpectedException(
                    () => reader.Read(),
                    ODataExpectedExceptions.ODataExceptionContains("JsonReader_InvalidPropertyNameOrUnexpectedComma", ""), this.ExceptionVerifier);
                this.Assert.AreEqual(ODataParameterReaderState.Exception, reader.State, "Reader should be in 'Exception' state.");

                reader = ParameterReaderTests.CreateODataParameterReader(model, functionImport, testConfiguration, "{ null : \"foo\" }");
                this.Assert.ExpectedException(
                    () => reader.Read(),
                    ODataExpectedExceptions.ODataException("ODataParameterReaderCore_ParameterNameNotInMetadata", "null", functionImport.Name), this.ExceptionVerifier);
                this.Assert.AreEqual(ODataParameterReaderState.Exception, reader.State, "Reader should be in 'Exception' state.");
            });
        }

        [TestMethod, TestCategory("Reader.Operations"), Variation(Description = "Verifies correct parsing of parameter payloads with annotations.")]
        public void ParameterReaderJsonLightTest()
        {
            EdmModel model = new EdmModel();
            model.Fixup();

            EdmEntityContainer container = model.EntityContainer as EdmEntityContainer;

            EdmFunction function = new EdmFunction(container.Namespace, "f1", EdmCoreModel.Instance.GetInt32(true));
            function.AddParameter("p1", EdmCoreModel.Instance.GetInt32(false) as EdmTypeReference);
            function.AddParameter("p2", EdmCoreModel.Instance.GetString(false) as EdmTypeReference);
            model.AddElement(function);
            EdmOperationImport f1 = container.FunctionImport(function);
                
            ComplexInstance f1Params = PayloadBuilder.ComplexValue().PrimitiveProperty("p1", 42).PrimitiveProperty("p2", "Vienna")
                .ExpectedFunctionImport(f1);

            var testCases = new[]
            {
                new
                {
                    DebugDescription = "Custom property annotation for a valid parameter property (before property) - should work.",
                    Json = "{\"" + JsonLightUtils.GetPropertyAnnotationName("p1", "my.custom") + "\":42, \"p1\":42, \"p2\":\"Vienna\"}",
                    ExpectedException = (ExpectedException)null,
                },
                new
                {
                    DebugDescription = "Custom property annotation for a valid parameter property (after property) - should fail.",
                    Json = "{\"p1\":42, \"" + JsonLightUtils.GetPropertyAnnotationName("p1", "my.custom") + "\":42, \"p2\":\"Vienna\"}",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty", "my.custom", "p1"),
                },
                new
                {
                    DebugDescription = "OData property annotation for a valid parameter property (before property) - should work.",
                    Json = "{\"" + JsonLightUtils.GetPropertyAnnotationName("p1", JsonLightConstants.ODataTypeAnnotationName) + "\":42, \"p1\":42, \"p2\":\"Vienna\"}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters"),
                },
                new
                {
                    DebugDescription = "OData property annotation for a valid parameter property (after property) - should fail.",
                    Json = "{\"p1\":42, \"" + JsonLightUtils.GetPropertyAnnotationName("p1", JsonLightConstants.ODataTypeAnnotationName) + "\":42, \"p2\":\"Vienna\"}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters"),
                },
                new
                {
                    DebugDescription = "Custom property annotation for an invalid parameter property - should fail.",
                    Json = "{\"" + JsonLightUtils.GetPropertyAnnotationName("p0", "my.custom") + "\":42, \"p1\":42, \"p2\":\"Vienna\"}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters", "p0"),
                },
                new
                {
                    DebugDescription = "Custom instance annotation - should work.",
                    Json = "{\"@my.custom\":42, \"p1\":42, \"p2\":\"Vienna\"}",
                    ExpectedException = (ExpectedException)null,
                },
                new
                {
                    DebugDescription = "OData instance annotation - should fail.",
                    Json = "{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.Int32\", \"p1\":42, \"p2\":\"Vienna\"}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties", JsonLightConstants.ODataTypeAnnotationName),
                },
            };

            var testDescriptors = testCases.Select(testCase =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.DebugDescription,
                    PayloadElement = f1Params.DeepCopy().JsonRepresentation(testCase.Json),
                    PayloadEdmModel = model,
                    PayloadKind = ODataPayloadKind.Parameter,
                    ExpectedException = testCase.ExpectedException,
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(c => c.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.ExpectedResultNormalizers.Add(tc => ParameterReaderTests.FixupExpectedCollectionParameterPayloadElement);

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

    }
}
