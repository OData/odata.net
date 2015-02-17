//---------------------------------------------------------------------
// <copyright file="FeedReaderAtomMetadataTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of ATOM metadata in feeds.
    /// </summary>
    [TestClass, TestCase]
    public class FeedReaderAtomMetadataTests : ODataAtomMetadataReaderTestCase
    {
        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of person ATOM metadata (ie, 'author' and 'contributor') in feeds.")]
        public void FeedAtomMetadataPersonConstructTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreatePersonConstructFeedTestDescriptors(
                    "author", AtomMetadataBuilder.AtomAuthor);

            testDescriptors = testDescriptors.Concat(
                this.CreatePersonConstructFeedTestDescriptors(
                    "contributor", AtomMetadataBuilder.AtomContributor));

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of text construct ATOM metadata (ie, 'subtitle', 'title', and 'rights') in feeds.")]
        public void FeedAtomMetadataTextConstructTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreateTextConstructFeedTestDescriptors(
                    "rights", AtomMetadataBuilder.AtomRights);

            testDescriptors = testDescriptors.Concat(
                this.CreateTextConstructFeedTestDescriptors(
                    "subtitle", AtomMetadataBuilder.AtomSubtitle));

            testDescriptors = testDescriptors.Concat(
                this.CreateTextConstructFeedTestDescriptors(
                    "title", AtomMetadataBuilder.AtomTitle));

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of the updated element in feeds.")]
        public void FeedAtomMetadataUpdatedTest()
        {
            // new[]
            // {
            //     PayloadBuilder.Entity("DefaultNamespace.Customer")
            // }
            var entitySet = PayloadBuilder.EntitySet();

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreateDateConstructTestDescriptors(
                    entitySet,
                    "feed",
                    "updated",
                    AtomMetadataBuilder.AtomUpdated);

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of URI-valued ATOM metadata elements (ie, 'icon' and 'logo') in feeds.")]
        public void FeedAtomMetadataUriValueTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreateUriValueFeedTestDescriptors(
                    "icon", AtomMetadataBuilder.AtomIcon);

            testDescriptors = testDescriptors.Concat(
                this.CreateUriValueFeedTestDescriptors(
                    "logo", AtomMetadataBuilder.AtomLogo));

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of category elements as ATOM metadata in feeds.")]
        public void FeedAtomMetadataCategoryTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreateCategoryTestDescriptors(
                    PayloadBuilder.EntitySet(), "feed");

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of the atom:generator element in a feed.")]
        public void FeedAtomMetadataGeneratorTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Basic generator with name, uri, and version
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomGenerator("Test Generator", "http://odata.org/generator", "1.0")
                },
                // Empty generator (empty element)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomGenerator(null, null, null)
                        .XmlRepresentation("<feed><generator /></feed>")
                },
                // Empty generator (no children)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomGenerator(String.Empty, null, null)
                        .XmlRepresentation("<feed><generator></generator></feed>")
                },
                // Generator with extra (ignored) attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomGenerator("Test Generator", null, null)
                        .XmlRepresentation("<feed><generator extra='attribute'>Test Generator</generator></feed>")
                },
                // Generator with attributes in the wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomGenerator("Test Generator", null, null)
                        .XmlRepresentation("<feed><generator cn:uri='http://odata.org' cn:version='4.0' xmlns:cn='http://customnamespace.com'>Test Generator</generator></feed>")
                },
                // Feed with multiple generator elements should fail
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .XmlRepresentation(@"<feed>
                                                <generator uri='http://odata.org' version='1'>Some Name</generator>
                                                <generator uri='http://second.uri' version='2'>Another Name</generator>
                                             </feed>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomMetadataDeserializer_MultipleSingletonMetadataElements", "generator", "feed")
                },
            };

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of link elements as ATOM metadata in a feed.")]
        public void FeedAtomMetadataLinkTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region atom:link elements which do not convey OData-specific information
                // Basic non-OData link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomLink("http://odata.org", "someRel", null)
                },
                // Multiple non-odata links
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .AtomLink("http://first.link/", "firstRel", null)
                        .AtomLink("http://second.link/", "secondRel", null)
                },
                // Multiple identical non-odata links
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .AtomLink("http://first.link/", "firstRel", null)
                        .AtomLink("http://first.link/", "firstRel", null)
                },
                // Link with all possible ATOM attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomLink("http://odata.org", "someRel", "text/html", "fr", "Link Title", "42")
                },
                // Link without "rel" attribute (only href is required by ATOM spec)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomLink("http://odata.org", null, null)
                },
                // Link with empty values for the ATOM attributes (should be stored as empty string, not null)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    // Note: This test violates the ATOM spec in a few ways (some attributes require the value to have at least one character, along with other constraints),
                    // but we still should be able to read such values.
                    // Note: length is stored as an int? so they're isn't a way to differentiate between the attribute value being an empty string and the attribute not being specified at all, so we ignore it for this test case.
                    PayloadElement = PayloadBuilder.EntitySet().AtomLink("http://odata.org", String.Empty, String.Empty, String.Empty, String.Empty)
                        .XmlRepresentation(@"<feed>
                                                <link rel='' title='' type='' hreflang='' href='http://odata.org' />
                                             </feed>")
                },
                #endregion atom:link elements which do not convey OData-specific information

                #region self link
                // Self link with href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomSelfLink("http://odata.org", null)
                },
                // Self link with all possible ATOM attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomSelfLink("http://odata.org", "text/html", "fr", "Link Title", "1337")
                },
                // Self link without href (against ATOM spec, but we can still handle it)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomSelfLink(null, null)
                },
                // Self link with extra (ignored) attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomSelfLink("http://odata.org", "text/html")
                        .XmlRepresentation(@"<feed>
                                                <link rel='self' href='http://odata.org' other='ignored' type='text/html' something='else' />
                                             </feed>")
                },
                // Self link with attributes in wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomSelfLink("http://odata.org", null)
                        .XmlRepresentation(@"<feed>
                                                <link rel='self' href='http://odata.org' cn:type='text/html' 
                                                      cn:hreflang='en' cn:title='Some Title' cn:length='1000' 
                                                      xmlns:cn='http://customnamespace.com' />
                                             </feed>")
                },
                #endregion self link

                #region next link
                // Next link with href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomNextPageLink("http://odata.org", null)
                        .XmlRepresentation(@"<feed>
                                                <link rel='next' href='http://odata.org' />
                                             </feed>")
                },
                // Next link with all possible ATOM attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomNextPageLink("http://odata.org", "text/html", "fr", "Link Title", "1337")
                },
                // Next link without href (violation of the ATOM spec, but we can still handle it)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomNextPageLink(null, null)
                },
                // Next link with extra (ignored) attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomNextPageLink("http://odata.org", "text/html")
                        .XmlRepresentation(@"<feed>
                                                <link rel='next' href='http://odata.org' other='ignored' type='text/html' something='else' />
                                             </feed>")
                },
                // Next link with attributes in wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomNextPageLink("http://odata.org", null)
                        .XmlRepresentation(@"<feed>
                                                <link rel='next' href='http://odata.org' cn:type='text/html' 
                                                      cn:hreflang='en' cn:title='Some Title' cn:length='1000'
                                                      xmlns:cn='http://customnamespace.com' />
                                             </feed>")
                },
                #endregion next link

                #region delta link
                // Delta link with href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomDeltaLink("http://odata.org", null)
                        .XmlRepresentation(@"<feed>
                                                <link rel='http://docs.oasis-open.org/odata/ns/delta' href='http://odata.org' />
                                             </feed>")
                },
                // Delta link with all possible ATOM attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomDeltaLink("http://odata.org", "text/html", "fr", "Link Title", "1337")
                },
                // Delta link without href (violation of the ATOM spec, but we can still handle it)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomDeltaLink(null, null)
                },
                // Delta link with extra (ignored) attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomDeltaLink("http://odata.org", "text/html")
                        .XmlRepresentation(@"<feed>
                                                <link rel='http://docs.oasis-open.org/odata/ns/delta' href='http://odata.org' other='ignored' type='text/html' something='else' />
                                             </feed>")
                },
                // Delta link with attributes in wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().AtomDeltaLink("http://odata.org", null)
                        .XmlRepresentation(@"<feed>
                                                <link rel='http://docs.oasis-open.org/odata/ns/delta' href='http://odata.org' cn:type='text/html' 
                                                      cn:hreflang='en' cn:title='Some Title' cn:length='1000'
                                                      xmlns:cn='http://customnamespace.com' />
                                             </feed>")
                }
                #endregion delta link
            };

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        /// <summary>
        /// Constructs test descriptors which test feed ATOM metadata elements whose value is a URI ("icon" and "logo").
        /// </summary>
        /// <param name="uriElementName">The name of the ATOM metadata element to create test descriptors for.</param>
        /// <param name="addUriValueAtomMetadataAnnotation">A Func which takes an entity set instance and a string URI 
        /// and produces the entity set instance with the added URI ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreateUriValueFeedTestDescriptors(
            string uriElementName,
            Func<EntitySetInstance, string, EntitySetInstance> addUriValueAtomMetadataAnnotation)
        {
            return new PayloadReaderTestDescriptor[]
            {
                // Absolute URI
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addUriValueAtomMetadataAnnotation(PayloadBuilder.EntitySet(), "http://odata.org")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                            @"<feed>
                                <{0}>http://odata.org</{0}>
                              </feed>)", uriElementName))
                },
                // Relative URI
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addUriValueAtomMetadataAnnotation(PayloadBuilder.EntitySet(), "http://odata.org/relative/uri")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                            @"<feed xml:base='http://odata.org/'>
                                <{0}>relative/uri</{0}>
                             </feed>", uriElementName))
                },
                // URI with special characters
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addUriValueAtomMetadataAnnotation(PayloadBuilder.EntitySet(), "http://odata.org:8080/page?param=value&otherparam=other%20value")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                            @"<feed>
                                <{0}>http://odata.org:8080/page?param=value&amp;otherparam=other%20value</{0}>
                              </feed>)", uriElementName))
                },
                // URI-valued element with extra attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addUriValueAtomMetadataAnnotation(PayloadBuilder.EntitySet(), "http://odata.org")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                            @"<feed>
                                <{0} extra='attribute' bla='bla'>http://odata.org</{0}>
                              </feed>)", uriElementName))
                },
                // Multiple URI-valued elements (not allowed for logos and icons, which are the only two URI-valued elements in feeds)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                            @"<feed>
                                <{0}>http://first.uri</{0}>
                                <{0}>http://second.uri</{0}>
                              </feed>", uriElementName)),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomMetadataDeserializer_MultipleSingletonMetadataElements", uriElementName, "feed")
                },
            };
        }

        /// <summary>
        /// Constructs test descriptors which test feed ATOM metadata elements of type atomPersonConstruct ("author" and "contributor").
        /// </summary>
        /// <param name="personElementName">The name of the ATOM metadata element to create test descriptors for.</param>
        /// <param name="addPersonAtomMetadataAnnotation">A Func which takes an entity set instance, a name of a person, a string URI for the person,
        /// and an email address and produces the entity set instance with the added person construct ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreatePersonConstructFeedTestDescriptors(
            string personElementName,
            Func<EntitySetInstance, string, string, string, EntitySetInstance> addPersonAtomMetadataAnnotation)
        {
            return this.CreatePersonConstructTestDescriptors(PayloadBuilder.EntitySet(), "feed", personElementName, addPersonAtomMetadataAnnotation);
        }

        /// <summary>
        /// Constructs test descriptors which test feed ATOM metadata elements of type atomTextConstruct ("rights", "title", and "subtitle").
        /// </summary>
        /// <param name="textConstructElementName">The name of the ATOM metadata element to create test descriptors for.</param>
        /// <param name="addTextAtomMetadataAnnotation">A Func which takes an entity set instance, the value of the text construct element, 
        /// and the name of the text construct kind and produces the entity set instance with the added text construct ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreateTextConstructFeedTestDescriptors(
            string textConstructElementName,
            Func<EntitySetInstance, string, string, EntitySetInstance> addTextAtomMetadataAnnotation)
        {
            return this.CreateTextConstructTestDescriptors(
                PayloadBuilder.EntitySet(),
                "feed",
                textConstructElementName,
                addTextAtomMetadataAnnotation);
        }
    }
}
