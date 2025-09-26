using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Text;
using System.Text.Json;

namespace Microsoft.OData.Serializer.Tests;

public class ResourceControlInformationSupport
{
    [Fact]
    public async Task CanSerializeODataId_GetODataId()
    {
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Alice" },
            new() { Id = 2, Name = "Bob" }
        };

        var options = new ODataSerializerOptions();
        options.AddTypeInfo<Customer>(new()
        {
            GetODataId = (customer, state) => $"Customers({customer.Id})",
            Properties = [
                new ODataPropertyInfo<Customer, int, DefaultState>()
                {
                    Name = "Id",
                    GetValue = (customer, state) => customer.Id
                },
                new ODataPropertyInfo<Customer, string, DefaultState>()
                {
                    Name = "Name",
                    GetValue = (customer, state) => customer.Name
                }
            ]
        });

        var output = new MemoryStream();
        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Customers", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(customers, output, odataUri, model, options);

        output.Position = 0;
        var actual = Encoding.UTF8.GetString(output.ToArray());
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers",
              "value": [
                {
                  "@odata.id": "Customers(1)",
                  "Id": 1,
                  "Name": "Alice"
                },
                {
                  "@odata.id": "Customers(2)",
                  "Id": 2,
                  "Name": "Bob"
                }
              ]
            }
            """;
        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(expectedNormalized, actualNormalized);
    }

    [Fact]
    public async Task CanSerializeODataId_WriteOdataId()
    {
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Alice" },
            new() { Id = 2, Name = "Bob" }
        };

        var options = new ODataSerializerOptions();
        options.AddTypeInfo<Customer>(new()
        {
            WriteODataId = (customer, writer, state) => writer.WriteId($"Customers({customer.Id})", state),
            Properties = [
                new ODataPropertyInfo<Customer, int, DefaultState>()
                {
                    Name = "Id",
                    GetValue = (customer, state) => customer.Id
                },
                new ODataPropertyInfo<Customer, string, DefaultState>()
                {
                    Name = "Name",
                    GetValue = (customer, state) => customer.Name
                }
            ]
        });

        var output = new MemoryStream();
        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Customers", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(customers, output, odataUri, model, options);

        output.Position = 0;
        var actual = Encoding.UTF8.GetString(output.ToArray());
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers",
              "value": [
                {
                  "@odata.id": "Customers(1)",
                  "Id": 1,
                  "Name": "Alice"
                },
                {
                  "@odata.id": "Customers(2)",
                  "Id": 2,
                  "Name": "Bob"
                }
              ]
            }
            """;
        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(expectedNormalized, actualNormalized);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        
        var customerType = model.AddEntityType("ns", "Customer");
        customerType.AddKeys(customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        customerType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("Customers", customerType);
        return model;
    }

    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
