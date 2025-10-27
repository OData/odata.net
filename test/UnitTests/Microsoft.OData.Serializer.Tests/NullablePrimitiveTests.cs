using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests;

public class NullablePrimitiveTests
{
    [Fact]
    public async Task WritesPrimitiveValueWhenNullablePrimitiveHasValue()
    {
        var entity = new TestEntity
        {
            Bool = true,
            Byte = 1,
            SByte = -1,
            Int = -123,
            Uint = 123,
            Short = -12,
            UShort = 12,
            Long = -12345,
            ULong = 12345,
            Float = 1.23f,
            Double = 4.56,
            Decimal = 7.89m,
            DateTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            DateTimeOffset = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero),
            Guid = Guid.Parse("12345678-1234-1234-1234-1234567890ab")
        };

        var model = CreateModel();

        var options = new ODataSerializerOptions();

        var stream = new MemoryStream();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("TestEntities(1)", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(entity, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#TestEntities/$entity",
              "Id": 0,
              "Bool": true,
              "Byte": 1,
              "SByte": -1,
              "Int": -123,
              "Uint": 123,
              "Short": -12,
              "UShort": 12,
              "Long": -12345,
              "ULong": 12345,
              "Float": 1.23,
              "Double": 4.56,
              "Decimal": 7.89,
              "DateTime": "2024-01-01T12:00:00Z",
              "DateTimeOffset": "2024-01-01T12:00:00Z",
              "Guid": "12345678-1234-1234-1234-1234567890ab"
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WritesNullWhenNullablePrimitiveIsNull()
    {
        var entity = new TestEntity
        {
            Bool = null,
            Byte = null,
            SByte = null,
            Int = null,
            Uint = null,
            Short = null,
            UShort = null,
            Long = null,
            ULong = null,
            Float = null,
            Double = null,
            Decimal = null,
            DateTime = null,
            DateTimeOffset = null,
            Guid = null
        };

        var model = CreateModel();

        var options = new ODataSerializerOptions();

        var stream = new MemoryStream();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("TestEntities(1)", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(entity, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#TestEntities/$entity",
              "Id": 0,
              "Bool": null,
              "Byte": null,
              "SByte": null,
              "Int": null,
              "Uint": null,
              "Short": null,
              "UShort": null,
              "Long": null,
              "ULong": null,
              "Float": null,
              "Double": null,
              "Decimal": null,
              "DateTime": null,
              "DateTimeOffset": null,
              "Guid": null
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel CreateModel()
    {
        var model = new EdmModel();
        var testEntityType = new EdmEntityType("ns", "TestEntity");
        testEntityType.AddKeys(testEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        testEntityType.AddStructuralProperty("Bool", EdmPrimitiveTypeKind.Boolean, isNullable: true);
        testEntityType.AddStructuralProperty("Byte", EdmPrimitiveTypeKind.Byte, isNullable: true);
        testEntityType.AddStructuralProperty("SByte", EdmPrimitiveTypeKind.SByte, isNullable: true);
        testEntityType.AddStructuralProperty("Int", EdmPrimitiveTypeKind.Int32, isNullable: true);
        testEntityType.AddStructuralProperty("Uint", EdmPrimitiveTypeKind.Int32, isNullable: true);
        testEntityType.AddStructuralProperty("Short", EdmPrimitiveTypeKind.Int16, isNullable: true);
        testEntityType.AddStructuralProperty("UShort", EdmPrimitiveTypeKind.Int16, isNullable: true);
        testEntityType.AddStructuralProperty("Long", EdmPrimitiveTypeKind.Int64, isNullable: true);
        testEntityType.AddStructuralProperty("ULong", EdmPrimitiveTypeKind.Int64, isNullable: true);
        testEntityType.AddStructuralProperty("Float", EdmPrimitiveTypeKind.Single, isNullable: true);
        testEntityType.AddStructuralProperty("Double", EdmPrimitiveTypeKind.Double, isNullable: true);
        testEntityType.AddStructuralProperty("Decimal", EdmPrimitiveTypeKind.Decimal, isNullable: true);
        testEntityType.AddStructuralProperty("DateTime", EdmPrimitiveTypeKind.DateTimeOffset, isNullable: true);
        testEntityType.AddStructuralProperty("DateTimeOffset", EdmPrimitiveTypeKind.DateTimeOffset, isNullable: true);
        testEntityType.AddStructuralProperty("Guid", EdmPrimitiveTypeKind.Guid, isNullable: true);
        model.AddElement(testEntityType);

        var container = new EdmEntityContainer("ns", "DefaultContainer");
        container.AddEntitySet("TestEntities", testEntityType);
        model.AddElement(container);
        return model;
    }

    [ODataType("ns.TestEntity")]
    class TestEntity
    {
        public int Id { get; set; }
        public bool? Bool { get; set; }
        public byte? Byte { get; set; }
        public sbyte? SByte { get; set; }
        public int? Int { get; set; }
        public uint? Uint { get; set; }
        public short? Short { get; set; }
        public ushort? UShort { get; set; }
        public long? Long { get; set; }
        public ulong? ULong { get; set; }

        public float? Float { get; set; }
        public double? Double { get; set; }
        public decimal? Decimal { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTimeOffset? DateTimeOffset { get; set; }
        public Guid? Guid { get; set; }
    }
}
