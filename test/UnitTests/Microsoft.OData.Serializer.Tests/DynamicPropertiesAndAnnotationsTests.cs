using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Text.Json;

namespace Microsoft.OData.Serializer.Tests;

public class DynamicPropertiesAndAnnotationsTests
{
    [Fact]
    public async Task CanWriteDynamicProperties()
    {
        var item = new Item
        {
            Id = 1,
            Name = "Item 1",
            Data = new Dictionary<string, object>
            {
                { "DynamicString", "A dynamic string" },
                { "DynamicInt", 42 },
                { "DynamicBool", true }
            }
        };

        var options = new ODataSerializerOptions();

        options.AddTypeInfo<Item>(new()
        {
            Properties = [
                new()
                {
                    Name = "Id",
                    WriteValue = (item, writer, state) => writer.WriteValue(item.Id, state)
                },
                new()
                {
                    Name = "Name",
                    WriteValue = (item, writer, state) => writer.WriteValue(item.Name, state)
                }
            ],
            GetOpenProperties = (item, state) => item.Data
        });


        var model = CreateModel();

        var stream = new MemoryStream();

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Items(1)", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(item, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Items/$entity",
              "Id": 1,
              "Name": "Item 1",
              "DynamicString": "A dynamic string",
              "DynamicInt": 42,
              "DynamicBool": true
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(normalizedExpected, normalizedActual);

    }

    [Fact]
    public async Task CanWriteAnnotationsOfDynamicProperties()
    {
        var item = new Item
        {
            Id = 1,
            Name = "Item 1",
            Data = new Dictionary<string, object>
            {
                { "DynamicString", "A dynamic string" },
                { "DynamicInt", 42 },
                { "DynamicBool", true }
            },
            PreAnnotations = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "DynamicString",
                    new Dictionary<string, object>()
                    {
                        { "ns.ann1", "Annotation for dynamic string" },
                        { "ns.ann2", 10 },
                        { "ns.ann3", false }
                    }
                },
                {
                    "DynamicInt",
                    new()
                    {
                        { "ns.ann2", 12345 }
                    }
                },
            },
            PostAnnotations = new Dictionary<string, List<KeyValuePair<string, object>>>
            {
                {
                    "DynamicBool",
                    new List<KeyValuePair<string, object>>
                    {
                        new("ns.post1", "Post annotation for bool"),
                        new("ns.post2", 3.14),
                        new("ns.post3", true)
                    }
                },
                {
                    "DynamicInt",
                    new()
                    {
                        new("ns.post1", "Post annotation for int")
                    }
                }
            }
        };

        var options = new ODataSerializerOptions();

        options.AddTypeInfo<Item>(new()
        {
            Properties = [
                new()
                {
                    Name = "Id",
                    WriteValue = (item, writer, state) => writer.WriteValue(item.Id, state)
                },
                new()
                {
                    Name = "Name",
                    WriteValue = (item, writer, state) => writer.WriteValue(item.Name, state)
                }
            ],
            GetOpenProperties = (item, state) => item.Data,
            GetPropertyPreValueAnnotations = (item, propName, state) => item.PreAnnotations.GetValueOrDefault(propName),
            GetPropertyPostValueAnnotations = (item, propName, state) => item.PostAnnotations.GetValueOrDefault(propName)

        });


        var model = CreateModel();

        var stream = new MemoryStream();

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Items(1)", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(item, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Items/$entity",
              "Id": 1,
              "Name": "Item 1",
              "DynamicString@ns.ann1": "Annotation for dynamic string",
              "DynamicString@ns.ann2": 10,
              "DynamicString@ns.ann3": false,
              "DynamicString": "A dynamic string",
              "DynamicInt@ns.ann2": 12345,
              "DynamicInt": 42,
              "DynamicInt@ns.post1": "Post annotation for int",
              "DynamicBool": true,
              "DynamicBool@ns.post1": "Post annotation for bool",
              "DynamicBool@ns.post2": 3.14,
              "DynamicBool@ns.post3": true
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(normalizedExpected, normalizedActual);

    }

    private static IEdmModel CreateModel()
    {
        var model = new EdmModel();
        var itemType = model.AddEntityType("ns", "Item", baseType: null, isAbstract: false, isOpen: true);
        itemType.AddKeys(itemType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        itemType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("Items", itemType);

        return model;
    }

    class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public Dictionary<string, Dictionary<string, object>> PreAnnotations { get; set; }
        public Dictionary<string, List<KeyValuePair<string, object>>> PostAnnotations { get; set; }
    }
}
