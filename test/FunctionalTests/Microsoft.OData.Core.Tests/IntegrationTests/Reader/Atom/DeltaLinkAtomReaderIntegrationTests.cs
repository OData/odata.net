//---------------------------------------------------------------------
// <copyright file="DeltaLinkAtomReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Reader.Atom
{
    public class DeltaLinkAtomReaderIntegrationTests
    {
        private const string AtomContentType = "application/atom+xml";

        [Fact]
        public void ReadingDeltaLinkOnExpandedFeedShouldThrowException()
        {
            // Payload with an entry and an inner feed, where the inner feed has a delta link.
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
                                <link rel=""http://docs.oasis-open.org/odata/ns/delta"" href=""http://host/deltaLink"" />
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
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_EncounteredDeltaLinkInNestedFeed);
        }

        [Fact]
        public void DeltaLinkAndNextLinkInTopLevelFeedShouldThrow()
        {
            const string payload = @"
                <feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:count>14</m:count>
                    <m:annotation term=""custom.namespace.FeedName"" target=""something"" string=""MyFeed"" />
                    <id>urn:myFeed</id>
                    <title/>
                    <updated>2013-01-04T21:32:48Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel=""http://docs.oasis-open.org/odata/ns/delta"" href=""http://host/deltaLink"" />
                    <link rel=""next"" href=""http://host/next"" />
                </feed>
                ";
            Action testSubject = () => this.ReadFeedPayload(payload, reader => { });
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink);
        }

        [Fact]
        public void DuplicateDeltaLinkInTopLevelFeedShouldThrow()
        {
            const string payload = @"
                <feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:count>14</m:count>
                    <m:annotation term=""custom.namespace.FeedName"" target=""something"" string=""MyFeed"" />
                    <id>urn:myFeed</id>
                    <title/>
                    <updated>2013-01-04T21:32:48Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel=""http://docs.oasis-open.org/odata/ns/delta"" href=""http://host/deltaLink1"" />
                    <link rel=""http://docs.oasis-open.org/odata/ns/delta"" href=""http://host/deltaLink2"" />
                </feed>
                ";
            Action testSubject = () => this.ReadFeedPayload(payload, reader => { });
            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataAtomEntryAndFeedDeserializer_MultipleLinksInFeed("http://docs.oasis-open.org/odata/ns/delta"));
        }

        private const string topLevelFeedWithDeltaLink = @"
                <feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:count>14</m:count>
                    <m:annotation term=""custom.namespace.FeedName"" target=""something"" string=""MyFeed"" />
                    <id>urn:myFeed</id>
                    <title/>
                    <updated>2013-01-04T21:32:48Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel=""http://docs.oasis-open.org/odata/ns/delta"" href=""http://host/deltaLink"" />
                </feed>
                ";

        [Fact]
        public void ReaderShouldReadDeltaLinkInTopLevelFeedForResponse()
        {
            ODataFeed feed = null;
            this.ReadFeedPayload(topLevelFeedWithDeltaLink, reader => { feed = reader.Item as ODataFeed; });
            feed.DeltaLink.Should().Be(new Uri("http://host/deltaLink"));
        }

        [Fact]
        public void ReaderShouldIgnoreDeltaLinkInTopLevelFeedForRequest()
        {
            ODataFeed feed = null;
            this.ReadFeedPayload(topLevelFeedWithDeltaLink, reader => { feed = reader.Item as ODataFeed; }, false /* isResponse */);

            // Delta link in requst is ignored in case of Atom but this behavior is different from json light.
            feed.DeltaLink.Should().BeNull();
        }

        private void ReadFeedPayload(string payload, Action<ODataReader> action, bool isResponse = true, ODataVersion maxProtocolVersion = ODataVersion.V4)
        {
            if (isResponse)
            {
                IODataResponseMessage message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)), StatusCode = 200 };
                message.SetHeader("Content-Type", "application/atom+xml;type=feed");
                ODataMessageReaderSettings settings = new ODataMessageReaderSettings()
                {
                    MaxProtocolVersion = maxProtocolVersion,
                    EnableAtom = true
                };
                using (var msgReader = new ODataMessageReader(message, settings))
                {
                    var reader = msgReader.CreateODataFeedReader();
                    while (reader.Read())
                    {
                        action(reader);
                    }
                }
            }
            else
            {
                IODataRequestMessage message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)), Method = "GET" };
                message.SetHeader("Content-Type", "application/atom+xml;type=feed");
                ODataMessageReaderSettings settings = new ODataMessageReaderSettings()
                    {
                        MaxProtocolVersion = maxProtocolVersion,
                        EnableAtom = true
                    };
                using (var msgReader = new ODataMessageReader(message, settings))
                {
                    var reader = msgReader.CreateODataFeedReader();
                    while (reader.Read())
                    {
                        action(reader);
                    }
                }
            }

        }

        private void ReadEntryPayload(string payload, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/atom+xml;type=entry");
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings { EnableAtom = true };
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, settings))
            {
                var reader = msgReader.CreateODataEntryReader();
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }
    }
}
