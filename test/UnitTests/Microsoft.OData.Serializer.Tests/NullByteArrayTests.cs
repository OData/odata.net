using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests;

public class NullByteArrayTests
{
    [Fact]
    public async Task WritesNullIfByteArrayPropertyIsNull()
    {
        var data = new Entity
        {
            Id = 1,
            Data = null
        };

        var options = new ODataSerializerOptions();

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();
        
        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(data, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Data": null
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        var entityType = model.AddEntityType("ns", "Entity");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
        entityType.AddStructuralProperty(
            "Data", EdmPrimitiveTypeKind.Binary, isNullable: true);

        model.AddElement(entityType);
        var container = new EdmEntityContainer("ns", "Container");
        model.AddElement(container);
        container.AddEntitySet("Entities", entityType);
        return model;

    }

    [ODataType("ns.Entity")]
    class Entity
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
    }
}
