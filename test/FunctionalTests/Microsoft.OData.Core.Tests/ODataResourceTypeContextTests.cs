//---------------------------------------------------------------------
// <copyright file="ODataResourceTypeContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Tests.IntegrationTests.Reader.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataResourceTypeContextTests
    {
        private static readonly ODataResourceTypeContext TypeContextWithoutModel;
        private static readonly ODataResourceTypeContext TypeContextWithModel;
        private static readonly ODataResourceTypeContext BaseTypeContextThatThrows;
        private static readonly ODataResourceTypeContext BaseTypeContextThatWillNotThrow;
        private static readonly ODataResourceTypeContext TypeContextWithEdmUnknowEntitySet;
        private static readonly ODataResourceSerializationInfo SerializationInfo;
        private static readonly ODataResourceSerializationInfo SerializationInfoWithEdmUnknowEntitySet;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmEntityType EntitySetElementType;
        private static readonly EdmEntityType ExpectedEntityType;
        private static readonly EdmEntityType ActualEntityType;
        private static readonly EdmComplexType ComplexType;
        private static readonly EdmModel Model;

        static ODataResourceTypeContextTests()
        {
            Model = new EdmModel();
            EntitySetElementType = new EdmEntityType("ns", "Customer");
            ExpectedEntityType = new EdmEntityType("ns", "VipCustomer", EntitySetElementType);
            ActualEntityType = new EdmEntityType("ns", "DerivedVipCustomer", ExpectedEntityType);
            ComplexType = new EdmComplexType("ns", "Address");

            EdmEntityContainer defaultContainer = new EdmEntityContainer("ns", "DefaultContainer");
            Model.AddElement(defaultContainer);

            EntitySet = new EdmEntitySet(defaultContainer, "Customers", EntitySetElementType);
            Model.AddElement(EntitySetElementType);
            Model.AddElement(ExpectedEntityType);
            Model.AddElement(ActualEntityType);
            defaultContainer.AddElement(EntitySet);



            SerializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "MyCustomers", NavigationSourceEntityTypeName = "ns.MyCustomer", ExpectedTypeName = "ns.MyVipCustomer" };
            SerializationInfoWithEdmUnknowEntitySet = new ODataResourceSerializationInfo() { NavigationSourceName = null, NavigationSourceEntityTypeName = "ns.MyCustomer", ExpectedTypeName = "ns.MyVipCustomer", NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet };
            TypeContextWithoutModel = ODataResourceTypeContext.Create(SerializationInfo, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: true);
            TypeContextWithModel = ODataResourceTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, throwIfMissingTypeInfo: true);
            TypeContextWithEdmUnknowEntitySet = ODataResourceTypeContext.Create(SerializationInfoWithEdmUnknowEntitySet, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: true);
            BaseTypeContextThatThrows = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: true);
            BaseTypeContextThatWillNotThrow = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: false);
        }

        [Fact]
        public void ShouldCreateSubclassWithoutModelWhenSerializationInfoIsGiven()
        {
            var typeContext = ODataResourceTypeContext.Create(SerializationInfo, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithoutModel").Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateSubclassWithModelWhenMetadataIsGiven()
        {
            var typeContext = ODataResourceTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithModel").Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateSubclassWithModelWhenExpectedTypeisGiven()
        {
            var typeContext = ODataResourceTypeContext.Create(null, navigationSource: null,
                navigationSourceEntityType: null, expectedResourceType: ComplexType, throwIfMissingTypeInfo: false);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithModel").Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateSubclassWithoutModelWhenBothSerializationInfoAndMetadataAreGiven()
        {
            var typeContext = ODataResourceTypeContext.Create(SerializationInfo, EntitySet, EntitySetElementType, ExpectedEntityType, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithoutModel").Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateBaseClassWhenSerializationInfoAndUserModelAreBothMissingAndThrowIfMissingTypeInfoIsTrue()
        {
            var typeContext = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: true);
            typeContext.Should().BeOfType<ODataResourceTypeContext>();
        }

        [Fact]
        public void ShouldCreateBaseClassWhenSerializationInfoAndUserModelAreBothMissingAndThrowIfMissingTypeInfoIsFalse()
        {
            var typeContext = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: false);
            typeContext.Should().BeOfType<ODataResourceTypeContext>();
        }

        #region TypeContextWithoutModel
        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnEntitySetName()
        {
            TypeContextWithoutModel.NavigationSourceName.Should().Be("MyCustomers");
        }

        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnEntitySetElementTypeName()
        {
            TypeContextWithoutModel.NavigationSourceEntityTypeName.Should().Be("ns.MyCustomer");
        }

        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnExpectedEntityTypeName()
        {
            TypeContextWithoutModel.ExpectedResourceTypeName.Should().Be("ns.MyVipCustomer");
        }

        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnFalseForIsMediaLinkEntry()
        {
            TypeContextWithoutModel.IsMediaLinkEntry.Should().BeFalse();
        }
        #endregion TypeContextWithoutModel

        #region TypeContextWithModel
        [Fact]
        public void TypeContextWithMetadataShouldReturnEntitySetName()
        {
            TypeContextWithModel.NavigationSourceName.Should().Be("Customers");
        }

        [Fact]
        public void TypeContextWithMetadataShouldReturnEntitySetElementTypeName()
        {
            TypeContextWithModel.NavigationSourceEntityTypeName.Should().Be("ns.Customer");
        }

        [Fact]
        public void TypeContextWithMetadataShouldReturnExpectedEntityTypeName()
        {
            TypeContextWithModel.ExpectedResourceTypeName.Should().Be("ns.VipCustomer");
        }

        [Fact]
        public void TypeContextWithMetadataShouldReturnIsMediaLinkEntry()
        {
            TypeContextWithModel.IsMediaLinkEntry.Should().BeFalse();
        }
        #endregion TypeContextWithModel

        #region BaseTypeContextThatThrows
        [Fact]
        public void BaseTypeContextThatThrowsShouldThrowForEntitySetName()
        {
            Action test = () => BaseTypeContextThatThrows.NavigationSourceName.Should().BeNull();
            test.ShouldThrow<ODataException>(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldThrowForEntitySetElementTypeName()
        {
            Action test = () => BaseTypeContextThatThrows.NavigationSourceEntityTypeName.Should().BeNull();
            test.ShouldThrow<ODataException>(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldReturnNullExpectedEntityTypeName()
        {
            BaseTypeContextThatThrows.ExpectedResourceTypeName.Should().BeNull();
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldReturnFalseIsMediaLinkEntry()
        {
            BaseTypeContextThatThrows.IsMediaLinkEntry.Should().BeFalse();
        }
        #endregion BaseTypeContextThatThrows

        #region BaseTypeContextThatWillNotThrow
        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullEntitySetName()
        {
            BaseTypeContextThatWillNotThrow.NavigationSourceName.Should().BeNull();
        }

        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullEntitySetElementTypeName()
        {
            BaseTypeContextThatWillNotThrow.NavigationSourceEntityTypeName.Should().BeNull();
        }

        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullExpectedEntityTypeName()
        {
            BaseTypeContextThatWillNotThrow.ExpectedResourceTypeName.Should().BeNull();
        }

        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnFalseIsMediaLinkEntry()
        {
            BaseTypeContextThatWillNotThrow.IsMediaLinkEntry.Should().BeFalse();
        }
        #endregion BaseTypeContextThatWillNotThrow

        #region TypeContextWithEdmUnknowEntitySet
        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnNullEntitySetName()
        {
            TypeContextWithEdmUnknowEntitySet.NavigationSourceName.Should().Be(null);
        }

        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnEntitySetElementTypeName()
        {
            TypeContextWithEdmUnknowEntitySet.NavigationSourceEntityTypeName.Should().Be("ns.MyCustomer");
        }

        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnExpectedEntityTypeName()
        {
            TypeContextWithEdmUnknowEntitySet.ExpectedResourceTypeName.Should().Be("ns.MyVipCustomer");
        }

        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnFalseForIsMediaLinkEntry()
        {
            TypeContextWithEdmUnknowEntitySet.IsMediaLinkEntry.Should().BeFalse();
        }
        #endregion TypeContextWithEdmUnknowEntitySet
    }
}
