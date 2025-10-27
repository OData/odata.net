using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections;
using System.Text.Json;

namespace Microsoft.OData.Serializer.Tests;

public class SupportForEnumerablePropertiesWithMultipleGenericTypes
{
    [Fact]
    public async Task CanSerializeDynamicallyTypedEnumerableWithMultipleGenericParameters()
    {
        List<int> source = [1, 2, 3, 4, 5];
        var entity = new EntityWithObjectProperty
        {
            Id = 1,
            Items = new FilterMap<int, string>(
                source,
                x => x % 2 == 0,
                x => $"Number {x}")
        };

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();
        var stream = new MemoryStream();
        await ODataSerializer.WriteAsync(entity, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": [
                "Number 2",
                "Number 4"
              ]
            }
            """;

        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task CanSerializeIEnumerableEnumerableWithMultipleGenericParameters()
    {
        List<int> source = [1, 2, 3, 4, 5];
        var entity = new EntityWithIEnumerableProperty
        {
            Id = 1,
            Items = new FilterMap<int, string>(
                source,
                x => x % 2 == 0,
                x => $"Number {x}")
        };

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();
        var stream = new MemoryStream();
        await ODataSerializer.WriteAsync(entity, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": [
                "Number 2",
                "Number 4"
              ]
            }
            """;

        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task CanSerializeGenericIEnumerableEnumerableWithMultipleGenericParameters()
    {
        List<int> source = [1, 2, 3, 4, 5];
        var entity = new EntityWithGenericEnumerableProperty
        {
            Id = 1,
            Items = new FilterMap<int, string>(
                source,
                x => x % 2 == 0,
                x => $"Number {x}")
        };

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();
        var stream = new MemoryStream();
        await ODataSerializer.WriteAsync(entity, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": [
                "Number 2",
                "Number 4"
              ]
            }
            """;

        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task CanSerializeGeneriMultiGenericTypeThatImplementsIEnumerable()
    {
        List<int> source = [1, 2, 3, 4, 5];
        var entity = new EntityWithFilterMapProperty
        {
            Id = 1,
            Items = new FilterMap<int, string>(
                source,
                x => x % 2 == 0,
                x => $"Number {x}")
        };

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();
        var stream = new MemoryStream();
        await ODataSerializer.WriteAsync(entity, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Items": [
                "Number 2",
                "Number 4"
              ]
            }
            """;

        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        var entityType = new EdmEntityType("NS", "Entity");
        var idProperty = entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
        var itemsProperty = entityType.AddStructuralProperty(
            "Items",
            new EdmCollectionTypeReference(
                new EdmCollectionType(
                    new EdmPrimitiveTypeReference(
                        EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false))));
        entityType.AddKeys(idProperty);
        model.AddElement(entityType);
        var container = new EdmEntityContainer("NS", "Container");
        model.AddElement(container);
        container.AddEntitySet("Entities", entityType);
        return model;
    }

    [ODataType("NS.Entity")]
    class EntityWithObjectProperty
    {
        public int Id { get; set; }
        public object Items { get; set; }
    }

    [ODataType("NS.Entity")]
    class EntityWithIEnumerableProperty
    {
        public int Id { get; set; }
        public IEnumerable Items { get; set; }
    }

    [ODataType("NS.Entity")]
    class EntityWithGenericEnumerableProperty
    {
        public int Id { get; set; }
        public IEnumerable<string> Items { get; set; }
    }

    [ODataType("NS.Entity")]
    class EntityWithFilterMapProperty
    {
        public int Id { get; set; }
        public FilterMap<int, string> Items { get; set; }
    }

    class FilterMap<TSource, Target>(
        IEnumerable<TSource> source,
        Func<TSource, bool> predicate,
        Func<TSource, Target> selector) : IEnumerable<Target>
    {
        public IEnumerator<Target> GetEnumerator()
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return selector(item);
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
