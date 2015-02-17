//---------------------------------------------------------------------
// <copyright file="FeedReaderAtomTests.cs" company="Microsoft">
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
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of feed in ATOM.
    /// </summary>
    [TestClass, TestCase]
    public class FeedReaderAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        // TODO: Add tests for metadata validation (maybe we can reuse existing format independent tests)
        // TODO: Add tests for no metadata feed reading - complex/collection/primitive

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of the feed start element.")]
        public void FeedStartElementTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Empty feed element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().XmlRepresentation("<feed/>")
                },
                // Element with wrong local name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().XmlRepresentation("<entry/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_FeedElementWrongName", "entry", "http://www.w3.org/2005/Atom")
                },
                // Element with wrong local name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().XmlRepresentation("<Feed/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_FeedElementWrongName", "Feed", "http://www.w3.org/2005/Atom")
                },
                // Element with wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().XmlRepresentation("<d:feed/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_FeedElementWrongName", "feed", "http://docs.oasis-open.org/odata/ns/data")
                },
                // Element with wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().XmlRepresentation("<m:type/>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_FeedElementWrongName", "type", "http://docs.oasis-open.org/odata/ns/metadata")
                },
                // Empty feed element with additional attributes - all attributes are ignored
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().XmlRepresentation("<feed some='bar' m:type='Edm.String' m:null='true' m:etag='foo' d:prop='1' />")
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

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of child nodes of atom:feed element.")]
        public void FeedChildrenTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Empty feed
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().XmlRepresentation("<feed/>")
                },
            };

            string[] paddingToSkip = new string[]
            {
                string.Empty,
                "  \r\n\t",
                "<!-- some -->   <?value?>",
                "some text",
                "<![CDATA[cdata]]>",
                "<c:some xmlns:c='c'/>",
                "<unknown/>",
                "<m:unknown/>",
                "<d:unknown/>"
            };

            testDescriptors = testDescriptors.Concat(paddingToSkip.SelectMany(padding =>
                new PayloadReaderTestDescriptor[]
                {
                    // Empty feed
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.EntitySet()
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<feed>{0}</feed>",
                                padding))
                    },
                    // Feed with only entries
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.EntitySet()
                            .Append(PayloadBuilder.Entity(), PayloadBuilder.Entity().ETag("second"))
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<feed>{0}" +
                                "<entry/>{0}" +
                                "<entry m:etag='second'></entry>{0}" +
                                "</feed>",
                                padding))
                    },
                    // Feed without entries but ID, inline count and next link
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.EntitySet().InlineCount(42).NextLink("http://odata.org/next").AtomId("urn:id")
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<feed>{0}" +
                                "<id>urn:id</id>{0}" +
                                "<m:count>42</m:count>{0}" +
                                "<link rel='next' href='http://odata.org/next'/>{0}" +
                                "</feed>",
                                padding)),
                        SkipTestConfiguration = tc => tc.IsRequest
                    },
                    // Full feed
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.EntitySet().InlineCount(42).NextLink("http://odata.org/next").AtomId("urn:id")
                            .Append(PayloadBuilder.Entity(), PayloadBuilder.Entity().ETag("second"))
                            .XmlRepresentation(string.Format(CultureInfo.InvariantCulture,
                                "<feed>{0}" +
                                "<id>urn:id</id>{0}" +
                                "<m:count>42</m:count>{0}" +
                                "<entry/>{0}" +
                                "<entry m:etag='second'></entry>{0}" +
                                "<link rel='next' href='http://odata.org/next'/>{0}" +
                                "</feed>",
                                padding)),
                        SkipTestConfiguration = tc => tc.IsRequest
                    },
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of the feed m:count element.")]
        public void FeedCountElementTest()
        {
            var testCases = new []
            {
                #region Handling duplicates
                // The default behavior is to disallow duplicates of feed/m:count element defined in ODATA spec.
                new
                {
                    ExpectedCountValue = (int?) null,
                    XmlRepresentation = "<feed><m:count/><m:count/></feed>",
                    ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_CannotConvertPrimitiveValue", "", "Edm.Int64")
                },
                new
                {
                    ExpectedCountValue = (int?) null,
                    XmlRepresentation = "<feed><m:count>42</m:count><m:count>42</m:count></feed>",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "count")
                },
                new
                {
                    ExpectedCountValue = (int?) null,
                    XmlRepresentation = "<feed><m:count>42</m:count><m:count/></feed>",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_DuplicateElements", "http://docs.oasis-open.org/odata/ns/metadata", "count")
                },
                new
                {
                    ExpectedCountValue = (int?) null,
                    XmlRepresentation = "<feed><m:count/><m:count>42</m:count></feed>",
                    ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_CannotConvertPrimitiveValue", "", "Edm.Int64")
                },
                new
                {
                    ExpectedCountValue = (int?) null,
                    XmlRepresentation = "<feed><m:count>42<foo/></m:count><m:count/></feed>",
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element")
                },
                new
                {
                    ExpectedCountValue = (int?) null,
                    XmlRepresentation = "<feed><m:count/><m:count>42<foo/></m:count></feed>",
                    ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_CannotConvertPrimitiveValue", "", "Edm.Int64")
                },
                #endregion
                // Simple value
                new
                {
                    ExpectedCountValue = (int?) 42,
                    XmlRepresentation = "<feed><m:count>42</m:count></feed>",
                    ExpectedException = (ExpectedException) null
                },
                // Negative value
                new
                {
                    ExpectedCountValue = (int?) -1,
                    XmlRepresentation = "<feed><m:count>-1</m:count></feed>",
                    ExpectedException = (ExpectedException) null
                },
                // Value with comments and such
                new
                {
                    ExpectedCountValue = (int?) 42,
                    XmlRepresentation = "<feed><m:count><!--comment-->  \r\n\t42<!--some--></m:count></feed>",
                    ExpectedException = (ExpectedException) null
                },
                // Value in multiple nodes
                new
                {
                    ExpectedCountValue = (int?) 42,
                    XmlRepresentation = "<feed><m:count><!--comment-->4<![CDATA[2]]>  </m:count></feed>",
                    ExpectedException = (ExpectedException) null
                },
                // Value with significant whitespaces
                new
                {
                    ExpectedCountValue = (int?) 42,
                    XmlRepresentation = "<feed><m:count xml:space='preserve'>42<!--comment-->   \r\n\t</m:count></feed>",
                    ExpectedException = (ExpectedException) null
                },
                // Additional attributes (all should be ignored)
                new
                {
                    ExpectedCountValue = (int?) 42,
                    XmlRepresentation = "<feed><m:count m:null='true' m:type='Edm.String' foo='bar' d:prop='1'>42</m:count></feed>",
                    ExpectedException = (ExpectedException) null
                },
                // Special test for m:null='false' since client used to handle that differently.
                new
                {
                    ExpectedCountValue = (int?) 42,
                    XmlRepresentation = "<feed><m:count m:null='false'>42</m:count></feed>",
                    ExpectedException = (ExpectedException) null
                },
                // Special test for m:null='foo' since client will fail on this.
                new
                {
                    ExpectedCountValue = (int?) 42,
                    XmlRepresentation = "<feed><m:count m:null='foo'>42</m:count></feed>",
                    ExpectedException = (ExpectedException) null
                },
                // Invalid inline count value
                new
                {
                    ExpectedCountValue = (int?) null,
                    XmlRepresentation = "<feed><m:count> foo </m:count></feed>",
                    ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_CannotConvertPrimitiveValue", " foo ", "Edm.Int64")
                },
                // Mixed content is not allowed
                new
                {
                    ExpectedCountValue = (int?) null,
                    XmlRepresentation = "<feed><m:count>42<foo/></m:count></feed>",
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element")
                },
            };

            // WCF DS client, server and default ODataLib show the same behavior when processing feed/link/[rel=next].
            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                testCases,
                new bool[] { false, true },
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (behaviorKind, testCase, inExpandedLink, testConfiguration) =>
                {
                    PayloadReaderTestDescriptor testDescriptor;

                    if (testConfiguration.IsRequest || inExpandedLink)
                    {
                        // m:count in request payloads should not be read or validated.
                        // m:count in V1 payloads should not be read or validated.
                        testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.EntitySet().XmlRepresentation(testCase.XmlRepresentation)
                        };
                    }
                    else 
                    {
                        // m:count in response payloads should be validated, and the inline count value should be set when there is no error.
                        testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.EntitySet()
                                .InlineCount(testCase.ExpectedCountValue ?? 1) // Use 1 as a dummy value when an exception is expected.
                                .XmlRepresentation(testCase.XmlRepresentation),
                            ExpectedException = testCase.ExpectedException
                        };
                    }

                    if (inExpandedLink)
                    {
                        testDescriptor = testDescriptor.InEntryWithExpandedLink(false);
                    }

                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of next link on a feed.")]
        public void NextLinkTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptorsWithRecognizedNextLink =
                EntryReaderAtomTests.CreateLinkTestDescriptors(
                    this.Settings,
                    PayloadBuilder.EntitySet(),
                    (entitySet, link) => entitySet.NextLink(link),
                    "next",
                    "<feed>{0}</feed>");

            testDescriptorsWithRecognizedNextLink = testDescriptorsWithRecognizedNextLink.Select(td =>
                new PayloadReaderTestDescriptor(td)
                {
                    SkipTestConfiguration = tc => tc.IsRequest,
                });

            IEnumerable<PayloadReaderTestDescriptor> testDescriptorsWithUnrecognizedNextLink =
               EntryReaderAtomTests.CreateLinkTestDescriptors(
                   this.Settings,
                   PayloadBuilder.EntitySet(),
                   (entitySet, link) => entitySet, // Next link should be ignored in requests and in V1
                   "next",
                   "<feed>{0}</feed>");

            // Set the expected exception to null for request and V1 payloads since we should be ignoring next links and not validating.
            testDescriptorsWithUnrecognizedNextLink = testDescriptorsWithUnrecognizedNextLink.Select(td =>
                new PayloadReaderTestDescriptor(td)
                {
                    SkipTestConfiguration = tc => !tc.IsRequest,
                    ExpectedException = null,
                });

            // WCF DS client, server and default ODataLib show the same behavior when processing feed/link/[rel=next].
            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                testDescriptorsWithRecognizedNextLink.Concat(testDescriptorsWithUnrecognizedNextLink),
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (behaviorKind, testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of self link on a feed.")]
        public void SelfLinkTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                TestReaderUtils.ODataBehaviorKinds,
                (behaviorKind) =>
                {
                    string xmlRepresentationTemplate = "<feed>{0}</feed>";
                    string relName = "self";
                    IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
                    {
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.EntitySet()
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"'/><link rel='" + relName +"'/>")),
                            ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                                ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                                                : null,
                        },
                        // The default behavior is to disallow duplicates of feed/link[@rel='self'] element defined in ODATA spec.
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.EntitySet()
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link' rel='" + relName +"'/><link href='' rel='" + relName +"'/>")),
                            ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                                ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                                                : null,
                        },
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.EntitySet()
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='' rel='" + relName +"'/><link href='http://odata.org/link' rel='" + relName +"'/>")),
                            ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                                ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                                                : null,
                        },
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.EntitySet()
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link rel='" + relName +"'/><link href='http://odata.org/link' rel='" + relName +"'/>")),
                            ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                                ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                                                : null,
                        },
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.EntitySet()
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link' rel='" + relName +"'/><link rel='" + relName +"'/>")),
                            ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                                ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                                                : null,
                        },
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.EntitySet()
                                .XmlRepresentation(string.Format(CultureInfo.InvariantCulture, xmlRepresentationTemplate, "<link href='http://odata.org/link' rel='" + relName +"'/><link href='http://odata.org/link' rel='" + relName +"'/>")),
                            ExpectedException = behaviorKind == TestODataBehaviorKind.Default
                                                ? ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed", relName)
                                                : null,
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

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of id on a feed.")]
        public void FeedIDTest()
        {
            // TODO: This is not really testing atom:id since the test infrastructure doesn't support it at all (payload element doesn't serialize it, converter doesn't convert it)
            // so we are effectively only testing error/success here, not real reading.
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                EntryReaderAtomTests.CreateIdTestDescriptors(
                    this.Settings,
                    PayloadBuilder.EntitySet(),
                    (entitySet, id) => entitySet.AtomId(id),
                    "<feed>{0}</feed>",
                    TestODataBehaviorKind.Default);

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of feed with entries.")]
        public void FeedWithEntriesTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Single empty entry (empty element)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().Append(PayloadBuilder.Entity())
                        .XmlRepresentation("<feed><entry/></feed>")
                },
                // Single empty entry (full end element)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().Append(PayloadBuilder.Entity())
                        .XmlRepresentation("<feed><entry></entry></feed>")
                },
                // Two entries (first empty element)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().Append(PayloadBuilder.Entity().ETag("etag1"), PayloadBuilder.Entity().ETag("etag2"))
                        .XmlRepresentation("<feed><entry m:etag='etag1'/><entry m:etag='etag2'></entry></feed>")
                },
                // Two entries (first full end element)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().Append(PayloadBuilder.Entity().ETag("etag1"), PayloadBuilder.Entity().ETag("etag2"))
                        .XmlRepresentation("<feed><entry m:etag='etag1'></entry><entry m:etag='etag2'/></feed>")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.EntitySet().InlineCount(42).NextLink("http://odata.org/next").AtomId("urn:id")
                        .Append(PayloadBuilder.Entity(), PayloadBuilder.Entity().ETag("second")),
                    SkipTestConfiguration = tc => tc.IsRequest
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

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates parsing of open type feed payloads where the open properties are not present on all entries")]
        public void OpenTypeFeedWithHeterogenousItems()
        {
            var model = new EdmModel();

            var openTestDescriptor = new PayloadReaderTestDescriptor(this.Settings)
            {
                PayloadElement = TestFeeds.CreateOpenEntitySetInstance(model, true, (p) =>true),
                PayloadEdmModel = model,
            };

            this.CombinatorialEngineProvider.RunCombinations(
                this.PayloadGenerator.GenerateReaderPayloads(openTestDescriptor),
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // These payloads get quite large and exceed default size limits
                    var actualConfiguration = new ReaderTestConfiguration(testConfiguration);
                    actualConfiguration.MessageReaderSettings.MessageQuotas.MaxReceivedMessageSize = long.MaxValue;
                    testDescriptor.RunTest(actualConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of feed with sub context uri in atom.")]
        public void FeedWithSubContextUriTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet citiesEntitySet = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType") as IEdmType;

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Feed containing entries with and without sub context uri",
                    PayloadElement = PayloadBuilder
                        .EntitySet( new EntityInstance[]
                            {
                                PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }),
                                PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }),
                            })
                        .XmlRepresentation(
                            "<feed m:context=\"http://odata.org/test/$metadata#Cities(Id)\">" + 
                                "<entry m:context=\"http://odata.org/test/$metadata#Cities(Id)/$entity\">" + 
                                     "<content type=\"application/xml\">" +
                                        "<m:properties>" +
                                            "<d:Id>1</d:Id>" +
                                        "</m:properties>" +
                                    "</content>" +
                                "</entry>" +
                                "<entry>" + 
                                     "<content type=\"application/xml\">" +
                                        "<m:properties>" +
                                            "<d:Id>2</d:Id>" +
                                        "</m:properties>" +
                                    "</content>" +
                                "</entry>" +
                            "</feed>")
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
