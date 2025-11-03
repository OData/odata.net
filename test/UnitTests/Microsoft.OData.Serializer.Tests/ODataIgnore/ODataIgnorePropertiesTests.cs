using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.ODataIgnore;

public class ODataIgnorePropertiesTests
{

    [Fact]
    public async Task WhenConditionIsAlways_IgnoreAllProperties()
    {
        // Arrange
        var customer = new CustomerWithIgnoreAlways
        {
            Id = 1,
            Name = "John Doe",
            Nickname = "JD",
            Age = 20,
            Active = true
        };

        var options = new ODataSerializerOptions();

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            GetEdmModel(),
            new Uri("http://service/odata"),
            new Uri("Customers(1)", UriKind.Relative)
        ).ParseUri();


        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(customer, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers/$entity"
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WhenConditionIsNever_DoesNotIgnoreAnySchemaProperty()
    {
        // Arrange
        var customer = new CustomerWithIgnoreNever
        {
            Id = 1,
            Name = "John Doe",
            Nickname = null,
            Age = null,
            Active = false
        };

        var options = new ODataSerializerOptions();

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            GetEdmModel(),
            new Uri("http://service/odata"),
            new Uri("Customers(1)", UriKind.Relative)
        ).ParseUri();


        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(customer, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers/$entity",
              "Id": 1,
              "Name": "John Doe",
              "Nickname": null,
              "Age": null,
              "Active": false
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WhenConditionIsWritingNull_IgnoreAllPropertiesWithNullValues()
    {
        // Arrange
        var customer = new CustomerWithIgnoreNull
        {
            Id = 1,
            Name = "John Doe",
            Active = false
        };

        var options = new ODataSerializerOptions();

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            GetEdmModel(),
            new Uri("http://service/odata"),
            new Uri("Customers(1)", UriKind.Relative)
        ).ParseUri();


        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(customer, stream, odataUri, model, options);

        // Assert
        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Customers/$entity",
              "Id": 1,
              "Name": "John Doe",
              "Active": false
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        var entityType = model.AddEntityType("ns", "Customer");
        entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
        entityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
        entityType.AddStructuralProperty("Nickname", EdmPrimitiveTypeKind.String, isNullable: true);
        entityType.AddStructuralProperty("Age", EdmPrimitiveTypeKind.Int32, isNullable: true);
        entityType.AddStructuralProperty("Active", EdmPrimitiveTypeKind.Boolean, isNullable: true);
        var container = model.AddEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("Customers", entityType);
        return model;
    }

    [ODataType("ns.Customer")]
    [ODataIgnoreProperties(ODataIgnoreCondition.Always)]
    class CustomerWithIgnoreAlways
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string? Nickname { get; set; }

        public int? Age { get; set; }
        public bool? Active { get; set; }
    }

    [ODataType("ns.Customer")]
    [ODataIgnoreProperties(ODataIgnoreCondition.WhenWritingNull)]
    class CustomerWithIgnoreNull
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Nickname { get; set; }

        public int? Age { get; set; }
        public bool? Active { get; set; }
    }

    [ODataType("ns.Customer")]
    [ODataIgnoreProperties(ODataIgnoreCondition.Never)]
    class CustomerWithIgnoreNever
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string? Nickname { get; set; }

        public int? Age { get; set; }
        public bool? Active { get; set; }
    }
}
