using Microsoft.OData.Edm;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal class ClrTypeEdmJsonPropertyWriter<T> : IResourcePropertyWriter<T, IEdmProperty> where T : class
{
    public ValueTask WriteProperty(T resource, IEdmProperty property, ODataWriterState state)
    {
        // TODO: do not use reflection
        // We should be able to customize property mapper
        var clrProperty = typeof(T).GetProperty(property.Name);
        object value = clrProperty.GetValue(resource, null);

        // to customize property writing?
        state.WriterContext.JsonWriter.WritePropertyName(property.Name);
        JsonSerializer.Serialize(state.WriterContext.JsonWriter, value, value.GetType(), state.WriterContext.JsonSerializerOptions);
        return ValueTask.CompletedTask;
    }
}
