//---------------------------------------------------------------------
// <copyright file="AtomLinkMetadataTests.cs" company="Microsoft">
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
    /// Tests to verify writer correctly works with ATOM link metadata
    /// </summary>
    [TestClass, TestCase]
    public class AtomLinkMetadataTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        private const string readLinkHref = "http://www.odata.org/entry/readlink";
        private const string editLinkHref = "http://www.odata.org/entry/editlink";
        private const string linkTitle = "Stream";
        private const string linkIncorrectTitle = "IncorrectLinkTitle";
        private const string linkHrefLang = "de-AT";
        private const string linkMediaType = "application/xml";
        private const string linkIncorrectMediaType = "application/incorrect";
        private const string linkIncorrectRelation = "http://odata.org/relation/1";
        private const string linkIncorrectHref = "http://odata.org/links/1";
        private static readonly int? linkLength = 999;

        private static readonly IEnumerable<LinkMetadataTestCase> linkMetadataTestCases = new LinkMetadataTestCase[]
        {
            // empty link metadata
            new LinkMetadataTestCase
            {
                LinkMetadata = (rel, href) => new AtomLinkMetadata(),
                ExpectedXml = (rel, href, title, type) => 
                    @"<link rel=""" + rel + 
                    (type == null ? string.Empty : @""" type=""" + type) + 
                    (title == null ? string.Empty : @""" title=""" + title) + 
                    @""" href=""" + href + 
                    @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                ExpectedException = null
            },

            // link metadata with incorrect href
            new LinkMetadataTestCase
            {
                LinkMetadata = (rel, href) => new AtomLinkMetadata()
                {
                    Href = new Uri(linkIncorrectHref),
                },
                ExpectedXml = null,
                ExpectedException = (rel, href) => ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkHrefsMustMatch", href, "http://odata.org/links/1")
            },

            // link metadata with incorrect relation
            new LinkMetadataTestCase
            {
                LinkMetadata = (rel, href) => new AtomLinkMetadata()
                {
                    Relation = linkIncorrectRelation,
                },
                ExpectedXml = null,
                ExpectedException = (rel, href) => ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkRelationsMustMatch", rel, linkIncorrectRelation)
            },

            // link metadata with matching href and relation
            new LinkMetadataTestCase
            {
                LinkMetadata = (rel, href) => new AtomLinkMetadata()
                {
                    Relation = rel,
                    Title = null,
                    Href = new Uri(href),
                    HrefLang = null,
                    Length = null,
                    MediaType = null
                },
                ExpectedXml = (rel, href, title, type) => 
                    @"<link rel=""" + rel + 
                    (type == null ? string.Empty : @""" type=""" + type) + 
                    (title == null ? string.Empty : @""" title=""" + title) + 
                    @""" href=""" + href + 
                    @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                ExpectedException = null
            },

            // link metadata with custom title, type, hreflang, length
            new LinkMetadataTestCase
            {
                LinkMetadata = (rel, href) => new AtomLinkMetadata()
                {
                    Relation = rel,
                    Title = linkTitle,
                    Href = new Uri(href),
                    HrefLang = linkHrefLang,
                    Length = linkLength,
                    MediaType = linkMediaType
                },
                ExpectedXml = (rel, href, title, type) => 
                    @"<link rel=""" + rel + 
                    @""" type=""" + linkMediaType + 
                    @""" title=""" + linkTitle + 
                    @""" href=""" + href + 
                    @""" hreflang=""" + linkHrefLang + 
                    @""" length=""" + linkLength + 
                    @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                ExpectedException = null
            },
        };

        // link metadata with incorrect title
        private static readonly LinkMetadataTestCase incorrectTitleLinkMetadataTestCases = new LinkMetadataTestCase
        {
            LinkMetadata = (rel, href) => new AtomLinkMetadata()
            {
                Title = linkIncorrectTitle,
            },
            ExpectedXml = null,
            ExpectedException = (rel, href) => ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkTitlesMustMatch", linkTitle, linkIncorrectTitle)
        };

        // link metadata with incorrect media type
        private static readonly LinkMetadataTestCase incorrectMediaTypeLinkMetadataTestCases = new LinkMetadataTestCase
        {
            LinkMetadata = (rel, href) => new AtomLinkMetadata()
            {
                MediaType = linkIncorrectMediaType,
            },
            ExpectedXml = null,
            ExpectedException = (rel, href) => ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkMediaTypesMustMatch", linkMediaType, linkIncorrectMediaType)
        };

        [TestMethod, Variation(Description = "Tests that navigation link metadata is correctly written.")]
        public void NavigationLinkMetadataWriterTest()
        {
            string testRelation = "http://docs.oasis-open.org/odata/ns/related/SampleLinkName";
            string testIncorrectRelation = "http://odata.org/relation/1";
            string testTitle = "SampleLinkName";
            string testIncorrectTitle = "Test link 1";
            string testHref = "http://odata.org/link";
            string testIncorrectHref = "http://odata.org/links/1";
            string testHrefLang = "de-AT";
            int? testLength = 999;
            string testMediaType = "application/atom+xml;type=feed";
            string testIncorrectMediaType = "image/png";

            var testCases = new[]
                {
                    new 
                    {
                        CustomizeLink = new Action<ODataNavigationLink>(
                            (link) =>
                                {
                                    AtomLinkMetadata metadata = link.Atom();
                                    metadata.Relation = testRelation;
                                    metadata.Title = testTitle;
                                    metadata.Href = new Uri(testHref);
                                    metadata.HrefLang = testHrefLang;
                                    metadata.Length = testLength;
                                    metadata.MediaType = testMediaType;
                                    link.Url = metadata.Href;
                                }),
                        Xml = @"<link rel=""" + testRelation + @""" type = """ + testMediaType + @""" title=""" + testTitle + @""" href=""" + testHref + @""" hreflang=""" + testHrefLang + @""" length=""" + testLength.Value + @"""  xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                        ExpectedException = (ExpectedException)null
                    },
                    new 
                    {
                        CustomizeLink = new Action<ODataNavigationLink>(
                            (link) =>
                                {
                                    AtomLinkMetadata metadata = link.Atom();
                                    metadata.Relation = testRelation;
                                    metadata.Title = testTitle;
                                    metadata.Href = new Uri(testIncorrectHref);
                                    metadata.HrefLang = testHrefLang;
                                    metadata.Length = testLength;
                                    metadata.MediaType = testMediaType;
                                }),
                        Xml = (string)null,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkHrefsMustMatch", "http://odata.org/link", "http://odata.org/links/1"),
                    },
                    new 
                    {
                        CustomizeLink = new Action<ODataNavigationLink>(
                            (link) =>
                                {
                                    AtomLinkMetadata metadata = link.Atom();
                                    metadata.Relation = testIncorrectRelation;
                                    metadata.Title = testTitle;
                                    metadata.Href = new Uri(testHref);
                                    metadata.HrefLang = testHrefLang;
                                    metadata.Length = testLength;
                                    metadata.MediaType = testMediaType;
                                }),
                        Xml = (string)null,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkRelationsMustMatch", "http://docs.oasis-open.org/odata/ns/related/SampleLinkName", "http://odata.org/relation/1"),
                    },
                    new 
                    {
                        CustomizeLink = new Action<ODataNavigationLink>(
                            (link) =>
                                {
                                    AtomLinkMetadata metadata = link.Atom();
                                    metadata.Relation = testRelation;
                                    metadata.Title = testTitle;
                                    metadata.Href = new Uri(testHref);
                                    metadata.HrefLang = testHrefLang;
                                    metadata.Length = testLength;
                                    metadata.MediaType = testIncorrectMediaType;
                                }),
                        Xml = (string)null,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkMediaTypesMustMatch", "application/atom+xml;type=feed", "image/png"),
                    },
                    new 
                    {
                        CustomizeLink = new Action<ODataNavigationLink>(
                            (link) =>
                                {
                                    AtomLinkMetadata metadata = link.Atom();
                                    metadata.Relation = testRelation;
                                    metadata.Title = testIncorrectTitle;
                                    metadata.Href = new Uri(testHref);
                                    metadata.HrefLang = testHrefLang;
                                    metadata.Length = testLength;
                                    metadata.MediaType = testMediaType;
                                }),
                        Xml = (string)null,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomWriterMetadataUtils_LinkTitlesMustMatch", "SampleLinkName", "Test link 1"),
                    },
                    new 
                    {
                        CustomizeLink = new Action<ODataNavigationLink>(
                            (link) =>
                                {
                                    AtomLinkMetadata metadata = link.Atom();
                                    metadata.Relation = testRelation;
                                    metadata.Title = null;
                                    metadata.Href = new Uri(testHref);
                                    metadata.HrefLang = null;
                                    metadata.Length = null;
                                    metadata.MediaType = null;
                                }),
                        Xml = @"<link rel=""" + testRelation + @""" type = """ + testMediaType + @""" title=""" + testTitle + @""" href=""" + testHref + @""" xmlns=""" + TestAtomConstants.AtomNamespace + @""" />",
                        ExpectedException = (ExpectedException)null
                    },
                };

            Func<XElement, XElement> fragmentExtractor = (e) => e;

            // Convert test cases to test descriptions
            Func<ODataNavigationLink>[] linkCreators = new Func<ODataNavigationLink>[]
            {
                () => ObjectModelUtils.CreateDefaultCollectionLink(),
                () => { var link = ObjectModelUtils.CreateDefaultCollectionLink(); link.SetAnnotation(new AtomLinkMetadata()); return link; }
            };
            var testDescriptors = testCases.SelectMany(testCase =>
                linkCreators.Select(linkCreator =>
                {
                    ODataNavigationLink link = linkCreator();
                    testCase.CustomizeLink(link);
                    return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, link, testConfiguration =>
                        new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Xml = testCase.Xml, ExpectedException2 = testCase.ExpectedException, FragmentExtractor = fragmentExtractor });
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.NavigationLinkPayloads),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration), testConfiguration, this.Assert, this.Logger);
                });
        }

        // TODO: Association Link - Add back support for customizing association link element in Atom

        [TestMethod, Variation(Description = "Tests that self and edit links on entries with link metadata are written correctly.")]
        public void EntryReadAndEditLinkMetadataWriterTest()
        {
            Func<XElement, XElement> fragmentExtractor = (e) => e.Element(TestAtomConstants.AtomXNamespace + "link");

            // Convert test cases to test descriptions; first for the entry's self link
            var selfLinkTestDescriptors = linkMetadataTestCases.Select(testCase =>
            {
                ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                entry.Atom().SelfLink = testCase.LinkMetadata("self", readLinkHref);
                return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                    new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) 
                    { 
                        Xml = testCase.ExpectedXml == null ? null : testCase.ExpectedXml("self", readLinkHref, null, null), 
                        ExpectedException2 = testCase.ExpectedException == null ? null : testCase.ExpectedException("self", readLinkHref), 
                        FragmentExtractor = fragmentExtractor 
                    });
            });

            // now the ones for the entry's edit link
            var editLinkTestDescriptors = linkMetadataTestCases.Select(testCase =>
            {
                ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                entry.ReadLink = null;
                entry.EditLink = new Uri(editLinkHref);
                entry.Atom().EditLink = testCase.LinkMetadata("edit", editLinkHref);
                return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                    new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Xml = testCase.ExpectedXml == null ? null : testCase.ExpectedXml("edit", editLinkHref, null, null),
                        ExpectedException2 = testCase.ExpectedException == null ? null : testCase.ExpectedException("edit", editLinkHref),
                        FragmentExtractor = fragmentExtractor
                    });
            });

            var testDescriptors = selfLinkTestDescriptors.Concat(editLinkTestDescriptors);

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

        [TestMethod, Variation(Description = "Tests that the edit link of a default stream with link metadata is written correctly.")]
        public void DefaultStreamEditLinkMetadataWriterTest()
        {
            Func<XElement, XElement> fragmentExtractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + "link").Last();

            // NOTE: no self-link test cases since the self link is represented as the <content> element and not customizable through link metadata
            var testDescriptors = linkMetadataTestCases.Select(testCase =>
            {
                ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue() 
                { 
                    ReadLink = new Uri(readLinkHref),
                    EditLink = new Uri(editLinkHref), 
                    ContentType = linkMediaType,
                };

                AtomStreamReferenceMetadata streamReferenceMetadata = new AtomStreamReferenceMetadata()
                {
                    EditLink = testCase.LinkMetadata("edit-media", editLinkHref)
                };

                streamReferenceValue.SetAnnotation<AtomStreamReferenceMetadata>(streamReferenceMetadata);
                entry.MediaResource = streamReferenceValue;

                return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                    new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Xml = testCase.ExpectedXml == null ? null : testCase.ExpectedXml("edit-media", editLinkHref, null, null),
                        ExpectedException2 = testCase.ExpectedException == null ? null : testCase.ExpectedException("edit-media", editLinkHref),
                        FragmentExtractor = fragmentExtractor
                    });
            });

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

        [TestMethod, Variation(Description = "Tests that the read and edit links of a named stream with link metadata are written correctly.")]
        public void NamedStreamReadAndEditLinkMetadataWriterTest()
        {
            Func<XElement, XElement> fragmentExtractor = (e) => e.Elements(TestAtomConstants.AtomXNamespace + "link").Last();

            var allTestCases = linkMetadataTestCases.ConcatSingle(incorrectMediaTypeLinkMetadataTestCases).ConcatSingle(incorrectTitleLinkMetadataTestCases);

            var readLinkTestDescriptors = allTestCases.Select(testCase =>
            {
                ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue()
                {
                    ReadLink = new Uri(readLinkHref),
                    ContentType = linkMediaType,
                };

                AtomStreamReferenceMetadata streamReferenceMetadata = new AtomStreamReferenceMetadata()
                {
                    SelfLink = testCase.LinkMetadata("http://docs.oasis-open.org/odata/ns/mediaresource/Stream", readLinkHref)
                };

                streamReferenceValue.SetAnnotation<AtomStreamReferenceMetadata>(streamReferenceMetadata);
                entry.Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Stream", Value = streamReferenceValue }
                };

                return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                    new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Xml = testCase.ExpectedXml == null ? null : testCase.ExpectedXml("http://docs.oasis-open.org/odata/ns/mediaresource/Stream", readLinkHref, "Stream", linkMediaType),
                        ExpectedException2 = testCase.ExpectedException == null ? null : testCase.ExpectedException("http://docs.oasis-open.org/odata/ns/mediaresource/Stream", readLinkHref),
                        FragmentExtractor = fragmentExtractor
                    });
            });

            var editLinkTestDescriptors = allTestCases.Select(testCase =>
            {
                ODataEntry entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
                ODataStreamReferenceValue streamReferenceValue = new ODataStreamReferenceValue()
                {
                    ReadLink = new Uri(readLinkHref),
                    EditLink = new Uri(editLinkHref),
                    ContentType = linkMediaType,
                };

                AtomStreamReferenceMetadata streamReferenceMetadata = new AtomStreamReferenceMetadata()
                {
                    EditLink = testCase.LinkMetadata("http://docs.oasis-open.org/odata/ns/edit-media/Stream", editLinkHref)
                };

                streamReferenceValue.SetAnnotation<AtomStreamReferenceMetadata>(streamReferenceMetadata);
                entry.Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Stream", Value = streamReferenceValue }
                };

                return new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, testConfiguration =>
                    new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Xml = testCase.ExpectedXml == null ? null : testCase.ExpectedXml("http://docs.oasis-open.org/odata/ns/edit-media/Stream", editLinkHref, "Stream", linkMediaType),
                        ExpectedException2 = testCase.ExpectedException == null ? null : testCase.ExpectedException("http://docs.oasis-open.org/odata/ns/edit-media/Stream", editLinkHref),
                        FragmentExtractor = fragmentExtractor
                    });
            });

            var testDescriptors = readLinkTestDescriptors.Concat(editLinkTestDescriptors);

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.AtomFormatConfigurations
                    .Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        /// <summary>
        /// Private class to represent a link metadata case.
        /// </summary>
        private sealed class LinkMetadataTestCase
        {
            /// <summary>The function to create the <see cref="AtomLinkMetadata"/> instance used to customize the link given the relationa and the href of the link.</summary>
            public Func<string, string, AtomLinkMetadata> LinkMetadata { get; set; }

            /// <summary>The function to create the expected Xml representation of the link given the relation, href, title and media type of the link.</summary>
            public Func<string, string, string, string, string> ExpectedXml { get; set; }

            /// <summary>The function to create the expected error message given the relation and the href of the link.</summary>
            public Func<string, string, ExpectedException> ExpectedException { get; set; }
        }
    }
}
