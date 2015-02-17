//---------------------------------------------------------------------
// <copyright file="ODataJsonLightServiceDocumentDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataJsonLightServiceDocumentDeserializerTests
    {
        private const string DefaultEmptyServiceDocumentStarter = @"{
  ""@odata.context"":""http://odata.org/$metadata"",""value"":[
    REPLACE
  ]
}";

        [TestMethod]
        public void ReadServiceDocumentWithFunctionImportInfo()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""functionImport"",""url"":""http://service/functionimport"", ""kind"":""FunctionImport""}"));
            serviceDocument.FunctionImports.Should().NotBeNull();
            var functionImports = serviceDocument.FunctionImports.ToList();
            functionImports.Count.Should().Be(1);
            functionImports[0].Name.Should().Be("functionImport");
            functionImports[0].Url.ToString().Should().Be("http://service/functionimport");
        }

        #region Singleton service document test

        [TestMethod]
        public void ReadServiceDocumentWithSingletonInfo()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""singleton"",""url"":""http://service/singleton"", ""kind"":""Singleton""}"));
            serviceDocument.Singletons.Should().NotBeEmpty();
            var singletons = serviceDocument.Singletons.ToList();
            singletons.Count.Should().Be(1);
            singletons[0].Name.Should().Be("singleton");
            singletons[0].Url.ToString().Should().Be("http://service/singleton");
        }

        // TODO: Json ServiceDocumentReader can not accept a name/value pair with name title
        [Ignore] 
        [TestMethod]
        public void ReadServiceDocumentWithSingletonInfoWithRelativeUrlAndTitle()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""singleton"",""url"":""singleton"", ""kind"":""Singleton"", ""title"":""Singleton Test""}"));
            serviceDocument.Singletons.Should().NotBeEmpty();
            var singletons = serviceDocument.Singletons.ToList();
            singletons.Count.Should().Be(1);
            singletons[0].Name.Should().Be("singleton");
            singletons[0].Url.ToString().Should().Be("http://odata.org/singleton");
        }

        [TestMethod]
        public void ReadServiceDocumentWithMultiSingletonInfo()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""singleton"",""url"":""singleton"", ""kind"":""Singleton""}, {""name"":""singleton1"",""url"":""http://service/singleton1"", ""kind"":""Singleton""}"));
            serviceDocument.Singletons.Should().NotBeEmpty();
            var singletons = serviceDocument.Singletons.ToList();
            singletons.Count.Should().Be(2);
            singletons[0].Name.Should().Be("singleton");
            singletons[0].Url.ToString().Should().Be("http://odata.org/singleton");
            singletons[1].Name.Should().Be("singleton1");
            singletons[1].Url.ToString().Should().Be("http://service/singleton1");
        }

        [TestMethod]
        public void ReadServiceDocumentWithSingletonInfoWithNameAbsent() 
        {
            Action test = () => ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""url"":""singleton"", ""kind"":""Singleton""}"));
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementName));
        }

        [TestMethod]
        public void ReadServiceDocumentWithSingletonInfoWithKindAbsent() 
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""url"":""singleton"", ""name"":""singleton""}"));
            serviceDocument.Singletons.Should().BeEmpty();
            serviceDocument.EntitySets.Should().NotBeEmpty();
        }

        [TestMethod]
        public void ReaderServiceDocumentWithSingletonInfoWithEmptyUrl() 
        {
            Action test = () => ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""url"":"""", ""name"":""singleton""}"));
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementUrlName));
        }
        #endregion

        [TestMethod]
        public void ReadServiceDocumentWithEntitySetInfo()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""entityset"",""url"":""http://service/entityset"", ""kind"":""EntitySet""}"));
            this.TestEntitySetInServiceDocument(serviceDocument);
        }

        [TestMethod]
        public void ReadServiceDocumentWithEntitySetInfoWithNoKindSpecified()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""entityset"",""url"":""http://service/entityset""}"));
            this.TestEntitySetInServiceDocument(serviceDocument);
        }

        [TestMethod]
        public void ReadServiceDocumentShouldIgnoreUnknownKind()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""entityset"",""url"":""http://service/entityset"", ""kind"":""somethingelse""}"));
            var entitySets = serviceDocument.EntitySets.ToList();
            entitySets.Count.Should().Be(0);
        }

        [TestMethod]
        public void ReadServiceDocumentWithTitle()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE",
@"{""name"":""entityset"",""url"":""http://service/entityset"", ""kind"":""EntitySet"", ""title"":""some entity set"" },
  {""name"":""singleton"",""url"":""http://service/singleton"", ""kind"":""Singleton"", ""title"":""some singleton"" },
  {""name"":""singleton2"",""url"":""http://service/singleton2"", ""kind"":""Singleton"" },
  {""name"":""functionImport"",""url"":""http://service/functionimport"", ""kind"":""FunctionImport"", ""title"":""some function import""}"));

            serviceDocument.EntitySets.Should().NotBeNull();
            var entitySets = serviceDocument.EntitySets.ToList();
            entitySets.Count.Should().Be(1);
            entitySets[0].Name.Should().Be("entityset");
            entitySets[0].Url.ToString().Should().Be("http://service/entityset");
            entitySets[0].Title.Should().Be("some entity set");

            serviceDocument.Singletons.Should().NotBeNull();
            var singleton = serviceDocument.Singletons.ToList();
            singleton.Count.Should().Be(2);
            singleton[0].Name.Should().Be("singleton");
            singleton[0].Url.ToString().Should().Be("http://service/singleton");
            singleton[0].Title.Should().Be("some singleton");
            singleton[1].Name.Should().Be("singleton2");
            singleton[1].Url.ToString().Should().Be("http://service/singleton2");
            singleton[1].Title.Should().Be(null);

            serviceDocument.FunctionImports.Should().NotBeNull();
            var functionImports = serviceDocument.FunctionImports.ToList();
            functionImports.Count.Should().Be(1);
            functionImports[0].Name.Should().Be("functionImport");
            functionImports[0].Url.ToString().Should().Be("http://service/functionimport");
            functionImports[0].Title.Should().Be("some function import");
        }

        private ODataServiceDocument ReadServiceDocument(string payload)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            ODataJsonLightServiceDocumentDeserializer deserializer = CreateODataJsonServiceDocumentDeserializer(stream);
            return deserializer.ReadServiceDocument();
        }

        private ODataJsonLightServiceDocumentDeserializer CreateODataJsonServiceDocumentDeserializer(MemoryStream stream, IODataUrlResolver urlResolver = null)
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();

            ODataJsonLightInputContext inputContext = new ODataJsonLightInputContext(ODataFormat.Json, stream, new ODataMediaType("application", "json"), Encoding.UTF8, settings, true /*readingResponse*/, true /*sync*/, new EdmModel() /*edmModel*/, urlResolver);
            return new ODataJsonLightServiceDocumentDeserializer(inputContext);
        }

        private void TestEntitySetInServiceDocument(ODataServiceDocument serviceDocument)
        {
            serviceDocument.EntitySets.Should().NotBeNull();
            var entitySets = serviceDocument.EntitySets.ToList();
            entitySets.Count.Should().Be(1);
            entitySets[0].Name.Should().Be("entityset");
            entitySets[0].Url.ToString().Should().Be("http://service/entityset");
        }
    }
}
