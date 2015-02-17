//---------------------------------------------------------------------
// <copyright file="EntryReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of entries in ATOM.
    /// </summary>
    [TestClass, TestCase]
    public class EntryReaderAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        private static readonly IEnumerable<KeyValuePair<string, XmlQualifiedName>> paddings = new KeyValuePair<string, XmlQualifiedName>[]
            {
                new KeyValuePair<string, XmlQualifiedName>(string.Empty, null),
                new KeyValuePair<string, XmlQualifiedName>("  \r\n\t", null),
                new KeyValuePair<string, XmlQualifiedName>("<!-- some -->   <?value?>", null),
                new KeyValuePair<string, XmlQualifiedName>("some text", null),
                new KeyValuePair<string, XmlQualifiedName>("<![CDATA[cdata]]>", null),
                new KeyValuePair<string, XmlQualifiedName>("<c:some xmlns:c='c'/>", new XmlQualifiedName("some", "c")),
                new KeyValuePair<string, XmlQualifiedName>("<unknown/>", new XmlQualifiedName("unknown")),
                new KeyValuePair<string, XmlQualifiedName>("<d:properties/>", new XmlQualifiedName("properties", "http://docs.oasis-open.org/odata/ns/data")),
                new KeyValuePair<string, XmlQualifiedName>("<m:properTies/>", new XmlQualifiedName("properTies", "http://docs.oasis-open.org/odata/ns/metadata")),
            };

        private readonly static Func<XmlQualifiedName, ExpectedException> getExpectedExceptionForPadding = (qName) =>
            qName == null || qName.Namespace != "http://docs.oasis-open.org/odata/ns/metadata"
            ? (ExpectedException)null
            : ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithInvalidNode", qName.Name);

        // TODO: Add tests for property metadata validation (maybe we can reuse existing format independent tests)
        // TODO: Add tests for no metadata property reading - complex/collection/primitive

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of the entry start element.")]
        public void EntryStartElementTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Empty entry element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry/>")
                },
                // Element with wrong local name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<feed/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", "feed", "http://www.w3.org/2005/Atom"),
                },
                // Element with wrong local name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<Entry/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", "Entry", "http://www.w3.org/2005/Atom"),
                },
                // Element with wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<d:entry/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", "entry", "http://docs.oasis-open.org/odata/ns/data"),
                },
                // Element with wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<m:type/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_EntryElementWrongName", "type", "http://docs.oasis-open.org/odata/ns/metadata"),
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

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct type name detection for entries - look ahead.")]
        public void EntryTypeNameDetectionTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                (behaviorKind) =>
                {
                    IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
                    {
                        // Type name
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.TypeName")
                                .XmlRepresentation("<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName'/></entry>")
                        },
                        // No term attribute - no type name
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme'/></entry>")
                        },
                        // No category - no type name
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry></entry>")
                        },
                        // Empty type name
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity(string.Empty)
                                .XmlRepresentation("<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term=''/></entry>"),
                        },
                        // Type name - other stuff before category
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.TypeName")
                                .XmlRepresentation("<entry>" +
                                    "<!-- some --><author/><c:custom xmlns:c='cust'>bar<bar/></c:custom> foo " + 
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName'/>" +
                                    "</entry>")
                        },
                        // Type name - other attributes on category
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.TypeName")
                                .XmlRepresentation("<entry><category foo='bar' term='TestModel.TypeName' c:c='c' scheme='http://docs.oasis-open.org/odata/ns/scheme' xmlns:c='c'/></entry>")
                        },
                        // Category with wrong scheme - no typename
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry><category scheme='http://docs.oasis-open.org/odata/ns/SCheme' term='TestModel.TypeName'/></entry>")
                        },
                        // Category with right scheme but no term (invalid, but we accept it) - no typename
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme'/></entry>")
                        },
                        // Type name - two valid category typenames - the first one should be used only
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.TypeName")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName'/>" + 
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeNameSecond'/>" + 
                                    "</entry>"),
                            ExpectedResultCallback = (testConfig) =>
                                new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    // The default behavior is to disallow duplicates of entry/category element defined in ODATA spec.
                                    ExpectedException = behaviorKind == TestODataBehaviorKind.Default 
                                    ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "category")
                                    : null,
                                },
                        },
                        // Two categories - first one with no term and the other with a term. Should pick the first category with a valid scheme.
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' />" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName'/>" +
                                    "</entry>"),
                            ExpectedResultCallback = (testConfig) =>
                                new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    // The default behavior is to disallow duplicates of entry/category element defined in ODATA spec.
                                    ExpectedException = behaviorKind == TestODataBehaviorKind.Default 
                                    ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "category")
                                    : null,
                                },
                        },
                        // Two categories - first one with a term and the other with no term. Should pick the first category with a valid scheme.
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.TypeName")
                                .XmlRepresentation("<entry>" +                            
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName'/>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' />" +
                                    "</entry>"),
                            ExpectedResultCallback = (testConfig) =>
                                new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    // The default behavior is to disallow duplicates of entry/category element defined in ODATA spec.
                                    ExpectedException = behaviorKind == TestODataBehaviorKind.Default 
                                    ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "category")
                                    : null,
                                },
                        },
                        // More categories - one of them with a valid schema and no type name, others have either wrong or missing pieces.
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' />" +
                                    "  <category term='TestModel.TypeName2'/>" +
                                    "  <CateGory scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName3'/>" +
                                    "  <category scheme='myscheme' term='myterm'/>" +
                                    "  <d:category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName4'/>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' />" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName5'/>" +
                                    "</entry>"),
                            ExpectedResultCallback = (testConfig) =>
                                new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    // The default behavior is to disallow duplicates of entry/category element defined in ODATA spec.
                                    ExpectedException = behaviorKind == TestODataBehaviorKind.Default 
                                    ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "category") 
                                    : null,
                                },
                        },
                        // More categories - one of them is the type name others have either wrong or missing pieces.
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.TypeName")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' />" +
                                    "  <category term='TestModel.TypeName2'/>" +
                                    "  <CateGory scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName3'/>" +
                                    "  <category scheme='myscheme' term='myterm'/>" +
                                    "  <d:category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName4'/>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName'/>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName5'/>" +
                                    "</entry>"),
                            ExpectedResultCallback = (testConfig) =>
                                new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    // The default behavior is to disallow duplicates of entry/category element defined in ODATA spec.
                                    ExpectedException = behaviorKind == TestODataBehaviorKind.Default 
                                    ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "category")
                                    : null,
                                },
                        },
                    };

                    this.CombinatorialEngineProvider.RunCombinations(
                        testDescriptors,
                        this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                        (testDescriptor, testConfiguration) =>
                        {
                            testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                        });
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of ETag on entry.")]
        public void EntryETagTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Standard etag
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().ETag("myetag").XmlRepresentation("<entry m:etag='myetag'/>")
                },
                // Empty etag (invalid, but we read it)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().ETag(string.Empty).XmlRepresentation("<entry m:etag=''/>")
                },
                // Etag attribute in wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry d:etag='myetag'/>")
                },
                // Etag attribute in wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry etag='myetag'/>")
                },
                // Etag attribute with wrong name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry m:Etag='myetag'/>")
                },
                // Additional attributes on the entry - ignored
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().ETag("entry:etag").XmlRepresentation("<entry foo='bar' m:ETAG='some' m:etag='entry:etag' m:type='foo'/>")
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

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of child nodes of atom:entry element.")]
        public void EntryChildrenTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Empty entry
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry/>")
                },
            };

            testDescriptors = testDescriptors.Concat(paddings.SelectMany(padding =>
                new PayloadReaderTestDescriptor[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry>{0}</entry>",
                                padding.Key)),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity("TestModel.TypeName").Id("urn:id").WithEditLink("http://odata.org/editlink")
                            .NavigationProperty("NavProp", "http://odata.org/link", "http://odata.org/link2")
                            .StreamProperty("StreamProperty", "http://odata.org/readlink", null, null, null)
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry>{0}" +
                                "<id>urn:id</id>{0}" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.TypeName'/>{0}" +
                                "<category scheme='foo' term='bar'/>{0}" +
                                "<link rel='edit' href='http://odata.org/editlink'/>{0}" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'/>{0}" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/NavProp' href='http://odata.org/link2' type='application/xml'/>{0}" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/readlink' m:etag='someetag'/>{0}" +
                                "<content type='application/xml'/>{0}" +
                                "</entry>",
                                padding.Key)),
                        SkipTestConfiguration = tc => tc.IsRequest
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr").StreamContentType("mime/type").StreamEditLink("http://odata.org/editmedialink")
                            .NavigationProperty("NavProp", "http://odata.org/link")
                            .StreamProperty("StreamProperty", "http://odata.org/readlink", null, null, null)
                            .PrimitiveProperty("foo", "bar")
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry>{0}" +
                                "<link rel='edit-media' href='http://odata.org/editmedialink'/>{0}" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'/>{0}" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/readlink' m:etag='someetag'/>{0}" +
                                "<content src='http://odata.org/mr' type='mime/type'/>{0}" +
                                "<m:properties><d:foo>bar</d:foo></m:properties>{0}" +
                                "</entry>",
                                padding.Key)),
                        SkipTestConfiguration = tc => tc.IsRequest
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr").StreamContentType("mime/type").StreamEditLink("http://odata.org/editmedialink")
                            .NavigationProperty("NavProp", "http://odata.org/link")
                            .StreamProperty("StreamProperty", "http://odata.org/readlink", null, null, null)
                            .OperationDescriptor(new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/actionMetadata", Target = "http://odata.org/actionTarget" })
                            .PrimitiveProperty("foo", "bar")
                            .OperationDescriptor(new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/functionMetadata", Target = "http://odata.org/functionTarget" })
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry>{0}" +
                                "<link rel='edit-media' href='http://odata.org/editmedialink'/>{0}" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='http://odata.org/link' type='application/atom+xml;type=entry'/>{0}" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/readlink' m:etag='someetag'/>{0}" +
                                "<content src='http://odata.org/mr' type='mime/type'/>{0}" +
                                "<m:action metadata='http://odata.org/actionMetadata' target='http://odata.org/actionTarget'>{0}</m:action>{0}" +
                                "<m:properties><d:foo>bar</d:foo></m:properties>{0}" +
                                "<m:function metadata='http://odata.org/functionMetadata' target='http://odata.org/functionTarget'>{0}</m:function>{0}" +
                                "</entry>",
                                padding.Key)),
                        SkipTestConfiguration = tc => tc.IsRequest,
                    },
                }));

            // Add test cases with duplicate <m:properties> elements
            testDescriptors = testDescriptors.Concat(new PayloadReaderTestDescriptor[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry()
                        .XmlRepresentation("<entry><m:properties/><m:properties/></entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "properties"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry()
                        .XmlRepresentation("<entry><m:properties m:type='foo' m:null='true' type='some'/><m:properties/></entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "properties"),
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

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of atom:content element.")]
        public void EntryContentElementTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region Handling duplicates
                // The default behavior is to disallow duplicates of entry/content element defined in ODATA spec.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content/><content/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "content"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml'/><content/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "content"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/atom+xml'/><content/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "content"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content/><content type='application/json'/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "content"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/json'/><content/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithWrongType", "application/json"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content/><content type='application/json'/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "content"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                        .XmlRepresentation("<entry><content src='http://odata.org/mr'>   \r\n<!--some--><?value?></content><content/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                        .XmlRepresentation("<entry><content/><content src='http://odata.org/mr'>   \r\n<!--some--><?value?></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "content"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                // The WCF DS client behavior is to read all content elements and pick the last src attribute.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr3").XmlRepresentation(@"
                                        <entry>
                                          <content src='http://odata.org/mr1'/>
                                          <content src='http://odata.org/mr2'/>
                                          <content src='http://odata.org/mr3'/>
                                        </entry>"),
                    SkipTestConfiguration = (config) => {return config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesClient; }
                },
                // The WCF DS client behavior is to read all content elements and pick the last type attribute.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamContentType("CustomType/CustomSubType3").StreamSourceLink("http://odata.org/mr3").XmlRepresentation(@"
                                        <entry>
                                          <content type='CustomType/CustomSubType1' src='http://odata.org/mr1'/>
                                          <content type='CustomType/CustomSubType2' src='http://odata.org/mr2'/>
                                          <content type='CustomType/CustomSubType3' src='http://odata.org/mr3'/>
                                        </entry>"),
                    SkipTestConfiguration = (config) => {return config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesClient; }
                },
                #endregion

                // No attributes - missing content type
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content/></entry>"),
                },
                // Unrecognized attributes - missing content type
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content foo='bar' m:type='application/json'/></entry>"),
                },
                // No src and wrong type with default behavior
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/json'/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithWrongType", "application/json"),
                },
                // Empty content - empty element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml'/></entry>"),
                },
                // Empty content
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml'></content></entry>"),
                },
                // atom+xml content type
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/atom+xml'/></entry>"),
                },
                // content type containing MIME type with subtype
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml;type=entry'/></entry>"),
                },
                // content type containing MIME type with subtype
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/atom+xml;type=entry'/></entry>"),
                },
                // src without type -> MLE
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                        .XmlRepresentation("<entry><content src='http://odata.org/mr'/></entry>"),
                },
                // src with type -> MLE with content type
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr").StreamContentType("mime/type")
                        .XmlRepresentation("<entry><content src='http://odata.org/mr' type='mime/type'/></entry>"),
                },
                // src with type and additional attributes to ignore -> MLE with content type
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr").StreamContentType("mime/type")
                        .XmlRepresentation("<entry><content foo='bar' src='http://odata.org/mr' m:type='some' type='mime/type' m:null='true' d:foo='bar2'/></entry>"),
                },
                // src with empty type -> MLE with empty content type
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr").StreamContentType("")
                        .XmlRepresentation("<entry><content src='http://odata.org/mr' type=''/></entry>"),
                },
                // src with empty content
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                        .XmlRepresentation("<entry><content src='http://odata.org/mr'></content></entry>"),
                },
                // src with empty content with insignificant nodes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                        .XmlRepresentation("<entry><content src='http://odata.org/mr'>   \r\n<!--some--><?value?></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty"),
                },
                // src with content with significant nodes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                        .XmlRepresentation("<entry><content src='http://odata.org/mr'>text node</content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty"),
                },
                // src with content with element nodes
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                        .XmlRepresentation("<entry><content src='http://odata.org/mr'><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty"),
                },
                // Empty properties
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml'><m:properties/></content></entry>"),
                },

                #region plain text behavior
                // empty content type with non-empty text
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type=''>plain_text</content></entry>"),
                },
                // no content type with text as child
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content>plain_text</content></entry>"),
                },
                // no content type with some element as child
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content><foo/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                // no content type with some element as child
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content><foo></foo></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                // no content type with text and some element as children
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content>plain_text<foo></foo></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                // empty content type with text and some element as children
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type=''>plain_text<foo></foo></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                // empty content type with valid child element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type=''><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                // no content type with valid child element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                // invalid content type with valid child element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='plain/text'><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithWrongType", "plain/text"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                #endregion

                #region content type parse tests
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml;a=b;c=d'><m:properties/></content></entry>"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml;ab;c=d'><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeMissingParameterValue", "ab"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='a=b'><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSlash", "a=b"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type=';a=b'><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSlash", ";a=b"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml, application/json'><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_NoOrMoreThanOneContentTypeSpecified", "application/xml, application/json"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/*'></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_WildcardInContentType", "application/*"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='*/*'></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_WildcardInContentType", "*/*"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='*/xml'></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataContentTypeException("ODataMessageReader_WildcardInContentType", "*/xml"),
                },
                #endregion
            };

            testDescriptors = testDescriptors.Concat(paddings.SelectMany(padding =>
                new PayloadReaderTestDescriptor[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry><content type='application/xml'>{0}</content></entry>",
                                padding.Key)),
                        ExpectedException = getExpectedExceptionForPadding(padding.Value),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry><content type='application/xml'>{0}<m:properties/></content></entry>",
                                padding.Key)),
                        ExpectedException = getExpectedExceptionForPadding(padding.Value),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry><content type='application/xml'><m:properties/>{0}</content></entry>",
                                padding.Key)),
                        ExpectedException = getExpectedExceptionForPadding(padding.Value),
                    },
                }));

            Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>> normalizer =
                (tc) => (payloadElement => WcfDsServerPayloadElementNormalizer.Normalize(payloadElement, tc.Format, (EdmModel)null));

            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (behaviorKind, testDescriptor, testConfiguration) =>
                {
                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    // In server mode we need to normalize payload to not expect information that the server does not read
                    if (behaviorKind == TestODataBehaviorKind.WcfDataServicesServer && !testDescriptor.ExpectedResultNormalizers.Contains(normalizer))
                    {
                        testDescriptor.ExpectedResultNormalizers.Add(normalizer);
                    }

                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of atom:content element when type is invalid but parsable.")]
        public void EntryContentElementTest_InvalidMediaType()
        {
            // For WCF DS clients, atom:content elements with type (that are parsable) other than application/xml and application/atom+xml 
            // are considered as empty elements. WCF DS server throws if the media type is neither application/xml nor application/atom+xml.
            // The type should be parsable in both cases.
            string[] contentStrings = new string[] {
                "<entry><content type='plain/text'/></entry>", // no src and wrong (but parsable) type
                "<entry><content type='plain/text'><m:properties/></content></entry>", // invalid but parsable content type with valid child element
                "<entry><content type='plain/text'><m:properties/><d:Name>Foo</d:Name></content></entry>", // invalid but parsable content type with valid child element having valid grand children
                "<entry><content type='plain/text'>plain_text</content></entry>", // invalid but parsable content type with non-empty text
                "<entry><content type='plain/text'><foo/></content></entry>", // invalid but parsable content type with some element as child
                "<entry><content type='plain/text'>plain_text<foo></foo></content></entry>", // invalid but parsable content type with text and some element as child
            };

            var testDescriptors = contentStrings.Select(contentString =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation(contentString),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithWrongType", "plain/text"),
                });

            // Parsing errors should supercede invalid media type errors for all behavior kinds.
            testDescriptors = testDescriptors.Concat(new[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type=';a=b'><m:properties/></content></entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_MediaTypeRequiresSlash", ";a=b"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml, application/json'><m:properties/></content></entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataContentTypeException("HttpUtils_NoOrMoreThanOneContentTypeSpecified", "application/xml, application/json"),
                    },
                }
                );

            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (behaviorKind, testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of m:properties element.")]
        public void EntryPropertiesElementTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region Handling duplicates
                // Multiple properties elements
                // The default behavior is to disallow duplicates of entry/content/m:properties element defined in ODATA spec.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml'><m:properties/><m:properties/></content></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "properties"),
                    SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                },
                // Other error cases for MLE properties are in EntryMLERecognitionTest
                #endregion

                // Empty properties
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml'><m:properties/></content></entry>"),
                },
                // Empty properties with additional attributes (to be ignored)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().XmlRepresentation("<entry><content type='application/xml'><m:properties m:type='foo' m:null='true' type='some'/></content></entry>"),
                },
                // Empty properties MLE
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().XmlRepresentation("<entry><m:properties/></entry>"),
                },
                // Empty properties with additional attributes (to be ignored)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().XmlRepresentation("<entry><m:properties m:type='foo' m:null='true' type='some'/></entry>"),
                },
            };

            // Add test cases with additional nodes before and/or after <m:properties>
            testDescriptors = testDescriptors.Concat(paddings.SelectMany(padding =>
                new PayloadReaderTestDescriptor[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry><content type='application/xml'>{0}<m:properties/></content></entry>",
                                padding.Key)),
                        ExpectedException = getExpectedExceptionForPadding(padding.Value),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<entry><content type='application/xml'><m:properties/>{0}</content></entry>",
                                padding.Key)),
                        ExpectedException = getExpectedExceptionForPadding(padding.Value),
                    },
                }));

            // Add padding tests
            testDescriptors = testDescriptors.Concat(
                PropertiesElementAtomValues.CreatePropertiesElementPaddingPayloads<EntityInstance>(
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity(),
                    },
                    (entityInstance, xmlValue) => entityInstance.XmlRepresentation("<entry><content type='application/xml'><m:properties>" + xmlValue + "</m:properties></content></entry>")));

            // Add padding tests - MLE
            testDescriptors = testDescriptors.Concat(
                PropertiesElementAtomValues.CreatePropertiesElementPaddingPayloads<EntityInstance>(
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry(),
                    },
                    (entityInstance, xmlValue) => entityInstance.XmlRepresentation("<entry><m:properties>" + xmlValue + "</m:properties></entry>")));

            // Add ordering tests
            testDescriptors = testDescriptors.Concat(
                PropertiesElementAtomValues.CreatePropertiesElementOrderingPayloads<EntityInstance>(
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity(),
                    },
                    (entityInstance, xmlNodes) => entityInstance.XmlRepresentation(
                        new XElement(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName,
                            new XElement(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomContentElementName,
                                new XAttribute("type", "application/xml"),
                                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomPropertiesElementName,
                                    xmlNodes))))));

            // Add ordering tests - MLE
            testDescriptors = testDescriptors.Concat(
                PropertiesElementAtomValues.CreatePropertiesElementOrderingPayloads<EntityInstance>(
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry(),
                    },
                    (entityInstance, xmlNodes) => entityInstance.XmlRepresentation(
                        new XElement(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomEntryElementName,
                            new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomPropertiesElementName,
                                xmlNodes)))));
            // WCF DS client behavior is to pick the first m:properties from each of entry/content elements.
            EntityInstance element = PayloadBuilder.Entity().XmlRepresentation(@"
  <entry>
    <content type='application/xml'>
        <m:properties>
          <d:ID m:type='Edm.Int32'>1</d:ID>
          <d:More1 m:type='Edm.Int32'>2</d:More1>
        </m:properties>
        <m:properties>
          <d:More1 m:type='Edm.Int32'>2</d:More1>
        </m:properties>
    </content>
    <content type='' />
    <content />
    <content type='application/json'>
        <m:properties>
          <d:Name m:type='Edm.String'>TestName</d:Name>
          <d:More1 m:type='Edm.Int32'>3</d:More1>
          <d:More2 m:type='Edm.Int32'>3</d:More2>
        </m:properties>
        <m:properties>
          <d:More2 m:type='Edm.Int32'>4</d:More2>
        </m:properties>
    </content>
    <content>
        <m:properties>
          <d:Name m:type='Edm.String'>TestName</d:Name>
          <d:More1 m:type='Edm.Int32'>3</d:More1>
          <d:More2 m:type='Edm.Int32'>3</d:More2>
        </m:properties>
        <m:properties>
          <d:More2 m:type='Edm.Int32'>4</d:More2>
        </m:properties>
    </content>
    <content type='application/xml'>
        <m:properties>
          <d:Name m:type='Edm.String'>TestName</d:Name>
          <d:More1 m:type='Edm.Int32'>3</d:More1>
          <d:More2 m:type='Edm.Int32'>3</d:More2>
        </m:properties>
        <m:properties>
          <d:More2 m:type='Edm.Int32'>4</d:More2>
        </m:properties>
    </content>
  </entry>");
            element.Add(PayloadBuilder.PrimitiveProperty("ID", 1));
            element.Add(PayloadBuilder.PrimitiveProperty("More1", 2));
            element.Add(PayloadBuilder.PrimitiveProperty("Name", "TestName"));
            element.Add(PayloadBuilder.PrimitiveProperty("More1", 3));
            element.Add(PayloadBuilder.PrimitiveProperty("More2", 3));
            testDescriptors = testDescriptors.ConcatSingle(new PayloadReaderTestDescriptor(this.Settings)
            {
                PayloadElement = element,
                SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesClient; },
                ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "properties")
            });

            // WCF DS client behavior is to take all the entry/m:properties elements.
            element = PayloadBuilder.Entity().AsMediaLinkEntry().XmlRepresentation(@"
  <entry>
    <m:properties>
        <d:ID m:type='Edm.Int32'>1</d:ID>
        <d:More1 m:type='Edm.Int32'>2</d:More1>
    </m:properties>
    <m:properties>
        <d:More1 m:type='Edm.Int32'>2</d:More1>
    </m:properties>
    <m:properties>
        <d:Name m:type='Edm.String'>TestName</d:Name>
        <d:More1 m:type='Edm.Int32'>3</d:More1>
        <d:More2 m:type='Edm.Int32'>3</d:More2>
    </m:properties>
    <m:properties>
        <d:More2 m:type='Edm.Int32'>4</d:More2>
    </m:properties>
  </entry>");
            element.Add(PayloadBuilder.PrimitiveProperty("ID", 1));
            element.Add(PayloadBuilder.PrimitiveProperty("More1", 2));
            element.Add(PayloadBuilder.PrimitiveProperty("More1", 2));
            element.Add(PayloadBuilder.PrimitiveProperty("Name", "TestName"));
            element.Add(PayloadBuilder.PrimitiveProperty("More1", 3));
            element.Add(PayloadBuilder.PrimitiveProperty("More2", 3));
            element.Add(PayloadBuilder.PrimitiveProperty("More2", 4));
            testDescriptors = testDescriptors.ConcatSingle(new PayloadReaderTestDescriptor(this.Settings)
            {
                PayloadElement = element,
                SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesClient; }
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

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct recognition of MLEs.")]
        public void EntryMLERecognitionTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                behaviorKind =>
                {
                    IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
                    {
                        #region Handling duplicates
                        // The default behavior is to disallow duplicates of entry/m:properties element defined in ODATA spec.
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry()
                                .XmlRepresentation("<entry>" +
                                    "  <m:properties/>" +
                                    "  <m:properties/>" +
                                    "</entry>"),
                            ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "properties")
                                : null,
                        },
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <content type='application/xml'/>" +
                                    "  <m:properties/>" +
                                    "  <m:properties/>" +
                                    "</entry>"),
                            ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                        },
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <m:properties/>" +
                                    "  <m:properties/>" +
                                    "  <content type='application/xml'/>" +
                                    "</entry>"),
                            ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "properties")
                                : ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                        },
                        #endregion

                        // m:properties outside -> MLE
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry()
                                .XmlRepresentation("<entry>" +
                                    "  <m:properties/>" +
                                    "</entry>"),
                        },
                        // m:properties outside + content with src -> MLE
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                                .XmlRepresentation("<entry>" +
                                    "  <m:properties/>" +
                                    "  <content src='http://odata.org/mr'/>" +
                                    "</entry>"),
                        }, 
                        // content without src -> non-MLE
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <content type='application/xml'/>" +
                                    "</entry>"),
                        },
                        // edit-media link -> MLE
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "</entry>"),
                        },
                        // m:properties outside but content without src -> error
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <content type='application/xml'/>" +
                                    "  <m:properties/>" +
                                    "</entry>"),
                            ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                        },
                        // m:properties outside but content without src (properties first) -> error
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <m:properties/>" +
                                    "  <content type='application/xml'/>" +
                                    "</entry>"),
                            ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                        },
                        // m:properties inside but edit-media link present -> error
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <content type='application/xml'><m:properties/></content>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "</entry>"),
                            ExpectedException = 
                                // content/@src is missing and in server mode links are ignored and therefore there is no exception on server
                                behaviorKind == TestODataBehaviorKind.WcfDataServicesServer
                                    ? null
                                    : ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                        },
                        // m:properties inside but edit-media link present (link first) -> error
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "  <content type='application/xml'><m:properties/></content>" +
                                    "</entry>"),
                            ExpectedException = 
                                // content/@src is missing and in server mode links are ignored and therefore there is no exception on server
                                behaviorKind == TestODataBehaviorKind.WcfDataServicesServer
                                    ? null
                                    : ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                        },
                    };

                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    // In server mode we need to normalize payload to not expect information that the server does not read
                    if (behaviorKind == TestODataBehaviorKind.WcfDataServicesServer)
                    {
                        foreach (PayloadReaderTestDescriptor td in testDescriptors)
                        {
                            td.ExpectedResultNormalizers.Add((tc) => (payloadElement => WcfDsServerPayloadElementNormalizer.Normalize(tc.Format, (EdmModel)td.PayloadEdmModel, payloadElement)));
                        }
                    }

                    this.CombinatorialEngineProvider.RunCombinations(
                        testDescriptors,
                        this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                        (testDescriptor, testConfiguration) =>
                        {
                            testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                        });
                });
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verifies correct recognition of MLEs when model is available.")]
        public void EntryMLERecognitionWithMetadataTest()
        {
            IEdmModel model = TestModels.BuildTestModel();
            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                behaviorKind =>
                {
                    IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
                    {
                        // m:properties outside -> MLE, model MLE as well
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityWithMapType").AsMediaLinkEntry()
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityWithMapType'/>" +
                                    "  <m:properties/>" +
                                    "</entry>"),
                            PayloadEdmModel = model,
                        },
                        // m:properties outside -> MLE, model non-MLE
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").AsMediaLinkEntry()
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityType'/>" +
                                    "  <m:properties/>" +
                                    "</entry>"),
                            PayloadEdmModel = model,
                            ExpectedException = behaviorKind == TestODataBehaviorKind.WcfDataServicesClient
                                ? null
                                : ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithMediaResourceAndNonMLEType", "TestModel.CityType"),
                        },
                        // m:properties outside + content with src -> MLE, model MLE as well
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityWithMapType").AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityWithMapType'/>" +
                                    "  <m:properties/>" +
                                    "  <content src='http://odata.org/mr'/>" +
                                    "</entry>"),
                            PayloadEdmModel = model,
                        },
                        // m:properties outside + content with src -> MLE, model non-MLE
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityType'/>" +
                                    "  <m:properties/>" +
                                    "  <content src='http://odata.org/mr'/>" +
                                    "</entry>"),
                            PayloadEdmModel = model,
                            ExpectedException = behaviorKind == TestODataBehaviorKind.WcfDataServicesClient
                                ? null
                                : ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithMediaResourceAndNonMLEType", "TestModel.CityType"),
                        },
                        // content without src -> non-MLE, non-MLE model
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityType'/>" +
                                    "  <content type='application/xml'/>" +
                                    "</entry>"),
                            PayloadEdmModel = model,
                        },
                        // content without src -> non-MLE, MLE model
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityWithMapType")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityWithMapType'/>" +
                                    "  <content type='application/xml'/>" +
                                    "</entry>"),
                            PayloadEdmModel = model, 
                            ExpectedException = behaviorKind == TestODataBehaviorKind.WcfDataServicesClient
                                ? null
                                : ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithoutMediaResourceAndMLEType", "TestModel.CityWithMapType"),
                        },
                        // edit-media link -> MLE, MLE model
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityWithMapType").AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityWithMapType'/>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "</entry>"),
                            PayloadEdmModel = model,
                        },
                        // edit-media link -> MLE, model non-MLE
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityType'/>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "</entry>"),
                            PayloadEdmModel = model,
                            ExpectedException = 
                                // content/@src is missing and in server mode links are ignored and therefore there is no exception on server
                                behaviorKind == TestODataBehaviorKind.WcfDataServicesClient || behaviorKind == TestODataBehaviorKind.WcfDataServicesServer
                                    ? null
                                    : ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithMediaResourceAndNonMLEType", "TestModel.CityType"),
                        },
                        // m:properties outside but content without src -> error
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityType'/>" +
                                    "  <content type='application/xml'/>" +
                                    "  <m:properties/>" +
                                    "</entry>"),
                            ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                            PayloadEdmModel = model,
                        },
                        // m:properties outside but content without src (properties first) -> error
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityWithMapType'/>" +
                                    "  <m:properties/>" +
                                    "  <content type='application/xml'/>" +
                                    "</entry>"),
                            ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                            PayloadEdmModel = model,
                        },
                        // m:properties inside but edit-media link present -> error, MLE model
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityWithMapType").AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityWithMapType'/>" +
                                    "  <content type='application/xml'><m:properties/></content>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "</entry>"),
                            ExpectedException = 
                                // content/@src not present and links ignored - entry.MediaResource not created
                                behaviorKind == TestODataBehaviorKind.WcfDataServicesServer ? ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithoutMediaResourceAndMLEType", "TestModel.CityWithMapType") :
                                         ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                            PayloadEdmModel = model,
                        },
                        // m:properties inside but edit-media link present -> error, non-MLE model
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityType'/>" +
                                    "  <content type='application/xml'><m:properties/></content>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "</entry>"),
                            ExpectedException = 
                                // content/@src is missing and in server mode links are ignored and therefore there is no exception on server
                                behaviorKind == TestODataBehaviorKind.WcfDataServicesServer ? null :
                                ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                            PayloadEdmModel = model,
                        },
                        // m:properties inside but edit-media link present (link first) -> error, MLE model
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityWithMapType").AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityWithMapType'/>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "  <content type='application/xml'><m:properties/></content>" +
                                    "</entry>"),
                            ExpectedException = 
                                // content/@src not present and links ignored - entry.MediaResource not created
                                behaviorKind == TestODataBehaviorKind.WcfDataServicesServer ? ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithoutMediaResourceAndMLEType", "TestModel.CityWithMapType") :
                                         ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                            PayloadEdmModel = model,
                        },
                        // m:properties inside but edit-media link present (link first) -> error, non-MLE model
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity("TestModel.CityType").AsMediaLinkEntry().StreamEditLink("http://odata.org")
                                .XmlRepresentation("<entry>" +
                                    "  <category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.CityType'/>" +
                                    "  <link rel='edit-media' href='http://odata.org'/>" +
                                    "  <content type='application/xml'><m:properties/></content>" +
                                    "</entry>"),
                            ExpectedException = 
                                // content/@src is missing and in server mode links are ignored and therefore there is no exception on server
                                behaviorKind == TestODataBehaviorKind.WcfDataServicesServer
                                ? null
                                : ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                            PayloadEdmModel = model,
                        },
                    };

                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    // In server mode we need to normalize payload to not expect information that the server does not read
                    if (behaviorKind == TestODataBehaviorKind.WcfDataServicesServer)
                    {
                        foreach (PayloadReaderTestDescriptor td in testDescriptors)
                        {
                            td.ExpectedResultNormalizers.Add((tc) => (payloadElement => WcfDsServerPayloadElementNormalizer.Normalize(tc.Format, (EdmModel)td.PayloadEdmModel, payloadElement)));
                        }
                    }

                    this.CombinatorialEngineProvider.RunCombinations(
                        testDescriptors,
                        this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                        (testDescriptor, testConfiguration) =>
                        {
                            testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                        });
                });
        }

        [TestMethod, TestCategory("Reader.Entries"), Variation(Description = "Verify correct reading of MLEs")]
        public void MediaLinkEntryTest()
        {
            var model = TestModels.BuildTestModel();
            var cityWithMapType = model.EntityTypes().FirstOrDefault(entryType => entryType.FullName().Equals("TestModel.CityWithMapType"));

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {                
                #region ATOM Tests.
                // no content, empty properties - pass as we set the MediaLinkEntry to true when we see entry/properties. Since there is no entry/content 
                // element we dont try to set the MediaLinkEntry again.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry()
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<m:properties/>" +
                                                   "</entry>"),
                    PayloadEdmModel = model,
                },
                // empty content, no properties - this should fail as we set the MediaLinkEntry flag to false when reading entry/content. So, we dont 
                // consider setting this to true, and hence don't create a MediaResource even if we see the entity is a MLE.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry()
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<content/>" +
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithoutMediaResourceAndMLEType", "TestModel.CityWithMapType"),
                },
                // empty content, empty properties - fail because we have entry/properties, but entry/content does not have src.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry()
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<m:properties/>" +
                                                   "<content/>" +
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                },
                // content with src, empty properties - pass
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<m:properties/>" +
                                                   "<content src='http://odata.org/mr'/>" +
                                                   "</entry>"),
                    PayloadEdmModel = model,
                },
                // content with src, no properties - pass
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr")
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<content src='http://odata.org/mr'/>" +
                                                   "</entry>"),
                    PayloadEdmModel = model,
                },
                // empty entry/properties with only content/@type - fail because entry/content does not have src.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry()
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<m:properties/>" +
                                                   "<content type='application/xml'/>" + 
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                },
                // no entry/properties with only content/@type - fail because we declare the entry to be non-MLE when the content type (without src) is parsed.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry()
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<content type='application/xml'/>" + 
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithoutMediaResourceAndMLEType"),
                },
                // only content/@type and content/@src
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr").StreamContentType("application/xml")
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<content type='application/xml' src='http://odata.org/mr'/>" + 
                                                   "</entry>"),
                    PayloadEdmModel = model,
                },
                // empty entry/properties with content/@type and content/some_child - fail because entry/content does not have src.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry()
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<m:properties/>" +
                                                   "<content type='application/xml'><d:Name>Foo</d:Name></content>" + 
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                },                
                // empty entry/properties with content/@type, content/@src and content/some_child
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr").StreamContentType("application/xml")
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<m:properties/>" +
                                                   "<content type='application/xml' src='http://odata.org/mr'><d:Name>Foo</d:Name></content>" + 
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty"),
                },
                // empty properties with content/@type, and content/properties - fail because entry/content does not have src.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry()
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<m:properties/>" +
                                                   "<content type='application/xml'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomReader_MediaLinkEntryMismatch"),
                },
                // Only content/@type, and content/properties - fail because we declare the entry to be non-MLE when the content type (without src) is parsed.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry()
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<content type='application/xml'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_EntryWithoutMediaResourceAndMLEType"),
                },
                // empty properties with content/@type, content/@src and content/properties
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity(cityWithMapType.FullName()).AsMediaLinkEntry().StreamSourceLink("http://odata.org/mr").StreamContentType("application/xml")
                                                   .XmlRepresentation(
                                                   "<entry>" +
                                                   "<category term='TestModel.CityWithMapType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                                   "<content type='application/xml' src='http://odata.org/mr'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                                                   "</entry>"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_ContentWithSourceLinkIsNotEmpty"),
                },
                #endregion
            };

            this.CombinatorialEngineProvider.RunCombinations(
            testDescriptors,
            this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
            (testDescriptor, testConfiguration) =>
            {
                testDescriptor.RunTest(testConfiguration);
            });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of id on an entry.")]
        public void EntryIDTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                (behaviorKind) =>
                {
                    IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                        CreateIdTestDescriptors(
                            this.Settings,
                            PayloadBuilder.Entity(),
                            (entity, id) => entity.Id(id),
                            "<entry>{0}</entry>",
                            behaviorKind);

                    // The WCF DS client behavior is to allow entry/id duplicates, reading only the first element.
                    testDescriptors = testDescriptors.ConcatSingle(new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().Id("urn:id2").XmlRepresentation("<entry><id>urn:id1</id><id>urn:id2</id></entry>"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesClient; }
                    });

                    this.CombinatorialEngineProvider.RunCombinations(
                        testDescriptors,
                        this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                        (testDescriptor, testConfiguration) =>
                        {
                            testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                        });
                });
        }

        private IEnumerable<PayloadReaderTestDescriptor> StandardLinkClientDuplicateTestDescriptors(string relation)
        {
            string link = "http://odata.org/link1";
            ODataPayloadElement element = null;
            EntityInstance elementWithNullLink = PayloadBuilder.Entity();

            if (relation == "edit")
            {
                element = PayloadBuilder.Entity().WithEditLink(link);
            }
            else if (relation == "edit-media")
            {
                element = PayloadBuilder.Entity().AsMediaLinkEntry().StreamEditLink(link);
                elementWithNullLink = elementWithNullLink.AsMediaLinkEntry();
            }
            else
            {
                element = PayloadBuilder.Entity().WithSelfLink(link);
            }

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Entity with multiple read links
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = element.DeepCopy().XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                            @"<entry>
                                <link rel='{0}' href='http://odata.org/link1' />
                                <link rel='{0}' href='http://odata.org/link2' />
                              </entry>", relation)),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", relation),
                    SkipTestConfiguration = (config) => {return config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesClient; }
                },
                // Entity with multiple read links (first element has an invalid Uri)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = element.DeepCopy().XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                            @"<entry>
                                <link rel='{0}' href='http://' />
                                <link rel='{0}' href='http://odata.org/link1' />
                              </entry>", relation)),
                    ExpectedException = new ExpectedException(typeof(System.UriFormatException)),
                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    SkipTestConfiguration = config => config.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesServer
                },

                // Entity with multiple read links (first element has an invalid Uri)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = element.DeepCopy().XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                            @"<entry>
                                <link rel='{0}' href='http://' />
                                <link rel='{0}' href='http://odata.org/link1' />
                              </entry>", relation)),
                    ExpectedException = null,
                    SkipTestConfiguration = config => config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesServer,
                    ExpectedResultNormalizers = new List<Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>>>() 
                        { 
                            (tc) => (payloadElement => WcfDsServerPayloadElementNormalizer.Normalize(payloadElement, tc.Format, (EdmModel)null))
                        }
                },
            };

            return testDescriptors;
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of self and edit link on an entry.")]
        public void SelfAndEditLinkTest()
        {
            // Self links
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                CreateLinkTestDescriptors(
                    this.Settings,
                    PayloadBuilder.Entity(),
                    (entity, link) => entity.WithSelfLink(link),
                    "self",
                    "<entry>{0}</entry>");

            // Edit links
            testDescriptors = testDescriptors.Concat(
                CreateLinkTestDescriptors(
                    this.Settings,
                    PayloadBuilder.Entity(),
                    (entity, link) => entity.WithEditLink(link),
                    "edit",
                    "<entry>{0}</entry>"));

            #region Multiple read links tests
            testDescriptors = testDescriptors.Concat(new PayloadReaderTestDescriptor[]
                {
                    // Entity with multiple read links
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .WithSelfLink("http://odata.org/self2")
                            .XmlRepresentation(
                                @"<entry>
                                    <link rel='self' href='http://odata.org/self1' />
                                    <link rel='self' href='http://odata.org/self2' />
                                    </entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "self"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                    },
                    // Entity with multiple read links (first one has no href)
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation(
                                @"<entry>
                                    <link rel='self' />
                                    <link rel='self' href='http://odata.org/self2' />
                                    </entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "self"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                    },
                    // Entity with multiple read links (second one has no href)
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .WithSelfLink("http://odata.org/self1")
                            .XmlRepresentation(
                                @"<entry>
                                    <link rel='self' href='http://odata.org/self1' />
                                    <link rel='self' />
                                    </entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "self"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                    },
                    // Entity with multiple read links (first and second have no href)
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .WithSelfLink("http://odata.org/self1")
                            .XmlRepresentation(
                                @"<entry>
                                    <link rel='self' />
                                    <link rel='self' />
                                    </entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "self"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                    },
                });
            #endregion Multiple read links tests

            #region Multiple edit links tests
            testDescriptors = testDescriptors.Concat(new PayloadReaderTestDescriptor[]
                {
                    // Entity with multiple read links
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .WithSelfLink("http://odata.org/edit2")
                            .XmlRepresentation(
                                @"<entry>
                                    <link rel='edit' href='http://odata.org/edit1' />
                                    <link rel='edit' href='http://odata.org/edit2' />
                                    </entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "edit"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                    },
                    // Entity with multiple edit links (first one has no href)
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .XmlRepresentation(
                                @"<entry>
                                    <link rel='edit' />
                                    <link rel='edit' href='http://odata.org/edit2' />
                                    </entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "edit"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                    },
                    // Entity with multiple edit links (second one has no href)
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .WithSelfLink("http://odata.org/edit1")
                            .XmlRepresentation(
                                @"<entry>
                                    <link rel='edit' href='http://odata.org/edit1' />
                                    <link rel='edit' />
                                    </entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "edit"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                    },
                    // Entity with multiple edit links (first and second have no href)
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                            .WithSelfLink("http://odata.org/edit1")
                            .XmlRepresentation(
                                @"<entry>
                                    <link rel='edit' />
                                    <link rel='edit' />
                                    </entry>"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "edit"),
                        SkipTestConfiguration = (config) => { return config.RunBehaviorKind != TestODataBehaviorKind.Default; },
                    },
                });
            #endregion Multiple edit links tests

            // The WCF DS client behavior is to allow standard link element duplicates choosing just the first one.
            testDescriptors = testDescriptors.Concat(StandardLinkClientDuplicateTestDescriptors("edit"));
            testDescriptors = testDescriptors.Concat(StandardLinkClientDuplicateTestDescriptors("edit-media"));
            testDescriptors = testDescriptors.Concat(StandardLinkClientDuplicateTestDescriptors("self"));

            Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>> normalizer =
                (tc) => (payloadElement => WcfDsServerPayloadElementNormalizer.Normalize(payloadElement, tc.Format, (EdmModel)null));

            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (behaviorKind, testDescriptor, testConfiguration) =>
                {
                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    // In server mode we need to normalize payload to not expect information that the server does not read
                    if (behaviorKind == TestODataBehaviorKind.WcfDataServicesServer && !testDescriptor.ExpectedResultNormalizers.Contains(normalizer))
                    {
                        testDescriptor.ExpectedResultNormalizers.Add(normalizer);
                    }

                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of edit-media link on an entry.")]
        public void EditMediaLinkTest()
        {
            // First use standard link payloads
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                CreateLinkTestDescriptors(
                    this.Settings,
                    PayloadBuilder.Entity(),
                    (entity, link) => entity.AsMediaLinkEntry().StreamEditLink(link),
                    "edit-media",
                    "<entry>{0}</entry>",
                    includeNoOrEmptyHref: false);

            // Add more specific paylods for handling of etag
            testDescriptors = testDescriptors.Concat(new PayloadReaderTestDescriptor[]
            {
                // Just etag - valid for readers
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamETag("etag")
                        .XmlRepresentation("<entry><link rel='edit-media' m:etag='etag'/></entry>")
                },
                // Both etag and edit link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamETag("etag").StreamEditLink("http://odata.org/mredit")
                        .XmlRepresentation("<entry><link m:etag='etag' rel='http://www.iana.org/assignments/relation/edit-media' href='http://odata.org/mredit'/></entry>")
                },
                // No etag nor edit link - valid for readers
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry()
                        .XmlRepresentation("<entry><link rel='edit-media'/></entry>")
                },
                // Link with empty href - invalid for readers without base URI
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry()
                        .XmlRepresentation("<entry><link rel='edit-media' href=''/></entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", string.Empty),
                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't - server ignores most of links
                    SkipTestConfiguration = config => config.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesServer
                },

                #region Multiple edit-media links tests
                // Entity with multiple read links
                // The default behavior is to disallow duplicates of entry/link[@rel='edit-media'] element defined in ODATA spec.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamEditLink("http://odata.org/edit1")
                        .XmlRepresentation(
                            @"<entry>
                                <link rel='edit-media' href='http://odata.org/edit1' />
                                <link rel='edit-media' href='http://odata.org/edit2' />
                              </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "edit-media"),
                },
                // Entity with multiple edit links (first one has no href)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamEditLink("http://odata.org/edit1")
                        .XmlRepresentation(
                            @"<entry>
                                <link rel='edit-media' />
                                <link rel='edit-media' href='http://odata.org/edit2' />
                              </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "edit-media"),
                },
                // Entity with multiple edit links (second one has no href)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamEditLink("http://odata.org/edit1")
                        .XmlRepresentation(
                            @"<entry>
                                <link rel='edit-media' href='http://odata.org/edit1' />
                                <link rel='edit-media' />
                              </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "edit-media"),
                },
                // Entity with multiple edit links (first and second have no href)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamEditLink("http://odata.org/edit1")
                        .XmlRepresentation(
                            @"<entry>
                                <link rel='edit-media' />
                                <link rel='edit-media' />
                              </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInEntry", "edit-media"),
                },
                #endregion Multiple edit-media links tests
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Special ATOM payloads for properties in an entry.")]
        public void EntryPropertyTest()
        {
            IEdmModel model = TestModels.BuildTestModel();

            var testCases = new[]
            {
                // This caused Debug.Assert in BufferingXmlReader.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .Id("http://temp.org/85b04a13-02e3-49e3-b285-5761ffbf3379")
                        .Property("GeographyProperty1", PayloadBuilder.PrimitiveValue(GeographyFactory.Point(32.0, -100.0).Build()))
                        .Property("ID", PayloadBuilder.PrimitiveValue(1)),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },

                // Primitive property declared as navigation singleton.
                // This cased Assert in ODataAtomPropertyAndValueDeserializer.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("PoliceStation", "test"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties", "PoliceStation", "TestModel.CityType")
                },
                // Primitive property declared as navigation collection.
                // This cased Assert in ODataAtomPropertyAndValueDeserializer.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("CityHall", "test"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties", "CityHall", "TestModel.CityType")
                },

                // Primitive property declared as navigation singleton.
                // This cased Assert in ODataAtomPropertyAndValueDeserializer.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("PoliceStation", "test"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties", "PoliceStation", "TestModel.CityType")
                },
                // Primitive property declared as navigation collection.
                // This cased Assert in ODataAtomPropertyAndValueDeserializer.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("CityHall", "test"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties", "CityHall", "TestModel.CityType")
                },
                // Invalid property name ('.' is a reserved character)
                // NOTE: we are not testing the other invalid characters ('@', ':') since the Xml reader will already throw
                //       and there is no reasonable way to produce such a payload in the test infrastructure.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityOpenType").PrimitiveProperty("Invalid.Name", "test"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertiesMustNotContainReservedChars", "Invalid.Name", "':', '.', '@'")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityOpenType").PrimitiveProperty("InvalidName.", "test"),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertiesMustNotContainReservedChars", "InvalidName.", "':', '.', '@'")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
               testCases,
               this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
               (testDescriptor, testConfiguration) =>
               {
                   testDescriptor.RunTest(testConfiguration);
               });
        }

        /// <summary>
        /// Creates list of interesting link test descriptors for ATOM reader.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to work with.</typeparam>
        /// <param name="settings">The payload reader test descriptor settings to use.</param>
        /// <param name="payloadElement">The payload element to start with.</param>
        /// <param name="setLink">Func which sets the expected value for a link, the first parameter is the payload element, the second is the link value
        /// the func should return a payload element after the link is applied.</param>
        /// <param name="relName">The rel name to use for the link.</param>
        /// <param name="xmlRepresentationTemplate">A template with one {0} which prepares the xml representation for the payload element. The method
        /// will inject atom link element into the {0}.</param>
        /// <param name="includeNoOrEmptyHref">true if test cases which don't have href or href is empty should be included.</param>
        /// <returns>List of interesting link payloads.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateLinkTestDescriptors<T>(
            PayloadReaderTestDescriptor.Settings settings,
            T payloadElement,
            Func<T, string, T> setLink,
            string relName,
            string xmlRepresentationTemplate,
            bool includeNoOrEmptyHref = true) where T : ODataPayloadElement
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Simple link
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setLink(payloadElement.DeepCopy(), "http://odata.org/link")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"' href='http://odata.org/link'/>")),
                },
                // Link with IANA rel
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setLink(payloadElement.DeepCopy(), "http://odata.org/link")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link' rel='http://www.iana.org/assignments/relation/" + relName +"'/>")),
                },
                // Link with content - should be ignored
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setLink(payloadElement.DeepCopy(), "http://odata.org/link")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"' href='http://odata.org/link'>foo <m:inline><entry/></m:inline><!--comment--></link>")),
                },
                // Link with child elements - should be ignored
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setLink(payloadElement.DeepCopy(), "http://odata.org/link")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"' href='http://odata.org/link'><child/></link>")),
                },
                // Link with text node only - should be ignored
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setLink(payloadElement.DeepCopy(), "http://odata.org/link")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"' href='http://odata.org/link'>some text</link>")),
                },
                // Link with additional attributes (title and type ignored here since ATOM metadata reading is off, but read and tested in EntryAtomMetadataTests.cs)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setLink(payloadElement.DeepCopy(), "http://odata.org/link")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link title='some' href='http://odata.org/link' type='application/json' rel='" + relName +"' m:type='Edm.String'/>")),
                },
                // Link with wrong IANA rel
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link' rel='http://www.iana.org/ASsignments/relation/" + relName +"'/>")),
                },
                // Link with wrong rel
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link' rel='" + relName.ToUpperInvariant() +"'/>")),
                },
                // Link without rel
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link'/>")),
                },
                // Link with attributes in the wrong namespace
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link m:rel='" + relName +"' m:href='http://odata.org/link'/>")),
                },
                // Link with attributes in the wrong namespace (another namespace)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = payloadElement.DeepCopy()
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link atom:rel='" + relName +"' atom:href='http://odata.org/link' xmlns:atom='http://www.w3.org/2005/Atom'/>")),
                },
            };

            // The default behavior is to disallow duplicates of feed/link[@rel='next'] element defined in ODATA spec.
            if (payloadElement.ElementType == ODataPayloadElementType.EntitySetInstance && relName.Equals("next"))
            {
                testDescriptors = testDescriptors.Concat(new PayloadReaderTestDescriptor[]
                {
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = payloadElement.DeepCopy()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"'/><link rel='" + relName +"'/>")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                    },
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = payloadElement.DeepCopy()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='' rel='" + relName +"'/><link href='' rel='" + relName +"'/>")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", string.Empty)
                    },
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = payloadElement.DeepCopy()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"'/><link href='' rel='" + relName +"'/>")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                    },
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = setLink(payloadElement.DeepCopy(), "http://odata.org/link")
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"'/><link href='http://odata.org/link' rel='" + relName +"'/>")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                    },
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = setLink(payloadElement.DeepCopy(), "http://odata.org/link")
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link' rel='" + relName +"'/><link rel='" + relName +"'/>")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                    },
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = payloadElement.DeepCopy()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link' rel='" + relName +"'/><link href='http://odata.org/link' rel='" + relName +"'/>")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                    },
                });
            }

            if (includeNoOrEmptyHref)
            {
                testDescriptors = testDescriptors.Concat(new PayloadReaderTestDescriptor[]
                {
                    // Link without href - as if doesn't exist
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = payloadElement.DeepCopy()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"'/>")),
                    },
                    // Link with empty href - error without a base URI
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = payloadElement.DeepCopy()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"' href=''/>")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", string.Empty),
                        // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                        SkipTestConfiguration = config => config.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesServer
                    },
                });
            }

            return testDescriptors;
        }

        /// <summary>
        /// Creates list of interesting tests for atom:id element.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to apply the ID to.</typeparam>
        /// <param name="settings">The payload reader test settings to use.</param>
        /// <param name="payloadElement">The payload element to apply the ID to.</param>
        /// <param name="setId">Func which sets the ID. The first parameter is the payload element to set the ID to, the second parameter is the ID value
        /// and the func should return a payload element after the ID is applied to it.</param>
        /// <param name="xmlRepresentationTemplate">A string template to use as XML representation of the payload element. The {0} in the string will be replaced
        /// with the atom:id element to test.</param>
        /// <returns>Enumeration of interesting atom:id tests.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateIdTestDescriptors<T>(
            PayloadReaderTestDescriptor.Settings settings,
            T payloadElement,
            Func<T, string, T> setId,
            string xmlRepresentationTemplate,
            TestODataBehaviorKind behaviorKind) where T : ODataPayloadElement
        {
            string emptyIdRepresentation;
            if (payloadElement.ElementType == ODataPayloadElementType.EntitySetInstance)
            {
                // On feeds the readers report string.Empty for <id /> or <id></id>
                // (since we report what is on the wire in these cases; chaining is 
                // not possible since the payload is invalid)
                emptyIdRepresentation = string.Empty;
            }
            else
            {
                Debug.Assert(
                    payloadElement.ElementType == ODataPayloadElementType.EntityInstance,
                    "Only entity sets and entities are supported.");

                // On entries the readers report 'null' for <id /> or <id></id>.
                // (since we need to support entries without IDs for insert scenarios
                // and using string.Empty here as well would prevent chaining).
                emptyIdRepresentation = null;
            }

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Simple valid id
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setId(payloadElement.DeepCopy(), "urn:id")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<id>urn:id</id>")),
                },
                // Simple valid id with additional attributes
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setId(payloadElement.DeepCopy(), "urn:id")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<id type='foo' m:type='Edm.Int16' d:prop='some'>urn:id</id>")),
                },
                // Simple valid id with CDATA
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setId(payloadElement.DeepCopy(), "urn:id")
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<id>urn<![CDATA[:]]>id</id>")),
                },
                // id element with mixed content - invalid
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = setId(payloadElement.DeepCopy(), string.Empty)
                        .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<id>some<foo>value</foo></id>")),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
            };

            // The default behavior is to disallow duplicates of entry/category element defined in ODATA spec.
            if (payloadElement.ElementType != ODataPayloadElementType.EntitySetInstance)
            {
                testDescriptors.Concat(new[]
                {
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = setId(payloadElement.DeepCopy(), "urn:id")
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<id>urn:id</id><id>urn:id</id>")),
                        ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                            ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "id")
                                            : null
                    },
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = setId(payloadElement.DeepCopy(), "urn:id")
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<id/><id/>")),
                        ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                            ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom", "id")
                                            : null
                    },
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = setId(payloadElement.DeepCopy(), string.Empty)
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<id/><id>some<foo>value</foo></id>")),
                        ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                            ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://www.w3.org/2005/Atom/id", "id")
                                            : null
                    },
                    new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = setId(payloadElement.DeepCopy(), string.Empty)
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<id>some<foo>value</foo></id><id/>")),
                        ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                    },
                });
            }

            return testDescriptors;
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of entries with sub context uri in json.")]
        public void EntryWithSubContextUriText()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet citiesEntitySet = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType") as IEdmType;

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry with expanded navagation properties with sub context uri",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("Id", 1)
                        .ExpandedNavigationProperty(
                            "CityHall", 
                            PayloadBuilder.EntitySet(new EntityInstance[]
                            {
                                PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }),
                            }))
                        .XmlRepresentation(
                                "<entry m:context=\"http://odata.org/test/$metadata#Cities(Id)/$entity\">" + 
                                    "<link rel=\"http://docs.oasis-open.org/odata/ns/related/CityHall\" title=\"CityHall\">" +
                                        "<m:inline>" +
                                            "<feed>" +
                                                "<entry m:context=\"http://odata.org/test/$metadata#Offices/$entity\">" +
                                                    "<content type=\"application/xml\">" +
                                                    "<m:properties>" +
                                                            "<d:Id>1</d:Id>" +
                                                    "</m:properties>" +
                                                    "</content>" +
                                                "</entry>" +
                                            "</feed>" +
                                        "</m:inline>" +
                                    "</link>" +
                                     "<content type=\"application/xml\">" +
                                        "<m:properties>" +
                                            "<d:Id>1</d:Id>" +                                            
                                        "</m:properties>" +
                                    "</content>" +
                                "</entry>")
                        .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
