using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Attributes;
using Microsoft.OData.UriParser;
using System.Text.Json;

namespace Microsoft.OData.Serializer.Tests.V3;

public class AutomaticPocoWriterTests
{
    [Fact]
    public async Task CanSerializePocosWithoutDefiningWriters()
    {
        var model = CreateModel();

        var orders = new List<Order>
        {
            new Order { Id = 1, TotalAmount = 150.00m },
            new Order { Id = 2, TotalAmount = 300.25m }
        };

        var customers = new List<Customer>
        {
            new Customer
            {
                Id = 1,
                Name = "Alice",
                BirthDate = new DateTime(1990, 1, 1),
                IsActive = true,
                Balance = 100.50m,
                Orders = orders
            },
            new Customer
            {
                Id = 2,
                Name = "Bob",
                BirthDate = new DateTime(1985, 5, 20),
                IsActive = false,
                Balance = 250.75m,
                Orders = new List<Order>()
            }
        };

        var options = new ODataSerializerOptions();
        var output = new MemoryStream();

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Customers", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(customers, output, odataUri, model, options);

        output.Position = 0;

        var actual = new StreamReader(output).ReadToEnd();
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers",
              "value": [
                {
                  "Id": 1,
                  "Name": "Alice",
                  "BirthDate": "1990-01-01T00:00:00Z",
                  "IsActive": true,
                  "Balance": 100.50,
                  "Orders": [
                    {
                      "Id": 1,
                      "TotalAmount": 150.00
                    },
                    {
                      "Id": 2,
                      "TotalAmount": 300.25
                    }
                  ]
                },
                {
                  "Id": 2,
                  "Name": "Bob",
                  "BirthDate": "1985-05-20T00:00:00Z",
                  "IsActive": false,
                  "Balance": 250.75,
                  "Orders": []
                }
              ]
            }
            """;


        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(expectedNormalized, actualNormalized);
    }

    [Fact]
    public async Task SkipsPropertiesNotFoundInODataModel()
    {
        var model = CreateModelWithoutBirthDate();

        var orders = new List<Order>
        {
            new Order { Id = 1, TotalAmount = 150.00m },
            new Order { Id = 2, TotalAmount = 300.25m }
        };

        var customers = new List<Customer>
        {
            new Customer
            {
                Id = 1,
                Name = "Alice",
                BirthDate = new DateTime(1990, 1, 1),
                IsActive = true,
                Balance = 100.50m,
                Orders = orders
            },
            new Customer
            {
                Id = 2,
                Name = "Bob",
                BirthDate = new DateTime(1985, 5, 20),
                IsActive = false,
                Balance = 250.75m,
                Orders = new List<Order>()
            }
        };

        var options = new ODataSerializerOptions();
        var output = new MemoryStream();

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Customers", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(customers, output, odataUri, model, options);

        output.Position = 0;

        var actual = new StreamReader(output).ReadToEnd();
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers",
              "value": [
                {
                  "Id": 1,
                  "Name": "Alice",
                  "IsActive": true,
                  "Balance": 100.50,
                  "Orders": [
                    {
                      "Id": 1,
                      "TotalAmount": 150.00
                    },
                    {
                      "Id": 2,
                      "TotalAmount": 300.25
                    }
                  ]
                },
                {
                  "Id": 2,
                  "Name": "Bob",
                  "IsActive": false,
                  "Balance": 250.75,
                  "Orders": []
                }
              ]
            }
            """;


        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(expectedNormalized, actualNormalized);
    }

    [ODataType("ns.Customer")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsActive { get; set; }
        public decimal Balance { get; set; }
        public List<Order> Orders { get; set; }
    }

    [ODataType("ns.Order")]
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
    }

    private static IEdmModel CreateModel()
    {
        var model = new EdmModel();

        var customer = model.AddEntityType("ns", "Customer");
        customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        customer.AddStructuralProperty("BirthDate", EdmPrimitiveTypeKind.DateTimeOffset);
        customer.AddStructuralProperty("IsActive", EdmPrimitiveTypeKind.Boolean);
        customer.AddStructuralProperty("Balance", EdmPrimitiveTypeKind.Decimal);

        var order = model.AddEntityType("ns", "Order");
        order.AddKeys(order.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        order.AddStructuralProperty("TotalAmount", EdmPrimitiveTypeKind.Decimal);

        customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "Orders",
            Target = order,
            TargetMultiplicity = EdmMultiplicity.Many
        });

        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("Customers", customer);
        container.AddEntitySet("Orders", order);
        return model;
    }

    private static IEdmModel CreateModelWithoutBirthDate()
    {
        var model = new EdmModel();

        var customer = model.AddEntityType("ns", "Customer");
        customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        customer.AddStructuralProperty("IsActive", EdmPrimitiveTypeKind.Boolean);
        customer.AddStructuralProperty("Balance", EdmPrimitiveTypeKind.Decimal);

        var order = model.AddEntityType("ns", "Order");
        order.AddKeys(order.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        order.AddStructuralProperty("TotalAmount", EdmPrimitiveTypeKind.Decimal);

        customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "Orders",
            Target = order,
            TargetMultiplicity = EdmMultiplicity.Many
        });

        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("Customers", customer);
        container.AddEntitySet("Orders", order);
        return model;
    }
}
