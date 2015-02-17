//---------------------------------------------------------------------
// <copyright file="NavigationLinkReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of navigation links in ATOM.
    /// </summary>
    [TestClass, TestCase]
    public class NavigationLinkReaderAtomTests : ODataReaderTestCase
    {
        private PayloadReaderTestDescriptor.Settings settings;

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings
        {
            get
            {
                return this.settings;
            }

            set
            {
                // Use the injected values except for the ODataOM -> payload element convertor
                // for which we need the ATOM specific variant.
                this.settings = value;
                this.settings.ExpectedResultSettings.ObjectModelToPayloadElementConverter = new AtomObjectModelToPayloadElementConverter();
            }
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of deferred and entity reference links without metadata.")]
        public void DeferredAndEntityReferenceLinkNoMetadataTest()
        {
            // Note that the difference between deferred link and entity reference link is not visible in the payload for ATOM at all.
            // It's just that in request it's reported as entity reference link and in response it's response as deferred link.
            // The test infrastructure verifies that the correct one is reported, so the test descriptor stays the same.
            IEnumerable<NavigationLinkTestCaseDescriptor> testDescriptors = new[]
            {
                // Navigation link with entry type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Navigation link with feed type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(true)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Navigation link with entry type - different casing
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='applicatioN/atoM+xMl;tYpe=enTry'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Navigation link with feed type - different casing
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(true)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' type='Application/ATOM+xmL;Type=FEED' href='http://odata.org/link'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Navigation link with entry type - more content type parameters
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;foo=bar;type=entry;mytype=other'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Navigation link with no type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link")
                            .XmlRepresentation("<link type='application/atom+xml' rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", null } },
                },
                // Navigation link with no type - more content type parameters
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link")
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+XML;param=value'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", null } },
                },
                // Navigation link with feed type - two type parameters (first one is used only)
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(true)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed;type=entry'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Navigation link with wrong type - type is ignored
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/link")
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=foo'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", null } },
                },
                // Navigation link with no type attribute - allowed
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp",
                        PayloadBuilder.DeferredLink("http://odata.org/link")
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", null } },
                },
                // Navigation link with empty type attribute - allowed and recognized as nav. link
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp",
                        PayloadBuilder.DeferredLink("http://odata.org/link")
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type=''/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", null } },
                },
                // Navigation link with type attribute in wrong namespace - treated as missing type attribute and thus allowed
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp",
                        PayloadBuilder.DeferredLink("http://odata.org/link")
                            .XmlRepresentation("<link m:type='application/atom+xml;type=entry' rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type=''/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", null } },
                },
                // Navigation link with wrong mime type name (just atom) - not recognized as nav. link at all
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation("<entry><link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/xml'/></entry>")
                },
                // Navigation link with wrong mime type name (just xml) - not recognized as nav. link at all
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation("<entry><link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom'/></entry>")
                },

                // Navigation link with empty name - not recognized as nav. link
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation("<entry><link rel='http://docs.oasis-open.org/odata/ns/related/' href='http://odata.org/link' type='application/atom+xml;type=entry'/></entry>")
                },
                // Navigation link with different cased rel namespace - not recognized as nav. link
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation("<entry><link rel='http://docs.oasis-open.org/odata/Ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'/></entry>")
                },
                // Navigation link without rel - not recognized as nav. link
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation("<entry><link href='http://odata.org/link' type='application/atom+xml;type=entry'/></entry>")
                },
                // Navigation link with empty rel - not recognized as nav. link
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation("<entry><link rel='' href='http://odata.org/link' type='application/atom+xml;type=entry'/></entry>")
                },
                // Navigation link without href with entry type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink(null).IsCollection(false)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' type='application/atom+xml;type=entry'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Navigation link without href with feed type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink(null).IsCollection(true)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' type='application/atom+xml;type=feed'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Navigation link without href and without type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink(null)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' type='application/atom+xml'/>")
                        ),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", null } },
                },
                // Navigation link with empty href (need to specify xml:base) - works as relative URI
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                        PayloadBuilder.DeferredLink("http://odata.org/some").IsCollection(false)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='' type='application/atom+xml;type=entry' xml:base='http://odata.org/some'/>")
                        ),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesServer; },
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of deferred and entity reference links with metadata.")]
        public void DeferredAndEntityReferenceLinkWithMetadataTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            // Note that the difference between deferred link and entity reference link is not visible in the payload for ATOM at all.
            // It's just that in request it's reported as entity reference link and in response it's response as deferred link.
            // The test infrastructure verifies that the correct one is reported, so the test descriptor stays the same.
            IEnumerable<NavigationLinkTestCaseDescriptor> testDescriptors = new[]
            {
                // Navigation link with entry type for a singleton property
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("PoliceStation", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)),
                    PayloadEdmModel = model,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
                // Navigation link with feed type for a collection property
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("CityHall", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(true)),
                    PayloadEdmModel = model,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                // Navigation link with entry type for a collection property (this is allowed for deferred links (entity reference links) in request)
                // WCF DS Client ignores the type=entry on nav. links.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("CityHall", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)),
                    PayloadEdmModel = model,
                    SkipTestConfiguration = (tc => tc.IsRequest == false || tc.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesClient),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", false } },
                },
                // Navigation link with entry type for a collection property (this is not allowed for deferred links in response)
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("CityHall", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)),
                    PayloadEdmModel = model,
                    SkipTestConfiguration = (tc => tc.IsRequest == true),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_DeferredEntryInFeedNavigationLink")
                },
                // Navigation link with feed type for a singleton property - this is not allowed
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("PoliceStation", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(true)),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty", "PoliceStation")
                },
                // Navigation link without type for a singleton property - the type is taken from metadata
                // WCF DS Client ignores the type=entry on nav. links.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("PoliceStation", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/PoliceStation' href='http://odata.org/link' type='application/atom+xml'/>")
                        ),
                    PayloadEdmModel = model,
                    SkipTestConfiguration = (tc => tc.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesClient),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false } },
                },
                // Navigation link without type for a collection property - the type is taken from metadata
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("CityHall", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(true)
                            .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/CityHall' href='http://odata.org/link' type='application/atom+xml'/>")
                        ),
                    PayloadEdmModel = model,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                // Navigation link which is not declared
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("Unknown", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "Unknown", "TestModel.CityType")
                },
                // Navigation link which is not declared on an open type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityOpenType").DeferredNavigationProperty("Unknown", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_OpenNavigationProperty", "Unknown", "TestModel.CityOpenType")
                },
                // Navigation link which is declared as primitive property
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("Name", 
                        PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_NavigationPropertyExpected", "Name", "TestModel.CityType", "Structural")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                TestReaderUtils.ODataBehaviorKinds,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, behaviorKind, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of child nodes of the navigation link.")]
        public void NavigationLinkChildrenTest()
        {
            string[] paddingToSkip = new string[]
            {
                string.Empty,
                "  \r\n\t",
                "<!-- some -->   <?value?>",
                "some text",
                "<![CDATA[cdata]]>",
                "<c:some xmlns:c='c'/>",
                "<m:unknown/>",
                "<entry/>",
                "<feed/>"
            };

            IEnumerable<NavigationLinkTestCaseDescriptor> testDescriptors = paddingToSkip.SelectMany(padding =>
                new[]
                {
                    // Empty link - entry 
                    new NavigationLinkTestCaseDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                            PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>{0}</link>",
                                    padding))
                            ),
                        SkipTestConfiguration = tc => tc.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesClient,
                        ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                    },
                    // Empty link - feed
                    new NavigationLinkTestCaseDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                            PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(true)
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>{0}</link>",
                                    padding))
                            ),
                        ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                    },
                    // Empty link - unknown
                    new NavigationLinkTestCaseDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().DeferredNavigationProperty("NavProp", 
                            PayloadBuilder.DeferredLink("http://odata.org/link")
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                    "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml'>{0}</link>",
                                    padding))
                            ),
                        ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", null } },
                    },
                    // Link with empty inline - entry
                    new NavigationLinkTestCaseDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().Property(
                            PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.NullEntity(), "http://odata.org/link").XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>{0}" +
                                "<m:inline/>{0}" +
                                "</link>",
                                padding))
                            ),
                        ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                    },
                    // Link with empty inline - unknown
                    new NavigationLinkTestCaseDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().Property(
                            PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.NullEntity(), "http://odata.org/link").XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml'>{0}" +
                                "<m:inline/>{0}" +
                                "</link>",
                                padding))
                            ),
                        ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                    },
                    // Two m:inline elements
                    new NavigationLinkTestCaseDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().Property(
                            PayloadBuilder.ExpandedNavigationProperty("NavProp", null, "http://odata.org/link").XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml'>{0}" +
                                "<m:inline/>{0}" +
                                "<m:inline/>{0}" +
                                "</link>",
                                padding))
                            ),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleInlineElementsInLink"),
                    },
                    // Two m:inline elements, the second element with a child element.
                    new NavigationLinkTestCaseDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().Property(
                            PayloadBuilder.ExpandedNavigationProperty("NavProp", null, "http://odata.org/link").XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                @"<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml'>{0}
                                    <m:inline/>{0}
                                    <m:inline>
                                        <entry>
                                            <id>http://host/BSet(2)</id>
                                            <content type='application/xml' >
                                                <m:properties>
                                                <d:ID m:type='Edm.Int32'>12</d:ID>
                                            </m:properties>
                                            </content>
                                        </entry>
                                    </m:inline>{0}
                                </link>",
                                padding))
                            ),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleInlineElementsInLink"),
                    },
                });

            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (behaviorKind, testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of child nodes of the m:inline element.")]
        public void InlineChildrenTest()
        {
            KeyValuePair<string, ExpectedException>[] paddingToSkip = new KeyValuePair<string, ExpectedException>[]
            {
                new KeyValuePair<string, ExpectedException>(string.Empty, null),
                new KeyValuePair<string, ExpectedException>("  \r\n\t", null),
                new KeyValuePair<string, ExpectedException>("<!-- some -->   <?value?>", null),
                new KeyValuePair<string, ExpectedException>("some text", null),
                new KeyValuePair<string, ExpectedException>("<![CDATA[cdata]]>", null),
                new KeyValuePair<string, ExpectedException>("<c:some xmlns:c='c'/>", null),
                // The default namespace is ATOM, so this element is in ATOM namespace and thus rejected.
                new KeyValuePair<string, ExpectedException>("<unknown/>", ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_UnknownElementInInline", "unknown")),
                new KeyValuePair<string, ExpectedException>("<m:unknown/>", null),
                new KeyValuePair<string, ExpectedException>("<d:unknown/>", null),
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = paddingToSkip.SelectMany(padding =>
                new[]
                {
                    // Empty m:inline
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().Property(
                            PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.NullEntity(), "http://odata.org/link").XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                                "<m:inline>{0}</m:inline>" +
                                "</link>",
                                padding.Key))
                            ),
                        ExpectedException = padding.Value
                    },
                    // Expanded entry
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().Property(
                            PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity(), "http://odata.org/link").XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                                "<m:inline>{0}" +
                                "<entry/>{0}" +
                                "</m:inline>" +
                                "</link>",
                                padding.Key))
                            ),
                        ExpectedException = padding.Value
                    },
                    // Expanded feed
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().Property(
                            PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link").XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                                "<m:inline>{0}" +
                                "<feed/>{0}" +
                                "</m:inline>" +
                                "</link>",
                                padding.Key))
                            ),
                        ExpectedException = padding.Value
                    },
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of expanded links without metadata.")]
        public void ExpandedLinkNoMetadataTest()
        {
            // TODO: Add the href validation once the Taupo OM can handle that.
            IEnumerable<NavigationLinkTestCaseDescriptor> testDescriptors = new[]
            {
                // Expanded entry
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity().ETag("1"), "http://odata.org/link")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Expanded entry - no type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity().ETag("1"), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml'>" +
                            "<m:inline><entry m:etag='1'/></m:inline>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Expanded feed
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Expanded feed - no type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml'>" +
                            "<m:inline><feed/></m:inline>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Expanded null entry
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.NullEntity(), "http://odata.org/link")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Expanded null entry with end m:inline element.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.NullEntity(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline></m:inline>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Expanded null entry with insignificant nodes in m:inline element.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.NullEntity(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline>    <!--comment--> <?value?>  </m:inline>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Expanded null entry with additional attributes on m:inline
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.NullEntity(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline foo='1' m:type='Edm.String' d:prop='null' href='' type='application/atom+xml;type=feed'/>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Expanded null entry without type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.NullEntity(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml'>" +
                            "<m:inline/>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Expanded empty inline with feed type - reports an empty feed
                // We decided we will relax ODL even by default and allow empty inline, we will treat it as empty feed.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline/>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Expanded empty inline with end m:inline element with feed type - reports an empty feed
                // We decided we will relax ODL even by default and allow empty inline, we will treat it as empty feed.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline></m:inline>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Expanded empty inline with insignificant nodes in m:inline element with feed type - reports an empty feed
                // We decided we will relax ODL even by default and allow empty inline, we will treat it as empty feed.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline>    <!--comment--> <?value?>  </m:inline>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Expanded empty inline with additional attributes with feed type - reports an empty feed
                // We decided we will relax ODL even by default and allow empty inline, we will treat it as empty feed.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline foo='1' m:type='Edm.String' d:prop='null' href='' type='application/atom+xml;type=feed'/>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Expanded entry with feed type -> should fail
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline><entry/></m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_ExpandedEntryInFeedNavigationLink")
                },
                // Expanded feed with entry type -> should fail
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline><feed/></m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_ExpandedFeedInEntryNavigationLink")
                },
                // Expanded entry with another expanded entry -> fail
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity().ETag("1"), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline><entry m:etag='1'/><entry/></m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline", "Entry")
                },
                // Expanded entry with another expanded feed -> fail
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity().ETag("1"), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline><entry m:etag='1'/><feed/></m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline", "Feed")
                },
                // Expanded feed with another expanded entry -> fail
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline><feed/><entry/></m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline", "Entry")
                },
                // Expanded feed with another expanded feed -> fail
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline><feed/><feed/></m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline", "Feed")
                },
                // Expanded entry - no href
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity().ETag("1"))),
                },
                // Expanded entry - empty href
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity().ETag("1"), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='' xml:base='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline><entry m:etag='1'/></m:inline>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", false } },
                },
                // Expanded feed - no href
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet())),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Expanded feed - empty href
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='' xml:base='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline><feed/></m:inline>" +
                            "</link>")),
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "NavProp", true } },
                },
                // Expanded feed - with additional elements in the m:inline
                // Regression test for this bug
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='' xml:base='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline>" +
                                "<feed/><foo><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='DefaultNamespace.Order'/>" +
                                    "<content type='application/xml'><m:properties><d:ID m:type='Edm.Int32'>1042</d:ID></m:properties></content>" +
                                "</entry>" +
                                "</feed></foo>" +
                            "</m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_UnknownElementInInline", "foo"),
                },
                // Expanded feed - with two feed elements
                // Regression test for this bug
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='' xml:base='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline>" +
                                "<feed/>" +
                                "<feed/>" +
                            "</m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline", "Feed"),
                },
                // Expanded feed - with feed and entry elements
                // Regression test for this bug
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='' xml:base='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline>" +
                                "<feed/>" +
                                "<entry/>" +
                            "</m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline", "Entry"),
                },
                // Expanded entry - with two entry elements
                // Regression test for this bug
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='' xml:base='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline>" +
                                "<entry/>" +
                                "<entry/>" +
                            "</m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline", "Entry"),
                },
                // Expanded entry - with entry and feed elements
                // Regression test for this bug
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().Property(
                        PayloadBuilder.ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='' xml:base='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline>" +
                                "<entry/>" +
                                "<feed/>" +
                            "</m:inline>" +
                            "</link>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleExpansionsInInline", "Feed"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of deferred and entity reference links with metadata.")]
        public void ExpandedLinkWithMetadataTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            IEnumerable<NavigationLinkTestCaseDescriptor> testDescriptors = new[]
            {
                // Expanded entry for singleton nav. prop.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").Property(
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation", 
                            PayloadBuilder.Entity("TestModel.OfficeType").AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }).ETag("1"),
                            "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/PoliceStation' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline><entry m:etag='1'/></m:inline>" +
                            "</link>")),
                    PayloadEdmModel = model,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false }},
                },
                // Expanded entry - no type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").Property(
                        PayloadBuilder.ExpandedNavigationProperty(
                            "PoliceStation",    
                            PayloadBuilder.Entity("TestModel.OfficeType").AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }).ETag("1"),
                            "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/PoliceStation' href='http://odata.org/link' type='application/atom+xml'>" +
                            "<m:inline><entry m:etag='1'/></m:inline>" +
                            "</link>")),
                    PayloadEdmModel = model,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "PoliceStation", false }},
                },
                // Expanded feed
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").Property(
                        PayloadBuilder.ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/CityHall' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline><feed/></m:inline>" +
                            "</link>")),
                    PayloadEdmModel = model,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                // Expanded feed - no type
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").Property(
                        PayloadBuilder.ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/CityHall' href='http://odata.org/link' type='application/atom+xml'>" +
                            "<m:inline><feed/></m:inline>" +
                            "</link>")),
                    PayloadEdmModel = model,
                    ExpectedIsCollectionValues = new Dictionary<string, bool?> { { "CityHall", true } },
                },
                // Expanded entry for collection nav. prop. - should fail
                // The fix ensures this test fails.
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").Property(
                        PayloadBuilder.ExpandedNavigationProperty(
                            "CityHall", 
                            PayloadBuilder.Entity("TestModel.OfficeType").AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }).ETag("1"),
                            "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/CityHall' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline><entry m:etag='1'/></m:inline>" +
                            "</link>")),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_ExpandedEntryInFeedNavigationLink")
                },
                // Expanded feed with link type entry for collection nav. prop. - should fail
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").Property(
                        PayloadBuilder.ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/CityHall' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                            "<m:inline><feed/></m:inline>" +
                            "</link>")),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_ExpandedFeedInEntryNavigationLink")
                },
                // Expanded feed for singleton nav. prop. - should fail
                new NavigationLinkTestCaseDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").Property(
                        PayloadBuilder.ExpandedNavigationProperty("PoliceStation", PayloadBuilder.EntitySet(), "http://odata.org/link")
                        .XmlRepresentation("<link rel='http://docs.oasis-open.org/odata/ns/related/PoliceStation' href='http://odata.org/link' type='application/atom+xml;type=feed'>" +
                            "<m:inline><feed/></m:inline>" +
                            "</link>")),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_FeedNavigationLinkForResourceReferenceProperty", "PoliceStation")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                TestReaderUtils.ODataBehaviorKinds,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, behaviorKind, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of deferred links in responses with regard to UndeclaredPropertyBehaviorKinds setting.")]
        public void UndeclaredPropertyBehaviorKindLinkTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataUndeclaredPropertyBehaviorKindsCombinations,
                undeclaredPropertyBehaviorKinds =>
                {
                    bool reportUndeclaredLink = undeclaredPropertyBehaviorKinds.HasFlag(ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty);
                    bool ignoreUndeclaredValue = undeclaredPropertyBehaviorKinds.HasFlag(ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty);
                    var propertyDoesNotExistOnTypeException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "UndeclaredProperty", "TestModel.CityType");

                    IEnumerable<NavigationLinkTestCaseDescriptor> testDescriptors = new[]
                    {
                        // Normal empty undeclared deferred link
                        new NavigationLinkTestCaseDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("UndeclaredProperty", PayloadBuilder.DeferredLink("http://odata.org/link")),
                            PayloadEdmModel = model,
                            ExpectedException = reportUndeclaredLink ? null : propertyDoesNotExistOnTypeException,
                            ExpectedIsCollectionValues = reportUndeclaredLink ? null : new Dictionary<string, bool?> { { "UndeclaredProperty", null } },
                        },
                        // Collection empty undeclared deferred link
                        new NavigationLinkTestCaseDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("UndeclaredProperty", 
                                PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(true)),
                            PayloadEdmModel = model,
                            ExpectedException = reportUndeclaredLink ? null : propertyDoesNotExistOnTypeException,
                            ExpectedIsCollectionValues = reportUndeclaredLink ? null : new Dictionary<string, bool?> { { "UndeclaredProperty", true } },
                        },
                        // Singleton empty undeclared deferred link
                        new NavigationLinkTestCaseDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("UndeclaredProperty", 
                                PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)),
                            PayloadEdmModel = model,
                            ExpectedException = reportUndeclaredLink ? null : propertyDoesNotExistOnTypeException,
                            ExpectedIsCollectionValues = reportUndeclaredLink ? null : new Dictionary<string, bool?> { { "UndeclaredProperty", false } },
                        },
                        // Non-empty undeclared deferred link - all content should be ignored as long as it's not m:inline
                        new NavigationLinkTestCaseDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("UndeclaredProperty", 
                                PayloadBuilder.DeferredLink("http://odata.org/link").IsCollection(false)
                                    .XmlRepresentation(
                                        "<link rel='http://docs.oasis-open.org/odata/ns/related/UndeclaredProperty' href='http://odata.org/link' type='application/atom+xml;type=entry'>" +
                                            "test<some/>value" +
                                        "</link>")),
                            PayloadEdmModel = model,
                            ExpectedException = reportUndeclaredLink ? null : propertyDoesNotExistOnTypeException,
                            ExpectedIsCollectionValues = reportUndeclaredLink ? null : new Dictionary<string, bool?> { { "UndeclaredProperty", false } },
                        },
                        // Expanded entry undeclared link
                        new NavigationLinkTestCaseDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").ExpandedNavigationProperty("UndeclaredProperty", 
                                PayloadBuilder.Entity("TestModel.CityType")),
                            ExpectedResultPayloadElement = tc => PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("UndeclaredProperty", 
                                PayloadBuilder.DeferredLink(null).IsCollection(false)),
                            PayloadEdmModel = model,
                            ExpectedException = reportUndeclaredLink ? (ignoreUndeclaredValue ? null : propertyDoesNotExistOnTypeException) : propertyDoesNotExistOnTypeException,
                            ExpectedIsCollectionValues = (reportUndeclaredLink && ignoreUndeclaredValue) ? null : new Dictionary<string, bool?> { { "UndeclaredProperty", false } },
                        },
                        // Expanded feed undeclared link
                        new NavigationLinkTestCaseDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").ExpandedNavigationProperty("UndeclaredProperty", 
                                PayloadBuilder.EntitySet()),
                            ExpectedResultPayloadElement = tc => PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("UndeclaredProperty", 
                                PayloadBuilder.DeferredLink(null).IsCollection(true)),
                            PayloadEdmModel = model,
                            ExpectedException = reportUndeclaredLink ? (ignoreUndeclaredValue ? null : propertyDoesNotExistOnTypeException) : propertyDoesNotExistOnTypeException,
                            ExpectedIsCollectionValues = (reportUndeclaredLink && ignoreUndeclaredValue) ? null : new Dictionary<string, bool?> { { "UndeclaredProperty", true } },
                        },
                        // Expanded null entry undeclared link
                        new NavigationLinkTestCaseDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").ExpandedNavigationProperty("UndeclaredProperty", 
                                null),
                            ExpectedResultPayloadElement = tc => PayloadBuilder.Entity("TestModel.CityType").DeferredNavigationProperty("UndeclaredProperty", 
                                PayloadBuilder.DeferredLink(null)),
                            PayloadEdmModel = model,
                            ExpectedException = reportUndeclaredLink ? (ignoreUndeclaredValue ? null : propertyDoesNotExistOnTypeException) : propertyDoesNotExistOnTypeException,
                            ExpectedIsCollectionValues = (reportUndeclaredLink && ignoreUndeclaredValue) ? null : new Dictionary<string, bool?> { { "UndeclaredProperty", null } },
                        },
                    };

                    this.CombinatorialEngineProvider.RunCombinations(
                        testDescriptors,
                        this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                        (testDescriptor, testConfiguration) =>
                        {
                            testConfiguration = new ReaderTestConfiguration(testConfiguration);
                            testConfiguration.MessageReaderSettings.UndeclaredPropertyBehaviorKinds = undeclaredPropertyBehaviorKinds;
                            testDescriptor.RunTest(testConfiguration);
                        });
                });
        }
    }
}
