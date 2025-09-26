using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json.State;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.V3;

public class SupportForDictionaryBasedPropertySelection
{
    [Fact]
    public async Task SerializesDataFromDictionaryUsingPropertyEnumerableSelector()
    {
        // Arrange
        var customer = new Customer
        {
            Data = new Dictionary<string, object>
            {
                { "Id", 1 },
                { "Name", "Alice" },
                { "BirthDate", new DateTimeOffset(new DateTime(1990, 1, 1)) }
            }
        };

        var options = new ODataSerializerOptions();
        options.AddTypeInfo<Customer>(new()
        {
            PropertySelector = new ODataPropertyEnumerableSelector<Customer, KeyValuePair<string, object>>()
            {
                GetProperties = (cust, state) => cust.Data,
                WriteProperty = (cust, prop, writer, state) => writer.WriteProperty(prop.Key, prop.Value, state)
            }
        });

        var model = CreateModel();

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Customers", UriKind.Relative)
        ).ParseUri();

        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(customer, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var result = new StreamReader(stream).ReadToEnd();
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(result));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers",
              "Id": 1,
              "Name": "Alice",
              "BirthDate": "1990-01-01T00:00:00Z"
            }
            """;

        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(expectedNormalized, actualNormalized);
    }

    [Fact]
    public async Task SerializesDataFromDictionaryUsingPropertyEnumerableSelectorWithValueGetter()
    {
        // Arrange
        var customer = new Customer
        {
            Data = new Dictionary<string, object>
            {
                { "Id", 1 },
                { "Name", "Alice" },
                { "BirthDate", new DateTimeOffset(new DateTime(1990, 1, 1)) }
            }
        };

        var options = new ODataSerializerOptions();
        options.AddTypeInfo<Customer>(new()
        {
            PropertySelector = new ODataPropertyEnumerableSelector<Customer, KeyValuePair<string, object>, object, DefaultState>()
            {
                GetProperties = (cust, state) => cust.Data,
                GetPropertyName = (cust, prop, state) => prop.Key,
                GetPropertyValue = (cust, prop, state) => prop.Value,
            }
        });

        var model = CreateModel();

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Customers", UriKind.Relative)
        ).ParseUri();

        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(customer, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var result = new StreamReader(stream).ReadToEnd();
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(result));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers",
              "Id": 1,
              "Name": "Alice",
              "BirthDate": "1990-01-01T00:00:00Z"
            }
            """;

        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(expectedNormalized, actualNormalized);
    }

    private static IEdmModel CreateModel()
    {
        EdmModel model = new EdmModel();
        var type = model.AddEntityType("ns", "Customer", null, false, true);
        type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
        type.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, true);
        type.AddStructuralProperty("IsActive", EdmPrimitiveTypeKind.Boolean, false);
        type.AddStructuralProperty("Balance", EdmPrimitiveTypeKind.Decimal, false);
        type.AddStructuralProperty("Age", EdmPrimitiveTypeKind.Int32, false);
        type.AddStructuralProperty("BirthDate", EdmPrimitiveTypeKind.DateTimeOffset, false);

        var container = model.AddEntityContainer("ns", "Container");
        container.AddEntitySet("Customers", type);

        return model;
    }
    class Customer
    {
        public required Dictionary<string, object> Data { get; set; }
    }
}
