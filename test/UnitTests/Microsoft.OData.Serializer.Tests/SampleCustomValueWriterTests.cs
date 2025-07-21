using Microsoft.OData.Serializer;
using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
namespace Microsoft.OData.Serializer.Tests;

public class SampleCustomValueWriterTests
{
    [Fact]
    public async Task NestedPocoResourceSetResponse_WithSelectExpand_And_MinimalControlInformation()
    {
        var model = CreateNestedEcommerceModel();

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Customers?$select=Id,Name,Emails,OtherAddresses&$expand=Orders($expand=Products($select=Id,Name,Price;$count=true);$count=true),WishList($select=Id,Name,Category)&count=true", UriKind.Relative)
        ).ParseUri();

        IList<Product> products = [
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Category = Category.Electronics },
            new Product { Id = 2, Name = "T-Shirt", Price = 19.99m, Category = Category.Clothing },
            // create 10 products
            new Product { Id = 3, Name = "Washing Machine", Price = 499.99m, Category = Category.HomeAppliances },
            // more products
            new Product { Id = 4, Name = "Smartphone", Price = 699.99m, Category = Category.Electronics },
            new Product { Id = 5, Name = "Jeans", Price = 39.99m, Category = Category.Clothing },
            new Product { Id = 6, Name = "Refrigerator", Price = 899.99m, Category = Category.HomeAppliances },
            new Product { Id = 7, Name = "Tablet", Price = 299.99m, Category = Category.Electronics },
            new Product { Id = 8, Name = "Sneakers", Price = 59.99m, Category = Category.Clothing },
            new Product { Id = 9, Name = "Microwave Oven", Price = 199.99m, Category = Category.HomeAppliances },
            new Product { Id = 10, Name = "Smartwatch", Price = 249.99m, Category = Category.Electronics }
        ];

        IList<Order> orders = [
            new Order
            {
                Id = 1,
                OrderDate = new DateTime(2025, 05, 31),
                Status = OrderStatus.Purchased,
                Products = products.Take(2).ToList()
            },
            new Order
            {
                Id = 2,
                OrderDate = new DateTime(2025, 06, 05),
                Status = OrderStatus.Shipped,
                Products = products.Skip(2).Take(3).ToList()
            },
            new Order
            {
                Id = 3,
                OrderDate = new DateTime(2025, 06, 08),
                Status = OrderStatus.Delivered,
                Products = [products[0], products[3], products[4]]
            }
        ];

        IList<Customer> customers = [
            new Customer
            {
                Id = 1,
                Name = "Alice",
                Emails = ["alice@example.com", "alice@mailer.com"],
                HomeAddress = new Address { City = "Wonderland", Country = "Fantasy" },
                OtherAddresses = [
                    new Address { City = "Wonderland", Country = "Fantasy" },
                    new Address { City = "Dreamland", Country = "Fantasy" }
                ],
                Orders = [orders[0], orders[1]],
                BestOrder = orders[1],
                WishList = [products[0], products[1]]
            },
            new Customer
            {
                Id = 2,
                Name = "Bob",
                Emails = ["bob@example.com", "bob@mailer.com"],
                HomeAddress = new Address { City = "Builderland", Country = "Fantasy" },
                OtherAddresses = [
                    new Address { City = "Builderland", Country = "Fantasy" },
                    new Address { City = "Dreamland", Country = "Fantasy" }
                ],
                Orders = [orders[2]],
                BestOrder = orders[2],
                WishList = [products[3], products[5]]
            }
        ];

        using var output = new MemoryStream();

        var options = new ODataSerializerOptions();

        options.AddValueWriter(new CustomerCollectionWriter());
        options.AddValueWriter(new CustomerWriter());
        options.AddValueWriter(new OrderWriter());
        options.AddValueWriter(new ProductWriter());
        options.AddValueWriter(new AddressWriter());

        await ODataSerializer.WriteAsync(
            customers,
            output,
            odataUri,
            model,
            options);

        output.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(output, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        // Actual OData context to use
        // http://service/odata/$meadata#Customers(Id,Name,Emails,OtherAddresses,Orders,WishList,Orders(Products(Id,Name,Price)).WishList(Id,Name,Category)
        var expectedJson = NormalizeJson(
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers",
              "@odata.nextLink": "http://service/odata/Customers?$skip=2",
              "value": [
                {
                  "@odata.etag": "W/\"1\"",
                  "Id": 1,
                  "Name": "Alice",
                  "Emails": [
                    "alice@example.com",
                    "alice@mailer.com"
                  ],
                  "OtherAddresses": [
                    { "City": "Wonderland", "Country": "Fantasy" },
                    { "City": "Dreamland", "Country": "Fantasy" }
                  ],
                  "Orders@odata.count": 2,
                  "Orders@odata.nextLink": "http://service/odata/Customers(1)/Orders?$skip=2",
                  "Orders": [
                    {
                      "@odata.etag": "W/\"order-1\"",
                      "Id": 1,
                      "OrderDate": "2025-05-31T00:00:00Z",
                      "Status": "Purchased",
                      "Products@odata.count": 2,
                      "Products@odata.nextLink": "http://service/odata/Customers(1)/Orders(1)/Products?$skip=2",
                      "Products": [
                        {
                          "@odata.etag": "W/\"product-1\"",
                          "Id": 1,
                          "Name": "Laptop",
                          "Price": 999.99
                        },
                        {
                          "@odata.etag": "W/\"product-2\"",
                          "Id": 2,
                          "Name": "T-Shirt",
                          "Price": 19.99
                        }
                      ]
                    },
                    {
                      "@odata.etag": "W/\"order-2\"",
                      "Id": 2,
                      "OrderDate": "2025-06-05T00:00:00Z",
                      "Status": "Shipped",
                      "Products@odata.count": 3,
                      "Products@odata.nextLink": "http://service/odata/Customers(1)/Orders(2)/Products?$skip=3",
                      "Products": [
                        {
                          "@odata.etag": "W/\"product-3\"",
                          "Id": 3,
                          "Name": "Washing Machine",
                          "Price": 499.99
                        },
                        {
                          "@odata.etag": "W/\"product-4\"",
                          "Id": 4,
                          "Name": "Smartphone",
                          "Price": 699.99
                        },
                        {
                          "@odata.etag": "W/\"product-5\"",
                          "Id": 5,
                          "Name": "Jeans",
                          "Price": 39.99
                        }
                      ]
                    }
                  ],
                  "WishList@odata.count": 2,
                  "WishList": [
                    {
                      "@odata.etag": "W/\"product-1\"",
                      "Id": 1,
                      "Name": "Laptop",
                      "Category": "Electronics"
                    },
                    {
                      "@odata.etag": "W/\"product-2\"",
                      "Id": 2,
                      "Name": "T-Shirt",
                      "Category": "Clothing"
                    }
                  ]
                },
                {
                  "@odata.etag": "W/\"2\"",
                  "Id": 2,
                  "Name": "Bob",
                  "Emails": [
                    "bob@example.com",
                    "bob@mailer.com"
                  ],
                  "OtherAddresses": [
                    { "City": "Builderland", "Country": "Fantasy" },
                    { "City": "Dreamland", "Country": "Fantasy" }
                  ],
                  "Orders@odata.count": 1,
                  "Orders": [
                    {
                      "@odata.etag": "W/\"order-3\"",
                      "Id": 3,
                      "OrderDate": "2025-06-08T00:00:00Z",
                      "Status": "Delivered",
                      "Products@odata.count": 3,
                      "Products@odata.nextLink": "http://service/odata/Customers(2)/Orders(3)/Products?$skip=3",
                      "Products": [
                        {
                          "@odata.etag": "W/\"product-1\"",
                          "Id": 1,
                          "Name": "Laptop",
                          "Price": 999.99
                        },
                        {
                          "@odata.etag": "W/\"product-4\"",
                          "Id": 4,
                          "Name": "Smartphone",
                          "Price": 699.99
                        },
                        {
                          "@odata.etag": "W/\"product-5\"",
                          "Id": 5,
                          "Name": "Jeans",
                          "Price": 39.99
                        }
                      ]
                    }
                  ],
                  "WishList@odata.count": 2,
                  "WishList": [
                    {
                      "@odata.etag": "W/\"product-4\"",
                      "Id": 4,
                      "Name": "Smartphone",
                      "Category": "Electronics"
                    },
                    {
                      "@odata.etag": "W/\"product-6\"",
                      "Id": 6,
                      "Name": "Refrigerator",
                      "Category": "HomeAppliances"
                    }
                  ]
                }
              ]
            }
            """);

        Assert.Equal(expectedJson, json);
    }

    private static string NormalizeJson([StringSyntax(StringSyntaxAttribute.Json)] string rawJson)
    {
        JsonDocument doc = JsonDocument.Parse(rawJson);
        var normalized = JsonSerializer.Serialize(doc);
        return normalized;
    }

    private static IEdmModel CreateNestedEcommerceModel()
    {
        var model = new EdmModel();

        var orderStatus = model.EnumType("OrderStatus", "ns");
        orderStatus.AddMember("Pending", new EdmEnumMemberValue(0));
        orderStatus.AddMember("Purchased", new EdmEnumMemberValue(1));
        orderStatus.AddMember("Shipped", new EdmEnumMemberValue(2));
        orderStatus.AddMember("Delivered", new EdmEnumMemberValue(3));

        var category = model.EnumType("Category", "ns");
        category.AddMember("Electronics", new EdmEnumMemberValue(0));
        category.AddMember("Clothing", new EdmEnumMemberValue(1));
        category.AddMember("HomeAppliances", new EdmEnumMemberValue(2));

        // Address complex type
        var addressComplex = model.AddComplexType("ns", "Address");
        addressComplex.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
        addressComplex.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);


        // Customer entity type
        var customerEntity = model.AddEntityType("ns", "Customer");
        customerEntity.AddKeys(customerEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        customerEntity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        customerEntity.AddStructuralProperty("Emails",
            new EdmCollectionTypeReference(
                new EdmCollectionType(
                    EdmCoreModel.Instance.GetString(false))));
        customerEntity.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(addressComplex, false));
        customerEntity.AddStructuralProperty("OtherAddresses",
            new EdmCollectionTypeReference(
                new EdmCollectionType(new EdmComplexTypeReference(addressComplex, true))));


        // Order entity type
        var orderEntity = model.AddEntityType("ns", "Order");
        orderEntity.AddKeys(orderEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        orderEntity.AddStructuralProperty("OrderDate", EdmPrimitiveTypeKind.DateTimeOffset);
        orderEntity.AddStructuralProperty("Status", new EdmEnumTypeReference(new EdmEnumType("ns", "OrderStatus"), false));

        // Product entity type
        var productEntity = model.AddEntityType("ns", "Product");
        productEntity.AddKeys(productEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        productEntity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        productEntity.AddStructuralProperty("Price", EdmPrimitiveTypeKind.Decimal);
        productEntity.AddStructuralProperty("Category", new EdmEnumTypeReference(new EdmEnumType("ns", "Category"), false));

        //Relationships
        customerEntity.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "Orders",
            Target = orderEntity,
            TargetMultiplicity = EdmMultiplicity.Many
        });

        customerEntity.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "BestOrder",
            Target = orderEntity,
            TargetMultiplicity = EdmMultiplicity.ZeroOrOne
        });

        customerEntity.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "WishList",
            Target = productEntity,
            TargetMultiplicity = EdmMultiplicity.Many
        });

        orderEntity.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "Products",
            Target = productEntity,
            TargetMultiplicity = EdmMultiplicity.Many
        });

        var container = model.AddEntityContainer("ns", "Container");
        container.AddEntitySet("Customers", customerEntity);
        container.AddEntitySet("Orders", orderEntity);
        container.AddEntitySet("Products", productEntity);

        return model;
    }

    class CustomerCollectionWriter : EnumerableResourceSetJsonWriter<IList<Customer>, Customer>
    {
        protected override bool HasNextLinkValue(
            IList<Customer> value,
            ODataJsonWriterStack state,
            ODataJsonWriterContext context,
            out Uri nextLink)
        {
            nextLink = new Uri("http://service/odata/Customers?$skip=2", UriKind.Absolute);
            return true;
        }
    }

    class CustomerWriter : ODataResourceBaseJsonWriter<Customer>
    {
        protected override ValueTask WritePropertyValue(
            Customer resource,
            IEdmProperty property,
            ODataJsonWriterStack state,
            ODataJsonWriterContext context)
        {
            if (property.Name == "Id")
            {
                return context.WriteValueAsync(resource.Id, state);
            }
            else if (property.Name == "Name")
            {
                return context.WriteValueAsync(resource.Name, state);
            }
            else if (property.Name == "Emails")
            {
                return context.WriteValueAsync(resource.Emails, state);
            }
            else if (property.Name == "OtherAddresses")
            {
                return context.WriteValueAsync(resource.OtherAddresses, state);
            }
            else if (property.Name == "Orders")
            {
                return context.WriteValueAsync(resource.Orders, state);
            }
            else if (property.Name == "WishList")
            {
                return context.WriteValueAsync(resource.WishList, state);
            }

            return ValueTask.CompletedTask;
        }

        protected override bool HasEtagValue(Customer value, ODataJsonWriterStack state, ODataJsonWriterContext context, out string etagValue)
        {
            etagValue = $"W/\"{value.Id}\"";
            return true;
        }

        protected override bool HasNestedCountValue(
            Customer value,
            IEdmProperty resourceProperty,
            ODataJsonWriterStack state,
            ODataJsonWriterContext context,
            out long? count)
        {
            if (resourceProperty.Name == "Orders")
            {
                count = value.Orders.Count;
                return true;
            }

            count = null;
            return false;
        }
    }

    class OrderWriter : ODataResourceBaseJsonWriter<Order>
    {
        protected override ValueTask WritePropertyValue(
            Order resource,
            IEdmProperty property,
            ODataJsonWriterStack state,
            ODataJsonWriterContext context)
        {
            if (property.Name == "Id")
            {
                return context.WriteValueAsync(resource.Id, state);
            }
            else if (property.Name == "OrderDate")
            {
                return context.WriteValueAsync(resource.OrderDate, state);
            }
            else if (property.Name == "Status")
            {
                return context.WriteValueAsync(resource.Status, state);
            }
            else if (property.Name == "Products")
            {
                return context.WriteValueAsync(resource.Products, state);
            }

            return ValueTask.CompletedTask;
        }
    }

    class ProductWriter : ODataResourceBaseJsonWriter<Product>
    {
        protected override ValueTask WritePropertyValue(
            Product resource,
            IEdmProperty property,
            ODataJsonWriterStack state,
            ODataJsonWriterContext context)
        {
            if (property.Name == "Id")
            {
                return context.WriteValueAsync(resource.Id, state);
            }
            else if (property.Name == "Name")
            {
                return context.WriteValueAsync(resource.Name, state);
            }
            else if (property.Name == "Price")
            {
                return context.WriteValueAsync(resource.Price, state);
            }
            else if (property.Name == "Category")
            {
                return context.WriteValueAsync(resource.Category, state);
            }

            return ValueTask.CompletedTask;
        }
    }

    class AddressWriter : ODataResourceBaseJsonWriter<Address>
    {
        protected override ValueTask WritePropertyValue(
            Address resource,
            IEdmProperty property,
            ODataJsonWriterStack state,
            ODataJsonWriterContext context)
        {
            if (property.Name == "City")
            {
                return context.WriteValueAsync(resource.City, state);
            }
            else if (property.Name == "Country")
            {
                return context.WriteValueAsync(resource.Country, state);
            }

            return ValueTask.CompletedTask;
        }
    }

    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<string> Emails { get; set; }

        public Address HomeAddress { get; set; }

        public IList<Address> OtherAddresses { get; set; }

        public Order BestOrder { get; set; }

        public IList<Order> Orders { get; set; }

        public IList<Product> WishList { get; set; }

    }

    class Address
    {
        public string City { get; set; }
        public string Country { get; set; }
    }

    class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }
        public IList<Product> Products { get; set; }

    }

    enum OrderStatus
    {
        Pending,
        Purchased,
        Shipped,
        Delivered,
        Cancelled
    }

    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Category Category { get; set; }
    }

    enum Category
    {
        Electronics,
        Clothing,
        HomeAppliances
    }
}
