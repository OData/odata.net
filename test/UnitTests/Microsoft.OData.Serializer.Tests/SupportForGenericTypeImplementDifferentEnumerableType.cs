using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests;

public class SupportForGenericTypeImplementDifferentEnumerableType
{
    [Fact]
    public async Task CanSerializeGenericTypeWithTypeParamterDifferentFromIEnumerableElementType()
    {
        List<int> source = [1, 2, 3, 4, 5];
        var entity = new Entity
        {
            Id = 1,
            Items = new StringCollection<int>([1, 2, 3, 4])
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
                "1",
                "2",
                "3",
                "4"
              ]
            }
            """;

        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        var entityType = new EdmEntityType("ns", "Entity");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        entityType.AddStructuralProperty("Items", new EdmCollectionTypeReference(new EdmCollectionType(new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false))));
        model.AddElement(entityType);
        var container = new EdmEntityContainer("ns", "Container");
        model.AddElement(container);
        var entitySet = container.AddEntitySet("Entities", entityType);
        return model;
    }

    [ODataType("ns.Entity")]
    class Entity
    {
        public int Id { get; set; }

        // Ensure the generic parameter of the collection "int" is different from the element type "string"
        public StringCollection<int> Items { get; set; }
    }

    class StringCollection<T>(T[] data) : IEnumerable<string>
    {
        public IEnumerator<string> GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item!.ToString()!;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
