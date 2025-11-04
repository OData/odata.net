using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.WriterParityTests;

public class PrimitiveValuesParityTests
{
    [Fact]
    public async Task ODataSerializer_Writes_PrimitiveValues_TheSame_As_ODataMessageWriter()
    {
        var entity = new Entity
        {
            Id = 1,
            Int64 = 1234567890123456789,
            DateTimeOffset = DateTimeOffset.Parse("2025-11-04T06:09:12.6365582Z")
        };

        var model = CreateEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Entities(1)", UriKind.Relative)
        ).ParseUri();

        var newSerializerOutput = await SerializedWithNewSerializer(entity, model, odataUri);
        var oldSerializerOutput = await SerializedWithODataMessageWriter(entity, model, odataUri);

        var expected = """
            {
              "@odata.context": "http://service/odata/$metadata#Entities/$entity",
              "Id": 1,
              "Int64": 1234567890123456789,
              "DateTimeOffset": "2025-11-04T06:09:12.6365582Z"
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(normalizedExpected, newSerializerOutput);
        Assert.Equal(newSerializerOutput, newSerializerOutput);
    }

    private async Task<string> SerializedWithNewSerializer(Entity entity, IEdmModel model, ODataUri odataUri)
    {
        var options = new ODataSerializerOptions();

        var output = new MemoryStream();
        await ODataSerializer.WriteAsync(entity, output, odataUri, model, options);
        output.Position = 0;
        var actual = Encoding.UTF8.GetString(output.ToArray());
        var normalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        return normalized;
    }

    private async Task<string> SerializedWithODataMessageWriter(Entity entity, IEdmModel model, ODataUri odataUri)
    {
        var output = new MemoryStream();
        var responseMessage = new TestResponseMessage(output);
        var settings = new ODataMessageWriterSettings
        {
            ODataUri = odataUri,
            EnableMessageStreamDisposal = false
        };

        await using var messageWriter = new ODataMessageWriter(responseMessage, settings, model);

        var writer = await messageWriter.CreateODataResourceWriterAsync();
        var resource = new ODataResource
        {
            TypeName = "ns.Entity",
            Properties = new[]
            {
                new ODataProperty { Name = "Id", Value = entity.Id },
                new ODataProperty { Name = "Int64", Value = entity.Int64 },
                new ODataProperty { Name = "DateTimeOffset", Value = entity.DateTimeOffset }
            }
        };

        await writer.WriteStartAsync(resource);
        await writer.WriteEndAsync();
        await writer.FlushAsync();

        output.Position = 0;
        var actual = Encoding.UTF8.GetString(output.ToArray());
        var normalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        return normalized;
    }

    private static IEdmModel CreateEdmModel()
    {
        EdmModel model = new EdmModel();
        var type = model.AddEntityType("ns", "Entity");
        type.AddKeys(type.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        type.AddStructuralProperty("Int64", EdmPrimitiveTypeKind.Int64);
        type.AddStructuralProperty("DateTimeOffset", EdmPrimitiveTypeKind.DateTimeOffset);

        var container = model.AddEntityContainer("ns", "Container");
        container.AddEntitySet("Entities", type);
        return model;
    }

    [ODataType("ns.Entity")]
    class Entity
    {
        public int Id { get; set; }
        public long Int64 { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
    }

    class TestResponseMessage(Stream output) : IODataResponseMessage, IODataResponseMessageAsync
    {
        private readonly Stream _stream = output;
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        public string GetHeader(string headerName)
        {
            _headers.TryGetValue(headerName, out var value);
            return value;
        }
        public Stream GetStream()
        {
            return _stream;
        }
        public IEnumerable<KeyValuePair<string, string>> Headers => _headers;
        public int StatusCode { get; set; }
        public void SetHeader(string headerName, string headerValue)
        {
            _headers[headerName] = headerValue;
        }

        public Task<Stream> GetStreamAsync()
        {
            return Task.FromResult(_stream);
        }
    }
}
