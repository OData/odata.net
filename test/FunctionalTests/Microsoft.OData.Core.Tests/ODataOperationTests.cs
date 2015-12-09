//---------------------------------------------------------------------
// <copyright file="ODataOperationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Core.Tests.Evaluation;
using Microsoft.OData.Core.Tests.JsonLight;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
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

        public ODataOperationTests()
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

        [Fact]
        public void NewODataOperationShouldHaveNullContextUri()
        {
            this.testSubject.Metadata.Should().BeNull();
        }

        [Fact]
        public void NewODataOperationShouldHaveNullTitle()
        {
            this.testSubject.Title.Should().BeNull();
        }

        [Fact]
        public void NewODataOperationShouldHaveNullTarget()
        {
            this.testSubject.Target.Should().BeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetContextUriToODataOperation()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.Metadata.Should().BeSameAs(ContextUri);
        }

        [Fact]
        public void ShouldBeAbleToSetTitleToODataOperation()
        {
            this.testSubject.Title = OperationName;
            this.testSubject.Title.Should().Be(OperationName);
        }

        [Fact]
        public void ShouldBeAbleToSetTargetToODataOperation()
        {
            this.testSubject.Target = Target;
            this.testSubject.Target.Should().BeSameAs(Target);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedContextUri()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Metadata.Should().BeSameAs(ContextUri);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedTitle()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.Title = OperationName;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Title.Should().Be(OperationName);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedTarget()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.Target = Target;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Target.Should().BeSameAs(Target);
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhemContextUriIsNotSet()
        {
            Action test = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(this.testSubject.GetType().Name));
        }

        [Fact]
        public void GetMetadataBuilderShouldReturnCurrentBuilder()
        {
            this.testSubject.GetMetadataBuilder().Should().BeNull();
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.GetMetadataBuilder().Should().BeSameAs(ODataEntityMetadataBuilder.Null);
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedTitle()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataEntry()), MetadataDocumentUri);
            this.testSubject.Title.Should().Be("MyOperation");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Title.Should().BeNull();
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedTarget()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataEntry()), MetadataDocumentUri);
            this.testSubject.Target.OriginalString.Should().Be("http://service/ComputedTargetUri/MyOperation()");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            this.testSubject.Target.Should().BeNull();
        }

        [Fact]
        public void SettingTitleToNullShouldOverrideTitleFromFullBuilder()
        {
            this.operationWithFullBuilder.Title.Should().Be("MyOperation");
            this.operationWithFullBuilder.Title = null;
            this.operationWithFullBuilder.Title.Should().BeNull();
        }

        [Fact]
        public void SettingTargetToNullShouldOverrideTargetFromFullBuilder()
        {
            this.operationWithFullBuilder.Target.OriginalString.Should().Be("http://service/Set(1)/ns.DerivedType/MyOperation");
            this.operationWithFullBuilder.Target = null;
            this.operationWithFullBuilder.Target.Should().BeNull();
        }

        [Fact]
        public void SetMetadataBuildShouldThrowWhenMetadataIsNull()
        {
            Action test = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            test.ShouldThrow<ODataException>(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(this.testSubject.GetType().Name));
        }

        [Fact]
        public void SetMetadataBuilderShouldNotThrowWhenMetadataIsRelativeAndStartsWithHash()
        {
            this.testSubject.Metadata = new Uri("#Action1", UriKind.Relative);
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsOpenMetadataReferenceProperty()
        {
            this.testSubject.Metadata = new Uri("http://www.example.com/$metadata#foo");
            Action test = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/$metadata#foo", MetadataDocumentUri.AbsoluteUri));
        }

        [Fact]
        public void SetMetadataBuilderShouldNotThrowWhenNameIsMetadataDocumentUriWithHashThenValidIdentifier()
        {
            this.testSubject.Metadata = new Uri(MetadataDocumentUri, "#Action1");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsWithoutHash()
        {
            this.testSubject.Metadata = new Uri("Action1", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidMetadataReferenceProperty("Action1"));
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsAbsoluteUriWithoutHash()
        {
            this.testSubject.Metadata = new Uri("http://www.example.com/Action1");
            Action action = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidMetadataReferenceProperty("http://www.example.com/Action1"));
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsIdentifierHashIdentifier()
        {
            this.testSubject.Metadata = new Uri("Action#1", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidMetadataReferenceProperty("Action#1"));
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsJustHash()
        {
            this.testSubject.Metadata = new Uri("#", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidMetadataReferenceProperty("#"));
        }

        [Fact]
        public void SetMetadataBuilderShouldNotThrowWhenNameHasTwoHashes()
        {
            this.testSubject.Metadata = new Uri("#Action#1", UriKind.Relative);
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, MetadataDocumentUri);
        }
    }
}
