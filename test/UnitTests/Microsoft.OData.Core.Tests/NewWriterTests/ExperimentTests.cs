using Microsoft.OData.Core.NewWriter;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Microsoft.OData.Core.Tests.NewWriterTests;

public class ExperimentTests
{
    [Fact]
    public async Task WriteClrPayloadBasedOnIEdmModel()
    {
        var model = new EdmModel();

        var addressEntity = model.AddComplexType("ns", "Address");
        addressEntity.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
        addressEntity.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);

        var customerEntity = model.AddEntityType("ns", "Customer");
        var idProp = customerEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

        customerEntity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        customerEntity.AddStructuralProperty("EmailAddresses", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
        customerEntity.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(addressEntity, true));

        var customerEntitySet = model.AddEntityContainer("ns", "DefaultContainer")
            .AddEntitySet("Customers", customerEntity);


        var odataUri = new ODataUriParser(model, new Uri("Customers", UriKind.Relative)).ParseUri();

        List<Customer> data = [
            new Customer
            {
                Id = 1,
                Name = "John Doe",
                EmailAddresses = ["johndoe@mailer.com"],
                HomeAddress = new Address
                {
                    City = "Nairobi",
                    Country = "Kenya"
                }
            },
            
            new()
            {
                Id = 2,
                Name = "Jane Doe",
                EmailAddresses = ["janedoe@mailer.com"],
                HomeAddress = new Address
                {
                    City = "Redmond",
                    Country = "United States"
                }
            }
        ];

        using var stream = new MemoryStream();
        Utf8JsonWriter writer = new(stream);
        var propertySelector = new ClrTypeEdmPropertySelector<Customer>();
        var propertyWriter = new ClrTypeEdmJsonPropertyWriter<Customer>();

        var resourceWriter = new ODataConventionalJsonResourceWriter<Customer>(
            writer,
            propertySelector,
            propertyWriter
            );

        var responseWriter = new ODataConventionalEntitySetJsonResponseWriter<Customer>(writer, resourceWriter);

        var context = new ODataWriterContext
        {
            Model = model,
            JsonWriter = writer,
            WriterProvider = new ClrODataValueWriterProvider()
        };
        var state = new ODataWriterState
        {
            EdmType = customerEntitySet.Type,
            WriterContext = context
        };

        await responseWriter.WriteAsync(data, state);

        writer.Flush();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var writtenPayload = await reader.ReadToEndAsync();

        var expectedPayload =
            "{\"@odata.context\":\"contextUrl\",\"value\":[{\"Id\":1,\"Name\":\"John Doe\",\"EmailAddresses\":[\"johndoe@mailer.com\"],\"HomeAddress\":{\"City\":\"Nairobi\",\"Country\":\"Kenya\"}},{\"Id\":2,\"Name\":\"Jane Doe\",\"EmailAddresses\":[\"janedoe@mailer.com\"],\"HomeAddress\":{\"City\":\"Redmond\",\"Country\":\"United States\"}}]}";
            

