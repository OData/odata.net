//---------------------------------------------------------------------
// <copyright file="ODataJsonLightServiceDocumentDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightServiceDocumentDeserializerTests
    {
        private const string DefaultEmptyServiceDocumentStarter = @"{
  ""@odata.context"":""http://odata.org/$metadata"",""value"":[
    REPLACE
  ]
}";

        [Fact]
        public void ReadServiceDocumentWithFunctionImportInfo()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""functionImport"",""url"":""http://service/functionimport"", ""kind"":""FunctionImport""}"));
            Assert.NotNull(serviceDocument.FunctionImports);
            var functionImport = Assert.Single(serviceDocument.FunctionImports);
            Assert.Equal("functionImport", functionImport.Name);
            Assert.Equal("http://service/functionimport", functionImport.Url.ToString());
        }

        #region Singleton service document test

        [Fact]
        public void ReadServiceDocumentWithSingletonInfo()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""singleton"",""url"":""http://service/singleton"", ""kind"":""Singleton""}"));
            Assert.NotEmpty(serviceDocument.Singletons);
            var singleton = Assert.Single(serviceDocument.Singletons);
            Assert.Equal("singleton", singleton.Name);
            Assert.Equal("http://service/singleton", singleton.Url.ToString());
        }

        [Fact]
        public void ReadServiceDocumentWithSingletonInfoWithRelativeUrlAndTitle()
        {
            // Json ServiceDocumentReader can not accept a name/value pair with name title
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""singleton"",""url"":""singleton"", ""kind"":""Singleton"", ""title"":""Singleton Test""}"));
            Assert.NotEmpty(serviceDocument.Singletons);
            var singleton = Assert.Single(serviceDocument.Singletons);
            Assert.Equal("singleton", singleton.Name);
            Assert.Equal("http://odata.org/singleton", singleton.Url.ToString());
        }

        [Fact]
        public void ReadServiceDocumentWithMultiSingletonInfo()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""singleton"",""url"":""singleton"", ""kind"":""Singleton""}, {""name"":""singleton1"",""url"":""http://service/singleton1"", ""kind"":""Singleton""}"));
            Assert.NotEmpty(serviceDocument.Singletons);
            var singletons = serviceDocument.Singletons.ToList();
            Assert.Equal(2, singletons.Count);
            Assert.Equal("singleton", singletons[0].Name);
            Assert.Equal("http://odata.org/singleton", singletons[0].Url.ToString());
            Assert.Equal("singleton1", singletons[1].Name);
            Assert.Equal("http://service/singleton1", singletons[1].Url.ToString());
        }

        [Fact]
        public void ReadServiceDocumentWithSingletonInfoWithNameAbsent() 
        {
            Action test = () => ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""url"":""singleton"", ""kind"":""Singleton""}"));
            test.Throws<ODataException>(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementName));
        }

        [Fact]
        public void ReadServiceDocumentWithSingletonInfoWithKindAbsent() 
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""url"":""singleton"", ""name"":""singleton""}"));
            Assert.Empty(serviceDocument.Singletons);
            Assert.NotEmpty(serviceDocument.EntitySets);
        }

        [Fact]
        public void ReaderServiceDocumentWithSingletonInfoWithEmptyUrl() 
        {
            Action test = () => ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""url"":"""", ""name"":""singleton""}"));
            test.Throws<ODataException>(Strings.ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(JsonLightConstants.ODataServiceDocumentElementUrlName));
        }
        #endregion

        [Fact]
        public void ReadServiceDocumentWithEntitySetInfo()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""entityset"",""url"":""http://service/entityset"", ""kind"":""EntitySet""}"));
            this.TestEntitySetInServiceDocument(serviceDocument);
        }

        [Fact]
        public void ReadServiceDocumentWithEntitySetInfoWithNoKindSpecified()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""entityset"",""url"":""http://service/entityset""}"));
            this.TestEntitySetInServiceDocument(serviceDocument);
        }

        [Fact]
        public void ReadServiceDocumentShouldIgnoreUnknownKind()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE", @"{""name"":""entityset"",""url"":""http://service/entityset"", ""kind"":""somethingelse""}"));
            Assert.Empty(serviceDocument.EntitySets);
        }

        [Fact]
        public void ReadServiceDocumentWithTitle()
        {
            var serviceDocument = ReadServiceDocument(DefaultEmptyServiceDocumentStarter.Replace("REPLACE",
@"{""name"":""entityset"",""url"":""http://service/entityset"", ""kind"":""EntitySet"", ""title"":""some entity set"" },
  {""name"":""singleton"",""url"":""http://service/singleton"", ""kind"":""Singleton"", ""title"":""some singleton"" },
  {""name"":""singleton2"",""url"":""http://service/singleton2"", ""kind"":""Singleton"" },
  {""name"":""functionImport"",""url"":""http://service/functionimport"", ""kind"":""FunctionImport"", ""title"":""some function import""}"));

            Assert.NotNull(serviceDocument.EntitySets);
            var entitySet = Assert.Single(serviceDocument.EntitySets);
            Assert.Equal("entityset", entitySet.Name);
            Assert.Equal("http://service/entityset", entitySet.Url.ToString());
            Assert.Equal("some entity set", entitySet.Title);

            Assert.NotNull(serviceDocument.Singletons);
            var singleton = serviceDocument.Singletons.ToList();
            Assert.Equal(2, singleton.Count);
            Assert.Equal("singleton", singleton[0].Name);
            Assert.Equal("http://service/singleton", singleton[0].Url.ToString());
            Assert.Equal("some singleton", singleton[0].Title);
            Assert.Equal("singleton2", singleton[1].Name);
            Assert.Equal("http://service/singleton2", singleton[1].Url.ToString());
            Assert.Null(singleton[1].Title);

            Assert.NotNull(serviceDocument.FunctionImports);
            var functionImport = Assert.Single(serviceDocument.FunctionImports);
            Assert.Equal("functionImport", functionImport.Name);
            Assert.Equal("http://service/functionimport", functionImport.Url.ToString());
            Assert.Equal("some function import", functionImport.Title);
        }

        private ODataServiceDocument ReadServiceDocument(string payload)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            ODataJsonLightServiceDocumentDeserializer deserializer = CreateODataJsonServiceDocumentDeserializer(stream);
            return deserializer.ReadServiceDocument();
        }

        private ODataJsonLightServiceDocumentDeserializer CreateODataJsonServiceDocumentDeserializer(MemoryStream stream, IODataPayloadUriConverter urlResolver = null)
        {
            var messageInfo = new ODataMessageInfo
            {
                Encoding = Encoding.UTF8,
                IsResponse = true,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = false,
                Model = new EdmModel(),
                PayloadUriConverter = urlResolver,
                MessageStream = stream
            };

            var inputContext = new ODataJsonLightInputContext(messageInfo, new ODataMessageReaderSettings());
            return new ODataJsonLightServiceDocumentDeserializer(inputContext);
        }

        private void TestEntitySetInServiceDocument(ODataServiceDocument serviceDocument)
        {
            Assert.NotNull(serviceDocument.EntitySets);
            var entitySet = Assert.Single(serviceDocument.EntitySets);
            Assert.Equal("entityset", entitySet.Name);
            Assert.Equal("http://service/entityset", entitySet.Url.ToString());
        }
    }
}
