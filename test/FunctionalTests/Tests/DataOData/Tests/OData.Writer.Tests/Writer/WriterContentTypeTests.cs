//---------------------------------------------------------------------
// <copyright file="WriterContentTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// This class is disabled on Silverlight and Phone because its method calls CreateContentTypeResultCallback which use ODataMessageWriterSettingsTestWrapper which uses private reflection
#if !SILVERLIGHT && !WINDOWS_PHONE
namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using ApprovalTests;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Tests.WriterTests.BatchWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.CollectionWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing different OData payloads with a variety of accept headers.
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    [TestClass, TestCase]
    public class WriterContentTypeTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public CollectionWriterTestDescriptor.Settings CollectionSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public BatchWriterTestDescriptor.Settings BatchSettings { get; set; }

        private const string Default = "";
        private const string StarStar = "*/*";
        private const string TextStar = "text/*";
        private const string TextXml = "text/xml";
        private const string TextPlain = "text/plain";
        private const string ApplicationStar = "application/*";
        private const string ApplicationXml = "application/xml";
        private const string ApplicationAtomXml = "application/atom+xml";
        private const string ApplicationAtomSvcXml = "application/atomsvc+xml";
        private const string ApplicationAtomXmlEntry = "application/atom+xml;type=entry";
        private const string ApplicationAtomXmlFeed = "application/atom+xml;type=feed";
        private const string ApplicationJson = "application/json";
        private const string ApplicationJsonODataLight = "application/json;odata.metadata=minimal";
        private const string ApplicationJsonODataLightStreaming = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false";
        private const string ApplicationJsonODataLightStreamingAndDefaultMetadata = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false";
        private const string ApplicationJsonODataLightNonStreaming = "application/json;odata.metadata=minimal;odata.streaming=false;IEEE754Compatible=false";
        private const string ApplicationJsonODataLightNonStreamingAndDefaultMetadata = "application/json;odata.metadata=minimal;odata.streaming=false;IEEE754Compatible=false";
        private const string ApplicationJsonAndAtomXml = "application/json,application/atom+xml";
        private const string ApplicationJsonAndJsonLightStreaming = "application/json,application/json;odata.metadata=minimal;odata.streaming=true";
        private const string ApplicationJsonAndJsonStreaming = "application/json,application/json;odata.streaming=true";
        private const string ApplicationJsonStreamingAndJsonNonStreaming = "application/json;odata.streaming=true,application/json;odata.streaming=false";
        private const string ApplicationJsonLightNonStreamingAndJsonStreaming = "application/json;odata.metadata=minimal;odata.streaming=false,application/json;odata.streaming=true";
        private const string ApplicationJsonLightNonStreamingAndJsonLightStreaming = "application/json;odata.metadata=minimal;odata.streaming=false,application/json;odata.metadata=minimal;odata.streaming=true";
        private const string ApplicationJsonAndAtomXmlWithQuality = "application/json,application/atom+xml;q=0.8";
        private const string ApplicationJsonWithQualityAndAtomXml = "application/json;q=0.8,application/atom+xml";
        private const string ApplicationOctetStream = "application/octet-stream";
        private const string MultipartStar = "multipart/*";
        private const string MultipartMixed = "multipart/mixed";
        private const string MultipartHttp = "multipart/http";
        private const string AcceptHeaderWithQualityValues1 = "application/json;odata.metadata=verbose; q=0.2, application/atom+xml; q=0.5";
        private const string AcceptHeaderWithQualityValues2 = "text/plain;q=0.2, application/json;odata.metadata=verbose;q=0.5, application/atom+xml";
        private const string AcceptHeaderWithQualityValues3 = "application/json;odata.metadata=verbose,application/atom+xml;q=0.8";
        private const string AcceptHeaderWithUnsupportedParameter = "application/atom+xml;a=b";
        private const string AcceptHeaderWithMultipleSupportedEntryTypes = "application/atom+xml, application/atom+xml;type=entry";
        private const string AcceptHeaderWithMultipleSupportedFeedTypes = "application/atom+xml, application/atom+xml;type=feed";
        private const string AcceptHeaderWithMultipleAcceptParameters = "application/json;odata.metadata=verbose;q=0.2;a=b;c=d";
        private const string AcceptHeaderWithInvalidValue = "application/atom+xml;type=foo";
        private const string AcceptHeaderWithInvalidValueEntry = "application/atom+xml;type=EnTrY";
        private const string AcceptHeaderWithInvalidValueFeed = "application/atom+xml;type=fEEd";

        private const string Charset = "charset=";
        private const string Utf8 = "utf-8";
        private const string Utf16 = "utf-16";
        private const string Iso88591 = "iso-8859-1";
        private const string CharsetUtf8 = Charset + Utf8;
        private const string CharsetUtf16 = Charset + Utf16;
        private const string CharSetIso88591 = Charset + Iso88591;

        private static readonly Uri baseUri = new Uri("http://www.odata.org");

        // TODO: add (in a separate file?) encoding tests (different encodings, different q-values, etc.)

        // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
        // ToDo: For this file, take a look at history to find out exactly what to translate.

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test response content types when writing entries.")]
        public void EntryContentTypeTests()
        {
            var model = new EdmModel();
            EdmEntityType entity = new EdmEntityType("TestModel", "DefaultEntityType");
            entity.AddKeys(entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(entity);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("DefaultEntitySet", entity);
            model.AddElement(container);

            var entry = CreateDefaultEntryWithAtomMetadata("TestModel.DefaultEntityType");
            var entitySet = container.FindEntitySet("DefaultEntitySet") as EdmEntitySet;
            var entityType = model.FindType("TestModel.DefaultEntityType") as EdmEntityType;

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = null, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata  + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // error cases
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, ApplicationAtomXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, ApplicationAtomXmlEntry, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithQualityValues1, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithQualityValues1, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithQualityValues2, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithQualityValues2, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithMultipleSupportedEntryTypes, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithMultipleSupportedEntryTypes, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithInvalidValueEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithInvalidValueEntry, tc.Version) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, TextStar, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, TextPlain, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, TextXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, ApplicationXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, ApplicationAtomSvcXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, ApplicationAtomXmlFeed, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithUnsupportedParameter, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithUnsupportedParameter, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithInvalidValue, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithInvalidValue, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationJson + ";some=value", ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, ApplicationJson + ";some=value", tc.Version) },
            };

            this.RunContentTypeTest(entry, ODataPayloadKind.Resource, model, entitySet, entityType, testCases);
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test response content types when writing feeds.")]
        public void FeedContentTypeTests()
        {
            var model = new EdmModel();
            EdmEntityType entity = new EdmEntityType("TestModel", "DefaultEntityType");
            entity.AddKeys(entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(entity);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("DefaultEntitySet", entity);
            model.AddElement(container);

            var entry = CreateDefaultEntryWithAtomMetadata("TestModel.DefaultEntityType");
            var entitySet = container.FindEntitySet("DefaultEntitySet") as EdmEntitySet;
            var entityType = model.FindType("TestModel.DefaultEntityType") as EdmEntityType;

            ODataResourceSet resourceCollection = ObjectModelUtils.CreateDefaultFeed();

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = null, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonODataLight, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonODataLightStreaming, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonODataLightNonStreaming, ExpectedContentType = BuildContentType(ApplicationJsonODataLightNonStreamingAndDefaultMetadata, CharsetUtf8) },
                // error cases
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, ApplicationAtomXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, ApplicationAtomXmlFeed, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithQualityValues1, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, AcceptHeaderWithQualityValues1, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithQualityValues2, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, AcceptHeaderWithQualityValues2, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithMultipleSupportedFeedTypes, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, AcceptHeaderWithMultipleSupportedFeedTypes, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithInvalidValueFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, AcceptHeaderWithInvalidValueFeed, tc.Version) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, TextStar, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, TextPlain, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextXml, ExpectedException =tc => GetExpectedException(ODataPayloadKind.ResourceSet, TextXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, ApplicationXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, ApplicationAtomSvcXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, ApplicationAtomXmlEntry, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithUnsupportedParameter, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, AcceptHeaderWithUnsupportedParameter, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = AcceptHeaderWithInvalidValue, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ResourceSet, AcceptHeaderWithInvalidValue, tc.Version) },
            };

            this.RunContentTypeTest(resourceCollection, ODataPayloadKind.ResourceSet, model, entitySet, entityType, testCases);
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test response content types when writing properties.")]
        public void PropertyContentTypeTests()
        {
            ODataProperty property = new ODataProperty() { Name = "Age", Value = 42 };

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = null, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                // error cases
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, ApplicationXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, TextStar, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, TextXml, tc.Version) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, TextPlain, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, ApplicationAtomXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, ApplicationAtomSvcXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, ApplicationAtomXmlEntry, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, ApplicationAtomXmlFeed, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationJson + ";some=value", ExpectedException = tc => GetExpectedException(ODataPayloadKind.Property, ApplicationJson + ";some=value", tc.Version) },
            };

            Func<PayloadWriterTestDescriptor<ODataProperty>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> writerFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteProperty(testDescriptor.PayloadItems[0]);
            this.RunTopLevelContentTypeTest(property, ODataPayloadKind.Property, null, null, writerFunc, testCases);
        }

        [TestMethod, Variation(Description = "Test response content types when writing raw values.")]
        public void RawValueContentTypeTests()
        {
            int value = 42;

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.RawValue, ExpectedContentType = BuildContentType(TextPlain, CharsetUtf8) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedContentType = BuildContentType(TextPlain, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = null, ExpectedContentType = BuildContentType(TextPlain, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = Default, ExpectedContentType = BuildContentType(TextPlain,  CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(TextPlain, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = TextStar, ExpectedContentType = BuildContentType(TextPlain,  CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = TextPlain, ExpectedContentType = BuildContentType(TextPlain, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = BuildContentType(TextPlain, CharsetUtf8), ExpectedContentType = BuildContentType(TextPlain, CharsetUtf8) },
                // error cases
                //new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Atom, ExpectedException = tc => GetExpectedException(ODataFormat.Atom) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, TextXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, ApplicationStar, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, ApplicationXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, ApplicationAtomXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, ApplicationAtomSvcXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, ApplicationAtomXmlEntry, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, ApplicationAtomXmlFeed, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationJson, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, ApplicationJson, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = TextPlain + ";some=value", ExpectedException = tc => GetExpectedException(ODataPayloadKind.Value, TextPlain + ";some=value", tc.Version) },
            };

            this.RunRawValueContentTypeTest(value, ODataPayloadKind.Value, testCases);
        }

        [TestMethod, Variation(Description = "Test response content types when writing raw binary values.")]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void BinaryValueContentTypeTests()
        {
            byte[] binaryValue = new byte[] { 0, 0, 1, 1 };

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.RawValue, ExpectedContentType = ApplicationOctetStream },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedContentType = ApplicationOctetStream },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = null, ExpectedContentType = ApplicationOctetStream },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = Default, ExpectedContentType = ApplicationOctetStream },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = StarStar, ExpectedContentType = ApplicationOctetStream },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationStar, ExpectedContentType = ApplicationOctetStream },
                // NOTE: charset is not considered for binary content type.
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = BuildContentType(ApplicationStar, CharsetUtf8), ExpectedContentType = ApplicationOctetStream },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = BuildContentType(ApplicationStar, "some=value"), ExpectedContentType = ApplicationOctetStream },
                // error cases
                //new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Atom, ExpectedException = tc => GetExpectedException(ODataFormat.Atom) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, TextStar, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, TextXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, TextPlain, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, ApplicationXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, ApplicationAtomXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, ApplicationAtomSvcXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, ApplicationAtomXmlEntry, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, ApplicationAtomXmlFeed, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = ApplicationJson, ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, ApplicationJson, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = BuildContentType(ApplicationJson, "some=value"), ExpectedException = tc => GetExpectedException(ODataPayloadKind.BinaryValue, BuildContentType(ApplicationJson, "some=value"), tc.Version) },
            };

            this.RunRawValueContentTypeTest(binaryValue, ODataPayloadKind.BinaryValue, testCases);
        }

        [TestMethod, Variation(Description = "Test response content types when writing a top-level link.")]
        public void EntityReferenceLinkContentTypeTests()
        {
            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org/") };

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJson },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = null, ExpectedContentType =  ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                // error cases
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, TextStar, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, TextXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, ApplicationXml, tc.Version) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                new ContentTypeTestCase { Format = ODataFormat.RawValue, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, TextPlain, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, ApplicationAtomXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, ApplicationAtomSvcXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, ApplicationAtomXmlEntry, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, ApplicationAtomXmlFeed, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = BuildContentType(ApplicationJson, "some=value"), ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLink, BuildContentType(ApplicationJson, "some=value"), tc.Version) },
            };

            Func<PayloadWriterTestDescriptor<ODataEntityReferenceLink>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> writerFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteEntityReferenceLink(testDescriptor.PayloadItems[0]);
            this.RunTopLevelContentTypeTest(entityReferenceLink, ODataPayloadKind.EntityReferenceLink, null, null, writerFunc, testCases);
        }

        [TestMethod, Variation(Description = "Test response content types when writing a sequence of top-level links.")]
        public void EntityReferenceLinksContentTypeTests()
        {
            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks
            {
                Links = new ODataEntityReferenceLink[]
                {
                    new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org/1") },
                    new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org/2") },
                    new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org/3") },
                },
            };

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJson },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = null, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // error cases
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, ApplicationAtomXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, ApplicationAtomXmlFeed, tc.Version) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, TextPlain, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, ApplicationAtomSvcXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, ApplicationAtomXmlEntry, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = TextStar,ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, TextStar, tc.Version)  },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, TextXml, tc.Version)  },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, ApplicationXml, tc.Version)  },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = BuildContentType(ApplicationJson, "some=value"), ExpectedException = tc => GetExpectedException(ODataPayloadKind.EntityReferenceLinks, BuildContentType(ApplicationJson, "some=value"), tc.Version) },
            };

            Func<PayloadWriterTestDescriptor<ODataEntityReferenceLinks>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> writerFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteEntityReferenceLinks(testDescriptor.PayloadItems[0]);
            this.RunTopLevelContentTypeTest(entityReferenceLinks, ODataPayloadKind.EntityReferenceLinks, null, null, writerFunc, testCases);
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test response content types when writing a collection of primitive values.")]
        public void PrimitiveCollectionContentTypeTests()
        {
            object[] values = new object[] { 41, 42, 43 };

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8  },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = null, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata  + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                // error cases
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, TextStar, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, TextXml, tc.Version) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, TextPlain, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationAtomXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationAtomSvcXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationAtomXmlEntry, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationAtomXmlFeed, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = BuildContentType(ApplicationJson, "some=value"), ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, BuildContentType(ApplicationJson, "some=value"), tc.Version) },
            };

            this.RunCollectionContentTypeTest(values, null, "TestCollection", "Collection(TestCollectionType)", null, testCases);
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test response content types when writing a collection of complex values.")]
        public void ComplexCollectionContentTypeTests()
        {
            ODataComplexValue item1 = new ODataComplexValue()
            {
                TypeName = "TestNS.AddressType",
                Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "FirstName", Value = "Bob" },
                    new ODataProperty { Name = "LastName", Value = "Marley" },
                }
            };
            ODataComplexValue item2 = new ODataComplexValue()
            {
                TypeName = "TestNS.AddressType",
                Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "FirstName", Value = "Jimmy" },
                    new ODataProperty { Name = "LastName", Value = "Hendrix" },
                }
            };

            object[] values = new object[] { item1, item2 };

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = null, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                // error cases
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, TextStar, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, TextXml, tc.Version) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, TextPlain, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationAtomXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationAtomSvcXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationAtomXmlEntry, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, ApplicationAtomXmlFeed, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = BuildContentType(ApplicationJson, "some=value"), ExpectedException = tc => GetExpectedException(ODataPayloadKind.Collection, BuildContentType(ApplicationJson, "some=value"), tc.Version) },
            };

            this.RunCollectionContentTypeTest(values, null, "TestCollection", "Collection(TestCollectionType)", null, testCases);
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test response content types when writing a service document.")]
        public void ServiceDocumentContentTypeTests()
        {
            ODataServiceDocument serviceDocument = ObjectModelUtils.CreateDefaultWorkspace();

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata  + ";" + CharsetUtf8 },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata  + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = null, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                // error cases
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationXml,ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, ApplicationXml, tc.Version)},
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, ApplicationAtomSvcXml, tc.Version)},
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, TextStar, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, TextXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, TextPlain, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, ApplicationAtomXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, ApplicationAtomXmlEntry, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, ApplicationAtomXmlFeed, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = BuildContentType(ApplicationJson, "some=value"), ExpectedException = tc => GetExpectedException(ODataPayloadKind.ServiceDocument, BuildContentType(ApplicationJson, "some=value"), tc.Version) },
            };

            Func<PayloadWriterTestDescriptor<ODataServiceDocument>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> writerFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteServiceDocument(testDescriptor.PayloadItems[0]);
            this.RunTopLevelContentTypeTest(serviceDocument, ODataPayloadKind.ServiceDocument, null, null, writerFunc, testCases, true);
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test response content types when writing an error.")]
        public void ErrorContentTypeTests()
        {
            ODataError error = new ODataError()
            {
                Message = "Some error message."
            };

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Json, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";" + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = null, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";"  + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = Default, ExpectedContentType = ApplicationJsonODataLightStreamingAndDefaultMetadata + ";"  + CharsetUtf8 },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                // error cases
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Error, ApplicationXml, tc.Version) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Error, TextStar, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Error, TextXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Error, TextPlain, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Error, ApplicationAtomXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Error, ApplicationAtomSvcXml, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Error, ApplicationAtomXmlEntry, tc.Version) },
                //new ContentTypeTestCase { Format = ODataFormat.Atom, AcceptHeaders = BuildContentType(ApplicationJson, "some=value"), ExpectedException = tc => GetExpectedException(ODataPayloadKind.Error, BuildContentType(ApplicationJson, "some=value"), tc.Version) },
            };

            Func<PayloadWriterTestDescriptor<ODataError>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> writerFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteError(testDescriptor.PayloadItems[0], false);
            this.RunTopLevelContentTypeTest(error, ODataPayloadKind.Error, null, null, writerFunc, testCases, true);
        }

        [TestMethod, Variation(Description = "Test response content types when writing batch.")]
        public void BatchContentTypeTests()
        {
            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { UseFormat = true, Format = null, ExpectedFormat = ODataFormat.Batch },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = null },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = Default },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = StarStar },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = MultipartStar },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = MultipartMixed },
                // error cases
                //new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Atom, ExpectedException = tc => GetExpectedException(ODataFormat.Atom) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = TextStar, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, TextStar, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = TextXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, TextXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = TextPlain, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, TextPlain, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = ApplicationXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, ApplicationXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = ApplicationAtomXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, ApplicationAtomXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = ApplicationAtomSvcXml, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, ApplicationAtomSvcXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = ApplicationAtomXmlEntry, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, ApplicationAtomXmlEntry, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = ApplicationAtomXmlFeed, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, ApplicationAtomXmlFeed, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = MultipartHttp, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, MultipartHttp, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Batch, AcceptHeaders = BuildContentType(MultipartHttp, "some=value"), ExpectedException = tc => GetExpectedException(ODataPayloadKind.Batch, BuildContentType(MultipartHttp, "some=value"), tc.Version) },
            };

            this.RunBatchContentTypeTest(testCases);
        }

        [TestMethod, Variation(Description = "Test content types when writing a parameters payload.")]
        public void ParameterPayloadContentTypeTests()
        {
            EdmModel model = new EdmModel();

            var streetType = new EdmComplexType("My", "StreetType");
            streetType.AddStructuralProperty("StreetName", EdmCoreModel.Instance.GetString(true));
            streetType.AddStructuralProperty("Number", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(streetType);

            var nestedAddressType = new EdmComplexType("My", "NestedAddressType");
            nestedAddressType.AddStructuralProperty("Street", new EdmComplexTypeReference(streetType, false));
            nestedAddressType.AddStructuralProperty("City", EdmCoreModel.Instance.GetString(true));
            model.AddElement(nestedAddressType);

            var container = new EdmEntityContainer("My", "ObjectModelContainer");

            var function = new EdmFunction("My", "DefaultParametersFunction", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("primitiveParameter", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("complexParameter", new EdmComplexTypeReference(nestedAddressType, false));
            function.AddParameter("primitiveCollectionParameter", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            function.AddParameter("complexCollectionParameter", EdmCoreModel.GetCollection(new EdmComplexTypeReference(nestedAddressType, false)));
            container.AddFunctionImport("DefaultParametersFunction", function);
            model.AddElement(container);

            ODataParameters parameterPayload = CreateDefaultParameter();

            //EdmOperationImport
            EdmOperationImport functionImport = container.FindOperationImports("DefaultParametersFunction").Single() as EdmOperationImport;

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = null, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = Default, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = StarStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationStar, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // error cases
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Batch, ExpectedException = tc => GetExpectedException(ODataFormat.Batch) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.Metadata, ExpectedException = tc => GetExpectedException(ODataFormat.Metadata) },
                new ContentTypeTestCase { UseFormat = true, Format = ODataFormat.RawValue, ExpectedException = tc => GetExpectedException(ODataFormat.RawValue) },
            };

            this.RunParameterContentTypeTest(parameterPayload, model, functionImport, testCases);
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Encoding content types variation")]
        public void EncodingContentTypeTests()
        {
            // TODO: this is a first, reasonable set of encoding tests. More are needed for full coverage.
            var model = new EdmModel();
            EdmEntityType entity = new EdmEntityType("TestModel", "DefaultEntityType");
            entity.AddKeys(entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(entity);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("DefaultEntitySet", entity);
            model.AddElement(container);

            var entry = CreateDefaultEntryWithAtomMetadata("TestModel.DefaultEntityType");
            var entitySet = container.FindEntitySet("DefaultEntitySet") as EdmEntitySet;
            var entityType = model.FindType("TestModel.DefaultEntityType") as EdmEntityType;

            var testCases = new ContentTypeTestCase[]
            {
                // success cases
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = StarStar, Encoding=Utf8, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationStar, Encoding=Utf16, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf16) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationStar, Encoding="utF-16", ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf16) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationStar, Encoding="iSo-8859-1", ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharSetIso88591) },
            };

            this.RunContentTypeTest(entry, ODataPayloadKind.Resource, model, entitySet, entityType, testCases);

            // error cases
            var errorCases = new ContentTypeTestCase[]
            {
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomXml, Encoding="UTF-8;q=0.6, ISO-10646-UCS-2;q=0.8", ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, ApplicationAtomXml, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationAtomXmlEntry, Encoding="ISO-10646-UCS-2", ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, ApplicationAtomXmlEntry, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = AcceptHeaderWithMultipleSupportedFeedTypes, Encoding=null, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithMultipleSupportedFeedTypes, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = AcceptHeaderWithQualityValues1, Encoding="UTF-8;q=0.8, UTF-16;q=0.8", ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithQualityValues1, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = AcceptHeaderWithQualityValues2, Encoding="UTF-16;q=0.8, UTF-8;q=0.8", ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithQualityValues2, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = AcceptHeaderWithMultipleSupportedFeedTypes, Encoding=string.Empty, ExpectedException = tc => GetExpectedException(ODataPayloadKind.Resource, AcceptHeaderWithMultipleSupportedFeedTypes, tc.Version) },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationStar, Encoding = "===,*", ExpectedException = tc => GetExpectedInvalidEncodingException("===,*") },
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationStar, Encoding = CharsetUtf8, ExpectedException = tc => GetExpectedEncodingException(CharsetUtf8) },
            };

            this.RunContentTypeTest(entry, ODataPayloadKind.Resource, model, entitySet, entityType, errorCases);
        }

        private EdmModel GenerateAppJsonContentTypeModel()
        {
            EdmModel model = new EdmModel();
            var testContainer = new EdmEntityContainer("TestNS", "TestContainer");

            var customerInfoType = new EdmEntityType("TestNS", "CustomerInfo");
            customerInfoType.AddKeys(customerInfoType.AddStructuralProperty("CustomerInfoID", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(customerInfoType);
            testContainer.AddEntitySet("CustomerInfo", customerInfoType);

            var orderType = new EdmEntityType("TestNS", "Order");
            orderType.AddKeys(orderType.AddStructuralProperty("OrderID", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(orderType);
            testContainer.AddEntitySet("Orders", orderType);

            var customerType = new EdmEntityType("TestNS", "Customer");
            customerType.AddKeys(customerType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Info",
                Target = customerInfoType,
                TargetMultiplicity = EdmMultiplicity.One,
            });
            customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Orders",
                Target = orderType,
                TargetMultiplicity = EdmMultiplicity.Many,
            });
            model.AddElement(customerType);
            testContainer.AddEntitySet("Customers", customerType);

            var addressComplexType = new EdmComplexType("TestNS", "AddressType");
            addressComplexType.AddStructuralProperty("FirstName", EdmCoreModel.Instance.GetString(true));
            addressComplexType.AddStructuralProperty("LastName", EdmCoreModel.Instance.GetString(true));
            model.AddElement(addressComplexType);

            var owningEntityType = new EdmEntityType("TestNS", "OwningEntityType");
            owningEntityType.AddStructuralProperty("PrimitiveCollectionProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            owningEntityType.AddStructuralProperty("ComplexCollectionProperty", EdmCoreModel.GetCollection(new EdmComplexTypeReference(addressComplexType, false)));
            owningEntityType.AddStructuralProperty("Age", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(owningEntityType);
            testContainer.AddEntitySet("OwningEntityType", owningEntityType);

            model.AddElement(testContainer);

            return model;
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that each payload kind which can be serialized in V1-V3 deals with 'application/json' and its variants in a version appropriate way")]
        public void AppJsonContentTypeVersioningTest()
        {
            EdmModel model = this.GenerateAppJsonContentTypeModel();

            var testContainer = model.EntityContainer;

            var custType = model.EntityTypes().Single(et => et.Name == "Customer");
            IEdmNavigationProperty singletonNavProp = custType.DeclaredProperties.OfType<IEdmNavigationProperty>().Single(p => p.Name == "Info");

            var customerSet = testContainer.FindEntitySet("Customers") as EdmEntitySet;
            var customerType = model.FindDeclaredType("TestNS.Customer") as EdmEntityType;

            var owningEntityType = testContainer.FindEntitySet("OwningEntityType") as EdmEntitySet;

            ODataError error = new ODataError()
            {
                Message = "Some error message."
            };

            ODataServiceDocument serviceDocument = ObjectModelUtils.CreateDefaultWorkspace();

            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org/") };
            ODataProperty property = new ODataProperty() { Name = "Age", Value = 42 };

            Func<PayloadWriterTestDescriptor<ODataError>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> errorWriterFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteError(testDescriptor.PayloadItems[0], false);
            Func<PayloadWriterTestDescriptor<ODataServiceDocument>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> serviceDocWriterFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteServiceDocument(testDescriptor.PayloadItems[0]);
            Func<PayloadWriterTestDescriptor<ODataEntityReferenceLink>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> linkWriterFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteEntityReferenceLink(testDescriptor.PayloadItems[0]);
            Func<PayloadWriterTestDescriptor<ODataProperty>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> propertyWriterFunc =
                (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteProperty(testDescriptor.PayloadItems[0]);
#if !SILVERLIGHT
            this.CombinatorialEngineProvider.SkipVerify();
#endif
            // Run the same set of test cases for each JSON-serializing payload kind.
            this.RunTopLevelContentTypeTest(error, ODataPayloadKind.Error, null, null, errorWriterFunc, CreateAppJsonVersioningTestCases(ODataPayloadKind.Error), true);
            this.RunTopLevelContentTypeTest(serviceDocument, ODataPayloadKind.ServiceDocument, model, null, serviceDocWriterFunc, CreateAppJsonVersioningTestCases(ODataPayloadKind.ServiceDocument), true);
            this.RunTopLevelContentTypeTest(entityReferenceLink, ODataPayloadKind.EntityReferenceLink, model, null, linkWriterFunc, CreateAppJsonVersioningTestCases(ODataPayloadKind.EntityReferenceLink));
            this.RunTopLevelContentTypeTest(property, ODataPayloadKind.Property, model, owningEntityType, propertyWriterFunc, CreateAppJsonVersioningTestCases(ODataPayloadKind.Property));
#if !SILVERLIGHT
            Approvals.Verify(new ApprovalTextWriter(this.Logger.GetBaseline()), new CustomSourcePathNamer(this.TestContext.DeploymentDirectory), Approvals.GetReporter());
#endif
        }

        /// <summary>
        /// TODO: This was test is split from AppJsonContentTypeVersioningTest, as the payload for entity reference links has been changed in V4.
        /// Earlier it used to be of type application/xml or text/xml, but in V4, it has to be application/atom+xml, since the root element is feed.
        /// The CreateAppJsonVersioningTestCases has one test case where it tries to set AcceptCharSet to "application/json;q=0.8,application/atom+xml
        /// Since both of these are acceptable in case of reference links, the product code sends out Atom payload (since that was the last one specified).
        /// Since this test is supposed to be testing JSON payload, this need to be changed to work with JSON.
        /// </summary>
        [Ignore] // Remove Atom
        // [TestMethod]
        public void AppJsonContentTypeVersioningEntityReferenceLinksTest()
        {
            EdmModel model = this.GenerateAppJsonContentTypeModel();

            var testContainer = model.EntityContainer;
            var custType = model.EntityTypes().Single(et => et.Name == "Customer");
            IEdmNavigationProperty collectionNavProp = custType.DeclaredProperties.OfType<IEdmNavigationProperty>().Single(p => p.Name == "Orders");

            var customerSet = testContainer.FindEntitySet("Customers") as EdmEntitySet;

            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks
            {
                Links = new ODataEntityReferenceLink[]
                {
                    new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org/1") },
                    new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org/2") },
                    new ODataEntityReferenceLink { Url = new Uri("http://www.odata.org/3") },
                },
            };

            Func<PayloadWriterTestDescriptor<ODataEntityReferenceLinks>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> linksWriterFunc =
    (testDescriptor, testConfiguration) => messageWriter => messageWriter.WriteEntityReferenceLinks(testDescriptor.PayloadItems[0]);

            this.RunTopLevelContentTypeTest(entityReferenceLinks, ODataPayloadKind.EntityReferenceLinks, model, null, linksWriterFunc, CreateAppJsonVersioningTestCases(ODataPayloadKind.EntityReferenceLinks));
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that each payload Primitive kind which can be serialized in V1-V3 deals with 'application/json' and its variants in a version appropriate way")]
        public void AppJsonContentTypeVersioningEntryTest()
        {
            EdmModel model = this.GenerateAppJsonContentTypeModel();

            var testContainer = model.EntityContainer;
            var customerSet = testContainer.FindEntitySet("Customers") as EdmEntitySet;
            var customerType = model.FindDeclaredType("TestNS.Customer") as EdmEntityType;

            ODataResource entry = CreateDefaultEntryWithAtomMetadata("TestNS.Customer");

            this.RunContentTypeTest(entry, ODataPayloadKind.Resource, model, customerSet, customerType, CreateAppJsonVersioningTestCases(ODataPayloadKind.Resource));
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that each payload Primitive kind which can be serialized in V1-V3 deals with 'application/json' and its variants in a version appropriate way")]
        public void AppJsonContentTypeVersioningFeedTest()
        {
            EdmModel model = this.GenerateAppJsonContentTypeModel();

            var testContainer = model.EntityContainer;
            var customerSet = testContainer.FindEntitySet("Customers") as EdmEntitySet;
            var customerType = model.FindDeclaredType("TestNS.Customer") as EdmEntityType;

            ODataResourceSet resourceCollection = ObjectModelUtils.CreateDefaultFeed();

            this.RunContentTypeTest(resourceCollection, ODataPayloadKind.ResourceSet, model, customerSet, customerType, CreateAppJsonVersioningTestCases(ODataPayloadKind.ResourceSet));
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that each payload Primitive kind which can be serialized in V1-V3 deals with 'application/json' and its variants in a version appropriate way")]
        public void AppJsonContentComplexTypeVersioningTest()
        {
            EdmModel model = this.GenerateAppJsonContentTypeModel();

            ODataComplexValue complexValue1 = new ODataComplexValue()
            {
                TypeName = "TestNS.AddressType",
                Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "FirstName", Value = "Bob" },
                    new ODataProperty { Name = "LastName", Value = "Marley" },
                }
            };
            ODataComplexValue complexValue2 = new ODataComplexValue()
            {
                TypeName = "TestNS.AddressType",
                Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "FirstName", Value = "Jimmy" },
                    new ODataProperty { Name = "LastName", Value = "Hendrix" },
                }
            };

            var addressComplexType = (EdmComplexType)model.FindDeclaredType("TestNS.AddressType");

            object[] complexCollection = new object[] { complexValue1, complexValue2 };

            this.RunCollectionContentTypeTest(complexCollection, model, "ComplexCollectionFunction", "Collection(TestCollectionType)", new EdmComplexTypeReference(addressComplexType, false), this.CreateAppJsonVersioningTestCases(ODataPayloadKind.Collection));
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that each payload Primitive kind which can be serialized in V1-V3 deals with 'application/json' and its variants in a version appropriate way")]
        public void AppJsonContentPrimitivetTypeVersioningTest()
        {
            EdmModel model = this.GenerateAppJsonContentTypeModel();

            object[] primitiveCollection = new object[] { 41, 42, 43 };

            this.RunCollectionContentTypeTest(primitiveCollection, model, "PrimitiveCollectionFunction", "Collection(TestCollectionType)", EdmCoreModel.Instance.GetInt32(false), this.CreateAppJsonVersioningTestCases(ODataPayloadKind.Collection));
        }

        private IEnumerable<ContentTypeTestCase> CreateAppJsonVersioningTestCases(ODataPayloadKind payloadKind)
        {
            Func<string> applicationAtomXmlContentType = () =>
                {
                    switch (payloadKind)
                    {
                        case ODataPayloadKind.ResourceSet:
                        case ODataPayloadKind.EntityReferenceLinks:
                            return BuildContentType(ApplicationJsonODataLightStreaming, CharsetUtf8);
                        case ODataPayloadKind.Resource:
                            return BuildContentType(ApplicationJsonODataLightStreaming, CharsetUtf8);
                        default:
                            return null;
                    }
                };

            return new ContentTypeTestCase[]
            {
                // "application/json,application/atom+xml;q=0.8" should use  "application/json;odata.metadata=minimal"
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonAndAtomXmlWithQuality, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // "application/json;q=0.8,application/atom+xml" should use  "application/json;odata.metadata=minimal" (for the payload kinds applicable)
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonWithQualityAndAtomXml, ExpectedContentType = applicationAtomXmlContentType() ?? BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // "application/json,application/atom+xml" should use "application/atom+xml" for all applicable payload kinds, otherwise JSON Light
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonAndAtomXml, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // "application/json,application/json;odata.metadata=minimal;odata.streaming=true" should use "application/json;odata.metadata=minimal;odata.streaming=true"
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonAndJsonLightStreaming, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // "application/json,application/json;odata.streaming=true" should use "application/json;odata.metadata=minimal;odata.streaming=true"
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonAndJsonStreaming, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // "application/json;odata.streaming=true,application/json;odata.streaming=false" "application/json;odata.metadata=minimal;odata.streaming=true"
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonStreamingAndJsonNonStreaming, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // "application/json;odata.metadata=minimal;odata.streaming=false,application/json;odata.streaming=true" "application/json;odata.metadata=minimal;odata.streaming=true"
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonLightNonStreamingAndJsonStreaming, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },

                // "application/json;odata.metadata=minimal;odata.streaming=false,application/json;odata.metadata=minimal;odata.streaming=true" "application/json;odata.metadata=minimal;odata.streaming=true"
                new ContentTypeTestCase { Format = ODataFormat.Json, AcceptHeaders = ApplicationJsonLightNonStreamingAndJsonLightStreaming, ExpectedContentType = BuildContentType(ApplicationJsonODataLightStreamingAndDefaultMetadata, CharsetUtf8) },
            };
        }

        private void RunParameterContentTypeTest(ODataParameters parameters, EdmModel model, EdmOperationImport functionImport, IEnumerable<ContentTypeTestCase> testCases)
        {
            var testConfigurations = CreateContentTypeTestConfigurations(testCases).Where(c => c.IsRequest == true);
            var testDescriptors = new[] {
                new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, parameters, CreateContentTypeResultCallback(testCases, this.Settings.ExpectedResultSettings, /*responseOnly*/ false))
                {
                    Model = model,
                    PayloadEdmElementContainer = functionImport
                }};

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                testConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    IEdmOperationImport edmFunctionImport = null;
                    EdmOperationImport modelFunctionImport = testDescriptor.PayloadEdmElementContainer as EdmOperationImport;
                    if (functionImport != null)
                    {
                        IEdmModel edmModel = testDescriptor.GetMetadataProvider();
                        edmFunctionImport = edmModel.FindEntityContainer(functionImport.Container.FullName()).FindOperationImports(functionImport.Name).First();
                    }

                    TestWriterUtils.WriteAndVerifyODataParameterPayload(testDescriptor, testConfiguration, this.Assert, this.Logger, edmFunctionImport);

                    WriterTestExpectedResults expectedResults = testDescriptor.ExpectedResultCallback(testConfiguration);
                    TestWriterUtils.SetPayloadKindAndVerifyContentType(ODataPayloadKind.Parameter, expectedResults, testConfiguration, this.Assert);
                });
        }

        private void RunContentTypeTest(ODataItem item, ODataPayloadKind payloadKind, EdmModel model, EdmEntitySet entitySet, EdmEntityType entityType, IEnumerable<ContentTypeTestCase> testCases)
        {
            var testConfigurations = CreateContentTypeTestConfigurations(testCases);
            var testDescriptors = new[] {
                new PayloadWriterTestDescriptor<ODataItem>(this.Settings, item, CreateContentTypeResultCallback(testCases, this.Settings.ExpectedResultSettings))
                {
                    Model = model,
                    PayloadEdmElementContainer = entitySet,
                    PayloadEdmElementType = entityType,
                }};

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                testConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);

                    WriterTestExpectedResults expectedResults = testDescriptor.ExpectedResultCallback(testConfiguration);
                    TestWriterUtils.SetPayloadKindAndVerifyContentType(payloadKind, expectedResults, testConfiguration, this.Assert);
                });
        }

        private void RunTopLevelContentTypeTest<T>(
            T value,
            ODataPayloadKind payloadKind,
            EdmModel model,
            EdmElement modelContainer,
            Func<PayloadWriterTestDescriptor<T>, WriterTestConfiguration, Action<ODataMessageWriterTestWrapper>> writerFunc,
            IEnumerable<ContentTypeTestCase> testCases,
            bool onlyResponses = false)
        {
            var testConfigurations = CreateContentTypeTestConfigurations(testCases);
            var callback = CreateContentTypeResultCallback(testCases, this.Settings.ExpectedResultSettings);
            var testDescriptors = new[] { new PayloadWriterTestDescriptor<T>(this.Settings, value, callback)
                {
                    Model = model,
                    PayloadEdmElementContainer = modelContainer
                }};

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                onlyResponses ? testConfigurations.Where(tc => !tc.IsRequest) : testConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // Top-level EntityReferenceLinks payload write requests are not allowed.
                    if (payloadKind == ODataPayloadKind.EntityReferenceLinks && testConfiguration.IsRequest)
                    {
                        return;
                    }

                    ODataMessageWriterSettings settingsWithBaseUri = testConfiguration.MessageWriterSettings.Clone();
                    settingsWithBaseUri.BaseUri = baseUri;

                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        writerFunc(testDescriptor, testConfiguration),
                        this.Assert,
                        settingsWithBaseUri,
                        baselineLogger: this.Logger);

                    WriterTestExpectedResults expectedResults = testDescriptor.ExpectedResultCallback(testConfiguration);
                    TestWriterUtils.SetPayloadKindAndVerifyContentType(payloadKind, expectedResults, testConfiguration, this.Assert);
                });
        }

        private void RunRawValueContentTypeTest(object value, ODataPayloadKind payloadKind, IEnumerable<ContentTypeTestCase> testCases)
        {
            var testConfigurations = CreateContentTypeTestConfigurations(testCases);
            var testDescriptors = new[] { new PayloadWriterTestDescriptor<object>(this.Settings, value, CreateContentTypeResultCallback(testCases, this.Settings.ExpectedResultSettings)) };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                testConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyRawContent(testDescriptor, testConfiguration, this.Assert, this.Logger);

                    WriterTestExpectedResults expectedResults = testDescriptor.ExpectedResultCallback(testConfiguration);
                    TestWriterUtils.SetPayloadKindAndVerifyContentType(payloadKind, expectedResults, testConfiguration, this.Assert);
                });
        }

        private void RunCollectionContentTypeTest(object[] values, EdmModel model, string collectionName, string collectionTypeName, IEdmTypeReference collectionItemType, IEnumerable<ContentTypeTestCase> testCases)
        {
            var testConfigurations = CreateContentTypeTestConfigurations(testCases);
            var testDescriptors = new[] {
                new CollectionWriterTestDescriptor(this.CollectionSettings, collectionName, collectionTypeName, values, CreateContentTypeResultCallback(testCases, this.Settings.ExpectedResultSettings), model)
                {
                    //PayloadModel = model,
                    ItemTypeParameter = collectionItemType
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                // collections are only supported in responses
                testConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);

                    WriterTestExpectedResults expectedResults = testDescriptor.ExpectedResultCallback(testConfiguration);
                    TestWriterUtils.SetPayloadKindAndVerifyContentType(ODataPayloadKind.Collection, expectedResults, testConfiguration, this.Assert);
                });
        }

        private void RunBatchContentTypeTest(IEnumerable<ContentTypeTestCase> testCases)
        {
            var testConfigurations = CreateContentTypeTestConfigurations(testCases);
            var resultCallback = CreateContentTypeResultCallback(testCases, this.Settings.ExpectedResultSettings);

            this.CombinatorialEngineProvider.RunCombinations(
                testConfigurations,
                (testConfiguration) =>
                {
                    var expectedResults = resultCallback(testConfiguration);
                    BatchWriterTestDescriptor testDescriptor;
                    BatchWriterTestDescriptor.InvocationAndOperationDescriptor[] batchPayload;
                    if (testConfiguration.IsRequest)
                    {
                        batchPayload = BatchWriterUtils.CreateDefaultQueryBatch(1);
                    }
                    else
                    {
                        batchPayload = BatchWriterUtils.CreateDefaultQueryResponseBatch(1);
                    }

                    if (expectedResults.ExpectedException2 != null)
                    {
                        testDescriptor = new BatchWriterTestDescriptor(this.BatchSettings, batchPayload, expectedResults.ExpectedException2);
                    }
                    else
                    {
                        // The Content-Type header is verified by default by the writer utils to be the multipart which is required for batch.
                        testDescriptor = new BatchWriterTestDescriptor(this.BatchSettings, batchPayload, new Dictionary<string, string>());
                    }

                    BatchWriterUtils.WriteAndVerifyBatchPayload(testDescriptor, testConfiguration, testConfiguration, this.Assert);

                    TestWriterUtils.SetPayloadKindAndVerifyContentType(ODataPayloadKind.Batch, expectedResults, testConfiguration, this.Assert);
                });
        }

        private static ExpectedException GetExpectedException(ODataPayloadKind kind, string acceptHeader, ODataVersion version)
        {
            string supportedTypes = TestMediaTypeUtils.GetSupportedMediaTypes(kind, /*includeApplicationJson*/ true);

            return ODataExpectedExceptions.ODataContentTypeException("MediaTypeUtils_DidNotFindMatchingMediaType", supportedTypes, acceptHeader);
        }

        private static ExpectedException GetExpectedException(ODataFormat format)
        {
            return ODataExpectedExceptions.ODataException("ODataUtils_DidNotFindDefaultMediaType", format.ToString());
        }

        private static ExpectedException GetExpectedEncodingException(string charset)
        {
            return ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MissingSeparatorBetweenCharsets", charset);
        }

        private static ExpectedException GetExpectedInvalidEncodingException(string charset)
        {
            return ODataExpectedExceptions.ODataContentTypeException("HttpUtils_InvalidCharsetName", charset);
        }

        private static PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateContentTypeResultCallback(
            IEnumerable<ContentTypeTestCase> testCases,
            WriterTestExpectedResults.Settings settings,
            bool responseOnly = true)
        {
            return (testConfiguration) =>
            {
                ContentTypeTestCase testCase;
                TestWriterUtils.ODataMessageWriterSettingsTestWrapper settingsWrapper = new TestWriterUtils.ODataMessageWriterSettingsTestWrapper(testConfiguration.MessageWriterSettings);
                if (settingsWrapper.UseFormat)
                {
                    testCase = testCases.SingleOrDefault(tc => tc.UseFormat == settingsWrapper.UseFormat && tc.Format == testConfiguration.Format);
                }
                else
                {
                    testCase = testCases.SingleOrDefault(tc =>
                        tc.UseFormat == settingsWrapper.UseFormat
                        && tc.Format == testConfiguration.Format
                        && tc.AcceptHeaders == settingsWrapper.AcceptableMediaTypes
                        && tc.Encoding == settingsWrapper.AcceptableCharsets
                        && tc.Version == settingsWrapper.Version);
                }

                if (testCase == null)
                {
                    throw new InvalidOperationException("Could not retrieve test case for accept header: " + settingsWrapper.AcceptableMediaTypes + ".");
                }

                ExpectedException expectedException = testCase.ExpectedException == null ? null : testCase.ExpectedException(testConfiguration);
                string expectedContentType = null;
                if (!responseOnly || !testConfiguration.IsRequest)
                {
                    expectedContentType = testCase.ExpectedContentType;
                }

                if (testConfiguration.Format == ODataFormat.Json)
                {
                    return new JsonWriterTestExpectedResults(settings) { ExpectedContentType = expectedContentType, ExpectedException2 = expectedException };
                }
                else if (testConfiguration.Format == ODataFormat.RawValue || testConfiguration.Format == ODataFormat.Batch || testConfiguration.Format == ODataFormat.Metadata)
                {
                    return new RawValueWriterTestExpectedResults(settings) { ExpectedContentType = expectedContentType, ExpectedException2 = expectedException };
                }
                else
                {
                    throw new TaupoNotSupportedException("Format " + testConfiguration.Format.GetType().Name + " is not supported.");
                }
            };
        }

        private static IEnumerable<WriterTestConfiguration> CreateContentTypeTestConfigurations(IEnumerable<ContentTypeTestCase> testCases)
        {
            Func<ODataFormat, bool, string, string, ODataVersion, ODataMessageWriterSettings> settingsCreator = (format, useFormat, acceptableMediaTypes, encoding, version) =>
                {
                    ODataMessageWriterSettings settings = new ODataMessageWriterSettings()
                    {
                        Version = version,
                        BaseUri = new Uri("http://www.odata.org/"),
                    };
                    settings.SetServiceDocumentUri(new Uri("http://odata.org/test/"));

                    if (useFormat)
                    {
                        settings.SetContentType(format);
                    }
                    else
                    {
                        settings.SetContentType(acceptableMediaTypes, encoding);
                    }

                    return settings;
                };

            bool[] IsRequest = new bool[] { true, false };
            bool[] synchronous = new bool[] { false, true };
            return testCases
                .SelectMany(tc => IsRequest
                    .SelectMany(wr => synchronous
                        .Select(sync => new WriterTestConfiguration(
                            tc.ExpectedFormat != null ? tc.ExpectedFormat : tc.Format,
                            settingsCreator(tc.Format, tc.UseFormat, tc.AcceptHeaders, tc.Encoding, tc.Version),
                            wr,
                            sync))));
        }

        private static string BuildContentType(string mediaTypeName, string charset)
        {
            return mediaTypeName + ";" + charset;
        }

        private sealed class ContentTypeTestCase
        {
            public ContentTypeTestCase()
            {
                Version = ODataVersion.V4;
            }

            public string AcceptHeaders { get; set; }
            public ODataFormat Format { get; set; }
            public bool UseFormat { get; set; }
            public ODataFormat ExpectedFormat { get; set; }
            public string ExpectedContentType { get; set; }
            public Func<WriterTestConfiguration, ExpectedException> ExpectedException { get; set; }
            public string Encoding { get; set; }
            public ODataVersion Version { get; set; }
        }

        private static ODataResource CreateDefaultEntryWithAtomMetadata(string fullTypeName = null)
        {
            ODataResource entry = new ODataResource()
            {
                Id = ObjectModelUtils.DefaultEntryId,
                ReadLink = ObjectModelUtils.DefaultEntryReadLink,
                TypeName = fullTypeName,
            };
            return entry;
        }

        private static ODataParameters CreateDefaultParameter()
        {
            var street = new ODataResource()
            {
                TypeName = "My.StreetType",
                Properties = new[]
                {
                    new ODataProperty { Name = "StreetName", Value = "One Redmond Way" },
                    new ODataProperty { Name = "Number", Value = 1234 },
                }
            };

            var streetInfo = new ODataNestedResourceInfo() { Name = "Street", IsCollection = false };

            var streetInfo_expand = new ODataNavigationLinkExpandedItemObjectModelAnnotation();
            streetInfo_expand.ExpandedItem = street;
            streetInfo.SetAnnotation(streetInfo_expand);

            var address = new ODataResource()
            {
                TypeName = "My.NestedAddressType",
                Properties = new[]
                {
                    new ODataProperty() { Name = "City", Value = "Redmond " },
                }
            };

            var address_nested = new ODataEntryNavigationLinksObjectModelAnnotation();
            address_nested.Add(streetInfo, 0);
            address.SetAnnotation(address_nested);

            var primitiveCollectionValue = new ODataCollectionStart();
            primitiveCollectionValue.SetAnnotation(new ODataCollectionItemsObjectModelAnnotation() { "Value1", "Value2", "Value3" });

            var complexCollection = new ODataResourceSet();

            complexCollection.SetAnnotation(new ODataFeedEntriesObjectModelAnnotation() { address });

            return new ODataParameters()
            {
                new KeyValuePair<string, object>("primitiveParameter", "StringValue"),
                new KeyValuePair<string, object>("complexParameter", address),
                new KeyValuePair<string, object>("primitiveCollectionParameter", primitiveCollectionValue),
                new KeyValuePair<string, object>("complexCollectionParameter", complexCollection),
            };
        }
    }
}
#endif
