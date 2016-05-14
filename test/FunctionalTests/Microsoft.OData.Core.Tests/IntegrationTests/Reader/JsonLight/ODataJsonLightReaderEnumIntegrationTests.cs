//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderEnumIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Reader.JsonLight
{
    public class ODataJsonLightReaderEnumIntegrationTests
    {
        private readonly Uri metadataDocumentUri = new Uri("http://odata.org/test/$metadata");
        private readonly IEdmModel userModel;
        private readonly EdmEntitySet entitySet;
        private readonly EdmEntityType entityType;

        public enum Color
        {
            Red = 1,
            Green = 2,
            Blue = 3,
        }

        [Flags]
        public enum ColorFlags
        {
            Red = 1,
            Green = 2,
            Blue = 4,
        }

        public ODataJsonLightReaderEnumIntegrationTests()
        {
            EdmModel tmpModel = new EdmModel();

            // enum without flags
            var enumType = new EdmEnumType("NS", "Color");
            var red = new EdmEnumMember(enumType, "Red", new EdmIntegerConstant(1));
            enumType.AddMember(red);
            enumType.AddMember("Green", new EdmIntegerConstant(2));
            enumType.AddMember("Blue", new EdmIntegerConstant(3));
            tmpModel.AddElement(enumType);

            // enum with flags
            var enumFlagsType = new EdmEnumType("NS", "ColorFlags", isFlags: true);
            enumFlagsType.AddMember("Red", new EdmIntegerConstant(1));
            enumFlagsType.AddMember("Green", new EdmIntegerConstant(2));
            enumFlagsType.AddMember("Blue", new EdmIntegerConstant(4));
            tmpModel.AddElement(enumFlagsType);

            this.entityType = new EdmEntityType("NS", "MyEntityType", isAbstract: false, isOpen: true, baseType: null);
            EdmStructuralProperty floatId = new EdmStructuralProperty(this.entityType, "FloatId", EdmCoreModel.Instance.GetSingle(false));
            this.entityType.AddKeys(floatId);
            this.entityType.AddProperty(floatId);
            var enumTypeReference = new EdmEnumTypeReference(enumType, true);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "Color", enumTypeReference));
            var enumFlagsTypeReference = new EdmEnumTypeReference(enumFlagsType, false);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "ColorFlags", enumFlagsTypeReference));

            // enum in complex type
            EdmComplexType myComplexType = new EdmComplexType("NS", "MyComplexType");
            myComplexType.AddProperty(new EdmStructuralProperty(myComplexType, "MyColorFlags", enumFlagsTypeReference));
            myComplexType.AddProperty(new EdmStructuralProperty(myComplexType, "Height", EdmCoreModel.Instance.GetDouble(false)));
            tmpModel.AddElement(myComplexType);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "MyComplexType", new EdmComplexTypeReference(myComplexType, true)));

            // enum in derived complex type
            EdmComplexType myDerivedComplexType = new EdmComplexType("NS", "MyDerivedComplexType", myComplexType, false);
            myDerivedComplexType.AddProperty(new EdmStructuralProperty(myDerivedComplexType, "MyDerivedColorFlags", new EdmEnumTypeReference(enumFlagsType, false)));
            tmpModel.AddElement(myDerivedComplexType);
            
            // enum in collection type
            EdmCollectionType myCollectionType = new EdmCollectionType(enumFlagsTypeReference);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "MyCollectionType", new EdmCollectionTypeReference(myCollectionType)));

            tmpModel.AddElement(this.entityType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            this.entitySet = new EdmEntitySet(defaultContainer, "MySet", this.entityType);
            defaultContainer.AddEntitySet(this.entitySet.Name, this.entityType);
            tmpModel.AddElement(defaultContainer);

            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", tmpModel);
        }

        #region enum in ComplexType CollectionType
        [Fact]
        public void FlagsEnumAsComplexProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"http://odata.org/test/MySet(12.3)\",\"@odata.editLink\":\"http://odata.org/test/MySet(12.3)\",\"@odata.readLink\":\"http://odata.org/test/MySet(12.3)\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags@odata.type\":\"#NS.ColorFlags\",\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyComplexType",
                            Value = new ODataComplexValue { TypeName ="NS.MyComplexType", Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags") }, new ODataProperty { Name = "Height", Value = 98.6 }} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=full", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsComplexProperty_StrAsValue_NoTypeName_MinimalMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyComplexType",
                            Value = new ODataComplexValue { TypeName ="NS.MyComplexType", Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags") }, new ODataProperty { Name = "Height", Value = 98.6 }} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsComplexProperty_NullValue_MinimalMetadata_Error()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":null,\"Height\":98.6}}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyComplexType",
                            Value = new ODataComplexValue { TypeName ="NS.MyComplexType", Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = null }, new ODataProperty { Name = "Height", Value = 98.6 }} }
                        }
                    }
            };

            Action action = () => this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
            string fullName = this.entityType.FindProperty("ColorFlags").Type.FullName();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ReaderValidationUtils_NullNamedValueForNonNullableType("MyColorFlags", fullName));
        }

        [Fact]
        public void FlagsEnumAsDerivedComplexProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"http://odata.org/test/MySet(12.3)\",\"@odata.editLink\":\"http://odata.org/test/MySet(12.3)\",\"@odata.readLink\":\"http://odata.org/test/MySet(12.3)\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags@odata.type\":\"#NS.ColorFlags\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags@odata.type\":\"#NS.ColorFlags\",\"MyDerivedColorFlags\":\"Red\"}}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyComplexType",
                            Value = new ODataComplexValue { TypeName ="NS.MyDerivedComplexType", Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags") }, new ODataProperty { Name = "Height", Value = 98.6 }, new ODataProperty { Name = "MyDerivedColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags") }} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=full", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsDerivedComplexProperty_StrAsValue_NoTypeName_MinimalMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags\":\"Red\"}}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyComplexType",
                            Value = new ODataComplexValue { TypeName ="NS.MyDerivedComplexType", Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags") }, new ODataProperty { Name = "Height", Value = 98.6 }, new ODataProperty { Name = "MyDerivedColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags") }} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsDerivedComplexProperty_NullValue_MinimalMetadata_Error()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags\":null,\"Height\":98.6,\"MyDerivedColorFlags\":null}}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyComplexType",
                            Value = new ODataComplexValue { TypeName ="NS.MyDerivedComplexType", Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = null }, new ODataProperty { Name = "Height", Value = 98.6 }, new ODataProperty { Name = "Height", Value = 98.6 }, new ODataProperty { Name = "MyDerivedColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags") }} }
                        }
                    }
            };

            Action action = () => this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
            string fullName = this.entityType.FindProperty("ColorFlags").Type.FullName();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ReaderValidationUtils_NullNamedValueForNonNullableType("MyColorFlags", fullName));
        }

        [Fact]
        public void FlagsEnumAsCollectionElement_StrAsValue_NoTypeName_FullMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"http://odata.org/test/MySet(12.3)\",\"@odata.editLink\":\"http://odata.org/test/MySet(12.3)\",\"@odata.readLink\":\"http://odata.org/test/MySet(12.3)\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyCollectionType",
                            Value = new ODataCollectionValue { TypeName = "Collection(NS.ColorFlags)", Items = new[] {  new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags"), new ODataEnumValue(ColorFlags.Green.ToString(),"NS.ColorFlags")} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=full", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsCollectionElement_StrAsValue_NoTypeName_MinimalMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyCollectionType",
                            Value = new ODataCollectionValue { TypeName = "Collection(NS.ColorFlags)", Items = new[] {  new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags"), new ODataEnumValue(ColorFlags.Green.ToString(),"NS.ColorFlags")} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsCollectionElement_EmptyStrAsValue_NoTypeName_MinimalMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"\",\"\"]}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), /*reader will get TypeName from model*/ "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyCollectionType",
                            Value = new ODataCollectionValue { TypeName = "Collection(NS.ColorFlags)", Items = new[] {  new ODataEnumValue("","NS.ColorFlags"), new ODataEnumValue("","NS.ColorFlags")} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
        }

        #endregion

        #region metadata level - full, minimal, none
        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.DefaultContainer.MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"http://odata.org/test/MySet(12.3)\",\"@odata.editLink\":\"http://odata.org/test/MySet(12.3)\",\"@odata.readLink\":\"http://odata.org/test/MySet(12.3)\",\"FloatId\":12.3,\"ColorFlags@odata.type\":\"NS.ColorFlags\",\"ColorFlags\":\"Green_undefined\"}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "ColorFlags", Value = new ODataEnumValue("Green_undefined", "NS.ColorFlags")},
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=full", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_NullAsTypeName_MinimalMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"ColorFlags\":\"Green_undefined\"}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "ColorFlags", Value = new ODataEnumValue("Green_undefined", "NS.ColorFlags")},
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_NullAsTypeName_NoMetadata()
        {
            const string payload = "{\"FloatId\":12.3,\"ColorFlags\":\"Green_undefined\"}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "ColorFlags", Value = new ODataEnumValue("Green_undefined", "NS.ColorFlags")},
                    }
            };

            // test payload as request
            ODataEntry entry = null;
            ReadReqeustEntryPayload(this.userModel, payload, "application/json;odata.metadata=none", this.entitySet, this.entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            entry.TypeName.Should().Be(expectedEntry.TypeName);
            TestUtils.AssertODataPropertiesAreEqual(expectedEntry.Properties, entry.Properties);

            // test payload as response
            // note: odata.metadata=none is not applicable to response message
        }
        #endregion

        #region enum in Entity
        [Fact]
        public void FlagsEnumAsEntityProperty_NullAsValue_NoNullable_MinimalMetadata_Error()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"ColorFlags\":null}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "ColorFlags", Value = null },
                    }
            };

            Action action = () => this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
            string fullName = this.entityType.FindProperty("ColorFlags").Type.FullName();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ReaderValidationUtils_NullNamedValueForNonNullableType("ColorFlags", fullName));
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_EmptyStrAsValue_NullAsTypeName_MinimalMetadata()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"ColorFlags\":''}";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3F)},       
                        new ODataProperty{Name = "ColorFlags", Value = new ODataEnumValue("", "NS.ColorFlags") },
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, "application/json;odata.metadata=minimal", expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_IntAsValue_NullAsTypeName_MinimalMetadata_Error()
        {
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"ColorFlags\":2}";
            ODataEntry entry = null;
            Action parse = () => ReadReqeustEntryPayload(this.userModel, payload, "application/json;odata.metadata=minimal", this.entitySet, this.entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            parse.ShouldThrow<ODataException>().Match(e => e.Message.StartsWith(Microsoft.OData.Core.Strings.JsonReaderExtensions_CannotReadValueAsString("2")));
        }
        #endregion

        #region enum as top level property (not value)
        [Fact]
        public void FlagsEnumAsTopLevelProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.Color\",\"@odata.type\":\"#NS.Color\",\"value\":\"Red\"}",
                contentType: "application/json;odata.metadata=full;",
                readerAction: (reader) =>
                {
                    ODataProperty expectedProperty = new ODataProperty()
                    {
                        Name = null,
                        Value = new ODataEnumValue(Color.Red.ToString(), "NS.Color")
                    };
                    ODataProperty property = reader.ReadProperty();
                    TestUtils.AssertODataPropertyAreEqual(expectedProperty, property);
                }
            );
        }

        [Fact]
        public void FlagsEnumAsTopLevelProperty_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.Color\",\"value\":\"Red\"}",
                contentType: "application/json;odata.metadata=minimal;",
                readerAction: (reader) =>
                {
                    ODataProperty expectedProperty = new ODataProperty()
                    {
                        Name = null,
                        Value = new ODataEnumValue(Color.Red.ToString(), "NS.Color")
                    };
                    ODataProperty property = reader.ReadProperty();
                    TestUtils.AssertODataPropertyAreEqual(expectedProperty, property);
                }
            );
        }

        [Fact]
        public void FlagsEnumAsComplexPropertyAsTopLevelValue_StrAsValue_StrAsTypeName_FullMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyComplexType\",\"@odata.type\":\"#NS.MyComplexType\",\"MyColorFlags@odata.type\":\"#NS.ColorFlags\",\"MyColorFlags\":\"Red\",\"Height\":98.6}",
                contentType: "application/json;odata.metadata=full;",
                readerAction: (reader) =>
                {
                    ODataProperty expectedProperty = new ODataProperty
                    {
                        Name = null,
                        Value = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags") }, new ODataProperty { Name = "Height", Value = 98.6 } }, TypeName = "NS.MyComplexType" }
                    };
                    ODataProperty property = reader.ReadProperty();
                    TestUtils.AssertODataPropertyAreEqual(expectedProperty, property);
                }
            );
        }

        [Fact]
        public void FlagsEnumAsComplexPropertyAsTopLevelValue_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyComplexType\",\"MyColorFlags\":\"Red\",\"Height\":98.6}",
                contentType: "application/json;odata.metadata=minimal;",
                readerAction: (reader) =>
                {
                    ODataProperty expectedProperty = new ODataProperty
                    {
                        Name = null,
                        Value = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags") }, new ODataProperty { Name = "Height", Value = 98.6 } }, TypeName = "NS.MyComplexType" }
                    };
                    ODataProperty property = reader.ReadProperty();
                    TestUtils.AssertODataPropertyAreEqual(expectedProperty, property);
                }
            );
        }

        [Fact]
        public void FlagsEnumAsDerivedComplexPropertyAsTopLevelValue_StrAsValue_StrAsTypeName_FullMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyComplexType\",\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags@odata.type\":\"#NS.ColorFlags\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags@odata.type\":\"#NS.ColorFlags\",\"MyDerivedColorFlags\":\"Red\"}",
                contentType: "application/json;odata.metadata=full;",
                readerAction: (reader) =>
                {
                    ODataProperty expectedProperty = new ODataProperty
                    {
                        Name = null,
                        Value = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags") }, new ODataProperty { Name = "Height", Value = 98.6 }, new ODataProperty { Name = "MyDerivedColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags") } }, TypeName = "NS.MyDerivedComplexType" }
                    };
                    ODataProperty property = reader.ReadProperty();
                    TestUtils.AssertODataPropertyAreEqual(expectedProperty, property);
                }
            );
        }

        [Fact]
        public void FlagsEnumAsDerivedComplexPropertyAsTopLevelValue_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyComplexType\",\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags\":\"Red\"}",
                contentType: "application/json;odata.metadata=minimal;",
                readerAction: (reader) =>
                {
                    ODataProperty expectedProperty = new ODataProperty
                    {
                        Name = null,
                        Value = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags") }, new ODataProperty { Name = "Height", Value = 98.6 }, new ODataProperty { Name = "MyDerivedColorFlags", Value = new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags") } }, TypeName = "NS.MyDerivedComplexType" }
                    };
                    ODataProperty property = reader.ReadProperty();
                    TestUtils.AssertODataPropertyAreEqual(expectedProperty, property);
                }
            );
        }

        [Fact]
        public void FlagsEnumAsCollectionItemAsTopLevelValue_StrAsValue_StrAsTypeName_FullMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.ColorFlags)\",\"@odata.type\":\"#Collection(NS.ColorFlags)\",\"value\":[\"Red\",\"Green\"]}",
                contentType: "application/json;odata.metadata=full;",
                readerAction: (reader) =>
                {
                    ODataProperty expectedProperty = new ODataProperty
                    {
                        Name = null,
                        Value = new ODataCollectionValue { Items = new[] { new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags"), new ODataEnumValue(ColorFlags.Green.ToString(), "NS.ColorFlags") }, TypeName = "Collection(NS.ColorFlags)" }
                    };
                    ODataProperty property = reader.ReadProperty();
                    TestUtils.AssertODataPropertyAreEqual(expectedProperty, property);
                }
            );
        }

        [Fact]
        public void FlagsEnumAsCollectionItemAsTopLevelValue_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.ColorFlags)\",\"value\":[\"Red\",\"Green\"]}",
                contentType: "application/json;odata.metadata=minimal;",
                readerAction: (reader) =>
                {
                    ODataProperty expectedProperty = new ODataProperty
                    {
                        Name = null,
                        Value = new ODataCollectionValue { Items = new[] { new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags"), new ODataEnumValue(ColorFlags.Green.ToString(), "NS.ColorFlags") }, TypeName = "Collection(NS.ColorFlags)" }
                    };
                    ODataProperty property = reader.ReadProperty();
                    TestUtils.AssertODataPropertyAreEqual(expectedProperty, property);
                }
            );
        }

        #endregion

        #region private methods
        public void ReadFromMessageReaderAndVerifyPayload(string payload, string contentType, Action<ODataMessageReader> readerAction)
        {
            var settings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };
            // with model
            {
                IODataResponseMessage message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
                message.SetHeader(ODataConstants.ContentTypeHeader, contentType);
                using (ODataMessageReader reader = new ODataMessageReader(message, settings, this.userModel))
                {
                    readerAction(reader);
                }
            }

            // without model should fail
            Action action = () =>
            {
                IODataResponseMessage message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
                message.SetHeader(ODataConstants.ContentTypeHeader, contentType);
                message.SetHeader(ODataConstants.ContentTypeHeader, contentType);
                using (ODataMessageReader reader = new ODataMessageReader(message, settings))
                {
                    readerAction(reader);
                }
            };
            action.ShouldThrow<ODataException>();
        }

        private void ReadEntryPayloadAndVerify(string payload, string contentType, ODataEntry expectedEntry)
        {
            // test payload as request
            ODataEntry entry = null;
            ReadReqeustEntryPayload(this.userModel, payload, contentType, this.entitySet, this.entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            entry.TypeName.Should().Be(expectedEntry.TypeName);
            TestUtils.AssertODataPropertiesAreEqual(expectedEntry.Properties, entry.Properties);

            // test payload as response
            entry = null;
            ReadResponseEntryPayload(this.userModel, payload, contentType, this.entitySet, this.entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            entry.TypeName.Should().Be(expectedEntry.TypeName);
            TestUtils.AssertODataPropertiesAreEqual(expectedEntry.Properties, entry.Properties);
        }

        private static void ReadReqeustEntryPayload(IEdmModel userModel, string payload, string contentType, EdmEntitySet entitySet, IEdmEntityType entityType, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };
            using (var msgReader = new ODataMessageReader((IODataRequestMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataEntryReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private static void ReadResponseEntryPayload(IEdmModel userModel, string payload, string contentType, EdmEntitySet entitySet, IEdmEntityType entityType, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataEntryReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }
        #endregion
    }
}
