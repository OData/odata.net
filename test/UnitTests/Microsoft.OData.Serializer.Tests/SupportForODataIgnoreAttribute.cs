using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Attributes;
using Microsoft.OData.UriParser;
using System.Text.Json;

namespace Microsoft.OData.Serializer.Tests;

public class SupportForODataIgnoreAttribute
{
    [Fact]
    public async Task SkipPropertiesWithODataIgnoreAttribute()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Password = "SecretPassword"
        };

        var options = new ODataSerializerOptions();

        var odataUri = new ODataUriParser(
            CreateModel(),
            new Uri("http://service/odata"),
            new Uri("Customers(1)", UriKind.Relative)
        ).ParseUri();

        var model = CreateModel();

        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(customer, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers/$entity",
              "Id": 1,
              "Name": "John Doe"
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel CreateModel()
    {
        var model = new EdmModel();
        var entityType = model.AddEntityType("ns", "Customer");

        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
        entityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: true);

        // Adding this property to ensure ODataIgnore works, since by default properties not defined in the model  will be skipped
        entityType.AddStructuralProperty("Password", EdmPrimitiveTypeKind.String, isNullable: true);

        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("Customers", entityType);

        return model;
    }

    [ODataType("ns.Customer")]
    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ODataIgnore]
        public string Password { get; set; }
    }
}
