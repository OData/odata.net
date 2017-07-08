//---------------------------------------------------------------------
// <copyright file="WriterEntryDefaultStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Tests for writing entries with default streams with the OData writer.
    /// </summary>
    [TestClass, TestCase]
    public class WriterEntryDefaultStreamTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        private readonly static string[] JsonDefaultStreamMetadataPropertyNames = new string[]
            {
                JsonConstants.ODataMetadataMediaSourcePropertyName,
                JsonConstants.ODataMetadataContentTypePropertyName,
                JsonConstants.ODataMetadataEditMediaPropertyName,
                JsonConstants.ODataMetadataMediaETagPropertyName
            };

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Validates the payloads for various default streams.")]
        public void EntryDefaultStreamTest()
        {
            string readLink = "http://odata.org/read";
            Uri readLinkUri = new Uri(readLink);
            string editLink = "http://odata.org/edit";
            Uri editLinkUri = new Uri(editLink);
            string contentType = "application/binary";
            string etag = "\"myetagvalue\"";
            string atomEscapedEtag = "&quot;myetagvalue&quot;";
            string outOfContentProperties = "<m:properties xmlns:m=\"" + TestAtomConstants.ODataMetadataNamespace + "\"><d:ID xmlns:d=\"" + TestAtomConstants.ODataNamespace + "\">foo</d:ID></m:properties>";

            var testCases = new[]
            {
                new {  // Completely empty default stream is valid and should not produce any MLE/MR related values in the payload
                    DefaultStream = new ODataStreamReferenceValue(),
                    Atom = outOfContentProperties,
                },
                new {  // Bare minimum for valid default stream
                    DefaultStream = new ODataStreamReferenceValue() { ReadLink = readLinkUri, EditLink = null, ContentType = contentType, ETag = null },
                    Atom = "<content type=\"" + contentType + "\" src=\"" + readLinkUri + "\" xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />" +
                           outOfContentProperties
                },
                new {  // with edit link
                    DefaultStream = new ODataStreamReferenceValue() { ReadLink = readLinkUri, EditLink = editLinkUri, ContentType = contentType, ETag = null },
                    Atom = "<content type=\"" + contentType + "\" src=\"" + readLinkUri + "\" xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />" +
                           "<link rel=\"edit-media\" href=\"" + editLink + "\" xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />" +
                           outOfContentProperties
                },
                new {  // just edit link
                    DefaultStream = new ODataStreamReferenceValue() { ReadLink = null, EditLink = editLinkUri, ContentType = null, ETag = null },
                    Atom = "<link rel=\"edit-media\" href=\"" + editLink + "\" xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />" +
                           outOfContentProperties
                },
                // etag without edit link is invalid and is covered in input validation tests
                new {  // with edit link and etag
                    DefaultStream = new ODataStreamReferenceValue() { ReadLink = readLinkUri, EditLink = editLinkUri, ContentType = contentType, ETag = etag },
                    Atom = "<content type=\"" + contentType + "\" src=\"" + readLinkUri + "\" xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />" +
                           "<link rel=\"edit-media\" href=\"" + editLink + "\" p2:etag=\"" + atomEscapedEtag + "\" xmlns:p2=\"" + TestAtomConstants.ODataMetadataNamespace + "\" " +
                            "xmlns=\"" + TestAtomConstants.AtomNamespace + "\" />" +
                            outOfContentProperties
                }
            };

            var testDescriptors = testCases.Select(testCase =>
                {
                    ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                    entry.Properties = new ODataProperty[] { new ODataProperty { Name = "ID", Value = "foo" } };
                    entry.MediaResource = testCase.DefaultStream;
                    return new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        entry,
                        (testConfiguration) =>
                        {
                            return new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Xml = "<DefaultStream>" + testCase.Atom + "</DefaultStream>",
                                FragmentExtractor = (result) =>
                                {
                                    var content = result.Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomContentElementName);
                                    content = content == null ? null : new XElement(content.Name, content.Attributes());
                                    var mediaEditLink = result.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .Where(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.ODataStreamPropertyEditMediaSegmentName);
                                    var properties = result.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomPropertiesElementName);
                                    return new XElement("DefaultStream", content, mediaEditLink, properties);
                                }
                            };
                        });
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }
    }
}
