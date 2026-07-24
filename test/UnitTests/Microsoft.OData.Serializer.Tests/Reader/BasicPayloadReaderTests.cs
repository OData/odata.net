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
    public async Task CanReadSimpleResourceOfPrimitiveProperties()
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

        options.AddTypeInfo<Product>(new()
        {
            CreateInstance = (state) => new Product(),
            Properties = [
                new()
                {
                    Name = "ID",
                    ReadValue = static (product, reader, state) =>
                    {
                        if (reader.ReadValue<int>(state, out int value))
                        {
                            product.ID = value;
                            return true;
                        }

                        return false;
                    }
                },
                new()
                {
                    Name = "Name",
                    ReadValue = static (product, reader, state) =>
                    {
                        if (reader.ReadValue<string>(state, out string value))
                        {
                            product.Name = value;
                            return true;
                        }

                        return false;
                    }
                },
                new()
                {
                    Name = "InStock",
                    ReadValue = static (product, reader, state) =>
                    {
                        if (reader.ReadValue(state, out bool value))
                        {
                            product.InStock = value;
                            return true;
                        }

                        return false;
                    }
                }
            ]
        });

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
        stream.Position = 0;

        var product = await ODataSerializer.ReadAsync<Product>(stream, model, options);

        Assert.NotNull(product);
        Assert.Equal(1, product.ID);
        Assert.Equal("Product 1", product.Name);
    }

    [Fact]
    public async Task CanReadSimpleResourceOfPrimitiveProperties_UsingGenericSetValueHook()
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

        options.AddTypeInfo<Product>(new()
        {
            CreateInstance = (state) => new Product(),
            Properties = [
                new ODataPropertyInfo<Product, int, DefaultState>
                {
                    Name = "ID",
                    SetValue = static (product, value, state) => product.ID = value
                },
                new ODataPropertyInfo<Product, string, DefaultState>
                {
                    Name = "Name",
                    SetValue = static (product, value, state) => product.Name = value
                },
                new ODataPropertyInfo<Product, bool, DefaultState>
                {
                    Name = "InStock",
                    SetValue = static (product, value, state) => product.InStock = value
                }
            ]
        });

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
        stream.Position = 0;

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
