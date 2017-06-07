//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettingsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests
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
            this.settings.BaseUri.Should().BeNull();
        }

        [Fact]
        public void CheckCharactersShouldBeFalseByDefault()
        {
            this.settings.EnableCharactersCheck.Should().BeFalse();
        }

        [Fact]
        public void EnableMessageStreamDisposalShouldBeTrueByDefault()
        {
            this.settings.EnableMessageStreamDisposal.Should().BeTrue();
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
        public void AllowDuplicatePropertyNamesShouldBeFalseByDefault()
        {
            this.settings.ThrowOnDuplicatePropertyNames.Should().BeTrue();
        }

        #endregion Default settings tests

        #region Copy constructor tests
        [Fact]
        public void CopyConstructorShouldCopyBaseUri()
        {
            var baseUri = new Uri("http://otherservice.svc");
            this.settings.BaseUri = baseUri;
            this.settings.Clone().BaseUri.Should().BeSameAs(baseUri);
        }

        [Fact]
        public void CopyConstructorShouldCopyCheckCharacters()
        {
            this.settings.EnableCharactersCheck = true;
            this.settings.Clone().EnableCharactersCheck.Should().BeTrue();
        }

        [Fact]
        public void CopyConstructorShouldCopyEnableMessageStreamDisposal()
        {
            this.settings.EnableMessageStreamDisposal = false;
            this.settings.Clone().EnableMessageStreamDisposal.Should().BeFalse();
        }

        [Fact]
        public void CopyConstructorShouldCopyJsonPCallback()
        {
            this.settings.JsonPCallback = "jsonp";
            this.settings.Clone().JsonPCallback.Should().Be("jsonp");
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

            var copiedQuotas = this.settings.Clone().MessageQuotas;

            copiedQuotas.MaxNestingDepth.Should().Be(originalQuotas.MaxNestingDepth);
            copiedQuotas.MaxOperationsPerChangeset.Should().Be(originalQuotas.MaxOperationsPerChangeset);
            copiedQuotas.MaxPartsPerBatch.Should().Be(originalQuotas.MaxPartsPerBatch);
            copiedQuotas.MaxReceivedMessageSize.Should().Be(originalQuotas.MaxReceivedMessageSize);
        }

        [Fact]
        public void CopyConstructorShouldCopyVersion()
        {
            this.settings.Version = ODataVersion.V4;
            this.settings.Clone().Version.Should().Be(ODataVersion.V4);
        }

        [Fact]
        public void CopyConstructorShouldCopyAnnotationFilter()
        {
            Func<string, bool> filter = name => true;

            this.settings.ShouldIncludeAnnotation = filter;
            this.settings.Clone().ShouldIncludeAnnotation.Should().Be(filter);
        }

        [Fact]
        public void CopyConstructorShouldCopyMetadataDocumentUri()
        {
            this.settings.SetServiceDocumentUri(new Uri("http://example.com"));
            this.settings.Clone().MetadataDocumentUri.Should().Be(new Uri("http://example.com/$metadata"));
        }

        [Fact]
        public void CopyConstructorShouldCopyFormat()
        {
            this.settings.SetContentType(ODataFormat.Metadata);
            this.settings.Clone().Format.Should().Be(ODataFormat.Metadata);
        }

        [Fact]
        public void CopyConstructorShouldCopyAccept()
        {
            this.settings.SetContentType("application/json,application/atom+xml", "iso-8859-5, unicode-1-1;q=0.8");
            var copiedSettings = this.settings.Clone();
            copiedSettings.AcceptableCharsets.Should().Be("iso-8859-5, unicode-1-1;q=0.8");
            copiedSettings.AcceptableMediaTypes.Should().Be("application/json,application/atom+xml");
        }

        [Fact]
        public void CopyConstructorShouldCopyServiceDocument()
        {
            this.settings.SetServiceDocumentUri(new Uri("http://test.org"));
            var newSetting = this.settings.Clone();
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

            var newSetting = this.settings.Clone();
            newSetting.MetadataDocumentUri.Should().Be(new Uri("http://test.org/$metadata"));
            newSetting.ODataUri.Path.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses).Should().Be("Cities(1)/Name");
            newSetting.IsIndividualProperty.Should().BeTrue();

            string select, expand;
            newSetting.SelectExpandClause.GetSelectExpandPaths(out select, out expand);
            select.Should().Be("*");
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
                Validations = ~ValidationKinds.ThrowOnDuplicatePropertyNames,
                BaseUri = baseUri,
                EnableCharactersCheck = true,
                EnableMessageStreamDisposal = false,
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxPartsPerBatch = maxPartsPerBatch,
                    MaxOperationsPerChangeset = maxOperationsPerChangeset,
                    MaxNestingDepth = maxNestingDepth,
                },
                Version = version,
            };

            this.settings.ThrowOnDuplicatePropertyNames.Should().BeFalse();
            this.settings.BaseUri.Should().BeSameAs(baseUri);
            this.settings.EnableCharactersCheck.Should().BeTrue();
            this.settings.EnableMessageStreamDisposal.Should().BeFalse();
            this.settings.MessageQuotas.MaxPartsPerBatch.Should().Be(maxPartsPerBatch);
            this.settings.MessageQuotas.MaxOperationsPerChangeset.Should().Be(maxOperationsPerChangeset);
            this.settings.MessageQuotas.MaxNestingDepth.Should().Be(maxNestingDepth);
            this.settings.Version.Should().Be(version);
        }

        [Fact]
        public void PayloadBaseUriGetterAndSetterTest()
        {
            var setting = new ODataMessageWriterSettings()
            {
                BaseUri = new Uri("http://example.org/odata.svc"),
            };

            var t = setting.BaseUri.ToString();
            setting.BaseUri.ToString().Should().BeEquivalentTo("http://example.org/odata.svc/");

            setting = new ODataMessageWriterSettings()
            {
                BaseUri = new Uri("http://example.org/odata.svc/"),
            };
            setting.BaseUri.ToString().Should().BeEquivalentTo("http://example.org/odata.svc/");
        }
        #endregion Property getters and setters tests

        #region Error tests

        [Fact]
        public void MaxPartsPerBatchShouldThrowIfSetToNegativeNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxPartsPerBatch = -1;
            testSubject.ShouldThrow<ArgumentOutOfRangeException>().Where(e => e.Message.StartsWith(ODataErrorStrings.ExceptionUtils_CheckIntegerNotNegative(-1)));
        }

        [Fact]
        public void MaxOperationsPerChangesetShouldThrowIfSetToNegativeNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxOperationsPerChangeset = -1;
            testSubject.ShouldThrow<ArgumentOutOfRangeException>().Where(e => e.Message.StartsWith(ODataErrorStrings.ExceptionUtils_CheckIntegerNotNegative(-1)));
        }

        [Fact]
        public void MaxNestingDepthShouldThrowIfSetToNonPositiveNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxNestingDepth = 0;
            testSubject.ShouldThrow<ArgumentOutOfRangeException>().Where(e => e.Message.StartsWith(ODataErrorStrings.ExceptionUtils_CheckIntegerPositive(0)));
        }
        #endregion Error tests

        #region Set content type tests
        [Fact]
        public void SetContentTypeTest()
        {
            settings.SetContentType("application/xml", null);
            ODataMessageWriterSettings copyOfSettings = settings.Clone();
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().Be("application/xml");
            copyOfSettings.AcceptableMediaTypes.Should().Be("application/xml");
            settings.AcceptableCharsets.Should().BeNull();
            copyOfSettings.AcceptableCharsets.Should().BeNull();

            settings.SetContentType("application/json", "utf8");
            copyOfSettings = settings.Clone();
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().Be("application/json");
            copyOfSettings.AcceptableMediaTypes.Should().Be("application/json");
            settings.AcceptableCharsets.Should().Be("utf8");
            copyOfSettings.AcceptableCharsets.Should().Be("utf8");

            settings.SetContentType(null, "utf8");
            copyOfSettings = settings.Clone();
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().BeNull();
            copyOfSettings.AcceptableMediaTypes.Should().BeNull();
            settings.AcceptableCharsets.Should().Be("utf8");
            copyOfSettings.AcceptableCharsets.Should().Be("utf8");

            settings.SetContentType(string.Empty, "utf8");
            copyOfSettings = settings.Clone();
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().BeEmpty();
            copyOfSettings.AcceptableMediaTypes.Should().BeEmpty();
            settings.AcceptableCharsets.Should().Be("utf8");
            copyOfSettings.AcceptableCharsets.Should().Be("utf8");

            settings.SetContentType("json", "utf8");
            copyOfSettings = settings.Clone();
            settings.Format.Should().BeNull();
            copyOfSettings.Format.Should().BeNull();
            settings.AcceptableMediaTypes.Should().Be("application/json");
            copyOfSettings.AcceptableMediaTypes.Should().Be("application/json");
            settings.AcceptableCharsets.Should().Be("utf8");
            copyOfSettings.AcceptableCharsets.Should().Be("utf8");
        }
        #endregion Set content type tests
    }
}
