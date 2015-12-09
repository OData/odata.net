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

namespace Microsoft.OData.Core.Tests
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
            Assert.True(settings.BaseUri == null, "BaseUri should be null by default.");
            Assert.False(settings.CheckCharacters, "The CheckCharacters should be off by default.");
            Assert.False(settings.DisablePrimitiveTypeConversion, "DisablePrimitiveTypeConversion should be false by default.");
            Assert.False(settings.DisableMessageStreamDisposal, "DisableMessageStreamDisposal should be false by default.");
            Assert.False(settings.EnableAtomMetadataReading, "EnableAtomMetadataReading should be false by default.");
            Assert.True(ODataUndeclaredPropertyBehaviorKinds.None == settings.UndeclaredPropertyBehaviorKinds, "UndeclaredPropertyBehaviorKinds should be Default by default.");
            Assert.True(ODataVersion.V4 == settings.MaxProtocolVersion, "MaxProtocolVersion should be V3.");
            Assert.True(100 == settings.MessageQuotas.MaxPartsPerBatch, "MaxPartsPerBatch should be int.MaxValue.");
            Assert.True(1000 == settings.MessageQuotas.MaxOperationsPerChangeset, "MaxOperationsPerChangeset should be int.MaxValue.");
            Assert.True(100 == settings.MessageQuotas.MaxNestingDepth, "The MaxNestingDepth should be set to 100 by default.");
            Assert.True(1024 * 1024 == settings.MessageQuotas.MaxReceivedMessageSize, "The MaxMessageSize should be set to 1024 * 1024 by default.");
        }

        [Fact]
        public void PropertyGettersAndSettersTest()
        {
            Uri baseUri = new Uri("http://odata.org");
            Func<ODataEntry, XmlReader, Uri, XmlReader> entryAtomXmlCustomizationCallback = (entry, reader, uri) => reader;

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings {
                BaseUri = baseUri,
                CheckCharacters = true,
                DisablePrimitiveTypeConversion = true,
                DisableMessageStreamDisposal = true,
                EnableAtomMetadataReading = true,
                UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty,
                MaxProtocolVersion = ODataVersion.V4,
                MessageQuotas = new ODataMessageQuotas {
                    MaxPartsPerBatch = 2,
                    MaxOperationsPerChangeset = 3,
                    MaxNestingDepth = 4,
                    MaxReceivedMessageSize = 5,
                },
            };

            Assert.True(baseUri.Equals(settings.BaseUri), "The BaseUri was not correctly remembered.");
            Assert.True(settings.CheckCharacters, "The CheckCharacters should be on when set.");
            Assert.True(settings.DisablePrimitiveTypeConversion, "DisablePrimitiveTypeConversion was not correctly remembered.");
            Assert.True(settings.DisableMessageStreamDisposal, "DisableMessageStreamDisposal was not correctly remembered.");
            Assert.True(settings.EnableAtomMetadataReading, "EnableAtomMetadataReading was not correctly remembered.");
            Assert.True(ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty == settings.UndeclaredPropertyBehaviorKinds, "UndeclaredPropertyBehaviorKinds was not correctly remembered.");
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
            var copyOfSettings = new ODataMessageReaderSettings(settings);
            // Compare original and settings created from copy constructor without setting values
            this.CompareMessageReaderSettings(settings, copyOfSettings);

            // Compare original and settings created from copy constructor after setting some values
            settings.BaseUri = new Uri("http://www.odata.org");
            settings.CheckCharacters = true;
            copyOfSettings = new ODataMessageReaderSettings(settings);
            this.CompareMessageReaderSettings(settings, copyOfSettings);

            // Compare original and settings created from copy constructor after setting rest of the values 
            settings.DisableMessageStreamDisposal = true;
            settings.EnableAtomMetadataReading = true;
            settings.UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty | ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty;
            settings.MaxProtocolVersion = ODataVersion.V4;
            settings.MessageQuotas.MaxPartsPerBatch = 100;
            settings.MessageQuotas.MaxOperationsPerChangeset = 200;
            settings.MessageQuotas.MaxNestingDepth = 20;
            settings.MessageQuotas.MaxReceivedMessageSize = 2000;
            settings.EnableAtom = true;
            copyOfSettings = new ODataMessageReaderSettings(settings);
            this.CompareMessageReaderSettings(settings, copyOfSettings);

            // Compare original and settings created from copy constructor after setting some values to null and changing some other values
            settings.BaseUri = null;
            settings.CheckCharacters = false;
            settings.EnableAtomMetadataReading = false;
            copyOfSettings = new ODataMessageReaderSettings(settings);
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

        // These tests and helpers are disabled on Silverlight and Phone because they  
        // use private reflection not available on Silverlight and Phone
#if !SILVERLIGHT && !WINDOWS_PHONE
        [Fact]
        public void SetBehaviorTest()
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();

            settings.EnableWcfDataServicesClientBehavior(null);
            this.CompareReaderBehavior(
                settings,
                /*formatBehaviorKind*/ODataBehaviorKind.WcfDataServicesClient,
                /*apiBehaviorKind*/ODataBehaviorKind.WcfDataServicesClient,
                true,
                /*typeResolver*/ null);

            Func<IEdmType, string, IEdmType> customTypeResolver = (expectedType, typeName) => expectedType;
            settings.EnableWcfDataServicesClientBehavior(customTypeResolver);
            this.CompareReaderBehavior(
                settings,
                /*formatBehaviorKind*/ODataBehaviorKind.WcfDataServicesClient,
                /*apiBehaviorKind*/ODataBehaviorKind.WcfDataServicesClient,
                true,
                customTypeResolver);

            settings.EnableODataServerBehavior();
            this.CompareReaderBehavior(
                settings,
                /*formatBehaviorKind*/ODataBehaviorKind.ODataServer,
                /*apiBehaviorKind*/ODataBehaviorKind.ODataServer,
                true,
                /*typeResolver*/ null);

            settings.EnableDefaultBehavior();
            this.CompareReaderBehavior(
                settings,
                /*formatBehaviorKind*/ODataBehaviorKind.Default,
                /*apiBehaviorKind*/ODataBehaviorKind.Default,
                false,
                /*typeResolver*/ null);
        }

        private void CompareReaderBehavior(
            ODataMessageReaderSettings settings,
            ODataBehaviorKind formatBehaviorKind,
            ODataBehaviorKind apiBehaviorKind,
            bool allowDuplicatePropertyNames,
            Func<IEdmType, string, IEdmType> typeResolver)
        {
            ODataReaderBehavior readerBehavior = settings.ReaderBehavior;

            Assert.True(formatBehaviorKind == readerBehavior.FormatBehaviorKind, "Reader format behavior kinds don't match.");
            Assert.True(apiBehaviorKind == readerBehavior.ApiBehaviorKind, "Reader API behavior kinds don't match.");
            Assert.True(allowDuplicatePropertyNames == readerBehavior.AllowDuplicatePropertyNames, "AllowDuplicatePropertyNames values don't match.");
            Assert.True(typeResolver == readerBehavior.TypeResolver, "TypeResolver values don't match.");
        }
