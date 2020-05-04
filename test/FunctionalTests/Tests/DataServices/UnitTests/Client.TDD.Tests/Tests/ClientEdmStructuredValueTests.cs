//---------------------------------------------------------------------
// <copyright file="ClientEdmStructuredValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Xunit;

    public class ClientEdmStructuredValueTests
    {
        private Customer _entity;
        private Address _address;
        private ClientEdmStructuredValue _complexValue;
        private ClientEdmStructuredValue _entityValue;

        public ClientEdmStructuredValueTests()
        {
            this._address = new Address { Street = "123 fake st" };
            this._entity = new Customer { Id = 1, Address = this._address, Emails = new List<string> { "fake@fake.com" } };

            var model = new ClientEdmModel(ODataProtocolVersion.V4);
            var entityType = model.GetOrCreateEdmType(typeof(Customer));
            var complexType = model.GetOrCreateEdmType(typeof(Address));

            this._complexValue = new ClientEdmStructuredValue(this._address, model, model.GetClientTypeAnnotation(complexType));
            this._entityValue = new ClientEdmStructuredValue(this._entity, model, model.GetClientTypeAnnotation(entityType));
        }

        [Fact]
        public void KindShouldBeStructured()
        {
            this._entityValue.ValueKind.Should().Be(EdmValueKind.Structured);
        }

        [Fact]
        public void TypeReferenceShouldBeEntity()
        {
            this._entityValue.Type.Should().BeAssignableTo<EdmEntityTypeReference>();
        }

        [Fact]
        public void TypeReferenceShouldBeComplex()
        {
            this._complexValue.Type.Should().BeAssignableTo<EdmComplexTypeReference>();
        }

        [Fact]
        public void FindPropertyValueShouldNotFindProperty()
        {
            this._entityValue.FindPropertyValue("Fake").Should().BeNull();
        }

        [Fact]
        public void FindPropertyValueShouldReturnPrimitive()
        {
            var property = this._entityValue.FindPropertyValue("Id");
            property.Should().NotBeNull();
            property.Name.Should().Be("Id");
            property.Value.Should().BeAssignableTo<EdmIntegerConstant>();
            property.Value.As<EdmIntegerConstant>().Value.Should().Be(this._entity.Id);
        }

        [Fact]
        public void FindPropertyValueShouldReturnComplex()
        {
            var property = this._entityValue.FindPropertyValue("Address");
            property.Should().NotBeNull();
            property.Name.Should().Be("Address");
            property.Value.Should().BeAssignableTo<ClientEdmStructuredValue>();
            property.Value.As<ClientEdmStructuredValue>().FindPropertyValue("Street").Should().NotBeNull();
        }

        [Fact]
        public void FindPropertyValueShouldReturnCollection()
        {
            var property = this._entityValue.FindPropertyValue("Emails");
            property.Should().NotBeNull();
            property.Name.Should().Be("Emails");
            property.Value.Should().BeAssignableTo<IEdmCollectionValue>();
            property.Value.As<IEdmCollectionValue>().Elements.Should().HaveCount(1).And.Contain(d => d.Value is EdmStringConstant);
        }


        [Fact]
        public void FindPropertyValueShouldReturnNullValue()
        {
            this._entity.Address = null;
            var property = this._entityValue.FindPropertyValue("Address");
            property.Should().NotBeNull();
            property.Name.Should().Be("Address");
            property.Value.ValueKind.Should().Be(EdmValueKind.Null);
        }

        [Fact]
        public void PropertyValuesShouldNotBeCached()
        {
            var property = this._entityValue.FindPropertyValue("Id");
            property.Value.As<EdmIntegerConstant>().Value.Should().Be(this._entity.Id);

            this._entity.Id += 10;
            property = this._entityValue.FindPropertyValue("Id");
            property.Value.As<EdmIntegerConstant>().Value.Should().Be(this._entity.Id);
        }

        [Fact]
        public void PropertyValuesShouldContainFullSet()
        {
            this._entityValue.PropertyValues.Select(p => p.Name).Should().Contain(new[] { "Id", "Address", "Emails" });
        }

        [Fact]
        public void ContextShouldCreateStructuredValueOnAttach()
        {
            var ctx = new DataServiceContext(new Uri("http://test.org/"), ODataProtocolVersion.V4);
            ctx.AttachTo("Customers", new Customer());
            ctx.Entities[0].EdmValue.Should().BeAssignableTo<ClientEdmStructuredValue>();
        }
    }

    [Key("Id")]
    public class Customer
    {
        public int Id { get; set; }
        public Address Address { get; set; }
        public IList<string> Emails { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
    }

    public class HomeAddress : Address
    {
        public string Number { get; set; }
    }
}