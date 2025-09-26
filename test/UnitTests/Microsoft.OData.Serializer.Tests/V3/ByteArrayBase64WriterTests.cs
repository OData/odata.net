using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Json.State;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.UriParser;
using System.Reflection;
using System.Text.Json;

namespace Microsoft.OData.Serializer.Tests.V3;

public class ByteArrayBase64WriterTests
{
    [Theory]
    [InlineData(512)]
    [InlineData(50_000)]
    public async Task CanWriteByteArrayField(int binaryDataSize)
    {
        var entity = new BlogPost
        {
            Id = 1,
            Title = "First Post",
            CoverImage = CreateByteArray(binaryDataSize)
        };

        var payload = new List<BlogPost> { entity };

        var options = new ODataSerializerOptions();

        
        options.AddTypeInfo<BlogPost>(new ()
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
                new ODataPropertyInfo<BlogPost, byte[], DefaultState>()
                {
                    Name = "CoverImage",
                    GetValue = (post, state) => post.CoverImage
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
