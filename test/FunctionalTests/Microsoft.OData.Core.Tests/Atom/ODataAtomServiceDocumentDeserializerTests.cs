//---------------------------------------------------------------------
// <copyright file="ODataAtomServiceDocumentDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class ODataAtomServiceDocumentDeserializerTests
    {
        private const string DefaultEmptyServiceDocumentStarter = @"﻿<?xml version=""1.0"" encoding=""utf-8""?><service xmlns=""http://www.w3.org/2007/app"" xmlns:atom=""http://www.w3.org/2005/Atom"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata""><workspace><atom:title type=""text"">Default</atom:title>{0}</workspace></service>";
        
        [Fact]
        public void ReadServiceDocumentWithFunctionImportInfoWithoutNameSpecifiedButWithTitle()
        {
            var serviceDocument = ReadServiceDocument(string.Format(DefaultEmptyServiceDocumentStarter, @"<m:function-import href=""http://service/functionimport""><atom:title type=""text"">functionImport</atom:title></m:function-import>"));
            serviceDocument.FunctionImports.Should().NotBeNull();
            var functionImports = serviceDocument.FunctionImports.ToList();
            functionImports.Count.Should().Be(1);
            functionImports[0].Name.Should().Be("functionImport");
            functionImports[0].Url.ToString().Should().Be("http://service/functionimport");
        }

        [Fact]
        public void ReadServiceDocumentElementWithNameWithTitleWithReadAtomMetadataOff()
        {
            var serviceDocument = ReadServiceDocument(string.Format(DefaultEmptyServiceDocumentStarter, @"<m:function-import href=""http://service/functionimport"" m:name=""functionImportName""><atom:title type=""text"">functionImportTitle</atom:title></m:function-import>"));
            serviceDocument.FunctionImports.Should().NotBeNull();
            var functionImports = serviceDocument.FunctionImports.ToList();
            functionImports.Count.Should().Be(1);
            functionImports[0].Name.Should().Be("functionImportName");
            functionImports[0].Url.ToString().Should().Be("http://service/functionimport");
            functionImports[0].GetAnnotation<AtomResourceCollectionMetadata>().Should().BeNull();
        }

        [Fact]
        public void ReadServiceDocumentElementWithNameWithTitleWithReadAtomMetadataOn()
        {
            var serviceDocument = ReadServiceDocument(string.Format(DefaultEmptyServiceDocumentStarter, @"<m:function-import href=""http://service/functionimport"" m:name=""functionImportName""><atom:title type=""text"">functionImportTitle</atom:title></m:function-import>"), true /*enableAtomReading*/);
            serviceDocument.FunctionImports.Should().NotBeNull();
            var functionImports = serviceDocument.FunctionImports.ToList();
            functionImports.Count.Should().Be(1);
            functionImports[0].Name.Should().Be("functionImportName");
            functionImports[0].Url.ToString().Should().Be("http://service/functionimport");
            var annotation = functionImports[0].GetAnnotation<AtomResourceCollectionMetadata>();
            annotation.Should().NotBeNull();
            annotation.Title.Text.Should().Be("functionImportTitle");
        }

        [Fact]
        public void ReadServiceDocumentWithSingletonInfoWithoutNameSpecifiedButWithTitle()
        {
            var serviceDocument = ReadServiceDocument(string.Format(DefaultEmptyServiceDocumentStarter, @"<m:singleton href=""http://service/singleton""><atom:title type=""text"">singleton</atom:title></m:singleton>"));
            serviceDocument.Singletons.Should().NotBeNull();
            var singletons = serviceDocument.Singletons.ToList();
            singletons.Count.Should().Be(1);
            singletons[0].Name.Should().Be("singleton");
            singletons[0].Url.ToString().Should().Be("http://service/singleton");
        }

        [Fact]
        public void ReadServiceDocumentWithEntitySetInfoWithoutNameSpecifiedButWithTitle()
        {
            var serviceDocument = ReadServiceDocument(string.Format(DefaultEmptyServiceDocumentStarter, @"<collection href=""http://service/singleton""><atom:title type=""text"">title</atom:title></collection>"));
            serviceDocument.EntitySets.Should().NotBeNull();
            var entitySets = serviceDocument.EntitySets.ToList();
            entitySets.Count.Should().Be(1);
            entitySets[0].Name.Should().Be("title");
            entitySets[0].Url.ToString().Should().Be("http://service/singleton");
        }

        [Fact]
        public void ReadServiceDocumentWithUnknownODataMetadataElementShouldThrow()
        {
            Action test = () => ReadServiceDocument(string.Format(DefaultEmptyServiceDocumentStarter, @"<m:badElement href=""http://service/singleton""><atom:title type=""text"">badElement</atom:title></m:badElement>"));
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataAtomServiceDocumentDeserializer_UnexpectedODataElementInWorkspace("badElement"));
        }

        private ODataServiceDocument ReadServiceDocument(string payload)
        {
            return ReadServiceDocument(payload, false);
        }

        private ODataServiceDocument ReadServiceDocument(string payload, bool enableAtomMetadataReading)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            ODataAtomServiceDocumentDeserializer deserializer = CreateODataAtomServiceDocumentDeserializer(stream, enableAtomMetadataReading);
            return deserializer.ReadServiceDocument();
        }

        private ODataAtomServiceDocumentDeserializer CreateODataAtomServiceDocumentDeserializer(MemoryStream stream, bool enableAtomMetadataReading, IODataUrlResolver urlResolver = null)
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            settings.EnableAtomMetadataReading = enableAtomMetadataReading;
            ODataAtomInputContext inputContext = new ODataAtomInputContext(ODataFormat.Atom, stream, Encoding.UTF8, settings, true /*readingResponse*/, true /*sync*/, null /*edmModel*/, urlResolver);
            return new ODataAtomServiceDocumentDeserializer(inputContext);
        }
    }
}
