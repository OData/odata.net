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
    using FluentAssertions;
    using Microsoft.OData;
    using Xunit;

    public class ODataReaderWrapperWithEventsUnitTests
    {
        [Fact]
        public void ShouldRaiseEntryStart()
        {
            this.TestConfigureAction<ODataResource>((config) =>
            {
                config.OnEntryStarted((ReadingEntryArgs args) => args.Entry.Id = new Uri("http://foo.org"));
                return ODataReaderState.ResourceStart;
            },
            (entry) => entry.Id.Should().Be(new Uri("http://foo.org")));
        }

        [Fact]
        public void ShouldRaiseEntryEnd()
        {
            this.TestConfigureAction<ODataResource>((config) =>
            {
                config.OnEntryEnded((ReadingEntryArgs args) => args.Entry.Id = new Uri("http://foo.org"));
                return ODataReaderState.ResourceEnd;
            },
            (entry) => entry.Id.Should().Be(new Uri("http://foo.org")));
        }

        [Fact]
        public void ShouldRaiseFeedStart()
        {
            this.TestConfigureAction<ODataResourceSet>((config) =>
            {
                config.OnFeedStarted((ReadingFeedArgs args) => args.Feed.Id = new Uri("urn:foo"));
                return ODataReaderState.ResourceSetStart;
            },
            (feed) => feed.Id.Should().Be(new Uri("urn:foo")));
        }

        [Fact]
        public void ShouldRaiseFeedEnd()
        {
            this.TestConfigureAction<ODataResourceSet>((config) =>
            {
                config.OnFeedEnded((ReadingFeedArgs args) => args.Feed.Id = new Uri("urn:foo"));
                return ODataReaderState.ResourceSetEnd;
            },
            (feed) => feed.Id.Should().Be(new Uri("urn:foo")));
        }

        [Fact]
        public void ShouldRaiseNestedResourceInfoStart()
        {
            this.TestConfigureAction<ODataNestedResourceInfo>((config) =>
            {
                config.OnNestedResourceInfoStarted((ReadingNestedResourceInfoArgs args) => args.Link.Name = "foo");
                return ODataReaderState.NestedResourceInfoStart;
            },
            (link) => link.Name.Should().Be("foo"));
        }

        [Fact]
        public void ShouldRaiseNestedResourceInfoEnd()
        {
            this.TestConfigureAction<ODataNestedResourceInfo>((config) =>
            {
                config.OnNestedResourceInfoEnded((ReadingNestedResourceInfoArgs args) => args.Link.Name = "foo");
                return ODataReaderState.NestedResourceInfoEnd;
            },
            (link) => link.Name.Should().Be("foo"));
        }

        [Fact]
        public void NoEventShouldBeFiredWhenReadIsFalse()
        {
            bool eventFiredIncorrectly = false;
            TestODataReader reader = new TestODataReader() { new TestODataReaderItem(ODataReaderState.ResourceStart, new ODataResource()) };
            reader.ReadFunc = () => false;
            var responsePipeline = new DataServiceClientResponsePipelineConfiguration(new DataServiceContext(new Uri("http://www.foo.com")));
            responsePipeline.OnEntryStarted((ReadingEntryArgs args) => eventFiredIncorrectly = true);
            var odataReaderWrapper = ODataReaderWrapper.CreateForTest(reader, responsePipeline);
            odataReaderWrapper.Read();

            Assert.False(eventFiredIncorrectly);
        }

        internal void TestConfigureAction<T>(Func<DataServiceClientResponsePipelineConfiguration, ODataReaderState> setup, Action<T> verify) where T : ODataItem, new()
        {
            var item = new T();
            var responsePipeline = new DataServiceClientResponsePipelineConfiguration(new DataServiceContext(new Uri("http://www.foo.com")));
            var readerState = setup(responsePipeline);
            var reader = new TestODataReader() { new TestODataReaderItem(readerState, item) };
            var odataReaderWrapper = ODataReaderWrapper.CreateForTest(reader, responsePipeline);
            odataReaderWrapper.Read();
            verify(item);
        }

        [Fact]
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
            // context.EnableAtom = true;
            var responsePipeline = new DataServiceClientResponsePipelineConfiguration(context);
            responsePipeline.OnEntryEnded(args => results.Add(new KeyValuePair<string, object>("OnEntryEnded", args)));
            responsePipeline.OnEntryStarted(args => results.Add(new KeyValuePair<string, object>("OnEntryStarted", args)));
            responsePipeline.OnFeedStarted(args => results.Add(new KeyValuePair<string, object>("OnFeedStarted", args)));
            responsePipeline.OnFeedEnded(args => results.Add(new KeyValuePair<string, object>("OnFeedEnded", args)));
            responsePipeline.OnNestedResourceInfoEnded(args => results.Add(new KeyValuePair<string, object>("OnNestedResourceInfoEnded", args)));
            responsePipeline.OnNestedResourceInfoStarted(args => results.Add(new KeyValuePair<string, object>("OnNestedResourceInfoStarted", args)));
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
                case "ReadingNestedResourceInfoArgs":
                    var navArgs = (ReadingNestedResourceInfoArgs)returnedValue.Value;
                    navArgs.Link.Name.Should().Be(expectedValue);
                    break;
                default:
                    throw new Exception("Shouldn't get here");
            }
        }
    }
}