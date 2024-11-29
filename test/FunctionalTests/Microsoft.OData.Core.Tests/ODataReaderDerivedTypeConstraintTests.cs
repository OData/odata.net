//---------------------------------------------------------------------
// <copyright file="ODataReaderDerivedTypeConstraintTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Unit tests for reading ODataItem with Org.OData.Validation.V1.DerivedTypeConstraint.
    /// </summary>
    public class ODataReaderDerivedTypeConstraintTests
    {
        public static string TestBaseUri = "http://example.com/";

        private EdmModel edmModel;
        private EdmEntityType edmCustomerType;
        private EdmEntityType edmVipCustomerType;
        private EdmEntityType edmNormalCustomerType;

        private EdmComplexType edmAddressType;
        private EdmComplexType edmUsAddressType;
        private EdmComplexType edmCnAddressType;

        private EdmSingleton edmMe;
        private EdmEntitySet edmCustomers;

        public ODataReaderDerivedTypeConstraintTests()
        {
            // Create the basic model
            edmModel = new EdmModel();

            // Entity Type
            edmCustomerType = new EdmEntityType("NS", "Customer");
            edmCustomerType.AddKeys(edmCustomerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

            edmVipCustomerType = new EdmEntityType("NS", "VipCustomer", edmCustomerType);
            edmVipCustomerType.AddStructuralProperty("Vip", EdmPrimitiveTypeKind.String);

            edmNormalCustomerType = new EdmEntityType("NS", "NormalCustomer", edmCustomerType);
            edmNormalCustomerType.AddStructuralProperty("Email", EdmPrimitiveTypeKind.String);

            edmModel.AddElements(new[] { edmCustomerType, edmVipCustomerType, edmNormalCustomerType });

            // Complex type
            edmAddressType = new EdmComplexType("NS", "Address");
            edmAddressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);

            edmUsAddressType = new EdmComplexType("NS", "UsAddress", edmAddressType);
            edmUsAddressType.AddStructuralProperty("ZipCode", EdmPrimitiveTypeKind.Int32);

            edmCnAddressType = new EdmComplexType("NS", "CnAddress", edmAddressType);
            edmCnAddressType.AddStructuralProperty("PostCode", EdmPrimitiveTypeKind.String);

            edmModel.AddElements(new[] { edmAddressType, edmUsAddressType, edmCnAddressType });

            // EntityContainer
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            edmMe = container.AddSingleton("Me", edmCustomerType);
            edmCustomers = container.AddEntitySet("Customers", edmCustomerType);
            edmModel.AddElement(container);
        }

        #region Singleton Derived Type Constraints

        [Fact]
        public void ReaderResourceWithSameTypeAsSingletonTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<Stack<ODataItem>> verifyItems = (s) =>
            {
                Assert.NotNull(s);
                var item = Assert.Single(s);
                ODataResource resource = item as ODataResource;
                Assert.NotNull(resource);
                var property = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", property.Name);
                Assert.Equal(7, property.Value);
            };

            string payload = @"{""@odata.context"":""http://example.com/$metadata#Me"",""Id"":7}";

            // Singleton doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            verifyItems(items);

            // Singleton has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmMe, "NS.VipCustomer");
            items = ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingResourceAsAllowedTypeForSingletonWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<Stack<ODataItem>> verifyItems = (s) =>
            {
                Assert.NotNull(s);
                var item = Assert.Single(s);
                ODataResource resource = item as ODataResource;
                Assert.NotNull(resource);
                Assert.Equal("NS.VipCustomer", resource.TypeName);
                var properties = resource.Properties.OfType<ODataProperty>();
                Assert.Equal(8, properties.Single(c => c.Name == "Id").Value);
                Assert.Equal("Boss", properties.Single(c => c.Name == "Vip").Value);
            };

            string payload = @"{""@odata.context"":""http://example.com/$metadata#Me"",""@odata.type"":""#NS.VipCustomer"",""Id"":8,""Vip"":""Boss""}";

            // Singleton doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            verifyItems(items);

            // Singleton has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmMe, "NS.VipCustomer");
            items = ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingResourceForSingletonWithoutDerivedTypeConstraintsWorksButWithConstraintFailed()
        {
            string payload = @"{""@odata.context"":""http://example.com/$metadata#Me"",""@odata.type"":""#NS.NormalCustomer"",""Id"":9,""Email"":""a@abc.com""}";

            // Singleton doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            var item = Assert.Single(items);
            ODataResource resource = item as ODataResource;
            Assert.NotNull(resource);
            Assert.Equal("NS.NormalCustomer", resource.TypeName);

            // Negative test case -- singleton has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmMe, "NS.VipCustomer");

            Action test = () => ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("NS.NormalCustomer", "navigation source", "Me"), exception.Message);
        }
        #endregion

        #region EntitySet Derived Type Constraints
        [Fact]
        public void ReadingResourceWithSameTypeAsEntitySetTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<Stack<ODataItem>> verifyItems = (ms) =>
            {
                Assert.NotNull(ms);
                Assert.Equal(3, ms.Count());
                Assert.IsType<ODataResourceSet>(ms.Pop()); // toplevel resource set
                ODataResource resource = Assert.IsType<ODataResource>(ms.Pop()); // second resource
                Assert.Equal(17, Assert.IsType<ODataProperty>(resource.Properties.Single(c => c.Name == "Id")).Value);
                resource = Assert.IsType<ODataResource>(ms.Pop());
                Assert.Equal(6, Assert.IsType<ODataProperty>(resource.Properties.Single(c => c.Name == "Id")).Value); // first resource
            };

            string payload = @"{""@odata.context"":""http://example.com/$metadata#Customers"",""value"":[{""Id"":6},{""Id"":17}]}";

            // EntitySet doesn't have the derived type constraint.
            var items = ReadEntitySetPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);

            // EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");
            items = ReadEntitySetPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingResourceWithSameAndAllowedTypeAsEntitySetTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<Stack<ODataItem>> verifyItems = (ms) =>
            {
                Assert.NotNull(ms);
                Assert.Equal(3, ms.Count());
                Assert.IsType<ODataResourceSet>(ms.Pop()); // toplevel resource set
                ODataResource resource = Assert.IsType<ODataResource>(ms.Pop()); // second resource
                Assert.Equal(17, resource.Properties.OfType<ODataProperty>().Single(c => c.Name == "Id").Value);
                Assert.Equal("NS.VipCustomer", resource.TypeName);
                resource = Assert.IsType<ODataResource>(ms.Pop());
                Assert.Equal(6, resource.Properties.OfType<ODataProperty>().Single(c => c.Name == "Id").Value); // first resource
            };

            string payload = @"{""@odata.context"":""http://example.com/$metadata#Customers"",""value"":[{""Id"":6},{""@odata.type"":""#NS.VipCustomer"",""Id"":17}]}";

            // EntitySet doesn't have the derived type constraint.
            var items = ReadEntitySetPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);

            // EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");
            items = ReadEntitySetPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingResourceForEntitySetWithoutDerivedTypeConstraintsWorksButWithConstraintFailed()
        {
            string payload = @"{""@odata.context"":""http://example.com/$metadata#Customers"",""value"":[{""Id"":6},{""@odata.type"":""#NS.VipCustomer"",""Id"":17}]}";

            // EntitySet doesn't have the derived type constraint.
            var items = ReadEntitySetPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            Assert.NotNull(items);
            Assert.Equal(3, items.Count()); // entityset + 2 resources

            // Negative test case -- EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.NormalCustomer");

            Action test = () => ReadEntitySetPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("NS.VipCustomer", "navigation source", "Customers"), exception.Message);
        }
        #endregion

        #region Entity Derived Type Constraints
        [Fact]
        public void ReadingResourceWithSameTypeAsEntityTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<Stack<ODataItem>> verifyItems = (s) =>
            {
                Assert.NotNull(s);
                var item = Assert.Single(s);
                ODataResource resource = item as ODataResource;
                Assert.NotNull(resource);
                var property = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal(7, property.Value);
            };

            string payload = @"{""@odata.context"":""http://example.com/$metadata#Customers/$entity"",""Id"":7}";

            // EntitySet doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);

            // EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");
            items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingResourceWithSameAndAllowedTypeAsEntityTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<Stack<ODataItem>> verifyItems = (s) =>
            {
                Assert.NotNull(s);
                var item = Assert.Single(s);
                ODataResource resource = item as ODataResource;
                Assert.NotNull(resource);
                Assert.Equal("NS.VipCustomer", resource.TypeName);
                var properties = resource.Properties.OfType<ODataProperty>();
                Assert.Equal(8, properties.Single(c => c.Name == "Id").Value);
                Assert.Equal("Boss", properties.Single(c => c.Name == "Vip").Value);
            };

            string payload = @"{""@odata.context"":""http://example.com/$metadata#Customers/$entity"",""@odata.type"":""#NS.VipCustomer"",""Id"":8,""Vip"":""Boss""}";

            // EntitySet doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);

            // EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");
            items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingResourceForEntityWorksWithoutDerivedTypeConstraintsButFailedWithConstraint()
        {
            string payload = @"{""@odata.context"":""http://example.com/$metadata#Customers/$entity"",""@odata.type"":""#NS.NormalCustomer"",""Id"":8,""Email"":""a@abc.com""}";

            // EntitySet doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            Assert.NotNull(items);
            Assert.Single(items);

            // Negative test case -- EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");

            Action test = () => ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("NS.NormalCustomer", "navigation source", "Customers"), exception.Message);
        }
        #endregion

        #region Navigation property nested resource info Derived Type Constraints
        [Fact]
        public void ReadingSingleNavigationProppertyWithSameTypeAsNavigationPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <NavigationProperty Name="FriendCustomer" Type="NS.Customer" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "FriendCustomer"
            });

            Action<Stack<ODataItem>> verifyItems = (ms) =>
            {
                Assert.NotNull(ms);
                Assert.Equal(4, ms.Count);
                ODataResource resource = Assert.IsType<ODataResource>(ms.Pop()); // top level resource (id=7)
                Assert.Equal(7, Assert.IsType<ODataProperty>(Assert.Single(resource.Properties)).Value);

                ODataNestedResourceInfo nestedLink = Assert.IsType<ODataNestedResourceInfo>(ms.Pop()); // FriendCustomer Navigation Link
                Assert.Equal(new Uri("http://example.com/Customers(7)/FriendCustomer"), nestedLink.Url);
                Assert.Equal("FriendCustomer", nestedLink.Name);

                resource = Assert.IsType<ODataResource>(ms.Pop()); // nested resource (id=5)
                Assert.Equal(5, resource.Properties.OfType<ODataProperty>().Single().Value);

                ODataNestedResourceInfo nestedResourceInfo = Assert.IsType<ODataNestedResourceInfo>(ms.Pop()); // FriendCustomer Nested resource info
                Assert.Equal(new Uri("http://example.com/Customers(7)/FriendCustomer/FriendCustomer"), nestedResourceInfo.Url);
                Assert.Equal("FriendCustomer", nestedResourceInfo.Name);
            };

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Customers/$entity\",\"Id\":7,\"FriendCustomer\":{\"Id\":5}}";

            // Navigation property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);

            // Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.VipCustomer");

            items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingSingleNavigationProppertyWithAllowedTypeAsNavigationPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <NavigationProperty Name="FriendCustomer" Type="NS.Customer" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "FriendCustomer"
            });

            Action<Stack<ODataItem>> verifyItems = (ms) =>
            {
                Assert.NotNull(ms);
                Assert.Equal(4, ms.Count);
                ODataResource resource = Assert.IsType<ODataResource>(ms.Pop()); // top level resource (id=7)
                Assert.Equal(7, Assert.IsType<ODataProperty>(Assert.Single(resource.Properties)).Value);

                ODataNestedResourceInfo nestedLink = Assert.IsType<ODataNestedResourceInfo>(ms.Pop()); // FriendCustomer Navigation Link
                Assert.Equal(new Uri("http://example.com/Customers(7)/FriendCustomer"), nestedLink.Url);
                Assert.Equal("FriendCustomer", nestedLink.Name);

                resource = Assert.IsType<ODataResource>(ms.Pop()); // nested resource (id=8)
                Assert.Equal(8, Assert.IsType<ODataProperty>(Assert.Single(resource.Properties)).Value);
                Assert.Equal("NS.VipCustomer", resource.TypeName);

                ODataNestedResourceInfo nestedResourceInfo = Assert.IsType<ODataNestedResourceInfo>(ms.Pop()); // FriendCustomer Nested resource info
                Assert.Equal(new Uri("http://example.com/Customers(7)/FriendCustomer/NS.VipCustomer/FriendCustomer"), nestedResourceInfo.Url);
                Assert.Equal("FriendCustomer", nestedResourceInfo.Name);
            };

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Customers/$entity\",\"Id\":7,\"FriendCustomer\":{\"@odata.type\":\"#NS.VipCustomer\",\"Id\":8}}";

            // Navigation property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);

            // Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.VipCustomer");

            items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingSingleNavigationProppertyWithNotAllowedTypeWorksWithoutDerivedTypeConstraintButFailedWithDerivedTypeConstraint()
        {
            // Add a <NavigationProperty Name="FriendCustomer" Type="NS.Customer" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "FriendCustomer"
            });

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Customers/$entity\",\"Id\":7,\"FriendCustomer\":{\"@odata.type\":\"#NS.VipCustomer\",\"Id\":8}}";

            // Navigation property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            Assert.Equal(4, items.Count);

            // Negative test case --Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.NormalCustomer");

            Action test = () => ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("NS.VipCustomer", "nested resource", "FriendCustomer"), exception.Message);
        }

        [Fact]
        public void ReadingMultipleNavigationProppertyWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <NavigationProperty Name="FriendCustomers" Type="Collection(NS.Customer)" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "FriendCustomers"
            });

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Customers/$entity\",\"Id\":7,\"FriendCustomers\":[{\"@odata.type\":\"#NS.NormalCustomer\",\"Id\":9,\"Email\":\"a@abc.com\"},{\"@odata.type\":\"#NS.VipCustomer\",\"Id\":8,\"Vip\":\"Boss\"}]}";

            // Navigation property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            Assert.Equal(7, items.Count); // 1 top level resource + 3 nested resource info + 1 nested resource set + 2 nested resource

            // Negative test case --Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.NormalCustomer");

            Action test = () => ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("NS.VipCustomer", "nested resource", "FriendCustomers"), exception.Message);
        }

        #endregion

        #region Complex nested resource info Derived Type Constraints

        [Fact]
        public void ReadingSingleComplexProppertyWithSameTypeAsPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <Property Name="Location" Type="NS.Address" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Location", new EdmComplexTypeReference(this.edmAddressType, false));

            Action<Stack<ODataItem>> verifyItems = (ms) =>
            {
                Assert.NotNull(ms);
                Assert.Equal(3, ms.Count);
                ODataResource resource = Assert.IsType<ODataResource>(ms.Pop()); // top level resource (id=7)
                Assert.Equal(7, Assert.IsType<ODataProperty>(Assert.Single(resource.Properties)).Value);

                ODataNestedResourceInfo nestedResourceInfo = Assert.IsType<ODataNestedResourceInfo>(ms.Pop()); // Location Nested resource info
                Assert.Null(nestedResourceInfo.Url);
                Assert.Equal("Location", nestedResourceInfo.Name);

                resource = Assert.IsType<ODataResource>(ms.Pop()); // nested complex resource (id=8)
                Assert.Equal("Way", Assert.IsType<ODataProperty>(Assert.Single(resource.Properties)).Value);
            };

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Customers/$entity\",\"Id\":7,\"Location\":{\"Street\":\"Way\"}}";

            // Structural property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);

            // Structural property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "NS.VipCustomer");

            items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingSingleComplexProppertyWithAllowedTypeAsPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <Property Name="Location" Type="NS.Address" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Location", new EdmComplexTypeReference(this.edmAddressType, false));

            Action<Stack<ODataItem>> verifyItems = (ms) =>
            {
                Assert.NotNull(ms);
                Assert.Equal(3, ms.Count);
                ODataResource resource = Assert.IsType<ODataResource>(ms.Pop()); // top level resource (id=7)
                Assert.Equal(7, Assert.IsType<ODataProperty>(Assert.Single(resource.Properties)).Value);

                ODataNestedResourceInfo nestedResourceInfo = Assert.IsType<ODataNestedResourceInfo>(ms.Pop()); // Location Nested resource info
                Assert.Null(nestedResourceInfo.Url);
                Assert.Equal("Location", nestedResourceInfo.Name);

                resource = Assert.IsType<ODataResource>(ms.Pop()); // nested complex resource (id=8)
                Assert.Equal("NS.UsAddress", resource.TypeName);
                var properties = resource.Properties.OfType<ODataProperty>();
                Assert.Equal("RedWay", properties.Single(c => c.Name == "Street").Value);
                Assert.Equal(98052, properties.Single(c => c.Name == "ZipCode").Value);
            };

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Me\",\"Id\":7,\"Location\":{\"@odata.type\":\"#NS.UsAddress\",\"Street\":\"RedWay\",\"ZipCode\":98052}}";

            // Structural property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            verifyItems(items);

            // Structural property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "NS.UsAddress");

            items = ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingingSingleComplexProppertyWithNotAllowedTypeWorksWithoutDerivedTypeConstraintButFailedWithDerivedTypeConstraint()
        {
            // Add a <Property Name="Location" Type="NS.Address" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Location", new EdmComplexTypeReference(this.edmAddressType, false));

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Me\",\"Id\":7,\"Location\":{\"@odata.type\":\"#NS.UsAddress\",\"Street\":\"RedWay\",\"ZipCode\":98052}}";

            // Structural property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            Assert.Equal(3, items.Count);

            // Negative test case --Structural property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "NS.CnAddress");

            Action test = () => ReadEntityPayload(payload, this.edmModel, this.edmMe, this.edmCustomerType);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("NS.UsAddress", "nested resource", "Location"), exception.Message);
        }

        [Fact]
        public void ReadingMultipleComplexProppertyWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <Property Name="Locations" Type="Collection(NS.Address)" />
            var locationsProperty = this.edmCustomerType.AddStructuralProperty("Locations", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(this.edmAddressType, false))));

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Customers/$entity\",\"Id\":7,\"Locations\":[{\"@odata.type\":\"#NS.CnAddress\",\"Street\":\"ShaWay\",\"PostCode\":\"201100\"},{\"@odata.type\":\"#NS.UsAddress\",\"Street\":\"RedWay\",\"ZipCode\":98052}]}";

            // Structural property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            Assert.Equal(5, items.Count); // 1 top level resource + 1 nested resource info + 1 nested resource set + 2 nested resources

            // Negative test case --Structural property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationsProperty, "NS.CnAddress");

            Action test = () => ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("NS.UsAddress", "nested resource", "Locations"), exception.Message);
        }

        #endregion

        #region Primitive property Derived Type Constraints

        [Fact]
        public void ReadingEdmPrimitiveProppertyWithAllowedTypeAsPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <Property Name="Data" Type="Edm.PrimitiveType" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetPrimitiveType(false));

            Action<Stack<ODataItem>> verifyItems = (ms) =>
            {
                Assert.NotNull(ms);
                Assert.Single(ms);
                ODataResource resource = Assert.IsType<ODataResource>(ms.Pop()); // top level resource (id=7)
                Assert.Equal(7, Assert.IsType<ODataProperty>(resource.Properties.Single(c => c.Name == "Id")).Value);
                Assert.Equal(false, Assert.IsType<ODataProperty>(resource.Properties.Single(c => c.Name == "Data")).Value);
            };

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Customers/$entity\",\"Id\":7,\"Data@odata.type\":\"#Boolean\",\"Data\":false}";

            // EdmPrimitive property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);

            // EdmPrimitive property has the derived type constraints.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "Edm.Boolean", "Edm.Int32");

            items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            verifyItems(items);
        }

        [Fact]
        public void ReadingEdmPrimitiveProppertyWithNotAllowedTypeWorksWithoutDerivedTypeConstraintButFailedWithDerivedTypeConstraint()
        {
            // Add a <Property Name="Data" Type="Edm.PrimitiveType" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetPrimitiveType(false));

            string payload = "{\"@odata.context\":\"http://example.com/$metadata#Customers/$entity\",\"Id\":7,\"Data@odata.type\":\"#Double\",\"Data\":8.9}";

            // EdmPrimitive property doesn't have the derived type constraint.
            var items = ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            Assert.Single(items);

            // Negative test case --EdmPrimitive property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "Edm.Boolean", "Edm.Int32");

            Action test = () => ReadEntityPayload(payload, this.edmModel, this.edmCustomers, this.edmCustomerType);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("Edm.Double", "property", "Data"), exception.Message);
        }
        #endregion

        #region Delta scenarios
        [Fact]
        public void ReadingDeltaResourceSetMultipleNavigationProppertyWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <NavigationProperty Name="FriendCustomers" Type="Collection(NS.Customer)" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "FriendCustomers"
            });

            string payload = "{\"@context\":\"http://example.com/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FriendCustomers@delta\":[{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}, {\"@odata.type\":\"#NS.NormalCustomer\",\"Id\":9,\"Email\":\"a@abc.com\"}]}]}";

            // Navigation property doesn't have the derived type constraint.
            var items = ReadDeltaPayload(payload, this.edmModel, this.edmCustomerType, ODataVersion.V401);
            Assert.Equal(6, items.Count); // 1 top level delta resource set
                                          //    [ 1 deleted resource
                                          //      1 nested resource info
                                          //         [ 1 nested delta resource set
                                          //             {2 nested delta resource }
                                          //         ]
                                          //    ]

            // Negative test case --Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.VipCustomer");

            Action test = () => ReadDeltaPayload(payload, this.edmModel, this.edmCustomerType, ODataVersion.V401);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint("NS.NormalCustomer", "nested resource", "FriendCustomers"), exception.Message);
        }
        #endregion

        private static void SetDerivedTypeAnnotation(EdmModel model, IEdmVocabularyAnnotatable target, params string[] derivedTypes)
        {
            IEdmTerm term = ValidationVocabularyModel.DerivedTypeConstraintTerm;
            var collectionExpression = new EdmCollectionExpression(derivedTypes.Select(d => new EdmStringConstant(d)));
            EdmVocabularyAnnotation valueAnnotationOnProperty = new EdmVocabularyAnnotation(target, term, collectionExpression);
            valueAnnotationOnProperty.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotationOnProperty);
        }

        private Stack<ODataItem> ReadEntityPayload(string payload, IEdmModel edmModel, IEdmNavigationSource source, IEdmEntityType sourceType)
        {
            Func<ODataMessageReader, ODataReader> createReader = (msReader) => msReader.CreateODataResourceReader(source, sourceType);

            return ReadPayload(payload, edmModel, createReader, ODataVersion.V4);
        }

        private Stack<ODataItem> ReadEntitySetPayload(string payload, IEdmModel edmModel, IEdmEntitySetBase source, IEdmEntityType sourceType)
        {
            Func<ODataMessageReader, ODataReader> createReader = (msReader) => msReader.CreateODataResourceSetReader(source, sourceType);

            return ReadPayload(payload, edmModel, createReader, ODataVersion.V4);
        }

        private Stack<ODataItem> ReadDeltaPayload(string payload, IEdmModel edmModel, IEdmEntityType sourceType, ODataVersion version)
        {
            Func<ODataMessageReader, ODataReader> createReader = (msReader) => msReader.CreateODataDeltaResourceSetReader(sourceType);

            return ReadPayload(payload, edmModel, createReader, version);
        }

        private Stack<ODataItem> ReadPayload(string payload, IEdmModel edmModel, Func<ODataMessageReader, ODataReader> createReader, ODataVersion version)
        {
            var message = new InMemoryMessage()
            {
                Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload))
            };
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
            {
                BaseUri = new Uri(TestBaseUri + "$metadata"),
                EnableMessageStreamDisposal = true,
                Version = version,
            };

            Stack<ODataItem> items = new Stack<ODataItem>();
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, edmModel))
            {
                var reader = createReader(msgReader);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceSetEnd:
                        case ODataReaderState.ResourceEnd:
                        case ODataReaderState.NestedResourceInfoEnd:
                        case ODataReaderState.DeltaResourceSetEnd:
                        case ODataReaderState.DeletedResourceEnd:
                            items.Push(reader.Item);
                            break;
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }

            return items;
        }
    }
}
