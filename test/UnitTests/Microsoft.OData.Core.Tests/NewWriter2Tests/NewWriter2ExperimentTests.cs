using Microsoft.OData.Core.NewWriter2;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.Test.OData.Utils.ODataLibTest;
using System;
using System.Collections.Generic;
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

        var metataProvider = new ODataMetadataValueProvider();
        
        var writerContext = new ODataJsonWriterContext
        {
            Model = model,
            ODataUri = odataUri,
            MetadataLevel = ODataMetadataLevel.Minimal,
            PayloadKind = ODataPayloadKind.ResourceSet,
            ODataVersion = ODataVersion.V4,
            JsonWriter = jsonWriter,
            ResourceWriterProvider = new ODataResourceWriterProvider(),
            MetadataWriterProvider = new ODataJsonMetadataWriterProvider(metataProvider),
        };

        var writerStack = new ODataJsonWriterStack();

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
        var metadataProvider = new ODataMetadataValueProvider();
        metadataProvider.MapCounter<IEnumerable<Project>>((projects, context, state) => projects.Count());
        metadataProvider.MapNextLinkRetriever<IEnumerable<Project>>((projects, context, state) =>
        {
            return new Uri("http://service/odata/Products?$skiptoken=skip", UriKind.Absolute);
        });
       
        var writerContext = new ODataJsonWriterContext
        {
            Model = model,
            ODataUri = odataUri,
            ODataVersion = ODataVersion.V4,
            MetadataLevel = ODataMetadataLevel.Minimal,
            PayloadKind = ODataPayloadKind.ResourceSet,
            JsonWriter = jsonWriter,
            ResourceWriterProvider = new ODataResourceWriterProvider(),
            MetadataWriterProvider = new ODataJsonMetadataWriterProvider(metadataProvider),
        };

        var writerStack = new ODataJsonWriterStack();

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
            new Uri("Customers?select=Id,Name,Emails,OtherAddresses&$expand=Orders($expand=Products($select=Id,Name,Price;$count=true)&$count=true),WishList($select=Id,Name,category)&count=true", UriKind.Relative)
        ).ParseUri();



    }

    private IEdmModel CreateNestedEcommerceModel()
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
