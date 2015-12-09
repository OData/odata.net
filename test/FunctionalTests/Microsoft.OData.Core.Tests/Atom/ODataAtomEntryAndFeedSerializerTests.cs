//---------------------------------------------------------------------
// <copyright file="ODataAtomEntryAndFeedSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class ODataAtomEntryAndFeedSerializerTests
    {
        [Fact]
        public void SerializedNavigationPropertyShouldIncludeAssociationLinkUrl()
        {
            var atomFragment =
                this.SerializeAtomFragment(
                    serializer =>
                    serializer.WriteNavigationLinkStart(
                        new ODataNavigationLink
                        {
                            Name = "NavigationProperty",
                            AssociationLinkUrl = new Uri("http://example.com/association"),
                            Url = new Uri("http://example.com/navigation"),
                            IsCollection = false
                        },
                        null));

            atomFragment.Should().Contain(
              "<link rel=\"http://docs.oasis-open.org/odata/ns/relatedlinks/NavigationProperty\" type=\"application/xml\" title=\"NavigationProperty\" href=\"http://example.com/association\"");
        }

        [Fact]
        public void SerializedNavigationPropertyShouldIncludeNavigationLinkUrl()
        {
            var atomFragment =
                this.SerializeAtomFragment(
                    serializer =>
                    serializer.WriteNavigationLinkStart(
                        new ODataNavigationLink
                        {
                            Name = "NavigationProperty",
                            AssociationLinkUrl = new Uri("http://example.com/association"),
                            Url = new Uri("http://example.com/navigation"),
                            IsCollection = false
                        },
                        null));

            atomFragment.Should().Contain(
              "<link rel=\"http://docs.oasis-open.org/odata/ns/related/NavigationProperty\" type=\"application/atom+xml;type=entry\" title=\"NavigationProperty\" href=\"http://example.com/navigation\"");
        }

        private string SerializeAtomFragment(Action<ODataAtomEntryAndFeedSerializer> writeWithSerializer)
        {
            string result;
            using (MemoryStream stream = new MemoryStream())
            {
                var context = this.CreateAtomOutputContext(stream);
                var serializer = new ODataAtomEntryAndFeedSerializer(context);
                context.XmlWriter.WriteStartDocument();
                context.XmlWriter.WriteStartElement("testroot");
                writeWithSerializer(serializer);

                serializer.XmlWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                result = new StreamReader(stream).ReadToEnd();
            }

            return result;
        }

        private ODataAtomOutputContext CreateAtomOutputContext(MemoryStream stream, bool writingResponse = true)
        {
            IEdmModel model = new EdmModel();

            ODataMessageWriterSettings settings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4,
                ODataUri = new ODataUri { ServiceRoot = new Uri("http://example.com/") }
            };

            return new ODataAtomOutputContext(
                ODataFormat.Atom,
                new NonDisposingStream(stream),
                Encoding.UTF8,
                settings,
                writingResponse,
                /*synchronous*/ true,
                model,
                /*urlResolver*/ null);
        }
    }
}
