//---------------------------------------------------------------------
// <copyright file="ODataJsonLightSingletonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Reader.JsonLight
{
    public class ODataJsonLightSingletonReaderTests
    {
        private readonly Uri serviceDocumentUri = new Uri("http://odata.org/test/");
        private EdmEntityContainer defaultContainer;
        private EdmModel referencedModel;
        private IEdmModel userModel;
        private EdmSingleton singleton;
        private EdmEntityType webType;
        private EdmEntityType pageType;

        public ODataJsonLightSingletonReaderTests()
        {
            EdmModel tmpModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web");
            this.webType.AddStructuralProperty("WebId", EdmPrimitiveTypeKind.Int32);
            this.webType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            tmpModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            tmpModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            this.defaultContainer.AddElement(this.singleton);

            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", tmpModel);
            this.referencedModel = tmpModel;
        }

        [Fact]
        public void ReadSingletonWithOnlyContexUrlTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"}";

            ODataResource entry = this.ReadSingleton(payload);

            entry.Id.Should().Be("http://odata.org/test/MySingleton");
            entry.TypeName.Should().Be("NS.Web");
            entry.EditLink.Should().Be("http://odata.org/test/MySingleton");
            entry.Properties.Count().Should().Be(0);
        }

        [Fact]
        public void ReadBasicSingletonTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"WebId\":10," +
                "\"Name\":\"SingletonWeb\"}";

            ODataResource entry = this.ReadSingleton(payload);

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Single(p => p.Name == "WebId").Value.Should().Be(10);
            entry.Properties.Single(p => p.Name == "Name").Value.Should().Be("SingletonWeb");
        }

        [Fact]
        public void ReadSingletonWithIdSpecifiedTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.id\":\"Bla\"," +
                "\"@odata.editLink\":\"BlaBla\"," +
                "\"@odata.readLink\":\"BlaBlaBla\"}";

            ODataResource entry = this.ReadSingleton(payload);

            entry.Id.Should().Be("http://odata.org/test/Bla");
            entry.EditLink.Should().Be("http://odata.org/test/BlaBla");
            entry.ReadLink.Should().Be("http://odata.org/test/BlaBlaBla");
        }

        [Fact]
        public void ReadSingletonWithContextUriHasKey()
        {
            const string payload = "{" +
               "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton(10)\"}";

            Action readResult = () => this.ReadSingleton(payload);
            readResult.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightContextUriParser_LastSegmentIsKeySegment("http://odata.org/test/$metadata#MySingleton(10)"));
        }

        [Fact]
        public void ReadSingletonWithMissingPropertyTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"WebId\":10}";

            ODataResource entry = this.ReadSingleton(payload);

            entry.Properties.Count().Should().Be(1);
            entry.Properties.Single().Value.Should().Be(10);
        }

        [Fact]
        public void ReadSingletonWithAbsoluteNavigationLinkTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"Pages@odata.navigationLink\":\"http://odata.org/test/MySingleton/Pages\"," +
                "\"Pages@odata.associationLink\":\"http://odata.org/test/MySingleton/Pages/$ref\"}";

            this.ReadAndVerifySingletonNavigationLink(payload);
        }

        [Fact]
        public void ReadSingletonWithRelativeNavigationLinkTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"Pages@odata.navigationLink\":\"MySingleton/Pages\"," +
                "\"Pages@odata.associationLink\":\"MySingleton/Pages/$ref\"}";

            this.ReadAndVerifySingletonNavigationLink(payload);
        }

        [Fact]
        public void ReadSingletonWithNoNavigationLinkSpecifiedTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"}";

            this.ReadAndVerifySingletonNavigationLink(payload);
        }

        [Fact]
        public void ReadSingletonWithOnlyNavigationLinkSpecfiedTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"Pages@odata.navigationLink\":\"MySingleton/Pages\"}";

            this.ReadAndVerifySingletonNavigationLink(payload);
        }

        private void ReadAndVerifySingletonNavigationLink(string payload)
        {
            this.NavigationLinkTestSetting();
            ODataNestedResourceInfo navigationLink = this.ReadSingletonNavigationLink(payload);

            navigationLink.Name.Should().Be("Pages");
            navigationLink.AssociationLinkUrl.Should().Be("http://odata.org/test/MySingleton/Pages/$ref");
        }

        [Fact]
        public void ReadSingletonWithArbitraryNavigationLinkTest()
        {
            this.NavigationLinkTestSetting();
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"Pages@odata.navigationLink\":\"Bla\"," +
                "\"Pages@odata.associationLink\":\"BlaBlaBla\"}";

            ODataNestedResourceInfo navigationLink = this.ReadSingletonNavigationLink(payload);

            navigationLink.Name.Should().Be("Pages");
            navigationLink.AssociationLinkUrl.Should().Be("http://odata.org/test/BlaBlaBla");
        }

        private void NavigationLinkTestSetting()
        {
            this.pageType = new EdmEntityType("NS", "Page");
            this.pageType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
            this.pageType.AddStructuralProperty("PageLink", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(this.pageType);

            this.webType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Pages",
                Target = this.pageType,
                TargetMultiplicity = EdmMultiplicity.Many
            });
        }

        [Fact]
        public void ReadSingletonWhichIsMediaEntryTest()
        {
            this.MediaEntrySetSetting();
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.mediaContentType\":\"image/jpeg\"," +
                "\"@odata.mediaReadLink\":\"Bla\"}";

            ODataResource entry = this.ReadSingleton(payload);

            entry.MediaResource.ReadLink.Should().Be("http://odata.org/test/Bla");
            entry.MediaResource.ContentType.Should().Be("image/jpeg");
        }

        private void MediaEntrySetSetting()
        {
            EdmModel tmpModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web", null, /*isAbstract*/false, /*isOpen*/false, /*hasStream*/true);
            tmpModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            tmpModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            defaultContainer.AddElement(this.singleton);

            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", tmpModel);
            this.referencedModel = tmpModel;
        }

        #region ReadSingletonWhichHasEtag

        private void ReadSingletonWhichHasEtagImplementation(string payload, bool enableReadingODataAnnotationWithoutPrefix)
        {
            ODataResource entry = this.ReadSingleton(payload, enableReadingODataAnnotationWithoutPrefix);

            entry.ETag.Should().Be("Bla");
        }

        [Fact]
        public void ReadSingletonWhichHasEtag()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.etag\":\"Bla\"}";
            ReadSingletonWhichHasEtagImplementation(payload, enableReadingODataAnnotationWithoutPrefix: false);
        }

        [Fact]
        public void ReadSingletonWhichHasSimplifiedEtagODataSimplified()
        {
            // cover "@etag"
            const string payload = "{" +
                "\"@context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@etag\":\"Bla\"}";
            ReadSingletonWhichHasEtagImplementation(payload, enableReadingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void ReadSingletonWhichHasFullEtagODataSimplified()
        {
            // cover "@odata.etag"
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.etag\":\"Bla\"}";
            ReadSingletonWhichHasEtagImplementation(payload, enableReadingODataAnnotationWithoutPrefix: true);
        }

        #endregion

        #region ReadSingletonWhichHasStreamPropertyTest

        private void ReadSingletonWhichHasStreamPropertyTestImplementation(string payload, bool enableReadingODataAnnotationWithoutPrefix)
        {
            this.StreamTestSetting();
            ODataResource entry = this.ReadSingleton(payload, enableReadingODataAnnotationWithoutPrefix);

            ODataStreamReferenceValue logo = (ODataStreamReferenceValue)entry.Properties.Single().Value;
            logo.ContentType.Should().Be("image/jpeg");
            logo.ETag.Should().Be("stream etag");
        }

        [Fact]
        public void ReadSingletonWhichHasStreamPropertyTest()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"Logo@odata.mediaEditLink\":\"http://example.com/stream/edit\"," +
                "\"Logo@odata.mediaReadLink\":\"http://example.com/stream/read\"," +
                "\"Logo@odata.mediaContentType\":\"image/jpeg\"," +
                "\"Logo@odata.mediaEtag\":\"stream etag\"}";
            ReadSingletonWhichHasStreamPropertyTestImplementation(payload, enableReadingODataAnnotationWithoutPrefix: false);
        }

        [Fact]
        public void ReadSingletonWhichHasStreamPropertyTestFullODataMediaAnnotationsODataSimplified()
        {
            // cover "prop@odata.media*"
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"Logo@odata.mediaEditLink\":\"http://example.com/stream/edit\"," +
                "\"Logo@odata.mediaReadLink\":\"http://example.com/stream/read\"," +
                "\"Logo@odata.mediaContentType\":\"image/jpeg\"," +
                "\"Logo@odata.mediaEtag\":\"stream etag\"}";
            ReadSingletonWhichHasStreamPropertyTestImplementation(payload, enableReadingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void ReadSingletonWhichHasStreamPropertyTestSimplifiedODataMediaAnnotationsODataSimplified()
        {
            // cover "prop@media*"
            const string payload = "{" +
                "\"@context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"Logo@mediaEditLink\":\"http://example.com/stream/edit\"," +
                "\"Logo@mediaReadLink\":\"http://example.com/stream/read\"," +
                "\"Logo@mediaContentType\":\"image/jpeg\"," +
                "\"Logo@mediaEtag\":\"stream etag\"}";
            ReadSingletonWhichHasStreamPropertyTestImplementation(payload, enableReadingODataAnnotationWithoutPrefix: true);
        }

        #endregion

        private void StreamTestSetting()
        {
            EdmModel tmpModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web");
            this.webType.AddStructuralProperty("Logo", EdmPrimitiveTypeKind.Stream);
            tmpModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            tmpModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            defaultContainer.AddElement(this.singleton);

            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", tmpModel);
            this.referencedModel = tmpModel;
        }

        [Fact]
        public void ReadSingletonWithOpenTypePropertiesTest()
        {
            this.OpenTypeTestSetting();
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"WebId\":10," +
                "\"OpenType1\":\"Bla\"," +
                "\"OpenType2\":\"BlaBla\"}";

            ODataResource entry = this.ReadSingleton(payload);

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(p => p.Name == "OpenType2").Value.As<ODataUntypedValue>().RawValue.Should().Be("\"BlaBla\"");
        }

        private void OpenTypeTestSetting()
        {
            EdmModel tmpModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web", null, false, true);
            this.webType.AddStructuralProperty("WebId", EdmPrimitiveTypeKind.Int32);
            tmpModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            tmpModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            this.defaultContainer.AddElement(this.singleton);

            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", tmpModel);
            this.referencedModel = tmpModel;
        }

        [Fact]
        public void ReadSingletonWithBoundFunctionTest()
        {
            this.BoundFunctionTestSetting();
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"#NS.SingletonFunction\":{" +
                    "\"title\":\"NS.SingletonFunction\"," +
                    "\"target\":\"MySingleton/NS.SingletonFunction\"" +
                    "}" +
                "}";
            ODataResource entry = this.ReadSingleton(payload);

            entry.Functions.Count().Should().Be(1);
            entry.Functions.Single().Title.Should().Be("NS.SingletonFunction");
            entry.Functions.Single().Target.Should().Be("http://odata.org/test/MySingleton/NS.SingletonFunction");
        }

        private void BoundFunctionTestSetting()
        {
            EdmFunction function = new EdmFunction("NS", "SingletonFunction", EdmCoreModel.Instance.GetInt32(false), true, null, false);
            function.AddParameter(new EdmOperationParameter(function, "p", new EdmEntityTypeReference(this.webType, false)));
            this.referencedModel.AddElement(function);
        }

        private ODataResource ReadSingleton(string payload, bool odataSimplified = false)
        {
            var settings = new ODataMessageReaderSettings();

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = false,
                Model = this.userModel,
                Container = ContainerBuilderHelper.BuildContainer(null)
            };

            using (var inputContext = new ODataJsonLightInputContext(
                new StringReader(payload), messageInfo, settings))
            {
                inputContext.Container.GetRequiredService<ODataSimplifiedOptions>()
                    .EnableReadingODataAnnotationWithoutPrefix = odataSimplified;
                var jsonLightReader = new ODataJsonLightReader(inputContext, singleton, webType, /*readingFeed*/ false);
                while (jsonLightReader.Read())
                {
                    if (jsonLightReader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = jsonLightReader.Item as ODataResource;
                        return entry;
                    }
                }
            }
            return null;
        }

        private ODataNestedResourceInfo ReadSingletonNavigationLink(string payload)
        {
            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = false,
                Model = this.userModel,
            };

            using (var inputContext = new ODataJsonLightInputContext(
                new StringReader(payload), messageInfo, new ODataMessageReaderSettings()))
            {
                var jsonLightReader = new ODataJsonLightReader(inputContext, singleton, webType, /*readingFeed*/ false);
                while (jsonLightReader.Read())
                {
                    if (jsonLightReader.State == ODataReaderState.NestedResourceInfoEnd)
                    {
                        ODataNestedResourceInfo navigationLink = jsonLightReader.Item as ODataNestedResourceInfo;
                        return navigationLink;
                    }
                }
            }
            return null;
        }
    }
}
