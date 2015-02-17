//---------------------------------------------------------------------
// <copyright file="AtomFeedMetadataTests.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests to verify writer correctly works with ATOM feed metadata
    /// </summary>
    [TestClass, TestCase]
    public class AtomFeedMetadataTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Tests that feed metadata is correctly written.")]
        public void FeedMetadataWriterTest()
        {
            AtomTextConstruct testRights = new AtomTextConstruct { Text = "Copyright Data Fx team." };
            AtomTextConstruct testTitle = new AtomTextConstruct { Text = "Test title" };
            AtomTextConstruct testSubtitle = new AtomTextConstruct { Text = "Test subtitle" };
            const string testUpdated = "2010-11-01T00:04:00Z";
            const string testIcon = "http://odata.org/icon";
            const string testLogo = "http://odata.org/logo";

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

            var selfLink = new AtomLinkMetadata()
            {
                Relation = TestAtomConstants.AtomSelfRelationAttributeValue,
                Title = testLinkTitle,
                Href = new Uri(testLinkHref),
                HrefLang = testLinkHrefLang,
                Length = testLinkLength,
                MediaType = testLinkMediaType
            };

            Func<string, Func<XElement, XElement>> fragmentExtractor = (localName) => (e) => e.Element(TestAtomConstants.AtomXNamespace + localName);

            // TODO, ckerer: specify an Id via metadata if the entry does not specify one; we first have to decide what rules
            //               we want to apply to merging of metadata and ODataLib OM data.
            var testCases = new FeedMetadataTestCase[] {
                new FeedMetadataTestCase { // specify an icon via metadata
                    CustomizeMetadata = metadata => metadata.Icon = new Uri(testIcon),
                    Xml = @"<icon xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testIcon + @"</icon>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomIconElementName)
                },
                new FeedMetadataTestCase { // specify a logo via metadata
                    CustomizeMetadata = metadata => metadata.Logo = new Uri(testLogo),
                    Xml = @"<logo xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testLogo + @"</logo>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomLogoElementName)
                },
                new FeedMetadataTestCase { // specify rights via metadata
                    CustomizeMetadata = metadata => metadata.Rights = testRights,
                    Xml = @"<rights type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testRights.Text + @"</rights>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomRightsElementName)
                },
                new FeedMetadataTestCase { // specify a subtitle via metadata
                    CustomizeMetadata = metadata => metadata.Subtitle = testSubtitle,
                    Xml = @"<subtitle type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testSubtitle.Text + @"</subtitle>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomSubtitleElementName)
                },
                new FeedMetadataTestCase { // specify a title via metadata
                    CustomizeMetadata = metadata => metadata.Title = testTitle,
                    Xml = @"<title type=""text"" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testTitle.Text + @"</title>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomTitleElementName)
                },
                new FeedMetadataTestCase { // specify an updated date via metadata
                    CustomizeMetadata = metadata => metadata.Updated = DateTimeOffset.Parse(testUpdated),
                    Xml = @"<updated xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testUpdated + @"</updated>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomUpdatedElementName)
                },
                new FeedMetadataTestCase { // no author specified, the default author with empty name is written
                    CustomizeMetadata = metadata => metadata.Authors = null,
                    Xml = string.Join(
                        "$(NL)",
                        @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <name />",
                        @"</author>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomAuthorElementName)
                },
                new FeedMetadataTestCase { // specify an author via metadata
                    CustomizeMetadata = metadata => metadata.Authors = testAuthors,
                    Xml = string.Join(
                        "$(NL)",
                        @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <name>" + testAuthorName + @"</name>",
                        @"  <uri>" + testAuthorUri + @"</uri>",
                        @"  <email>" + testAuthorEmail + @"</email>",
                        @"</author>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomAuthorElementName)
                },
                new FeedMetadataTestCase { // no author specified but some entries written, no author should be written
                    CustomizeMetadata = metadata => metadata.Authors = null,
                    CustomizePayload = feed => new ODataItem[] { feed, ObjectModelUtils.CreateDefaultEntry() },
                    Xml = "<authors />",
                    Extractor = result => new XElement("authors", fragmentExtractor(TestAtomConstants.AtomAuthorElementName)(result))
                },
                new FeedMetadataTestCase { // specify an author via metadata and some entries (the author should be written anyway)
                    CustomizeMetadata = metadata => metadata.Authors = testAuthors,
                    CustomizePayload = feed => new ODataItem[] { feed, ObjectModelUtils.CreateDefaultEntry() },
                    Xml = string.Join(
                        "$(NL)",
                        @"<author xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <name>" + testAuthorName + @"</name>",
                        @"  <uri>" + testAuthorUri + @"</uri>",
                        @"  <email>" + testAuthorEmail + @"</email>",
                        @"</author>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomAuthorElementName)
                },
                new FeedMetadataTestCase { // specify a category via metadata
                    CustomizeMetadata = metadata => metadata.Categories = testCategories,
                    Xml = @"<category term=""" + testCategoryTerm + @""" scheme=""" + testCategoryScheme + @""" label=""" + testCategoryLabel + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomCategoryElementName)
                },
                new FeedMetadataTestCase { // specify a contributor via metadata
                    CustomizeMetadata = metadata => metadata.Contributors = testContributors,
                    Xml = string.Join(
                        "$(NL)",
                        @"<contributor xmlns=""" + TestAtomConstants.AtomNamespace + @""">",
                        @"  <name>" + testContributorName + @"</name>",
                        @"  <uri>" + testContributorUri + @"</uri>",
                        @"  <email>" + testContributorEmail + @"</email>",
                        @"</contributor>"),
                    Extractor = fragmentExtractor(TestAtomConstants.AtomContributorElementName)
                },
                new FeedMetadataTestCase { // specify a generator via metadata
                    CustomizeMetadata = metadata => metadata.Generator = testGenerator,
                    Xml = @"<generator uri=""" + testGeneratorUri + @""" version=""" +testGeneratorVersion + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""">" + testGeneratorName + @"</generator>",
                    Extractor = fragmentExtractor(TestAtomConstants.AtomGeneratorElementName)
                },
                new FeedMetadataTestCase { // specify a link via metadata
                    CustomizeMetadata = metadata => metadata.Links = testLinks,
                    Xml = @"<link rel=""" + testLinkRelation + @""" type = """ + testLinkMediaType + @""" title=""" + testLinkTitle + @""" href=""" + testLinkHref + @""" hreflang=""" + testLinkHrefLang + @""" length=""" + testLinkLength + @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .Single()
                },
                new FeedMetadataTestCase { // no self link specified
                    CustomizeMetadata = metadata => metadata.SelfLink = null,
                    Xml = @"<selflink/>",
                    Extractor = (e) => new XElement("selflink", 
                        e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                         .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomSelfRelationAttributeValue))
                },
                new FeedMetadataTestCase { // Some self link specified
                    CustomizeMetadata = metadata => metadata.SelfLink = selfLink,
                    Xml = @"<link rel=""" + TestAtomConstants.AtomSelfRelationAttributeValue + @""" type = """ + testLinkMediaType + @""" title=""" + testLinkTitle + @""" href=""" + testLinkHref + @""" hreflang=""" + testLinkHrefLang + @""" length=""" + testLinkLength + @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomSelfRelationAttributeValue)
                },
                new FeedMetadataTestCase { // Non-self relation
                    CustomizeMetadata = metadata => metadata.SelfLink = new AtomLinkMetadata { Relation = "SelF", Href = new Uri(testLinkHref) },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkRelationsMustMatch", "self", "SelF"),
                },
                new FeedMetadataTestCase { // no next link on either OM or metadata specified
                    CustomizeMetadata = metadata => metadata.NextPageLink = null,
                    Xml = @"<nextlink/>",
                    Extractor = (e) => new XElement("nextlink", 
                        e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                         .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomNextRelationAttributeValue)),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // the next link is only specified on metadata - nothing is written
                    CustomizeMetadata = metadata => metadata.NextPageLink = new AtomLinkMetadata { Relation = "next", Href = new Uri(testLinkHref) },
                    Xml = @"<nextlink/>",
                    Extractor = (e) => new XElement("nextlink", 
                        e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                         .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomNextRelationAttributeValue)),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Next link specified only on the OM
                    CustomizeMetadata = metadata => metadata.NextPageLink = null,
                    CustomizePayload = feed => { feed.NextPageLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    Xml = @"<link rel=""" + TestAtomConstants.AtomNextRelationAttributeValue + @""" href=""" + testLinkHref + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomNextRelationAttributeValue),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Next link specified both on OM and on metadata - no conflicts
                    CustomizeMetadata = metadata => metadata.NextPageLink = new AtomLinkMetadata { Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType },
                    CustomizePayload = feed => { feed.NextPageLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    Xml = @"<link rel=""" + TestAtomConstants.AtomNextRelationAttributeValue + @""" type = """ + testLinkMediaType + @""" title=""" + testLinkTitle + @""" href=""" + testLinkHref + @""" hreflang=""" + testLinkHrefLang + @""" length=""" + testLinkLength + @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomNextRelationAttributeValue),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Next link specified both on OM and on metadata - no conflicts cause values are identical
                    CustomizeMetadata = metadata => metadata.NextPageLink = new AtomLinkMetadata { Relation = TestAtomConstants.AtomNextRelationAttributeValue, Href = new Uri(testLinkHref), Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType },
                    CustomizePayload = feed => { feed.NextPageLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    Xml = @"<link rel=""" + TestAtomConstants.AtomNextRelationAttributeValue + @""" type = """ + testLinkMediaType + @""" title=""" + testLinkTitle + @""" href=""" + testLinkHref + @""" hreflang=""" + testLinkHrefLang + @""" length=""" + testLinkLength + @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomNextRelationAttributeValue),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Next link specified both on OM and on metadata - conflict on relation
                    CustomizeMetadata = metadata => metadata.NextPageLink = new AtomLinkMetadata { Relation = "Next", Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType },
                    CustomizePayload = feed => { feed.NextPageLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkRelationsMustMatch", "next", "Next"),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Next link specified both on OM and on metadata - conflict on href
                    CustomizeMetadata = metadata => metadata.NextPageLink = new AtomLinkMetadata { Href = new Uri("http://odata.org/different"), Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType },
                    CustomizePayload = feed => { feed.NextPageLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkHrefsMustMatch", testLinkHref, "http://odata.org/different"),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
            };

            this.CreateTestDescriptorsAndRunTests(testCases, WriterPayloads.FeedPayloads);
        }

        [TestMethod, Variation(Description = "Tests that delta link in feed metadata is correctly written.")]
        public void DeltaLinkInFeedMetadataWriterTest()
        {
            const string testLinkTitle = "Test link 1";
            const string testLinkHref = "http://odata.org/links/1";
            const string testLinkHrefLang = "de-AT";
            int? testLinkLength = 999;
            const string testLinkMediaType = "image/png";

            var testCases = new FeedMetadataTestCase[] {
                new FeedMetadataTestCase { // Delta link specified both on OM and on metadata for a request.
                    CustomizeMetadata = metadata => metadata.Links = new AtomLinkMetadata[]{ new AtomLinkMetadata { Relation = TestAtomConstants.AtomDeltaRelationAttributeValue, Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType }},
                    CustomizePayload = feed => { feed.DeltaLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    Xml = @"<deltalink/>",
                    Extractor = (e) => new XElement("deltalink", 
                        e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                         .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomDeltaRelationAttributeValue)),
                    SkipTestConfiguration = tc => !tc.IsRequest
                },
                new FeedMetadataTestCase { // no delta link on either OM or metadata specified
                    CustomizeMetadata = metadata => metadata.Links = new AtomLinkMetadata[]{},
                    Xml = @"<deltalink/>",
                    Extractor = (e) => new XElement("deltalink", 
                        e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                         .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomDeltaRelationAttributeValue)),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // the delta link is only specified on metadata - nothing is written
                    CustomizeMetadata = metadata => metadata.Links = new AtomLinkMetadata[]{ new AtomLinkMetadata{ Relation = TestAtomConstants.AtomDeltaRelationAttributeValue, Href = new Uri(testLinkHref), Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType} },
                    Xml = @"<deltalink/>",
                    Extractor = (e) => new XElement("deltalink", 
                        e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                         .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomDeltaRelationAttributeValue)),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Delta link specified only on the OM
                    CustomizeMetadata = metadata => metadata.Links = new AtomLinkMetadata[]{},
                    CustomizePayload = feed => { feed.DeltaLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    Xml = @"<link rel=""" + TestAtomConstants.AtomDeltaRelationAttributeValue + @""" href=""" + testLinkHref + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomDeltaRelationAttributeValue),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Delta link specified both on OM and on metadata - no conflicts
                    CustomizeMetadata = metadata => metadata.Links = new AtomLinkMetadata[]{ new AtomLinkMetadata { Relation = TestAtomConstants.AtomDeltaRelationAttributeValue, Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType }},
                    CustomizePayload = feed => { feed.DeltaLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    Xml = @"<link rel=""" + TestAtomConstants.AtomDeltaRelationAttributeValue + @""" type = """ + testLinkMediaType + @""" title=""" + testLinkTitle + @""" href=""" + testLinkHref + @""" hreflang=""" + testLinkHrefLang + @""" length=""" + testLinkLength + @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomDeltaRelationAttributeValue),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Delta link specified both on OM and on metadata - no conflicts cause values are identical
                    CustomizeMetadata = metadata => metadata.Links = new AtomLinkMetadata[]{ new AtomLinkMetadata { Relation = TestAtomConstants.AtomDeltaRelationAttributeValue, Href = new Uri(testLinkHref), Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType }},
                    CustomizePayload = feed => { feed.DeltaLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    Xml = @"<link rel=""" + TestAtomConstants.AtomDeltaRelationAttributeValue + @""" type = """ + testLinkMediaType + @""" title=""" + testLinkTitle + @""" href=""" + testLinkHref + @""" hreflang=""" + testLinkHrefLang + @""" length=""" + testLinkLength + @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @"""/>",
                    Extractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                                        .SingleOrDefault(l => (string)l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName) == TestAtomConstants.AtomDeltaRelationAttributeValue),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new FeedMetadataTestCase { // Delta link specified both on OM and on metadata - conflict on href
                    CustomizeMetadata = metadata => metadata.Links = new AtomLinkMetadata[]{ new AtomLinkMetadata { Relation = TestAtomConstants.AtomDeltaRelationAttributeValue, Href = new Uri("http://odata.org/different"), Title = testLinkTitle, HrefLang = testLinkHrefLang, Length = testLinkLength, MediaType = testLinkMediaType }},
                    CustomizePayload = feed => { feed.DeltaLink = new Uri(testLinkHref); return new ODataItem[] { feed }; },
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkHrefsMustMatch", testLinkHref, "http://odata.org/different"),
                    SkipTestConfiguration = tc => tc.IsRequest
                }
             };

            this.CreateTestDescriptorsAndRunTests(testCases, WriterPayloads.TopLevelValuePayload);
        }

        private void CreateTestDescriptorsAndRunTests(FeedMetadataTestCase[] testCases, Func<PayloadWriterTestDescriptor<ODataItem>, IEnumerable<PayloadWriterTestDescriptor<ODataItem>>> payloads)
        {
            // Convert test cases to test descriptions
            IEnumerable<Func<ODataFeed>> feedCreators = new Func<ODataFeed>[] {() => ObjectModelUtils.CreateDefaultFeed(), () => ObjectModelUtils.CreateDefaultFeedWithAtomMetadata(),};
            var testDescriptors = testCases.SelectMany(testCase => feedCreators.Select(feedCreator =>
            {
                ODataFeed feed = feedCreator();
                AtomFeedMetadata metadata = feed.Atom();
                this.Assert.IsNotNull(metadata, "Expected default feed metadata on a default feed.");
                testCase.CustomizeMetadata(metadata);
                ODataItem[] payloadItems = testCase.CustomizePayload == null ? new ODataItem[] {feed} : testCase.CustomizePayload(feed);
                return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, payloadItems, testConfiguration =>
                {
                    if (testCase.SkipTestConfiguration != null && testCase.SkipTestConfiguration(testConfiguration)) return null;
                    return new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) {Xml = testCase.Xml, FragmentExtractor = testCase.Extractor, ExpectedException2 = testCase.ExpectedException};
                });
            }));

            this.CombinatorialEngineProvider.RunCombinations(testDescriptors.PayloadCases(WriterPayloads.TopLevelValuePayload), this.WriterTestConfigurationProvider.AtomFormatConfigurations, (testDescriptor, testConfiguration) =>
            {
                testConfiguration = testConfiguration.Clone();
                testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
            });
        }

        private sealed class FeedMetadataTestCase
        {
            public Action<AtomFeedMetadata> CustomizeMetadata { get; set; }
            public Func<ODataFeed, ODataItem[]> CustomizePayload { get; set; }
            public string Xml { get; set; }
            public Func<XElement, XElement> Extractor { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public Func<WriterTestConfiguration, bool> SkipTestConfiguration { get; set; }
        }
    }
}
