//---------------------------------------------------------------------
// <copyright file="ODataReaderWrapperWithEventsUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using System.IO;
    using System.Text;
    using AstoriaUnitTests.TDD.Tests;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataReaderWrapperWithEventsUnitTests
    {
        private const string ProductsWithExpandedCategoryEntry = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title type=""text"">Products</title>
  <id>http://services.odata.org/OData/OData.svc/Products</id>
  <updated>2012-11-14T00:54:12Z</updated>
  <link rel=""self"" title=""Products"" href=""Products"" />
  <entry>
    <id>http://services.odata.org/OData/OData.svc/Products(0)</id>
    <title type=""text"">Bread</title>
    <summary type=""text"">Whole grain bread</summary>
    <updated>2012-11-14T00:54:12Z</updated>
    <author>
      <name />
    </author>
    <link rel=""edit"" title=""Product"" href=""Products(0)"" />
    <link rel=""http://docs.oasis-open.org/odata/ns/related/Category"" type=""application/atom+xml;type=entry"" title=""Category"" href=""Products(0)/Category"">
      <m:inline>
        <entry>
          <id>http://services.odata.org/OData/OData.svc/Categories(0)</id>
          <title type=""text"">Food</title>
          <updated>2012-11-14T00:54:12Z</updated>
          <author>
            <name />
          </author>
          <link rel=""edit"" title=""Category"" href=""Categories(0)"" />
          <link rel=""http://docs.oasis-open.org/odata/ns/related/Products"" type=""application/atom+xml;type=feed"" title=""Products"" href=""Categories(0)/Products"" />
          <category term=""#ODataDemo.Category"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
          <content type=""application/xml"">
            <m:properties>
              <d:ID m:type=""Edm.Int32"">0</d:ID>
              <d:Name>Food</d:Name>
            </m:properties>
          </content>
        </entry>
      </m:inline>
    </link>
    <link rel=""http://docs.oasis-open.org/odata/ns/related/Orders"" type=""application/atom+xml;type=feed"" title=""Order"" href=""Products(0)/Orders"">
      <m:inline>
        <feed>
            <title type=""text"">Orders</title>
            <id>http://services.odata.org/OData/OData.svc/Products(0)/Orders</id>
            <updated>2012-11-14T00:54:12Z</updated>
            <link rel=""self"" title=""Orders"" href=""Products(0)/Orders"" />
            <entry>
                <id>http://services.odata.org/OData/OData.svc/Orders(0)</id>
                <title type=""text"">Order 1</title>
                <updated>2012-11-14T00:54:12Z</updated>
                <author>
                <name />
                </author>
                <link rel=""edit"" title=""Order"" href=""Orders(0)"" />
                <category term=""#ODataDemo.Order"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
                <content type=""application/xml"">
                <m:properties>
                    <d:ID m:type=""Edm.Int32"">0</d:ID>
                    <d:Name>Large Order</d:Name>
                </m:properties>
                </content>
            </entry>
        </feed>
      </m:inline>
    </link>
    <link rel=""http://docs.oasis-open.org/odata/ns/related/Supplier"" type=""application/atom+xml;type=entry"" title=""Supplier"" href=""Products(0)/Supplier"" />
    <category term=""#ODataDemo.Product"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">0</d:ID>
        <d:ReleaseDate m:type=""Edm.DateTimeOffset"">1992-01-01T00:00:00Z</d:ReleaseDate>
        <d:DiscontinuedDate m:type=""Edm.DateTimeOffset"">1993-01-01T00:00:00Z</d:DiscontinuedDate>
        <d:Rating m:type=""Edm.Int32"">4</d:Rating>
        <d:Price m:type=""Edm.Decimal"">2.5</d:Price>
      </m:properties>
    </content>
  </entry>
</feed>";
        private const string OnFeedStart = "OnFeedStarted";
        private const string OnEntryStart = "OnEntryStarted";
        private const string OnNavigationLinkStart = "OnNavigationLinkStarted";
        private const string OnNavigationLinkEnd = "OnNavigationLinkEnded";
        private const string OnFeedEnd = "OnFeedEnded";
        private const string OnEntryEnd = "OnEntryEnded";

        [TestMethod]
        public void ShortIntegrationEventsShouldOccurInReadOrderForAtom()
        {
            var results = TestValidReadWithAllHooked(() =>
            {
                var simulator = new ODataResponseMessageSimulator();
                simulator.SetHeader("Content-Type", "application/atom+xml");
                simulator.SetHeader("Accept", "application/atom+xml");
                simulator.Stream = new MemoryStream(Encoding.UTF8.GetBytes(ProductsWithExpandedCategoryEntry));

                var oms = new ODataMessageReaderSettings() { EnableAtom = true };
                var messageReader = new ODataMessageReader((IODataResponseMessage)simulator, oms);
                return messageReader.CreateODataFeedReader();
            });

            results.Should().HaveCount(18);
            results[0].Validate<ReadingFeedArgs>(OnFeedStart, "http://services.odata.org/OData/OData.svc/Products");
            results[1].Validate<ReadingEntryArgs>(OnEntryStart, "http://services.odata.org/OData/OData.svc/Products(0)");
            results[2].Validate<ReadingNavigationLinkArgs>(OnNavigationLinkStart, "Category");
            results[3].Validate<ReadingEntryArgs>(OnEntryStart, "http://services.odata.org/OData/OData.svc/Categories(0)");
            results[4].Validate<ReadingNavigationLinkArgs>(OnNavigationLinkStart, "Products");
            results[5].Validate<ReadingNavigationLinkArgs>(OnNavigationLinkEnd, "Products");
            results[6].Validate<ReadingEntryArgs>(OnEntryEnd, "http://services.odata.org/OData/OData.svc/Categories(0)");
            results[7].Validate<ReadingNavigationLinkArgs>(OnNavigationLinkEnd, "Category");
            results[8].Validate<ReadingNavigationLinkArgs>(OnNavigationLinkStart, "Orders");
            results[9].Validate<ReadingFeedArgs>(OnFeedStart, "http://services.odata.org/OData/OData.svc/Products(0)/Orders");
            results[10].Validate<ReadingEntryArgs>(OnEntryStart, "http://services.odata.org/OData/OData.svc/Orders(0)");
            results[11].Validate<ReadingEntryArgs>(OnEntryEnd, "http://services.odata.org/OData/OData.svc/Orders(0)");
            results[12].Validate<ReadingFeedArgs>(OnFeedEnd, "http://services.odata.org/OData/OData.svc/Products(0)/Orders");
            results[13].Validate<ReadingNavigationLinkArgs>(OnNavigationLinkEnd, "Orders");
            results[14].Validate<ReadingNavigationLinkArgs>(OnNavigationLinkStart, "Supplier");
            results[15].Validate<ReadingNavigationLinkArgs>(OnNavigationLinkEnd, "Supplier");
            results[16].Validate<ReadingEntryArgs>(OnEntryEnd, "http://services.odata.org/OData/OData.svc/Products(0)");
            results[17].Validate<ReadingFeedArgs>(OnFeedEnd, "http://services.odata.org/OData/OData.svc/Products");
        }

        [TestMethod]
        public void ShouldRaiseEntryStart()
        {
            this.TestConfigureAction<ODataEntry>((config) =>
            {
                config.OnEntryStarted((ReadingEntryArgs args) => args.Entry.Id = new Uri("http://foo.org"));
                return ODataReaderState.EntryStart;
            },
            (entry) => entry.Id.Should().Be(new Uri("http://foo.org")));
        }

        [TestMethod]
        public void ShouldRaiseEntryEnd()
        {
            this.TestConfigureAction<ODataEntry>((config) =>
            {
                config.OnEntryEnded((ReadingEntryArgs args) => args.Entry.Id = new Uri("http://foo.org"));
                return ODataReaderState.EntryEnd;
            },
            (entry) => entry.Id.Should().Be(new Uri("http://foo.org")));
        }

        [TestMethod]
        public void ShouldRaiseFeedStart()
        {
            this.TestConfigureAction<ODataFeed>((config) =>
            {
                config.OnFeedStarted((ReadingFeedArgs args) => args.Feed.Id = new Uri("urn:foo"));
                return ODataReaderState.FeedStart;
            },
            (feed) => feed.Id.Should().Be(new Uri("urn:foo")));
        }

        [TestMethod]
        public void ShouldRaiseFeedEnd()
        {
            this.TestConfigureAction<ODataFeed>((config) =>
            {
                config.OnFeedEnded((ReadingFeedArgs args) => args.Feed.Id = new Uri("urn:foo"));
                return ODataReaderState.FeedEnd;
            },
            (feed) => feed.Id.Should().Be(new Uri("urn:foo")));
        }

        [TestMethod]
        public void ShouldRaiseNavigationLinkStart()
        {
            this.TestConfigureAction<ODataNavigationLink>((config) =>
            {
                config.OnNavigationLinkStarted((ReadingNavigationLinkArgs args) => args.Link.Name = "foo");
                return ODataReaderState.NavigationLinkStart;
            },
            (link) => link.Name.Should().Be("foo"));
        }

        [TestMethod]
        public void ShouldRaiseNavigationLinkEnd()
        {
            this.TestConfigureAction<ODataNavigationLink>((config) =>
            {
                config.OnNavigationLinkEnded((ReadingNavigationLinkArgs args) => args.Link.Name = "foo");
                return ODataReaderState.NavigationLinkEnd;
            },
            (link) => link.Name.Should().Be("foo"));
        }

        [TestMethod]
        public void NoEventShouldBeFiredWhenReadIsFalse()
        {
            bool eventFiredIncorrectly = false;
            TestODataReader reader = new TestODataReader() { new TestODataReaderItem(ODataReaderState.EntryStart, new ODataEntry()) };
            reader.ReadFunc = () => false;
            var responsePipeline = new DataServiceClientResponsePipelineConfiguration(new DataServiceContext(new Uri("http://www.foo.com")));
            responsePipeline.OnEntryStarted((ReadingEntryArgs args) => eventFiredIncorrectly = true);
            var odataReaderWrapper = ODataReaderWrapper.CreateForTest(reader, responsePipeline);
            odataReaderWrapper.Read();

            Assert.IsFalse(eventFiredIncorrectly);
        }

        public void TestConfigureAction<T>(Func<DataServiceClientResponsePipelineConfiguration, ODataReaderState> setup, Action<T> verify) where T : ODataItem, new()
        {
            var item = new T();
            var responsePipeline = new DataServiceClientResponsePipelineConfiguration(new DataServiceContext(new Uri("http://www.foo.com")));
            var readerState = setup(responsePipeline);
            var reader = new TestODataReader() { new TestODataReaderItem(readerState, item) };
            var odataReaderWrapper = ODataReaderWrapper.CreateForTest(reader, responsePipeline);
            odataReaderWrapper.Read();
            verify(item);
        }

        [TestMethod]
        public void ShouldNotErrorOrRaiseyEventsForOtherReaderStateChanges()
        {
            var results = TestValidReadWithAllHooked(() =>
            {
                return new TestODataReader()
                {
                    new TestODataReaderItem(ODataReaderState.Completed, null),
                    new TestODataReaderItem(ODataReaderState.EntityReferenceLink, null),
                    new TestODataReaderItem(ODataReaderState.Exception, null),
                    new TestODataReaderItem(ODataReaderState.Start, null)
                };
            });
            results.Should().HaveCount(0);
        }

        internal List<KeyValuePair<string, object>> TestValidReadWithAllHooked(Func<ODataReader> createOdataReader)
        {
            List<KeyValuePair<string, object>> results = new List<KeyValuePair<string, object>>();
            var odataReader = createOdataReader();
            DataServiceContext context = new DataServiceContext(new Uri("http://www.foo.com"));
            context.EnableAtom = true;
            var responsePipeline = new DataServiceClientResponsePipelineConfiguration(context);
            responsePipeline.OnEntryEnded(args => results.Add(new KeyValuePair<string, object>("OnEntryEnded", args)));
            responsePipeline.OnEntryStarted(args => results.Add(new KeyValuePair<string, object>("OnEntryStarted", args)));
            responsePipeline.OnFeedStarted(args => results.Add(new KeyValuePair<string, object>("OnFeedStarted", args)));
            responsePipeline.OnFeedEnded(args => results.Add(new KeyValuePair<string, object>("OnFeedEnded", args)));
            responsePipeline.OnNavigationLinkEnded(args => results.Add(new KeyValuePair<string, object>("OnNavigationLinkEnded", args)));
            responsePipeline.OnNavigationLinkStarted(args => results.Add(new KeyValuePair<string, object>("OnNavigationLinkStarted", args)));
            var odataReaderTracker = ODataReaderWrapper.CreateForTest(odataReader, responsePipeline);
            while (odataReaderTracker.Read()) { }

            return results;
        }
    }

    /// <summary>
    /// Holds information on the event that occurred and the args that it passed
    /// </summary>
    internal static class Verification
    {
        public static void Validate<TExpectedEventArgsType>(this KeyValuePair<string, object> returnedValue, string expectedEventName, string expectedValue)
        {
            returnedValue.Value.Should().BeAssignableTo<TExpectedEventArgsType>();
            returnedValue.Key.Should().Be(expectedEventName);
            switch (typeof(TExpectedEventArgsType).Name)
            {
                case "ReadingEntryArgs":
                    var odataEntryEventArgs = (ReadingEntryArgs)returnedValue.Value;
                    odataEntryEventArgs.Entry.Id.Should().Be(expectedValue);
                    break;
                case "ReadingFeedArgs":
                    var feedArgs = (ReadingFeedArgs)returnedValue.Value;
                    feedArgs.Feed.Id.Should().Be(expectedValue);
                    break;
                case "ReadingNavigationLinkArgs":
                    var navArgs = (ReadingNavigationLinkArgs)returnedValue.Value;
                    navArgs.Link.Name.Should().Be(expectedValue);
                    break;
                default:
                    throw new InternalTestFailureException("Shouldn't get here");
            }
        }
    }
}