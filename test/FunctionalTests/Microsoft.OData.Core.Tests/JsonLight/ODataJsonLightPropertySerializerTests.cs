//---------------------------------------------------------------------
// <copyright file="ODataJsonLightPropertySerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Microsoft.OData.JsonLight;
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
            edmEntityType.AddStructuralProperty("PrimitiveProperty", EdmPrimitiveTypeKind.PrimitiveType);

            // add derived type constraint property.
            var derivedTypeConstrictionProperty = edmEntityType.AddStructuralProperty("PrimitiveConstraintProperty", EdmPrimitiveTypeKind.PrimitiveType);
            var term = ValidationVocabularyModel.DerivedTypeConstraintTerm;
            IEdmStringConstantExpression stringConstant1 = new EdmStringConstant("Edm.Int32");
            IEdmStringConstantExpression stringConstant2 = new EdmStringConstant("Edm.Boolean");
            var collectionExpression = new EdmCollectionExpression(new[] { stringConstant1, stringConstant2 });
            EdmVocabularyAnnotation valueAnnotationOnProperty = new EdmVocabularyAnnotation(derivedTypeConstrictionProperty, term, collectionExpression);
            valueAnnotationOnProperty.SetSerializationLocation(edmModel, EdmVocabularyAnnotationSerializationLocation.Inline);
            edmModel.AddVocabularyAnnotation(valueAnnotationOnProperty);

            edmModel.AddElement(edmEntityType);

            // Initialize ComplexType: Address, HomeAddress, and OpenAddress
            this.addressType = new EdmComplexType("TestNamespace", "Address", baseType: null, isAbstract: false, isOpen: false);
            this.addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            this.derivedAddressType = new EdmComplexType("TestNamespace", "HomeAddress", baseType: this.addressType, isAbstract: false, isOpen: false);
            this.derivedAddressType.AddStructuralProperty("FamilyName", EdmPrimitiveTypeKind.String);

            this.openAddressType = new EdmComplexType("TestNamespace", "OpenAddress", baseType: null, isAbstract: false, isOpen: true);
            this.openAddressType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);

            edmModel.AddElement(this.addressType);
            edmModel.AddElement(this.derivedAddressType);
            edmModel.AddElement(this.openAddressType);

            this.model = TestUtils.WrapReferencedModelsToMainModel(edmModel);
            this.entityType = edmEntityType;
            this.declaredProperty = new ODataProperty { Name = "DeclaredProperty", Value = Guid.Empty };
            this.undeclaredProperty = new ODataProperty { Name = "UndeclaredProperty", Value = DateTimeOffset.MinValue };
            this.declaredGeometryProperty = new ODataProperty { Name = "DeclaredGeometryProperty", Value = GeometryPoint.Create(0.0, 0.0) };
            this.declaredPropertyTimeOfDay = new ODataProperty { Name = "TimeOfDayProperty", Value = new TimeOfDay(1, 30, 5, 123) };
            this.declaredPropertyDate = new ODataProperty { Name = "DateProperty", Value = new Date(2014, 9, 17) };

            this.declaredPropertyAddress = new ODataProperty()
            {
                Name = "AddressProperty",
                Value = new ODataResourceValue
                {
                    TypeName = "TestNamespace.Address",
                    Properties = new ODataProperty[] { new ODataProperty { Name = "City", Value = "Shanghai" } }
                }
            };
            this.declaredPropertyHomeAddress = new ODataProperty()
            {
                Name = "HomeAddressProperty",
                Value = new ODataResourceValue
                {
                    TypeName = "TestNamespace.HomeAddress",
                    Properties = new ODataProperty[]
                    {
                        new ODataProperty { Name = "FamilyName", Value = "Green" },
                        new ODataProperty { Name = "City", Value = "Shanghai" }
                    }
                }
            };
            this.declaredPropertyAddressWithInstanceAnnotation = new ODataProperty()
            {
                Name = "AddressProperty",
                Value = new ODataResourceValue
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
                Value = new ODataResourceValue
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

        #region Serializing top level resource type properties

        [Fact]
        public void WritingTopLevelUnderivedResourceTypePropertyWithJsonMiniShouldNotWriteTypeName()
        {
            var result = this.SerializeProperty(null, this.declaredPropertyAddress);

            Assert.Equal("{\"AddressProperty\":{\"City\":\"Shanghai\"}}", result);
            Assert.DoesNotContain("@odata.type", result);
        }

        [Fact]
        public void WritingTopLevelDerivedResourceTypePropertyWithJsonMiniShouldWriteTypeName()
        {
            var result = this.SerializeProperty(null, this.declaredPropertyHomeAddress);

            Assert.Equal("{\"HomeAddressProperty\":{\"@odata.type\":\"#TestNamespace.HomeAddress\",\"FamilyName\":\"Green\",\"City\":\"Shanghai\"}}", result);
            Assert.Contains("\"@odata.type\":\"#TestNamespace.HomeAddress\"", result);
        }

        [Fact]
        public void WritingTopLevelResourceTypePropertyShouldWriteInstanceAnnotation()
        {
            var result = this.SerializeProperty(null, this.declaredPropertyAddressWithInstanceAnnotation);

            Assert.Equal("{\"AddressProperty\":{\"@Is.ReadOnly\":true,\"City\":\"Shanghai\"}}", result);
            Assert.Contains("\"@Is.ReadOnly\":true", result);
        }

        [Fact]
        public void WritingTopLevelResourceTypePropertyShouldWriteInstanceAnnotations()
        {
            var result = this.SerializeProperty(null, this.declaredPropertyHomeAddressWithInstanceAnnotations);

            Assert.Equal("{\"HomeAddressProperty\":{\"@odata.type\":\"#TestNamespace.HomeAddress\",\"@Is.AutoComputable\":true,\"@Is.ReadOnly\":false,\"FamilyName@FamilyName.annotation\":true,\"FamilyName\":\"Green\",\"City@City.annotation1\":true,\"City@City.annotation2\":123,\"City\":\"Shanghai\"}}", result);
            Assert.Contains("\"@Is.AutoComputable\":true,\"@Is.ReadOnly\":false", result);
        }

        [Fact]
        public void WritingTopLevelCollectionResourceTypePropertyWorks()
        {
            var addresses = new ODataProperty()
            {
                Name = "Addresses",
                Value = new ODataCollectionValue
                {
                    TypeName = "Collection(TestNamespace.Address)",
                    Items = new[]
                    {
                        new ODataResourceValue // underived type
                        {
                            TypeName = "TestNamespace.Address", Properties = new ODataProperty[] { new ODataProperty { Name = "City", Value = "Shanghai" } }
                        },
                        new ODataResourceValue
                        {
                            TypeName = "TestNamespace.HomeAddress", // derived type
                            Properties = new ODataProperty[]
                            {
                                new ODataProperty { Name = "FamilyName", Value = "Green" },
                                new ODataProperty { Name = "City", Value = "Shanghai" }
                            }
                        }
                    }
                }
            };

            var result = this.SerializeProperty(null, addresses);

            Assert.Equal("{\"Addresses\":[{\"City\":\"Shanghai\"},{\"@odata.type\":\"#TestNamespace.HomeAddress\",\"FamilyName\":\"Green\",\"City\":\"Shanghai\"}]}", result);
            Assert.Contains("\"},{\"@odata.type\":\"#TestNamespace.HomeAddress\"", result);
        }

        #endregion Serializing top level resource type properties

        #region Default type name serialization behavior for primitive values
        [Fact]
        public void DeclaredPrimitivePropertyWithNoSerializationTypeNameAnnotationShouldNotWriteTypeName()
        {
            Assert.DoesNotContain("@odata.type", this.SerializeProperty(this.entityType, this.declaredProperty));
        }

        [Fact]
        public void DeclaredPrimitivePropertyOfDerivedTypeShouldWriteTypeNameFromValue()
        {
            // If the type of the actual value is more derived than the property type declared in metadata, we write out the more derived type name.
            Assert.Contains("@odata.type\":\"#GeometryPoint\"", this.SerializeProperty(this.entityType, this.declaredGeometryProperty));
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
            Assert.DoesNotContain("@odata.type", this.SerializeProperty(this.entityType, property));
        }

        private void UndeclaredPropertyShouldWriteTypeName(object value, string typeName)
        {
            var property = new ODataProperty { Name = "UndeclaredProperty", Value = value };
            Assert.Contains(string.Format("@odata.type\":\"#{0}\"", typeName), this.SerializeProperty(this.entityType, property));
        }
        #endregion Default type name serialization behavior for primitive values

        #region SerializationTypeNameAnnotation on primitive values
        [Fact]
        public void DeclaredPrimitivePropertyWithSerializationTypeNameAnnotationShouldWriteTypeNameFromAnnotation()
        {
            this.declaredProperty.ODataValue.TypeAnnotation = new ODataTypeAnnotation("MyArbitraryTypeName" );
            Assert.Contains("@odata.type\":\"#MyArbitraryTypeName\"", this.SerializeProperty(this.entityType, this.declaredProperty));
        }

        [Fact]
        public void DeclaredPrimitivePropertyWithNullSerializationTypeNameAnnotationShouldNotWriteTypeName()
        {
            this.declaredProperty.ODataValue.TypeAnnotation = new ODataTypeAnnotation();
            Assert.DoesNotContain("@odata.type", this.SerializeProperty(this.entityType, this.declaredProperty));
        }

        [Fact]
        public void UndeclaredPrimitivePropertyWithSerializationTypeNameAnnotationShouldWriteTypeNameFromAnnotation()
        {
            this.undeclaredProperty.ODataValue.TypeAnnotation = new ODataTypeAnnotation("MyArbitraryTypeName");
            Assert.Contains("@odata.type\":\"#MyArbitraryTypeName\"", this.SerializeProperty(this.entityType, this.undeclaredProperty));
        }

        [Fact]
        public void UndeclaredPrimitivePropertyWithNullSerializationTypeNameAnnotationShouldNotWriteTypeName()
        {
            this.undeclaredProperty.ODataValue.TypeAnnotation = new ODataTypeAnnotation();
            Assert.DoesNotContain("@odata.type", this.SerializeProperty(this.entityType, this.undeclaredProperty));
        }
        #endregion SerializationTypeNameAnnotation on primitive values

        #region float point infinity value tests
        [Fact]
        public void UndeclaredSingleTypePropertyWithInfinityValueShouldWriteInfWithCorrectTypeName()
        {
            Assert.Equal("{\"UndeclaredSingleProperty@odata.type\":\"#Single\",\"UndeclaredSingleProperty\":\"INF\"}",
                this.SerializeProperty(this.entityType, new ODataProperty { Name = "UndeclaredSingleProperty", Value = Single.PositiveInfinity }));

            Assert.Equal("{\"UndeclaredSingleProperty@odata.type\":\"#Single\",\"UndeclaredSingleProperty\":\"-INF\"}",
                this.SerializeProperty(this.entityType, new ODataProperty { Name = "UndeclaredSingleProperty", Value = Single.NegativeInfinity }));
        }

        [Fact]
        public void UndeclaredDoubleTypePropertyOfInfinityValueShouldWriteInfWithCorrectTypeName()
        {
            Assert.Equal("{\"UndeclaredDoubleProperty@odata.type\":\"#Double\",\"UndeclaredDoubleProperty\":\"INF\"}",
                this.SerializeProperty(this.entityType, new ODataProperty { Name = "UndeclaredDoubleProperty", Value = Double.PositiveInfinity }));

            Assert.Equal("{\"UndeclaredDoubleProperty@odata.type\":\"#Double\",\"UndeclaredDoubleProperty\":\"-INF\"}",
                this.SerializeProperty(this.entityType, new ODataProperty { Name = "UndeclaredDoubleProperty", Value = Double.NegativeInfinity }));
        }

        [Fact]
        public void DeclaredSingleTypePropertyWithInfinityValueShouldWriteInf()
        {
            Assert.Equal("{\"DeclaredSingleProperty\":\"INF\"}",
                this.SerializeProperty(this.entityType, new ODataProperty { Name = "DeclaredSingleProperty", Value = Single.PositiveInfinity }));

            Assert.Equal("{\"DeclaredSingleProperty\":\"-INF\"}",
                this.SerializeProperty(this.entityType, new ODataProperty { Name = "DeclaredSingleProperty", Value = Single.NegativeInfinity }));
        }

        [Fact]
        public void DeclaredDoubleTypePropertyWithInfinityValueShouldWriteInf()
        {
            Assert.Equal("{\"DeclaredDoubleProperty\":\"INF\"}",
                this.SerializeProperty(this.entityType, new ODataProperty { Name = "DeclaredDoubleProperty", Value = Double.PositiveInfinity }));

            Assert.Equal("{\"DeclaredDoubleProperty\":\"-INF\"}",
                this.SerializeProperty(this.entityType, new ODataProperty { Name = "DeclaredDoubleProperty", Value = Double.NegativeInfinity }));
        }
        #endregion float point infinity value tests

        #region Serializing type definition properties

        [Fact]
        public void WritingTypeDefinitionOfInt32PropertyShouldWorkJsonLight()
        {
            Assert.Contains("\"MyInt32Property\":12345", this.SerializeProperty(this.entityType, this.declaredPropertyMyInt32));
        }

        [Fact]
        public void WritingTypeDefinitionOfStringPropertyShouldWorkJsonLight()
        {
            Assert.Contains("\"MyStringProperty\":\"abcde\"", this.SerializeProperty(this.entityType, this.declaredPropertyMyString));
        }

        #endregion

        #region Serializing TimeOfDay/Date properties
        [Fact]
        public void WritingTimeOfDayPropertyShouldWorkJsonLight()
        {
            Assert.Contains("\"TimeOfDayProperty\":\"01:30:05.1230000\"", this.SerializeProperty(this.entityType, this.declaredPropertyTimeOfDay));
        }

        [Fact]
        public void WritingDatePropertyShouldWorkJsonLight()
        {
            Assert.Contains("\"DateProperty\":\"2014-09-17\"", this.SerializeProperty(this.entityType, this.declaredPropertyDate));
        }
        #endregion

        public static IEnumerable<object[]> PrimitiveData => new List<object[]>
        {
            new object[] { 42,                     "{\"PrimitiveProperty@odata.type\":\"#Int32\",\"PrimitiveProperty\":42}" },
            new object[] { new Date(2018, 11, 28), "{\"PrimitiveProperty@odata.type\":\"#Date\",\"PrimitiveProperty\":\"2018-11-28\"}" },
            new object[] { 8.9,                    "{\"PrimitiveProperty@odata.type\":\"#Double\",\"PrimitiveProperty\":8.9}" },
            new object[] { true,                   "{\"PrimitiveProperty@odata.type\":\"#Boolean\",\"PrimitiveProperty\":true}" }
        };

        [Theory]
        [MemberData(nameof(PrimitiveData))]
        public void WritingEdmPrimitiveTypePropertyShouldWork(object value, string expect)
        {
            var primitiveTypeProperty = new ODataProperty { Name = "PrimitiveProperty", Value = value };
            string actual = this.SerializeProperty(this.entityType, primitiveTypeProperty);
            Assert.Equal(expect, actual);
        }

        [Theory]
        [InlineData(42, "{\"PrimitiveConstraintProperty@odata.type\":\"#Int32\",\"PrimitiveConstraintProperty\":42}")]
        [InlineData(true, "{\"PrimitiveConstraintProperty@odata.type\":\"#Boolean\",\"PrimitiveConstraintProperty\":true}")]
        public void WritingEdmPrimitiveConstraintPropertyShouldWorkWithCorrectInputValue(object value, string expect)
        {
            var primitiveTypeProperty = new ODataProperty { Name = "PrimitiveConstraintProperty", Value = value };
            string actual = this.SerializeProperty(this.entityType, primitiveTypeProperty);
            Assert.Equal(expect, actual);
        }

        [Theory]
        [InlineData(8.9, "Edm.Double")]
        [InlineData("abc", "Edm.String")]
        public void WritingEdmPrimitiveConstraintPropertyShouldThrowWithNotAllowedValue(object value, string fullTypeName)
        {
            var primitiveTypeProperty = new ODataProperty { Name = "PrimitiveConstraintProperty", Value = value };
            Action action = () => this.SerializeProperty(this.entityType, primitiveTypeProperty);
            var exception = Assert.Throws<ODataException>(action);
            Assert.Equal(Strings.WriterValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint(fullTypeName, "property", "PrimitiveConstraintProperty"), exception.Message);
        }

        #region Serializing regular properties

        [Fact]
        public void WritingPropertyInEntryShouldWriteInstanceAnnotation()
        {
            Assert.Contains("\"CountryRegion@Is.ReadOnly\":true", this.SerializeProperty(null, this.declaredPropertyCountryRegionWithInstanceAnnotation));
        }

        [Fact]
        public void WritingPropertyInEntryShouldWriteInstanceAnnotations()
        {
            Assert.Contains("\"MyInt32Property@Is.AutoComputable\":true,\"MyInt32Property@Is.ReadOnly\":false", this.SerializeProperty(null, this.declaredPropertyMyInt32WithInstanceAnnotations));
        }

        #endregion

        #region Serializing declared and dynamic properties in open ComplexType
        [Fact]
        public void WritingDeclaredPropertyInOpenComplexTypeShouldWorkJsonLight()
        {
            Assert.Contains("\"CountryRegion\":\"China\"", this.SerializeProperty(this.openAddressType, this.declaredPropertyCountryRegion));
        }

        [Fact]
        public void WritingDynamicPropertyInOpenComplexTypeShouldWorkJsonLight()
        {
            Assert.Contains("\"City\":\"Shanghai\"", this.SerializeProperty(this.openAddressType, this.undeclaredPropertyCity));
        }
        #endregion Serializing declared and dynamic properties in open ComplexType

        [Fact]
        public void WritingPropertyShouldWriteResourceInstanceAnnotation()
        {
            var resourceValue = new ODataResourceValue
            {
                TypeName = "TestNamespace.Address",
                Properties = new[] { new ODataProperty { Name = "City", Value = "Redmond" } }
            };
            var property = new ODataProperty()
            {
                Name = "IntProp",
                Value = 12345,
            };
            property.InstanceAnnotations.Add(new ODataInstanceAnnotation("Resource.Annotation", resourceValue));

            var result = SerializeProperty(null, property);
            Assert.Equal("{\"IntProp@Resource.Annotation\":{\"@odata.type\":\"#TestNamespace.Address\",\"City\":\"Redmond\"},\"IntProp\":12345}", result);
        }

        [Fact]
        public void WritingPropertyShouldWriteCollectionResourceInstanceAnnotation()
        {
            var property = new ODataProperty()
            {
                Name = "IntProp",
                Value = 12345,
            };
            property.InstanceAnnotations.Add(new ODataInstanceAnnotation("Collection.Resource", new ODataCollectionValue
            {
                TypeName = "Collection(TestNamespace.Address)",
                Items = new[]
                {
                    new ODataResourceValue
                    {
                        TypeName = "TestNamespace.Address",
                        Properties = new[] { new ODataProperty { Name = "City", Value = "Redmond" } }
                    },
                    new ODataResourceValue
                    {
                        TypeName = "TestNamespace.HomeAddress", // derived type
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "FamilyName", Value = "Green" },
                            new ODataProperty { Name = "City", Value = "Shanghai" }
                        }
                    }
                }
            }));

            var result = SerializeProperty(null, property);
            Assert.Equal(
                "{" +
                   "\"Collection.Resource@odata.type\":\"#Collection(TestNamespace.Address)\"," +
                   "\"IntProp@Collection.Resource\":[" +
                     "{\"City\":\"Redmond\"}," +
                     "{\"@odata.type\":\"#TestNamespace.HomeAddress\",\"FamilyName\":\"Green\",\"City\":\"Shanghai\"}" +
                   "]," +
                   "\"IntProp\":12345" +
                "}",
                result);
        }

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
                new NullDuplicatePropertyNameChecker(),
                null);
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
