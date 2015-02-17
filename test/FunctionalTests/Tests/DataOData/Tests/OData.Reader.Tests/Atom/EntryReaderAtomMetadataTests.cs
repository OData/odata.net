//---------------------------------------------------------------------
// <copyright file="EntryReaderAtomMetadataTests.cs" company="Microsoft">
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
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of ATOM metadata in entries.
    /// </summary>
    [TestClass, TestCase]
    public class EntryReaderAtomMetadataTests : ODataAtomMetadataReaderTestCase
    {
        /// <summary>
        /// Adds link information to an entity instance by setting the appropriate properties and adding the appropriate annotations.
        /// </summary>
        /// <param name="entity">The entity instance to add link information to.</param>
        /// <param name="href">The value of the href attribute on the link element.</param>
        /// <param name="rel">The value of the rel attribute on the link element.</param>
        /// <param name="type">The value of the type attribute on the link element.</param>
        /// <param name="hrefLang">The value of the hrefLang attribute on the link element.</param>
        /// <param name="title">The value of the title attribute on the link element.</param>
        /// <param name="length">The value of the length attribute on the link element.</param>
        /// <returns>The entity instance with the added link information.</returns>
        internal delegate EntityInstance AddLinkDetailsToEntity(EntityInstance entity,
            string href, string rel, string type = null, string hrefLang = null, string title = null, string length = null);

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of the category element as ATOM metadata in entries.")]
        public void EntryAtomMetadataCategoryTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = this.CreateCategoryTestDescriptors(PayloadBuilder.Entity(), "entry");

            // Add entry-specific category test case.
            testDescriptors = testDescriptors.Concat(
                new []
                {
                    // Category which also conveys the entity type in OData.
                    // If the category element is used to convey entity type it's stored separately.
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.TypeName")
                            .XmlRepresentation("<entry><category term='TestModel.TypeName' scheme='http://docs.oasis-open.org/odata/ns/scheme' label='someLabel' /></entry>")
                            .AtomCategoryWithTypeName("TestModel.TypeName", "someLabel")
                    },
                    // Category with type name, with no term - null TypeName reported
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation("<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' label='someLabel' /></entry>")
                            .AtomCategoryWithTypeName(null, "someLabel")
                    },
                });

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of date time ATOM metadata (ie, 'published' and 'updated') in entries.")]
        public void EntryAtomMetadataDateTimeOffsetTest()
        {
            // Published and updated are the two ATOM metadata elements which take the atomDateConstruct form (DateTimeOffset in our type system).
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreateDateConstructEntryTestDescriptors(
                    "published", AtomMetadataBuilder.AtomPublished);

            testDescriptors = testDescriptors.Concat(
                this.CreateDateConstructEntryTestDescriptors(
                    "updated", AtomMetadataBuilder.AtomUpdated));

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of the source element in an entry.")]
        public void EntryAtomMetadataSourceTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Empty source element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet())
                        .XmlRepresentation("<entry><source /></entry>")
                },
                // Source element with no children
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet())
                        .XmlRepresentation("<entry><source></source></entry>")
                },
                // Source element with only text value
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(PayloadBuilder.EntitySet())
                        .XmlRepresentation("<entry><source>test text</source></entry>")
                },
                // Source element with every type of feed metadata inside
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    // This includes two elements for every feed metadata element where duplicates are allowed.
                    PayloadElement = PayloadBuilder.Entity().AtomSource(
                        PayloadBuilder.EntitySet()
                            .AtomAuthor("John Smith", "http://john.smith", "john@smith.com")
                            .AtomAuthor("Jane Doe", null, null)
                            .AtomCategory("term", "http://scheme.org", "label")
                            .AtomContributor("John Smith", "http://john.smith", "john@smith.com")
                            .AtomContributor("Jane Doe", null, null)
                            .AtomGenerator("Generator Name", "http://odata.org", "2.5")
                            .AtomIcon("http://odata.org/icon")
                            .AtomId("http://some.unique.id/")
                            .AtomLink("http://testlink.com", "http://somerel.com", null)
                            .AtomLink("http://otherlink.com", "relval", "text/html", "en", "LinkTitle", "100")
                            .AtomLogo("http://odata.org/logo")
                            .AtomRights("Copyright (C) Some Company, 2015", TestAtomConstants.AtomTextConstructTextKind)
                            .AtomSubtitle("Subtitle <em>text</em>", TestAtomConstants.AtomTextConstructHtmlKind)
                            .AtomTitle("A Title", TestAtomConstants.AtomTextConstructTextKind)
                            .AtomUpdated("2001-10-26T21:32:52Z")
                            )
                },
                // Source element with feed metadata elements which would be interpreted differently if they were entry metadata
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AtomSource(
                        PayloadBuilder.EntitySet()
                            // The following links have rel values which are special to entries but not to feeds
                            .AtomLink("http://odata.org/edit", "edit", null)
                            .AtomLink("http://odata.org/editmedia", "edit-media", null)
                            .AtomLink("http://odata.org/namedstream", "http://docs.oasis-open.org/odata/ns/mediaresource/NamedStream", null)
                            .AtomLink("http://odata.org/editnamedstream", "http://docs.oasis-open.org/odata/ns/edit-media/NamedStream", null)
                            .AtomLink("http://odata.org/assoclink", "http://docs.oasis-open.org/odata/ns/relatedlinks/AssociationLink", null)
                            .AtomLink("http://odata.org/navprop", "http://docs.oasis-open.org/odata/ns/related/NavProp", null)
                            // On an entry this would modify the type name, but should have no effect with feeds
                            .AtomCategory("TestModel.TypeName", "http://docs.oasis-open.org/odata/ns/scheme", "someLabel"))
                },
            };

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of link elements as ATOM metadata.")]
        public void EntryAtomMetadataLinkTest()
        {
            // Self link
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreateLinkEntryTestDescriptors(
                    "self",
                    (entity, href, rel, type, hrefLang, title, length)
                        =>
                        {
                            if (href != null)
                            {
                                entity = entity.WithSelfLink(href);
                            }

                            return entity.AtomSelfLink(href, type, hrefLang, title, length);
                        });

            // Edit link
            testDescriptors = testDescriptors.Concat(
                this.CreateLinkEntryTestDescriptors(
                    "edit",
                    (entity, href, rel, type, hrefLang, title, length)
                        =>
                        {
                            if (href != null)
                            {
                                entity = entity.WithEditLink(href);
                            }

                            return entity.AtomEditLink(href, type, hrefLang, title, length);
                        }));

            // Edit-media link
            testDescriptors = testDescriptors.Concat(
                this.CreateLinkEntryTestDescriptors(
                    "edit-media",
                    (entity, href, rel, type, hrefLang, title, length)
                        => entity.AsMediaLinkEntry().StreamEditLink(href).SetAnnotation(
                            new NamedStreamAtomLinkMetadataAnnotation
                            {
                                Href = href,
                                HrefLang = hrefLang,
                                Length = length,
                                Relation = rel,
                                Title = title,
                                Type = type,
                            })));

            // Stream property self link
            testDescriptors = testDescriptors.Concat(
                this.CreateLinkEntryTestDescriptors(
                    "http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty",
                    (entity, href, rel, type, hrefLang, title, length)
                        => entity.StreamProperty(PayloadBuilder.StreamProperty("StreamProperty", href, null, type)
                                 .AtomNamedStreamSourceLink(href, type, hrefLang, title, length)),
                    titleAttributeOverride: "StreamProperty",
                    skipIanaPrefix: true));

            // Stream property edit link
            testDescriptors = testDescriptors.Concat(
                this.CreateLinkEntryTestDescriptors(
                    "http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty",
                    (entity, href, rel, type, hrefLang, title, length)
                        => entity.StreamProperty(PayloadBuilder.StreamProperty("StreamProperty", null, href, type)
                                 .AtomNamedStreamEditLink(href, type, hrefLang, title, length)),
                    titleAttributeOverride: "StreamProperty",
                    skipIanaPrefix: true));

            // TODO: Association Link - Add back support for customizing association link element in Atom

            // Navigation link
            testDescriptors = testDescriptors.Concat(
                this.CreateLinkEntryTestDescriptors(
                    "http://docs.oasis-open.org/odata/ns/related/NavProp",
                    (entity, href, rel, type, hrefLang, title, length)
                        => entity.NavigationProperty(
                            PayloadBuilder.DeferredNavigationProperty(
                                "NavProp",
                                new DeferredLink { UriString = href }.AtomLinkAttributes(href, rel, type, hrefLang, title, length),
                                null)),
                    titleAttributeOverride: "NavProp",
                    typeAttributeOverride: "application/atom+xml;type=entry",
                    skipIanaPrefix: true));

            // Non-OData link
            testDescriptors = testDescriptors.Concat(
                this.CreateLinkEntryTestDescriptors(
                    "http://some.uri",
                    (entity, href, rel, type, hrefLang, title, length) => entity.AtomLink(href, rel, type, hrefLang, title, length),
                    skipIanaPrefix: true));

            // Add some custom payloads.
            testDescriptors = testDescriptors.Concat(new PayloadReaderTestDescriptor[]
            {
                #region stream property tests
                // Stream property with both read link and edit link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .StreamProperty(PayloadBuilder.StreamProperty("StreamProperty", "http://odata.org/readlink", "http://odata.org/editlink")
                            .AtomNamedStreamSourceLink("http://odata.org/readlink", null)
                            .AtomNamedStreamEditLink("http://odata.org/editlink", null)),
                },
                #endregion stream property tests

                #region navigation property tests
                // Navigation link with association link
                // TODO: Association Link - Add back support for customizing association link element in Atom
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .NavigationProperty(PayloadBuilder.DeferredNavigationProperty(
                            "NavProp", 
                            new DeferredLink { UriString = "http://odata.org/navlink" }
                                .AtomLinkAttributes("http://odata.org/navlink", "http://docs.oasis-open.org/odata/ns/related/NavProp", "application/atom+xml;type=entry")))
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/navlink' type='application/atom+xml;type=entry'/>
                                                </entry>"),
                },
                #endregion navigation property tests
            });

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of person ATOM metadata (ie, 'author' and 'contributor') in entries.")]
        public void EntryAtomMetadataPersonConstructTest()
        {
            // Author and contributor are the two ATOM metadata elements which take the atomPersonConstruct form.
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreatePersonConstructEntryTestDescriptors(
                    "author", AtomMetadataBuilder.AtomAuthor);

            testDescriptors = testDescriptors.Concat(
                this.CreatePersonConstructEntryTestDescriptors(
                    "contributor", AtomMetadataBuilder.AtomContributor));

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of text construct ATOM metadata (ie, 'rights', 'title', and 'summary') in entries.")]
        public void EntryAtomMetadataTextConstructTest()
        {
            // Rights, title, and summary are the three ATOM metadata elements which take the atomTextConstruct form.
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                this.CreateTextConstructEntryTestDescriptors(
                    "rights", AtomMetadataBuilder.AtomRights);

            testDescriptors = testDescriptors.Concat(
                this.CreateTextConstructEntryTestDescriptors(
                    "title", AtomMetadataBuilder.AtomTitle));

            testDescriptors = testDescriptors.Concat(
                this.CreateTextConstructEntryTestDescriptors(
                    "summary", AtomMetadataBuilder.AtomSummary));

            this.RunAtomMetadataReaderTests(testDescriptors);
        }

        /// <summary>
        /// Constructs test descriptors which test the "link" element as ATOM metadata in an entry.
        /// </summary>
        /// <param name="relName">The value to use for the "rel" attribute of the link element. This value is also fed back to the <paramref name="addLinkDetails"/> function for convenience.</param>
        /// <param name="addLinkDetails">A delegate which adds link information to the payload in a manner specific to the type of link.</param>
        /// <param name="titleAttributeOverride">An optional override for the "title" attribute of the link element when this attribute is present on the element.</param>
        /// <param name="typeAttributeOverride">An optional override for the "type" attribute of the link element when this attribute is present on the element.</param>
        /// <param name="skipIanaPrefix">If true, there will not be a test descriptor where the "rel" attribute is prefixed with the IANA namespace.</param>
        /// <returns>A list of test descriptors.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreateLinkEntryTestDescriptors(
            string relName,
            AddLinkDetailsToEntity addLinkDetails, 
            string titleAttributeOverride = "sometitle",
            string typeAttributeOverride = "application/json",
            bool skipIanaPrefix = false)
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Link with just rel and href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addLinkDetails(PayloadBuilder.Entity(), "http://odata.org/link", relName)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                           @"<entry>
                                                <link rel='{0}' href='http://odata.org/link'/>
                                             </entry>", relName)),
                },
                // Link with additional ATOM metadata attributes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addLinkDetails(PayloadBuilder.Entity(), "http://odata.org/link", relName, typeAttributeOverride, "fr", titleAttributeOverride, "42")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                           @"<entry>
                                                <link href='http://odata.org/link' rel='{0}' title='{1}' type='{2}' hreflang='fr' length='42'/>
                                             </entry>", relName, titleAttributeOverride, typeAttributeOverride)),
                },
                // Link with additional ATOM metadata attributes in wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addLinkDetails(PayloadBuilder.Entity(), "http://odata.org/link", relName)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                            @"<entry>
                                                <link rel='{0}' href='http://odata.org/link' cn:title='Some Title' cn:type='text/html' cn:hreflang='fr' cn:length='42' xmlns:cn='http://custom.namespace.com' />
                                              </entry>", relName)),
                },
                // Link with empty attribute (should produce empty string, not null)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addLinkDetails(PayloadBuilder.Entity(), "http://odata.org/link", relName, null, string.Empty, null, null)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                            @"<entry>
                                                <link rel='{0}' href='http://odata.org/link' hreflang=''/>
                                              </entry>", relName)),
                },
                // Link with content (inner content should be ignored)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addLinkDetails(PayloadBuilder.Entity(), "http://odata.org/link", relName)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                            @"<entry>
                                                <link rel='{0}' href='http://odata.org/link'>
                                                    <child some='attribute'>
                                                        <grandchild bla='bla'/>
                                                    </child>
                                                </link>
                                              </entry>", relName)),
                },
                // Link with just rel - no href
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = addLinkDetails(PayloadBuilder.Entity(), null, relName)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                           @"<entry>
                                                <link rel='{0}'/>
                                             </entry>", relName)),
                },
            };

            if (!skipIanaPrefix)
            {
                testDescriptors = testDescriptors.ConcatSingle(
                    // Link with IANA rel
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = addLinkDetails(PayloadBuilder.Entity(), "http://odata.org/link", relName)
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                               @"<entry>
                                                    <link href='http://odata.org/link' rel='http://www.iana.org/assignments/relation/{0}'/>
                                                 </entry>", relName)),
                    });
            }

            return testDescriptors;
        }

        /// <summary>
        /// Constructs test descriptors which test entry ATOM metadata elements of type atomPersonConstruct ("author" and "contributor").
        /// </summary>
        /// <param name="elementName">The name of the ATOM metadata element to create test descriptors for.</param>
        /// <param name="addPersonAtomMetadataAnnotation">A Func which takes an entity instance, a name of a person, a string URI for the person,
        /// and an email address and produces the entity instance with the added person construct ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreatePersonConstructEntryTestDescriptors(
            string elementName, 
            Func<EntityInstance, string, string, string, EntityInstance> addPersonAtomMetadataAnnotation)
        {
            return this.CreatePersonConstructTestDescriptors(PayloadBuilder.Entity(), "entry", elementName, addPersonAtomMetadataAnnotation);
        }

        /// <summary>
        /// Constructs test descriptors which test entry ATOM metadata elements of type atomDateConstruct ("updated" and "published").
        /// </summary>
        /// <param name="elementName">The name of the ATOM metadata element to create test descriptors for.</param>
        /// <param name="addDateAtomMetadataAnnotation">A Func which takes an entity instance and a date time string 
        /// and produces the entity instance with the added date construct ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreateDateConstructEntryTestDescriptors(
            string elementName,
            Func<EntityInstance, string, EntityInstance> addDateAtomMetadataAnnotation)
        {
            return this.CreateDateConstructTestDescriptors(
                PayloadBuilder.Entity(),
                "entry",
                elementName, 
                addDateAtomMetadataAnnotation);
        }

        /// <summary>
        /// Constructs test descriptors which test entry ATOM metadata elements of type atomTextConstruct ("rights", "title", and "summary").
        /// </summary>
        /// <param name="elementName">The name of the ATOM metadata element to create test descriptors for.</param>
        /// <param name="addTextAtomMetadataAnnotation">A Func which takes an entity instance, the value of the text construct element, 
        /// and the name of the text construct kind and produces the entity instance with the added text construct ATOM metadata.</param>
        /// <returns>A list of test descriptors.</returns>
        private IEnumerable<PayloadReaderTestDescriptor> CreateTextConstructEntryTestDescriptors(
            string elementName,
            Func<EntityInstance, string, string, EntityInstance> addTextAtomMetadataAnnotation)
        {
            return this.CreateTextConstructTestDescriptors(
                PayloadBuilder.Entity(), 
                "entry",
                elementName, 
                addTextAtomMetadataAnnotation);
        }
    }
}
