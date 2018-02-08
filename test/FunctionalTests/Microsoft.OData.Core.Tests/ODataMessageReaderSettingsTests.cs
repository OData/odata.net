//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderSettingsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Xml;
using FluentAssertions;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Tests for ODataMessageReaderSettings class.
    /// </summary>
    public class ODataMessageReaderSettingsTests
    {
        [Fact]
        public void DefaultValuesTest()
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();

            Assert.True((settings.Validations & ValidationKinds.ThrowOnDuplicatePropertyNames) != 0, "The ThrowOnDuplicatePropertyNames should be true by default");
            Assert.Null(settings.BaseUri);
            Assert.Null(settings.ClientCustomTypeResolver);
            Assert.Null(settings.PrimitiveTypeResolver);
            Assert.True(settings.EnablePrimitiveTypeConversion, "EnablePrimitiveTypeConversion should be true by default.");
            Assert.True(settings.EnableMessageStreamDisposal, "EnableMessageStreamDisposal should be false by default.");
            Assert.False(settings.EnableCharactersCheck, "The CheckCharacters should be off by default.");
            Assert.True((settings.Validations & ValidationKinds.ThrowIfTypeConflictsWithMetadata) != 0, "The ThrowIfTypeConflictsWithMetadata should be true by default");                        
            Assert.Null(settings.ShouldIncludeAnnotation);
            Assert.True(settings.ReadUntypedAsString, "ReadUntypedAsString should be true by default for OData 4.0.");
            Assert.True((settings.Validations & ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType) != 0, "ThrowOnUndeclaredPropertyForNonOpenType should be true by default.");
            Assert.True(ODataVersion.V4 == settings.Version, "Version should be 4.0.");
            Assert.True(ODataVersion.V4 == settings.MaxProtocolVersion, "MaxProtocolVersion should be V4.");
            Assert.True(100 == settings.MessageQuotas.MaxPartsPerBatch, "MaxPartsPerBatch should be int.MaxValue.");
            Assert.True(1000 == settings.MessageQuotas.MaxOperationsPerChangeset, "MaxOperationsPerChangeset should be int.MaxValue.");
            Assert.True(100 == settings.MessageQuotas.MaxNestingDepth, "The MaxNestingDepth should be set to 100 by default.");
            Assert.True(1024 * 1024 == settings.MessageQuotas.MaxReceivedMessageSize, "The MaxMessageSize should be set to 1024 * 1024 by default.");
        }

        [Fact]
        public void DefaultValuesTest401()
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings(ODataVersion.V401);

            Assert.True((settings.Validations & ValidationKinds.ThrowOnDuplicatePropertyNames) != 0, "The ThrowOnDuplicatePropertyNames should be true by default");
            Assert.Null(settings.BaseUri);
            Assert.Null(settings.ClientCustomTypeResolver);
            Assert.Null(settings.PrimitiveTypeResolver);
            Assert.True(settings.EnablePrimitiveTypeConversion, "EnablePrimitiveTypeConversion should be true by default.");
            Assert.True(settings.EnableMessageStreamDisposal, "EnableMessageStreamDisposal should be false by default.");
            Assert.False(settings.EnableCharactersCheck, "The CheckCharacters should be off by default.");
            Assert.False(settings.ReadUntypedAsString, "ReadUntypedAsString should be false by default for OData 4.01.");
            Assert.True((settings.Validations & ValidationKinds.ThrowIfTypeConflictsWithMetadata) != 0, "The ThrowIfTypeConflictsWithMetadata should be true by default");
            Assert.Null(settings.ShouldIncludeAnnotation);
            Assert.True((settings.Validations & ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType) == 0, "ThrowOnUndeclaredPropertyForNonOpenType should be false by default for OData 4.01.");
            Assert.True(ODataVersion.V401 == settings.Version, "Version should be 4.01.");
            Assert.True(ODataVersion.V401 == settings.MaxProtocolVersion, "MaxProtocolVersion should be 4.01.");
            Assert.True(100 == settings.MessageQuotas.MaxPartsPerBatch, "MaxPartsPerBatch should be int.MaxValue.");
            Assert.True(1000 == settings.MessageQuotas.MaxOperationsPerChangeset, "MaxOperationsPerChangeset should be int.MaxValue.");
            Assert.True(100 == settings.MessageQuotas.MaxNestingDepth, "The MaxNestingDepth should be set to 100 by default.");
            Assert.True(1024 * 1024 == settings.MessageQuotas.MaxReceivedMessageSize, "The MaxMessageSize should be set to 1024 * 1024 by default.");
        }

        [Fact]
        public void PropertyGettersAndSettersTest()
        {
            Uri baseUri = new Uri("http://odata.org");

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings
            {
                BaseUri = baseUri,
                EnablePrimitiveTypeConversion = false,
                EnableMessageStreamDisposal = false,
                EnableCharactersCheck = true,
                MaxProtocolVersion = ODataVersion.V4,
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxPartsPerBatch = 2,
                    MaxOperationsPerChangeset = 3,
                    MaxNestingDepth = 4,
                    MaxReceivedMessageSize = 5,
                },
            };
            settings.Validations &= ~ValidationKinds.ThrowIfTypeConflictsWithMetadata
                                    & ~ValidationKinds.ThrowOnDuplicatePropertyNames;
            settings.Validations |= ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            Assert.True((settings.Validations & ValidationKinds.ThrowOnDuplicatePropertyNames) == 0, "The ThrowOnDuplicatePropertyNames was not correctly remembered");
            Assert.True(baseUri.Equals(settings.BaseUri), "The BaseUri was not correctly remembered.");
            Assert.False(settings.EnablePrimitiveTypeConversion, "EnablePrimitiveTypeConversion was not correctly remembered.");
            Assert.False(settings.EnableMessageStreamDisposal, "EnableMessageStreamDisposal was not correctly remembered.");
            Assert.True(settings.EnableCharactersCheck, "The CheckCharacters should be on when set.");
            Assert.False(settings.ThrowIfTypeConflictsWithMetadata, "The ThrowIfTypeConflictsWithMetadata was not correctly remembered");
            Assert.True((settings.Validations & ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType) != 0, "ThrowOnUndeclaredPropertyForNonOpenType was not correctly remembered.");
            Assert.True(ODataVersion.V4 == settings.MaxProtocolVersion, "The MaxProtocolVersion was not correctly remembered.");
            Assert.True(2 == settings.MessageQuotas.MaxPartsPerBatch, "MaxPartsPerBatch should be 2");
            Assert.True(3 == settings.MessageQuotas.MaxOperationsPerChangeset, "MaxOperationsPerChangeset should be 3");
            Assert.True(4 == settings.MessageQuotas.MaxNestingDepth, "MaxNestingDepth should be 4");
            Assert.True(5 == settings.MessageQuotas.MaxReceivedMessageSize, "MaxMessageSize should be 5");
        }

        [Fact]
        public void BaseUriGetterAndSetterTest()
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings
            {
                BaseUri = new Uri("http://example.org/odata.svc"),
            };

            var t = settings.BaseUri.ToString();
            settings.BaseUri.ToString().Should().BeEquivalentTo("http://example.org/odata.svc/");

            settings = new ODataMessageReaderSettings()
            {
                BaseUri = new Uri("http://example.org/odata.svc/"),
            };

            settings.BaseUri.ToString().Should().BeEquivalentTo("http://example.org/odata.svc/");
        }

        [Fact]
        public void ODataMessageReaderCopyConstructorTest()
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            var copyOfSettings = settings.Clone();
            // Compare original and settings created from copy constructor without setting values
            this.CompareMessageReaderSettings(settings, copyOfSettings);

            // Compare original and settings created from copy constructor after setting some values
            settings.BaseUri = new Uri("http://www.odata.org");
            settings.EnableCharactersCheck = true;
            copyOfSettings = settings.Clone();
            this.CompareMessageReaderSettings(settings, copyOfSettings);

            // Compare original and settings created from copy constructor after setting rest of the values 
            settings.EnableMessageStreamDisposal = false;            
            settings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            settings.MaxProtocolVersion = ODataVersion.V4;
            settings.MessageQuotas.MaxPartsPerBatch = 100;
            settings.MessageQuotas.MaxOperationsPerChangeset = 200;
            settings.MessageQuotas.MaxNestingDepth = 20;
            settings.MessageQuotas.MaxReceivedMessageSize = 2000;
            copyOfSettings = settings.Clone();
            this.CompareMessageReaderSettings(settings, copyOfSettings);

            // Compare original and settings created from copy constructor after setting some values to null and changing some other values
            settings.BaseUri = null;
            settings.EnableCharactersCheck = false;
            copyOfSettings = settings.Clone();
            this.CompareMessageReaderSettings(settings, copyOfSettings);
        }

        [Fact]
        public void ODataMessageReaderSettingsErrorTest()
        {
            // MaxPartsPerBatch
            Action test = () => new ODataMessageReaderSettings() { MessageQuotas = new ODataMessageQuotas() { MaxPartsPerBatch = -1 } };
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.ExceptionUtils_CheckIntegerNotNegative("-1") + "\r\nParameter name: MaxPartsPerBatch");

            // MaxOperationsPerChangeset
            test = () => new ODataMessageReaderSettings() { MessageQuotas = new ODataMessageQuotas() { MaxOperationsPerChangeset = -1 } };
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.ExceptionUtils_CheckIntegerNotNegative("-1") + "\r\nParameter name: MaxOperationsPerChangeset");

            // MaxNestingDepth
            test = () => new ODataMessageReaderSettings() { MessageQuotas = new ODataMessageQuotas() { MaxNestingDepth = -1 } };
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.ExceptionUtils_CheckIntegerPositive("-1") + "\r\nParameter name: MaxNestingDepth");

            test = () => new ODataMessageReaderSettings() { MessageQuotas = new ODataMessageQuotas() { MaxNestingDepth = 0 } };
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.ExceptionUtils_CheckIntegerPositive("0") + "\r\nParameter name: MaxNestingDepth");

            // MaxMessageSize
            test = () => new ODataMessageReaderSettings() { MessageQuotas = new ODataMessageQuotas() { MaxReceivedMessageSize = -1 } };
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.ExceptionUtils_CheckLongPositive("-1") + "\r\nParameter name: MaxReceivedMessageSize");

            test = () => new ODataMessageReaderSettings() { MessageQuotas = new ODataMessageQuotas() { MaxReceivedMessageSize = 0 } };
            test.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(Strings.ExceptionUtils_CheckLongPositive("0") + "\r\nParameter name: MaxReceivedMessageSize");
        }

        private void CompareMessageReaderSettings(ODataMessageReaderSettings expected, ODataMessageReaderSettings actual)
        {
            if (expected == null && actual == null)
            {
                return;
            }

            Assert.True(expected != null, "expected settings cannot be null");
            Assert.True(actual != null, "actual settings cannot be null");
            Assert.True(expected.Validations == actual.Validations, "Validations does not match");
            Assert.True(Uri.Compare(expected.BaseUri, actual.BaseUri, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.CurrentCulture) == 0,
                "BaseUri does not match");
            Assert.True(expected.ClientCustomTypeResolver == actual.ClientCustomTypeResolver, "ClientCustomTypeResolver does not match");
            Assert.True(expected.PrimitiveTypeResolver == actual.PrimitiveTypeResolver, "PrimitiveTypeResolver does not match");
            Assert.True(expected.EnableMessageStreamDisposal == actual.EnableMessageStreamDisposal, "EnableMessageStreamDisposal does not match");
            Assert.True(expected.EnablePrimitiveTypeConversion == actual.EnablePrimitiveTypeConversion, "EnablePrimitiveTypeConversion does not match");
            Assert.True(expected.EnableCharactersCheck == actual.EnableCharactersCheck, "CheckCharacters does not match");
            Assert.True(expected.ShouldIncludeAnnotation == actual.ShouldIncludeAnnotation, "UseKeyAsSegment does not match");
            Assert.True(expected.Validations == actual.Validations, "Validations does not match");
            Assert.True(expected.MaxProtocolVersion == actual.MaxProtocolVersion, "MaxProtocolVersion does not match.");
            Assert.True(expected.MessageQuotas.MaxPartsPerBatch == actual.MessageQuotas.MaxPartsPerBatch, "MaxPartsPerBatch does not match");
            Assert.True(expected.MessageQuotas.MaxOperationsPerChangeset == actual.MessageQuotas.MaxOperationsPerChangeset, "MaxOperationsPerChangeset does not match");
            Assert.True(expected.MessageQuotas.MaxNestingDepth == actual.MessageQuotas.MaxNestingDepth, "MaxNestingDepth does not match");
            Assert.True(expected.MessageQuotas.MaxReceivedMessageSize == actual.MessageQuotas.MaxReceivedMessageSize, "MaxMessageSize does not match");
        }

        [Fact]
        public void ShouldSkipAnnotationsByDefaultForV4()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings();
            readerSettings.MaxProtocolVersion = ODataVersion.V4;
            readerSettings.ShouldSkipAnnotation("any.any").Should().BeTrue();
        }

        [Fact]
        public void ShouldHonorAnnotationFilterForV4()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings();
            readerSettings.MaxProtocolVersion = ODataVersion.V4;
            readerSettings.ShouldIncludeAnnotation = name => name.StartsWith("ns1.");
            readerSettings.ShouldIncludeAnnotation.Should().NotBeNull();
            readerSettings.ShouldSkipAnnotation("any.any").Should().BeTrue();
            readerSettings.ShouldSkipAnnotation("ns1.any").Should().BeFalse();
        }

        [Fact]
        public void CopyConstructorShouldCopyAnnotationFilter()
        {
            ODataMessageReaderSettings testSubject = new ODataMessageReaderSettings();
            testSubject.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("namespace.*");
            var copy = testSubject.Clone();
            copy.ShouldIncludeAnnotation.Should().BeSameAs(testSubject.ShouldIncludeAnnotation);
        }
    }
}
