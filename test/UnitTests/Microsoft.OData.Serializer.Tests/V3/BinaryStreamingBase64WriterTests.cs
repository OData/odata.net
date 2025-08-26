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

    [Theory]
    [InlineData(512)]
    [InlineData(50_000)]
    public async Task CanWriteBinaryStringFromStream(int binaryDataSize)
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = CreateByteArray(binaryDataSize)
        };

        var readerFactory = new AsyncEnumerableFactory();

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions();


        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, DefaultState>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, DefaultState>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, Stream, DefaultState>()
                {
                    Name = "CoverImage",

                    GetStreamingValue = (post, state) => new MemoryStream(post.CoverImage)
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

        await ODataSerializer.WriteAsync(payload, output, odataUri, model, options);

        output.Position = 0;
        var decoded = JsonDocument.Parse(output);
        var actualBase64String = decoded.RootElement.GetProperty("value")[0].GetProperty("CoverImage").GetString();
        var expectedBase64String = Convert.ToBase64String(entity.CoverImage);

        Assert.Equal(expectedBase64String, actualBase64String);
    }

    [Fact]
    public async Task CompletesPipeReaderByDefault()
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = CreateByteArray(512)
        };

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions<Dictionary<int, PipeReader>>();

        var stream = new MemoryStream(entity.CoverImage);
        var reader = PipeReader.Create(stream);
        var state = new Dictionary<int, PipeReader>
        {
            { entity.Id, reader }
        };


        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, Dictionary<int, PipeReader>>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, Dictionary<int, PipeReader>>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, Stream, Dictionary<int, PipeReader>>()
                {
                    Name = "CoverImage",

                    GetStreamingValue = (post, state) => state.CustomState[post.Id]
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

        await ODataSerializer.WriteAsync(payload, output, odataUri, model, options, state);

        Assert.Throws<InvalidOperationException>(() => reader.TryRead(out _));
        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }


    [Fact]
    public async Task DisposesStreamByDefault()
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = CreateByteArray(512)
        };

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions<Dictionary<int, Stream>>();

        var stream = new MemoryStream(entity.CoverImage);
        var state = new Dictionary<int, Stream>
        {
            { entity.Id, stream }
        };


        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, Dictionary<int, Stream>>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, Dictionary<int, Stream>>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, Stream, Dictionary<int, Stream>>()
                {
                    Name = "CoverImage",

                    GetStreamingValue = (post, state) => state.CustomState[post.Id]
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

        await ODataSerializer.WriteAsync(payload, output, odataUri, model, options, state);

        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }

    [Fact]
    public async Task DisposesStreamEvenIfExceptionOccurs()
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = []
        };

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions<Dictionary<int, Stream>>();

        var stream = new StreamWithException();
        var state = new Dictionary<int, Stream>
        {
            { entity.Id, stream }
        };


        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, Dictionary<int, Stream>>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, Dictionary<int, Stream>>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, Stream, Dictionary<int, Stream>>()
                {
                    Name = "CoverImage",

                    GetStreamingValue = (post, state) => state.CustomState[post.Id]
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

        await Assert.ThrowsAnyAsync<Exception>(async () => await ODataSerializer.WriteAsync(payload, output, odataUri, model, options, state));

        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }

    [Fact]
    public async Task DoesNotDisposeStreamIfLeaveOpenSetToTrue()
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = CreateByteArray(512)
        };

        var readerFactory = new AsyncEnumerableFactory();

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions<Dictionary<int, Stream>>();

        var stream = new MemoryStream(entity.CoverImage);
        var state = new Dictionary<int, Stream>
        {
            { entity.Id, stream }
        };


        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, Dictionary<int, Stream>>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, Dictionary<int, Stream>>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, Stream, Dictionary<int, Stream>>()
                {
                    Name = "CoverImage",

                    GetStreamingValue = (post, state) => state.CustomState[post.Id],
                    LeaveStreamOpen = true
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

        await ODataSerializer.WriteAsync(payload, output, odataUri, model, options, state);

        stream.Position = 0;
        Assert.Equal(0, stream.ReadByte());
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

    class StreamWithException : Stream
    {
        bool disposed;
        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ObjectDisposedException.ThrowIf(disposed, nameof(StreamWithException));
            throw new NotImplementedException();
        }

        public override int ReadByte()
        {
            ObjectDisposedException.ThrowIf(disposed, nameof(StreamWithException));
            return base.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ObjectDisposedException.ThrowIf(disposed, nameof(StreamWithException));
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            ObjectDisposedException.ThrowIf(disposed, nameof(StreamWithException));
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ObjectDisposedException.ThrowIf(disposed, nameof(StreamWithException));
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            disposed = true;
            base.Dispose(disposing);
        }

        public override ValueTask DisposeAsync()
        {
            disposed = true;
            return base.DisposeAsync();
        }
    }
}
