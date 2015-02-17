//---------------------------------------------------------------------
// <copyright file="WriterFeedAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing feed with the OData writer in ATOM format.
    /// </summary>
    [TestClass, TestCase]
    public class WriterFeedAtomTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        private sealed class PayloadOrderTestCase
        {
            public string DebugDescription { get; set; }
            public ODataFeed Feed { get; set; }
            public string Xml { get; set; }
            public bool NoEntitiesOnly { get; set; }
            public bool WithEntitiesOnly { get; set; }
        }

        [TestMethod, Variation(Description = "Test payload order when writing ATOM feeds.")]
        public void PayloadOrderTest()
        {
            ODataFeedAndEntrySerializationInfo info = new ODataFeedAndEntrySerializationInfo()
            {
                NavigationSourceEntityTypeName = "Null",
                NavigationSourceName = "MySet",
                ExpectedTypeName = "Null"
            };

            AtomFeedMetadata defaultAtomFeedMetadata = new AtomFeedMetadata() { 
                Authors = new[] { new AtomPersonMetadata { Name = "Bart" } }, 
                Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime };

            IEnumerable<PayloadOrderTestCase> testCases = new[]
            {
                new PayloadOrderTestCase
                {
                    DebugDescription = "Just ID",
                    Feed = new ODataFeed() { Id = new Uri("http://MyFeedId"), SerializationInfo = info }
                        .WithAnnotation(defaultAtomFeedMetadata),
                    Xml = string.Join(
                        "$(NL)",
                        "<feed xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <id>http://MyFeedId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name>Bart</name>",
                        "  </author>{0}",
                        "</feed>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "Just ID, and another ID at the end, the one from the starts is used",
                    Feed = new ODataFeed() { SerializationInfo = info }
                        .WithAnnotation(new WriteFeedCallbacksAnnotation
                            {
                                BeforeWriteStartCallback = (feed) => { feed.Id = new Uri("http://MyFeedId"); },
                                BeforeWriteEndCallback = (feed) => { feed.Id = new Uri("http://AnotherFeedId"); }
                            })
                        .WithAnnotation(defaultAtomFeedMetadata),
                    Xml = string.Join(
                        "$(NL)",
                        "<feed xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <id>http://MyFeedId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name>Bart</name>",
                        "  </author>{0}",
                        "</feed>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "ID and no author metadata and no entities, the default author should be written.",
                    Feed = new ODataFeed() { Id = new Uri("http://MyFeedId"), SerializationInfo = info }
                        .WithAnnotation(new AtomFeedMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<feed xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <id>http://MyFeedId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>{0}",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "</feed>"),
                    NoEntitiesOnly = true
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "ID and no author metadata but with entities, no author is written.",
                    Feed = new ODataFeed() { Id = new Uri("http://MyFeedId"), SerializationInfo = info }
                        .WithAnnotation(new AtomFeedMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<feed xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <id>http://MyFeedId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>{0}",
                        "</feed>"),
                    WithEntitiesOnly = true
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "ID, count at the end - the count is ignored",
                    Feed = new ODataFeed() { Id = new Uri("http://MyFeedId"), SerializationInfo = info }
                        .WithAnnotation(new WriteFeedCallbacksAnnotation
                            {
                                BeforeWriteStartCallback = (feed) => { feed.Count = null; },
                                BeforeWriteEndCallback = (feed) => { feed.Count = 42; }
                            })
                        .WithAnnotation(defaultAtomFeedMetadata),
                    Xml = string.Join(
                        "$(NL)",
                        "<feed xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <id>http://MyFeedId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name>Bart</name>",
                        "  </author>{0}",
                        "</feed>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "ID, count and next link",
                    Feed = new ODataFeed() { Id = new Uri("http://MyFeedId"), Count = 42, NextPageLink = new Uri("http://odata.org/nextlink"), SerializationInfo = info  }
                        .WithAnnotation(defaultAtomFeedMetadata),
                    Xml = string.Join(
                        "$(NL)",
                        "<feed xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <count xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">42</count>",
                        "  <id>http://MyFeedId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name>Bart</name>",
                        "  </author>{0}",
                        "  <link rel=\"next\" href=\"http://odata.org/nextlink\" />",
                        "</feed>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "ID, count and next link and some ATOM metadata, metadata goes before entries but author is the last of them.",
                    Feed = new ODataFeed() { Id = new Uri("http://MyFeedId"), Count = 42, NextPageLink = new Uri("http://odata.org/nextlink"), SerializationInfo = info }
                        .WithAnnotation(new AtomFeedMetadata() { 
                            Authors = new[] { new AtomPersonMetadata { Name = "Bart" } }, 
                            Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime,
                            Logo = new Uri("http://odata.org/logo") }),
                    Xml = string.Join(
                        "$(NL)",
                        "<feed xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <count xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">42</count>",
                        "  <id>http://MyFeedId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <logo>http://odata.org/logo</logo>",
                        "  <author>",
                        "    <name>Bart</name>",
                        "  </author>{0}",
                        "  <link rel=\"next\" href=\"http://odata.org/nextlink\" />",
                        "</feed>"),
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Where(testCase => !testCase.WithEntitiesOnly).Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Feed },
                    tc => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Xml = string.Format(CultureInfo.InvariantCulture, testCase.Xml, string.Empty),
                        FragmentExtractor = (element) => element
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                });

            testDescriptors = testDescriptors.Concat(testCases.Where(testCase => !testCase.NoEntitiesOnly).Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { 
                        testCase.Feed, 
                        new ODataEntry { }
                            .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    },
                    tc => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Xml = string.Format(CultureInfo.InvariantCulture, testCase.Xml,
                            string.Join(
                                "$(NL)",
                                "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                                "  <id />",
                                "  <title />",
                                "  <updated>2010-10-12T17:13:00Z</updated>",
                                "  <author>",
                                "    <name />",
                                "  </author>",
                                "  <content type=\"application/xml\" />",
                                "</entry>")),
                        FragmentExtractor = (element) => element
                    })
                {
                    DebugDescription = testCase.DebugDescription + "- with entry",
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent
                    .Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test writing feeds with various ID values.")]
        public void FeedIdTest()
        {
            var testCases = new[]
                {
                    new
                    {
                        Id = (Uri)null,
                        Xml = (string)null,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriter_FeedsMustHaveNonEmptyId"),
                    },
                    new
                    {
                        Id = new Uri("urn:id"),
                        Xml = @"<id xmlns=""" + TestAtomConstants.AtomNamespace + @""">urn:id</id>",
                        ExpectedException = (ExpectedException)null,
                    },
                };

            var testDescriptors = testCases.Select(tc =>
            {
                ODataFeed feed = ObjectModelUtils.CreateDefaultFeed();
                feed.Id = tc.Id;

                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    feed,
                    testConfiguration =>
                    {
                        this.Assert.AreEqual(ODataFormat.Atom, testConfiguration.Format, "Only ATOM feeds support ID.");
                        return new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Xml = tc.Xml,
                            FragmentExtractor = (element) => element.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomIdElementName).Single(),
                            ExpectedException2 = tc.ExpectedException,
                        };
                    });
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.FeedPayloads),
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent.Where(tc => !tc.IsRequest),
                (testPayload, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testPayload, testConfiguration, this.Assert, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Verifies the writer uses the same default value for <atom:updated> in a given payload.")]
        public void FeedAndEntryUpdatedTimeTests()
        {
            ODataFeed defaultFeedWithEmptyMetadata = ObjectModelUtils.CreateDefaultFeed();
            defaultFeedWithEmptyMetadata.SetAnnotation<AtomFeedMetadata>(new AtomFeedMetadata());
            ODataEntry defaultEntryWithEmptyMetadata = ObjectModelUtils.CreateDefaultEntry();
            defaultEntryWithEmptyMetadata.SetAnnotation<AtomEntryMetadata>(new AtomEntryMetadata());

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testPayloads =
                new[]
                {
                    new PayloadWriterTestDescriptor<ODataItem>(this.Settings, ObjectModelUtils.CreateDefaultFeed()),
                    new PayloadWriterTestDescriptor<ODataItem>(this.Settings, defaultFeedWithEmptyMetadata),
                    new PayloadWriterTestDescriptor<ODataItem>(this.Settings, ObjectModelUtils.CreateDefaultFeedWithAtomMetadata()),
                }.PayloadCases(WriterPayloads.FeedPayloads)
                .Concat((new[]
                {
                    new PayloadWriterTestDescriptor<ODataItem>(this.Settings, ObjectModelUtils.CreateDefaultEntry()),
                    new PayloadWriterTestDescriptor<ODataItem>(this.Settings, defaultEntryWithEmptyMetadata),
                    new PayloadWriterTestDescriptor<ODataItem>(this.Settings, ObjectModelUtils.CreateDefaultEntryWithAtomMetadata()),
                }.PayloadCases(WriterPayloads.EntryPayloads)));

            this.CombinatorialEngineProvider.RunCombinations(
                testPayloads,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent.Where(tc => !tc.IsRequest),
                (testPayload, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    string lastUpdatedTimeStr = null;
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var testMemoryStream = TestWriterUtils.CreateTestStream(testConfiguration, memoryStream, ignoreDispose: true))
                        {
                            bool feedWriter = testPayload.PayloadItems[0] is ODataFeed;
                            TestMessage testMessage = null;
                            Exception exception = TestExceptionUtils.RunCatching(() =>
                            {
                                using (var messageWriter = TestWriterUtils.CreateMessageWriter(testMemoryStream, testConfiguration, this.Assert, out testMessage, null, testPayload.Model))
                                {
                                    ODataWriter writer = messageWriter.CreateODataWriter(feedWriter);
                                    TestWriterUtils.WritePayload(messageWriter, writer, true, testPayload.PayloadItems, testPayload.ThrowUserExceptionAt);
                                }
                            });
                            this.Assert.IsNull(exception, "Received exception but expected none.");
                        }

                        memoryStream.Position = 0;
                        XElement result = XElement.Load(memoryStream);
                        foreach (XElement updated in result.Descendants(((XNamespace)TestAtomConstants.AtomNamespace) + "updated"))
                        {
                            if (updated.Value != ObjectModelUtils.DefaultFeedUpdated && updated.Value != ObjectModelUtils.DefaultEntryUpdated)
                            {
                                if (lastUpdatedTimeStr == null)
                                {
                                    lastUpdatedTimeStr = updated.Value;
                                }
                                else
                                {
                                    this.Assert.AreEqual(lastUpdatedTimeStr, updated.Value, "<atom:updated> should contain the same value.");
                                }
                            }
                        }
                    }
                });
        }
    }
}
