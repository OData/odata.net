//---------------------------------------------------------------------
// <copyright file="ODataDerivedResourceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataDerivedResourceTests : IClassFixture<ODataDerivedResourceTestsFixture>
    {
        private readonly ODataResource derivedResource;

        public ODataDerivedResourceTests(ODataDerivedResourceTestsFixture fixture)
        {
            derivedResource = fixture.InitDerivedResource();
        }

        [Fact]
        public void DerivedResourceWithComputedIdShouldHaveExpectedEditLink()
        {
            var editLink = derivedResource.MetadataBuilder.GetEditLink();

            Assert.NotNull(editLink);
            Assert.Equal("http://tempuri.org/Customers(1)/NS.EnterpriseCustomer", editLink.AbsoluteUri);
        }

        [Theory]
        [InlineData("Customers(1)", "Customers(1)/NS.EnterpriseCustomer")]
        [InlineData("Customers(1)/NS.EnterpriseCustomer", "Customers(1)/NS.EnterpriseCustomer")]
        [InlineData("Customers/NS.EnterpriseCustomer(1)", "Customers/NS.EnterpriseCustomer(1)")]
        public void DerivedResourceWithNonComputedIdShouldHaveExpectedEditLink(string odataPath, string expected)
        {
            derivedResource.Id = new Uri($"http://tempuri.org/{odataPath}");
            var editLink = derivedResource.MetadataBuilder.GetEditLink();

            Assert.NotNull(editLink);
            Assert.Equal($"http://tempuri.org/{expected}", editLink.AbsoluteUri);
        }

        [Fact]
        public void DerivedResourceWithComputedIdShouldHaveExpectedReadLink()
        {
            var readLink = derivedResource.MetadataBuilder.GetReadLink();

            Assert.NotNull(readLink);
            Assert.Equal("http://tempuri.org/Customers(1)/NS.EnterpriseCustomer", readLink.AbsoluteUri);
        }

        [Theory]
        [InlineData("Customers(1)", "Customers(1)/NS.EnterpriseCustomer")]
        [InlineData("Customers(1)/NS.EnterpriseCustomer", "Customers(1)/NS.EnterpriseCustomer")]
        [InlineData("Customers/NS.EnterpriseCustomer(1)", "Customers/NS.EnterpriseCustomer(1)")]
        public void DerivedResourceWithNonComputedIdShouldHaveExpectedReadLink(string odataPath, string expected)
        {
            derivedResource.Id = new Uri($"http://tempuri.org/{odataPath}");
            var readLink = derivedResource.MetadataBuilder.GetReadLink();

            Assert.NotNull(readLink);
            Assert.Equal($"http://tempuri.org/{expected}", readLink.AbsoluteUri);
        }

        [Fact]
        public void DerivedResourceWithComputedIdShouldHaveExpectedNavigationLink()
        {
            var navigationLink = derivedResource.MetadataBuilder.GetNavigationLinkUri("RelationshipManager", navigationLinkUrl: null, hasNestedResourceInfoUrl: false);

            Assert.NotNull(navigationLink);
            Assert.Equal("http://tempuri.org/Customers(1)/NS.EnterpriseCustomer/RelationshipManager", navigationLink.AbsoluteUri);
        }

        [Theory]
        [InlineData("Customers(1)", "Customers(1)/NS.EnterpriseCustomer")]
        [InlineData("Customers(1)/NS.EnterpriseCustomer", "Customers(1)/NS.EnterpriseCustomer")]
        [InlineData("Customers/NS.EnterpriseCustomer(1)", "Customers/NS.EnterpriseCustomer(1)")]
        public void DerivedResourceWithNonComputedIdShouldHaveExpectedNavigationLink(string odataPath, string expected)
        {
            derivedResource.Id = new Uri($"http://tempuri.org/{odataPath}");
            var navigationLink = derivedResource.MetadataBuilder.GetNavigationLinkUri(
                "RelationshipManager",
                navigationLinkUrl: null,
                hasNestedResourceInfoUrl: false);

            Assert.NotNull(navigationLink);
            Assert.Equal($"http://tempuri.org/{expected}/RelationshipManager", navigationLink.AbsoluteUri);
        }

        [Fact]
        public void DerivedResourceWithComputedIdShouldHaveExpectedAssociationLink()
        {
            var associationLink = derivedResource.MetadataBuilder.GetAssociationLinkUri(
                "RelationshipManager",
                associationLinkUrl: null,
                hasAssociationLinkUrl: false);

            Assert.NotNull(associationLink);
            Assert.Equal("http://tempuri.org/Customers(1)/NS.EnterpriseCustomer/RelationshipManager/$ref", associationLink.AbsoluteUri);
        }

        [Theory]
        [InlineData("Customers(1)", "Customers(1)/NS.EnterpriseCustomer")]
        [InlineData("Customers(1)/NS.EnterpriseCustomer", "Customers(1)/NS.EnterpriseCustomer")]
        [InlineData("Customers/NS.EnterpriseCustomer(1)", "Customers/NS.EnterpriseCustomer(1)")]
        public void DerivedResourceWithNonComputedIdShouldHaveExpectedAssociationLink(string odataPath, string expected)
        {
            derivedResource.Id = new Uri($"http://tempuri.org/{odataPath}");
            var associationLink = derivedResource.MetadataBuilder.GetAssociationLinkUri(
                "RelationshipManager",
                associationLinkUrl: null,
                hasAssociationLinkUrl: false);

            Assert.NotNull(associationLink);
            Assert.Equal($"http://tempuri.org/{expected}/RelationshipManager/$ref", associationLink.AbsoluteUri);
        }
    }

    public class ODataDerivedResourceTestsFixture : IDisposable
    {
        private const string baseUri = "http://tempuri.org";
        private static Uri serviceRoot = new Uri(baseUri);
        private readonly ODataMetadataContext metadataContext;
        private readonly ODataUriBuilder odataUriBuilder;
        private readonly IODataResourceTypeContext resourceTypeContext;
        private readonly ODataResourceSerializationInfo resourceSerializationInfo;
        private readonly EdmEntityType enterpriseCustomerEntityType;

        public ODataDerivedResourceTestsFixture()
        {
            var model = new EdmModel();

            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddElement(customerEntityType);

            var employeeEntityType = new EdmEntityType("NS", "Employee");
            employeeEntityType.AddKeys(employeeEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            model.AddElement(employeeEntityType);

            this.enterpriseCustomerEntityType = new EdmEntityType("NS", "EnterpriseCustomer", customerEntityType);
            model.AddElement(this.enterpriseCustomerEntityType);

            var relationshipManagerNavigationProperty = this.enterpriseCustomerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "RelationshipManager",
                    Target = employeeEntityType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                });

            var entityContainer = new EdmEntityContainer("NS", "Default");
            model.AddElement(entityContainer);

            var customersEntitySet = entityContainer.AddEntitySet("Customers", customerEntityType);
            var employeesEntitySet = entityContainer.AddEntitySet("Employees", employeeEntityType);

            customersEntitySet.AddNavigationTarget(relationshipManagerNavigationProperty, employeesEntitySet);

            this.resourceSerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceName = "Customers",
                NavigationSourceEntityTypeName = "NS.Customer",
                ExpectedTypeName = "NS.EnterpriseCustomer"
            };

            var odataUri = new ODataUri { ServiceRoot = serviceRoot, Path = new ODataPath(new EntitySetSegment(customersEntitySet)) };
            this.odataUriBuilder = new ODataConventionalUriBuilder(serviceRoot, ODataUrlKeyDelimiter.Parentheses);
            this.resourceTypeContext = ODataResourceTypeContext.Create(
                this.resourceSerializationInfo,
                customersEntitySet,
                customerEntityType,
                this.enterpriseCustomerEntityType,
                throwIfMissingTypeInfo: true);
            this.metadataContext = new ODataMetadataContext(
                isResponse: true,
                model: model,
                metadataDocumentUri: new Uri(serviceRoot, "$metadata"),
                odataUri: new ODataUriSlim(odataUri));
        }

        public ODataResource InitDerivedResource()
        {
            var derivedResource = new ODataResource
            {
                TypeName = "NS.EnterpriseCustomer",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    }
                }
            };

            var resourceMetadataContext = ODataResourceMetadataContext.Create(
                derivedResource,
                this.resourceTypeContext,
                this.resourceSerializationInfo,
                this.enterpriseCustomerEntityType,
                this.metadataContext,
                selectedProperties: new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree),
                metadataSelector: null);

            derivedResource.MetadataBuilder = new ODataConventionalEntityMetadataBuilder(
                resourceMetadataContext,
                this.metadataContext,
                this.odataUriBuilder);

            return derivedResource;
        }

        public void Dispose()
        {

        }
    }
}
