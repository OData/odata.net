//---------------------------------------------------------------------
// <copyright file="ODataAtomServiceDocumentSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class ODataAtomServiceDocumentSerializerTests
    {
        #region Test Writing FunctionImports
        [Fact]
        public void WriteFunctionImportShouldBeWrittenCorrectly()
        {
            var serviceDocument = new ODataServiceDocument()
            {
                FunctionImports = new ODataFunctionImportInfo[] {new ODataFunctionImportInfo() { Name = "functionImport", Url = new Uri("http://service/functionimport") }}
            };

            // Verifying the Href and name are written in the function Import element
            WriteServiceDocumentVerifyOutput(
                serviceDocument,
@"﻿<?xml version=""1.0"" encoding=""utf-8""?><service xmlns=""http://www.w3.org/2007/app"" xmlns:atom=""http://www.w3.org/2005/Atom"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata""><workspace><atom:title type=""text"">Default</atom:title><m:function-import href=""http://service/functionimport""><atom:title type=""text"">functionImport</atom:title></m:function-import></workspace></service>");
        }

        [Fact]
        public void FunctionImportWithSameNameShouldBeWrittenOnceInAtom()
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
"﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><service xmlns=\"http://www.w3.org/2007/app\" xmlns:atom=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\"><workspace><atom:title type=\"text\">Default</atom:title><m:function-import href=\"http://service/FunctionImport\"><atom:title type=\"text\">name1</atom:title></m:function-import></workspace></service>");
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
            WriteServiceDocumentShouldError(serviceDocument).ShouldThrow<ODataException>().WithMessage(ODataStrings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
        }

        [Fact]
        public void EnsureNonNullODataUrlResolverExecutedWhenWritingFunctionImportInServiceDocument()
        {
            var serviceDocument = new ODataServiceDocument()
            {
                FunctionImports = new ODataFunctionImportInfo[] { new ODataFunctionImportInfo() { Name = "functionImport", Url = new Uri("http://service/functionimport") } }
            };

            bool invoked = false;
            var resolver = new ServiceDocumentTestUrlResolver();
            resolver.ResolveUrlFunc = (uri, uri1) => 
            { 
                invoked = true;
                return uri;
            };

            WriteServiceDocumentVerifyOutput(serviceDocument, null, resolver);
            invoked.Should().BeTrue();
        }
        
        #endregion

        private static Action WriteServiceDocumentShouldError(ODataServiceDocument serviceDocument, IODataUrlResolver resolver = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            ODataAtomServiceDocumentSerializer serializer = CreateAtomOutputContext(memoryStream, resolver);
            return new Action(() => serializer.WriteServiceDocument(serviceDocument));
        }

        private static void WriteServiceDocumentVerifyOutput(ODataServiceDocument serviceDocument, string expectedoutput = null, IODataUrlResolver resolver = null)
        {
            MemoryStream memoryStream = new MemoryStream();
            ODataAtomServiceDocumentSerializer serializer = CreateAtomOutputContext(memoryStream, resolver);

            serializer.WriteServiceDocument(serviceDocument);
            serializer.XmlWriter.Flush();
            var resultsWritten = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            if (expectedoutput != null)
            {
                resultsWritten.Should().Be(expectedoutput);
            }
        }

        private static ODataAtomServiceDocumentSerializer CreateAtomOutputContext(MemoryStream memoryStream, IODataUrlResolver resolver = null)
        {
            var model = new EdmModel();
            var odataMessageWriterSettings = new ODataMessageWriterSettings();
            var atomOutputContext = new ODataAtomOutputContext(ODataFormat.Atom, memoryStream, Encoding.UTF8, odataMessageWriterSettings, false /*writingResponse*/, true /*sync*/, model, resolver);
            return new ODataAtomServiceDocumentSerializer(atomOutputContext);
        }

        internal class ServiceDocumentTestUrlResolver : IODataUrlResolver
        {
            public Func<Uri, Uri, Uri> ResolveUrlFunc { get; set; }
            public Uri ResolveUrl(Uri baseUri, Uri payloadUri)
            {
                return this.ResolveUrlFunc(baseUri, payloadUri);
            }
        }
    }
}
