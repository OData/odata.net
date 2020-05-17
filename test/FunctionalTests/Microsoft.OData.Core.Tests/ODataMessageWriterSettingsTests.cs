//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettingsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
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
            Assert.Null(this.settings.ShouldIncludeAnnotation);
        }

        [Fact]
        public void ShouldBeAbleToSetAnnotationFilter()
        {
            Func<string, bool> filter = name => true;

            this.settings.ShouldIncludeAnnotation = filter;
            Assert.Same(this.settings.ShouldIncludeAnnotation, filter);
        }

        [Fact]
        public void ShouldSkipAnnotationsByDefaultForV4()
        {
            this.settings.Version = ODataVersion.V4;
            Assert.True(this.settings.ShouldSkipAnnotation("any.any"));
        }

        [Fact]
        public void ShouldHonorAnnotationFilterForV4()
        {
            this.settings.Version = ODataVersion.V4;
            this.settings.ShouldIncludeAnnotation = name => name.StartsWith("ns1.");
            Assert.True(this.settings.ShouldSkipAnnotation("any.any"));
            Assert.False(this.settings.ShouldSkipAnnotation("ns1.any"));
        }

        #region metadata document uri tests
        [Fact]
        public void WriterSettingsIntegrationTest()
        {
            var settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(ServiceDocumentUri);
            Assert.Equal(settings.MetadataDocumentUri, new Uri(ServiceDocumentUri + "/$metadata"));

            settings.SetServiceDocumentUri(null);
            Assert.Null(settings.MetadataDocumentUri);
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
            Assert.Null(setting.MetadataDocumentUri);
            string select, expand;
            foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
            {
                setting.SelectExpandClause.GetSelectExpandPaths(version, out select, out expand);
                Assert.Equal("*", select);
            }
        }
        #endregion metadata document uri tests

        #region Default settings tests
        [Fact]
        public void BaseUriShouldBeNullByDefault()
        {
            Assert.Null(this.settings.BaseUri);
        }

        [Fact]
        public void CheckCharactersShouldBeFalseByDefault()
        {
            Assert.False(this.settings.EnableCharactersCheck);
        }

        [Fact]
        public void EnableMessageStreamDisposalShouldBeTrueByDefault()
        {
            Assert.True(this.settings.EnableMessageStreamDisposal);
        }

        [Fact]
        public void JsonPCallbackShouldBeNullByDefault()
        {
            Assert.Null(this.settings.JsonPCallback);
        }

        [Fact]
        public void LibraryCompatibilityShouldBeLatestByDefault()
        {
            Assert.Equal(ODataLibraryCompatibility.Latest, this.settings.LibraryCompatibility);
        }

        [Fact]
        public void VersionShouldBeNullByDefault()
        {
            Assert.Null(this.settings.Version);
        }

        [Fact]
        public void MaxReceivedMessageSizeShouldBeSetByDefault()
        {
            Assert.Equal(this.settings.MessageQuotas.MaxReceivedMessageSize, 1024 * 1024);
        }

        [Fact]
        public void MaxPartsPerBatchShouldBeSetByDefault()
        {
            Assert.Equal(100, this.settings.MessageQuotas.MaxPartsPerBatch);
        }

        [Fact]
        public void MaxOperationsPerChangesetShouldBeSetByDefault()
        {
            Assert.Equal(1000, this.settings.MessageQuotas.MaxOperationsPerChangeset);
        }

        [Fact]
        public void MaxNestingDepthShouldBeSetByDefault()
        {
            Assert.Equal(100, this.settings.MessageQuotas.MaxNestingDepth);
        }

        [Fact]
        public void ODataUriShouldBeNotNullByDefault()
        {
            Assert.NotNull(this.settings.ODataUri);
        }

        [Fact]
        public void IsIndividualPropertyShouldBeFalseByDefault()
        {
            Assert.False(this.settings.IsIndividualProperty);
        }

        [Fact]
        public void SelectExpandClauseBeNullByDefault()
        {
            Assert.Null(this.settings.SelectExpandClause);
        }

        [Fact]
        public void AllowDuplicatePropertyNamesShouldBeFalseByDefault()
        {
            Assert.True(this.settings.ThrowOnDuplicatePropertyNames);
        }

        #endregion Default settings tests

        #region Copy constructor tests
        [Fact]
        public void CopyConstructorShouldCopyBaseUri()
        {
            var baseUri = new Uri("http://otherservice.svc");
            this.settings.BaseUri = baseUri;
            Assert.Same(this.settings.Clone().BaseUri, baseUri);
        }

        [Fact]
        public void CopyConstructorShouldCopyCheckCharacters()
        {
            this.settings.EnableCharactersCheck = true;
            Assert.True(this.settings.Clone().EnableCharactersCheck);
        }

        [Fact]
        public void CopyConstructorShouldCopyEnableMessageStreamDisposal()
        {
            this.settings.EnableMessageStreamDisposal = false;
            Assert.False(this.settings.Clone().EnableMessageStreamDisposal);
        }

        [Fact]
        public void CopyConstructorShouldCopyJsonPCallback()
        {
            this.settings.JsonPCallback = "jsonp";
            Assert.Equal("jsonp", this.settings.Clone().JsonPCallback);
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

            Assert.Equal(copiedQuotas.MaxNestingDepth, originalQuotas.MaxNestingDepth);
            Assert.Equal(copiedQuotas.MaxOperationsPerChangeset, originalQuotas.MaxOperationsPerChangeset);
            Assert.Equal(copiedQuotas.MaxPartsPerBatch, originalQuotas.MaxPartsPerBatch);
            Assert.Equal(copiedQuotas.MaxReceivedMessageSize, originalQuotas.MaxReceivedMessageSize);
        }

        [Fact]
        public void CopyConstructorShouldCopyVersion()
        {
            this.settings.Version = ODataVersion.V4;
            Assert.Equal(ODataVersion.V4, this.settings.Clone().Version);
        }

        [Fact]
        public void CopyConstructorShouldCopyLibraryCompatibility()
        {
            this.settings.LibraryCompatibility = ODataLibraryCompatibility.Version6;
            Assert.Equal(ODataLibraryCompatibility.Version6, this.settings.Clone().LibraryCompatibility);
        }

        [Fact]
        public void CopyConstructorShouldCopyAnnotationFilter()
        {
            Func<string, bool> filter = name => true;

            this.settings.ShouldIncludeAnnotation = filter;
            Assert.Same(this.settings.Clone().ShouldIncludeAnnotation, filter);
        }

        [Fact]
        public void CopyConstructorShouldCopyMetadataDocumentUri()
        {
            this.settings.SetServiceDocumentUri(new Uri("http://example.com"));
            Assert.Equal(this.settings.Clone().MetadataDocumentUri, new Uri("http://example.com/$metadata"));
        }

        [Fact]
        public void CopyConstructorShouldCopyFormat()
        {
            this.settings.SetContentType(ODataFormat.Metadata);
            Assert.Equal(this.settings.Clone().Format, ODataFormat.Metadata);
        }

        [Fact]
        public void CopyConstructorShouldCopyAccept()
        {
            this.settings.SetContentType("application/json,application/atom+xml", "iso-8859-5, unicode-1-1;q=0.8");
            var copiedSettings = this.settings.Clone();
            Assert.Equal("iso-8859-5, unicode-1-1;q=0.8", copiedSettings.AcceptableCharsets);
            Assert.Equal("application/json,application/atom+xml", copiedSettings.AcceptableMediaTypes);
        }

        [Fact]
        public void CopyConstructorShouldCopyServiceDocument()
        {
            this.settings.SetServiceDocumentUri(new Uri("http://test.org"));
            var newSetting = this.settings.Clone();
            Assert.Equal(newSetting.MetadataDocumentUri, new Uri("http://test.org/$metadata"));
        }

        [Fact]
        public void CopyConstructorShouldCopyMetadataSelector()
        {
            this.settings.MetadataSelector = new TestMetadataSelector() { PropertyToOmit = "TestProperty" };
            var newSetting = this.settings.Clone();
            Assert.Equal("TestProperty", (newSetting.MetadataSelector as TestMetadataSelector).PropertyToOmit);
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
            Assert.Equal(newSetting.MetadataDocumentUri, new Uri("http://test.org/$metadata"));
            Assert.Equal("Cities(1)/Name", newSetting.ODataUri.Path.ToResourcePathString(ODataUrlKeyDelimiter.Parentheses));
            Assert.True(newSetting.IsIndividualProperty);

            string select, expand;
            foreach (ODataVersion version in new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 })
            {
                newSetting.SelectExpandClause.GetSelectExpandPaths(version, out select, out expand);
                Assert.Equal("*", select);
            }
        }

        [Fact]
        public void CopyConstructorShouldCopyAll()
        {
            this.settings.MetadataSelector = new TestMetadataSelector() { PropertyToOmit = "TestProperty" };
            this.settings.SetContentType("application/json,application/atom+xml", "iso-8859-5, unicode-1-1;q=0.8");
            this.settings.SetServiceDocumentUri(new Uri("http://example.com"));
            this.settings.ArrayPool = new TestCharArrayPool(5);
            this.settings.EnableMessageStreamDisposal = false;
            this.settings.EnableCharactersCheck = true;

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

            this.settings.LibraryCompatibility = ODataLibraryCompatibility.Version6;
            this.settings.Version = ODataVersion.V4;

            Func<string, bool> filter = name => true;
            this.settings.ShouldIncludeAnnotation = filter;

            var newSetting = this.settings.Clone();

            var differences = ValidationHelper.GetDifferences<ODataMessageWriterSettings>(this.settings, newSetting);
            Assert.True(differences.Count == 0, String.Join(",", differences));
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
            const ODataLibraryCompatibility library = ODataLibraryCompatibility.Version6;

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
                LibraryCompatibility = library,
            };

            Assert.False(this.settings.ThrowOnDuplicatePropertyNames);
            Assert.Same(this.settings.BaseUri, baseUri);
            Assert.True(this.settings.EnableCharactersCheck);
            Assert.False(this.settings.EnableMessageStreamDisposal);
            Assert.Equal(this.settings.MessageQuotas.MaxPartsPerBatch, maxPartsPerBatch);
            Assert.Equal(this.settings.MessageQuotas.MaxOperationsPerChangeset, maxOperationsPerChangeset);
            Assert.Equal(this.settings.MessageQuotas.MaxNestingDepth, maxNestingDepth);
            Assert.Equal(this.settings.Version, version);
            Assert.Equal(this.settings.LibraryCompatibility, library);
        }

        [Fact]
        public void PayloadBaseUriGetterAndSetterTest()
        {
            var setting = new ODataMessageWriterSettings()
            {
                BaseUri = new Uri("http://example.org/odata.svc"),
            };

            var t = setting.BaseUri.ToString();
            Assert.Equal("http://example.org/odata.svc/", setting.BaseUri.ToString());

            setting = new ODataMessageWriterSettings()
            {
                BaseUri = new Uri("http://example.org/odata.svc/"),
            };
            Assert.Equal("http://example.org/odata.svc/", setting.BaseUri.ToString());
        }
        #endregion Property getters and setters tests

        #region Error tests

        [Fact]
        public void MaxPartsPerBatchShouldThrowIfSetToNegativeNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxPartsPerBatch = -1;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(testSubject);
            Assert.StartsWith(ODataErrorStrings.ExceptionUtils_CheckIntegerNotNegative(-1), exception.Message);
        }

        [Fact]
        public void MaxOperationsPerChangesetShouldThrowIfSetToNegativeNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxOperationsPerChangeset = -1;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(testSubject);
            Assert.StartsWith(ODataErrorStrings.ExceptionUtils_CheckIntegerNotNegative(-1), exception.Message);
        }

        [Fact]
        public void MaxNestingDepthShouldThrowIfSetToNonPositiveNumber()
        {
            Action testSubject = () => this.settings.MessageQuotas.MaxNestingDepth = 0;
            var exception = Assert.Throws<ArgumentOutOfRangeException>(testSubject);
            Assert.StartsWith(ODataErrorStrings.ExceptionUtils_CheckIntegerPositive(0), exception.Message);
        }
        #endregion Error tests

        #region Set content type tests
        [Fact]
        public void SetContentTypeTest()
        {
            settings.SetContentType("application/xml", null);
            ODataMessageWriterSettings copyOfSettings = settings.Clone();
            Assert.Null(settings.Format);
            Assert.Null(copyOfSettings.Format);
            Assert.Equal("application/xml", settings.AcceptableMediaTypes);
            Assert.Equal("application/xml", copyOfSettings.AcceptableMediaTypes);
            Assert.Null(settings.AcceptableCharsets);
            Assert.Null(copyOfSettings.AcceptableCharsets);

            settings.SetContentType("application/json", "utf8");
            copyOfSettings = settings.Clone();
            Assert.Null(settings.Format);
            Assert.Null(copyOfSettings.Format);
            Assert.Equal("application/json", settings.AcceptableMediaTypes);
            Assert.Equal("application/json", copyOfSettings.AcceptableMediaTypes);
            Assert.Equal("utf8", settings.AcceptableCharsets);
            Assert.Equal("utf8", copyOfSettings.AcceptableCharsets);

            settings.SetContentType(null, "utf8");
            copyOfSettings = settings.Clone();
            Assert.Null(settings.Format);
            Assert.Null(copyOfSettings.Format);
            Assert.Null(settings.AcceptableMediaTypes);
            Assert.Null(copyOfSettings.AcceptableMediaTypes);
            Assert.Equal("utf8", settings.AcceptableCharsets);
            Assert.Equal("utf8", copyOfSettings.AcceptableCharsets);

            settings.SetContentType(string.Empty, "utf8");
            copyOfSettings = settings.Clone();
            Assert.Null(settings.Format);
            Assert.Null(copyOfSettings.Format);
            Assert.Empty(settings.AcceptableMediaTypes);
            Assert.Empty(copyOfSettings.AcceptableMediaTypes);
            Assert.Equal("utf8", settings.AcceptableCharsets);
            Assert.Equal("utf8", copyOfSettings.AcceptableCharsets);

            settings.SetContentType("json", "utf8");
            copyOfSettings = settings.Clone();
            Assert.Null(settings.Format);
            Assert.Null(copyOfSettings.Format);
            Assert.Equal("application/json", settings.AcceptableMediaTypes);
            Assert.Equal("application/json", copyOfSettings.AcceptableMediaTypes);
            Assert.Equal("utf8", settings.AcceptableCharsets);
            Assert.Equal("utf8", copyOfSettings.AcceptableCharsets);
        }
        #endregion Set content type tests
    }
}
