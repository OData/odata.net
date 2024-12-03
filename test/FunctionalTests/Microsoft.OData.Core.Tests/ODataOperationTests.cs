//---------------------------------------------------------------------
// <copyright file="ODataOperationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Tests.Evaluation;
using Microsoft.OData.Tests.Json;
using Xunit;

namespace Microsoft.OData.Tests
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

            var entry = new ODataResource
            {
                TypeName = "ns.DerivedType",
                Properties = new[]
                {
                    new ODataProperty{Name = "Id", Value = 1, SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.Key}},
                    new ODataProperty{Name = "Name", Value = "Bob", SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.ETag}}
                }
            };

            var serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.BaseType", ExpectedTypeName = "ns.BaseType" };
            var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null);
            var metadataContext = new TestMetadataContext();
            var entryMetadataContext = ODataResourceMetadataContext.Create(entry, typeContext, serializationInfo, null, metadataContext, new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree), null);
            var fullMetadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext,
                new ODataConventionalUriBuilder(ServiceUri, ODataUrlKeyDelimiter.Parentheses));
            this.operationWithFullBuilder = new TestODataOperation { Metadata = ContextUri };
            this.operationWithFullBuilder.SetMetadataBuilder(fullMetadataBuilder, MetadataDocumentUri);
        }

        [Fact]
        public void NewODataOperationShouldHaveNullContextUri()
        {
            Assert.Null(this.testSubject.Metadata);
        }

        [Fact]
        public void NewODataOperationShouldHaveNullTitle()
        {
            Assert.Null(this.testSubject.Title);
        }

        [Fact]
        public void NewODataOperationShouldHaveNullTarget()
        {
            Assert.Null(this.testSubject.Target);
        }

        [Fact]
        public void ShouldBeAbleToSetContextUriToODataOperation()
        {
            this.testSubject.Metadata = ContextUri;
            Assert.Same(ContextUri, this.testSubject.Metadata);
        }

        [Fact]
        public void ShouldBeAbleToSetTitleToODataOperation()
        {
            this.testSubject.Title = OperationName;
            Assert.Equal(OperationName, this.testSubject.Title);
        }

        [Fact]
        public void ShouldBeAbleToSetTargetToODataOperation()
        {
            this.testSubject.Target = Target;
            Assert.Same(Target, this.testSubject.Target);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedContextUri()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            Assert.Same(ContextUri, this.testSubject.Metadata);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedTitle()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.Title = OperationName;
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            Assert.Equal(OperationName, this.testSubject.Title);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedTarget()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.Target = Target;
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            Assert.Same(Target, this.testSubject.Target);
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhemContextUriIsNotSet()
        {
            Action test = () => this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            test.Throws<ODataException>(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(this.testSubject.GetType().Name));
        }

        [Fact]
        public void GetMetadataBuilderShouldReturnCurrentBuilder()
        {
            Assert.Null(this.testSubject.GetMetadataBuilder());
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            Assert.Same(ODataResourceMetadataBuilder.Null, this.testSubject.GetMetadataBuilder());
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedTitle()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataResource()), MetadataDocumentUri);
            Assert.Equal("MyOperation", this.testSubject.Title);
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            Assert.Null(this.testSubject.Title);
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedTarget()
        {
            this.testSubject.Metadata = ContextUri;
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataResource()), MetadataDocumentUri);
            Assert.Equal("http://service/ComputedTargetUri/MyOperation()", this.testSubject.Target.OriginalString);
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            Assert.Null(this.testSubject.Target);
        }

        [Fact]
        public void SettingTitleToNullShouldOverrideTitleFromFullBuilder()
        {
            Assert.Equal("MyOperation", this.operationWithFullBuilder.Title);
            this.operationWithFullBuilder.Title = null;
            Assert.Null(this.operationWithFullBuilder.Title);
        }

        [Fact]
        public void SettingTargetToNullShouldOverrideTargetFromFullBuilder()
        {
            Assert.Equal("http://service/Set(1)/ns.DerivedType/MyOperation", this.operationWithFullBuilder.Target.OriginalString);
            this.operationWithFullBuilder.Target = null;
            Assert.Null(this.operationWithFullBuilder.Target);
        }

        [Fact]
        public void SetMetadataBuildShouldThrowWhenMetadataIsNull()
        {
            Action test = () => this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            test.Throws<ODataException>(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(this.testSubject.GetType().Name));
        }

        [Fact]
        public void SetMetadataBuilderShouldNotThrowWhenMetadataIsRelativeAndStartsWithHash()
        {
            this.testSubject.Metadata = new Uri("#Action1", UriKind.Relative);
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsOpenMetadataReferenceProperty()
        {
            this.testSubject.Metadata = new Uri("http://www.example.com/$metadata#foo");
            Action test = () => this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            test.Throws<ODataException>(Strings.ODataJsonValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/$metadata#foo", MetadataDocumentUri.AbsoluteUri));
        }

        [Fact]
        public void SetMetadataBuilderShouldNotThrowWhenNameIsMetadataDocumentUriWithHashThenValidIdentifier()
        {
            this.testSubject.Metadata = new Uri(MetadataDocumentUri, "#Action1");
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsWithoutHash()
        {
            this.testSubject.Metadata = new Uri("Action1", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            action.Throws<ODataException>(Strings.ValidationUtils_InvalidMetadataReferenceProperty("Action1"));
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsAbsoluteUriWithoutHash()
        {
            this.testSubject.Metadata = new Uri("http://www.example.com/Action1");
            Action action = () => this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            action.Throws<ODataException>(Strings.ValidationUtils_InvalidMetadataReferenceProperty("http://www.example.com/Action1"));
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsIdentifierHashIdentifier()
        {
            this.testSubject.Metadata = new Uri("Action#1", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            action.Throws<ODataException>(Strings.ValidationUtils_InvalidMetadataReferenceProperty("Action#1"));
        }

        [Fact]
        public void SetMetadataBuilderShouldThrowWhenNameIsJustHash()
        {
            this.testSubject.Metadata = new Uri("#", UriKind.Relative);
            Action action = () => this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
            action.Throws<ODataException>(Strings.ValidationUtils_InvalidMetadataReferenceProperty("#"));
        }

        [Fact]
        public void SetMetadataBuilderShouldNotThrowWhenNameHasTwoHashes()
        {
            this.testSubject.Metadata = new Uri("#Action#1", UriKind.Relative);
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, MetadataDocumentUri);
        }
    }
}
