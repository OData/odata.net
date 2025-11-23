using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests;

public class RawValuePropertyBufferWriterTests
{
    [Theory]
    [InlineData(EdmPrimitiveTypeKind.Int32, "12")]
    public async Task CanWritePrimitivePropertyValueToBufferWriterDirectly(
        EdmPrimitiveTypeKind propertyType,
        object propertyValue,
        string expectedJson)
    {
        // Arrange
        var model = new EdmModel();
        var entityType = model.AddEntityType("ns", "Entity");
        entityType.AddStructuralProperty("RawProperty", propertyType);
        entityType.AddStructuralProperty("Foo", EdmPrimitiveTypeKind.String);
        model.AddEntityContainer("ns", "Container").AddEntitySet("Entities", entityType);

        var options = new ODataSerializerOptions();
        options.AddTypeInfo<Dictionary<string, object>>(new()
        {
            PropertySelector = new ODataPropertyEnumerableSelector<Dictionary<string, object>, KeyValuePair<string, object>>()
            {
                GetProperties = (entity, state) => entity,
                WriteProperty = (entity, property, writer, state) =>
                {
                    if (property.Key != "RawProperty")
                    {
                        return writer.WriteProperty(property.Key, property.Value, state);
                    }

                    // Writes the value to completion in a single synchronous call. Not resumable.
                    writer.WritePropertyToBuffer(static (bufferWriter, value, state) =>
                    {
                        var raw = value.ToString();
                        var maxTranscodingLength = raw!.Length * 6;
                        // note that this can grow the writer's internal buffer.
                        Span<byte> destination = bufferWriter.GetSpan(maxTranscodingLength);
                        Utf8.FromUtf16(raw, destination, out _, out var bytesWritten);
                        bufferWriter.Advance(bytesWritten);
                    }, property.Key, property.Value, state);

                    return true;
                }
            }
        });

        var odataUri = new ODataUriParser(
                model,
                new Uri("http://service/odata"),
                new Uri("Entities", UriKind.Relative)
            ).ParseUri();

        var stream = new MemoryStream();

        // Act
        var data = new Dictionary<string, object>
        {
            ["Id"] = 1,
            ["RawProperty"] = propertyValue,
            ["Foo"] = "Bar"
        };

        // Act
        await ODataSerializer.WriteAsync(data, stream, odataUri, model, options);
        
        // Assert
    }

    class Entity
    {
        public int Id { get; set; }
        public object Property { get; set; }
    }
}
