using Microsoft.OData.Core.NewWriter2;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.Test.OData.Utils.ODataLibTest;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Core.Tests.NewWriter2Tests;

public class NewWriter2ExperimentTests
{
    [Fact]
    public async Task SimplePocoResourceSetResponse()
    {
        var model = new EdmModel();

        var entity = model.AddEntityType("ns", "Project");
        entity.AddKeys(entity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        entity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        entity.AddStructuralProperty("IsActive", EdmPrimitiveTypeKind.Boolean);

        model.AddEntityContainer("ns", "Container")
            .AddEntitySet("Projects", entity);

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Projects", UriKind.Relative)
        ).ParseUri();

        IEnumerable<Project> projects = [
            new() { Id = 1, Name = "P1", IsActive = true },
            new() { Id = 2, Name = "P2", IsActive = false },
        ];

        using var output = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(output);

        var metataProvider = new JsonMetadataValueProvider();
        var propertyValueWriterProvider = new EdmPropertyValueJsonWriterProvider();
        propertyValueWriterProvider.Add<Project>((resource, property, state, context) =>
        {
            if (property.Name == "Id")
            {
                context.JsonWriter.WriteNumberValue(resource.Id);
            }
            else if (property.Name == "Name")
            {
                context.JsonWriter.WriteStringValue(resource.Name);
            }
            else if (property.Name == "IsActive")
            {
                context.JsonWriter.WriteBooleanValue(resource.IsActive);
            }

            return ValueTask.CompletedTask;
        });


        var writerContext = new ODataJsonWriterContext
        {
            Model = model,
            ODataUri = odataUri,
            MetadataLevel = ODataMetadataLevel.Minimal,
            PayloadKind = ODataPayloadKind.ResourceSet,
            ODataVersion = ODataVersion.V4,
            JsonWriter = jsonWriter,
            ValueWriterProvider = new ResourceJsonWriterProvider(),
            MetadataWriterProvider = new JsonMetadataWriterProvider(metataProvider),
            PropertyValueWriterProvider = propertyValueWriterProvider,
            ResourcePropertyWriterProvider = new EdmPropertyJsonWriterProvider()
        };

        var writerStack = new ODataJsonWriterStack();
        writerStack.Push(new ODataJsonWriterStackFrame
        {
            EdmType = new EdmCollectionType(
                new EdmEntityTypeReference(
                    model.FindType("ns.Project") as IEdmEntityType, isNullable: false)),
            SelectExpandClause = odataUri.SelectAndExpand
        });

        var odataWriter = new ODataResourceSetEnumerableJsonWriter<Project>();
        await odataWriter.WriteAsync(projects, writerStack, writerContext);

        // TODO: should we guarantee flushing from within the writer?
        await jsonWriter.FlushAsync();

        output.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(output, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        var expectedJson = @"{""@odata.context"":""http://service/odata/$metadata#Projects"",""value"":[{""Id"":1,""Name"":""P1"",""IsActive"":true},{""Id"":2,""Name"":""P2"",""IsActive"":false}]}";
        Assert.Equal(expectedJson, json);
    }

    [Fact]
    public async Task SimplePocoResourseSetResponse_WithCountAndNextLink()
    {
        var model = new EdmModel();

        var entity = model.AddEntityType("ns", "Project");
        entity.AddKeys(entity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        entity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        entity.AddStructuralProperty("IsActive", EdmPrimitiveTypeKind.Boolean);

        model.AddEntityContainer("ns", "Container")
            .AddEntitySet("Projects", entity);

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Projects?$count=true", UriKind.Relative)
        ).ParseUri();

        IEnumerable<Project> projects = [
            new() { Id = 1, Name = "P1", IsActive = true },
            new() { Id = 2, Name = "P2", IsActive = false },
        ];

        using var output = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(output);

        // What a bout a generic counter for IEnumerable<T>
        var metadataProvider = new JsonMetadataValueProvider();
        metadataProvider.MapCounter<IEnumerable<Project>>((projects, state, context) => projects.Count());
        metadataProvider.MapNextLinkHandler<IEnumerable<Project>>((projects, state, context) =>
        {
            return new Uri("http://service/odata/Products?$skiptoken=skip", UriKind.Absolute);
        });

        metadataProvider.MapCounter<IEnumerable<Order>>((orders, state, context) => orders.Count());


        var propertyValueWriterProvider = new EdmPropertyValueJsonWriterProvider();
        propertyValueWriterProvider.Add<Project>((resource, property, state, context) =>
        {
            if (property.Name == "Id")
            {
                context.JsonWriter.WriteNumberValue(resource.Id);
            }
            else if (property.Name == "Name")
            {
                context.JsonWriter.WriteStringValue(resource.Name);
            }
            else if (property.Name == "IsActive")
            {
                context.JsonWriter.WriteBooleanValue(resource.IsActive);
            }
            return ValueTask.CompletedTask;
        });


        var writerContext = new ODataJsonWriterContext
        {
            Model = model,
            ODataUri = odataUri,
            ODataVersion = ODataVersion.V4,
            MetadataLevel = ODataMetadataLevel.Minimal,
            PayloadKind = ODataPayloadKind.ResourceSet,
            JsonWriter = jsonWriter,
            ValueWriterProvider = new ResourceJsonWriterProvider(),
            MetadataWriterProvider = new JsonMetadataWriterProvider(metadataProvider),
            PropertyValueWriterProvider = propertyValueWriterProvider,
            ResourcePropertyWriterProvider = new EdmPropertyJsonWriterProvider()
        };

        var writerStack = new ODataJsonWriterStack();
        writerStack.Push(new ODataJsonWriterStackFrame
        {
            EdmType = new EdmCollectionType(
                new EdmEntityTypeReference(
                    model.FindType("ns.Project") as IEdmEntityType, isNullable: false)),
            SelectExpandClause = odataUri.SelectAndExpand
        });

        var odataWriter = new ODataResourceSetEnumerableJsonWriter<Project>();
        await odataWriter.WriteAsync(projects, writerStack, writerContext);

        // TODO: should we guarantee flushing from within the writer?
        await jsonWriter.FlushAsync();

        output.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(output, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        var expectedJson = @"{""@odata.context"":""http://service/odata/$metadata#Projects"",""@odata.count"":2,""@odata.nextLink"":""http://service/odata/Products?$skiptoken=skip"",""value"":[{""Id"":1,""Name"":""P1"",""IsActive"":true},{""Id"":2,""Name"":""P2"",""IsActive"":false}]}";
        Assert.Equal(expectedJson, json);
    }

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
                OrderDate = DateTime.UtcNow.AddDays(-10),
                Status = OrderStatus.Purchased,
                Products = products.Take(2).ToList()
            },
            new Order
            {
                Id = 2,
                OrderDate = DateTime.UtcNow.AddDays(-5),
                Status = OrderStatus.Shipped,
                Products = products.Skip(2).Take(3).ToList()
            },
            new Order
            {
                Id = 3,
                                OrderDate = DateTime.UtcNow.AddDays(-2),
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
        var jsonWriter = new Utf8JsonWriter(output);

        // What a bout a generic counter for IEnumerable<T>
        var metadataProvider = new JsonMetadataValueProvider();
        // TODO: would like to map IList<Customer> but would not work since the writer supports IEnumerable<T>
        metadataProvider.MapCounter<IList<Customer>>((customers, state, context) => customers.Count);
        metadataProvider.MapNextLinkHandler<IList<Customer>>((customers, state, context) =>
        {
            return new Uri("http://service/odata/Customers?$skip=2", UriKind.Absolute);
        });
        metadataProvider.MapEtagHandler<Customer>((customer, state, context) =>
        {
            return $"W/\"{customer.Id}\"";
        });

        var propertyValueWriterProvider = new EdmPropertyValueJsonWriterProvider();
        propertyValueWriterProvider.Add<Customer>(async (resource, property, state, context) =>
        {
            if (property.Name == "Id")
            {
                context.JsonWriter.WriteNumberValue(resource.Id);
            }
            else if (property.Name == "Name")
            {
                context.JsonWriter.WriteStringValue(resource.Name);
            }
            else if (property.Name == "Emails")
            {
                context.JsonWriter.WriteStartArray();
                foreach (var email in resource.Emails)
                {
                    context.JsonWriter.WriteStringValue(email);
                }
                context.JsonWriter.WriteEndArray();
            }
            else if (property.Name == "OtherAddresses")
            {
                await context.WriteValueAsync(resource.OtherAddresses, state);
            }
            else if (property.Name == "Orders")
            {
                await context.WriteValueAsync(resource.Orders, state);
            }
            else if (property.Name == "WishList")
            {
                await context.WriteValueAsync(resource.WishList, state);
            }
        });

        propertyValueWriterProvider.Add<Order>(async (resource, property, state, context) =>
        {
            if (property.Name == "Id")
            {
                context.JsonWriter.WriteNumberValue(resource.Id);
            }
            else if (property.Name == "OrderDate")
            {
                context.JsonWriter.WriteStringValue(resource.OrderDate.ToString("o")); // ISO 8601 format
            }
            else if (property.Name == "Status")
            {
                context.JsonWriter.WriteStringValue(resource.Status.ToString());
            }
            else if (property.Name == "Products")
            {
                await context.WriteValueAsync(resource.Products, state);
            }
        });

        propertyValueWriterProvider.Add<Product>((resource, property, state, context) =>
        {
            if (property.Name == "Id")
            {
                context.JsonWriter.WriteNumberValue(resource.Id);
            }
            else if (property.Name == "Name")
            {
                context.JsonWriter.WriteStringValue(resource.Name);
            }
            else if (property.Name == "Price")
            {
                context.JsonWriter.WriteNumberValue(resource.Price);
            }
            else if (property.Name == "Category")
            {
                context.JsonWriter.WriteStringValue(resource.Category.ToString());
            }
        });

        propertyValueWriterProvider.Add<Address>((resource, property, state, context) =>
        {
            if (property.Name == "City")
            {
                context.JsonWriter.WriteStringValue(resource.City);
            }
            else if (property.Name == "Country")
            {
                context.JsonWriter.WriteStringValue(resource.Country);
            }
        });

        var writerContext = new ODataJsonWriterContext
        {
            Model = model,
            ODataUri = odataUri,
            ODataVersion = ODataVersion.V4,
            MetadataLevel = ODataMetadataLevel.Minimal,
            PayloadKind = ODataPayloadKind.ResourceSet,
            JsonWriter = jsonWriter,
            ValueWriterProvider = new ResourceJsonWriterProvider(),
            MetadataWriterProvider = new JsonMetadataWriterProvider(metadataProvider),
            PropertyValueWriterProvider = propertyValueWriterProvider,
            ResourcePropertyWriterProvider = new EdmPropertyJsonWriterProvider()
        };

        var writerStack = new ODataJsonWriterStack();
        writerStack.Push(new ODataJsonWriterStackFrame
        {
            EdmType = new EdmCollectionType(
                new EdmEntityTypeReference(
                    model.FindType("ns.Customer") as IEdmEntityType, isNullable: false)),
            SelectExpandClause = odataUri.SelectAndExpand
        });

        var odataWriter = new EnumerableResourceSetJsonWriter<IList<Customer>, Customer>();
        await odataWriter.WriteAsync(customers, writerStack, writerContext);

        // TODO: should we guarantee flushing from within the writer?
        await jsonWriter.FlushAsync();

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
                      "@odata.etag": "W/\"1-1\"",
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
                      "@odata.etag": "W/\"1-2\"",
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
                      "@odata.etag": "W/\"wishlist-1-1\"",
                      "Id": 1,
                      "Name": "Laptop",
                      "category": "Electronics"
                    },
                    {
                      "@odata.etag": "W/\"wishlist-1-2\"",
                      "Id": 2,
                      "Name": "T-Shirt",
                      "category": "Clothing"
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
                      "@odata.etag": "W/\"2-1\"",
                      "Id": 3,
                      "OrderDate": "2025-06-08T00:00:00Z",
                      "Status": "Delivered",
                      "Products@odata.count": 3,
                      "Products@odata.nextLink": "http://service/odata/Customers(2)/Orders(1)/Products?$skip=3",
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
                      "@odata.etag": "W/\"wishlist-2-1\"",
                      "Id": 4,
                      "Name": "Smartphone",
                      "category": "Electronics"
                    },
                    {
                      "@odata.etag": "W/\"wishlist-2-2\"",
                      "Id": 6,
                      "Name": "Refrigerator",
                      "category": "HomeAppliances"
                    }
                  ]
                }
              ]
            }
            """);

        Assert.Equal(expectedJson, json);
    }

    public static string NormalizeJson([StringSyntax(StringSyntaxAttribute.Json)] string rawJson)
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

    class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
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
