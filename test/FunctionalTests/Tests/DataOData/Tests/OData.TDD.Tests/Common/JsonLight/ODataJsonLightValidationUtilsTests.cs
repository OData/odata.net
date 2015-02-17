//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValidationUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common.JsonLight
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class ODataJsonLightValidationUtilsTests
    {
        private static Uri metadataDocumentUri = new Uri("http://www.myservice.svc/$metadata");

        [TestMethod]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameIsHashThenValidIdentifier()
        {
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#Action1");
        }
        
        [TestMethod]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsOpenMetadataReferenceProperty()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/$metadata#foo");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/$metadata#foo", metadataDocumentUri.AbsoluteUri));
        }
        
        [TestMethod]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameIsMetadataDocumentUriWithHashThenValidIdentifier()
        {
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, metadataDocumentUri.OriginalString + "#bar");
        }

        [TestMethod]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsWithoutHash()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "bar");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("bar"));
        }

        [TestMethod]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsAbsoluteUriWithoutHash()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foo");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("http://www.example.com/foo"));
        }

        [TestMethod]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsIdentifierHashIdentifier()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "foo#baz");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("foo#baz"));
        }

        [TestMethod]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsJustHash()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("#"));
        }

        [TestMethod]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameHasTwoHashes()
        {
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#foo#baz");
        }

        //ValidateOperation
        [TestMethod]
        public void IsOpenMetadataReferencePropertyNameShouldBeTrueWhenNameIsAbsoluteUriNotRelativeToMetadataDocumentUri()
        {
            ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foo#baz").Should().BeTrue();
        }

        [TestMethod]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameDoesNotContainHash()
        {
            ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foobaz").Should().BeFalse();
            ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "foobaz").Should().BeFalse();
        }

        [TestMethod]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameIsAbsoluteUriRelativeToMetadataDocumentUri()
        {
            ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, metadataDocumentUri.OriginalString + "#baz").Should().BeFalse();
        }

        [TestMethod]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameIsHashThenIdentifier()
        {
            ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "#baz").Should().BeFalse();
        }

        [TestMethod]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsNull()
        {
            ODataOperation operation = new ODataAction();
            Action action = () => ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(operation.GetType().Name));
        }

        [TestMethod]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsNotMetadataReferenceProperty()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("foobaz", UriKind.Relative)};
            Action action = () => ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("foobaz"));
        }

        [TestMethod]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsOpenAndOperationTargetIsNull()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("http://www.example.com/foo#baz")};
            Action action = () => ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/foo#baz", metadataDocumentUri.AbsoluteUri));
        }

        [TestMethod]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsOpenAndOperationTargetIsNotNull()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("http://www.example.com/foo#baz"), Target = new Uri("http://www.example.com")};
            Action action = () => ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/foo#baz", metadataDocumentUri.AbsoluteUri));
        }

        [TestMethod]
        public void ValidateOperationShouldNotThrowWhenOperationMetadataIsNotOpenAndOperationTargetNull()
        {
            ODataOperation operation = new ODataAction { Metadata = new Uri(metadataDocumentUri.OriginalString + "#baz") };
            ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
        }

        [TestMethod]
        public void ValidateOperationPropertyShouldThrowWhenPropertyValueIsNull()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(null, "target", "#action1");
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightValidationUtils_OperationPropertyCannotBeNull("target", "#action1"));
        }

        [TestMethod]
        public void ValidateOperationPropertyShouldNotThrowWhenPropertyValueIsNotNull()
        {
            ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(string.Empty, "target", "#action1");
            ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull("value", "target", "#action1");
        }
    }
}
