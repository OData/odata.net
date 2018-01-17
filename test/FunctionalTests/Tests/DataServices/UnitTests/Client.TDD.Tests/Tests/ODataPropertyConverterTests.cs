//---------------------------------------------------------------------
// <copyright file="ODataPropertyConverterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using AstoriaUnitTests.TDD.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Tests a subset of the serializer functionality in the client. More tests should be added as changes are made.
    /// </summary>
    [TestClass]
    public class ODataPropertyConverterTests
    {
        private ClientEdmModel clientModel;
        private EdmModel serverModel;
        private ODataPropertyConverter testSubject;
        private ClientTypeAnnotation clientTypeAnnotation;
        private DataServiceContext context;
        private EdmEntityType serverEntityType;
        private string serverEntityTypeName;
        private TestClientEntityType entity;
        private TestClientEntityType entityWithDerivedComplexProperty;
        private TestClientEntityType entityWithDerivedComplexInCollection;
        private string serverComplexTypeName;

        [TestInitialize]
        public void Init()
        {
            this.serverModel = new EdmModel();
            this.serverEntityType = new EdmEntityType("Server.NS", "ServerEntityType");
            this.serverEntityTypeName = "Server.NS.ServerEntityType";
            this.serverModel.AddElement(this.serverEntityType);
            this.serverEntityType.AddKeys(this.serverEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            var addressType = new EdmComplexType("Server.NS", "Address");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            this.serverEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));
            this.serverComplexTypeName = "Server.NS.Address";

            var homeAddressType = new EdmComplexType("Server.NS", "HomeAddress", addressType);
            homeAddressType.AddStructuralProperty("FamilyName", EdmPrimitiveTypeKind.String);
            this.serverComplexTypeName = "Server.NS.HomeAddress";

            var colorType = new EdmEnumType("Server.NS", "Color");
            colorType.AddMember(new EdmEnumMember(colorType, "Red", new EdmEnumMemberValue(1)));
            colorType.AddMember(new EdmEnumMember(colorType, "Blue", new EdmEnumMemberValue(2)));
            colorType.AddMember(new EdmEnumMember(colorType, "Green", new EdmEnumMemberValue(3)));
            this.serverEntityType.AddStructuralProperty("Color", new EdmEnumTypeReference(colorType, false));

            this.serverEntityType.AddStructuralProperty("Nicknames", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            this.serverEntityType.AddStructuralProperty("OtherAddresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressType, false))));

            this.clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            this.clientTypeAnnotation = this.clientModel.GetClientTypeAnnotation(typeof(TestClientEntityType));

            this.context = new DataServiceContext(new Uri("http://temp/"), ODataProtocolVersion.V4, this.clientModel);
            this.context.Format.UseJson(this.serverModel);

            this.testSubject = new ODataPropertyConverter(new RequestInfo(this.context));

            this.context.Format.UseJson(this.serverModel);
            this.context.ResolveName = t =>
                                       {
                                           if (t == typeof(TestClientEntityType))
                                           {
                                               return this.serverEntityTypeName;
                                           }

                                           if (t == typeof(Address))
                                           {
                                               return this.serverComplexTypeName;
                                           }

                                           return null;
                                       };
            this.entity = new TestClientEntityType
            {
                Id = 1,
                Name = "foo",
                Number = 1.23f,
                Address = new Address(),
                OpenAddress = new Address(),
                Nicknames = new string[0],
                OtherAddresses = new[] { new Address() },
                Color = 0
            };
            this.entityWithDerivedComplexProperty = new TestClientEntityType
            {
                Id = 1,
                Name = "foo",
                Number = 1.23f,
                Address = new HomeAddress(),
                OpenAddress = new Address(),
                Nicknames = new string[0],
                OtherAddresses = new[] { new Address() }
            };
            this.entityWithDerivedComplexInCollection = new TestClientEntityType
            {
                Id = 1,
                Name = "foo",
                Number = 1.23f,
                Address = new HomeAddress(),
                OpenAddress = new Address(),
                Nicknames = new string[0],
                OtherAddresses = new[] { new Address(), new HomeAddress() }
            };
        }

        [TestMethod]
        public void PrimitivePropertyThatIsNotDefinedOnTheServerShouldHaveTypeAnnotation()
        {
            var value = this.ConvertSinglePropertyValue("Number");
            value.Should().HavePrimitiveValue(this.entity.Number).And.HaveSerializationTypeName("Edm.Single");
        }

        [TestMethod]
        public void PrimitivePropertyThatIsDefinedOnTheServerShouldNotHaveTypeAnnotation()
        {
            var value = this.ConvertSinglePropertyValue("Id");
            value.Should().HavePrimitiveValue(this.entity.Id).And.NotHaveSerializationTypeName();
        }

        [TestMethod]
        public void PrimitivePropertyThatIsNullAndNotDefinedOnTheServerShouldNotHaveTypeAnnotation()
        {
            this.entity.Number = null;
            var value = this.ConvertSinglePropertyValue("Number");
            value.Should().BeODataNullValue().And.NotHaveSerializationTypeName();
        }

        [TestMethod]
        public void PrimitivePropertyWithMatchingJsonTypeAndNotDefinedOnTheServerShouldNotHaveTypeAnnotation()
        {
            var value = this.ConvertSinglePropertyValue("Name");
            value.Should().HavePrimitiveValue(this.entity.Name).And.NotHaveSerializationTypeName();
        }

        [TestMethod]
        public void ComplexPropertyNotDefinedOnTheServerShouldHaveTypeAnnotation()
        {
            var complex = this.ConvertSingleComplexProperty("OpenAddress");
            complex.Item.Should().BeResource().And.HaveSerializationTypeName(this.serverComplexTypeName);
        }

        [TestMethod]
        public void ComplexPropertyDefinedOnTheServerShouldHaveTypeAnnotation()
        {
            var complex = this.ConvertSingleComplexProperty("Address");
            complex.Item.Should().BeResource().And.HaveSerializationTypeName(this.serverComplexTypeName);
        }

        [TestMethod]
        public void DerivedComplexPropertyDefinedOnTheServerShouldHaveDerivedTypeAnnotation()
        {
            var property = this.clientTypeAnnotation.GetProperty("Address", UndeclaredPropertyBehavior.ThrowException);
            var results = this.testSubject.PopulateNestedComplexProperties(this.entityWithDerivedComplexProperty, this.serverEntityTypeName, new[] { property }, null);
            results.Should().HaveCount(1);

            var nestedResourceInfo = results.Single().NestedResourceInfo;
            var nestedResource = results.Single().NestedResourceOrResourceSet.Item;
            nestedResource.Should().BeResource().And.HaveSerializationTypeName(null);
            nestedResourceInfo.Name.Should().Be("Address");
        }

        [TestMethod]
        public void EnumPropertyWithNonExistingValueShouldThrow()
        {
            Action action = () => this.ConvertSinglePropertyValue("Color");
            action.ShouldThrow<NotSupportedException>().WithMessage("The enum type 'Color' has no member named '0'.");
        }

        [TestMethod]
        public void CollectionPropertyDefinedOnTheServerShouldHaveTypeAnnotation()
        {
            var value = this.ConvertSinglePropertyValue("Nicknames");
            value.Should().BeCollection().And.HaveSerializationTypeName("Collection(Edm.String)");
        }

        [TestMethod]
        public void ComplexInsideCollectionPropertyShouldHaveTypeAnnotation()
        {
            var collection = this.ConvertSingleComplexProperty("OtherAddresses");
            collection.Item.Should().BeResourceSet();
            var resourceSet = (ODataResourceSetWrapper)collection;
            resourceSet.Resources.Should().HaveCount(1);
            resourceSet.Resources.Cast<ODataResourceWrapper>().Single().Item.Should().BeResource().And.HaveSerializationTypeName("Server.NS.HomeAddress");
        }

        [TestMethod]
        public void DerivedComplexInsideCollectionPropertyShouldNotThrow()
        {
            var property = this.clientTypeAnnotation.GetProperty("OtherAddresses", UndeclaredPropertyBehavior.ThrowException);
            var results = this.testSubject.PopulateNestedComplexProperties(this.entityWithDerivedComplexInCollection, this.serverEntityTypeName, new[] { property }, null);
            var nested = results.Single().NestedResourceOrResourceSet as ODataResourceSetWrapper;
            nested.Item.Should().BeResourceSet();
            nested.Resources.Should().HaveCount(2);
        }

        private ODataValue ConvertSinglePropertyValue(string propertyName)
        {
            var property = this.clientTypeAnnotation.GetProperty(propertyName, UndeclaredPropertyBehavior.ThrowException);
            var results = this.testSubject.PopulateProperties(this.entity, this.serverEntityTypeName, new[] { property });
            results.Should().HaveCount(1);
            var odataProperty = results.Single();
            odataProperty.Name.Should().Be(propertyName);
            return odataProperty.ODataValue;
        }

        private ODataItemWrapper ConvertSingleComplexProperty(string propertyName)
        {
            var property = this.clientTypeAnnotation.GetProperty(propertyName, UndeclaredPropertyBehavior.ThrowException);
            var results = this.testSubject.PopulateNestedComplexProperties(this.entity, this.serverEntityTypeName, new[] { property }, null);
            results.Should().HaveCount(1);
            var odataProperty = results.Single();
            odataProperty.NestedResourceInfo.Name.Should().Be(propertyName);
            return odataProperty.NestedResourceOrResourceSet;
        }

        [Key("Id")]
        public class TestClientEntityType
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public ICollection<string> Nicknames { get; set; }
            public float? Number { get; set; }
            public Address Address { get; set; }
            public Address OpenAddress { get; set; }
            public ICollection<Address> OtherAddresses { get; set; }
            public Color Color { get; set; }
        }

        public class Address
        {
            public string Street { get; set; }
        }

        public class HomeAddress : Address
        {
            public string FamilyName { get; set; }
        }
    }
}