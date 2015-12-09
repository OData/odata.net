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
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightPropertySerializerTests
    {
        private IEdmModel model;
        private IEdmEntityType entityType;
        private ODataProperty declaredProperty;
        private ODataProperty undeclaredProperty;
        private ODataProperty declaredGeometryProperty;
        private EdmComplexType addressType;
        private EdmComplexType derivedAddressType;
        private EdmComplexType openAddressType;
        private EdmTypeDefinition myInt32;
        private EdmTypeDefinition myString;
        private ODataProperty declaredPropertyCountryRegion;
        private ODataProperty declaredPropertyCountryRegionWithInstanceAnnotation;
        private ODataProperty undeclaredPropertyCity;
        private ODataProperty declaredPropertyHomeAddress;
        private ODataProperty declaredPropertyAddress;
        private ODataProperty declaredPropertyAddressWithInstanceAnnotation;
        private ODataProperty declaredPropertyHomeAddressWithInstanceAnnotations;
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

            this.model = TestUtils.WrapReferencedModelsToMainModel(edmModel);
            this.entityType = edmEntityType;
            this.declaredProperty = new ODataProperty { Name = "DeclaredProperty", Value = Guid.Empty };
            this.undeclaredProperty = new ODataProperty { Name = "UndeclaredProperty", Value = DateTimeOffset.MinValue };
            this.declaredGeometryProperty = new ODataProperty { Name = "DeclaredGeometryProperty", Value = GeometryPoint.Create(0.0, 0.0) };
            this.declaredPropertyTimeOfDay = new ODataProperty { Name = "TimeOfDayProperty", Value = new TimeOfDay(1, 30, 5, 123) };
            this.declaredPropertyDate = new ODataProperty { Name = "DateProperty", Value = new Date(2014, 9, 17) };

            // Initialize derived ComplexType: Address and HomeAddress
            this.addressType = new EdmComplexType("TestNamespace", "Address", baseType: null, isAbstract: false, isOpen: false);
            this.addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            this.derivedAddressType = new EdmComplexType("TestNamespace", "HomeAddress", baseType: this.addressType, isAbstract: false, isOpen: false);
            this.derivedAddressType.AddStructuralProperty("FamilyName", EdmPrimitiveTypeKind.String);

            // Initialize open ComplexType: OpenAddress.
            this.openAddressType = new EdmComplexType("TestNamespace", "OpenAddress", baseType: null, isAbstract: false, isOpen: true);
            this.openAddressType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);

            edmModel.AddElement(this.addressType);
            edmModel.AddElement(this.derivedAddressType);
            edmModel.AddElement(this.openAddressType);

            this.declaredPropertyAddress = new ODataProperty() { Name = "AddressProperty", Value = new ODataComplexValue { TypeName = "TestNamespace.Address", Properties = new ODataProperty[] { new ODataProperty { Name = "City", Value = "Shanghai" } } } };
            this.declaredPropertyHomeAddress = new ODataProperty() { Name = "HomeAddressProperty", Value = new ODataComplexValue { TypeName = "TestNamespace.HomeAddress", Properties = new ODataProperty[] { new ODataProperty { Name = "FamilyName", Value = "Green" }, new ODataProperty { Name = "City", Value = "Shanghai" } } } };
            this.declaredPropertyAddressWithInstanceAnnotation = new ODataProperty()
            {
                Name = "AddressProperty",
                Value = new ODataComplexValue
                {
                    TypeName = "TestNamespace.Address",
                    Properties = new ODataProperty[]
                    {
                        new ODataProperty { Name = "City", Value = "Shanghai" }
                    },
                    InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(true))
                    }
                }
            };
            this.declaredPropertyHomeAddressWithInstanceAnnotations = new ODataProperty()
            {
                Name = "HomeAddressProperty",
                Value = new ODataComplexValue
                {
                    TypeName = "TestNamespace.HomeAddress",
                    Properties = new ODataProperty[]
                    {
                        new ODataProperty
                        {
                            Name = "FamilyName",
                            Value = "Green",
                            InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                            {
                                new ODataInstanceAnnotation("FamilyName.annotation", new ODataPrimitiveValue(true))
                            }
                        },
                        new ODataProperty
                        {
                            Name = "City",
                            Value = "Shanghai",
                            InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                            {
                                new ODataInstanceAnnotation("City.annotation1", new ODataPrimitiveValue(true)),
                                new ODataInstanceAnnotation("City.annotation2", new ODataPrimitiveValue(123))
                            }
                        }
                    },
                    InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Is.AutoComputable", new ODataPrimitiveValue(true)),
                        new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(false))
                    }
                }
            };
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

        #region Serializing top-level complextype properties
        [Fact]
        public void WritingTopLevelDerivedComplexTypePropertyWithJsonMiniShouldWriteTypeName()
        {
            this.SerializeProperty(null, this.declaredPropertyHomeAddress).Should().Contain("\"@odata.type\":\"#TestNamespace.HomeAddress\"");
        }
        [Fact]
        public void WritingTopLevelUnderivedComplexTypePropertyWithJsonMiniShouldNotWriteTypeName()
        {
            this.SerializeProperty(null, this.declaredPropertyAddress).Should().NotContain("\"@odata.type\":\"#TestNamespace.Address\"");
        }
        [Fact]
        public void WritingTopLevelComplexTypePropertyShouldWriteInstanceAnnotation()
        {
            this.SerializeProperty(null, this.declaredPropertyAddressWithInstanceAnnotation).Should().Contain("\"@Is.ReadOnly\":true");
        }
        [Fact]
        public void WritingTopLevelComplexTypePropertyShouldWriteInstanceAnnotations()
        {
            this.SerializeProperty(null, this.declaredPropertyHomeAddressWithInstanceAnnotations).Should().Contain("\"@Is.AutoComputable\":true,\"@Is.ReadOnly\":false");
        }
        #endregion Serializing top-level complextype properties

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
            this.declaredProperty.ODataValue.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "MyArbitraryTypeName" });
            this.SerializeProperty(this.entityType, this.declaredProperty).Should().Contain("@odata.type\":\"#MyArbitraryTypeName\"");
        }

        [Fact]
        public void DeclaredPrimitivePropertyWithNullSerializationTypeNameAnnotationShouldNotWriteTypeName()
        {
            this.declaredProperty.ODataValue.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = null });
            this.SerializeProperty(this.entityType, this.declaredProperty).Should().NotContain("@odata.type");
        }

        [Fact]
        public void UndeclaredPrimitivePropertyWithSerializationTypeNameAnnotationShouldWriteTypeNameFromAnnotation()
        {
            this.undeclaredProperty.ODataValue.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "MyArbitraryTypeName" });
            this.SerializeProperty(this.entityType, this.undeclaredProperty).Should().Contain("@odata.type\":\"#MyArbitraryTypeName\"");
        }

        [Fact]
        public void UndeclaredPrimitivePropertyWithNullSerializationTypeNameAnnotationShouldNotWriteTypeName()
        {
            this.undeclaredProperty.ODataValue.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = null });
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

        #region Serializing complex properties

        [Fact]
        public void WritingPropertyInComplexTypeShouldWriteInsatanceAnnotation()
        {
            this.SerializeProperty(null, this.declaredPropertyHomeAddressWithInstanceAnnotations).Should()
                .Contain("\"FamilyName@FamilyName.annotation\":true,\"FamilyName\":\"Green\"").And
                .Contain("\"City@City.annotation1\":true,\"City@City.annotation2\":123,\"City\":\"Shanghai\"");
        }

        #endregion

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
                new DuplicatePropertyNamesChecker(allowDuplicateProperties: true, isResponse: true),
                ProjectedPropertiesAnnotation.AllProjectedPropertiesInstance);
            jsonLightOutputContext.JsonWriter.EndObjectScope();

            jsonLightOutputContext.Flush();
            outputStream.Position = 0;
            string result = new StreamReader(outputStream).ReadToEnd();

            return result;
        }

        private ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            settings.SetServiceDocumentUri(new Uri("http://example.com/"));

            return new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                /*writingResponse*/ true,
                /*synchronous*/ true,
                this.model,
                /*urlResolver*/ null);
        }
    }
}
