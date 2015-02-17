//---------------------------------------------------------------------
// <copyright file="ODataOperationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.Test.OData.TDD.Tests.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataOperationTests
    {
        private const string OperationName = "MyOperation";
        private static readonly Uri ServiceUri = new Uri("http://service/", UriKind.Absolute);
        private static readonly Uri MetadataDocumentUri = new Uri(ServiceUri, "$metadata");
        private static readonly Uri ContextUri = new Uri(MetadataDocumentUri, "#" + OperationName);
        private static readonly Uri Target = new Uri("http://service/" + OperationName);
        private TestODataOperation testSubject;
        private TestODataOperation operationWithFullBuilder;

        private class TestODataOperation : ODataOperation
        {
        }

        [TestInitialize]
        public void InitTest()
        {
            this.testSubject = new TestODataOperation();

            var entry = new ODataEntry
            {
                TypeName = "ns.DerivedType",
                Properties = new[]
                {
                    new ODataProperty{Name = "Id", Value = 1, SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.Key}},
                    new ODataProperty{Name = "Name", Value = "Bob", SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.ETag}}
                }
            };

            var serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.BaseType", ExpectedTypeName = "ns.BaseType" };
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo, null, null, null, EdmCoreModel.Instance, true);
            var metadataContext = new TestMetadataContext();
            var entryMetadataContext = ODataEntryMetadataContext.Create(entry, typeContext, serializationInfo, null, metadataContext, SelectedPropertiesNode.EntireSubtree);
            var fullMetadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext, new ODataConventionalUriBuilder(ServiceUri, UrlConvention.CreateWithExplicitValue(false)));
            this.operationWithFullBuilder = new TestODataOperation { Metadata = ContextUri};
            this.operationWithFullBuilder.SetMetadataBuilder(fullMetadataBuilder, MetadataDocumentUri);
        }

        [TestMethod]
        public void NewODataOperationShouldHaveNullContextUri()
        {
            this.testSubject.Metadata.Should().BeNull();
        }

        [TestMethod]
        public void NewODataOperationShouldHaveNullTitle()
        {
            this.testSubject.Title.Should().BeNull();
        }

        [TestMethod]
        public void NewODataOperationShouldHaveNullTarget()
        {
            this.testSubject.Target.Should().BeNull();
        }

        [TestMethod]
        public void ShouldBeAbleToSetContextUriToODataOperation()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.Metadata.Should().BeSameAs(ContextUri);
        }

        [TestMethod]
        public void ShouldBeAbleToSetTitleToODataOperation()
        {
            this.testSubject.Title = OperationName;
            this.testSubject.Title.Should().Be(OperationName);
        }

        [TestMethod]
        public void ShouldBeAbleToSetTargetToODataOperation()
        {
            this.testSubject.Target = Target;
            this.testSubject.Target.Should().BeSameAs(Target);
        }

        [TestMethod]
        public void MetadataBuilderShouldNotAffectUserAssignedContextUri()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Metadata.Should().BeSameAs(ContextUri);
        }

        [TestMethod]
        public void MetadataBuilderShouldNotAffectUserAssignedTitle()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.Title = OperationName;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Title.Should().Be(OperationName);
        }

        [TestMethod]
        public void MetadataBuilderShouldNotAffectUserAssignedTarget()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.Target = Target;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Target.Should().BeSameAs(Target);
        }

        [TestMethod]
        public void SetMetadataBuilderShouldThrowWhemContextUriIsNotSet()
        {
            Action test = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(this.testSubject.GetType().Name));
        }

        [TestMethod]
        public void GetMetadataBuilderShouldReturnCurrentBuilder()
        {
            this.testSubject.GetMetadataBuilder().Should().BeNull();
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.GetMetadataBuilder().Should().BeSameAs(ODataEntityMetadataBuilder.Null);
        }

        [TestMethod]
        public void ChangingMetadataBuilderShouldUpdateCalculatedTitle()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataEntry()), MetadataDocumentUri);
            this.testSubject.Title.Should().Be("MyOperation");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Title.Should().BeNull();
        }

        [TestMethod]
        public void ChangingMetadataBuilderShouldUpdateCalculatedTarget()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataEntry()), MetadataDocumentUri);
            this.testSubject.Target.OriginalString.Should().Be("http://service/ComputedTargetUri/MyOperation()");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Target.Should().BeNull();
        }

        [TestMethod]
        public void SettingTitleToNullShouldOverrideTitleFromFullBuilder()
        {
            this.operationWithFullBuilder.Title.Should().Be("MyOperation");
            this.operationWithFullBuilder.Title = null;
            this.operationWithFullBuilder.Title.Should().BeNull();
        }

        [TestMethod]
        public void SettingTargetToNullShouldOverrideTargetFromFullBuilder()
        {
            this.operationWithFullBuilder.Target.OriginalString.Should().Be("http://service/Set(1)/ns.DerivedType/MyOperation");
            this.operationWithFullBuilder.Target = null;
            this.operationWithFullBuilder.Target.Should().BeNull();
        }

        [TestMethod]
        public void SetMetadataBuildShouldThrowWhenMetadataIsNull()
        {
            Action test = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            test.ShouldThrow<ODataException>(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(this.testSubject.GetType().Name));
        }

        [TestMethod]
        public void SetMetadataBuilderShouldNotThrowWhenMetadataIsRelativeAndStartsWithHash()
        {
            this.testSubject.Metadata = new Uri("#Action1", UriKind.Relative);
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
        }

        [TestMethod]
        public void SetMetadataBuilderShouldThrowWhenNameIsOpenMetadataReferenceProperty()
        {
            this.testSubject.Metadata = new Uri("http://www.example.com/$metadata#foo");
            Action test = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/$metadata#foo", MetadataDocumentUri.AbsoluteUri));
        }

        [TestMethod]
        public void SetMetadataBuilderShouldNotThrowWhenNameIsMetadataDocumentUriWithHashThenValidIdentifier()
        {
            this.testSubject.Metadata = new Uri(MetadataDocumentUri, "#Action1");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
        }

        [TestMethod]
        public void SetMetadataBuilderShouldThrowWhenNameIsWithoutHash()
        {
            this.testSubject.Metadata = new Uri("Action1", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidMetadataReferenceProperty("Action1"));
        }

        [TestMethod]
        public void SetMetadataBuilderShouldThrowWhenNameIsAbsoluteUriWithoutHash()
        {
            this.testSubject.Metadata = new Uri("http://www.example.com/Action1");
            Action action = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidMetadataReferenceProperty("http://www.example.com/Action1"));
        }

        [TestMethod]
        public void SetMetadataBuilderShouldThrowWhenNameIsIdentifierHashIdentifier()
        {
            this.testSubject.Metadata = new Uri("Action#1", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidMetadataReferenceProperty("Action#1"));
        }

        [TestMethod]
        public void SetMetadataBuilderShouldThrowWhenNameIsJustHash()
        {
            this.testSubject.Metadata = new Uri("#", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidMetadataReferenceProperty("#"));
        }

        [TestMethod]
        public void SetMetadataBuilderShouldNotThrowWhenNameHasTwoHashes()
        {
            this.testSubject.Metadata = new Uri("#Action#1", UriKind.Relative);
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
        }
    }
}
