//---------------------------------------------------------------------
// <copyright file="ComplexValueReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Contracts.JsonLight;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of complex values in JSON.
    /// </summary>
    [TestClass, TestCase]
    public class ComplexValueReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of complex values")]
        public void ComplexValueTest()
        {
            var injectedProperties = new[]
                {
                    new
                    {
                        InjectedJSON = string.Empty,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        InjectedJSON = "\"@custom.annotation\": null",
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        InjectedJSON = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAnnotationNamespacePrefix + "unknown\": { }",
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        InjectedJSON = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\": { }",
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties", JsonLightConstants.ODataContextAnnotationName)
                    },
                    new
                    {
                        InjectedJSON = "\"@custom.annotation\": null, \"@custom.annotation\": 42",
                        ExpectedException = (ExpectedException)null
                    },
                };

            var payloads = new[]
                {
                    new
                    {
                        Json = "{0}",
                        ExpectedValue = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true)
                    },
                    new
                    {
                        Json = "{0}{1} \"Name\": \"Value\"",
                        ExpectedValue = PayloadBuilder.Entity("TestModel.ComplexType")
                            .PrimitiveProperty("Name", "Value").IsComplex(true)
                    },
                    new
                    {
                        Json = "\"Name\": \"Value\"{1}{0}",
                        ExpectedValue = PayloadBuilder.Entity("TestModel.ComplexType")
                            .PrimitiveProperty("Name", "Value").IsComplex(true)
                    },
                    new
                    {
                        Json = "\"Name\":\"Value\",{0}{1}\"City\":\"Redmond\"",
                        ExpectedValue = PayloadBuilder.Entity("TestModel.ComplexType")
                            .PrimitiveProperty("Name", "Value")
                            .PrimitiveProperty("City", "Redmond")
                            .IsComplex(true)
                    },
                };

            EdmModel model = new EdmModel();

            var addressType = new EdmComplexType("TestModel", "Address");
            addressType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(isNullable: false));

            var complexType = new EdmComplexType("TestModel", "ComplexType");
            complexType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: false));
            complexType.AddStructuralProperty("City", EdmCoreModel.Instance.GetString(isNullable: false));
            complexType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, isNullable: false));
            complexType.AddStructuralProperty("Location", EdmPrimitiveTypeKind.GeographyPoint);

            var owningType = new EdmEntityType("TestModel", "OwningType");
            owningType.AddKeys(owningType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(isNullable: false)));
            owningType.AddStructuralProperty("TopLevelProperty", new EdmComplexTypeReference(complexType, isNullable: true));
            model.AddElement(owningType);
            model.AddElement(addressType);
            model.AddElement(complexType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("OwningType", owningType);
            model.AddElement(container);

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = payloads.SelectMany(payload => injectedProperties.Select(injectedProperty =>
            {
                var json = string.Format(payload.Json, injectedProperty.InjectedJSON, string.IsNullOrEmpty(injectedProperty.InjectedJSON) ? string.Empty : ",");
                json = string.Format("{{{0}{1}{2}}}",
                    "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.ComplexType\"",
                    string.IsNullOrEmpty(json) ? string.Empty : ",", json);
                return new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    PayloadElement = payload.ExpectedValue.DeepCopy()
                            .JsonRepresentation(json)
                        // JSON Light payloads don't store complex type names since the type can be inferred from the context URI and or API.
                            .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                            .ExpectedEntityType(complexType),
                    PayloadEdmModel = model,
                    ExpectedException = injectedProperty.ExpectedException,
                    ExpectedResultPayloadElement = tc => tc.IsRequest
                        ? payload.ExpectedValue
                            .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }).IsComplex(true)
                        : payload.ExpectedValue
                            .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }).IsComplex(true)
                };
            }));

            var explicitPayloadsForPrimitiveAndArray = new[]
                {
                    new
                    {
                        Description = "Primitive value as complex - should fail.",
                        Json = "42",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType"),
                        ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "PrimitiveValue")
                    },
                    new
                    {
                        Description = "Array value as complex - should fail.",
                        Json = "[]",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType"),
                        ExpectedException = ODataExpectedExceptions.ODataException("JsonReaderExtensions_UnexpectedNodeDetected", "StartObject", "StartArray")
                    },
                };

            testDescriptors = testDescriptors.Concat(explicitPayloadsForPrimitiveAndArray.Select(payload =>
            {
                return new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payload.ExpectedPayload.DeepCopy()
                            .JsonRepresentation(payload.Json)
                        // JSON Light payloads don't store complex type names since the type can be inferred from the context URI and or API.
                            .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                            .ExpectedEntityType(complexType),
                    PayloadEdmModel = model,
                    ExpectedException = payload.ExpectedException,
                    DebugDescription = payload.Description,
                    ExpectedResultPayloadElement = tc => payload.ExpectedPayload.DeepCopy()
                            .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                };
            }));

            var explicitPayloads = new[]
                {
                    new
                    {
                        Description = "Type annotation preceded by custom annotation - should fail",
                        Json = "\"@custom.annotation\": null," +
                               "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"TestModel.ComplexType\"," +
                               "\"Name\": \"Value\"",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_ResourceTypeAnnotationNotFirst")
                    },
                    new
                    {
                        Description = "Custom property annotation - should be ignored.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", "custom.annotation") + "\": null," +
                               "\"Name\": \"Value\"",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType")
                            .PrimitiveProperty("Name", "Value").IsComplex(true),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Duplicate custom property annotation - should not fail.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", "custom.annotation") + "\": null," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", "custom.annotation") + "\": 42," +
                                "\"Name\": \"Value\"",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType")
                                          .PrimitiveProperty("Name", "Value").IsComplex(true),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Unrecognized odata property annotation - should fail.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataAnnotationNamespacePrefix + "unknown") + "\": null," +
                               "\"Name\": \"Value\"",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").PrimitiveProperty("Name", "Value").IsComplex(true),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Custom property annotation after the property - should fail.",
                        Json = "\"Name\": \"Value\"," +
                               "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", "custom.annotation") + "\": null",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true),
                        ExpectedException = ODataExpectedExceptions.ODataException("PropertyAnnotationAfterTheProperty", "custom.annotation", "Name")
                    },
                    new
                    {
                        Description = "OData property annotation.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\": \"Edm.String\"," +
                               "\"Name\": \"Value\"",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType")
                            .PrimitiveProperty("Name", "Value").IsComplex(true),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Duplicate odata property annotation.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\": \"Edm.Int32\"," +
                               "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\": \"Edm.String\"," +
                               "\"Name\": \"Value\"",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true),
                        ExpectedException = ODataExpectedExceptions.ODataException("DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataTypeAnnotationName, "Name")
                    },
                    new
                    {
                        Description = "Property with object value and type name annotation - should fail.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Address", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.Address\"," +
                                "\"Address\":{}",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation", JsonLightConstants.ODataTypeAnnotationName)
                    },
                    new
                    {
                        Description = "String property with type annotation after the property - should fail.",
                        Json = "\"Name\":\"value\"," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.String\"",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true),
                        ExpectedException = ODataExpectedExceptions.ODataException("PropertyAnnotationAfterTheProperty", JsonLightConstants.ODataTypeAnnotationName, "Name")
                    },
                    new
                    {
                        Description = "String property with null type annotation - should fail.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\":null," +
                                "\"Name\":\"value\"",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName", string.Empty)
                    },
                    new
                    {
                        Description = "null property with unknown type annotation - should fail.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Unknown\"," +
                                "\"Name\":null",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true),
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Unknown", "Primitive", "Complex"),
                    },
                    new
                    {
                        Description = "Spatial property with odata.type annotation inside the GeoJson object - should fail.",
                        Json = "\"Location\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, GeographyFactory.Point(33.1, -110.0).Build(), "Edm.GeographyPoint"),
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue", JsonLightConstants.ODataTypeAnnotationName),
                    },
                    new
                    {
                        Description = "Spatial property with odata.type annotation inside and outside the GeoJson object - should fail.",
                        Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("Location", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.GeographyPoint\"," +
                                "\"Location\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, GeographyFactory.Point(33.1, -110.0).Build(), "Edm.GeographyPoint"),
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.ComplexType").IsComplex(true),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue", JsonLightConstants.ODataTypeAnnotationName),
                    },
                };

            testDescriptors = testDescriptors.Concat(explicitPayloads.Select(payload =>
            {
                var json = string.Format("{{{0}{1}{2}}}",
                    "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.ComplexType\"",
                    string.IsNullOrEmpty(payload.Json) ? string.Empty : ",", payload.Json);
                return new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payload.ExpectedPayload.DeepCopy()
                            .JsonRepresentation(json)
                        // JSON Light payloads don't store complex type names since the type can be inferred from the context URI and or API.
                            .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                            .ExpectedEntityType(complexType),
                    PayloadEdmModel = model,
                    ExpectedException = payload.ExpectedException,
                    DebugDescription = payload.Description,
                    ExpectedResultPayloadElement = tc => payload.ExpectedPayload.DeepCopy()
                            .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                };
            }));

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