        Assert.Equal(expectedPayload, writtenPayload);
    }

    [Fact]
    public async Task WriteClrPayloadBasedOnIEdmModelWithSelect()
    {
        var model = new EdmModel();

        var addressEntity = model.AddComplexType("ns", "Address");
        addressEntity.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
        addressEntity.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);

        var customerEntity = model.AddEntityType("ns", "Customer");
        var idProp = customerEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

        customerEntity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        customerEntity.AddStructuralProperty("EmailAddresses", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
        customerEntity.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(addressEntity, true));

        var customerEntitySet = model.AddEntityContainer("ns", "DefaultContainer")
            .AddEntitySet("Customers", customerEntity);


        var odataUri = new ODataUriParser(model, new Uri("Customers", UriKind.Relative)).ParseUri();

        List<Customer> data = [
            new Customer
            {
                Id = 1,
                Name = "John Doe",
                EmailAddresses = ["johndoe@mailer.com"],
                HomeAddress = new Address
                {
                    City = "Nairobi",
                    Country = "Kenya"
                }
            },

            new()
            {
                Id = 2,
                Name = "Jane Doe",
                EmailAddresses = ["janedoe@mailer.com"],
                HomeAddress = new Address
                {
                    City = "Redmond",
                    Country = "United States"
                }
            }
        ];

        using var stream = new MemoryStream();
        Utf8JsonWriter writer = new(stream);
        var propertySelector = new ClrTypeEdmPropertySelector<Customer>();
        var propertyWriter = new ClrTypeEdmJsonPropertyWriter<Customer>();

        var resourceWriter = new ODataConventionalJsonResourceWriter<Customer>(
            writer,
            propertySelector,
            propertyWriter
            );

        var uri = new ODataUriParser(model, new Uri("Customers?$select=Id,Name", UriKind.Relative)).ParseUri();

        var responseWriter = new ODataConventionalEntitySetJsonResponseWriter<Customer>(writer, resourceWriter);

        var context = new ODataWriterContext
        {
            Model = model,
            JsonWriter = writer,
            SelectExpandClause = uri.SelectAndExpand,
            WriterProvider = new ClrODataValueWriterProvider()
        };
        var state = new ODataWriterState
        {
            EdmType = customerEntitySet.Type,
            WriterContext = context
        };

        await responseWriter.WriteAsync(data, state);

        writer.Flush();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var writtenPayload = await reader.ReadToEndAsync();

        var expectedPayload =
            "{\"@odata.context\":\"contextUrl\",\"value\":[{\"Id\":1,\"Name\":\"John Doe\"},{\"Id\":2,\"Name\":\"Jane Doe\"}]}";


        Assert.Equal(expectedPayload, writtenPayload);
    }

    [Fact]
    public async Task WriteClrPayloadBasedOnIEdmModelWithSelectAndExpand()
    {
        var model = new EdmModel();

        var addressEntity = model.AddComplexType("ns", "Address");
        addressEntity.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
        addressEntity.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);

        var ordersEntity = model.AddEntityType("ns", "Orders");
        ordersEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
        ordersEntity.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Int32);
        ordersEntity.AddStructuralProperty("Currency", EdmPrimitiveTypeKind.String);

        var customerEntity = model.AddEntityType("ns", "Customer");
        var idProp = customerEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

        customerEntity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        customerEntity.AddStructuralProperty("EmailAddresses", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
        customerEntity.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(addressEntity, true));
        customerEntity.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "Orders",
            TargetMultiplicity = EdmMultiplicity.Many,
            Target = ordersEntity,
        });

        var customerEntitySet = model.AddEntityContainer("ns", "DefaultContainer")
            .AddEntitySet("Customers", customerEntity);


        var odataUri = new ODataUriParser(model, new Uri("Customers", UriKind.Relative)).ParseUri();

        List<Customer> data = [
            new Customer
            {
                Id = 1,
                Name = "John Doe",
                EmailAddresses = ["johndoe@mailer.com"],
                HomeAddress = new Address
                {
                    City = "Nairobi",
                    Country = "Kenya"
                },
                Orders = [
                    new Order {
                        Id = 1,
                        Amount = 100,
                        Currency = "USD"
                    },
                    new Order {
                        Id = 2,
                        Amount = 200,
                        Currency = "USD"
                    },
                ],
            },

            new()
            {
                Id = 2,
                Name = "Jane Doe",
                EmailAddresses = ["janedoe@mailer.com"],
                HomeAddress = new Address
                {
                    City = "Redmond",
                    Country = "United States"
                },
                Orders = [
                    new Order {
                        Id = 3,
                        Amount = 300,
                        Currency = "USD"
                    },
                ]
            }
        ];

        using var stream = new MemoryStream();
        Utf8JsonWriter jsonWriter = new(stream);
        var propertySelector = new ClrTypeEdmPropertySelector<Customer>();
        var propertyWriter = new ClrTypeEdmJsonPropertyWriter<Customer>();

        var resourceWriter = new ODataConventionalJsonResourceWriter<Customer>(
            jsonWriter,
            propertySelector,
            propertyWriter
            );

        var writerProvider = new ClrODataValueWriterProvider();

        var uri = new ODataUriParser(model, new Uri("Customers?$select=Id,Name&$expand=Orders($select=Amount)", UriKind.Relative)).ParseUri();

        var responseWriter = new ODataConventionalEntitySetJsonResponseWriter<Customer>(jsonWriter, resourceWriter);

        var context = new ODataWriterContext
        {
            Model = model,
            JsonWriter = jsonWriter,
            SelectExpandClause = uri.SelectAndExpand,
            WriterProvider = writerProvider
        };
        var state = new ODataWriterState
        {
            EdmType = customerEntitySet.Type,
            WriterContext = context
        };

        await responseWriter.WriteAsync(data, state);

        jsonWriter.Flush();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var writtenPayload = await reader.ReadToEndAsync();

        var expectedPayload =
            "{\"@odata.context\":\"contextUrl\",\"value\":[{\"Id\":1,\"Name\":\"John Doe\",\"Orders\":[{\"Amount\":100},{\"Amount\":200}]},{\"Id\":2,\"Name\":\"Jane Doe\",\"Orders\":[{\"Amount\":300}]}]}";


        Assert.Equal(expectedPayload, writtenPayload);
    }

    [Fact]
    public async Task WriteClrPayloadWithDynamicProperties()
    {
        var model = new EdmModel();

        var projectEntity = model.AddEntityType("ns", "Project", baseType: null, isAbstract: false, isOpen: true);
        projectEntity.AddKeys(projectEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        projectEntity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

        var projectSet = model.AddEntityContainer("ns", "DefaultContainer")
            .AddEntitySet("Projects", projectEntity);

        IEnumerable<Project> projects = [
            new Project
            {
                Id = 1,
                Name = "P1",
                DynamicProperties = new Dictionary<string, object>
                {
                    { "Status", "Active" },
                    { "Description", "test" },
                    { "Budget", 10000 }
                }
            },
            new Project
            {
                Id = 2,
                Name = "P2",
                DynamicProperties = new Dictionary<string, object>
                {
                    { "Status", "Complete" },
                    { "Description", "Great" },
                    { "Budget", 2000 }
                }
            }
        ];

        using var stream = new MemoryStream();
        Utf8JsonWriter jsonWriter = new(stream);
        var propertySelector = new ClrTypeEdmPropertySelector<Project>();
        var propertyWriter = new ClrTypeEdmJsonPropertyWriter<Project>();

        var resourceWriter = new ODataConventionalJsonResourceWriter<Project>(
            jsonWriter,
            propertySelector,
            propertyWriter
            );

        var writerProvider = new ClrODataValueWriterProvider();

        var uri = new ODataUriParser(model, new Uri("Projects", UriKind.Relative)).ParseUri();

        var responseWriter = new ODataConventionalEntitySetJsonResponseWriter<Project>(jsonWriter, resourceWriter);

        var context = new ODataWriterContext
        {
            Model = model,
            JsonWriter = jsonWriter,
            SelectExpandClause = uri.SelectAndExpand,
            WriterProvider = writerProvider
        };
        var state = new ODataWriterState
        {
            EdmType = projectSet.Type,
            WriterContext = context
        };

        await responseWriter.WriteAsync(projects, state);

        jsonWriter.Flush();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var writtenPayload = await reader.ReadToEndAsync();

        var expectedPayload =
            "{\"@odata.context\":\"contextUrl\",\"value\":["
            + "{\"Id\":1,\"Name\":\"P1\",\"Status\":\"Active\",\"Description\":\"test\",\"Budget\":10000},"
            + "{\"Id\":1,\"Name\":\"P2\",\"Status\":\"Complete\",\"Description\":\"Great\",\"Budget\":2000}"
            + "]}";

        Assert.Equal(expectedPayload, writtenPayload);
    }

    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<string> EmailAddresses { get; set; }

        public Address HomeAddress { get; set; }

        public List<Order> Orders { get; set; }

    }

    class Order
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }

    class Address
    {
        public string City { get; set; }
        public string Country { get; set; }
    }

    class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Dictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
    }
}
