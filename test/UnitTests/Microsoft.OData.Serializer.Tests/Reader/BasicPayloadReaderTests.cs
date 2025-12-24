using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.Reader;

public class BasicPayloadReaderTests
{
    [Fact]
    public async Task ReadPayload()
    {
        string payload = """
        {
            "@odata.context": "http://service/odata/$metadata#Products",
            "ID": 1,
            "Name": "Product 1",
            "InStock": true
        }
        """;

        var model = GetEdmModel();
        var options = new ODataSerializerOptions();

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        var product = await ODataSerializer.ReadAsync<Product>(stream, model, options);

        Assert.NotNull(product);
        Assert.Equal(1, product.ID);
        Assert.Equal("Product 1", product.Name);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        var productType = model.AddEntityType("ns", "Product");
        productType.AddKeys(productType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32));
        productType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        productType.AddStructuralProperty("InStock", EdmPrimitiveTypeKind.Boolean);

        var container = model.AddEntityContainer("ns", "Container");
        var productsSet = container.AddEntitySet("ns.Product", productType);
        return model;
    }

    [ODataType("ns.Product")]
    class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool InStock { get; set; }
    }
}
