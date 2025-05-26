using Microsoft.OData.Edm;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal class ClrTypeEdmJsonPropertyWriter<T> : IResourcePropertyWriter<T, IEdmProperty>
{
    public async ValueTask WriteProperty(T resource, IEdmProperty property, ODataWriterState state)
    {
        // TODO: do not use reflection
        // We should be able to customize property mapper
        var clrProperty = typeof(T).GetProperty(property.Name);

        object value = clrProperty.GetValue(resource, null);

        // to customize property writing?
        state.WriterContext.JsonWriter.WritePropertyName(property.Name);

       

        if (property.Type.IsCollection())
        {
            var edmElementType = property.Type.AsCollection().ElementType();
            state.WriterContext.JsonWriter.WriteStartArray();

            // TODO: should not use reflection casting
            foreach (var item in (System.Collections.IEnumerable)value)
            {
                if (item == null)
                {
                    continue;
                }

                if (edmElementType.IsEntity() || edmElementType.IsComplex())
                {
                    // Susceptible to boxing, so we should avoid this if possible
                    var writer = state.WriterContext.WriterProvider.GetValueWriter(
                        edmElementType.Definition,
                        item.GetType(),
                        state.WriterContext);


                    await writer.WriteAsync(item, state);
                }
                else
                {
                    // TODO better handling for primitives
                    JsonSerializer.Serialize(state.WriterContext.JsonWriter, item, item.GetType(), state.WriterContext.JsonSerializerOptions);
                }
                
            }

            state.WriterContext.JsonWriter.WriteEndArray();
            return;
        }
        else if (property.Type.IsEntity() || property.Type.IsComplex())
        {
            var writer = state.WriterContext.WriterProvider.GetValueWriter(
                property.Type.Definition,
                clrProperty.PropertyType,
                state.WriterContext);
            await writer.WriteAsync(value, state);
            return;
        }


        // Susceptible to boxing, so we should avoid this if possible
        JsonSerializer.Serialize(state.WriterContext.JsonWriter, value, value.GetType(), state.WriterContext.JsonSerializerOptions);
    }
}
