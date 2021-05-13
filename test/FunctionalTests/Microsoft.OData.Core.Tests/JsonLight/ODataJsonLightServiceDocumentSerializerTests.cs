//---------------------------------------------------------------------
// <copyright file="ODataJsonLightServiceDocumentSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightServiceDocumentSerializerTests
    {
        [Fact]
        public void FunctionImportShouldWriteOutNameAndUrlCorrectly()
        {
            WriteServiceDocumentVerifyOutput(
                new ODataServiceDocument()
                {
                    FunctionImports = new ODataFunctionImportInfo[] {new ODataFunctionImportInfo() {Name = "name1", Url = new Uri("http://Service/FunctionImport")}}
                },
                @"﻿{""value"":[{""name"":""name1"",""kind"":""FunctionImport"",""url"":""http://service/FunctionImport""}]}");
        }

        [Fact]
        public void FunctionImportWithSameNameShouldBeWrittenOnceInJson()
        {
            WriteServiceDocumentVerifyOutput(
                new ODataServiceDocument()
                {
                    FunctionImports = new ODataFunctionImportInfo[]
                    {
                        new ODataFunctionImportInfo() { Name = "name1", Url = new Uri("http://Service/FunctionImport") },
                        new ODataFunctionImportInfo() { Name = "name1", Url = new Uri("http://Service/FunctionImport") }
                    }
                },
                @"﻿{""value"":[{""name"":""name1"",""kind"":""FunctionImport"",""url"":""http://service/FunctionImport""}]}");
        }

        [Fact]
        public void WriteSingleton()
        {
            WriteServiceDocumentVerifyOutput(
                new ODataServiceDocument()
                {
                    Singletons = new[] { new ODataSingletonInfo { Name = "singleton", Url = new Uri("http://Service/Singleton"), Title = "Some Singleton" } }
                },
                @"﻿{""value"":[{""name"":""singleton"",""title"":""Some Singleton"",""kind"":""Singleton"",""url"":""http://service/Singleton""}]}");
        }

        [Fact]
        public void WriteEntitySet()
        {
            WriteServiceDocumentVerifyOutput(
                new ODataServiceDocument()
                {
                    EntitySets = new[] { new ODataEntitySetInfo { Name = "entityset", Url = new Uri("http://Service/EntitySet"), Title = "Some EntitySet" } }
                },
                @"﻿{""value"":[{""name"":""entityset"",""title"":""Some EntitySet"",""kind"":""EntitySet"",""url"":""http://service/EntitySet""}]}");
        }

        [Fact]
        public void WriteSingletonWithSameTitleAsName()
        {
            WriteServiceDocumentVerifyOutput(
                new ODataServiceDocument()
                {
                    Singletons = new[] { new ODataSingletonInfo { Name = "someSingleton", Url = new Uri("http://Service/Singleton"), Title = "someSingleton" } }
                },
                @"﻿{""value"":[{""name"":""someSingleton"",""kind"":""Singleton"",""url"":""http://service/Singleton""}]}");
        }

        [Fact]
        public void WriteEntitySetWithNullTitle()
        {
            WriteServiceDocumentVerifyOutput(
                new ODataServiceDocument()
                {
                    EntitySets = new[] { new ODataEntitySetInfo { Name = "entityset", Url = new Uri("http://Service/EntitySet"), Title = null } }
                },
                @"﻿{""value"":[{""name"":""entityset"",""kind"":""EntitySet"",""url"":""http://service/EntitySet""}]}");
        }

        [Fact]
        public void WriteEntitySetWithEmptyTitle()
        {
            WriteServiceDocumentVerifyOutput(
                new ODataServiceDocument()
                {
                    EntitySets = new[] { new ODataEntitySetInfo { Name = "entityset", Url = new Uri("http://Service/EntitySet"), Title = "" } }
                },
                @"﻿{""value"":[{""name"":""entityset"",""kind"":""EntitySet"",""url"":""http://service/EntitySet""}]}");
        }

        [Fact]
        public void WriteNullFunctionImportShouldThrow()
        {
            var functionImports = new List<ODataFunctionImportInfo>();
            functionImports.Add(null);
            var serviceDocument = new ODataServiceDocument()
            {
                FunctionImports = functionImports
            };

            //Note: Only testing one of three exceptions that occurs in the Validate ServiceDocumentElements because if one throws then we know that the code path is going through
            // the validation method. If the validation method is changed then in the future two exception tests might be needed.
            WriteServiceDocumentShouldError(serviceDocument).Throws<ODataException>(Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
        }

        public static IEnumerable<object[]> GetWriteServiceDocumentTestData()
        {
            yield return new object[]
            {
                new ODataServiceDocument {},
                "{\"value\":[]}"
            };

            var entitySets = new List<ODataEntitySetInfo>
            {
                new ODataEntitySetInfo { Name = "Customers", Title = "Customers", Url = new Uri("http://tempuri.org/Customers") },
                new ODataEntitySetInfo { Name = "Orders", Title = "Orders", Url = new Uri("http://tempuri.org/Orders") }
            };

            // EntitySet

            yield return new object[]
            {
                new ODataServiceDocument
                {
                    EntitySets = entitySets
                },
                "{\"value\":[" +
                "{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/Customers\"}," +
                "{\"name\":\"Orders\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/Orders\"}" +
                "]}"
            };

            var singletons = new List<ODataSingletonInfo>
            {
                new ODataSingletonInfo { Name = "Company", Title = "BusinessEntity", Url = new Uri("http://tempuri.org/Company") }
            };

            // Singleton (Title different from Name)

            yield return new object[]
            {
                new ODataServiceDocument
                {
                    Singletons = singletons
                },
                "{\"value\":[" +
                "{\"name\":\"Company\",\"title\":\"BusinessEntity\",\"kind\":\"Singleton\",\"url\":\"http://tempuri.org/Company\"}" +
                "]}"
            };

            var functionImports = new List<ODataFunctionImportInfo>
            {
                new ODataFunctionImportInfo { Name = "GetOpenOrders", Url = new Uri("http://tempuri.org/GetOpenOrders") },
                new ODataFunctionImportInfo { Name = "GetTop5Customers", Url = new Uri("http://tempuri.org/GetTop5Customers") }
            };

            // FunctionImport

            yield return new object[]
            {
                new ODataServiceDocument
                {
                    FunctionImports = functionImports
                },
                "{\"value\":[" +
                "{\"name\":\"GetOpenOrders\",\"kind\":\"FunctionImport\",\"url\":\"http://tempuri.org/GetOpenOrders\"}," +
                "{\"name\":\"GetTop5Customers\",\"kind\":\"FunctionImport\",\"url\":\"http://tempuri.org/GetTop5Customers\"}" +
                "]}"
            };

            // FunctionImport (Collection containing duplicates)

            var duplicatedFunctionImports = new List<ODataFunctionImportInfo>(functionImports);
            duplicatedFunctionImports[1] = duplicatedFunctionImports[0];

            yield return new object[]
            {
                new ODataServiceDocument
                {
                    FunctionImports = duplicatedFunctionImports
                },
                "{\"value\":[" +
                "{\"name\":\"GetOpenOrders\",\"kind\":\"FunctionImport\",\"url\":\"http://tempuri.org/GetOpenOrders\"}" +
                "]}"
            };


            // Multiple element types

            yield return new object[]
            {
                new ODataServiceDocument
                {
                    EntitySets = entitySets,
                    Singletons = singletons,
                    FunctionImports = functionImports
                },
                "{\"value\":[" +
                "{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/Customers\"}," +
                "{\"name\":\"Orders\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/Orders\"}," +
                "{\"name\":\"Company\",\"title\":\"BusinessEntity\",\"kind\":\"Singleton\",\"url\":\"http://tempuri.org/Company\"}," +
                "{\"name\":\"GetOpenOrders\",\"kind\":\"FunctionImport\",\"url\":\"http://tempuri.org/GetOpenOrders\"}," +
                "{\"name\":\"GetTop5Customers\",\"kind\":\"FunctionImport\",\"url\":\"http://tempuri.org/GetTop5Customers\"}" +
                "]}"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteServiceDocumentTestData))]
        public async Task WriteServiceDocumentAsync_WritesExpectedOutput(ODataServiceDocument serviceDocument, string expected)
        {
            var result = await SetupJsonLightServiceDocumentSerializerAndRunTestAsync(
                (jsonLightServiceDocumentSerializer) =>
                {
                    return jsonLightServiceDocumentSerializer.WriteServiceDocumentAsync(serviceDocument);
                });

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetWriteServiceDocumentExceptionsTestData()
        {
            // Null FunctionImport
            yield return new object[]
            {
                new ODataServiceDocument
                {
                    FunctionImports = new List<ODataFunctionImportInfo> { null }
                },
                Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem
            };

            // Null EntitySet (Singletons handled the same)
            yield return new object[]
            {
                new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo> { null }
                },
                Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem
            };

            // EntitySet with null value as Url (Singletons handled the same)
            yield return new object[]
            {
                new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo>
                    {
                        new ODataEntitySetInfo { Name = "Customers", Url = null }
                    }
                },
                Strings.ValidationUtils_ResourceMustSpecifyUrl
            };

            // EntitySet with null value as Name (Singletons handled the same) 
            yield return new object[]
            {
                new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo>
                    {
                        new ODataEntitySetInfo { Name = null, Url = new Uri("http://tempuri.org/Customers") }
                    }
                },
                Strings.ValidationUtils_ResourceMustSpecifyName("http://tempuri.org/Customers")
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteServiceDocumentExceptionsTestData))]
        public async Task WriteServiceDocumentAsync_ThrowsException(ODataServiceDocument serviceDocument, string exceptionMessage)
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightServiceDocumentSerializerAndRunTestAsync(
                    (jsonLightServiceDocumentSerializer) =>
                    {
                        return jsonLightServiceDocumentSerializer.WriteServiceDocumentAsync(serviceDocument);
                    }));

            Assert.Equal(exceptionMessage, exception.Message);
        }

        private static ODataJsonLightServiceDocumentSerializer CreateODataJsonLightServiceDocumentSerializer(MemoryStream memoryStream, IODataPayloadUriConverter urlResolver = null, bool isAsync = false)
        {
            var model = new EdmModel();
            var messageWriterSettings = new ODataMessageWriterSettings();
            var mainModel = TestUtils.WrapReferencedModelsToMainModel(model);
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = memoryStream,
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = false,
                IsAsync = isAsync,
                Model = mainModel,
                PayloadUriConverter = urlResolver
            };
            var jsonLightOutputContext = new ODataJsonLightOutputContext(messageInfo, messageWriterSettings);
            return new ODataJsonLightServiceDocumentSerializer(jsonLightOutputContext);
        }

        private static Action WriteServiceDocumentShouldError(ODataServiceDocument serviceDocument, IODataPayloadUriConverter urlResolver = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            var serializer = CreateODataJsonLightServiceDocumentSerializer(memoryStream, urlResolver);

            return () => serializer.WriteServiceDocument(serviceDocument);
        }

        private static void WriteServiceDocumentVerifyOutput(ODataServiceDocument serviceDocument, string expectedOutput)
        {
            MemoryStream memoryStream = new MemoryStream();
            var serializer = CreateODataJsonLightServiceDocumentSerializer(memoryStream);

            serializer.WriteServiceDocument(serviceDocument);
            serializer.JsonWriter.Flush();
            string actualResult = Encoding.UTF8.GetString(memoryStream.ToArray());
            Assert.Equal(expectedOutput, actualResult);
        }

        private async Task<string> SetupJsonLightServiceDocumentSerializerAndRunTestAsync(Func<ODataJsonLightServiceDocumentSerializer, Task> func)
        {
            MemoryStream memoryStream = new MemoryStream();
            var jsonLightServiceDocumentSerializer = CreateODataJsonLightServiceDocumentSerializer(memoryStream, /* urlResolver */ null, true);
            await func(jsonLightServiceDocumentSerializer);
            await jsonLightServiceDocumentSerializer.JsonLightOutputContext.FlushAsync();
            await jsonLightServiceDocumentSerializer.AsynchronousJsonWriter.FlushAsync();

            memoryStream.Position = 0;
            return await new StreamReader(memoryStream).ReadToEndAsync();
        }
    }
}
