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

public class StreamWriterChunkedByteStreamingTests
{
    [Fact]
    public async Task CanWriteByteArrayChunksInAsyncAPI()
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "Title",
            ContentLength = 512
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
                        var buffer = ArrayPool<byte>.Shared.Rent(4096);

                        var bytesRead = await stream.ReadAsync(buffer);
                        do
                        {
                            var read = buffer[..bytesRead];
                            writer.WriteBinarySegment(read, isFinalBlock: false, state);
                            bytesRead = await stream.ReadAsync(buffer);

                            // TODO check for flushing
                        } while (bytesRead > 0);

                        writer.WriteBinarySegment(ReadOnlySpan<byte>.Empty, isFinalBlock: true, state);

                        ArrayPool<byte>.Shared.Return(buffer);
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
        var actualBase64String = decoded.RootElement.GetProperty("value")[0].GetProperty("Contents").GetString();
  
        var expectedBase64String = Convert.ToBase64String(entity.GetContents().ToArray());

        Assert.Equal(expectedBase64String, actualBase64String);
    }

    private static IEdmModel CreateBlogPostModel()
    {
        var model = new EdmModel();

        var entityType = model.AddEntityType("ns", "BlogPost");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        entityType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String);
        entityType.AddStructuralProperty("Contents", EdmPrimitiveTypeKind.Binary);

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
            var data = Enumerable.Range(0, ContentLength).Select(i => (byte)(i % 256)).ToArray();
            return new MemoryStream(data);
        }
    }
}
