//---------------------------------------------------------------------
// <copyright file="ODataResourceTypeContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
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
            Assert.NotNull(typeContext);
            Assert.EndsWith("WithoutModel", typeContext.GetType().Name);
        }

        [Fact]
        public void ShouldCreateSubclassWithModelWhenMetadataIsGiven()
        {
            var typeContext = ODataResourceTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, throwIfMissingTypeInfo: true);
            Assert.NotNull(typeContext);
            Assert.EndsWith("WithModel", typeContext.GetType().Name);
        }

        [Fact]
        public void ShouldCreateSubclassWithModelWhenExpectedTypeisGiven()
        {
            var typeContext = ODataResourceTypeContext.Create(null, navigationSource: null,
                navigationSourceEntityType: null, expectedResourceType: ComplexType, throwIfMissingTypeInfo: false);
            Assert.NotNull(typeContext);
            Assert.EndsWith("WithModel", typeContext.GetType().Name);
        }

        [Fact]
        public void ShouldCreateSubclassWithoutModelWhenBothSerializationInfoAndMetadataAreGiven()
        {
            var typeContext = ODataResourceTypeContext.Create(SerializationInfo, EntitySet, EntitySetElementType, ExpectedEntityType, throwIfMissingTypeInfo: true);
            Assert.NotNull(typeContext);
            Assert.EndsWith("WithoutModel", typeContext.GetType().Name);
        }

        [Fact]
        public void ShouldCreateBaseClassWhenSerializationInfoAndUserModelAreBothMissingAndThrowIfMissingTypeInfoIsTrue()
        {
            var typeContext = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: true);
            Assert.IsType<ODataResourceTypeContext>(typeContext);
        }

        [Fact]
        public void ShouldCreateBaseClassWhenSerializationInfoAndUserModelAreBothMissingAndThrowIfMissingTypeInfoIsFalse()
        {
            var typeContext = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: false);
            Assert.IsType<ODataResourceTypeContext>(typeContext);
        }

        #region TypeContextWithoutModel
        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnEntitySetName()
        {
            Assert.Equal("MyCustomers", TypeContextWithoutModel.NavigationSourceName);
        }

        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnEntitySetElementTypeName()
        {
            Assert.Equal("ns.MyCustomer", TypeContextWithoutModel.NavigationSourceEntityTypeName);
        }

        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnExpectedEntityTypeName()
        {
            Assert.Equal("ns.MyVipCustomer", TypeContextWithoutModel.ExpectedResourceTypeName);
        }

        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnFalseForIsMediaLinkEntry()
        {
            Assert.False(TypeContextWithoutModel.IsMediaLinkEntry);
        }
        #endregion TypeContextWithoutModel

        #region TypeContextWithModel
        [Fact]
        public void TypeContextWithMetadataShouldReturnEntitySetName()
        {
            Assert.Equal("Customers", TypeContextWithModel.NavigationSourceName);
        }

        [Fact]
        public void TypeContextWithMetadataShouldReturnEntitySetElementTypeName()
        {
            Assert.Equal("ns.Customer", TypeContextWithModel.NavigationSourceEntityTypeName);
        }

        [Fact]
        public void TypeContextWithMetadataShouldReturnExpectedEntityTypeName()
        {
            Assert.Equal("ns.VipCustomer", TypeContextWithModel.ExpectedResourceTypeName);
        }

        [Fact]
        public void TypeContextWithMetadataShouldReturnIsMediaLinkEntry()
        {
            Assert.False(TypeContextWithModel.IsMediaLinkEntry);
        }
        #endregion TypeContextWithModel

        #region BaseTypeContextThatThrows
        [Fact]
        public void BaseTypeContextThatThrowsShouldThrowForEntitySetName()
        {
            Action test = () => Assert.Null(BaseTypeContextThatThrows.NavigationSourceName);
            test.Throws<ODataException>(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldThrowForEntitySetElementTypeName()
        {
            Action test = () => Assert.Null(BaseTypeContextThatThrows.NavigationSourceEntityTypeName);
            test.Throws<ODataException>(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldReturnNullExpectedEntityTypeName()
        {
            Assert.Null(BaseTypeContextThatThrows.ExpectedResourceTypeName);
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldReturnFalseIsMediaLinkEntry()
        {
            Assert.False(BaseTypeContextThatThrows.IsMediaLinkEntry);
        }
        #endregion BaseTypeContextThatThrows

        #region BaseTypeContextThatWillNotThrow
        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullEntitySetName()
        {
            Assert.Null(BaseTypeContextThatWillNotThrow.NavigationSourceName);
        }

        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullEntitySetElementTypeName()
        {
            Assert.Null(BaseTypeContextThatWillNotThrow.NavigationSourceEntityTypeName);
        }

        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullExpectedEntityTypeName()
        {
            Assert.Null(BaseTypeContextThatWillNotThrow.ExpectedResourceTypeName);
        }

        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnFalseIsMediaLinkEntry()
        {
            Assert.False(BaseTypeContextThatWillNotThrow.IsMediaLinkEntry);
        }
        #endregion BaseTypeContextThatWillNotThrow

        #region TypeContextWithEdmUnknowEntitySet
        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnNullEntitySetName()
        {
            Assert.Null(TypeContextWithEdmUnknowEntitySet.NavigationSourceName);
        }

        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnEntitySetElementTypeName()
        {
            Assert.Equal("ns.MyCustomer", TypeContextWithEdmUnknowEntitySet.NavigationSourceEntityTypeName);
        }

        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnExpectedEntityTypeName()
        {
            Assert.Equal("ns.MyVipCustomer", TypeContextWithEdmUnknowEntitySet.ExpectedResourceTypeName);
        }

        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnFalseForIsMediaLinkEntry()
        {
            Assert.False(TypeContextWithEdmUnknowEntitySet.IsMediaLinkEntry);
        }
        #endregion TypeContextWithEdmUnknowEntitySet
    }
}
