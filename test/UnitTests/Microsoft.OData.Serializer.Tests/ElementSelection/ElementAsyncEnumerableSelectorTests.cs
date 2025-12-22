using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.OData.Serializer.Tests.AutomaticPocoWriterTests;

namespace Microsoft.OData.Serializer.Tests.ElementSelection;

public class ElementAsyncEnumerableSelectorTests
{
    [Fact]
    public async Task CanSerializeCollectionItemsFromAsynchronousSource()
    {
        // Arrange
        var options = new ODataSerializerOptions();
        options.AddTypeInfo<AsyncCustomerSource>(new()
        {
            ElementSelector = new ODataElementAsyncEnumerableSelector<AsyncCustomerSource, Customer>()
            {
                GetElements = static (source, state) => source.GetCustomers(3),
                WriteElement = (source, customer, writer, state) => writer.WriteValue(customer, state)
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
        var customers = new AsyncCustomerSource();
        await ODataSerializer.WriteAsync(customers, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var result = new StreamReader(stream).ReadToEnd();
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(result));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers",
              "value": [
                {
                    "Id": 1,
                    "Name": "Customer 1"
                },
                {
                    "Id": 2,
                    "Name": "Customer 2"
                },
                {
                    "Id": 3,
                    "Name": "Customer 3"
                }
              ]
            }
            """;

        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(expectedNormalized, actualNormalized);
    }

    private static IEdmModel CreateModel()
    {
        var model = new EdmModel();
        var customer = model.AddEntityType("ns", "Customer");
        customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

        model.AddEntityContainer("ns", "Container").AddEntitySet("Customers", customer);

        return model;
    }

    class AsyncCustomerSource
    {
        public async IAsyncEnumerable<Customer> GetCustomers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                await Task.Yield();
                yield return new Customer { Id = i + 1, Name = $"Customer {i + 1}" };
            }
        }
    }

    [ODataType("ns.Customer")]
    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
