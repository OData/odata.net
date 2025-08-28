using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json.State;
using Microsoft.OData.UriParser;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.V3;

public class StreamWriterChunkedTextStreamingTests
{
    [Theory]
    [InlineData(525)]
    [InlineData(15_000)]
    [InlineData(50_000)]
    public async Task CanWriteStringChunksInAsyncAPI(int contentLength)
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "Title",
            ContentLength = contentLength
        };

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
                new()
                {
                    Name = "Contents",
                    WriteValueAsync = static async (post, writer, state) =>
                    {
                        using var stream = post.GetContents();
                        var decoder = Encoding.UTF8.GetDecoder();
                        var buffer = ArrayPool<byte>.Shared.Rent(4096);
                        var decodedBuffer = ArrayPool<char>.Shared.Rent(4096);

                        var bytesRead = await stream.ReadAsync(buffer);
                        do
                        {
                            var read = buffer.AsMemory()[..bytesRead];
                            int numDecoded = decoder.GetChars(read.Span, decodedBuffer, flush: false);
                            var decodedChars = decodedBuffer.AsMemory()[..numDecoded];

                            writer.WriteStringSegment(decodedChars.Span, isFinalBlock: false, state);

                            bytesRead = await stream.ReadAsync(buffer);

                            // TODO to avoid growing the internal buffer, you may want to trigger manual flushes in-between writes
                            // e.g. if (checkIfSomeThresholdIsMet) { await writer.FlushAsync(state) };
                            // But we also tentatively provide convenience below. Not sure if this is a good idea to expose, not sure if it'll be final release
                            await writer.FlushIfBufferGettingFullAsync(state);
                        } while (bytesRead > 0);

                        int finalChars = decoder.GetChars([], decodedBuffer, flush: true);
                        writer.WriteStringSegment(decodedBuffer.AsMemory()[..finalChars].Span, isFinalBlock: true, state);

                        ArrayPool<byte>.Shared.Return(buffer);
                        ArrayPool<char>.Shared.Return(decodedBuffer);
                    }
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
        var actualString = decoded.RootElement.GetProperty("value")[0].GetProperty("Contents").GetString();

        var expectedString = new StreamReader(entity.GetContents()).ReadToEnd();

        Assert.Equal(expectedString, actualString);
    }

    private static IEdmModel CreateBlogPostModel()
    {
        var model = new EdmModel();

        var entityType = model.AddEntityType("ns", "BlogPost");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        entityType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);
        entityType.AddStructuralProperty("Contents", EdmPrimitiveTypeKind.String);

        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("BlogPosts", entityType);
        return model;
    }

    class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public required int ContentLength { get; set; }

        public MemoryStream GetContents()
        {
            var data = Enumerable.Range(0, ContentLength).Select(i => (char)(i % char.MaxValue)).ToArray();
            return new MemoryStream(Encoding.UTF8.GetBytes(data));
        }
    }
}
