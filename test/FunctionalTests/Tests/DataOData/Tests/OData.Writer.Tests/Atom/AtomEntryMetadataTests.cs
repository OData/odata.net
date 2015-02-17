//---------------------------------------------------------------------
// <copyright file="AtomEntryMetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests to verify writer correctly works with ATOM entry metadata
    /// </summary>
    [TestClass, TestCase]
    public class AtomEntryMetadataTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Tests that entry metadata is correctly written.")]
        public void EntryMetadataWriterTest()
        {
            const string testPublished = "2010-10-20T20:10:00Z";
            AtomTextConstruct testRights = new AtomTextConstruct { Text = "Copyright Data Fx team." };
            AtomTextConstruct testSummary = new AtomTextConstruct { Text = "Test summary." };
            AtomTextConstruct testTitle = new AtomTextConstruct { Text = "Test title" };
            const string testUpdated = "2010-11-01T00:04:00Z";
            const string testIcon = "http://odata.org/icon";
            const string testSourceId = "http://odata.org/id/random";
            const string testLogo = "http://odata.org/logo";
            AtomTextConstruct testSubtitle = new AtomTextConstruct { Text = "Test subtitle." };

            const string testAuthorName = "Test Author 1";
            const string testAuthorEmail = "author1@odata.org";
            const string testAuthorUri = "http://odata.org/authors/1";

            var testAuthors = new AtomPersonMetadata[]
                {
                    new AtomPersonMetadata()
                    {
                        Email = testAuthorEmail,
                        Name = testAuthorName,
                        Uri = new Uri(testAuthorUri)
                    }
                };

            var testAuthors2 = new AtomPersonMetadata[0];

            const string testCategoryTerm = "Test category 1 term";
            const string testCategoryLabel = "Test category 1 label";
            const string testCategoryScheme = "http://odata.org/categories/1";

            var testCategories = new AtomCategoryMetadata[]
                {
                    new AtomCategoryMetadata()
                    {
                        Term = testCategoryTerm,
                        Label = testCategoryLabel,
                        Scheme = testCategoryScheme
                    }
                };

            const string testContributorName = "Test Contributor 1";
            const string testContributorEmail = "contributor1@odata.org";
            const string testContributorUri = "http://odata.org/contributors/1";

            var testContributors = new AtomPersonMetadata[]
                {
                    new AtomPersonMetadata()
                    {
                        Email = testContributorEmail,
                        Name = testContributorName,
                        Uri = new Uri(testContributorUri)
                    }
                };

            const string testGeneratorName = "Test generator";
            const string testGeneratorUri = "http://odata.org/generator";
            const string testGeneratorVersion = "3.0";

            var testGenerator = new AtomGeneratorMetadata()
                {
                    Name = testGeneratorName,
                    Uri = new Uri(testGeneratorUri),
                    Version = testGeneratorVersion
                };

            const string testLinkRelation = "http://odata.org/links/1";
            const string testLinkTitle = "Test link 1";
            const string testLinkHref = "http://odata.org/links/1";
            const string testLinkHrefLang = "de-AT";
            int? testLinkLength = 999;
            const string testLinkMediaType = "image/png";

            var testLinks = new AtomLinkMetadata[] 
                {
                    new AtomLinkMetadata()
                    {
                        Relation = testLinkRelation,
                        Title = testLinkTitle,
                        Href = new Uri(testLinkHref),
                        HrefLang = testLinkHrefLang,
                        Length = testLinkLength,
                        MediaType = testLinkMediaType
                    }
                };

            AtomFeedMetadata testSource = new AtomFeedMetadata()
            {
                Authors = testAuthors,
                Categories = testCategories,
                Contributors = testContributors,
                Generator = testGenerator,
                Icon = new Uri(testIcon),
                SourceId = new Uri(testSourceId),
                Links = testLinks,
                Logo = new Uri(testLogo),
                Rights = testRights,
                Subtitle = testSubtitle,
                Title = testTitle,
                Updated = DateTimeOffset.Parse(testUpdated)
            };

            Func<string, Func<XElement, XElement>> fragmentExtractor = (localName) => (e) => e.Element(TestAtomConstants.AtomXNamespace + localName);

            // TODO, ckerer: specify an Id via metadata if the entry does not specify one; we first have to decide what rules
            //               we want to apply to merging of metadata and ODataLib OM data.
            var testCases = new[] {
                new { // specify an author via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Authors = testAuthors),
                    Xml = string.Join(
                        "$(NL)",
                        @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <name>" + testAuthorName + @"</name>",
                        @"  <uri>" + testAuthorUri + @"</uri>",
                        @"  <email>" + testAuthorEmail + @"</email>",
                        @"</author>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomAuthorElementName)
                },
                new { // specify an empty author array via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Authors = testAuthors2),
                    Xml = string.Join(
                        "$(NL)",
                        @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <name />",
                        @"</author>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomAuthorElementName)
                },
                new { // specify no authors via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Authors = null),
                    Xml = string.Join(
                        "$(NL)",
                        @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"$(Indent)<name />",
                        @"</author>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomAuthorElementName)
                },
                new { // specify a category via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Categories = testCategories),
                    Xml = @"<category term=""" + testCategoryTerm + @""" scheme=""" + testCategoryScheme + @""" label=""" + testCategoryLabel + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomCategoryElementName)
                },
                new { // specify a contributor via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Contributors = testContributors),
                    Xml = string.Join(
                        "$(NL)",
                        @"<contributor xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <name>" + testContributorName + @"</name>",
                        @"  <uri>" + testContributorUri + @"</uri>",
                        @"  <email>" + testContributorEmail + @"</email>",
                        @"</contributor>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomContributorElementName)
                },
                new { // specify a link via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Links = testLinks),
                    Xml = @"<link rel=""" + testLinkRelation + @""" type = """ + testLinkMediaType + @""" title=""" + testLinkTitle + @""" href=""" + testLinkHref + @""" hreflang=""" + testLinkHrefLang + @""" length=""" + testLinkLength + @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = new Func<XElement, XElement>(
                            (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                    .Where(l => l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName).Value != TestAtomConstants.AtomSelfRelationAttributeValue)
                                    .Single())
                },
                new { // specify a published date via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Published = DateTimeOffset.Parse(testPublished)),
                    Xml = @"<published xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testPublished + @"</published>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomPublishedElementName)
                },
                new { // specify rights via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Rights = testRights),
                    Xml = @"<rights type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testRights.Text + @"</rights>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomRightsElementName)
                },
                new { // specify a source via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Source = testSource),
                    Xml = string.Join(
                        "$(NL)",
                        @"<source xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <id>" + testSourceId + "</id>",
                        @"  <title type=""text"">" + testTitle.Text + @"</title>",
                        @"  <subtitle type=""text"">" + testSubtitle.Text + @"</subtitle>",
                        @"  <updated>" + testUpdated + @"</updated>",
                        @"  <link rel=""" + testLinkRelation + @""" type = """ + testLinkMediaType + @""" title=""" + testLinkTitle + @""" href=""" + testLinkHref + @""" hreflang=""" + testLinkHrefLang + @""" length=""" + testLinkLength + @"""/>",
                        @"  <category term=""" + testCategoryTerm + @""" scheme=""" + testCategoryScheme + @""" label=""" + testCategoryLabel + @""" />",
                        @"  <logo>" + testLogo + @"</logo>",
                        @"  <rights type=""text"">" + testRights.Text + @"</rights>",
                        @"  <contributor>",
                        @"    <name>" + testContributorName + @"</name>",
                        @"    <uri>" + testContributorUri + @"</uri>",
                        @"    <email>" + testContributorEmail + @"</email>",
                        @"  </contributor>",
                        @"  <generator uri=""" + testGeneratorUri + @""" version=""" +testGeneratorVersion + @""">" + testGeneratorName + @"</generator>",
                        @"  <icon>" + testIcon + @"</icon>",
                        @"  <author>",
                        @"    <name>" + testAuthorName + @"</name>",
                        @"    <uri>" + testAuthorUri + @"</uri>",
                        @"    <email>" + testAuthorEmail + @"</email>",
                        @"  </author>",
                        @"</source>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomSourceElementName)
                },
                new { // specify default feed metadata as source
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Source = new AtomFeedMetadata()),
                    Xml = string.Join(
                        "$(NL)",
                        @"<source xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <id />",
                        @"  <title />",
                        @"  <updated />",
                        @"</source>"),
                    Extractor = new Func<XElement, XElement>(result => {
                        var source = fragmentExtractor(TestAtomConstants.AtomSourceElementName)(result);
                        // Remove the value of updates as it can't be reliably predicted
                        source.Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomUpdatedElementName).Nodes().Remove();
                        return source;
                    })
                },
                new { // specify a summary via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Summary = testSummary),
                    Xml = @"<summary type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testSummary.Text + @"</summary>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomSummaryElementName)
                },
                new { // specify a title via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Title = testTitle),
                    Xml = @"<title type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testTitle.Text + @"</title>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomTitleElementName)
                },
                new { // specify an updated date via metadata
                    CustomizeMetadata = new Action<AtomEntryMetadata>(metadata => metadata.Updated = DateTimeOffset.Parse(testUpdated)),
                    Xml = @"<updated xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testUpdated + @"</updated>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomUpdatedElementName)
                },
            };

            // Convert test cases to test descriptions
            IEnumerable<Func<ODataEntry>> entryCreators = new Func<ODataEntry>[]
                {
                    () => ObjectModelUtils.CreateDefaultEntry(),
                    () => ObjectModelUtils.CreateDefaultEntryWithAtomMetadata(),
                };
            var testDescriptors = testCases.SelectMany(testCase =>
                entryCreators.Select(entryCreator =>
                {
                    ODataEntry entry = entryCreator();
                    AtomEntryMetadata metadata = entry.Atom();
                    this.Assert.IsNotNull(metadata, "Expected default entry metadata on a default entry.");
                    testCase.CustomizeMetadata(metadata);
                    return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                        new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Xml = testCase.Xml, FragmentExtractor = testCase.Extractor });
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }
    }
}
