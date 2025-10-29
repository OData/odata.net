using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests;

public class NullStructuredPropertiesTests
{
    [Fact]
    public async Task WhenStructuredPropertyIsNull_WritesNullValue()
    {
        var data = new Customer
        {
            Id = 1,
            Address = null
        };

        var options = new ODataSerializerOptions();

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Customers(1)", UriKind.Relative)
        ).ParseUri();

        var stream = new MemoryStream();
        await ODataSerializer.WriteAsync(data, stream, odataUri, model, options);

        var actual = new StreamReader(stream).ReadToEnd();
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected = """
            {
              "@odata.context": "http://service/odata/$metadata#Customers/$entity",
              "Id": 1,
              "Address": null
            }
            """;
        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(expectedNormalized, actualNormalized);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();

        var address = model.AddComplexType("ns", "Address");
        address.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
        var customer = model.AddEntityType("ns", "Customer");
        customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        customer.AddStructuralProperty("Address", new EdmComplexTypeReference(address, isNullable: true));

        model.AddEntityContainer("ns", "Container").AddEntitySet("Customers", customer);

        return model;
    }

    [ODataType("ns.Customer")]
    class Customer
    {
        public int Id { get; set; }
        public Address Address { get; set; }
    }

    [ODataType("ns.Address")]
    class Address
    {
        public string Street { get; set; }
    }
}
