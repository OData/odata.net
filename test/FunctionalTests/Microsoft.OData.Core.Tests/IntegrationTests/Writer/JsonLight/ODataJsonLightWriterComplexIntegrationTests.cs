//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriterComplexIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;
using System.Collections.ObjectModel;
using Microsoft.Spatial;

namespace Microsoft.OData.Tests.IntegrationTests.Writer.JsonLight
{
    public class ODataJsonLightWriterComplexIntegrationTests
    {
        private IEdmModel model;
        private IEdmEntityType entityType;
        private EdmComplexType addressType;
        private EdmComplexType derivedAddressType;
        private EdmComplexType openAddressType;
        private ODataResource homeAddress;
        private ODataResource address;
        private ODataResource addressWithInstanceAnnotation;
        private ODataResource homeAddressWithInstanceAnnotations;


        public ODataJsonLightWriterComplexIntegrationTests()
        {

            // Initialize open EntityType: EntityType.
            EdmModel edmModel = new EdmModel();

            EdmEntityType edmEntityType = new EdmEntityType("TestNamespace", "EntityType", baseType: null, isAbstract: false, isOpen: true);
            edmEntityType.AddStructuralProperty("DeclaredProperty", EdmPrimitiveTypeKind.Guid);
            edmEntityType.AddStructuralProperty("DeclaredGeometryProperty", EdmPrimitiveTypeKind.Geometry);
            edmEntityType.AddStructuralProperty("DeclaredSingleProperty", EdmPrimitiveTypeKind.Single);
            edmEntityType.AddStructuralProperty("DeclaredDoubleProperty", EdmPrimitiveTypeKind.Double);
            edmEntityType.AddStructuralProperty("TimeOfDayProperty", EdmPrimitiveTypeKind.TimeOfDay);
            edmEntityType.AddStructuralProperty("DateProperty", EdmPrimitiveTypeKind.Date);

            edmModel.AddElement(edmEntityType);

            this.model = TestUtils.WrapReferencedModelsToMainModel(edmModel);
            this.entityType = edmEntityType;

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

            edmModel.AddEntityContainer("TestNamespace", "Container").AddEntitySet("entitySet", this.entityType);

            this.address = new ODataResource() { TypeName = "TestNamespace.Address", Properties = new ODataProperty[] { new ODataProperty { Name = "City", Value = "Shanghai" } } };
            this.homeAddress = new ODataResource { TypeName = "TestNamespace.HomeAddress", Properties = new ODataProperty[] { new ODataProperty { Name = "FamilyName", Value = "Green" }, new ODataProperty { Name = "City", Value = "Shanghai" } } };
            this.addressWithInstanceAnnotation = new ODataResource()
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
            };
            this.homeAddressWithInstanceAnnotations = new ODataResource()
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
            };
        }

        #region Serializing top-level complextype properties
        [Fact]
        public void WritingTopLevelDerivedComplexTypePropertyWithJsonMiniShouldWriteTypeName()
        {
            this.SerializeComplexResource(addressType, this.homeAddress).Should().Contain("\"@odata.type\":\"#TestNamespace.HomeAddress\"");
        }
        [Fact]
        public void WritingTopLevelUnderivedComplexTypePropertyWithJsonMiniShouldNotWriteTypeName()
        {
            this.SerializeComplexResource(null, this.address).Should().NotContain("\"@odata.type\":\"#TestNamespace.Address\"");
        }
        [Fact]
        public void WritingTopLevelComplexTypePropertyShouldWriteInstanceAnnotation()
        {
            this.SerializeComplexResource(null, this.addressWithInstanceAnnotation).Should().Contain("\"@Is.ReadOnly\":true");
        }
        [Fact]
        public void WritingTopLevelComplexTypePropertyShouldWriteInstanceAnnotations()
        {
            this.SerializeComplexResource(null, this.homeAddressWithInstanceAnnotations).Should().Contain("\"@Is.AutoComputable\":true,\"@Is.ReadOnly\":false");
        }
        #endregion Serializing top-level complextype properties

        #region Serializing complex properties

        [Fact]
        public void WritingPropertyInComplexTypeShouldWriteInsatanceAnnotation()
        {
            this.SerializeComplexResource(null, this.homeAddressWithInstanceAnnotations).Should()
                .Contain("\"FamilyName@FamilyName.annotation\":true,\"FamilyName\":\"Green\"").And
                .Contain("\"City@City.annotation1\":true,\"City@City.annotation2\":123,\"City\":\"Shanghai\"");
        }

        #endregion

        #region Serializing undeclared complex type properties
        [Fact]
        public void ShouldWriteODataTypeForUndeclaredComplexType()
        {
            string result = this.SerializeComplexResourceInEntity(address);
            result.Should().Contain("@odata.type\":\"#TestNamespace.Address");
        }

        [Fact]
        public void ShouldWriteODataTypeForForUndeclaredDerivedComplexType()
        {
            string result = this.SerializeComplexResourceInEntity(homeAddress);
            result.Should().Contain("@odata.type\":\"#TestNamespace.HomeAddress");
        }

        #endregion
        /// <summary>
        /// Serialize the given property as a non-top-level property in JSON Light.
        /// </summary>
        /// <param name="odataProperty">The property to serialize.</param>
        /// <returns>A string of JSON text, where the given ODataProperty has been serialized and wrapped in a JSON object.</returns>
        private string SerializeComplexResource(IEdmStructuredType owningType, ODataResource complex)
        {
            MemoryStream outputStream = new MemoryStream();
            ODataJsonLightOutputContext jsonLightOutputContext = this.CreateJsonLightOutputContext(outputStream);
            var odataWriter = jsonLightOutputContext.CreateODataResourceWriter(null, owningType);
            odataWriter.WriteStart(complex);
            odataWriter.WriteEnd();
            odataWriter.Flush();
            outputStream.Position = 0;
            string result = new StreamReader(outputStream).ReadToEnd();

            return result;
        }

        private string SerializeComplexResourceInEntity(ODataResource complex)
        {
            MemoryStream outputStream = new MemoryStream();
            ODataJsonLightOutputContext jsonLightOutputContext = this.CreateJsonLightOutputContext(outputStream);
            var odataWriter = jsonLightOutputContext.CreateODataResourceWriter(null, entityType);
            odataWriter.WriteStart(new ODataResource()
            {
                SerializationInfo = new ODataResourceSerializationInfo()
                {
                    NavigationSourceEntityTypeName = "TestNamespace.EntityType",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                    NavigationSourceName = "entitySet",
                }
            });
            odataWriter.WriteStart(new ODataNestedResourceInfo() { Name = "TestProperty", IsCollection = false });
            odataWriter.WriteStart(complex);
            odataWriter.WriteEnd();
            odataWriter.WriteEnd();
            odataWriter.WriteEnd();
            odataWriter.Flush();
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
