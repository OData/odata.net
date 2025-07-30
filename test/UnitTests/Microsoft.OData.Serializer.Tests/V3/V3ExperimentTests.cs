using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.V3;
using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.UriParser;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

public class V3ODataSerializerTests
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

        options.AddTypeInfo<IList<Customer>>(new()
        {
            HasNextLink = (customers, state) => true,
            WriteNextLink = (customers, state) => state.WriteValue(new Uri("http://service/odata/Customers?$skip=2", UriKind.Absolute))
        });


        options.AddTypeInfo<Customer>(new()
        {
            HasEtag = (customers, state) => true,
            WriteEtag = (customer, state) => state.WriteValue($"W/\"{customer.Id}\""),
            Properties =
            [
                new()
                {
                    Name = "Id",
                    WriteValue = (customer, state) => state.WriteValue(customer.Id)
                },
                new()
                {
                    Name = "Name",
                    WriteValue = (customer, state) => state.WriteValue(customer.Name)
                },
                new()
                {
                    Name = "Emails",
                    WriteValue = (customer, state) => state.WriteValue(customer.Emails)
                },
                new()
                {
                    Name = "OtherAddresses",
                    WriteValue = (customer, state) => state.WriteValue(customer.OtherAddresses)
                },
                new()
                {
                    Name = "Orders",
                    WriteValue = (customer, state) => state.WriteValue(customer.Orders),
                    // TODO: Considering whether to have the count annotation of the orders type info instead.
                    // TODO: this should be dynamic logic that e.g. checks that the $count=true is present in the $expand.
                    // But it would be expensive to compute here unless the $expand is already parsed internally.
                    HasCount = (customer, state) => true,
                    WriteCount = (customer, state) => state.WriteValue(customer.Orders.Count),

                    // TODO: Realistically, this information would be computed from the result of the orders collection computation.
                    HasNextLink = (customer, state) => customer.Id == 1,
                    // A slight advantage of nested annotation handlers on the declaring type is that you still have access to the parent object.
                    // But this advantage breaks apart if we need a value from a grandparent object.
                    WriteNextLink = (customer, state) => state.WriteValue(new Uri($"http://service/odata/Customers({customer.Id})/Orders?$skip=2", UriKind.Absolute))
    },
                new()
                {
                    Name = "WishList",
                    WriteValue = (customer, state) => state.WriteValue(customer.WishList),

                    HasCount = (customer, state) => true,
                    WriteCount = (customer, state) => state.WriteValue(customer.WishList.Count),
                }
            ]
        });

        options.AddTypeInfo<Address>(new()
        {
            Properties =
            [
                new()
                {
                    Name = "City",
                    WriteValue = (address, state) => state.WriteValue(address.City)
                },
                new()
                {
                    Name = "Country",
                    WriteValue = (address, state) => state.WriteValue(address.Country)
                }
            ]
        });


        // TODO: Currently this is handled in the parent type info. But perhaps it's better to handle it here.
        //options.AddTypeInfo<IList<Order>>(new()
        //{
        //    // TODO: should be able to control whether it occurs above or below the collection value
        //    HasCount = (orders, state) => true,
        //    WriteCount = (orders, state) => state.WriteValue(orders.Count),
        //});

        options.AddTypeInfo<Order>(new()
        {
            HasEtag = (order, state) => true,
            WriteEtag = (order, state) => state.WriteValue($"W/\"order-{order.Id}\""),
            Properties =
            [
                new()
                {
                    Name = "Id",
                    WriteValue = (order, state) => state.WriteValue(order.Id)
                },
                new()
                {
                    Name = "OrderDate",
                    WriteValue = (order, state) => state.WriteValue(order.OrderDate)
                },
                new()
                {
                    Name = "Status",
                    WriteValue = (order, state) => state.WriteValue(order.Status)
                },
                new()
                {
                    Name = "Products",
                    WriteValue = (order, state) => state.WriteValue(order.Products),

                    HasCount = (order, state) => true,
                    WriteCount = (order, state) => state.WriteValue(order.Products.Count),

                    HasNextLink = (order, state) => true,
                    // TODO: we don't have access to the customer here do we compute the correct next link?
                    // We need a performant, customizable and generalizable way to handle this.
                    // Perhaps we need a mechanism for passing and modifying custom state,
                    // as well as exposing some state to the user.
                    WriteNextLink = (order, state) =>
                    {
                        // TODO: hack
                        int skip = order.Id == 1 ? 2 : 3;
                        int customerId = order.Id == 1 || order.Id == 2 ? 1 : 2;
                        return state.WriteValue(new Uri($"http://service/odata/Customers({customerId})/Orders({order.Id})/Products?$skip={skip}", UriKind.Absolute));
                    }
                }
            ]
        });

        options.AddTypeInfo<Product>(new()
        {
            HasEtag = (product, state) => true,
            WriteEtag = (product, state) => state.WriteValue($"W/\"product-{product.Id}\""),

            Properties =
            [
                new()
                {
                    Name = "Id",
                    WriteValue = (product, state) => state.WriteValue(product.Id)
                },
                new()
                {
                    Name = "Name",
                    WriteValue = (product, state) => state.WriteValue(product.Name)
                },
                new()
                {
                    Name = "Price",
                    WriteValue = (product, state) => state.WriteValue(product.Price),

                    // TODO: This should be more dynamic logic, that e.g. takes the $SelectExpand into consideration
                    // SelectExpand traversal would be expensive here, unless it's already stored into the state.
                    // Need to think about how to handle this in a performant, customizable and generalizable way.
                    // Hack: Skip Price property when the product is part of the "Customer.WishList" property
                    ShouldSkip = (product, state) => state.ParentPropertyInfo()?.Name == "WishList"
                },
                new()
                {
                    Name = "Category",
                    WriteValue = (product, state) => state.WriteValue(product.Category),
                    // TODO: This should be more dynamic logic, that e.g. takes the $SelectExpand into consideration
                    // SelectExpand traversal would be expensive here, unless it's already stored into the state.
                    // Need to think about how to handle this in a performant, customizable and generalizable way.
                    // Hack: Skip property when the product is part of the "Order.Products" property
                    ShouldSkip = (product, state) => state.ParentPropertyInfo()?.Name == "Products"
                }
            ]
        });

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

    //class CustomerCollectionWriter : EnumerableResourceSetJsonWriter<IList<Customer>, Customer>
    //{
    //    protected override bool HasNextLinkValue(
    //        IList<Customer> value,
    //        ODataJsonWriterStack state,
    //        ODataJsonWriterContext context,
    //        out Uri nextLink)
    //    {
    //        nextLink = new Uri("http://service/odata/Customers?$skip=2", UriKind.Absolute);
    //        return true;
    //    }

    //    protected override bool HasCountValue(IList<Customer> value, ODataJsonWriterStack state, ODataJsonWriterContext context, out long? count)
    //    {
    //        count = value.Count;
    //        return true;
    //    }
    //}

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
