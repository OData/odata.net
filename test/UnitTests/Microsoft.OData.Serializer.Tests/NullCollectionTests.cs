using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests;

public class NullCollectionTests
{
    [Fact]
    public async Task WritesNullWhenListCollectionPropertyIsNull()
    {
        // Arrange
        var entity = new EntityWithList
        {
            Id = 1,
            Items = null
        };

        var options = new ODataSerializerOptions();

        var model = CreateModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();

        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(entity, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": []
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WritesNullWhenReadOnlyListCollectionPropertyIsNull()
    {
        // Arrange
        var order = new EntityWithReadOnlyList
        {
            Id = 1,
            Items = null
        };

        var options = new ODataSerializerOptions();
        var odataUri = new ODataUriParser(
            CreateModel(),
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();
        var model = CreateModel();
        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(order, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": []
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WritesNullWhenArrayCollectionPropertyIsNull()
    {
        // Arrange
        var order = new EntityWithArray
        {
            Id = 1,
            Items = null
        };

        var options = new ODataSerializerOptions();
        var odataUri = new ODataUriParser(
            CreateModel(),
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();
        var model = CreateModel();
        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(order, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": []
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WritesNullWhenEnumerableOfTCollectionPropertyIsNull()
    {
        // Arrange
        var order = new EntityWithEnumerableOfT
        {
            Id = 1,
            Items = null
        };

        var options = new ODataSerializerOptions();
        var odataUri = new ODataUriParser(
            CreateModel(),
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();
        var model = CreateModel();
        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(order, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": []
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WritesNullWhenEnumerablePropertyIsNull()
    {
        // Arrange
        var order = new EntityWithEnumerable
        {
            Id = 1,
            Items = null
        };

        var options = new ODataSerializerOptions();
        var odataUri = new ODataUriParser(
            CreateModel(),
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();
        var model = CreateModel();
        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(order, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": []
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WhenTopLevelCollectionIsNull_WritesEmptyArray()
    {
        List<EntityWithList> data = null;

        var options = new ODataSerializerOptions();
        var odataUri = new ODataUriParser(
            CreateModel(),
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();
        var model = CreateModel();
        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(data, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected = """
            {
              "value": []
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private IEdmModel CreateModel()
    {
        var model = new EdmModel();
        var entityType = model.AddEntityType("ns", "Entity");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
        var itemsProperty = entityType.AddStructuralProperty(
            "Items",
            new EdmCollectionTypeReference(
                new EdmCollectionType(
                    new EdmPrimitiveTypeReference(
                        EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false))));
        model.AddElement(entityType);
        var container = new EdmEntityContainer("ns", "Container");
        model.AddElement(container);
        container.AddEntitySet("Entities", entityType);
        return model;
    }

    [ODataType("ns.Entity")]
    class EntityWithList
    {
        public int Id { get; set; }
        public List<string>? Items { get; set; }
    }

    [ODataType("ns.Entity")]
    class EntityWithReadOnlyList
    {
        public int Id { get; set; }
        public IReadOnlyList<string>? Items { get; set; }
    }

    [ODataType("ns.Entity")]
    class EntityWithArray
    {
        public int Id { get; set; }
        public string[]? Items { get; set; }
    }

    [ODataType("ns.Entity")]
    class EntityWithEnumerableOfT
    {
        public int Id { get; set; }
        public IEnumerable<string>? Items { get; set; }
    }

    [ODataType("ns.Entity")]
    class EntityWithEnumerable
    {
        public int Id { get; set; }
        public IEnumerable? Items { get; set; }
    }
}
