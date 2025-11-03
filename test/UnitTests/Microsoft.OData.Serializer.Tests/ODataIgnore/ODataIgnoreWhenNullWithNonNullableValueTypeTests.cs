using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Serializer;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Serializer.Tests.ODataIgnore;

public class ODataIgnoreWhenNullWithNonNullableValueTypeTests
{
    [Fact]
    public async Task ODataIgnoreWhenWritingNull_ShouldNotSkipValueTypeWithDefaultValue()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Age = default,
        };

        var options = new ODataSerializerOptions();

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            GetEdmModel(),
            new Uri("http://service/odata"),
            new Uri("Customers(1)", UriKind.Relative)
        ).ParseUri();


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
              "Age": 0
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task ODataIgnoreWhenWritingNull_ShouldNotSkipValueTypePropertyWithNonDefaultValue()
    {
        // Arrange
        var customer = new Customer
        {
            Id = 1,
            Age = 30
        };

        var options = new ODataSerializerOptions();

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            GetEdmModel(),
            new Uri("http://service/odata"),
            new Uri("Customers(1)", UriKind.Relative)
        ).ParseUri();


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
              "Age": 30
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }


    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        var entityType = model.AddEntityType("ns", "Customer");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
        entityType.AddStructuralProperty("Age", EdmPrimitiveTypeKind.Int32, isNullable: true);
        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("Customers", entityType);
        return model;
    }

    [ODataType("ns.Customer")]
    class Customer
    {
        public int Id { get; set; }

        [ODataIgnore(ODataIgnoreCondition.WhenWritingNull)]
        public int Age { get; set; }
    }
}
