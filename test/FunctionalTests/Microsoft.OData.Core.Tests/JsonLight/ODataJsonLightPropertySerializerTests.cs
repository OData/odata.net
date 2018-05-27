//---------------------------------------------------------------------
// <copyright file="ODataJsonLightPropertySerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightPropertySerializerTests
    {
        private IEdmModel model;
        private IEdmEntityType entityType;
        private ODataProperty declaredProperty;
        private ODataProperty undeclaredProperty;
        private ODataProperty declaredGeometryProperty;
        private EdmComplexType openAddressType;
        private EdmTypeDefinition myInt32;
        private EdmTypeDefinition myString;
        private ODataProperty declaredPropertyCountryRegion;
        private ODataProperty declaredPropertyCountryRegionWithInstanceAnnotation;
        private ODataProperty undeclaredPropertyCity;
        private ODataProperty declaredPropertyMyInt32;
        private ODataProperty declaredPropertyMyInt32WithInstanceAnnotations;
        private ODataProperty declaredPropertyMyString;
        private ODataProperty declaredPropertyTimeOfDay;
        private ODataProperty declaredPropertyDate;

        public ODataJsonLightPropertySerializerTests()
        {
            // Initialize open EntityType: EntityType.
            EdmModel edmModel = new EdmModel();

            myInt32 = new EdmTypeDefinition("TestNamespace", "MyInt32", EdmPrimitiveTypeKind.Int32);
            EdmTypeDefinitionReference myInt32Reference = new EdmTypeDefinitionReference(myInt32, true);
            edmModel.AddElement(myInt32);

            myString = new EdmTypeDefinition("TestNamespace", "MyString", EdmPrimitiveTypeKind.String);
            EdmTypeDefinitionReference myStringReference = new EdmTypeDefinitionReference(myString, true);
            edmModel.AddElement(myString);

            EdmEntityType edmEntityType = new EdmEntityType("TestNamespace", "EntityType", baseType: null, isAbstract: false, isOpen: true);
            edmEntityType.AddStructuralProperty("DeclaredProperty", EdmPrimitiveTypeKind.Guid);
            edmEntityType.AddStructuralProperty("DeclaredGeometryProperty", EdmPrimitiveTypeKind.Geometry);
            edmEntityType.AddStructuralProperty("DeclaredSingleProperty", EdmPrimitiveTypeKind.Single);
            edmEntityType.AddStructuralProperty("DeclaredDoubleProperty", EdmPrimitiveTypeKind.Double);
            edmEntityType.AddStructuralProperty("MyInt32Property", myInt32Reference);
            edmEntityType.AddStructuralProperty("MyStringProperty", myStringReference);
            edmEntityType.AddStructuralProperty("TimeOfDayProperty", EdmPrimitiveTypeKind.TimeOfDay);
            edmEntityType.AddStructuralProperty("DateProperty", EdmPrimitiveTypeKind.Date);

            edmModel.AddElement(edmEntityType);

            // Initialize open ComplexType: OpenAddress.
            this.openAddressType = new EdmComplexType("TestNamespace", "OpenAddress", baseType: null, isAbstract: false, isOpen: true);
            this.openAddressType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            edmModel.AddElement(this.openAddressType);

            this.model = TestUtils.WrapReferencedModelsToMainModel(edmModel);
            this.entityType = edmEntityType;
            this.declaredProperty = new ODataProperty { Name = "DeclaredProperty", Value = Guid.Empty };
            this.undeclaredProperty = new ODataProperty { Name = "UndeclaredProperty", Value = DateTimeOffset.MinValue };
            this.declaredGeometryProperty = new ODataProperty { Name = "DeclaredGeometryProperty", Value = GeometryPoint.Create(0.0, 0.0) };
            this.declaredPropertyTimeOfDay = new ODataProperty { Name = "TimeOfDayProperty", Value = new TimeOfDay(1, 30, 5, 123) };
            this.declaredPropertyDate = new ODataProperty { Name = "DateProperty", Value = new Date(2014, 9, 17) };

            this.declaredPropertyCountryRegion = new ODataProperty() { Name = "CountryRegion", Value = "China" };
            this.declaredPropertyCountryRegionWithInstanceAnnotation = new ODataProperty()
            {
                Name = "CountryRegion",
                Value = "China",
                InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(true))
                }
            };
            this.undeclaredPropertyCity = new ODataProperty() { Name = "City", Value = "Shanghai" };

            this.declaredPropertyMyInt32 = new ODataProperty() { Name = "MyInt32Property", Value = 12345 };
            this.declaredPropertyMyInt32WithInstanceAnnotations = new ODataProperty()
            {
                Name = "MyInt32Property",
                Value = 12345,
                InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.AutoComputable", new ODataPrimitiveValue(true)),
                    new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(false))
                }
            };
            this.declaredPropertyMyString = new ODataProperty() { Name = "MyStringProperty", Value = "abcde" };
        }

        #region Default type name serialization behavior for primitive values
        [Fact]
        public void DeclaredPrimitivePropertyWithNoSerializationTypeNameAnnotationShouldNotWriteTypeName()
        {
            this.SerializeProperty(this.entityType, this.declaredProperty).Should().NotContain("@odata.type");
        }

        [Fact]
        public void DeclaredPrimitivePropertyOfDerivedTypeShouldWriteTypeNameFromValue()
        {
            // If the type of the actual value is more derived than the property type declared in metadata, we write out the more derived type name.
            this.SerializeProperty(this.entityType, this.declaredGeometryProperty).Should().Contain("@odata.type\":\"#GeometryPoint\"");
        }

        [Fact]
        public void UndeclaredDoubleShouldNotWriteTypeName()
        {
            this.UndeclaredPropertyShouldNotWriteTypeName(42.0);
        }

        [Fact]
        public void UndeclaredStringShouldNotWriteTypeName()
        {
            this.UndeclaredPropertyShouldNotWriteTypeName("MyString");
        }

        [Fact]
        public void UndeclaredBoolShouldNotWriteTypeName()
        {
            this.UndeclaredPropertyShouldNotWriteTypeName(true);
        }

        [Fact]
        public void UndeclaredInt32ShouldNotWriteTypeName()
        {
            this.UndeclaredPropertyShouldNotWriteTypeName(42);
        }

        [Fact]
        public void UndeclaredStringSerializedDoubleShouldWriteTypeName()
        {
            // Double values which are serialized as strings (Infinity, NaN) must include type information since the reader can't infer it from the json value.
            this.UndeclaredPropertyShouldWriteTypeName(Double.PositiveInfinity, "Double");
        }

        [Fact]
        public void UndeclaredInt16ShouldWriteTypeName()
        {
            this.UndeclaredPropertyShouldWriteTypeName((Int16)42, "Int16");
        }

        [Fact]
        public void UndeclaredUntypedShouldNotWriteTypeName()
        {
            this.UndeclaredPropertyShouldNotWriteTypeName(new ODataUntypedValue { RawValue = "\"rawValue\"" });
        }

        private void UndeclaredPropertyShouldNotWriteTypeName(object value)
        {
            var property = new ODataProperty { Name = "UndeclaredProperty", Value = value };
            this.SerializeProperty(this.entityType, property).Should().NotContain("@odata.type");
        }

        private void UndeclaredPropertyShouldWriteTypeName(object value, string typeName)
        {
            var property = new ODataProperty { Name = "UndeclaredProperty", Value = value };
            this.SerializeProperty(this.entityType, property).Should().Contain(string.Format("@odata.type\":\"#{0}\"", typeName));
        }
        #endregion Default type name serialization behavior for primitive values

        #region SerializationTypeNameAnnotation on primitive values
        [Fact]
        public void DeclaredPrimitivePropertyWithSerializationTypeNameAnnotationShouldWriteTypeNameFromAnnotation()
        {
            this.declaredProperty.ODataValue.TypeAnnotation = new ODataTypeAnnotation("MyArbitraryTypeName" );
            this.SerializeProperty(this.entityType, this.declaredProperty).Should().Contain("@odata.type\":\"#MyArbitraryTypeName\"");
        }

        [Fact]
        public void DeclaredPrimitivePropertyWithNullSerializationTypeNameAnnotationShouldNotWriteTypeName()
        {
            this.declaredProperty.ODataValue.TypeAnnotation = new ODataTypeAnnotation();
            this.SerializeProperty(this.entityType, this.declaredProperty).Should().NotContain("@odata.type");
        }

        [Fact]
        public void UndeclaredPrimitivePropertyWithSerializationTypeNameAnnotationShouldWriteTypeNameFromAnnotation()
        {
            this.undeclaredProperty.ODataValue.TypeAnnotation = new ODataTypeAnnotation("MyArbitraryTypeName");
            this.SerializeProperty(this.entityType, this.undeclaredProperty).Should().Contain("@odata.type\":\"#MyArbitraryTypeName\"");
        }

        [Fact]
        public void UndeclaredPrimitivePropertyWithNullSerializationTypeNameAnnotationShouldNotWriteTypeName()
        {
            this.undeclaredProperty.ODataValue.TypeAnnotation = new ODataTypeAnnotation();
            this.SerializeProperty(this.entityType, this.undeclaredProperty).Should().NotContain("@odata.type");
        }
        #endregion SerializationTypeNameAnnotation on primitive values

        #region float point infinity value tests
        [Fact]
        public void UndeclaredSingleTypePropertyWithInfinityValueShouldWriteInfWithCorrectTypeName()
        {
            this.SerializeProperty(this.entityType, new ODataProperty { Name = "UndeclaredSingleProperty", Value = Single.PositiveInfinity })
                .Should().Be("{\"UndeclaredSingleProperty@odata.type\":\"#Single\",\"UndeclaredSingleProperty\":\"INF\"}");
            this.SerializeProperty(this.entityType, new ODataProperty { Name = "UndeclaredSingleProperty", Value = Single.NegativeInfinity })
                .Should().Be("{\"UndeclaredSingleProperty@odata.type\":\"#Single\",\"UndeclaredSingleProperty\":\"-INF\"}");
        }

        [Fact]
        public void UndeclaredDoubleTypePropertyOfInfinityValueShouldWriteInfWithCorrectTypeName()
        {
            this.SerializeProperty(this.entityType, new ODataProperty { Name = "UndeclaredDoubleProperty", Value = Double.PositiveInfinity })
                .Should().Be("{\"UndeclaredDoubleProperty@odata.type\":\"#Double\",\"UndeclaredDoubleProperty\":\"INF\"}");
            this.SerializeProperty(this.entityType, new ODataProperty { Name = "UndeclaredDoubleProperty", Value = Double.NegativeInfinity })
                .Should().Be("{\"UndeclaredDoubleProperty@odata.type\":\"#Double\",\"UndeclaredDoubleProperty\":\"-INF\"}");
        }

        [Fact]
        public void DeclaredSingleTypePropertyWithInfinityValueShouldWriteInf()
        {
            this.SerializeProperty(this.entityType, new ODataProperty { Name = "DeclaredSingleProperty", Value = Single.PositiveInfinity })
                .Should().Be("{\"DeclaredSingleProperty\":\"INF\"}");
            this.SerializeProperty(this.entityType, new ODataProperty { Name = "DeclaredSingleProperty", Value = Single.NegativeInfinity })
                .Should().Be("{\"DeclaredSingleProperty\":\"-INF\"}");
        }

        [Fact]
        public void DeclaredDoubleTypePropertyWithInfinityValueShouldWriteInf()
        {
            this.SerializeProperty(this.entityType, new ODataProperty { Name = "DeclaredDoubleProperty", Value = Double.PositiveInfinity })
                .Should().Be("{\"DeclaredDoubleProperty\":\"INF\"}");
            this.SerializeProperty(this.entityType, new ODataProperty { Name = "DeclaredDoubleProperty", Value = Double.NegativeInfinity })
                .Should().Be("{\"DeclaredDoubleProperty\":\"-INF\"}");
        }
        #endregion float point infinity value tests

        #region Serializing type definition properties

        [Fact]
        public void WritingTypeDefinitionOfInt32PropertyShouldWorkJsonLight()
        {
            this.SerializeProperty(this.entityType, this.declaredPropertyMyInt32).Should().Contain("\"MyInt32Property\":12345");
        }

        [Fact]
        public void WritingTypeDefinitionOfStringPropertyShouldWorkJsonLight()
        {
            this.SerializeProperty(this.entityType, this.declaredPropertyMyString).Should().Contain("\"MyStringProperty\":\"abcde\"");
        }

        #endregion

        #region Serializing TimeOfDay/Date properties
        [Fact]
        public void WritingTimeOfDayPropertyShouldWorkJsonLight()
        {
            this.SerializeProperty(this.entityType, this.declaredPropertyTimeOfDay).Should().Contain("\"TimeOfDayProperty\":\"01:30:05.1230000\"");
        }

        [Fact]
        public void WritingDatePropertyShouldWorkJsonLight()
        {
            this.SerializeProperty(this.entityType, this.declaredPropertyDate).Should().Contain("\"DateProperty\":\"2014-09-17\"");
        }
        #endregion

        #region Serializing regular properties

        [Fact]
        public void WritingPropertyInEntryShouldWriteInstanceAnnotation()
        {
            this.SerializeProperty(null, this.declaredPropertyCountryRegionWithInstanceAnnotation).Should().Contain("\"CountryRegion@Is.ReadOnly\":true");
        }

        [Fact]
        public void WritingPropertyInEntryShouldWriteInstanceAnnotations()
        {
            this.SerializeProperty(null, this.declaredPropertyMyInt32WithInstanceAnnotations).Should().Contain("\"MyInt32Property@Is.AutoComputable\":true,\"MyInt32Property@Is.ReadOnly\":false");
        }

        #endregion

        #region Serializing declared and dynamic properties in open ComplexType
        [Fact]
        public void WritingDeclaredPropertyInOpenComplexTypeShouldWorkJsonLight()
        {
            this.SerializeProperty(this.openAddressType, this.declaredPropertyCountryRegion).Should().Contain("\"CountryRegion\":\"China\"");
        }

        [Fact]
        public void WritingDynamicPropertyInOpenComplexTypeShouldWorkJsonLight()
        {
            this.SerializeProperty(this.openAddressType, this.undeclaredPropertyCity).Should().Contain("\"City\":\"Shanghai\"");
        }
        #endregion Serializing declared and dynamic properties in open ComplexType

        /// <summary>
        /// Serialize the given property as a non-top-level property in JSON Light.
        /// </summary>
        /// <param name="odataProperty">The property to serialize.</param>
        /// <returns>A string of JSON text, where the given ODataProperty has been serialized and wrapped in a JSON object.</returns>
        private string SerializeProperty(IEdmStructuredType owningType, ODataProperty odataProperty)
        {
            MemoryStream outputStream = new MemoryStream();
            ODataJsonLightOutputContext jsonLightOutputContext = this.CreateJsonLightOutputContext(outputStream);
            var serializer = new ODataJsonLightPropertySerializer(jsonLightOutputContext);

            jsonLightOutputContext.JsonWriter.StartObjectScope();
            serializer.WriteProperties(
                owningType,
                new[] { odataProperty },
                /*isComplexValue*/ false,
                new NullDuplicatePropertyNameChecker());
            jsonLightOutputContext.JsonWriter.EndObjectScope();

            jsonLightOutputContext.Flush();
            outputStream.Position = 0;
            string result = new StreamReader(outputStream).ReadToEnd();

            return result;
        }

        private ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream)
        {
            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            settings.SetServiceDocumentUri(new Uri("http://example.com/"));

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = true,
                IsAsync = false,
                Model = this.model
            };

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }
    }
}
