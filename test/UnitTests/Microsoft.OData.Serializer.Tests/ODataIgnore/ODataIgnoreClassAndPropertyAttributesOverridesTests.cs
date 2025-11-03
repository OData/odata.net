using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.ODataIgnore;

/// <summary>
/// Tests scenarios where both class-level ODataIgnoreProperties and property-level ODataIgnore attributes are applied,
/// ensuring that the property-level attributes take precedence over class-level settings.
/// </summary>
public class ODataIgnoreClassAndPropertyAttributesOverridesTests
{
    [Fact]
    public async Task WhenTypeIgnoreConditionIsAlways_ShouldOnlyWritePropertiesWithConditionNever()
    {
        // Arrange
        var customer = new CustomerWithIgnoreAlways
        {
            Id = "1",
            Name = "John Doe",
            Nickname = "JD",
            Age = 30,
            Active = true,
            Password = "SecretPassword"
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
              "Id": "1"
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WhenTypeIgnoreConditionIsNever_ShouldWriteBasedOnPropertyIgnoreCondition()
    {
        // Arrange
        var customer = new CustomerWithIgnoreNever
        {
            Id = "1",
            Name = "John Doe",
            Nickname = null,
            Age = null,
            Active = false,
            Password = "SecretPassword"
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
              "Id": "1",
              "Name": "John Doe",
              "Age": null,
              "Active": false
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WhenTypeIgnoreConditionIsNull_ShouldSkillNullPropertiesUnlessTheyHaveIgnoreNeverCondition()
    {
        // Arrange
        var customer = new CustomerWithIgnoreNull
        {
            Id = "1",
            Name = null,
            Nickname = null,
            Age = null,
            Active = false,
            Password = "SecretPassword"
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
              "Id": "1",
              "Age": null,
              "Active": false
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        var customerType = new EdmEntityType("ns", "Customer");
        var idProp = customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
        customerType.AddKeys(idProp);
        customerType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        customerType.AddStructuralProperty("Nickname", EdmPrimitiveTypeKind.String);
        customerType.AddStructuralProperty("Age", EdmPrimitiveTypeKind.Int32);
        customerType.AddStructuralProperty("Active", EdmPrimitiveTypeKind.Boolean);
        customerType.AddStructuralProperty("Password", EdmPrimitiveTypeKind.String);
        model.AddElement(customerType);

        model.AddEntityContainer("ns", "Container")
            .AddEntitySet("Customers", customerType);

        return model;
    }

    [ODataType("ns.Customer")]
    [ODataIgnoreProperties(ODataIgnoreCondition.Always)]
    class CustomerWithIgnoreAlways
    {
        // This property will always be written, it overrides the class-level ignore condition.
        [ODataIgnore(ODataIgnoreCondition.Never)]
        public string Id { get; set; }

        // This property will never be written.
        public string Name { get; set; }

        // This property will never be written.
        [ODataIgnore(ODataIgnoreCondition.WhenWritingNull)]
        public string? Nickname { get; set; }

        // This property will never be written.
        public int? Age { get; set; }

        // This property will never be written.
        public bool Active { get; set; }

        // This property will never be written.
        [ODataIgnore(ODataIgnoreCondition.Always)]
        public string Password { get; set; }
    }

    [ODataType("ns.Customer")]
    [ODataIgnoreProperties(ODataIgnoreCondition.Never)]
    class CustomerWithIgnoreNever
    {
        // This property will always be written.
        public string Id { get; set; }

        // This property will always be written.
        public string Name { get; set; }

        // This property will be written only if the value is not null.
        [ODataIgnore(ODataIgnoreCondition.WhenWritingNull)]
        public string? Nickname { get; set; }

        // This property will always be written.
        public int? Age { get; set; }

        // This property will always be written.
        public bool Active { get; set; }

        // This property will never be written.
        [ODataIgnore(ODataIgnoreCondition.Always)]
        public string Password { get; set; }
    }

    [ODataType("ns.Customer")]
    [ODataIgnoreProperties(ODataIgnoreCondition.WhenWritingNull)]
    class CustomerWithIgnoreNull
    {
        // This property will always be written.
        public string Id { get; set; }

        // This property will be written only if the value is not null.
        [ODataIgnore(ODataIgnoreCondition.WhenWritingNull)]
        public string Name { get; set; }

        // This property will be written only if the value is not null.
        public string? Nickname { get; set; }

        // This property will always be written.
        [ODataIgnore(ODataIgnoreCondition.Never)]
        public int? Age { get; set; }

        // This property will always be written.
        public bool Active { get; set; }

        // This property will never be written.
        [ODataIgnore(ODataIgnoreCondition.Always)]
        public string Password { get; set; }
    }
}
