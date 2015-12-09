//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettingsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests
{
    public class ODataMessageWriterSettingsTests
    {
        private static Uri ServiceDocumentUri = new Uri("http://odata.org/service", UriKind.Absolute);
        private ODataMessageWriterSettings settings;

        public ODataMessageWriterSettingsTests()
        {
            this.settings = new ODataMessageWriterSettings();
        }

        [Fact]
        public void AnnotationFilterShouldBeNullByDefault()
        {
            this.settings.ShouldIncludeAnnotation.Should().BeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetAnnotationFilter()
        {
            Func<string, bool> filter = name => true;

            this.settings.ShouldIncludeAnnotation = filter;
            this.settings.ShouldIncludeAnnotation.Should().Be(filter);
        }

        [Fact]
        public void ShouldSkipAnnotationsByDefaultForV4()
        {
            this.settings.Version = ODataVersion.V4;
            this.settings.ShouldSkipAnnotation("any.any").Should().BeTrue();
        }

        [Fact]
        public void ShouldHonorAnnotationFilterForV4()
        {
            this.settings.Version = ODataVersion.V4;
            this.settings.ShouldIncludeAnnotation = name => name.StartsWith("ns1.");
            this.settings.ShouldSkipAnnotation("any.any").Should().BeTrue();
            this.settings.ShouldSkipAnnotation("ns1.any").Should().BeFalse();
        }

        #region metadata document uri tests
        [Fact]
        public void WriterSettingsIntegrationTest()
        {
            var settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(ServiceDocumentUri);
            settings.MetadataDocumentUri.Should().Equals(ServiceDocumentUri + "/$metadata");

            settings.SetServiceDocumentUri(null);
            settings.MetadataDocumentUri.Should().BeNull();
        }

        [Fact]
        public void WriterSettingsIntegrationTestWithSelect()
        {
            var setting = new ODataMessageWriterSettings();
            var edmModel = new EdmModel();
            var defaultContainer = new EdmEntityContainer("TestModel", "DefaultContainer");
            edmModel.AddElement(defaultContainer);
            var cityType = new EdmEntityType("TestModel", "City");
            var cityIdProperty = cityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            cityType.AddKeys(cityIdProperty);
            cityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            cityType.AddStructuralProperty("Size", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            edmModel.AddElement(cityType);
            var citySet = defaultContainer.AddEntitySet("Cities", cityType);

            var result = new ODataQueryOptionParser(edmModel, cityType, citySet, new Dictionary<string, string> { { "$expand", "" }, { "$select", "Id,*" } }).ParseSelectAndExpand();

            setting.SetServiceDocumentUri(ServiceDocumentUri);
            setting.ODataUri = new ODataUri() { SelectAndExpand = result };
            setting.MetadataDocumentUri.Should().Equals(ServiceDocumentUri + "/$metadata");
            string select, expand;
            setting.SelectExpandClause.GetSelectExpandPaths(out select, out expand);
            select.Should().Be("*");
        }
        #endregion metadata document uri tests

        #region Default settings tests
        [Fact]
        public void BaseUriShouldBeNullByDefault()
        {
            this.settings.PayloadBaseUri.Should().BeNull();
        }

        [Fact]
        public void CheckCharactersShouldBeFalseByDefault()
        {
            this.settings.CheckCharacters.Should().BeFalse();
        }

        [Fact]
        public void IndentShouldBeFalseByDefault()
        {
            this.settings.Indent.Should().BeFalse();
        }

        [Fact]
        public void DisableMessageStreamDisposalShouldBeFalseByDefault()
        {
            this.settings.DisableMessageStreamDisposal.Should().BeFalse();
        }

        [Fact]
        public void JsonPCallbackShouldBeNullByDefault()
        {
            this.settings.JsonPCallback.Should().BeNull();
        }

        [Fact]
        public void VersionShouldBeNullByDefault()
        {
            this.settings.Version.Should().BeNull();
        }

        [Fact]
        public void MaxReceivedMessageSizeShouldBeSetByDefault()
        {
            this.settings.MessageQuotas.MaxReceivedMessageSize.Should().Be(1024 * 1024);
        }

        [Fact]
        public void MaxPartsPerBatchShouldBeSetByDefault()
        {
            this.settings.MessageQuotas.MaxPartsPerBatch.Should().Be(100);
        }

        [Fact]
        public void MaxOperationsPerChangesetShouldBeSetByDefault()
        {
            this.settings.MessageQuotas.MaxOperationsPerChangeset.Should().Be(1000);
        }

        [Fact]
        public void MaxNestingDepthShouldBeSetByDefault()
        {
            this.settings.MessageQuotas.MaxNestingDepth.Should().Be(100);
        }

        [Fact]
        public void AutoComputePayloadMetadataInJsonShouldBeFalseByDefault()
        {
            this.settings.AutoComputePayloadMetadataInJson.Should().BeFalse();
        }

        [Fact]
        public void AutoGeneratedUrlsShouldPutKeyValueInDedicatedSegmentShouldBeNullByDefault()
        {
            this.settings.UseKeyAsSegment.Should().Be(null);
        }

        [Fact]
        public void ODataUriShouldBeNotNullByDefault()
        {
            this.settings.ODataUri.Should().NotBeNull();
        }

        [Fact]
        public void IsIndividualPropertyShouldBeFalseByDefault()
        {
            this.settings.IsIndividualProperty.Should().Be(false);
        }

        [Fact]
        public void SelectExpandClauseBeNullByDefault()
        {
            this.settings.SelectExpandClause.Should().Be(null);
        }

        [Fact]
        public void EnableAtomBeFalseByDefault()
        {
            this.settings.EnableAtom.Should().BeFalse();
        }
        #endregion Default settings tests

        #region Copy constructor tests
        [Fact]
        public void CopyConstructorShouldCopyBaseUri()
        {
            var baseUri = new Uri("http://otherservice.svc");
            this.settings.PayloadBaseUri = baseUri;
            (new ODataMessageWriterSettings(this.settings)).PayloadBaseUri.Should().BeSameAs(baseUri);
        }

        [Fact]
        public void CopyConstructorShouldCopyCheckCharacters()
        {
            this.settings.CheckCharacters = true;
            (new ODataMessageWriterSettings(this.settings)).CheckCharacters.Should().BeTrue();
        }

        [Fact]
        public void CopyConstructorShouldCopyDisableMessageStreamDisposal()
        {
            this.settings.DisableMessageStreamDisposal = true;
            (new ODataMessageWriterSettings(this.settings)).DisableMessageStreamDisposal.Should().BeTrue();
        }

        [Fact]
        public void CopyConstructorShouldCopyIndent()
        {
            this.settings.Indent = true;
            (new ODataMessageWriterSettings(this.settings)).Indent.Should().BeTrue();
        }

        [Fact]
        public void CopyConstructorShouldCopyJsonPCallback()
        {
            this.settings.JsonPCallback = "jsonp";
            (new ODataMessageWriterSettings(this.settings)).JsonPCallback.Should().Be("jsonp");
        }

        [Fact]
        public void CopyConstructorShouldCopyMessageQuotas()
        {
            var originalQuotas = new ODataMessageQuotas()
            {
                MaxNestingDepth = 2,
                MaxOperationsPerChangeset = 3,
                MaxPartsPerBatch = 4,
                MaxReceivedMessageSize = 5
            };

            this.settings.MessageQuotas = originalQuotas;

            var copiedQuotas = (new ODataMessageWriterSettings(this.settings)).MessageQuotas;

            copiedQuotas.MaxNestingDepth.Should().Be(originalQuotas.MaxNestingDepth);
            copiedQuotas.MaxOperationsPerChangeset.Should().Be(originalQuotas.MaxOperationsPerChangeset);
            copiedQuotas.MaxPartsPerBatch.Should().Be(originalQuotas.MaxPartsPerBatch);
            copiedQuotas.MaxReceivedMessageSize.Should().Be(originalQuotas.MaxReceivedMessageSize);
        }

        [Fact]
        public void CopyConstructorShouldCopyVersion()
        {
            this.settings.Version = ODataVersion.V4;
            (new ODataMessageWriterSettings(this.settings)).Version.Should().Be(ODataVersion.V4);
        }

        [Fact]
        public void CopyConstructorShouldCopyAnnotationFilter()
        {
            Func<string, bool> filter = name => true;

            this.settings.ShouldIncludeAnnotation = filter;
            (new ODataMessageWriterSettings(this.settings)).ShouldIncludeAnnotation.Should().Be(filter);
        }

        [Fact]
        public void CopyConstructorShouldCopyMetadataDocumentUri()
        {
            this.settings.SetServiceDocumentUri(new Uri("http://example.com"));
            (new ODataMessageWriterSettings(this.settings)).MetadataDocumentUri.Should().Be(new Uri("http://example.com/$metadata"));
        }

        [Fact]
        public void CopyConstructorShouldCopyFormat()
        {
            this.settings.SetContentType(ODataFormat.Metadata);
            (new ODataMessageWriterSettings(this.settings)).Format.Should().Be(ODataFormat.Metadata);
        }

        [Fact]
        public void CopyConstructorShouldCopyAccept()
        {
            this.settings.SetContentType("application/json,application/atom+xml", "iso-8859-5, unicode-1-1;q=0.8");
            var copiedSettings = (new ODataMessageWriterSettings(this.settings));
            copiedSettings.AcceptableCharsets.Should().Be("iso-8859-5, unicode-1-1;q=0.8");
            copiedSettings.AcceptableMediaTypes.Should().Be("application/json,application/atom+xml");
        }

        [Fact]
        public void CopyConstructorShouldCopyAutoComputePayloadMetadataInJson()
        {
            this.settings.AutoComputePayloadMetadataInJson = true;
            (new ODataMessageWriterSettings(this.settings)).AutoComputePayloadMetadataInJson.Should().BeTrue();
        }

        [Fact]
        public void CopyConstructorShouldCopyAutoGeneratedUrlsShouldPutKeyValueInDedicatedSegment()
        {
            this.settings.UseKeyAsSegment = true;
            (new ODataMessageWriterSettings(this.settings)).UseKeyAsSegment.Should().BeTrue();
        }

        [Fact]
        public void CopyConstructorShouldCopyServiceDocument()
        {
            this.settings.SetServiceDocumentUri(new Uri("http://test.org"));
            var newSetting = new ODataMessageWriterSettings(this.settings);
            newSetting.MetadataDocumentUri.Should().Be(new Uri("http://test.org/$metadata"));
        }

        [Fact]
        public void CopyConstructorShouldCopyODataUri()
        {
            var edmModel = new EdmModel();
            var defaultContainer = new EdmEntityContainer("TestModel", "DefaultContainer");
            edmModel.AddElement(defaultContainer);
            var cityType = new EdmEntityType("TestModel", "City");
            var cityIdProperty = cityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            cityType.AddKeys(cityIdProperty);
            cityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            cityType.AddStructuralProperty("Size", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            edmModel.AddElement(cityType);
            var citySet = defaultContainer.AddEntitySet("Cities", cityType);

            var result = new ODataQueryOptionParser(edmModel, cityType, citySet, new Dictionary<string, string> { { "$expand", "" }, { "$select", "Id,*" } }).ParseSelectAndExpand();

            this.settings.ODataUri = new ODataUri()
            {
                ServiceRoot = new Uri("http://test.org"),
                SelectAndExpand = result,
                Path = new ODataUriParser(edmModel, new Uri("http://test.org"), new Uri("http://test.org/Cities(1)/Name")).ParsePath()
            };

            var newSetting = new ODataMessageWriterSettings(this.settings);
            newSetting.MetadataDocumentUri.Should().Be(new Uri("http://test.org/$metadata"));
            newSetting.ODataUri.Path.ToResourcePathString(ODataUrlConventions.Default).Should().Be("Cities(1)/Name");
            newSetting.IsIndividualProperty.Should().BeTrue();

            string select, expand;
            newSetting.SelectExpandClause.GetSelectExpandPaths(out select, out expand);
            select.Should().Be("*");
        }

        [Fact]
        public void CopyConstructorShouldCopyEnableAtom()
        {
            this.settings.EnableAtom = true;
            var newSetting = new ODataMessageWriterSettings(this.settings);
            newSetting.EnableAtom.Should().BeTrue();
        }

        [Fact]
        public void CopyConstructorShouldCopyMediaTypeResolver()
        {
            var resolver = new ODataMediaTypeResolver();
            this.settings.MediaTypeResolver = resolver;
            var newSetting = new ODataMessageWriterSettings(this.settings);
            newSetting.MediaTypeResolver.Should().Be(resolver);
        }
        #endregion Copy constructor tests

        #region Property getters and setters tests
        [Fact]
        public void PropertyGettersAndSettersTest()
        {
            Uri baseUri = new Uri("http://odatalib.org");
            const int maxPartsPerBatch = 42;
            const int maxOperationsPerChangeset = 43;
            const int maxNestingDepth = 44;
            const ODataVersion version = ODataVersion.V4;

            this.settings = new ODataMessageWriterSettings()
            {
                PayloadBaseUri = baseUri,
                CheckCharacters = true,
                Indent = true,
                Version = version,
                DisableMessageStreamDisposal = true,
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxPartsPerBatch = maxPartsPerBatch,
                    MaxOperationsPerChangeset = maxOperationsPerChangeset,
                    MaxNestingDepth = maxNestingDepth,
                },
                AutoComputePayloadMetadataInJson = true,
                UseKeyAsSegment = true
            };

            this.settings.PayloadBaseUri.Should().BeSameAs(baseUri);
            this.settings.CheckCharacters.Should().BeTrue();
            this.settings.Indent.Should().BeTrue();
            this.settings.MessageQuotas.MaxPartsPerBatch.Should().Be(maxPartsPerBatch);
            this.settings.MessageQuotas.MaxOperationsPerChangeset.Should().Be(maxOperationsPerChangeset);
            this.settings.MessageQuotas.MaxNestingDepth.Should().Be(maxNestingDepth);
            this.settings.Version.Should().Be(version);
            this.settings.DisableMessageStreamDisposal.Should().BeTrue();
            this.settings.AutoComputePayloadMetadataInJson.Should().BeTrue();
            this.settings.UseKeyAsSegment.Should().BeTrue();
        }

        [Fact]
        public void PayloadBaseUriGetterAndSetterTest()
        {
            var setting = new ODataMessageWriterSettings()
            {
                PayloadBaseUri = new Uri("http://example.org/odata.svc"),
            };

            var t = setting.PayloadBaseUri.ToString();
            setting.PayloadBaseUri.ToString().Should().BeEquivalentTo("http://example.org/odata.svc/");

            setting = new ODataMessageWriterSettings()
            {
                PayloadBaseUri = new Uri("http://example.org/odata.svc/"),
            };
            setting.PayloadBaseUri.ToString().Should().BeEquivalentTo("http://example.org/odata.svc/");
        }
        #endregion Property getters and setters tests

        #region Error tests

        [Fact]
        public void MaxPartsPerBatchShouldThrowIfSetToNegativeNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxPartsPerBatch = -1;
            testSubject.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(ODataErrorStrings.ExceptionUtils_CheckIntegerNotNegative(-1), ComparisonMode.StartWith);
        }

        [Fact]
        public void MaxOperationsPerChangesetShouldThrowIfSetToNegativeNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxOperationsPerChangeset = -1;
            testSubject.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(ODataErrorStrings.ExceptionUtils_CheckIntegerNotNegative(-1), ComparisonMode.StartWith);
        }

        [Fact]
        public void MaxNestingDepthShouldThrowIfSetToNonPositiveNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxNestingDepth = 0;
            testSubject.ShouldThrow<ArgumentOutOfRangeException>().WithMessage(ODataErrorStrings.ExceptionUtils_CheckIntegerPositive(0), ComparisonMode.StartWith);
        }
        #endregion Error tests

        #region Set content type tests
        [Fact]
        public void SetContentTypeTest()
        {
            settings.SetContentType(ODataFormat.Atom);
            ODataMessageWriterSettings copyOfSettings = new ODataMessageWriterSettings(settings);
            settings.Format.Should().Be(ODataFormat.Atom);
            copyOfSettings.Format.Should().Be(ODataFormat.Atom);
            settings.AcceptableMediaTypes.Should().BeNull();
            copyOfSettings.AcceptableMediaTypes.Should().BeNull();
            settings.AcceptableCharsets.Should().BeNull();
            copyOfSettings.AcceptableCharsets.Should().BeNull();

            settings.SetContentType("application/xml", null);
            copyOfSettings = new ODataMessageWriterSettings(settings);
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().Be("application/xml");
            copyOfSettings.AcceptableMediaTypes.Should().Be("application/xml");
            settings.AcceptableCharsets.Should().BeNull();
            copyOfSettings.AcceptableCharsets.Should().BeNull();

            settings.SetContentType("application/json", "utf8");
            copyOfSettings = new ODataMessageWriterSettings(settings);
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().Be("application/json");
            copyOfSettings.AcceptableMediaTypes.Should().Be("application/json");
            settings.AcceptableCharsets.Should().Be("utf8");
            copyOfSettings.AcceptableCharsets.Should().Be("utf8");

            settings.SetContentType(null, "utf8");
            copyOfSettings = new ODataMessageWriterSettings(settings);
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().BeNull();
            copyOfSettings.AcceptableMediaTypes.Should().BeNull();
            settings.AcceptableCharsets.Should().Be("utf8");
            copyOfSettings.AcceptableCharsets.Should().Be("utf8");

            settings.SetContentType(string.Empty, "utf8");
            copyOfSettings = new ODataMessageWriterSettings(settings);
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().BeEmpty();
            copyOfSettings.AcceptableMediaTypes.Should().BeEmpty();
            settings.AcceptableCharsets.Should().Be("utf8");
            copyOfSettings.AcceptableCharsets.Should().Be("utf8");

            settings.SetContentType("json", "utf8");
            copyOfSettings = new ODataMessageWriterSettings(settings);
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().Be("application/json");
            copyOfSettings.AcceptableMediaTypes.Should().Be("application/json");
            settings.AcceptableCharsets.Should().Be("utf8");
            copyOfSettings.AcceptableCharsets.Should().Be("utf8");
        }
        #endregion Set content type tests

        #region Set behavior tests
        [Fact]
        public void SetBehaviorTest()
        {
            this.settings.EnableWcfDataServicesClientBehavior();
            this.AssertWriterBehavior(/*formatBehaviorKind*/ODataBehaviorKind.WcfDataServicesClient, /*apiBehaviorKind*/ODataBehaviorKind.WcfDataServicesClient, false, false, false);

            this.settings.EnableODataServerBehavior(true);
            this.AssertWriterBehavior(/*formatBehaviorKind*/ODataBehaviorKind.ODataServer, /*apiBehaviorKind*/ODataBehaviorKind.ODataServer, true, true, true);

            this.settings.EnableODataServerBehavior(false);
            this.AssertWriterBehavior(/*formatBehaviorKind*/ODataBehaviorKind.ODataServer, /*apiBehaviorKind*/ODataBehaviorKind.ODataServer, false, true, true);

            this.settings.EnableDefaultBehavior();
            this.AssertWriterBehavior(/*formatBehaviorKind*/ODataBehaviorKind.Default, /*apiBehaviorKind*/ODataBehaviorKind.Default, false, false, false);
        }

        private void AssertWriterBehavior(
            ODataBehaviorKind formatBehaviorKind,
            ODataBehaviorKind apiBehaviorKind,
            bool useV1ProviderBehavior,
            bool allowNullValuesForNonNullablePrimitiveTypes,
            bool allowDuplicatePropertyNames)
        {
            var writerBehavior = this.settings.WriterBehavior;

            formatBehaviorKind.Should().Be(writerBehavior.FormatBehaviorKind);
            apiBehaviorKind.Should().Be(writerBehavior.ApiBehaviorKind);
            allowNullValuesForNonNullablePrimitiveTypes.Should().Be(writerBehavior.AllowNullValuesForNonNullablePrimitiveTypes);
            allowDuplicatePropertyNames.Should().Be(writerBehavior.AllowDuplicatePropertyNames);
        }
        #endregion Set behavior tests
    }
}
