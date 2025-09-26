using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Attributes;
using Microsoft.OData.UriParser;
using System.Text.Json;

namespace Microsoft.OData.Serializer.Tests.V3;

public class NullSerializationSupport
{
    [Fact]
    public async Task WhenStringPropertyIsNull_WritesNullValue()
    {
        // Arrange
        var post = new Post
        {
            Id = 1,
            Title = "Test Post",
            Description = null
        };

        var options = new ODataSerializerOptions();

        var odataUri = new ODataUriParser(
            CreateModel(),
            new Uri("http://service/odata"),
            new Uri("Posts(1)", UriKind.Relative)
        ).ParseUri();

        var model = CreateModel();

        var stream = new MemoryStream();
        // Act
        await ODataSerializer.WriteAsync(post, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Posts/$entity",
              "Id": 1,
              "Title": "Test Post",
              "Description": null
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel CreateModel()
    {
        var model = new EdmModel();
        var entityType = model.AddEntityType("ns", "Post");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
        entityType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String, isNullable: false);
        entityType.AddStructuralProperty("Description", EdmPrimitiveTypeKind.String, isNullable: true);

        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("Posts", entityType);
        return model;
    }

    [ODataType("ns.Post")]
    class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
