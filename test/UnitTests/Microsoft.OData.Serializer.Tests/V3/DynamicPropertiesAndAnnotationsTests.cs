using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.V3;

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
            GetDynamicProperties = (item, state) => item.Data
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
    }
}
