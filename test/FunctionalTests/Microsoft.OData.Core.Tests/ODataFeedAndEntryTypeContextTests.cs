//---------------------------------------------------------------------
// <copyright file="ODataFeedAndEntryTypeContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Annotations;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataFeedAndEntryTypeContextTests
    {
        private static readonly ODataFeedAndEntryTypeContext TypeContextWithoutModel;
        private static readonly ODataFeedAndEntryTypeContext TypeContextWithModel;
        private static readonly ODataFeedAndEntryTypeContext BaseTypeContextThatThrows;
        private static readonly ODataFeedAndEntryTypeContext BaseTypeContextThatWillNotThrow;
        private static readonly ODataFeedAndEntryTypeContext TypeContextWithEdmUnknowEntitySet;
        private static readonly ODataFeedAndEntrySerializationInfo SerializationInfo;
        private static readonly ODataFeedAndEntrySerializationInfo SerializationInfoWithEdmUnknowEntitySet;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmEntityType EntitySetElementType;
        private static readonly EdmEntityType ExpectedEntityType;
        private static readonly EdmEntityType ActualEntityType;
        private static readonly EdmModel Model;

        static ODataFeedAndEntryTypeContextTests()
        {
            Model = new EdmModel();
            EntitySetElementType = new EdmEntityType("ns", "Customer");
            ExpectedEntityType = new EdmEntityType("ns", "VipCustomer", EntitySetElementType);
            ActualEntityType = new EdmEntityType("ns", "DerivedVipCustomer", ExpectedEntityType);

            EdmEntityContainer defaultContainer = new EdmEntityContainer("ns", "DefaultContainer");
            Model.AddElement(defaultContainer);
            Model.AddVocabularyAnnotation(new EdmAnnotation(defaultContainer, UrlConventionsConstants.ConventionTerm, UrlConventionsConstants.KeyAsSegmentAnnotationValue));

            EntitySet = new EdmEntitySet(defaultContainer, "Customers", EntitySetElementType);
            Model.AddElement(EntitySetElementType);
            Model.AddElement(ExpectedEntityType);
            Model.AddElement(ActualEntityType);
            defaultContainer.AddElement(EntitySet);

            SerializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "MyCustomers", NavigationSourceEntityTypeName = "ns.MyCustomer", ExpectedTypeName = "ns.MyVipCustomer" };
            SerializationInfoWithEdmUnknowEntitySet = new ODataFeedAndEntrySerializationInfo() { NavigationSourceName = null, NavigationSourceEntityTypeName = "ns.MyCustomer", ExpectedTypeName = "ns.MyVipCustomer", NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet };
            TypeContextWithoutModel = ODataFeedAndEntryTypeContext.Create(SerializationInfo, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: Model, throwIfMissingTypeInfo: true);
            TypeContextWithModel = ODataFeedAndEntryTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, Model, throwIfMissingTypeInfo: true);
            TypeContextWithEdmUnknowEntitySet = ODataFeedAndEntryTypeContext.Create(SerializationInfoWithEdmUnknowEntitySet, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: Model, throwIfMissingTypeInfo: true);
            BaseTypeContextThatThrows = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: Model, throwIfMissingTypeInfo: true);
            BaseTypeContextThatWillNotThrow = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: Model, throwIfMissingTypeInfo: false);
        }

        [Fact]
        public void ShouldCreateSubclassWithoutModelWhenSerializationInfoIsGiven()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(SerializationInfo, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: EdmCoreModel.Instance, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithoutModel").Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateSubclassWithModelWhenMetadataIsGiven()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, Model, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithModel").Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateBaseClassWhenEntitySetIsGivenButModelIsNotUserModel()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, EdmCoreModel.Instance, throwIfMissingTypeInfo: true);
            typeContext.Should().BeOfType<ODataFeedAndEntryTypeContext>();
        }

        [Fact]
        public void ShouldCreateSubclassWithoutModelWhenBothSerializationInfoAndMetadataAreGiven()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(SerializationInfo, EntitySet, EntitySetElementType, ExpectedEntityType, Model, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithoutModel").Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateBaseClassWhenSerializationInfoAndUserModelAreBothMissingAndThrowIfMissingTypeInfoIsTrue()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: EdmCoreModel.Instance, throwIfMissingTypeInfo: true);
            typeContext.Should().BeOfType<ODataFeedAndEntryTypeContext>();
        }

        [Fact]
        public void ShouldCreateBaseClassWhenSerializationInfoAndUserModelAreBothMissingAndThrowIfMissingTypeInfoIsFalse()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: EdmCoreModel.Instance, throwIfMissingTypeInfo: false);
            typeContext.Should().BeOfType<ODataFeedAndEntryTypeContext>();
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
            TypeContextWithoutModel.ExpectedEntityTypeName.Should().Be("ns.MyVipCustomer");
        }

        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnFalseForIsMediaLinkEntry()
        {
            TypeContextWithoutModel.IsMediaLinkEntry.Should().BeFalse();
        }

        [Fact]
        public void TypeContextWithSerializationInfoShouldReturnDefaultUrlConvention()
        {
            TypeContextWithoutModel.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
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
            TypeContextWithModel.ExpectedEntityTypeName.Should().Be("ns.VipCustomer");
        }

        [Fact]
        public void TypeContextWithMetadataShouldReturnIsMediaLinkEntry()
        {
            TypeContextWithModel.IsMediaLinkEntry.Should().BeFalse();
        }

        [Fact]
        public void TypeContextWithMetadataShouldReturnKeyAsSegmentUrlConvention()
        {
            TypeContextWithModel.UrlConvention.GenerateKeyAsSegment.Should().BeTrue();
        }
        #endregion TypeContextWithModel

        #region BaseTypeContextThatThrows
        [Fact]
        public void BaseTypeContextThatThrowsShouldThrowForEntitySetName()
        {
            Action test = () => BaseTypeContextThatThrows.NavigationSourceName.Should().BeNull();
            test.ShouldThrow<ODataException>(Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldThrowForEntitySetElementTypeName()
        {
            Action test = () => BaseTypeContextThatThrows.NavigationSourceEntityTypeName.Should().BeNull();
            test.ShouldThrow<ODataException>(Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldReturnNullExpectedEntityTypeName()
        {
            BaseTypeContextThatThrows.ExpectedEntityTypeName.Should().BeNull();
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldReturnFalseIsMediaLinkEntry()
        {
            BaseTypeContextThatThrows.IsMediaLinkEntry.Should().BeFalse();
        }

        [Fact]
        public void BaseTypeContextThatThrowsShouldReturnDefaultUrlConvention()
        {
            BaseTypeContextThatThrows.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
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
            BaseTypeContextThatWillNotThrow.ExpectedEntityTypeName.Should().BeNull();
        }

        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnFalseIsMediaLinkEntry()
        {
            BaseTypeContextThatWillNotThrow.IsMediaLinkEntry.Should().BeFalse();
        }

        [Fact]
        public void BaseTypeContextThatWillNotThrowShouldReturnDefaultUrlConvention()
        {
            BaseTypeContextThatWillNotThrow.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
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
            TypeContextWithEdmUnknowEntitySet.ExpectedEntityTypeName.Should().Be("ns.MyVipCustomer");
        }

        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnFalseForIsMediaLinkEntry()
        {
            TypeContextWithEdmUnknowEntitySet.IsMediaLinkEntry.Should().BeFalse();
        }

        [Fact]
        public void TypeContextWithEdmUnknowEntitySetShouldReturnDefaultUrlConvention()
        {
            TypeContextWithEdmUnknowEntitySet.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
        }
        #endregion TypeContextWithEdmUnknowEntitySet
    }
}
