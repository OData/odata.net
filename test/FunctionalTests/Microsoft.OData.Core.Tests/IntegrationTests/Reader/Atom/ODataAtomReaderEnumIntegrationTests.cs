//---------------------------------------------------------------------
// <copyright file="ODataAtomReaderEnumIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Reader.Atom
{
    public class ODataAtomReaderEnumIntegrationTests
    {
        private readonly Uri metadataDocumentUri = new Uri("http://odata.org/test/$metadata");
        private readonly EdmModel userModel;
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

        public ODataAtomReaderEnumIntegrationTests()
        {
            this.userModel = new EdmModel();

            // enum without flags
            var enumType = new EdmEnumType("NS", "Color");
            var red = new EdmEnumMember(enumType, "Red", new EdmIntegerConstant(1));
            enumType.AddMember(red);
            enumType.AddMember("Green", new EdmIntegerConstant(2));
            enumType.AddMember("Blue", new EdmIntegerConstant(3));
            this.userModel.AddElement(enumType);

            // enum with flags
            var enumFlagsType = new EdmEnumType("NS", "ColorFlags", isFlags: true);
            enumFlagsType.AddMember("Red", new EdmIntegerConstant(1));
            enumFlagsType.AddMember("Green", new EdmIntegerConstant(2));
            enumFlagsType.AddMember("Blue", new EdmIntegerConstant(4));
            this.userModel.AddElement(enumFlagsType);

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
            this.userModel.AddElement(myComplexType);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "MyComplexType", new EdmComplexTypeReference(myComplexType, true)));

            // enum in collection type
            EdmCollectionType myCollectionType = new EdmCollectionType(enumFlagsTypeReference);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "MyCollectionType", new EdmCollectionTypeReference(myCollectionType)));

            this.userModel.AddElement(this.entityType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.entitySet = new EdmEntitySet(defaultContainer, "MySet", this.entityType);
            defaultContainer.AddEntitySet(this.entitySet.Name, this.entityType);
            this.userModel.AddElement(defaultContainer);
        }

        #region enum in ComplexType CollectionType Entity
        [Fact]
        public void FlagsEnumAsComplexProperty_StrAsValue_StrAsTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:FloatId m:type=\"Single\">12.3</d:FloatId>" +
                            "<d:Color m:type=\"#NS.Color\">Green</d:Color>" +
                            "<d:MyComplexType m:type=\"#NS.MyComplexType\">" +
                                "<d:MyColorFlags m:type=\"#NS.ColorFlags\">Red</d:MyColorFlags>" +
                                "<d:Height m:type=\"Double\">98.6</d:Height>" +
                            "</d:MyComplexType>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
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
            this.ReadEntryPayloadAndVerify(payload, expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsComplexProperty_StrAsValue_NoTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:FloatId m:type=\"Single\">12.3</d:FloatId>" +
                            "<d:Color>Green</d:Color>" +
                            "<d:MyComplexType m:type=\"#NS.MyComplexType\">" +
                                "<d:MyColorFlags>Red</d:MyColorFlags>" +
                                "<d:Height>98.6</d:Height>" +
                            "</d:MyComplexType>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
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
            this.ReadEntryPayloadAndVerify(payload, expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsCollectionElement_StrAsValue_StrAsTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:FloatId m:type=\"Single\">12.3</d:FloatId>" +
                            "<d:Color m:type=\"#NS.Color\">Green</d:Color>" +
                            "<d:MyCollectionType m:type=\"#Collection(NS.ColorFlags)\">" +
                                "<m:element m:type=\"#NS.ColorFlags\">Red</m:element>" +
                                "<m:element m:type=\"#NS.ColorFlags\">Green</m:element>" +
                            "</d:MyCollectionType>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
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
                            Value = new ODataCollectionValue { TypeName="Collection(NS.ColorFlags)", Items = new[] {  new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags"), new ODataEnumValue(ColorFlags.Green.ToString(),"NS.ColorFlags")} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsCollectionElement_StrAsValue_NoTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:FloatId m:type=\"Single\">12.3</d:FloatId>" +
                            "<d:Color>Green</d:Color>" +
                            "<d:MyCollectionType m:type=\"#Collection(NS.ColorFlags)\">" +
                                "<m:element>Red</m:element>" +
                                "<m:element>Green</m:element>" +
                            "</d:MyCollectionType>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
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
                            Value = new ODataCollectionValue { TypeName="Collection(NS.ColorFlags)", Items = new[] {  new ODataEnumValue(ColorFlags.Red.ToString(),"NS.ColorFlags"), new ODataEnumValue(ColorFlags.Green.ToString(),"NS.ColorFlags")} }
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_StrAsTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags m:type=\"#NS.ColorFlags\">3</d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "ColorFlags",
                            Value = new ODataEnumValue(((int)(ColorFlags.Green | ColorFlags.Red)).ToString(CultureInfo.InvariantCulture), "NS.ColorFlags")
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_NoTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags>3</d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "ColorFlags",
                            Value = new ODataEnumValue(((int)(ColorFlags.Green | ColorFlags.Red)).ToString(CultureInfo.InvariantCulture), "NS.ColorFlags")
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_EmptyStrAsValue_NoTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags></d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "ColorFlags",
                            Value = new ODataEnumValue("", "NS.ColorFlags")
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, expectedEntry);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_NullAsValue_NoTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:Color m:null=\"true\" />" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            ODataEntry expectedEntry = new ODataEntry
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "Color",
                            Value = null
                        }
                    }
            };
            this.ReadEntryPayloadAndVerify(payload, expectedEntry);
        }
        #endregion

        #region error cases
        [Fact]
        public void FlagsEnumAsEntityProperty_IntAsValue_NoTypeName_Error()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:Color m:type=\"Int32\">16</d:Color>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            ODataEntry expectedEntry = null;
            Action parse = () => this.ReadEntryPayloadAndVerify(payload, expectedEntry);
            parse.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.ValidationUtils_IncorrectTypeKind("Edm.Int32", "Enum", "Primitive"));
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_NullAsValue_NoNullable_Error()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            string payload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags m:null=\"true\" />" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            ODataEntry expectedEntry = null;
            Action parse = () => this.ReadEntryPayloadAndVerify(payload, expectedEntry);
            string fullName = this.entityType.FindProperty("ColorFlags").Type.FullName();
            parse.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.ReaderValidationUtils_NullNamedValueForNonNullableType("ColorFlags", fullName));
        }
        #endregion

        #region enum as top level property (not value)
        [Fact]
        public void FlagsEnumAsTopLevelProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            ReadFromMessageReaderAndVerifyPayload(
                payload: "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<m:value xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:georss=\"http://www.georss.org/georss\" " +
                            "xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#NS.Color\" m:type=\"#NS.Color\" " +
                            "xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                            "Red" +
                        "</m:value>",
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

        // TODO: ATOM top level individual property testings
        // FlagsEnumAsComplexPropertyAsTopLevelValue_StrAsValue_StrAsTypeName
        // FlagsEnumAsCollectionItemAsTopLevelValue_StrAsValue_StrAsTypeName

        #endregion

        #region private methods
        public void ReadFromMessageReaderAndVerifyPayload(string payload, Action<ODataMessageReader> readerAction)
        {
            string contentType = "application/xml;";
            var settings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false, EnableAtom = true };
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
            action.ShouldThrow<Exception>();
        }

        private void ReadEntryPayloadAndVerify(string payload, ODataEntry expectedEntry)
        {
            // test payload as request
            ODataEntry entry = null;
            ReadReqeustEntryPayload(this.userModel, payload, this.entitySet, this.entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            entry.TypeName.Should().Be(expectedEntry.TypeName);
            TestUtils.AssertODataPropertiesAreEqual(expectedEntry.Properties, entry.Properties);

            // test payload as response
            entry = null;
            ReadResponseEntryPayload(this.userModel, payload, this.entitySet, this.entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            entry.TypeName.Should().Be(expectedEntry.TypeName);
            TestUtils.AssertODataPropertiesAreEqual(expectedEntry.Properties, entry.Properties);
        }

        private static void ReadReqeustEntryPayload(EdmModel userModel, string payload, EdmEntitySet entitySet, IEdmEntityType entityType, Action<ODataReader> action)
        {
            string contentType = "application/atom+xml;type=entry";
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false, EnableAtom = true };
            using (var msgReader = new ODataMessageReader((IODataRequestMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataEntryReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private static void ReadResponseEntryPayload(EdmModel userModel, string payload, EdmEntitySet entitySet, IEdmEntityType entityType, Action<ODataReader> action)
        {
            string contentType = "application/atom+xml;type=entry";
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false, EnableAtom = true };
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