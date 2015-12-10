//---------------------------------------------------------------------
// <copyright file="AtomInstanceAnnotationReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Tests.IntegrationTests.Reader.JsonLight;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Reader.Atom
{
    public class AtomInstanceAnnotationReaderIntegrationTests
    {
        private const string AtomContentType = "application/atom+xml";
        private readonly ODataMessageReaderSettings atomReaderSettings = new ODataMessageReaderSettings { EnableAtom = true };

        #region Reading Instance Annotations from Expanded Feeds

        [Fact]
        public void AnnotationOnNestedFeedShouldThrowException()
        {
            // Payload with an entry and an inner feed, where the inner feed has an annotation.
            const string payload = @"
                <entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <id/>
                    <title/>
                    <link rel=""http://docs.oasis-open.org/odata/ns/related/CollectionNavigation"" type=""application/atom+xml;type=feed"" title=""CollectionNavigation"" href=""http://example.com/LinkToNavigation"">
                        <m:inline>
                            <feed>
                                <id>urn:InnerFeed</id>
                                <title/>
                                <updated>2013-01-04T21:32:48Z</updated>
                                <author>
                                    <name/>
                                </author>
                                <m:annotation term=""custom.namespace.FeedName"" target=""."" int=""42"" />
                            </feed>
                        </m:inline>
                    </link>
                    <updated>2013-01-22T01:09:20Z</updated>
                    <author>
                        <name/>
                    </author>
                    <content type=""application/xml"">
                    <m:properties>
                        <d:PropertyName>PropertyValue</d:PropertyName>
                    </m:properties>
                    </content>
                </entry>
                ";

            Action testSubject = () => this.ReadEntryPayload(payload, reader => { });
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_EncounteredAnnotationInNestedFeed);
        }

        [Fact]
        public void AnnotationOnEntryTargetingSomethingOtherThanEntryShouldThrow()
        {
            const string payload = @"
                <entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <id/>
                    <title/>
                    <m:annotation term=""my.namespace.term"" target=""PropertyName"" m:type=""Edm.Int32"">42</m:annotation>
                    <updated>2013-01-22T01:09:20Z</updated>
                    <author>
                        <name/>
                    </author>
                    <content type=""application/xml"">
                    <m:properties>
                        <d:PropertyName>PropertyValue</d:PropertyName>
                    </m:properties>
                    </content>
                </entry>
                ";

            Action testSubject = () => this.ReadEntryPayload(payload, reader => { });

            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_AnnotationWithNonDotTarget("PropertyName", "my.namespace.term"));
        }

        [Fact]
        public void AnnotationOnFeedTargetingSomethingOtherThanFeedShouldThrow()
        {
            const string payload = @"
                <feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:count>14</m:count>
                    <m:annotation term=""custom.namespace.FeedName"" target=""something"" string=""MyFeed"" />
                    <id>myFeed</id>
                    <title/>
                    <updated>2013-01-04T21:32:48Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel=""next"" href=""http://next.link/""/>
                </feed>
                ";

            Action testSubject = () => this.ReadFeedPayload(payload, reader => { });

            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_AnnotationWithNonDotTarget("something", "custom.namespace.FeedName"));
        }

        private void ReadFeedPayload(string payload, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/atom+xml;type=feed");
            message.PreferenceAppliedHeader().AnnotationFilter = "*";
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, atomReaderSettings))
            {
                var reader = msgReader.CreateODataFeedReader();
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private void ReadEntryPayload(string payload, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/atom+xml;type=entry");
            message.PreferenceAppliedHeader().AnnotationFilter = "*";
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, atomReaderSettings))
            {
                var reader = msgReader.CreateODataEntryReader();
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        #endregion

        #region Reading Instance Annotations from Top-Level Feeds

        private const string TopLevelAtomFeedWithInstanceAnnotation =
@"<?xml version=""1.0"" encoding=""utf-8"" ?> 
<feed xml:base=""http://localhost:34402/WcfDataService1.svc/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <m:annotation term=""custom.Int32Annotation1"" int=""123"" />
  <id>http://localhost:34402/WcfDataService1.svc/Folders</id> 
  <title type=""text"">Folders</title> 
  <updated>2013-03-12T17:56:53Z</updated> 
  <link rel=""self"" title=""Folders"" href=""Folders"" /> 
  <author>
    <name /> 
  </author>
  <m:annotation term=""custom.Int32Annotation2"" int=""456"" />
</feed>";

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedResponseInAtom()
        {
            InstanceAnnotationsReaderIntegrationTests.TopLevelFeedInstanceAnnotationTest(TopLevelAtomFeedWithInstanceAnnotation, AtomContentType, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedResponseInAtom()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelAtomFeedWithInstanceAnnotation, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedRequestInAtom()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelAtomFeedWithInstanceAnnotation, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedRequestInAtom()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelAtomFeedWithInstanceAnnotation, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        private static void TopLevelFeedInstanceAnnotationTest(string feedPayload, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true)
        {
            InstanceAnnotationsReaderIntegrationTests.TopLevelFeedInstanceAnnotationTest(feedPayload, AtomContentType, isResponse, shouldReadAndValidateCustomInstanceAnnotations);
        }

        #endregion

        #region Reading Instance Annotations from Top-Level Entries

        private const string TopLevelAtomEntryResponseWithInstanceAnnotation =
@"<?xml version=""1.0"" encoding=""utf-8"" ?> 
 <entry xml:base=""http://localhost:34402/WcfDataService1.svc/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
   <m:annotation term=""custom.Int32Annotation1"" int=""123"" />
 <id>http://localhost:34402/WcfDataService1.svc/Folders(1)</id> 
  <category term=""#TestNamespace.TestEntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" /> 
  <link rel=""edit"" title=""TestEntityType"" href=""Folders(1)"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceNavigationProperty"" type=""application/atom+xml;type=entry"" title=""ResourceNavigationProperty"" href=""Folders(1)/ResourceNavigationProperty"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""Folders(1)/ResourceSetNavigationProperty"" /> 
  <title /> 
  <updated>2013-03-12T20:56:00Z</updated> 
  <author>
  <name /> 
  </author>
  <content type=""application/xml"">
  <m:properties>
  <d:ID m:type=""Edm.Int32"">1</d:ID> 
  </m:properties>
  </content>
  <m:annotation term=""custom.Int32Annotation2"" int=""456"" />
  </entry>";

        private const string TopLevelAtomEntryRequestWithInstanceAnnotation = TopLevelAtomEntryResponseWithInstanceAnnotation;

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryResponseInAtom()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelAtomEntryResponseWithInstanceAnnotation, AtomContentType, isResponse: true);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryRequestInAtom()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelAtomEntryRequestWithInstanceAnnotation, AtomContentType, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryResponseInAtom()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelAtomEntryResponseWithInstanceAnnotation, AtomContentType, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryRequestInAtom()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelAtomEntryRequestWithInstanceAnnotation, AtomContentType, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        private static void TopLevelEntryInstanceAnnotationTest(string feedPayload, string contentType, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true)
        {
            InstanceAnnotationsReaderIntegrationTests.TopLevelEntryInstanceAnnotationTest(feedPayload, AtomContentType, /*isSingleton*/ false, isResponse, shouldReadAndValidateCustomInstanceAnnotations);
        }

        #endregion

        #region Reading Instance Annotations from Expanded Entries

        private const string ExpandedEntryResponseWithInstanceAnnotation =
        @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
  <entry xml:base=""http://localhost:34402/WcfDataService1.svc/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <id>http://localhost:34402/WcfDataService1.svc/Folders(1)</id> 
  <category term=""#TestNamespace.TestEntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" /> 
  <link rel=""edit"" title=""TestEntityType"" href=""Folders(1)"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceNavigationProperty"" type=""application/atom+xml;type=entry"" title=""ResourceNavigationProperty"" href=""Folders(1)/ResourceNavigationProperty"">
  <m:inline>
  <entry>
  <m:annotation term=""custom.Int32Annotation1"" int=""123"" />
  <id>http://localhost:34402/WcfDataService1.svc/Folders(2)</id> 
  <category term=""#TestNamespace.TestEntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" /> 
  <link rel=""edit"" title=""TestEntityType"" href=""Folders(2)"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceNavigationProperty"" type=""application/atom+xml;type=entry"" title=""ResourceNavigationProperty"" href=""Folders(2)/ResourceNavigationProperty"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""Folders(2)/ResourceSetNavigationProperty"" /> 
  <title /> 
  <updated>2013-03-13T00:25:16Z</updated> 
  <author>
  <name /> 
  </author>
  <content type=""application/xml"">
  <m:properties>
  <d:ID m:type=""Edm.Int32"">2</d:ID> 
  </m:properties>
  </content>
  <m:annotation term=""custom.Int32Annotation2"" int=""456"" />
  </entry>
  </m:inline>
  </link>
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""Folders(1)/ResourceSetNavigationProperty"" /> 
  <title /> 
  <updated>2013-03-13T00:25:16Z</updated> 
  <author>
  <name /> 
  </author>
  <content type=""application/xml"">
  <m:properties>
  <d:ID m:type=""Edm.Int32"">1</d:ID> 
  </m:properties>
  </content>
  </entry>";

        private const string ExpandedEntryRequestWithInstanceAnnotation = ExpandedEntryResponseWithInstanceAnnotation;

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryResponse()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, AtomContentType, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryResponse()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, AtomContentType, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryRequest()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, AtomContentType, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryRequest()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, AtomContentType, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        private const string EntryInsideExpandedFeedResponseWithInstanceAnnotation =
        @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
  <entry xml:base=""http://localhost:34402/WcfDataService1.svc/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <id>http://localhost:34402/WcfDataService1.svc/Folders(1)</id> 
  <category term=""#TestNamespace.TestEntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" /> 
  <link rel=""edit"" title=""TestEntityType"" href=""Folders(1)"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceNavigationProperty"" type=""application/atom+xml;type=entry"" title=""ResourceNavigationProperty"" href=""Folders(1)/ResourceNavigationProperty"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""Folders(1)/ResourceSetNavigationProperty"">
  <m:inline>
  <feed>
  <id>http://localhost:34402/WcfDataService1.svc/Folders(1)/ResourceSetNavigationProperty</id> 
  <title type=""text"">ResourceSetNavigationProperty</title> 
  <updated>2013-03-13T01:07:30Z</updated> 
  <link rel=""self"" title=""ResourceSetNavigationProperty"" href=""Folders(1)/ResourceSetNavigationProperty"" /> 
  <entry>
  <m:annotation term=""custom.Int32Annotation1"" int=""123"" />
  <id>http://localhost:34402/WcfDataService1.svc/Folders(3)</id> 
  <category term=""#TestNamespace.TestEntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" /> 
  <link rel=""edit"" title=""TestEntityType"" href=""Folders(3)"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceNavigationProperty"" type=""application/atom+xml;type=entry"" title=""ResourceNavigationProperty"" href=""Folders(3)/ResourceNavigationProperty"" /> 
  <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""Folders(3)/ResourceSetNavigationProperty"" /> 
  <title /> 
  <updated>2013-03-13T01:07:30Z</updated> 
  <author>
  <name /> 
  </author>
  <content type=""application/xml"">
  <m:properties>
  <d:ID m:type=""Edm.Int32"">3</d:ID> 
  </m:properties>
  </content>
  <m:annotation term=""custom.Int32Annotation2"" int=""456"" />
</entry>
  </feed>
  </m:inline>
  </link>
  <title /> 
  <updated>2013-03-13T01:07:30Z</updated> 
  <author>
  <name /> 
  </author>
  <content type=""application/xml"">
  <m:properties>
  <d:ID m:type=""Edm.Int32"">1</d:ID> 
  </m:properties>
  </content>
  </entry>";

        private const string EntryInsideExpandedFeedRequestWithInstanceAnnotation = EntryInsideExpandedFeedResponseWithInstanceAnnotation;

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedResponse()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, AtomContentType, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedResponse()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, AtomContentType, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedRequest()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, AtomContentType, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedRequest()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, AtomContentType, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        private static void InlineEntryInstanceAnnotationTest(string payload, string contentType, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true)
        {
            InstanceAnnotationsReaderIntegrationTests.InlineEntryInstanceAnnotationTest(payload, AtomContentType, isResponse, shouldReadAndValidateCustomInstanceAnnotations);
        }

        #endregion Reading Instance Annotations from Expanded Entries
    }
}
