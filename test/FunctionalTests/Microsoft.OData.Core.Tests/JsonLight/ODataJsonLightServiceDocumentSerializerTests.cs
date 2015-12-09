//---------------------------------------------------------------------
// <copyright file="ODataJsonLightServiceDocumentSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
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
            WriteServiceDocumentShouldError(serviceDocument).ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
        }

        private static ODataJsonLightServiceDocumentSerializer CreateODataJsonLightServiceDocumentSerializer(MemoryStream memoryStream, IODataUrlResolver urlResolver = null)
        {
            var model = new EdmModel();
            var messageWriterSettings = new ODataMessageWriterSettings();
            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel(model);
            var jsonLightOutputContext = new ODataJsonLightOutputContext(ODataFormat.Json, memoryStream, new ODataMediaType("application", "json") /*mediaType*/, Encoding.UTF8, messageWriterSettings, false /*WritingResponse*/, true /*sync*/, mainModel, urlResolver);
            return new ODataJsonLightServiceDocumentSerializer(jsonLightOutputContext);
        }

        private static Action WriteServiceDocumentShouldError(ODataServiceDocument serviceDocument, IODataUrlResolver urlResolver = null)
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
            actualResult.Should().Be(expectedOutput);
        }
    }
}
