//---------------------------------------------------------------------
// <copyright file="StreamReferenceValueReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Reader;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Test reading of named streams in ATOM.
    /// </summary>
    [TestClass, TestCase]
    public class StreamReferenceValueReaderAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of stream properties without metadata.")]
        public void StreamPropertyNoMetadataAtomTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region only href
                // read link with only href.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", "http://odata.org/readlink", null, null, null),
                },

                // edit-link with only href.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", null, "http://odata.org/editlink", null, null),
                },

                // read link with only href. - two links - case sensitive difference.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .StreamProperty("StreamProperty", "http://odata.org/readlink", null, null, null)
                        .StreamProperty("StreampRoperty", "http://odata.org/readlink2", null, null, null),
                },
                // edit-link with only href. - two links - case sensitive difference.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .StreamProperty("StreamProperty", null, "http://odata.org/editlink", null, null)
                        .StreamProperty("sTreamProperty", null, "http://odata.org/editlink2", null, null),
                },
                #endregion
                
                #region missing content type
                // read link with missing content type.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", "http://odata.org/readlink", null, null, null),
                },

                // edit-link with missing content type.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", null, "http://odata.org/editlink", null, null),
                },

                // edit-link with content type in different namespace.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", null, "http://odata.org/editlink", null, null)
                        .XmlRepresentation(@"<entry xmlns:foo='http://www.w3.org/SomeNamespace'>
                                              <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' 
                                                href='http://odata.org/editlink' foo:type='application/atom+xml' />
                                             </entry>"),
                },
                #endregion

                #region content type on both
                // both read and edit link with the same content type, read link first
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", "http://odata.org/readlink", "http://odata.org/editlink", "mime/type")
                        .XmlRepresentation(@"<entry>
                                              <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/readlink' type='mime/type'/>
                                              <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' href='http://odata.org/editlink' type='mime/type'/>
                                             </entry>"),
                },
                // both read and edit link with the same content type, edit link first
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", "http://odata.org/readlink", "http://odata.org/editlink", "mime/type")
                        .XmlRepresentation(@"<entry>
                                              <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' href='http://odata.org/editlink' type='mime/type'/>
                                              <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/readlink' type='mime/type'/>
                                             </entry>"),
                },
                #endregion
                
                #region etag
                // verify that read link discards etag.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", "http://odata.org/readlink", null, null, null)
                        .XmlRepresentation(@"<entry>
                                              <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' 
                                                  href='http://odata.org/readlink' m:etag='someetag'/>
                                             </entry>"),    
                },
                #endregion etag

                #region all possible valid attributes
                // edit-link with href, content type and etag.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", null, "http://odata.org/editlink", "application/atom+xml", "some_etag"),
                },
                // read link with href and content type.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", "http://odata.org/readlink", null, "application/atom+xml", null),
                },
                #endregion
                
                #region extra attributes
                // extra attributes in the read link.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", "http://odata.org/readlink")
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty'  foo1='bar1' href='http://odata.org/readlink' foo='bar'/>
                                             </entry>"),    
                },
                // extra attributes in the edit-link.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty",null, "http://odata.org/editlink")
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' foo1='bar1' href='http://odata.org/editlink' foo='bar'/>
                                             </entry>"),    
                },
                #endregion

                #region missing rel 
                // missing rel
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link href='http://odata.org/editlink' type='application/atom+xml'/>
                                            </entry>"),
                },
                // rel attribute in different namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry xmlns:foo='http://www.w3.org/SomeNamespace'>
                                                <link href='http://odata.org/editlink' type='application/atom+xml' foo:rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty'/>
                                            </entry>"),
                },
                #endregion missing rel

                #region name prefix
                // verify that invalid stream name prefix is discarded.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/data/somename/StreamProperty' href='http://odata.org/readlink' />
                                             </entry>"),   
                },
                // verify that stream name prefix is case-sensitive.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/Mediaresource/StreamProperty' href='http://odata.org/readlink' type='application/atom+xml'/>
                                             </entry>"),    
                },
                // verify that incomplete rel is not recognized as stream property (missing the last slash in read link)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/mediaresource' href='http://odata.org/readlink' />
                                             </entry>"),   
                },
                // verify that incomplete rel is not recognized as stream property (missing the last slash in edit link)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/edit-media' href='http://odata.org/editlink' />
                                             </entry>"),   
                },
                #endregion

                #region order of the attributes and link
                // edit link first.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .StreamProperty("StreamProperty1", null, "http://odata.org/editlink", null, null)
                        .StreamProperty("StreamProperty2", "http://odata.org/readlink", null, null, null),
                },
                // read link first.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .StreamProperty("StreamProperty1", "http://odata.org/readlink", null, null, null)
                        .StreamProperty("StreamProperty2", null, "http://odata.org/editlink", null, null),
                },
                // rel after etag, href and type.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", null, "http://odata.org/editlink", "application/atom+xml", "some_etag")
                        .XmlRepresentation(@"<entry>
                                                <link m:etag='some_etag' href='http://odata.org/editlink' type='application/atom+xml' rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty'/>
                                            </entry>"),
                },
                #endregion order of the attributes and link

                #region content inside link element
                // verify that all contents inside link element are skipped.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .StreamProperty("StreamProperty", null, "http://odata.org/editlink", "application/atom+xml", null)
                        .XmlRepresentation(@"<entry>
                                                <link href='http://odata.org/editlink' type='application/atom+xml' rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty'>
                                                <m:unknown>some text</m:unknown>
                                                <!-- Some comments -->
                                                <d:unknown/>
                                                <ns:someelement xmlns:ns='somenamespace'/>                
                                                <![CDATA[cdata]]>
                                                </link>
                                            </entry>"),
                },
                #endregion content inside link element
            };

            #region empty or missing etag
            KeyValuePair<string, string>[] etags = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>(string.Empty, "m:etag=''"),  // empty etag
                new KeyValuePair<string, string>("some_etag", "m:etag='some_etag'"), // valid etag
                new KeyValuePair<string, string>(null, "foo:etag='some_etag'"), // etag in a different namespace
            };

            testDescriptors = testDescriptors.Concat(etags.Select(etag =>

                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty", null, "http://odata.org/editlink", null, etag.Key)
                        .XmlRepresentation(string.Format(@"<entry xmlns:foo='http://www.w3.org/SomeNamespace'>
                                                             <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' 
                                                                href='http://odata.org/editlink' {0} />
                                                           </entry>", etag.Value)),
                }));
            #endregion empty or missing etag

            // WCF DS client, server and default ODataLib show the same behavior for stream properties.
            this.CombinatorialEngineProvider.RunCombinations(
               TestReaderUtils.ODataBehaviorKinds,
               testDescriptors,
               this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
               (behaviorKind, testDescriptor, testConfiguration) =>
               {
                   if (testConfiguration.IsRequest)
                   {
                       testDescriptor = new PayloadReaderTestDescriptor(testDescriptor)
                       {
                           ExpectedException = null,
                           ExpectedResultPayloadElement = tc => RemoveStreamPropertyPayloadElementNormalizer.Normalize(testDescriptor.PayloadElement.DeepCopy())
                       };
                   }

                   testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
               });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies error cases for stream properties.")]
        public void StreamPropertyErrorAtomTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region multiple attributes
                // same stream property, multiple edit-links         
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' href='http://odata.org/editlink'/>
                                                <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' href='http://odata.org/editlink'/>
                                                </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleEditLinks", "StreamProperty"),
                },
                // same stream property, multiple read links
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/readlink'/>
                                                <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/readlink'/>
                                                </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleReadLinks", "StreamProperty"),
                },
                // same stream property, multiple and different content types
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/readlink' type='application/atom+xml'/>
                                                <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' href='http://odata.org/readlink' type='application/atom+xml+foo'/>  
                                                </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_StreamPropertyWithMultipleContentTypes", "StreamProperty"),
                },
                #endregion

                #region property name collision
                // multiple properties with the same name.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().NavigationProperty("StreamProperty", "http://odata.org/link")
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/related/StreamProperty' href='http://odata.org/link' type='application/atom+xml'/>
                                                <link href='http://odata.org/editlink' type='application/atom+xml' rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty'/>                                               
                                            </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed", "StreamProperty")
                },
                // multiple properties with the same name. read link. (stream property first)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().PrimitiveProperty("StreamProperty", "foo")
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/link'/>
                                                <content type='application/xml'><m:properties><d:StreamProperty>foo</d:StreamProperty></m:properties></content>
                                            </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed", "StreamProperty")
                },
                // multiple properties with the same name. read link. (normal property first)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().PrimitiveProperty("StreamProperty", "foo")
                        .XmlRepresentation(@"<entry>
                                                <content type='application/xml'><m:properties><d:StreamProperty>foo</d:StreamProperty></m:properties></content>
                                                <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty' href='http://odata.org/link'/>
                                            </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_StreamPropertyDuplicatePropertyName", "StreamProperty")
                }, 
                // multiple properties with the same name. edit link. (stream property first)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().PrimitiveProperty("StreamProperty", "foo")
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' href='http://odata.org/link'/>
                                                <content type='application/xml'><m:properties><d:StreamProperty>foo</d:StreamProperty></m:properties></content>
                                            </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed", "StreamProperty")
                },
                // multiple properties with the same name. edit link. (normal property first)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().PrimitiveProperty("StreamProperty", "foo")
                        .XmlRepresentation(@"<entry>
                                                <content type='application/xml'><m:properties><d:StreamProperty>foo</d:StreamProperty></m:properties></content>
                                                <link rel='http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty' href='http://odata.org/link'/>
                                            </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_StreamPropertyDuplicatePropertyName", "StreamProperty")
                },
                #endregion property name collision

                #region empty name
                // empty name on the read link of a stream property
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/mediaresource/' href='http://odata.org/link' type='application/atom+xml'/>
                                            </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_StreamPropertyWithEmptyName")
                },
                // empty name on the edit link of a stream property
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation(@"<entry>
                                                <link rel='http://docs.oasis-open.org/odata/ns/edit-media/' href='http://odata.org/link' type='application/atom+xml'/>
                                            </entry>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_StreamPropertyWithEmptyName")
                },
                #endregion
            };

            #region missing href
            string[] hrefs = new string[]
            {
                string.Empty, // missing href
                "foo:href='http://odata.org/differentNamespace'", // href in different namespace
            };

            string[] links = new string[]
            {
                "http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty",
                "http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty"
            };

            testDescriptors = testDescriptors.Concat(hrefs.SelectMany(href => links.Select(link =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity().StreamProperty("StreamProperty")
                        .XmlRepresentation(string.Format(
                                                @"<entry xmlns:foo='http://www.w3.org/SomeNamespace'>
                                                        <link rel='{0}' {1}/>
                                                    </entry>", link, href)),
                })));
            #endregion missing href

            // WCF DS client, server and default ODataLib show the same behavior for stream properties.
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                TestReaderUtils.ODataBehaviorKinds,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, behaviorKind, testConfiguration) =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor)
                        {
                            ExpectedException = null,
                            ExpectedResultPayloadElement = tc => RemoveStreamPropertyPayloadElementNormalizer.Normalize(testDescriptor.PayloadElement.DeepCopy())
                        };
                    }

                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behaviorKind));
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of stream properties with metadata.")]
        public void StreamPropertyWithMetadataTests()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                StreamReferenceValueReaderTests.CreateStreamPropertyMetadataTestDescriptors(this.Settings).SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor)
                        {
                            ExpectedException = null,
                            ExpectedResultPayloadElement = tc => RemoveStreamPropertyPayloadElementNormalizer.Normalize(testDescriptor.PayloadElement.DeepCopy())
                        };
                    }

                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
