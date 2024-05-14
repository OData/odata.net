//---------------------------------------------------------------------
// <copyright file="ODataJsonValidationUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonValidationUtilsTests
    {
        private static Uri metadataDocumentUri = new Uri("http://www.myservice.svc/$metadata");

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameIsHashThenValidIdentifier()
        {
            ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#Action1");
        }
        
        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsOpenMetadataReferenceProperty()
        {
            Action action = () => ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/$metadata#foo");
            action.Throws<ODataException>(ErrorStrings.ODataJsonValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/$metadata#foo", metadataDocumentUri.AbsoluteUri));
        }
        
        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameIsMetadataDocumentUriWithHashThenValidIdentifier()
        {
            ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, metadataDocumentUri.OriginalString + "#bar");
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsWithoutHash()
        {
            Action action = () => ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "bar");
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("bar"));
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsAbsoluteUriWithoutHash()
        {
            Action action = () => ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foo");
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("http://www.example.com/foo"));
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsIdentifierHashIdentifier()
        {
            Action action = () => ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "foo#baz");
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("foo#baz"));
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldThrowWhenNameIsJustHash()
        {
            Action action = () => ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#");
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("#"));
        }

        [Fact]
        public void ValidateMetadataReferencePropertyNameShouldNotThrowWhenNameHasTwoHashes()
        {
            ODataJsonValidationUtils.ValidateMetadataReferencePropertyName(metadataDocumentUri, "#foo#baz");
        }

        //ValidateOperation
        [Fact]
        public void IsOpenMetadataReferencePropertyNameShouldBeTrueWhenNameIsAbsoluteUriNotRelativeToMetadataDocumentUri()
        {
            Assert.True(ODataJsonValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foo#baz"));
        }

        [Fact]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameDoesNotContainHash()
        {
            Assert.False(ODataJsonValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "http://www.example.com/foobaz"));
            Assert.False(ODataJsonValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "foobaz"));
        }

        [Fact]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameIsAbsoluteUriRelativeToMetadataDocumentUri()
        {
            Assert.False(ODataJsonValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, metadataDocumentUri.OriginalString + "#baz"));
        }

        [Fact]
        public void IsOpenMetadataReferencePropertyNameShouldBeFalseWhenNameIsHashThenIdentifier()
        {
            Assert.False(ODataJsonValidationUtils.IsOpenMetadataReferencePropertyName(metadataDocumentUri, "#baz"));
        }

        [Fact]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsNull()
        {
            ODataOperation operation = new ODataAction();
            Action action = () => ODataJsonValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(operation.GetType().Name));
        }

        [Fact]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsNotMetadataReferenceProperty()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("foobaz", UriKind.Relative)};
            Action action = () => ODataJsonValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.Throws<ODataException>(ErrorStrings.ValidationUtils_InvalidMetadataReferenceProperty("foobaz"));
        }

        [Fact]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsOpenAndOperationTargetIsNull()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("http://www.example.com/foo#baz")};
            Action action = () => ODataJsonValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/foo#baz", metadataDocumentUri.AbsoluteUri));
        }

        [Fact]
        public void ValidateOperationShouldThrowWhenOperationMetadataIsOpenAndOperationTargetIsNotNull()
        {
            ODataOperation operation = new ODataAction {Metadata = new Uri("http://www.example.com/foo#baz"), Target = new Uri("http://www.example.com")};
            Action action = () => ODataJsonValidationUtils.ValidateOperation(metadataDocumentUri, operation);
            action.Throws<ODataException>(ErrorStrings.ODataJsonValidationUtils_OpenMetadataReferencePropertyNotSupported("http://www.example.com/foo#baz", metadataDocumentUri.AbsoluteUri));
        }

        [Fact]
        public void ValidateOperationShouldNotThrowWhenOperationMetadataIsNotOpenAndOperationTargetNull()
        {
            ODataOperation operation = new ODataAction { Metadata = new Uri(metadataDocumentUri.OriginalString + "#baz") };
            ODataJsonValidationUtils.ValidateOperation(metadataDocumentUri, operation);
        }

        [Fact]
        public void ValidateOperationPropertyShouldThrowWhenPropertyValueIsNull()
        {
            Action action = () => ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(null, "target", "#action1");
            action.Throws<ODataException>(ErrorStrings.ODataJsonValidationUtils_OperationPropertyCannotBeNull("target", "#action1"));
        }

        [Fact]
        public void ValidateOperationPropertyShouldNotThrowWhenPropertyValueIsNotNull()
        {
            ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull(string.Empty, "target", "#action1");
            ODataJsonValidationUtils.ValidateOperationPropertyValueIsNotNull("value", "target", "#action1");
        }
    }
}
