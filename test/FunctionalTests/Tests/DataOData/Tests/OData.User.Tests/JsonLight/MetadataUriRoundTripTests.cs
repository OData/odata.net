//---------------------------------------------------------------------
// <copyright file="MetadataUriRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.User.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class MetadataUriRoundTripTests
    {
        private const string ModelNamespace = "Default.ExtendedNamespace";

        private IEdmEntityContainer defaultContainer;
        private IEdmEntitySet peopleSet;
        private IEdmEntitySet placesSet;
        private IEdmEntitySet organizationsSet;
        private IEdmModel model;

        [TestInitialize]
        public void Initialize()
        {
            this.model = Utils.BuildModel("Default.ExtendedNamespace");
            this.defaultContainer = model.EntityContainer;

            this.peopleSet = this.defaultContainer.FindEntitySet("People");
            this.placesSet = this.defaultContainer.FindEntitySet("Places");
            this.organizationsSet = this.defaultContainer.FindEntitySet("Organizations");
        }

        [TestMethod]
        public void EntryMetadataUrlRoundTrip()
        {
            var stream = new MemoryStream();
            var writerRequestMemoryMessage = new InMemoryMessage();
            writerRequestMemoryMessage.Stream = stream;
            writerRequestMemoryMessage.SetHeader("Content-Type", "application/json");

            var writerSettings = new ODataMessageWriterSettings() {Version = ODataVersion.V4, DisableMessageStreamDisposal = true};
            writerSettings.ODataUri = new ODataUri() {ServiceRoot = new Uri("http://christro.svc/")};

            var messageWriter = new ODataMessageWriter((IODataResponseMessage)writerRequestMemoryMessage, writerSettings, this.model);
            var organizationSetWriter = messageWriter.CreateODataEntryWriter(this.organizationsSet);
            var odataEntry = new ODataEntry(){ TypeName = ModelNamespace + ".Corporation" };
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

            var messageReader = new ODataMessageReader((IODataResponseMessage)readerResponseMemoryMessage, new ODataMessageReaderSettings() {MaxProtocolVersion = ODataVersion.V4, DisableMessageStreamDisposal = true}, this.model);
            var organizationReader = messageReader.CreateODataEntryReader(this.organizationsSet, this.organizationsSet.EntityType());
            organizationReader.Read().Should().Be(true);
            organizationReader.Item.As<ODataEntry>();
        }
    }
}
