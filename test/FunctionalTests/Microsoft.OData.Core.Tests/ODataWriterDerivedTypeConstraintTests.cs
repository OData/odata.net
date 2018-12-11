//---------------------------------------------------------------------
// <copyright file="ODataWriterDerivedTypeConstraintTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Unit tests for writing ODataItem with Org.OData.Validation.V1.DerivedTypeConstraint.
    /// </summary>
    public class ODataWriterDerivedTypeConstraintTests
    {
        private EdmModel edmModel;
        private EdmEntityType edmCustomerType;
        private EdmEntityType edmVipCustomerType;
        private EdmEntityType edmNormalCustomerType;

        private EdmComplexType edmAddressType;
        private EdmComplexType edmUsAddressType;
        private EdmComplexType edmCnAddressType;

        private EdmSingleton edmMe;
        private EdmEntitySet edmCustomers;

        private ODataResource odataCustomerResource;
        private ODataResource odataVipResource;
        private ODataResource odataNormalResource;

        private ODataResource odataAddressResource;
        private ODataResource odataUsAddressResource;
        private ODataResource odataCnAddressResource;

        public ODataWriterDerivedTypeConstraintTests()
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

            // resource
            odataCustomerResource = new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = new[] { new ODataProperty { Name = "Id", Value = 7 } }
            };

            odataVipResource = new ODataResource
            {
                TypeName = "NS.VipCustomer",
                Properties = new[] { new ODataProperty { Name = "Id", Value = 8 }, new ODataProperty { Name = "Vip", Value = "Boss" } }
            };

            odataNormalResource = new ODataResource
            {
                TypeName = "NS.NormalCustomer",
                Properties = new[] { new ODataProperty { Name = "Id", Value = 9 }, new ODataProperty { Name = "Email", Value = "a@abc.com" } }
            };

            odataAddressResource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new[] { new ODataProperty { Name = "Street", Value = "Way" }}
            };

            odataUsAddressResource = new ODataResource
            {
                TypeName = "NS.UsAddress",
                Properties = new[] { new ODataProperty {Name = "Street", Value = "RedWay"}, new ODataProperty {Name = "ZipCode", Value = 98052}}
            };

            odataCnAddressResource = new ODataResource
            {
                TypeName = "NS.CnAddress",
                Properties = new[] { new ODataProperty {Name = "Street", Value = "ShaWay"}, new ODataProperty {Name = "PostCode", Value = "201100"}}
            };
        }

        #region Singleton Derived Type Constraints
        [Fact]
        public void WritingResourceWithSameTypeAsSingletonTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<ODataMessageWriter> writeCustomerAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmMe, this.edmCustomerType); // Singleton
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteEnd();
            };

            // Singleton doesn't have the derived type constraint.
            string customerActual = GetWriterOutput(this.edmModel, writeCustomerAction);
            Assert.Contains("/$metadata#Me\",\"Id\":7}", customerActual);

            // Singleton has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmMe, "NS.VipCustomer");
            string customerActualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeCustomerAction);
            Assert.Equal(customerActual, customerActualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingResourceAsAllowedTypeForSingletonWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<ODataMessageWriter> writeVipCustomerAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmMe, this.edmCustomerType); // Singleton
                writer.WriteStart(this.odataVipResource);
                writer.WriteEnd();
            };

            // Singleton doesn't have the derived type constraint.
            string vipCustomerActual = GetWriterOutput(this.edmModel, writeVipCustomerAction);
            Assert.Contains("/$metadata#Me\",\"@odata.type\":\"#NS.VipCustomer\",\"Id\":8,\"Vip\":\"Boss\"}", vipCustomerActual);

            // Singleton has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmMe, "NS.VipCustomer");
            string vipCustomerActualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeVipCustomerAction);
            Assert.Equal(vipCustomerActual, vipCustomerActualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingResourceForSingletonWithoutDerivedTypeConstraintsWorksButWithConstraintFailed()
        {
            Action<ODataMessageWriter> writeNormalCustomerAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmMe, this.edmCustomerType); // Singleton
                writer.WriteStart(this.odataNormalResource);
                writer.WriteEnd();
            };

            // Singleton doesn't have the derived type constraint.
            string normalCustomerActual = GetWriterOutput(this.edmModel, writeNormalCustomerAction);
            Assert.Contains("/$metadata#Me\",\"@odata.type\":\"#NS.NormalCustomer\",\"Id\":9,\"Email\":\"a@abc.com\"}", normalCustomerActual);

            // Negative test case -- singleton has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmMe, "NS.VipCustomer");

            Action test = () => GetWriterOutput(this.edmModel, writeNormalCustomerAction);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.WriterValidationUtils_ResourceValueTypeNotAllowedInDerivedTypeConstraint("NS.NormalCustomer", "Me"), exception.Message);
        }
        #endregion

        #region EntitySet Derived Type Constraints
        [Fact]
        public void WritingResourceWithSameTypeAsEntitySetTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            ODataResource anotherCustomerResource = new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = new[] { new ODataProperty { Name = "Id", Value = 77 } }
            };

            Action<ODataMessageWriter> writeCustomersAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceSetWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteEnd();
                writer.WriteStart(anotherCustomerResource);
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // EntitySet doesn't have the derived type constraint.
            string customersActual = GetWriterOutput(this.edmModel, writeCustomersAction);
            Assert.Contains("/$metadata#Customers\",\"value\":[{\"Id\":7},{\"Id\":77}]}", customersActual);

            // EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");
            string customersActualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeCustomersAction);
            Assert.Equal(customersActual, customersActualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingResourceWithSameAndAllowedTypeAsEntitySetTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<ODataMessageWriter> writeCustomersAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceSetWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteEnd();
                writer.WriteStart(this.odataVipResource);
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // EntitySet doesn't have the derived type constraint.
            string customersActual = GetWriterOutput(this.edmModel, writeCustomersAction);
            Assert.Contains("/$metadata#Customers\",\"value\":[{\"Id\":7},{\"@odata.type\":\"#NS.VipCustomer\",\"Id\":8,", customersActual);

            // EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");
            string customersActualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeCustomersAction);
            Assert.Equal(customersActual, customersActualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingResourceForEntitySetWithoutDerivedTypeConstraintsWorksButWithConstraintFailed()
        {
            Action<ODataMessageWriter> writeCustomersAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceSetWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteEnd();
                writer.WriteStart(this.odataNormalResource);
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // EntitySet doesn't have the derived type constraint.
            string normalCustomersActual = GetWriterOutput(this.edmModel, writeCustomersAction);
            Assert.Contains("/$metadata#Customers\",\"value\":[{\"Id\":7},{\"@odata.type\":\"#NS.NormalCustomer\",\"Id\":9,\"Email\":\"a@abc.com\"}]}", normalCustomersActual);

            // Negative test case -- EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");

            Action test = () => GetWriterOutput(this.edmModel, writeCustomersAction);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.WriterValidationUtils_ResourceValueTypeNotAllowedInDerivedTypeConstraint("NS.NormalCustomer", "Customers"), exception.Message);
        }
        #endregion

        #region Entity Derived Type Constraints
        [Fact]
        public void WritingResourceWithSameTypeAsEntityTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<ODataMessageWriter> writeCustomerAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteEnd();
            };

            // EntitySet doesn't have the derived type constraint.
            string customerActual = GetWriterOutput(this.edmModel, writeCustomerAction);
            Assert.Contains("/$metadata#Customers/$entity\",\"Id\":7}", customerActual);

            // EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");

            string customerActualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeCustomerAction);
            Assert.Equal(customerActual, customerActualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingResourceWithSameAndAllowedTypeAsEntityTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            Action<ODataMessageWriter> writeCustomersAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(this.odataVipResource);
                writer.WriteEnd();
            };

            // EntitySet doesn't have the derived type constraint.
            string vipActual = GetWriterOutput(this.edmModel, writeCustomersAction);
            Assert.Contains("/$metadata#Customers/$entity\",\"@odata.type\":\"#NS.VipCustomer\",\"Id\":8,", vipActual);

            // EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");

            string vipActualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeCustomersAction);
            Assert.Equal(vipActual, vipActualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingResourceForEntityWorksWithoutDerivedTypeConstraintsButFailedWithConstraint()
        {
            Action<ODataMessageWriter> writeCustomersAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(this.odataNormalResource);
                writer.WriteEnd();
            };

            // EntitySet doesn't have the derived type constraint.
            string normalCustomerActual = GetWriterOutput(this.edmModel, writeCustomersAction);
            Assert.Contains("/$metadata#Customers/$entity\",\"@odata.type\":\"#NS.NormalCustomer\",\"Id\":9,\"Email\":\"a@abc.com\"}", normalCustomerActual);

            // Negative test case -- EntitySet has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, this.edmCustomers, "NS.VipCustomer");

            Action test = () => GetWriterOutput(this.edmModel, writeCustomersAction);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.WriterValidationUtils_ResourceValueTypeNotAllowedInDerivedTypeConstraint("NS.NormalCustomer", "Customers"), exception.Message);
        }
        #endregion

        #region Navigation property nested resource info Derived Type Constraints
        [Fact]
        public void WritingSingleNavigationProppertyWithSameTypeAsNavigationPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <NavigationProperty Name="FriendCustomer" Type="NS.Customer" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "FriendCustomer"
            });

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "FriendCustomer",
                IsCollection = false
            };

            ODataResource nestedFriendCustomer = new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = new[] { new ODataProperty { Name = "Id", Value = 5 } }
            };

            Action<ODataMessageWriter> writeNavigationPropertyAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteStart(nestedResourceInfo);
                writer.WriteStart(nestedFriendCustomer); // Customer
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // Navigation property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writeNavigationPropertyAction);
            Assert.Contains("#Customers/$entity\",\"Id\":7,\"FriendCustomer\":{\"Id\":5}}", actual);

            // Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.VipCustomer");

            string actualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeNavigationPropertyAction);
            Assert.Equal(actual, actualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingSingleNavigationProppertyWithAllowedTypeAsNavigationPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <NavigationProperty Name="FriendCustomer" Type="NS.Customer" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "FriendCustomer"
            });

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "FriendCustomer",
                IsCollection = false
            };

            Action<ODataMessageWriter> writeNavigationPropertyAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmMe, this.edmCustomerType); // singleton
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteStart(nestedResourceInfo);
                writer.WriteStart(this.odataVipResource); // VipCustomer
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // Navigation property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writeNavigationPropertyAction);
            Assert.Contains("#Me\",\"Id\":7,\"FriendCustomer\":{\"@odata.type\":\"#NS.VipCustomer\",\"Id\":8", actual);

            // Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.VipCustomer");

            string actualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeNavigationPropertyAction);
            Assert.Equal(actual, actualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingSingleNavigationProppertyWithNotAllowedTypeWorksWithoutDerivedTypeConstraintButFailedWithDerivedTypeConstraint()
        {
            // Add a <NavigationProperty Name="FriendCustomer" Type="NS.Customer" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.One,
                Name = "FriendCustomer"
            });

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "FriendCustomer",
                IsCollection = false
            };

            Action<ODataMessageWriter> writeNavigationPropertyAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteStart(nestedResourceInfo);
                writer.WriteStart(this.odataNormalResource); // Normal customer
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // Navigation property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writeNavigationPropertyAction);
            Assert.Contains("#Customers/$entity\",\"Id\":7,\"FriendCustomer\":{\"@odata.type\":\"#NS.NormalCustomer\",\"Id\":9", actual);

            // Negative test case --Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.VipCustomer");

            Action test = () => GetWriterOutput(this.edmModel, writeNavigationPropertyAction);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.WriterValidationUtils_PropertyValueTypeNotAllowedInDerivedTypeConstraint("NS.NormalCustomer", "FriendCustomer"), exception.Message);
        }

        [Fact]
        public void WritingMultipleNavigationProppertyWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <NavigationProperty Name="FriendCustomers" Type="Collection(NS.Customer)" />
            var navigationProperty = this.edmCustomerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Target = this.edmCustomerType,
                TargetMultiplicity = EdmMultiplicity.Many,
                Name = "FriendCustomers"
            });

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "FriendCustomers",
                IsCollection = true
            };

            Action<ODataMessageWriter> writeNavigationPropertyAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType);
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteStart(nestedResourceInfo);
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(this.odataNormalResource); // normalCustomer
                writer.WriteEnd();
                writer.WriteStart(this.odataVipResource); // vipCustomer
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // Navigation property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writeNavigationPropertyAction);
            Assert.Contains(",\"Id\":7,\"FriendCustomers\":[{\"@odata.type\":\"#NS.NormalCustomer\",\"Id\":9,\"Email\":\"", actual);

            // Negative test case --Navigation property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, navigationProperty, "NS.NormalCustomer");

            Action test = () => GetWriterOutput(this.edmModel, writeNavigationPropertyAction);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.WriterValidationUtils_PropertyValueTypeNotAllowedInDerivedTypeConstraint("NS.VipCustomer", "FriendCustomers"), exception.Message);
        }
        #endregion

        #region Complex nested resource info Derived Type Constraints

        [Fact]
        public void WritingSingleComplexProppertyWithSameTypeAsPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <Property Name="Location" Type="NS.Address" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Location", new EdmComplexTypeReference(this.edmAddressType, false));

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Location",
                IsCollection = false
            };

            Action<ODataMessageWriter> writePropertyAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteStart(nestedResourceInfo);
                writer.WriteStart(this.odataAddressResource); // Address
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // Structural property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writePropertyAction);
            Assert.Contains("#Customers/$entity\",\"Id\":7,\"Location\":{\"Street\":\"Way\"}}", actual);

            // Structural property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "NS.VipCustomer");

            string actualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writePropertyAction);
            Assert.Equal(actual, actualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingSingleComplexProppertyWithAllowedTypeAsPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <Property Name="Location" Type="NS.Address" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Location", new EdmComplexTypeReference(this.edmAddressType, false));

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Location",
                IsCollection = false
            };

            Action<ODataMessageWriter> writePropertyAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmMe, this.edmCustomerType); // singleton
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteStart(nestedResourceInfo);
                writer.WriteStart(this.odataUsAddressResource); // UsAddress
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // Structural property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writePropertyAction);
            Assert.Contains("#Me\",\"Id\":7,\"Location\":{\"@odata.type\":\"#NS.UsAddress\",\"Street\":\"RedWay\",\"ZipCode\":98052}}", actual);

            // Structural property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "NS.UsAddress");

            string actualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writePropertyAction);
            Assert.Equal(actual, actualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingSingleComplexProppertyWithNotAllowedTypeWorksWithoutDerivedTypeConstraintButFailedWithDerivedTypeConstraint()
        {
            // Add a <Property Name="Location" Type="NS.Address" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Location", new EdmComplexTypeReference(this.edmAddressType, false));

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Location",
                IsCollection = false
            };

            Action<ODataMessageWriter> writePropertyAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteStart(nestedResourceInfo);
                writer.WriteStart(this.odataCnAddressResource); // CnAddress
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // Structural property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writePropertyAction);
            Assert.Contains("#Customers/$entity\",\"Id\":7,\"Location\":{\"@odata.type\":\"#NS.CnAddress\",\"Street\":\"ShaWay\",", actual);

            // Negative test case --Structural property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "NS.UsAddress");

            Action test = () => GetWriterOutput(this.edmModel, writePropertyAction);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.WriterValidationUtils_PropertyValueTypeNotAllowedInDerivedTypeConstraint("NS.CnAddress", "Location"), exception.Message);
        }

        [Fact]
        public void WritingMultipleComplexProppertyWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <Property Name="Locations" Type="Collection(NS.Address)" />
            var locationsProperty = this.edmCustomerType.AddStructuralProperty("Locations", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(this.edmAddressType, false))));

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Locations",
                IsCollection = true
            };

            Action<ODataMessageWriter> writePropertyAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(this.odataCustomerResource);
                writer.WriteStart(nestedResourceInfo);
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(this.odataCnAddressResource); // CnAddress
                writer.WriteEnd();
                writer.WriteStart(this.odataUsAddressResource); // UsAddress
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            // Structural property doesn't have the derived type constraint.
            string usAddressActual = GetWriterOutput(this.edmModel, writePropertyAction);
            Assert.Contains(",\"Id\":7,\"Locations\":[{\"@odata.type\":\"#NS.CnAddress\",\"Street\":\"Sha", usAddressActual);

            // Negative test case --Structural property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationsProperty, "NS.CnAddress");

            Action test = () => GetWriterOutput(this.edmModel, writePropertyAction);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.WriterValidationUtils_PropertyValueTypeNotAllowedInDerivedTypeConstraint("NS.UsAddress", "Locations"), exception.Message);
        }
        #endregion

        #region Primitive property Derived Type Constraints

        [Fact]
        public void WritingEdmPrimitiveProppertyWithAllowedTypeAsPropertyTypeWithOrWithoutDerivedTypeConstraintWorks()
        {
            // Add a <Property Name="Data" Type="Edm.PrimitiveType" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetPrimitiveType(false));

            ODataResource customer = new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = new[] { new ODataProperty { Name = "Id", Value = 7 }, new ODataProperty { Name = "Data", Value = false } }
            };

            Action<ODataMessageWriter> writeAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(customer);
                writer.WriteEnd();
            };

            // EdmPrimitive property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writeAction);
            Assert.Contains("#Customers/$entity\",\"Id\":7,\"Data@odata.type\":\"#Boolean\",\"Data\":false}", actual);

            // EdmPrimitive property has the derived type constraints.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "Edm.Boolean", "Edm.Int32");

            string actualWithDerivedTypeConstraint = GetWriterOutput(this.edmModel, writeAction);
            Assert.Equal(actual, actualWithDerivedTypeConstraint);
        }

        [Fact]
        public void WritingEdmPrimitiveProppertyWithNotAllowedTypeWorksWithoutDerivedTypeConstraintButFailedWithDerivedTypeConstraint()
        {
            // Add a <Property Name="Data" Type="Edm.PrimitiveType" />
            var locationProperty = this.edmCustomerType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetPrimitiveType(false));

            ODataResource customer = new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = new[] { new ODataProperty { Name = "Id", Value = 7 }, new ODataProperty { Name = "Data", Value = 8.9 } }
            };

            Action<ODataMessageWriter> writeAction = (messageWriter) =>
            {
                ODataWriter writer = messageWriter.CreateODataResourceWriter(this.edmCustomers, this.edmCustomerType); // entityset
                writer.WriteStart(customer);
                writer.WriteEnd();
            };

            // EdmPrimitive property doesn't have the derived type constraint.
            string actual = GetWriterOutput(this.edmModel, writeAction);
            Assert.Contains("#Customers/$entity\",\"Id\":7,\"Data@odata.type\":\"#Double\",\"Data\":8.9}", actual);

            // Negative test case --EdmPrimitive property has the derived type constraint.
            SetDerivedTypeAnnotation(this.edmModel, locationProperty, "Edm.Boolean", "Edm.Int32");

            Action test = () => GetWriterOutput(this.edmModel, writeAction);
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.WriterValidationUtils_PropertyValueTypeNotAllowedInDerivedTypeConstraint("Edm.Double", "Data"), exception.Message);
        }
        #endregion

        private string GetWriterOutput(IEdmModel model, Action<ODataMessageWriter> writeAction)
        {
            MemoryStream outputStream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = outputStream };
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(new Uri("http://example.com"));

            string output;
            using (var messageWriter = new ODataMessageWriter(message, settings, model))
            {
                writeAction(messageWriter);
                outputStream.Seek(0, SeekOrigin.Begin);
                output = new StreamReader(outputStream).ReadToEnd();
            }

            return output;
        }

        private static void SetDerivedTypeAnnotation(EdmModel model, IEdmVocabularyAnnotatable target, params string[] derivedTypes)
        {
            IEdmTerm term = ValidationVocabularyModel.DerivedTypeConstraintTerm;
            var collectionExpression = new EdmCollectionExpression(derivedTypes.Select(d => new EdmStringConstant(d)));
            EdmVocabularyAnnotation valueAnnotationOnProperty = new EdmVocabularyAnnotation(target, term, collectionExpression);
            valueAnnotationOnProperty.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotationOnProperty);
        }
    }
}
