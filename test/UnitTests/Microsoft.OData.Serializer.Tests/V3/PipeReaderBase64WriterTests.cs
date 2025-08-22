using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json.State;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.V3;

public class PipeReaderBase64WriterTests
{
    [Fact]
    public async Task CanWriteSmallBinaryStringFromPipeReader()
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = CreateByteArray(512)
        };

        var readerFactory = new PipeReaderFactory();

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions<CustomState>();


        options.AddTypeInfo<BlogPost>(new()
        {
            Properties = [
                new ODataPropertyInfo<BlogPost, int, CustomState>()
                {
                    Name = "Id",
                    GetValue = (post, state) => post.Id
                },
                new ODataPropertyInfo<BlogPost, string, CustomState>()
                {
                    Name = "Title",
                    GetValue = (post, state) => post.Title
                },
                new ODataPropertyInfo<BlogPost, PipeReader, CustomState>()
                {
                    Name = "CoverImage",
                    GetValue = (post, state) => state.CurrentCustomState().ReaderFactory.GetContentsPipeReader(post)
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

    struct CustomState
    {
        public PipeReaderFactory ReaderFactory { get; set; }
    }

    private static byte[] CreateByteArray(int size)
    {
        var array = new byte[size];

        for (int i = 0; i < size; i++)
        {
            array[i] = (byte)(i % 256);
        }

        return array;
    }

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
