//---------------------------------------------------------------------
// <copyright file="MetadataUriRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class MetadataUriRoundTripTests
    {
        private const string ModelNamespace = "Default.ExtendedNamespace";

        private IEdmEntityContainer defaultContainer;
        private IEdmEntitySet peopleSet;
        private IEdmEntitySet placesSet;
        private IEdmEntitySet organizationsSet;
        private IEdmModel model;

        public MetadataUriRoundTripTests()
        {
            this.model = Utils.BuildModel("Default.ExtendedNamespace");
            this.defaultContainer = model.EntityContainer;

            this.peopleSet = this.defaultContainer.FindEntitySet("People");
            this.placesSet = this.defaultContainer.FindEntitySet("Places");
            this.organizationsSet = this.defaultContainer.FindEntitySet("Organizations");
        }

#if !NETCOREAPP1_0
        [Fact]
        public void EntryMetadataUrlRoundTrip()
        {
            var stream = new MemoryStream();
            var writerRequestMemoryMessage = new InMemoryMessage();
            writerRequestMemoryMessage.Stream = stream;
            writerRequestMemoryMessage.SetHeader("Content-Type", "application/json");

            var writerSettings = new ODataMessageWriterSettings() {Version = ODataVersion.V4, EnableMessageStreamDisposal = false};
            writerSettings.ODataUri = new ODataUri() {ServiceRoot = new Uri("http://christro.svc/")};

            var messageWriter = new ODataMessageWriter((IODataResponseMessage)writerRequestMemoryMessage, writerSettings, this.model);
            var organizationSetWriter = messageWriter.CreateODataResourceWriter(this.organizationsSet);
            var odataEntry = new ODataResource(){ TypeName = ModelNamespace + ".Corporation" };
            odataEntry.Property("Id", 1);
            odataEntry.Property("Name", "");
            odataEntry.Property("TickerSymbol", "MSFT");

            organizationSetWriter.WriteStart(odataEntry);
            organizationSetWriter.WriteEnd();

            var readerPayloadInput = Encoding.UTF8.GetString(stream.GetBuffer());
            Console.WriteLine(readerPayloadInput);

            var readerResponseMemoryMessage = new InMemoryMessage();
            readerResponseMemoryMessage.Stream = new MemoryStream(stream.GetBuffer());
            readerResponseMemoryMessage.SetHeader("Content-Type", "application/json");

            var messageReader = new ODataMessageReader((IODataResponseMessage)readerResponseMemoryMessage, new ODataMessageReaderSettings() {MaxProtocolVersion = ODataVersion.V4, EnableMessageStreamDisposal = false}, this.model);
            var organizationReader = messageReader.CreateODataResourceReader(this.organizationsSet, this.organizationsSet.EntityType());
            organizationReader.Read().Should().BeTrue();
            organizationReader.Item.As<ODataResource>();
        }
#endif
    }
}
