//---------------------------------------------------------------------
// <copyright file="CollectionReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of top-level collection values in JSON Light.
    /// </summary>
    [TestClass, TestCase]
    public class CollectionReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of top-level collections.")]
        public void CollectionReaderTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            EdmOperationImport primitiveCollectionResultOperation = container.FindOperationImports("PrimitiveCollectionResultOperation").Single() as EdmOperationImport;
            EdmOperationImport complexCollectionResultOperation = container.FindOperationImports("ComplexCollectionResultOperation").Single() as EdmOperationImport;

            var primitiveCollection = PayloadBuilder.PrimitiveCollection("PrimitiveCollectionResultOperation");
            primitiveCollection.Add(PayloadBuilder.PrimitiveValue(1));
            primitiveCollection.Add(PayloadBuilder.PrimitiveValue(2));
            primitiveCollection.Add(PayloadBuilder.PrimitiveValue(3));

            var complexCollection = PayloadBuilder.ComplexCollection("ComplexCollectionResultOperation");
            var complexValue1 = PayloadBuilder.ComplexValue("TestModel.Address")
                    .PrimitiveProperty("Street", "Am Euro Platz")
                    .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
            complexCollection.Add(complexValue1);

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Primitive collection - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ] }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Complex collection - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = complexCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(TestModel.Address)\", \"" + JsonLightConstants.ODataValuePropertyName + "\":[ { \"Street\":\"Am Euro Platz\" } ] }")
                        .ExpectedFunctionImport(complexCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null collection - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy().JsonRepresentation("null")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Primitive value for collection - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy().JsonRepresentation("42")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Array value for collection - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy().JsonRepresentation("[]")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Extra property before collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"extra\": null, \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ] }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName", "extra", "value")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Extra property after collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ], \"extra\": null }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd", "extra")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom instance annotation before collection property - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"@my.extra\": null, \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ] }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom instance annotation after collection property - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ], \"@my.extra\": null }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "OData instance annotations before collection property - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName + "\":3," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNextLinkAnnotationName + "\":\"http://next-link\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ] }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "OData instance annotations after collection property - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ]," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNextLinkAnnotationName + "\":\"http://next-link\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataCountAnnotationName + "\":3}")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom property annotation before collection property - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"value@my.extra\": null, \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ] }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom property annotation on 'value' after collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ], \"value@my.extra\": null }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd", "value")
                },                
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom property annotation on 'extra' before collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"extra@my.extra\": null, \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ] }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty", "extra")
                },                
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Custom property annotation on 'extra' after collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ], \"extra@my.extra\": null }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd", "extra")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Invalid collection property name - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"invalid\":[ 1, 2, 3 ] }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName", "invalid", JsonLightConstants.ODataValuePropertyName)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Missing collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\" }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound", JsonLightConstants.ODataValuePropertyName)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Invalid collection property value - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", \"" + JsonLightConstants.ODataValuePropertyName + "\":{} }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart", "StartObject")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Invalid OData instance annotation before collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataEditLinkAnnotationName + "\":null, " +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[] }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties", JsonLightConstants.ODataEditLinkAnnotationName)
                },                
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Invalid OData instance annotation after collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[], " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataEditLinkAnnotationName + "\":null }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd", JsonLightConstants.ODataEditLinkAnnotationName)
                },                
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Invalid OData instance annotation after collection property - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[], " +

                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataEditLinkAnnotationName + "\":null }")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd", JsonLightConstants.ODataEditLinkAnnotationName)
                },                
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of top-level collections that specify an odata.type annotation.")]
        public void CollectionWithODataTypeReaderTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            EdmOperationImport primitiveCollectionResultOperation = container.FindOperationImports("PrimitiveCollectionResultOperation").Single() as EdmOperationImport;
            EdmOperationImport complexCollectionResultOperation = container.FindOperationImports("ComplexCollectionResultOperation").Single() as EdmOperationImport;

            var primitiveCollection = PayloadBuilder.PrimitiveCollection("PrimitiveCollectionResultOperation");
            primitiveCollection.Add(PayloadBuilder.PrimitiveValue(1));
            primitiveCollection.Add(PayloadBuilder.PrimitiveValue(2));
            primitiveCollection.Add(PayloadBuilder.PrimitiveValue(3));

            var complexCollection = PayloadBuilder.ComplexCollection("ComplexCollectionResultOperation");
            var complexValue1 = PayloadBuilder.ComplexValue("TestModel.Address")
                    .PrimitiveProperty("Street", "Am Euro Platz")
                    .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
            complexCollection.Add(complexValue1);

            // NOTE: tests to cover consistency between the expected function import and the function import
            //       in the payload exist in the ContextUriValidationJsonLightTests.
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Non-collection type specified in odata.type - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.Address\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ]" +
                            "}")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName", "TestModel.Address"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Expected primitive item type and odata.type consistent - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(Edm.Int32)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ]" +
                            "}")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Expected complex item type and odata.type consistent - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = complexCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(TestModel.Address)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(TestModel.Address)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ { \"Street\": \"Am Euro Platz\"} ]" +
                            "}")
                        .ExpectedFunctionImport(complexCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Context URI (primitive collection) and odata.type consistent - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(Edm.Int32)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ]" +
                            "}")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Context URI (complex collection) and odata.type consistent - should work.",
                    PayloadEdmModel = model,
                    PayloadElement = complexCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(TestModel.Address)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(TestModel.Address)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ { \"Street\": \"Am Euro Platz\"} ]" +
                            "}")
                        .ExpectedFunctionImport(complexCollectionResultOperation),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Expected primitive item type and odata.type NOT consistent - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(Edm.Single)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ]" +
                            "}")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatibleType", "Edm.Single", "Edm.Int32")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Expected primitive item type and odata.type NOT consistent (2) - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = primitiveCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(TestModel.Address)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(Edm.Int32)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ 1, 2, 3 ]" +
                            "}")
                        .ExpectedFunctionImport(complexCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Edm.Int32", "Complex", "Primitive")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Expected complex item type and odata.type NOT consistent - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = complexCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(TestModel.Address)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(TestModel.OfficeType)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ { \"Street\": \"Am Euro Platz\"} ]" +
                            "}")
                        .ExpectedFunctionImport(complexCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestModel.OfficeType", "Complex", "Entity")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Expected primitive item type and odata.type NOT consistent (2) - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = complexCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(TestModel.Address)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ { \"Street\": \"Am Euro Platz\"} ]" +
                            "}")
                        .ExpectedFunctionImport(primitiveCollectionResultOperation),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestModel.Address", "Primitive", "Complex")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Complex item type in context URI and odata.type NOT consistent - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = complexCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(TestModel.Address)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(TestModel.OfficeType)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ { \"Street\": \"Am Euro Platz\"} ]" +
                            "}"),
                    SkipTestConfiguration = tc => tc.IsRequest,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestModel.OfficeType", "Complex", "Entity")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Primitive item type in context URI and odata.type NOT consistent - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = complexCollection.DeepCopy()
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(TestModel.Address)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":[ { \"Street\": \"Am Euro Platz\"} ]" +
                            "}"),
                    SkipTestConfiguration = tc => tc.IsRequest,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestModel.Address", "Primitive", "Complex")
                },
            };
            
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}