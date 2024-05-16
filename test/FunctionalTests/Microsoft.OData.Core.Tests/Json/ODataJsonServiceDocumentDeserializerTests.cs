//---------------------------------------------------------------------
// <copyright file="ODataJsonServiceDocumentDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonServiceDocumentDeserializerTests
    {
        private const string DefaultEmptyServiceDocumentStarter = @"{
  ""@odata.context"":""http://odata.org/$metadata"",""value"":[
    REPLACE
  ]
}";
        private readonly ODataMessageReaderSettings messageReaderSettings;

        public ODataJsonServiceDocumentDeserializerTests()
        {
            this.messageReaderSettings = new ODataMessageReaderSettings();
        }

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
            test.Throws<ODataException>(Strings.ODataJsonServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(ODataJsonConstants.ODataServiceDocumentElementName));
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
            test.Throws<ODataException>(Strings.ODataJsonServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(ODataJsonConstants.ODataServiceDocumentElementUrlName));
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

        [Theory]
        [InlineData("{\"name\":\"Customers\",\"name\":\"Customers\"}", "name")]
        [InlineData("{\"url\":\"http://tempuri.org/Customers\",\"url\":\"http://tempuri.org/Customers\"}", "url")]
        [InlineData("{\"kind\":\"EntitySet\",\"kind\":\"EntitySet\"}", "kind")]
        [InlineData("{\"title\":\"Customers\",\"title\":\"Customers\"}", "title")]
        public void ReadServiceDocument_ThrowsExceptionForRepeatedPropertyInServiceDocumentElement(string fragment, string property)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                $"\"value\":[{fragment}]}}";

            var exception = Assert.Throws<ODataException>(() => ReadServiceDocument(payload));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(property),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[" +
                "{\"name\":\"Customers\",\"url\":\"http://tempuri.org/Customers\",\"kind\":\"EntitySet\",\"title\":\"Customers\"}," +
                "{\"name\":\"Company\",\"url\":\"http://tempuri.org/Company\",\"kind\":\"Singleton\",\"title\":\"Company\"}," +
                "{\"name\":\"Top5Customers\",\"url\":\"http://tempuri.org/Top5Customers\",\"kind\":\"FunctionImport\",\"title\":\"Top5Customers\"}]}";

            await SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                payload,
                async (jsonServiceDocumentDeserializer) =>
                {
                    var serviceDocument = await jsonServiceDocumentDeserializer.ReadServiceDocumentAsync();

                    var customersEntitySet = Assert.Single(serviceDocument.EntitySets);
                    Assert.Equal("Customers", customersEntitySet.Name);
                    Assert.Equal("http://tempuri.org/Customers", customersEntitySet.Url.AbsoluteUri);
                    Assert.Equal("Customers", customersEntitySet.Title);

                    var companySingleton = Assert.Single(serviceDocument.Singletons);
                    Assert.Equal("Company", companySingleton.Name);
                    Assert.Equal("http://tempuri.org/Company", companySingleton.Url.AbsoluteUri);
                    Assert.Equal("Company", companySingleton.Title);

                    var top5CustomersFunctionImport = Assert.Single(serviceDocument.FunctionImports);
                    Assert.Equal("Top5Customers", top5CustomersFunctionImport.Name);
                    Assert.Equal("http://tempuri.org/Top5Customers", top5CustomersFunctionImport.Url.AbsoluteUri);
                    Assert.Equal("Top5Customers", top5CustomersFunctionImport.Title);
                });
        }

        [Fact]
        public async Task ReadServiceDocumentEmptyOfServiceDocumentElementsAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[]}";

            await SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                payload,
                async (jsonServiceDocumentDeserializer) =>
                {
                    var serviceDocument = await jsonServiceDocumentDeserializer.ReadServiceDocumentAsync();

                    Assert.Empty(serviceDocument.EntitySets);
                    Assert.Empty(serviceDocument.Singletons);
                    Assert.Empty(serviceDocument.FunctionImports);
                });
        }

        [Fact]
        public async Task ReadServiceDocumentWithMultipleServiceDocumentElementsOfSameKindAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[" +
                "{\"name\":\"RankCustomers\",\"url\":\"http://tempuri.org/RankCustomers\",\"kind\":\"FunctionImport\",\"title\":\"RankCustomers\"}," +
                "{\"name\":\"Top5Customers\",\"url\":\"http://tempuri.org/Top5Customers\",\"kind\":\"FunctionImport\",\"title\":\"Top5Customers\"}]}";

            await SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                payload,
                async (jsonServiceDocumentDeserializer) =>
                {
                    var serviceDocument = await jsonServiceDocumentDeserializer.ReadServiceDocumentAsync();

                    Assert.Empty(serviceDocument.EntitySets);
                    Assert.Empty(serviceDocument.Singletons);

                    Assert.Equal(2, serviceDocument.FunctionImports.Count());
                    var rankCustomersFunctionImport = serviceDocument.FunctionImports.First();
                    var top5CustomersFunctionImport = serviceDocument.FunctionImports.Last();
                    Assert.Equal("RankCustomers", rankCustomersFunctionImport.Name);
                    Assert.Equal("http://tempuri.org/RankCustomers", rankCustomersFunctionImport.Url.AbsoluteUri);
                    Assert.Equal("RankCustomers", rankCustomersFunctionImport.Title);
                    Assert.Equal("Top5Customers", top5CustomersFunctionImport.Name);
                    Assert.Equal("http://tempuri.org/Top5Customers", top5CustomersFunctionImport.Url.AbsoluteUri);
                    Assert.Equal("Top5Customers", top5CustomersFunctionImport.Title);
                });
        }

        [Fact]
        public async Task ReadServiceDocumentWithoutTitleInServiceDocumentElementAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"name\":\"Company\",\"url\":\"http://tempuri.org/Company\",\"kind\":\"Singleton\"}]}";

            await SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                payload,
                async (jsonServiceDocumentDeserializer) =>
                {
                    var serviceDocument = await jsonServiceDocumentDeserializer.ReadServiceDocumentAsync();

                    Assert.Empty(serviceDocument.EntitySets);
                    Assert.Empty(serviceDocument.FunctionImports);

                    var companySingleton = Assert.Single(serviceDocument.Singletons);
                    Assert.Equal("Company", companySingleton.Name);
                    Assert.Equal("http://tempuri.org/Company", companySingleton.Url.AbsoluteUri);
                    Assert.Null(companySingleton.Title);
                });
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ServiceDocumentElementDefaultsToEntitySetForTitleNotSpecified()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"name\":\"Customers\",\"url\":\"http://tempuri.org/Customers\",\"title\":\"Customers\"}]}";

            await SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                payload,
                async (jsonServiceDocumentDeserializer) =>
                {
                    var serviceDocument = await jsonServiceDocumentDeserializer.ReadServiceDocumentAsync();

                    var customersEntitySet = Assert.Single(serviceDocument.EntitySets);
                    Assert.Equal("Customers", customersEntitySet.Name);
                    Assert.Equal("http://tempuri.org/Customers", customersEntitySet.Url.AbsoluteUri);
                    Assert.Equal("Customers", customersEntitySet.Title);

                    Assert.Empty(serviceDocument.Singletons);
                    Assert.Empty(serviceDocument.FunctionImports);
                });
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedODataAnnotationInServiceDocument()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"@odata.type\":\"#Collection(NS.ServiceDocumentElement)\"," +
                "\"value\":[]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_InstanceAnnotationInServiceDocument("odata.type", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedODataPropertyAnnotationInServiceDocument()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value@odata.type\":\"#Collection(NS.ServiceDocumentElement)\"," +
                "\"value\":[]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationInServiceDocument("odata.type", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedMetadataReferencePropertyInServiceDocument()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"#NS.Top5Customers\":{\"title\":\"Top5Customers\",\"target\":\"http://tempuri.org/Top5Customers\"}," +
                "\"value\":[]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.Top5Customers"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedPropertyAnnotationWithoutPropertyInServiceDocument()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value@custom.annotation\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationWithoutProperty("value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForMissingODataValuePropertyInServiceDocument()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_MissingValuePropertyInServiceDocument("value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForRepeatedODataValuePropertyInServiceDocument()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"name\":\"Customers\",\"url\":\"http://tempuri.org/Customers\",\"kind\":\"EntitySet\",\"title\":\"Customers\"}]," +
                "\"value\":[{\"name\":\"Company\",\"url\":\"http://tempuri.org/Company\",\"kind\":\"Singleton\",\"title\":\"Company\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument("value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedPropertyInServiceDocument()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"unexpectedProp\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument("unexpectedProp", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedODataInstanceAnnotationInServiceDocumentElement()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"@odata.type\":\"#NS.ServiceDocumentElement\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement("odata.type"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedODataPropertyAnnotationInServiceDocumentElement()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"name@odata.type\":\"#Edm.String\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement("odata.type"),
                exception.Message);
        }

        [Theory]
        [InlineData("{\"name\":\"Customers\",\"name\":\"Customers\"}", "name")]
        [InlineData("{\"url\":\"http://tempuri.org/Customers\",\"url\":\"http://tempuri.org/Customers\"}", "url")]
        [InlineData("{\"kind\":\"EntitySet\",\"kind\":\"EntitySet\"}", "kind")]
        [InlineData("{\"title\":\"Customers\",\"title\":\"Customers\"}", "title")]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForRepeatedPropertyInServiceDocumentElement(string fragment, string property)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                $"\"value\":[{fragment}]}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(property),
                exception.Message);
        }

        [Theory]
        [InlineData("{\"url\":\"http://tempuri.org/Customers\",\"kind\":\"EntitySet\",\"title\":\"Customers\"}", "name")]
        [InlineData("{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"title\":\"Customers\"}", "url")]
        [InlineData("{\"name\":null,\"url\":\"http://tempuri.org/Customers\",\"kind\":\"EntitySet\",\"title\":\"Customers\"}", "name")]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForMissingOrNullRequiredPropertyInServiceDocumentElement(string fragment, string property)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                $"\"value\":[{fragment}]}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(property),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForServiceDocumentElementUrlIsNull()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"name\":\"Customers\",\"url\":null,\"kind\":\"EntitySet\",\"title\":\"Customers\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ValidationUtils_ServiceDocumentElementUrlMustNotBeNull,
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedMetadataReferencePropertyInServiceDocumentElement()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"#NS.Top5Customers\":{\"title\":\"Top5Customers\",\"target\":\"http://tempuri.org/Top5Customers\"}}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.Top5Customers"),
                exception.Message);
        }

        [Fact]        
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedPropertyAnnotationWithoutPropertyInServiceDocumentElement()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"name@custom.annotation\":\"foobar\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_PropertyAnnotationWithoutProperty("name"),
                exception.Message);
        }

        [Fact]
        public async Task ReadServiceDocumentAsync_ThrowsExceptionForUnexpectedPropertyInServiceDocumentElement()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[{\"unexpectedProp\":\"foobar\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonServiceDocumentDeserializerAndRunTestAsync(
                    payload,
                    (jsonServiceDocumentDeserializer) => jsonServiceDocumentDeserializer.ReadServiceDocumentAsync()));

            Assert.Equal(
                ErrorStrings.ODataJsonServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement("unexpectedProp", "name", "url"),
                exception.Message);
        }

        private ODataServiceDocument ReadServiceDocument(string payload)
        {
            ODataServiceDocument serviceDocument;

            using (var jsonInputContext = CreateJsonInputContext(payload, isAsync: false))
            {
                var jsonServiceDocumentDeserializer = new ODataJsonServiceDocumentDeserializer(jsonInputContext);

                serviceDocument = jsonServiceDocumentDeserializer.ReadServiceDocument();
            }

            return serviceDocument;
        }

        private ODataJsonInputContext CreateJsonInputContext(string payload, bool isAsync = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                Encoding = Encoding.UTF8,
                IsResponse = true,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = isAsync,
                Model = new EdmModel(),
            };

            return new ODataJsonInputContext(new StringReader(payload), messageInfo, this.messageReaderSettings);
        }

        private void TestEntitySetInServiceDocument(ODataServiceDocument serviceDocument)
        {
            Assert.NotNull(serviceDocument.EntitySets);
            var entitySet = Assert.Single(serviceDocument.EntitySets);
            Assert.Equal("entityset", entitySet.Name);
            Assert.Equal("http://service/entityset", entitySet.Url.ToString());
        }

        private async Task SetupJsonServiceDocumentDeserializerAndRunTestAsync(
            string payload,
            Func<ODataJsonServiceDocumentDeserializer, Task> func)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, isAsync: true))
            {
                var jsonServiceDocumentDeserializer = new ODataJsonServiceDocumentDeserializer(jsonInputContext);

                await func(jsonServiceDocumentDeserializer);
            }
        }
    }
}
