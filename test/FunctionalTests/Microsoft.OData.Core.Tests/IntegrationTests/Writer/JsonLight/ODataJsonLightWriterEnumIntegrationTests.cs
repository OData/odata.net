//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriterEnumIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Writer.JsonLight
{
    /// <summary>
    /// Writer may modify entry (like entry.MetadataBuilder = builder, looks like a problem), 
    /// so "Func&lt;ODataResource&gt; entryClone" is used to ensure always creating new entry for multiple testings within one test case.
    /// </summary>
    public class ODataJsonLightWriterEnumIntegrationTests
    {
        private readonly Uri serviceDocumentUri = new Uri("http://odata.org/test/");
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

        public ODataJsonLightWriterEnumIntegrationTests()
        {
            EdmModel tmpModel = new EdmModel();

            var enumType = new EdmEnumType("NS", "Color");
            var red = new EdmEnumMember(enumType, "Red", new EdmEnumMemberValue(1));
            enumType.AddMember(red);
            enumType.AddMember("Green", new EdmEnumMemberValue(2));
            enumType.AddMember("Blue", new EdmEnumMemberValue(3));
            tmpModel.AddElement(enumType);

            // enum with flags
            var enumFlagsType = new EdmEnumType("NS", "ColorFlags", isFlags: true);
            enumFlagsType.AddMember("Red", new EdmEnumMemberValue(1));
            enumFlagsType.AddMember("Green", new EdmEnumMemberValue(2));
            enumFlagsType.AddMember("Blue", new EdmEnumMemberValue(4));
            tmpModel.AddElement(enumFlagsType);

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
            tmpModel.AddElement(myComplexType);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "MyComplexType", new EdmComplexTypeReference(myComplexType, true)));

            // enum in derived complex type
            EdmComplexType myDerivedComplexType = new EdmComplexType("NS", "MyDerivedComplexType", myComplexType, false);
            myDerivedComplexType.AddProperty(new EdmStructuralProperty(myDerivedComplexType, "MyDerivedColorFlags", enumFlagsTypeReference));
            tmpModel.AddElement(myDerivedComplexType);

            // enum in collection type
            EdmCollectionType myCollectionType = new EdmCollectionType(enumFlagsTypeReference);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "MyCollectionType", new EdmCollectionTypeReference(myCollectionType)));

            tmpModel.AddElement(this.entityType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            tmpModel.AddElement(defaultContainer);

            this.entitySet = new EdmEntitySet(defaultContainer, "MySet", this.entityType);
            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", tmpModel);
        }

        #region Enum tests
        [Fact]
        public void FlagsEnumAsComplexProperty_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            Func<ODataItem[]> getItems = () => GetResourceWithEnum();

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: null, nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: null, nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: getItems(), expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsComplexProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            Func<ODataItem[]> getItems = () => GetResourceWithEnum();

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueFull));

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"MySet(12.3)\",\"@odata.editLink\":\"MySet(12.3)\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":\"Red\",\"Height\":98.6}}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: getItems(), expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsDerivedComplexProperty_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            Func<ODataItem[]> getItems = () => GetResourceWithEnum(true);

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags\":\"Red\"}}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: null, nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags\":\"Red\"}}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: null, nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyDerivedColorFlags\":\"Red\"}}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags\":\"Red\"}}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: getItems(), expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsDerivedComplexProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            Func<ODataItem[]> getItems = () => GetResourceWithEnum(true);

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueFull));

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags\":\"Red\"}}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"MySet(12.3)\",\"@odata.editLink\":\"MySet(12.3)\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyDerivedColorFlags\":\"Red\"}}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"MyColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyDerivedColorFlags\":\"Red\"}}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: getItems(), expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"Color\":\"Green\",\"MyComplexType\":{\"MyColorFlags\":\"Red\",\"Height\":98.6,\"MyDerivedColorFlags\":\"Red\"}}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: getItems(), expectedPayload: expectedPayload);
        }

        private ODataItem[] GetResourceWithEnum(bool derived = false)
        {
            ODataResource topResource = new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                {
                    new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3D)},
                    new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString())},
                }
            };

            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo
            {
                Name = "MyComplexType",
                IsCollection = false
            };

            var propertiesForNestedResource = new List<ODataProperty>()
            {
                new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename") },
                new ODataProperty { Name = "Height", Value = 98.6 }
            };

            ODataResource nestedResource = new ODataResource();
            if (derived)
            {
                propertiesForNestedResource.Add(new ODataProperty { Name = "MyDerivedColorFlags", Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename") });
                nestedResource.TypeName = "NS.MyDerivedComplexType";
                nestedResource.Properties = propertiesForNestedResource;
            }
            else
            {
                nestedResource.Properties = propertiesForNestedResource;
            }

            return new ODataItem[] { topResource, nestedComplexInfo, nestedResource };
        }

        [Fact]
        public void FlagsEnumAsCollectionElement_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                {
                    new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3D)},
                    new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString())},
                    new ODataProperty
                    {
                        Name = "MyCollectionType",
                        Value = new ODataCollectionValue { Items = new[] {  new ODataEnumValue(Color.Red.ToString(),"NS.EnumUndefinedTypename"), new ODataEnumValue(Color.Green.ToString(),"NS.EnumUndefinedTypename")} }
                    }
                }
            };

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: null, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: null, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsCollectionElement_StrAsValue_StrAsTypeName_FullMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                {
                    new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3D)},
                    new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString())},
                    new ODataProperty
                    {
                        Name = "MyCollectionType",
                        Value = new ODataCollectionValue { Items = new[] {  new ODataEnumValue(Color.Red.ToString(),"NS.EnumUndefinedTypename"), new ODataEnumValue(Color.Green.ToString(),"NS.EnumUndefinedTypename")} }
                    }
                }
            };

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueFull));

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"MySet(12.3)\",\"@odata.editLink\":\"MySet(12.3)\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"Color\":\"Green\",\"MyCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsOpenCollectionPropertyElement_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                {
                    new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3D)},
                    new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString())},
                    new ODataProperty
                    {
                        Name = "MyOpenCollectionType",
                        Value = new ODataCollectionValue { TypeName = "Collection(NS.ColorFlags)", Items = new[] {  new ODataEnumValue(Color.Red.ToString(),"NS.EnumUndefinedTypename"), new ODataEnumValue(Color.Green.ToString(),"NS.EnumUndefinedTypename")} }
                    }
                }
            };

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyOpenCollectionType@odata.type\":\"#Collection(NS.ColorFlags)\",\"MyOpenCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: null, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyOpenCollectionType@odata.type\":\"#Collection(NS.ColorFlags)\",\"MyOpenCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: null, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyOpenCollectionType@odata.type\":\"#Collection(NS.ColorFlags)\",\"MyOpenCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"Color\":\"Green\",\"MyOpenCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsOpenCollectionPropertyElement_StrAsValue_StrAsTypeName_FullMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                {
                    new ODataProperty{Name = "FloatId", Value = new ODataPrimitiveValue(12.3D)},
                    new ODataProperty{Name = "Color", Value = new ODataEnumValue(Color.Green.ToString())},
                    new ODataProperty
                    {
                        Name = "MyOpenCollectionType",
                        Value = new ODataCollectionValue { TypeName = "Collection(NS.ColorFlags)", Items = new[] {  new ODataEnumValue(Color.Red.ToString(),"NS.EnumUndefinedTypename"), new ODataEnumValue(Color.Green.ToString(),"NS.EnumUndefinedTypename")} }
                    }
                }
            };

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueFull));

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyOpenCollectionType@odata.type\":\"#Collection(NS.ColorFlags)\",\"MyOpenCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"MySet(12.3)\",\"@odata.editLink\":\"MySet(12.3)\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyOpenCollectionType@odata.type\":\"#Collection(NS.ColorFlags)\",\"MyOpenCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"Color\":\"Green\",\"MyOpenCollectionType@odata.type\":\"#Collection(NS.ColorFlags)\",\"MyOpenCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"Color\":\"Green\",\"MyOpenCollectionType\":[\"Red\",\"Green\"]}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void NoFlagsEnumAsEntityProperty_StrAsValue_NullAsTypeName_MinimalMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "Color",
                                Value = new ODataEnumValue(Color.Green.ToString())
                            }
                        }
            };

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal));

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"Color\":\"Green\"}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"Color\":\"Green\"}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"Color\":\"Green\"}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"Color\":\"Green\"}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_NullAsTypeName_MinimalMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue(ColorFlags.Green.ToString())
                            }
                        }
            };

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal));

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"ColorFlags\":\"Green\"}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"ColorFlags\":\"Green\"}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"ColorFlags\":\"Green\"}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"ColorFlags\":\"Green\"}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_IntAsValue_NullAsTypeName_MinimalMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue(((int)(ColorFlags.Green | ColorFlags.Red)).ToString(CultureInfo.InvariantCulture))
                            }
                        }
            };
            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal));

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"ColorFlags\":\"3\"}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"ColorFlags\":\"3\"}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"ColorFlags\":\"3\"}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"ColorFlags\":\"3\"}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_EmptyStrAsValue_NullAsTypeName_MinimalMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue("")
                            }
                        }
            };
            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal));

            // Model-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"ColorFlags\":\"\"}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // Model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"ColorFlags\":\"\"}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"ColorFlags\":\"\"}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"ColorFlags\":\"\"}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }
        #endregion

        #region metadata level - full, minimal, none

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "FloatId",
                                Value = new ODataPrimitiveValue(12.3D)
                            },
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue(ColorFlags.Green.ToString(), "Fully.Qualified.Namespace.ColorFlags")
                            }
                        }
            };

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueFull));

            // NoModel-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"ColorFlags\":\"Green\"}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyEntityType\",\"@odata.id\":\"MySet(12.3)\",\"@odata.editLink\":\"MySet(12.3)\",\"FloatId\":12.3,\"ColorFlags@odata.type\":\"#Fully.Qualified.Namespace.ColorFlags\",\"ColorFlags\":\"Green\"}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"ColorFlags@odata.type\":\"#Fully.Qualified.Namespace.ColorFlags\",\"ColorFlags\":\"Green\"}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"ColorFlags\":\"Green\"}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "FloatId",
                                Value = new ODataPrimitiveValue(12.3D)
                            },
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue(ColorFlags.Green.ToString(), "Fully.Qualified.Namespace.ColorFlags")
                            }
                        }
            };

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal));

            // NoModel-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"ColorFlags\":\"Green\"}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"ColorFlags\":\"Green\"}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"ColorFlags@odata.type\":\"#Fully.Qualified.Namespace.ColorFlags\",\"ColorFlags\":\"Green\"}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"ColorFlags\":\"Green\"}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_StrAsValue_StrAsTypeName_NoMetadata()
        {
            Func<ODataResource> entryClone = () => new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "FloatId",
                                Value = new ODataPrimitiveValue(12.3D)
                            },
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = new ODataEnumValue(ColorFlags.Green.ToString(), "Fully.Qualified.Namespace.ColorFlags")
                            }
                        }
            };

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueNone));

            // NoModel-request (request: forced MinimalMetadata)
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"FloatId\":12.3,\"ColorFlags\":\"Green\"}";
            this.WriteMinimalRequestWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // model-reseponse
            expectedPayload = expectedPayload = "{\"FloatId\":12.3,\"ColorFlags\":\"Green\"}";
            this.WriteResponseWithModelAndValidatePayload(mediaType: mediaType, nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-request (request: forced MinimalMetadata)
            expectedPayload = "{\"@odata.type\":\"#NS.MyEntityType\",\"FloatId\":12.3,\"ColorFlags@odata.type\":\"#Fully.Qualified.Namespace.ColorFlags\",\"ColorFlags\":\"Green\"}";
            this.WriteMinimalMetadataRequestWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);

            // NoModel-response (using NoMetadata)
            expectedPayload = "{\"FloatId\":12.3,\"ColorFlags\":\"Green\"}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite: new[] { entryClone() }, expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_NullAsValue_NonNullable_WithModelMinimalMetadata_NullError()
        {
            var entry = new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                {
                    new ODataProperty { Name = "ColorFlags", Value = null }
                }
            };

            var nestedItemToWrite = new ODataItem[] { entry };
            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal));
            string fullName = this.entityType.FindProperty("ColorFlags").Type.FullName();

            // with model: write response
            Action action = () => { this.WriteResponseWithModelAndValidatePayload(mediaType, nestedItemToWrite, "no_expectedPayload", true); };
            action.Throws<ODataException>(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue("ColorFlags", fullName));

            // with model: write request
            action = () => { this.WriteMinimalRequestWithModelAndValidatePayload(mediaType, nestedItemToWrite, "no_expectedPayload", true); };
            action.Throws<ODataException>(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue("ColorFlags", fullName));
        }

        [Fact]
        public void FlagsEnumAsEntityProperty_NullAsValue_NonNullable_NoModelNoMetadata_NoError()
        {
            var entry = new ODataResource
            {
                TypeName = "NS.MyEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "FloatId",
                                Value = new ODataPrimitiveValue(12.3D)
                            },
                            new ODataProperty
                            {
                                Name = "ColorFlags",
                                Value = null
                            }
                        }
            };
            var nestedItemToWrite = new ODataItem[] { entry };
            const string expectedPayload = "{\"FloatId\":12.3,\"ColorFlags\":null}";
            this.WriteNoMetadataResponseWithoutModelAndValidatePayload(nestedItemToWrite, expectedPayload);
        }

        #endregion

        #region enum as top level property and value
        [Fact]
        public void FlagsEnumAsTopLevelProperty_StrAsValue_StrAsTypeName_FullMetadata()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "application/json;odata.metadata=full;",
                writerAction: (writer, withModel) =>
                {
                    ODataProperty property = new ODataProperty()
                    {
                        Name = "MyColorPropertyName",
                        Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename")
                    };
                    writer.WriteProperty(property);
                },
                expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.EnumUndefinedTypename\",\"@odata.type\":\"#NS.EnumUndefinedTypename\",\"value\":\"Red\"}"
            );
        }

        [Fact]
        public void FlagsEnumAsTopLevelProperty_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "application/json;odata.metadata=minimal;",
                writerAction: (writer, withModel) =>
                {
                    ODataProperty property = new ODataProperty()
                    {
                        Name = "MyColorPropertyName",
                        Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename")
                    };
                    writer.WriteProperty(property);
                },
                expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.EnumUndefinedTypename\",\"value\":\"Red\"}"
            );
        }

        [Fact]
        public void FlagsEnumAsTopLevelProperty_StrAsValue_StrAsTypeName_NoMetadata()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "application/json;odata.metadata=none;",
                writerAction: (writer, withModel) =>
                {
                    ODataProperty property = new ODataProperty()
                    {
                        Name = "MyColorPropertyName",
                        Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename")
                    };
                    writer.WriteProperty(property);
                },
                expectedPayload: "{\"value\":\"Red\"}"
            );
        }

        [Fact]
        public void FlagsEnumAsTopLevelValue_StrAsValue_StrAsTypeName_textplainContentType()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "text/plain", // can't be full/minimal/none metadata
                writerAction: (writer, withModel) =>
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
                writerAction: (writer, withModel) =>
                {
                    ODataEnumValue enumValue = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename");
                    writer.WriteValue(enumValue);
                },
                expectedPayload: Color.Red.ToString()
            );
        }

        [Fact]
        public void FlagsEnumAsComplexPropertyAsTopLevelValue_StrAsValue_StrAsTypeName_FullMetadata()
        {
            var resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename") },
                    new ODataProperty { Name = "Height", Value = 98.6 }
                },
                TypeName = "NS.MyComplexType"
            };

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueFull));

            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyComplexType\",\"@odata.type\":\"#NS.MyComplexType\",\"MyColorFlags@odata.type\":\"#NS.EnumUndefinedTypename\",\"MyColorFlags\":\"Red\",\"Height\":98.6}";
            this.WriteToMessageWriterAndVerifyPayload(
                contentType: "application/json;odata.metadata=full;",
                writerAction: (writer, withModel) =>
                {
                    ODataWriter odatawriter = writer.CreateODataResourceWriter() as ODataWriter;
                    if (!withModel)
                    {
                        resource.SerializationInfo = new ODataResourceSerializationInfo()
                        {
                            ExpectedTypeName = "NS.MyComplexType"
                        };
                    }
                    WriteNestedItems(new ODataItem[] { resource }, odatawriter);
                },
                expectedPayload: expectedPayload);
        }

        [Fact]
        public void FlagsEnumAsComplexPropertyAsTopLevelValue_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            var resource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "MyColorFlags", Value = new ODataEnumValue(Color.Red.ToString(), "NS.EnumUndefinedTypename") },
                    new ODataProperty { Name = "Height", Value = 98.6 }
                },
                TypeName = "NS.MyComplexType"
            };

            WriteToMessageWriterAndVerifyPayload(
                contentType: "application/json;odata.metadata=minimal;",
                writerAction: (writer, withModel) =>
                {
                    ODataWriter odatawriter = writer.CreateODataResourceWriter() as ODataWriter;
                    if (!withModel)
                    {
                        resource.SerializationInfo = new ODataResourceSerializationInfo()
                        {
                            ExpectedTypeName = "NS.MyComplexType"
                        };
                    }
                    WriteNestedItems(new ODataItem[] { resource }, odatawriter);
                },
                expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyComplexType\",\"MyColorFlags\":\"Red\",\"Height\":98.6}"
            );
        }

        [Fact]
        public void FlagsEnumAsCollectionItemAsTopLevelValue_StrAsValue_StrAsTypeName_FullMetadata()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "application/json;odata.metadata=full;",
                writerAction: (writer, withModel) =>
                {
                    ODataProperty property = new ODataProperty
                    {
                        Name = "MyCollectionTypeValue1",
                        Value = new ODataCollectionValue { Items = new[] { new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags"), new ODataEnumValue(ColorFlags.Green.ToString(), "NS.ColorFlags") }, TypeName = "Collection(NS.ColorFlags)" }
                    };
                    writer.WriteProperty(property);
                },
                expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.ColorFlags)\",\"@odata.type\":\"#Collection(NS.ColorFlags)\",\"value\":[\"Red\",\"Green\"]}"
            );
        }

        [Fact]
        public void FlagsEnumAsCollectionItemAsTopLevelValue_StrAsValue_StrAsTypeName_MinimalMetadata()
        {
            WriteToMessageWriterAndVerifyPayload(
                contentType: "application/json;odata.metadata=minimal;",
                writerAction: (writer, withModel) =>
                {
                    ODataProperty property = new ODataProperty
                    {
                        Name = "MyCollectionTypeValue1",
                        Value = new ODataCollectionValue { Items = new[] { new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags"), new ODataEnumValue(ColorFlags.Green.ToString(), "NS.ColorFlags") }, TypeName = "Collection(NS.ColorFlags)" }
                    };
                    writer.WriteProperty(property);
                },
                expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.ColorFlags)\",\"value\":[\"Red\",\"Green\"]}"
            );
        }

        [Fact]
        public void FlagsEnumAsCollectionItemAsTopLevelValue_StrAsValue_StrAsTypeName_MinimalMetadata_CollecionWriter()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            collectionStart.SetSerializationInfo(new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(NS.ColorFlags)" });
            ODataEnumValue[] items = new ODataEnumValue[]
            {
                new ODataEnumValue(ColorFlags.Red.ToString(), "NS.ColorFlags"),
                new ODataEnumValue(null, "NS.ColorFlags_Undefined"),
                new ODataEnumValue("Red,Green", "NS.ColorFlags"),
                new ODataEnumValue("Red|Green", "NS.ColorFlags"),
                new ODataEnumValue(ColorFlags.Green.ToString(), "NS.ColorFlags")
            };

            EdmEnumTypeReference enumRef = new EdmEnumTypeReference((IEdmEnumType)this.userModel.FindType("NS.ColorFlags"), true);
            WriteToMessageWriterAndVerifyPayload(
                contentType: "application/json;odata.metadata=minimal;",
                writerAction: (writer, withModel) =>
                {
                    ODataCollectionWriter collectionWriter = writer.CreateODataCollectionWriter(enumRef);
                    collectionWriter.WriteStart(collectionStart);
                    foreach (object item in items)
                    {
                        collectionWriter.WriteItem(item);
                    }

                    collectionWriter.WriteEnd();
                },
                expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.ColorFlags)\",\"value\":[\"Red\",null,\"Red,Green\",\"Red|Green\",\"Green\"]}"
            );
        }

        #endregion

        private void WriteToMessageWriterAndVerifyPayload(string contentType, Action<ODataMessageWriter, bool> writerAction, string expectedPayload)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings() { Version = ODataVersion.V4, EnableMessageStreamDisposal = false };
            settings.SetContentType(contentType, "utf-8");
            settings.SetServiceDocumentUri(this.serviceDocumentUri);
            // with model
            {
                MemoryStream stream = new MemoryStream();
                IODataResponseMessage message = new InMemoryMessage() { Stream = stream };
                using (ODataMessageWriter writer = new ODataMessageWriter(message, settings, this.userModel))
                {
                    writerAction(writer, true);
                }

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                Assert.Equal(expectedPayload, payload);
            }

            // without model
            {
                MemoryStream stream = new MemoryStream();
                IODataResponseMessage message = new InMemoryMessage() { Stream = stream };
                using (ODataMessageWriter writer = new ODataMessageWriter(message, settings))
                {
                    writerAction(writer, false);
                }

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                Assert.Equal(expectedPayload, payload);
            }
        }

        private void WriteMinimalRequestWithModelAndValidatePayload(ODataMediaType mediaType, ODataItem[] nestedItemToWrite, string expectedPayload, bool setMetadataDocumentUri = true)
        {
            // with model
            // write request (JsonLightMetadataLevel.Create method will internally use MinimalMetadata for writing request)
            var stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, mediaType, false, this.userModel, setMetadataDocumentUri ? this.serviceDocumentUri : null);
            var writer = new ODataJsonLightWriter(outputContext, this.entitySet, this.entityType, nestedItemToWrite[0] is ODataResourceSet);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteResponseWithModelAndValidatePayload(ODataMediaType mediaType, ODataItem[] nestedItemToWrite, string expectedPayload, bool setMetadataDocumentUri = true)
        {
            // with model
            // write response
            var stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, mediaType, true, this.userModel, setMetadataDocumentUri ? this.serviceDocumentUri : null);
            var writer = new ODataJsonLightWriter(outputContext, this.entitySet, this.entityType, nestedItemToWrite[0] is ODataResourceSet);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteMinimalMetadataRequestWithoutModelAndValidatePayload(ODataItem[] nestedItemToWrite, string expectedPayload, bool setMetadataDocumentUri = true)
        {
            // without model, write request (JsonLightMetadataLevel.Create method will internally use MinimalMetadata for writing request)
            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal));

            // 1. CreateEntityContainerElementContextUri(): no entitySetName --> no context uri is output.
            // 2. but odata.type will be output because of no model. JsonMinimalMetadataTypeNameOracle.GetEntryTypeNameForWriting method: // We only write entity type names in Json Light if it's more derived (different) from the expected type name.
            var stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, mediaType, false, null, setMetadataDocumentUri ? this.serviceDocumentUri : null);
            var writer = new ODataJsonLightWriter(outputContext, null, null, nestedItemToWrite[0] is ODataResourceSet);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteNoMetadataResponseWithoutModelAndValidatePayload(ODataItem[] nestedItemToWrite, string expectedPayload, bool setMetadataDocumentUri = true)
        {
            // without model, write response
            // (when entityType==null: nonemetadata or (serviceDocumentUri==null && writingResponse==false) -> avoid the below exception. pls refer to ODataContextUriBuilder method)
            // "When writing a JSON response, a user model must be specified and the entity set and entity type must be passed to the ODataMessageWriter.CreateResourceWriter method or the ODataFeedAndResourceSerializationInfo must be set on the ODataResource or ODataFeed that is being writen."
            // so here use nonemetadata:
            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueNone));
            var stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, mediaType, true, null, setMetadataDocumentUri ? this.serviceDocumentUri : null);
            var writer = new ODataJsonLightWriter(outputContext, null, null, nestedItemToWrite[0] is ODataResourceSet);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private static void WriteNestedItems(ODataItem[] nestedItemsToWrite, ODataWriter writer)
        {
            foreach (ODataItem itemToWrite in nestedItemsToWrite)
            {
                ODataResourceSet feedToWrite = itemToWrite as ODataResourceSet;
                if (feedToWrite != null)
                {
                    writer.WriteStart(feedToWrite);
                }
                else
                {
                    ODataResource entryToWrite = itemToWrite as ODataResource;
                    if (entryToWrite != null)
                    {
                        writer.WriteStart(entryToWrite);
                    }
                    else
                    {
                        writer.WriteStart((ODataNestedResourceInfo)itemToWrite);
                    }
                }
            }

            for (int count = 0; count < nestedItemsToWrite.Length; count++)
            {
                writer.WriteEnd();
            }
        }

        private static void ValidateWrittenPayload(MemoryStream stream, ODataJsonLightWriter writer, string expectedPayload)
        {
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            Assert.Equal(expectedPayload, payload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, ODataMediaType mediaType, bool writingResponse = true, IEdmModel userModel = null, Uri serviceDocumentUri = null)
        {
            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            if (serviceDocumentUri != null)
            {
                settings.SetServiceDocumentUri(serviceDocumentUri);
            }

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = mediaType ?? new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = false,
                Model = userModel ?? EdmCoreModel.Instance
            };

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }
    }
}
