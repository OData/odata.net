//-----------------------------------------------------------------------------
// <copyright file="ContentTypeCacheTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
//------------------------------------------------------------------------------

using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.ContentTypeTests;

[CollectionDefinition("ContentType cache", DisableParallelization = true)]
public class ContentTypeCacheCollection
{
}

[Collection("ContentType cache")]
public class ContentTypeCacheTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void MissingOrEmptyContentTypeShouldUseDefaultFormat(string? contentType)
    {
        MediaTypeUtils.ClearCache();

        using MemoryStream stream = new MemoryStream();
        TestStreamRequestMessage requestMessage = new TestStreamRequestMessage(stream, new Uri("https://example.org/odata/Entities"), "POST");
        if (contentType != null)
        {
            requestMessage.SetHeader("Content-Type", contentType);
        }

        using ODataMessageReader messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), new EdmModel());
        ODataReader reader = messageReader.CreateODataResourceReader();

        Assert.NotNull(reader);

        string cachedContentType = Assert.Single(MediaTypeUtils.GetCacheKeys());
        Assert.Equal("application/json", cachedContentType);
    }

    [Fact]
    public void UniqueContentTypesBeyondLimitShouldRemainBoundedThroughMessageReader()
    {
        MediaTypeUtils.ClearCache();

        // Fill the cache to its maximum size and ensure that the first content type is evicted from the cache.
        for (int i = 0; i <= MediaTypeUtils.MatchInfoCacheMaxSize; i++)
        {
            using MemoryStream stream = new MemoryStream();
            TestStreamRequestMessage requestMessage = new TestStreamRequestMessage(stream, new Uri("https://example.org/odata/Entities"), "POST");
            requestMessage.SetHeader("Content-Type", $"application/json;cache-e2e={i}");
            using ODataMessageReader messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), new EdmModel());
            ODataReader reader = messageReader.CreateODataResourceReader();

            Assert.NotNull(reader);
        }

        string onlyContentType = Assert.Single(MediaTypeUtils.GetCacheKeys());
        Assert.NotNull(onlyContentType);
        Assert.Equal($"application/json;cache-e2e={MediaTypeUtils.MatchInfoCacheMaxSize}", onlyContentType);
    }

    [Fact]
    public void ContentTypesShouldContinueResolvingAfterCacheRollover()
    {
        const string firstContentType = "application/json;cache-e2e=0";
        MediaTypeUtils.ClearCache();

        // Fill the cache to its maximum size and ensure that the first content type is evicted from the cache.
        for (int i = 0; i <= MediaTypeUtils.MatchInfoCacheMaxSize; i++)
        {
            using MemoryStream stream = new MemoryStream();
            TestStreamRequestMessage requestMessage = new TestStreamRequestMessage(stream, new Uri("https://example.org/odata/Entities"), "POST");
            requestMessage.SetHeader("Content-Type", $"application/json;cache-e2e={i}");
            using ODataMessageReader messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), new EdmModel());
            ODataReader reader = messageReader.CreateODataResourceReader();

            Assert.NotNull(reader);
        }

        Assert.DoesNotContain(firstContentType, MediaTypeUtils.GetCacheKeys());

        using MemoryStream firstStream = new MemoryStream();
        TestStreamRequestMessage firstRequestMessage = new TestStreamRequestMessage(firstStream, new Uri("https://example.org/odata/Entities"), "POST");
        firstRequestMessage.SetHeader("Content-Type", firstContentType);
        using ODataMessageReader firstMessageReader = new ODataMessageReader(firstRequestMessage, new ODataMessageReaderSettings(), new EdmModel());
        ODataReader firstReader = firstMessageReader.CreateODataResourceReader();

        using MemoryStream standardStream = new MemoryStream();
        TestStreamRequestMessage standardRequestMessage = new TestStreamRequestMessage(standardStream, new Uri("https://example.org/odata/Entities"), "POST");
        standardRequestMessage.SetHeader("Content-Type", "application/json");
        using ODataMessageReader standardMessageReader = new ODataMessageReader(standardRequestMessage, new ODataMessageReaderSettings(), new EdmModel());
        ODataReader standardReader = standardMessageReader.CreateODataResourceReader();

        Assert.NotNull(firstReader);
        Assert.NotNull(standardReader);
        Assert.Contains(firstContentType, MediaTypeUtils.GetCacheKeys());
        Assert.Contains("application/json", MediaTypeUtils.GetCacheKeys());
    }
}