#endif

        private void CompareMessageReaderSettings(ODataMessageReaderSettings expected, ODataMessageReaderSettings actual)
        {
            if (expected == null && actual == null)
            {
                return;
            }

            Assert.True(expected != null, "expected settings cannot be null");
            Assert.True(actual != null, "actual settings cannot be null");
            Assert.True(Uri.Compare(expected.BaseUri, actual.BaseUri, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.CurrentCulture) == 0,
                "BaseUri does not match");
            Assert.True(expected.CheckCharacters == actual.CheckCharacters, "CheckCharacters does not match");
            Assert.True(expected.DisableMessageStreamDisposal == actual.DisableMessageStreamDisposal, "DisableMessageStreamDisposal does not match");
            Assert.True(expected.EnableAtomMetadataReading == actual.EnableAtomMetadataReading, "EnableAtomMetadataReading does not match");
            Assert.True(expected.UndeclaredPropertyBehaviorKinds == actual.UndeclaredPropertyBehaviorKinds, "UndeclaredPropertyBehaviorKinds does not match");
            Assert.True(expected.MaxProtocolVersion == actual.MaxProtocolVersion, "MaxProtocolVersion does not match.");
            Assert.True(expected.MessageQuotas.MaxPartsPerBatch == actual.MessageQuotas.MaxPartsPerBatch, "MaxPartsPerBatch does not match");
            Assert.True(expected.MessageQuotas.MaxOperationsPerChangeset == actual.MessageQuotas.MaxOperationsPerChangeset, "MaxOperationsPerChangeset does not match");
            Assert.True(expected.MessageQuotas.MaxNestingDepth == actual.MessageQuotas.MaxNestingDepth, "MaxNestingDepth does not match");
            Assert.True(expected.MessageQuotas.MaxReceivedMessageSize == actual.MessageQuotas.MaxReceivedMessageSize, "MaxMessageSize does not match");
            Assert.True(expected.EnableAtom == actual.EnableAtom, "EnableAtom does not match");
        }

        [Fact]
        public void ShouldSkipAnnotationsByDefaultForV3()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings();
            readerSettings.MaxProtocolVersion = ODataVersion.V4;
            readerSettings.ShouldSkipAnnotation("any.any").Should().BeTrue();
        }

        [Fact]
        public void ShouldHonorAnnotationFilterForV3()
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
            var copy = new ODataMessageReaderSettings(testSubject);
            copy.ShouldIncludeAnnotation.Should().BeSameAs(testSubject.ShouldIncludeAnnotation);
        }

        [Fact]
        public void CopyConstructorShouldCopyMediaTypeResolver()
        {
            var resolver = new ODataMediaTypeResolver();
            ODataMessageReaderSettings testSubject = new ODataMessageReaderSettings();
            testSubject.MediaTypeResolver = resolver;
            var copy = new ODataMessageReaderSettings(testSubject);
            copy.MediaTypeResolver.Should().Be(resolver);
        }
    }
}
