using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json.State;
using Microsoft.OData.UriParser;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.V3;

public class BinaryStreamingBase64WriterTests
{
    [Theory]
    [InlineData(512)]
    [InlineData(50_000)]
    public async Task CanWriteBinaryStringFromPipeReader(int binaryDataSize)
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = CreateByteArray(binaryDataSize)
        };

        var readerFactory = new PipeReaderFactory();

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions<PipeReaderState>();


        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, PipeReaderState>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, PipeReaderState>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, PipeReader, PipeReaderState>()
                {
                    Name = "CoverImage",
                    
                    GetStreamingValue = (post, state) => state.CustomState.ReaderFactory.GetContentsPipeReader(post)
                }
            ]
        });

        var output = new MemoryStream();

        var model = CreateBlogPostModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("BlogPosts", UriKind.Relative)
        ).ParseUri();

        var customState = new PipeReaderState
        {
            ReaderFactory = readerFactory
        };

        await ODataSerializer.WriteAsync(payload, output, odataUri, model, options, customState);

        output.Position = 0;
        var decoded = JsonDocument.Parse(output);
        var actualBase64String = decoded.RootElement.GetProperty("value")[0].GetProperty("CoverImage").GetString();
        var expectedBase64String = Convert.ToBase64String(entity.CoverImage);

        Assert.Equal(expectedBase64String, actualBase64String);
    }


    [Theory]
    [InlineData(512)]
    [InlineData(50_000)]
    public async Task CanWriteBinaryStringFromAsyncEnumerable(int binaryDataSize)
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = CreateByteArray(binaryDataSize)
        };

        var readerFactory = new AsyncEnumerableFactory();

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions<AsyncEnumerableState>();


        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, AsyncEnumerableState>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, AsyncEnumerableState>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, PipeReader, AsyncEnumerableState>()
                {
                    Name = "CoverImage",
                    
                    GetStreamingValue = (post, state) => state.CustomState.ReaderFactory.GetContentStream(post)
                }
            ]
        });

        var output = new MemoryStream();

        var model = CreateBlogPostModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("BlogPosts", UriKind.Relative)
        ).ParseUri();

        var customState = new AsyncEnumerableState
        {
            ReaderFactory = readerFactory
        };

        await ODataSerializer.WriteAsync(payload, output, odataUri, model, options, customState);

        output.Position = 0;
        var decoded = JsonDocument.Parse(output);
        var actualBase64String = decoded.RootElement.GetProperty("value")[0].GetProperty("CoverImage").GetString();
        var expectedBase64String = Convert.ToBase64String(entity.CoverImage);

        Assert.Equal(expectedBase64String, actualBase64String);
    }

    class PipeReaderFactory
    {

        public PipeReader GetContentsPipeReader(BlogPost post)
        {
            // Simulate some activity that fetches the pipe reader
            // e.g. opens a stream or reads from a network source.
            // this assumes that activity synchronous, but we should also consider async support.
            return PipeReader.Create(new MemoryStream(post.CoverImage));
        }
    }

    class AsyncEnumerableFactory
    {
        public async IAsyncEnumerable<ReadOnlyMemory<byte>> GetContentStream(BlogPost post)
        {
            const int bufferSize = 4096;
            int offset = 0;
            while (offset < post.CoverImage.Length)
            {
                int segmentSize = Math.Min(post.CoverImage.Length - offset, bufferSize);
                var segment = post.CoverImage.AsMemory().Slice(offset, segmentSize);
                yield return segment;
                await Task.Yield();

                offset += segment.Length;
            }
        }
    }

    struct PipeReaderState
    {
        public PipeReaderFactory ReaderFactory { get; set; }
    }

    struct AsyncEnumerableState
    {
        public AsyncEnumerableFactory ReaderFactory { get; set; }
    }

    private static byte[] CreateByteArray(int size) => [.. Enumerable.Range(0, size).Select(i => (byte)i)];

    private static IEdmModel CreateBlogPostModel()
    {
        var model = new EdmModel();

        var entityType = model.AddEntityType("ns", "BlogPost");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        entityType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);
        entityType.AddStructuralProperty("CoverImage", EdmPrimitiveTypeKind.Binary);

        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("BlogPosts", entityType);
        return model;
    }

    class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public byte[] CoverImage { get; set; }
    }
}
