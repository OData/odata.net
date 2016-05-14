//---------------------------------------------------------------------
// <copyright file="CollectionValueReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of collection values in JSON.
    /// </summary>
    [TestClass, TestCase]
    public class CollectionValueReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of collection values")]
        public void CollectionValueTest()
        {
            EdmModel model = new EdmModel();
            var complexType = new EdmComplexType("TestModel", "ComplexType").Property("Name", EdmPrimitiveTypeKind.String, true);
            model.AddElement(complexType);
            var owningType = new EdmEntityType("TestModel", "OwningType");
            owningType.AddKeys(owningType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            owningType.AddStructuralProperty("PrimitiveCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            owningType.AddStructuralProperty("ComplexCollection", EdmCoreModel.GetCollection(complexType.ToTypeReference()));
            model.AddElement(owningType);
            model.Fixup();
            
            var primitiveMultiValue = PayloadBuilder.PrimitiveMultiValue("Collection(Edm.Int32)").Item(42).Item(43);
            var complexMultiValue = PayloadBuilder.ComplexMultiValue("Collection(TestModel.ComplexType)").Item(
                PayloadBuilder.ComplexValue("TestModel.ComplexType")
                    .PrimitiveProperty("Name", "Value")
                    .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }))
                .JsonRepresentation("[{\"Name\":\"Value\"}]")
                .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null collection in request - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Property("PrimitiveCollection",
                        PayloadBuilder.PrimitiveMultiValue("Collection(Edm.Int32)"))
                        .JsonRepresentation("{\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNullAnnotationName + "\":true }")
                        .ExpectedProperty(owningType, "PrimitiveCollection"),
                    SkipTestConfiguration = tc => !tc.IsRequest,
                    ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullValueForNonNullableType", "Collection(Edm.Int32)")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null collection in response - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Property("PrimitiveCollection",
                        PayloadBuilder.PrimitiveMultiValue("Collection(Edm.Int32)"))
                        .JsonRepresentation(
                            "{" + 
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Collection(Edm.Int32)\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNullAnnotationName + "\":true" +
                            "}")
                        .ExpectedProperty(owningType, "PrimitiveCollection"),
                    SkipTestConfiguration = tc => tc.IsRequest,
                    ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullValueForNonNullableType", "Collection(Edm.Int32)")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Primitive value for collection - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Property("PrimitiveCollection",
                        PayloadBuilder.PrimitiveMultiValue("Collection(Edm.Int32)")
                            .JsonRepresentation("42"))
                        .ExpectedProperty(owningType, "PrimitiveCollection"),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "StartArray", "PrimitiveValue")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Object value for collection - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Property("PrimitiveCollection",
                        PayloadBuilder.PrimitiveMultiValue("Collection(Edm.Int32)")
                            .JsonRepresentation("{}"))
                        .ExpectedProperty(owningType, "PrimitiveCollection"),
                    ExpectedException = ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName", "StartArray", "StartObject", "value")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Simple primitive collection.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Property("PrimitiveCollection",
                        primitiveMultiValue
                            .JsonRepresentation("[42,43]")
                            .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }))
                        .ExpectedProperty(owningType, "PrimitiveCollection"),
                    ExpectedResultPayloadElement = tc => tc.IsRequest
                        ? PayloadBuilder.Property(string.Empty, primitiveMultiValue)
                        : PayloadBuilder.Property("PrimitiveCollection", primitiveMultiValue)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Simple complex collection.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Property("ComplexCollection", complexMultiValue)
                        .ExpectedProperty(owningType, "ComplexCollection"),
                    ExpectedResultPayloadElement = tc => tc.IsRequest
                        ? PayloadBuilder.Property(string.Empty, complexMultiValue)
                        : PayloadBuilder.Property("ComplexCollection", complexMultiValue)
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