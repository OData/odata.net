//---------------------------------------------------------------------
// <copyright file="ODataAtomWriterEnumIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Writer.Atom
{
    public class ODataAtomWriterEnumIntegrationTests
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/test/");
        private static readonly ODataFeedAndEntrySerializationInfo MySerializationInfo = new ODataFeedAndEntrySerializationInfo()
        {
            NavigationSourceEntityTypeName = "NS.MyEntityType",
            NavigationSourceName = "MySet",
            ExpectedTypeName = "NS.MyEntityType"
        };
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

        public ODataAtomWriterEnumIntegrationTests()
        {
            this.userModel = new EdmModel();

            var enumType = new EdmEnumType("NS", "Color");
            var red = new EdmEnumMember(enumType, "Red", new EdmIntegerConstant(1));
            enumType.AddMember(red);
            enumType.AddMember("Green", new EdmIntegerConstant(2));
            enumType.AddMember("Blue", new EdmIntegerConstant(3));

            // enum with flags
            var enumFlagsType = new EdmEnumType("NS", "ColorFlags", isFlags: true);
            enumFlagsType.AddMember("Red", new EdmIntegerConstant(1));
            enumFlagsType.AddMember("Green", new EdmIntegerConstant(2));
            enumFlagsType.AddMember("Blue", new EdmIntegerConstant(4));

            this.entityType = new EdmEntityType("NS", "MyEntityType", isAbstract: false, isOpen: true, baseType: null);
            this.entityType.AddKeys(new EdmStructuralProperty(this.entityType, "FloatId", EdmCoreModel.Instance.GetDouble(false)));
            var enumTypeReference = new EdmEnumTypeReference(enumType, true);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "Color", enumTypeReference));

            // add enum with flags
            var enumFlagsTypeReference = new EdmEnumTypeReference(enumFlagsType, false);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "ColorFlags", enumFlagsTypeReference));

            // enum in complex type
            EdmComplexType myComplexType = new EdmComplexType("NS", "MyComplexType");
            myComplexType.AddProperty(new EdmStructuralProperty(myComplexType, "MyColorFlags", enumFlagsTypeReference));
            myComplexType.AddProperty(new EdmStructuralProperty(myComplexType, "Height", EdmCoreModel.Instance.GetDouble(false)));
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "MyComplexType", new EdmComplexTypeReference(myComplexType, true)));

            // enum in collection type
            EdmCollectionType myCollectionType = new EdmCollectionType(enumFlagsTypeReference);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "MyCollectionType", new EdmCollectionTypeReference(myCollectionType)));

            this.userModel.AddElement(this.entityType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.entitySet = new EdmEntitySet(defaultContainer, "MySet", this.entityType);
        }

        #region Enum tests
        [Fact]
        public void FlagsEnumAsComplexProperty_StrAsValue_StrAsTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            Func<ODataEntry> entryClone = () =>
            {
                var tmp = new ODataEntry
                {
                    TypeName = "NS.MyEntityType",
                    Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3D)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyComplexType",
                            Value = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename") }, new ODataProperty { Name = "Height", Value = 98.6 }} }
                        }
                    },
                    SerializationInfo = MySerializationInfo
                };
                tmp.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
                return tmp;
            };

            // model-request
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:FloatId m:type=\"Double\">12.3</d:FloatId>" +
                            "<d:Color m:type=\"#NS.Color\">Green</d:Color>" +
                            "<d:MyComplexType m:type=\"#NS.MyComplexType\">" +
                                "<d:MyColorFlags m:type=\"#NS.EnumUndefinedTypename\">Red</d:MyColorFlags>" +
                                "<d:Height m:type=\"Double\">98.6</d:Height>" +
                            "</d:MyComplexType>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            this.WriteResponseWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request
            expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:FloatId m:type=\"Double\">12.3</d:FloatId>" +
                            "<d:Color m:type=\"#NS.Color\">Green</d:Color>" +
                            "<d:MyComplexType>" +
                                "<d:MyColorFlags m:type=\"#NS.EnumUndefinedTypename\">Red</d:MyColorFlags>" +
                                "<d:Height m:type=\"Double\">98.6</d:Height>" +
                            "</d:MyComplexType>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response
            this.WriteResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsCollectionElement_StrAsValue_StrAsTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            Func<ODataEntry> entryClone = () =>
            {
                var tmp = new ODataEntry
                {
                    TypeName = "NS.MyEntityType",
                    Properties = new[]
                    {
                        new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3D)},       
                        new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString(), "NS.Color")},
                        new ODataProperty
                        {
                            Name = "MyCollectionType",
                            Value = new ODataCollectionValue { Items = new[] {  new ODataEnumValue(Color.Red.ToString(),"NS.EnumUndefinedTypename"), new ODataEnumValue(Color.Green.ToString(),"NS.EnumUndefinedTypename")} }
                        }
                    },
                    SerializationInfo = MySerializationInfo
                };
                tmp.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
                return tmp;
            };

            // model-request
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:FloatId m:type=\"Double\">12.3</d:FloatId>" +
                            "<d:Color m:type=\"#NS.Color\">Green</d:Color>" +
                            "<d:MyCollectionType m:type=\"#Collection(NS.ColorFlags)\">" +
                                "<m:element m:type=\"#NS.EnumUndefinedTypename\">Red</m:element>" +
                                "<m:element m:type=\"#NS.EnumUndefinedTypename\">Green</m:element>" +
                            "</d:MyCollectionType>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            this.WriteResponseWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request
            expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:FloatId m:type=\"Double\">12.3</d:FloatId>" +
                            "<d:Color m:type=\"#NS.Color\">Green</d:Color>" +
                            "<d:MyCollectionType>" +
                                "<m:element m:type=\"#NS.EnumUndefinedTypename\">Red</m:element>" +
                                "<m:element m:type=\"#NS.EnumUndefinedTypename\">Green</m:element>" +
                            "</d:MyCollectionType>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response
            this.WriteResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_NullAsTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            Func<ODataEntry> entryClone = () =>
            {
                var tmp = new ODataEntry
                {
                    TypeName = "NS.MyEntityType",
                    Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue(((int)(ColorFlags.Green | ColorFlags.Red)).ToString(CultureInfo.InvariantCulture))
                            }
                        },
                    SerializationInfo = MySerializationInfo
                };
                tmp.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
                return tmp;
            };

            // model-request
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                    "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                        "<id />" +
                        "<title />" +
                        "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                        "<author><name /></author>" +
                        "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags m:type=\"#NS.ColorFlags\">3</d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            this.WriteResponseWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request
            expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                    "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                        "<id />" +
                        "<title />" +
                        "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                        "<author><name /></author>" +
                        "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags>3</d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response
            this.WriteResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_StrAsTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            Func<ODataEntry> entryClone = () =>
            {
                var tmp = new ODataEntry
                {
                    TypeName = "NS.MyEntityType",
                    Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue(
                                    (ColorFlags.Green | ColorFlags.Red).ToString(CultureInfo.InvariantCulture), 
                                    "NS.MyEnumTypeName") // type name
                            }
                        },
                    SerializationInfo = MySerializationInfo
                };
                tmp.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
                return tmp;
            };

            // model-request
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                    "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                        "<id />" +
                        "<title />" +
                        "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                        "<author><name /></author>" +
                        "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags m:type=\"#NS.MyEnumTypeName\">Red, Green</d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            this.WriteResponseWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request
            this.WriteRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response
            this.WriteResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_NullAsTypeName_WithTypeNameAnnotation()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            Func<ODataEntry> entryClone = () =>
            {
                var tmp = new ODataEntry
                {
                    TypeName = "NS.MyEntityType",
                    Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue(
                                    (ColorFlags.Green | ColorFlags.Red).ToString(CultureInfo.InvariantCulture), 
                                    null) // type name is set to null, to be overwritten by annotation
                            }
                        },
                    SerializationInfo = MySerializationInfo
                };

                // set SerializationTypeNameAnnotation:
                tmp.Properties.First().As<ODataProperty>().Value.As<ODataEnumValue>().SetAnnotation(
                    new SerializationTypeNameAnnotation() { TypeName = "NS.MyEnumAnnotationTypeName" });
                tmp.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
                return tmp;
            };

            // model-request
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                    "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                        "<id />" +
                        "<title />" +
                        "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                        "<author><name /></author>" +
                        "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags m:type=\"#NS.MyEnumAnnotationTypeName\">Red, Green</d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            this.WriteResponseWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request
            this.WriteRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response
            this.WriteResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_NullAsValue_ButNonNullable_GetNullError()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            Func<ODataEntry> entryClone = () =>
            {
                var tmp = new ODataEntry
                {
                    TypeName = "NS.MyEntityType",
                    Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = null
                            }
                        },
                    SerializationInfo = MySerializationInfo
                };

                tmp.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
                return tmp;
            };

            string fullName = this.entityType.FindProperty("ColorFlags").Type.FullName();

            // model-request
            Action action = () => this.WriteRequestWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: "");
            action.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue("ColorFlags", fullName));

            // model-reseponse
            action = () => this.WriteResponseWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: "");
            action.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue("ColorFlags", fullName));

            // NoModel-request 
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author><name /></author>" +
                    "<content type=\"application/xml\">" +
                    "<m:properties>" +
                        "<d:ColorFlags m:null=\"true\" />" +
                    "</m:properties>" +
                "</content>" +
            "</entry>";
            this.WriteRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response  
            this.WriteResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_NullAsValue_ForNonNullable()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            Func<ODataEntry> entryClone = () =>
            {
                var tmp = new ODataEntry
                {
                    TypeName = "NS.MyEntityType",
                    Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "Color",
                                Value = null
                            }
                        },
                    SerializationInfo = MySerializationInfo
                };

                tmp.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
                return tmp;
            };

            // model-request
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                    "<id />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author><name /></author>" +
                    "<content type=\"application/xml\">" +
                    "<m:properties>" +
                        "<d:Color m:null=\"true\" />" +
                    "</m:properties>" +
                "</content>" +
            "</entry>";
            this.WriteRequestWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            this.WriteResponseWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request 
            this.WriteRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response  
            this.WriteResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_EmptyStrAsValue_NullAsTypeName()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            Func<ODataEntry> entryClone = () =>
            {
                var tmp = new ODataEntry
                {
                    TypeName = "NS.MyEntityType",
                    Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue("")
                            }
                        },
                    SerializationInfo = MySerializationInfo
                };
                tmp.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
                return tmp;
            };

            // model-request
            string expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                    "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                        "<id />" +
                        "<title />" +
                        "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                        "<author><name /></author>" +
                        "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags m:type=\"#NS.ColorFlags\"></d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            this.WriteResponseWithModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request
            expectedPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" " +
                    "xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#MySet/$entity\">" +
                    "<category term=\"#NS.MyEntityType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" />" +
                        "<id />" +
                        "<title />" +
                        "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                        "<author><name /></author>" +
                        "<content type=\"application/xml\">" +
                        "<m:properties>" +
                            "<d:ColorFlags></d:ColorFlags>" +
                        "</m:properties>" +
                    "</content>" +
                "</entry>";
            this.WriteRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response
            this.WriteResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }
        #endregion

        #region enum as top level property and value
        [Fact]
        public void FlagsEnumAsTopLevelProperty_StrAsValue_StrAsTypeName()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "application/xml;",
                writerAction: (writer) =>
                {
                    ODataProperty property = new ODataProperty()
                    {
                        Name = "MyColorPropertyName",
                        Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename")
                    };
                    writer.WriteProperty(property);
                },
                expectedPayload: "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<m:value xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:georss=\"http://www.georss.org/georss\" " +
                            "xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://odata.org/test/$metadata#NS.EnumUndefinedTypename\" m:type=\"#NS.EnumUndefinedTypename\" " +
                            "xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                            "Red" +
                        "</m:value>"
            );
        }

        [Fact]
        public void FlagsEnumAsTopLevelValue_StrAsValue_StrAsTypeName_textplainContentType()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "text/plain", // can't be full/minimal/none metadata
                writerAction: (writer) =>
                {
                    ODataEnumValue enumValue = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename");
                    writer.WriteValue(enumValue);
                },
                expectedPayload: Color.Red.ToString()
            );
        }

        [Fact]
        public void FlagsEnumAsTopLevelValue_StrAsValue_StrAsTypeName_anyContentType()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "*/*",
                writerAction: (writer) =>
                {
                    ODataEnumValue enumValue = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename");
                    writer.WriteValue(enumValue);
                },
                expectedPayload: Color.Red.ToString()
            );
        }

        // TODO: ATOM top level individual property testings
        // FlagsEnumAsComplexPropertyAsTopLevelValue_StrAsValue_StrAsTypeName
        // FlagsEnumAsCollectionItemAsTopLevelValue_StrAsValue_StrAsTypeName

        #endregion

        #region private methods
        public void WriteToMessageWriterAndVerifyPayload(string contentType, Action<ODataMessageWriter> writerAction, string expectedPayload)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings() { Version = ODataVersion.V4, AutoComputePayloadMetadataInJson = true, DisableMessageStreamDisposal = true, EnableAtom = true};
            settings.SetContentType(contentType, "utf-8");
            settings.SetServiceDocumentUri(ServiceDocumentUri);
            // with model
            {
                MemoryStream stream = new MemoryStream();
                IODataResponseMessage message = new InMemoryMessage() { Stream = stream };
                using (ODataMessageWriter writer = new ODataMessageWriter(message, settings, this.userModel))
                {
                    writerAction(writer);
                }

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                payload.Should().Be(expectedPayload);
            }

            // without model
            {
                MemoryStream stream = new MemoryStream();
                IODataResponseMessage message = new InMemoryMessage() { Stream = stream };
                using (ODataMessageWriter writer = new ODataMessageWriter(message, settings))
                {
                    writerAction(writer);
                }

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                payload.Should().Be(expectedPayload);
            }
        }

        private static ODataAtomOutputContext CreateAtomOutputContext(MemoryStream stream, ODataMediaType mediaType, bool writingResponse = true, IEdmModel userModel = null)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, AutoComputePayloadMetadataInJson = true };
            settings.SetServiceDocumentUri(ServiceDocumentUri);
            return new ODataAtomOutputContext(
                ODataFormat.Atom,
                new NonDisposingStream(stream),
                Encoding.UTF8,
                settings,
                writingResponse,
                /*synchronous*/ true,
                userModel ?? EdmCoreModel.Instance,
                /*urlResolver*/ null);
        }

        private void WriteRequestWithModelAndValidatePayload(ODataItem[] nestedItemToWrite, string expectedPayload, bool setMetadataDocumentUri = true)
        {
            // with model, write request
            var stream = new MemoryStream();
            var outputContext = CreateAtomOutputContext(stream, null, false, this.userModel);
            var writer = new ODataAtomWriter(outputContext, this.entitySet, this.entityType, nestedItemToWrite[0] is ODataFeed);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteResponseWithModelAndValidatePayload(ODataItem[] nestedItemToWrite, string expectedPayload, bool setMetadataDocumentUri = true)
        {
            // with model, write response
            var stream = new MemoryStream();
            var outputContext = CreateAtomOutputContext(stream, null, true, this.userModel);
            var writer = new ODataAtomWriter(outputContext, this.entitySet, this.entityType, nestedItemToWrite[0] is ODataFeed);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteRequestWithoutModelAndValidatePayload(ODataItem[] nestedItemToWrite, string expectedPayload, bool setMetadataDocumentUri = true)
        {
            // without model, write request
            // 1. CreateEntityContainerElementContextUri(): no entitySetName --> no context uri is output.
            // 2. but odata.type will be output because of no model. JsonMinimalMetadataTypeNameOracle.GetEntryTypeNameForWriting method: // We only write entity type names in Json Light if it's more derived (different) from the expected type name.
            var stream = new MemoryStream();
            var outputContext = CreateAtomOutputContext(stream, null, false, null);
            var writer = new ODataAtomWriter(outputContext, null, null, nestedItemToWrite[0] is ODataFeed);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteResponseWithoutModelAndValidatePayload(ODataItem[] nestedItemToWrite, string expectedPayload, bool setMetadataDocumentUri = true)
        {
            // without model, write response
            var stream = new MemoryStream();
            var outputContext = CreateAtomOutputContext(stream, null, true, null);
            var writer = new ODataAtomWriter(outputContext, null, null, nestedItemToWrite[0] is ODataFeed);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private static void WriteNestedItems(ODataItem[] nestedItemsToWrite, ODataAtomWriter writer)
        {
            foreach (ODataItem itemToWrite in nestedItemsToWrite)
            {
                ODataFeed feedToWrite = itemToWrite as ODataFeed;
                if (feedToWrite != null)
                {
                    writer.WriteStart(feedToWrite);
                }
                else
                {
                    ODataEntry entryToWrite = itemToWrite as ODataEntry;
                    if (entryToWrite != null)
                    {
                        writer.WriteStart(entryToWrite);
                    }
                    else
                    {
                        writer.WriteStart((ODataNavigationLink)itemToWrite);
                    }
                }
            }

            for (int count = 0; count < nestedItemsToWrite.Length; count++)
            {
                writer.WriteEnd();
            }
        }

        private static void ValidateWrittenPayload(MemoryStream stream, ODataAtomWriter writer, string expectedPayload)
        {
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(expectedPayload);
        }
        #endregion
    }
}
