//---------------------------------------------------------------------
// <copyright file="ODataFeedAndEntryTypeContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer
{
    using System;
    using AstoriaUnitTests.TDD.Common;
    using FluentAssertions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Annotations;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataFeedAndEntryTypeContextTests
    {
        private static readonly ODataFeedAndEntryTypeContext TypeContextWithoutModel;
        private static readonly ODataFeedAndEntryTypeContext TypeContextWithModel;
        private static readonly ODataFeedAndEntryTypeContext BaseTypeContextThatThrows;
        private static readonly ODataFeedAndEntryTypeContext BaseTypeContextThatWillNotThrow;
        private static readonly ODataFeedAndEntrySerializationInfo SerializationInfo;
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

            SerializationInfo = new ODataFeedAndEntrySerializationInfo {NavigationSourceName = "MyCustomers", NavigationSourceEntityTypeName = "ns.MyCustomer", ExpectedTypeName = "ns.MyVipCustomer"};
            TypeContextWithoutModel = ODataFeedAndEntryTypeContext.Create(SerializationInfo, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: Model, throwIfMissingTypeInfo: true);
            TypeContextWithModel = ODataFeedAndEntryTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, Model, throwIfMissingTypeInfo: true);
            BaseTypeContextThatThrows = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: Model, throwIfMissingTypeInfo: true);
            BaseTypeContextThatWillNotThrow = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: Model, throwIfMissingTypeInfo: false);
        }

        [TestMethod]
        public void ShouldCreateSubclassWithoutModelWhenSerializationInfoIsGiven()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(SerializationInfo, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: EdmCoreModel.Instance, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithoutModel").Should().BeTrue();
        }

        [TestMethod]
        public void ShouldCreateSubclassWithModelWhenMetadataIsGiven()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, Model, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithModel").Should().BeTrue();
        }

        [TestMethod]
        public void ShouldCreateBaseClassWhenEntitySetIsGivenButModelIsNotUserModel()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(/*serializationInfo*/null, EntitySet, EntitySetElementType, ExpectedEntityType, EdmCoreModel.Instance, throwIfMissingTypeInfo: true);
            typeContext.Should().BeOfType<ODataFeedAndEntryTypeContext>();
        }

        [TestMethod]
        public void ShouldCreateSubclassWithoutModelWhenBothSerializationInfoAndMetadataAreGiven()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(SerializationInfo, EntitySet, EntitySetElementType, ExpectedEntityType, Model, throwIfMissingTypeInfo: true);
            typeContext.Should().NotBeNull();
            typeContext.GetType().Name.EndsWith("WithoutModel").Should().BeTrue();
        }

        [TestMethod]
        public void ShouldCreateBaseClassWhenSerializationInfoAndUserModelAreBothMissingAndThrowIfMissingTypeInfoIsTrue()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: EdmCoreModel.Instance, throwIfMissingTypeInfo: true);
            typeContext.Should().BeOfType<ODataFeedAndEntryTypeContext>();
        }

        [TestMethod]
        public void ShouldCreateBaseClassWhenSerializationInfoAndUserModelAreBothMissingAndThrowIfMissingTypeInfoIsFalse()
        {
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: EdmCoreModel.Instance, throwIfMissingTypeInfo: false);
            typeContext.Should().BeOfType<ODataFeedAndEntryTypeContext>();
        }

        #region TypeContextWithoutModel
        [TestMethod]
        public void TypeContextWithSerializationInfoShouldReturnEntitySetName()
        {
            TypeContextWithoutModel.NavigationSourceName.Should().Be("MyCustomers");
        }

        [TestMethod]
        public void TypeContextWithSerializationInfoShouldReturnEntitySetElementTypeName()
        {
            TypeContextWithoutModel.NavigationSourceEntityTypeName.Should().Be("ns.MyCustomer");
        }

        [TestMethod]
        public void TypeContextWithSerializationInfoShouldReturnExpectedEntityTypeName()
        {
            TypeContextWithoutModel.ExpectedEntityTypeName.Should().Be("ns.MyVipCustomer");
        }

        [TestMethod]
        public void TypeContextWithSerializationInfoShouldReturnFalseForIsMediaLinkEntry()
        {
            TypeContextWithoutModel.IsMediaLinkEntry.Should().BeFalse();
        }

        [TestMethod]
        public void TypeContextWithSerializationInfoShouldReturnDefaultUrlConvention()
        {
            TypeContextWithoutModel.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
        }
        #endregion TypeContextWithoutModel

        #region TypeContextWithModel
        [TestMethod]
        public void TypeContextWithMetadataShouldReturnEntitySetName()
        {
            TypeContextWithModel.NavigationSourceName.Should().Be("Customers");
        }

        [TestMethod]
        public void TypeContextWithMetadataShouldReturnEntitySetElementTypeName()
        {
            TypeContextWithModel.NavigationSourceEntityTypeName.Should().Be("ns.Customer");
        }

        [TestMethod]
        public void TypeContextWithMetadataShouldReturnExpectedEntityTypeName()
        {
            TypeContextWithModel.ExpectedEntityTypeName.Should().Be("ns.VipCustomer");
        }

        [TestMethod]
        public void TypeContextWithMetadataShouldReturnIsMediaLinkEntry()
        {
            TypeContextWithModel.IsMediaLinkEntry.Should().BeFalse();
        }

        [TestMethod]
        public void TypeContextWithMetadataShouldReturnKeyAsSegmentUrlConvention()
        {
            TypeContextWithModel.UrlConvention.GenerateKeyAsSegment.Should().BeTrue();
        }
        #endregion TypeContextWithModel

        #region BaseTypeContextThatThrows
        [TestMethod]
        public void BaseTypeContextThatThrowsShouldThrowForEntitySetName()
        {
            Action test = () => BaseTypeContextThatThrows.NavigationSourceName.Should().BeNull();
            test.ShouldThrow<ODataException>(Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
        }

        [TestMethod]
        public void BaseTypeContextThatThrowsShouldThrowForEntitySetElementTypeName()
        {
            Action test = () => BaseTypeContextThatThrows.NavigationSourceEntityTypeName.Should().BeNull();
            test.ShouldThrow<ODataException>(Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
        }

        [TestMethod]
        public void BaseTypeContextThatThrowsShouldReturnNullExpectedEntityTypeName()
        {
            BaseTypeContextThatThrows.ExpectedEntityTypeName.Should().BeNull();
        }

        [TestMethod]
        public void BaseTypeContextThatThrowsShouldReturnFalseIsMediaLinkEntry()
        {
            BaseTypeContextThatThrows.IsMediaLinkEntry.Should().BeFalse();
        }

        [TestMethod]
        public void BaseTypeContextThatThrowsShouldReturnDefaultUrlConvention()
        {
            BaseTypeContextThatThrows.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
        }
        #endregion BaseTypeContextThatThrows

        #region BaseTypeContextThatWillNotThrow
        [TestMethod]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullEntitySetName()
        {
            BaseTypeContextThatWillNotThrow.NavigationSourceName.Should().BeNull();
        }

        [TestMethod]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullEntitySetElementTypeName()
        {
            BaseTypeContextThatWillNotThrow.NavigationSourceEntityTypeName.Should().BeNull();
        }

        [TestMethod]
        public void BaseTypeContextThatWillNotThrowShouldReturnNullExpectedEntityTypeName()
        {
            BaseTypeContextThatWillNotThrow.ExpectedEntityTypeName.Should().BeNull();
        }

        [TestMethod]
        public void BaseTypeContextThatWillNotThrowShouldReturnFalseIsMediaLinkEntry()
        {
            BaseTypeContextThatWillNotThrow.IsMediaLinkEntry.Should().BeFalse();
        }

        [TestMethod]
        public void BaseTypeContextThatWillNotThrowShouldReturnDefaultUrlConvention()
        {
            BaseTypeContextThatWillNotThrow.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
        }
        #endregion BaseTypeContextThatWillNotThrow
    }
}
