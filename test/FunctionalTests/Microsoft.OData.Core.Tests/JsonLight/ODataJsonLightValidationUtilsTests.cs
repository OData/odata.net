//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValidationUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.JsonLight;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightValidationUtilsTests
    {
        private static Uri metadataDocumentUri = new Uri("http://www.myservice.svc/$metadata");

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameIsHashThenValidIdentifier()
        {
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#Action1");
        }
        
        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsOpenMetadataReferenceProperty()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/$metadata#foo");
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/$metadata#foo", metadataDocumentUri.AbsoluteUri));
        }
        
        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameIsMetadataDocumentUriWithHashThenValidIdentifier()
        {
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, metadataDocumentUri.OriginalString + "#bar");
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsWithoutHash()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "bar");
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("bar"));
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsAbsoluteUriWithoutHash()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foo");
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("http://www.example.com/foo"));
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsIdentifierHashIdentifier()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "foo#baz");
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("foo#baz"));
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsJustHash()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#");
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("#"));
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameHasTwoHashes()
        {
            ODataJsonLightValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#foo#baz");
        }

        //ValidateOperation
        [Fact]
        public void IsOpenMetadataReferencePropertyNameShouldBeTrueWhenNameIsAbsoluteUriNotRelativeToMetadataDocumentUri()
        {
            Assert.True(ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foo#baz"));
        }

        [Fact]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameDoesNotContainHash()
        {
            Assert.False(ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foobaz"));
            Assert.False(ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "foobaz"));
        }

        [Fact]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameIsAbsoluteUriRelativeToMetadataDocumentUri()
        {
            Assert.False(ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, metadataDocumentUri.OriginalString + "#baz"));
        }

        [Fact]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameIsHashThenIdentifier()
        {
            Assert.False(ODataJsonLightValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "#baz"));
        }

        [Fact]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsNull()
        {
            ODataOperation operation = new ODataAction();
            Action action = () => ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(operation.GetType().Name));
        }

        [Fact]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsNotMetadataReferenceProperty()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("foobaz", UriKind.Relative)};
            Action action = () => ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("foobaz"));
        }

        [Fact]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsOpenAndOperationTargetIsNull()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("http://www.example.com/foo#baz")};
            Action action = () => ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/foo#baz", metadataDocumentUri.AbsoluteUri));
        }

        [Fact]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsOpenAndOperationTargetIsNotNull()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("http://www.example.com/foo#baz"), Target = new Uri("http://www.example.com")};
            Action action = () => ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/foo#baz", metadataDocumentUri.AbsoluteUri));
        }

        [Fact]
        public void ValidateOperationShouldNotThrowWhenOperationMetadataIsNotOpenAndOperationTargetNull()
        {
            ODataOperation operation = new ODataAction { Metadata = new Uri(metadataDocumentUri.OriginalString + "#baz") };
            ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, operation);
        }

        [Fact]
        public void ValidateOperationPropertyShouldThrowWhenPropertyValueIsNull()
        {
            Action action = () => ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(null, "target", "#action1");
            action.Throws<ODataException>(ErrorStrings.ODataJsonLightValidationUtils_OperationPropertyCannotBeNull("target", "#action1"));
        }

        [Fact]
        public void ValidateOperationPropertyShouldNotThrowWhenPropertyValueIsNotNull()
        {
            ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull(string.Empty, "target", "#action1");
            ODataJsonLightValidationUtils.ValidateOperationPropertyValueIsNotNull("value", "target", "#action1");
        }
    }
}
