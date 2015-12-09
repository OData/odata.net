//---------------------------------------------------------------------
// <copyright file="ODataAtomEntryAndFeedDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class ODataAtomEntryAndFeedDeserializerTests
    {
        private ODataMessageReaderSettings atomReaderSettings = new ODataMessageReaderSettings { EnableAtom = true };

        [Fact]
        public void ValidAbsoluteUriShouldBeReadInAtomFeed()
        {
            const string payload = @"
                <feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <m:count>14</m:count>
                    <id>urn:feedid</id>
                    <title/>
                    <updated>2013-11-05:22:48Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel=""next"" href=""http://next.link/""/>
                </feed>
                ";
            ODataFeed feed = null;
            this.ReadFeedPayload(payload, reader => { feed = reader.Item as ODataFeed; });
            feed.Id.Should().Be(new Uri("urn:feedid"));
        }

        [Fact]
        public void ValidAbsoluteUriShouldBeReadInAtomEntry()
        {
            const string payload = @"
                <entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <id>urn:entryid</id>
                    <title/>
                    <updated>2013-11-05:22:48Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel=""next"" href=""http://next.link/""/>
                </entry>
                ";

            ODataEntry entry = null;
            this.ReadEntryPayload(payload, reader => { entry = reader.Item as ODataEntry; });
            entry.Id.Should().Be(new Uri("urn:entryid"));
        }

        [Fact]
        public void InvalidAbsoluteUriShouldThrowInAtomFeed()
        {
            const string payload = @"
                <feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <id>feedid</id>
                    <title/>
                    <updated>2013-11-05:22:48Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel=""next"" href=""http://next.link/""/>
                </feed>
                ";

            Action testSubject = () => this.ReadFeedPayload(payload, reader => { });

            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataUriUtils_InvalidUriFormatForEntryIdOrFeedId("feedid"));
        }

        [Fact]
        public void InvalidAbsoluteUriShouldThrowInAtomEntry()
        {
            const string payload = @"
                <entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <id>entryid</id>
                    <title/>
                    <updated>2013-11-05:22:48Z</updated>
                    <author>
                        <name/>
                    </author>
                    <link rel=""next"" href=""http://next.link/""/>
                </entry>
                ";

            Action testSubject = () => this.ReadEntryPayload(payload, reader => { });

            testSubject.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataUriUtils_InvalidUriFormatForEntryIdOrFeedId("entryid"));
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
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, atomReaderSettings))
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
