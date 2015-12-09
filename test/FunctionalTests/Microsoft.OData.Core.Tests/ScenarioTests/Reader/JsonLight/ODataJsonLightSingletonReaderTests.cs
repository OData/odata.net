//---------------------------------------------------------------------
// <copyright file="ODataJsonLightSingletonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Reader.JsonLight
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

            ODataEntry entry = this.ReadSingleton(payload);

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

            ODataEntry entry = this.ReadSingleton(payload);

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

            ODataEntry entry = this.ReadSingleton(payload);

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

            ODataEntry entry = this.ReadSingleton(payload);

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
            ODataNavigationLink navigationLink = this.ReadSingletonNavigationLink(payload);

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

            ODataNavigationLink navigationLink = this.ReadSingletonNavigationLink(payload);

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

            ODataEntry entry = this.ReadSingleton(payload);

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

        private void ReadSingletonWhichHasEtagImplementation(string payload, bool odataSimplified)
        {
            ODataEntry entry = this.ReadSingleton(payload, odataSimplified);

            entry.ETag.Should().Be("Bla");
        }

        [Fact]
        public void ReadSingletonWhichHasEtag()
        {
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.etag\":\"Bla\"}";
            ReadSingletonWhichHasEtagImplementation(payload, odataSimplified: false);
        }

        [Fact]
        public void ReadSingletonWhichHasSimplifiedEtagODataSimplified()
        {
            // cover "@etag"
            const string payload = "{" +
                "\"@context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@etag\":\"Bla\"}";
            ReadSingletonWhichHasEtagImplementation(payload, odataSimplified: true);
        }

        [Fact]
        public void ReadSingletonWhichHasFullEtagODataSimplified()
        {
            // cover "@odata.etag"
            const string payload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.etag\":\"Bla\"}";
            ReadSingletonWhichHasEtagImplementation(payload, odataSimplified: true);
        }

        #endregion

        #region ReadSingletonWhichHasStreamPropertyTest

        private void ReadSingletonWhichHasStreamPropertyTestImplementation(string payload, bool odataSimplified)
        {
            this.StreamTestSetting();
            ODataEntry entry = this.ReadSingleton(payload, odataSimplified);

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
            ReadSingletonWhichHasStreamPropertyTestImplementation(payload, odataSimplified: false);
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
            ReadSingletonWhichHasStreamPropertyTestImplementation(payload, odataSimplified: true);
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
            ReadSingletonWhichHasStreamPropertyTestImplementation(payload, odataSimplified: true);
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

            ODataEntry entry = this.ReadSingleton(payload);

            entry.Properties.Count().Should().Be(3);
            entry.Properties.Single(p => p.Name == "OpenType2").Value.Should().Be("BlaBla");
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
            ODataEntry entry = this.ReadSingleton(payload);

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

        private ODataEntry ReadSingleton(string payload, bool odataSimplified = false)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings { ODataSimplified = odataSimplified };

            using (ODataJsonLightInputContext inputContext = new ODataJsonLightInputContext(
                ODataFormat.Json,
                stream,
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                /*readingResponse*/ true,
                /*synchronous*/ true,
                this.userModel,
                /*urlResolver*/ null))
            {
                var jsonLightReader = new ODataJsonLightReader(inputContext, singleton, webType, /*readingFeed*/ false);
                while (jsonLightReader.Read())
                {
                    if (jsonLightReader.State == ODataReaderState.EntryEnd)
                    {
                        ODataEntry entry = jsonLightReader.Item as ODataEntry;
                        return entry;
                    }
                }
            }
            return null;
        }

        private ODataNavigationLink ReadSingletonNavigationLink(string payload)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();

            using (ODataJsonLightInputContext inputContext = new ODataJsonLightInputContext(
                ODataFormat.Json,
                stream,
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                /*readingResponse*/ true,
                /*synchronous*/ true,
                this.userModel,
                /*urlResolver*/ null))
            {
                var jsonLightReader = new ODataJsonLightReader(inputContext, singleton, webType, /*readingFeed*/ false);
                while (jsonLightReader.Read())
                {
                    if (jsonLightReader.State == ODataReaderState.NavigationLinkEnd)
                    {
                        ODataNavigationLink navigationLink = jsonLightReader.Item as ODataNavigationLink;
                        return navigationLink;
                    }
                }
            }
            return null;
        }
    }
}
