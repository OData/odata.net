using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataResourcePocoJsonWriter<T> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T>
{
    public async ValueTask WriteAsync(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        // TODO: handle null, should we check value == null? or should have an IsNull method
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteStartObject();


        // TODO: implement field selection, annotations, etc.
        if (context.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            // Write context URL if needed
            var metadataWriter = context.MetadataWriterProvider.GetMetadataWriter<T>(context, state);
            await metadataWriter.WriteEtagPropertyAsync(value, state, context);
        }

        // TODO: bad, don't use reflection, and don't select properties without checking if they are selected
        foreach (var prop in (typeof(T)).GetProperties())
        {
            var propVal = prop.GetValue(value, null);
            jsonWriter.WritePropertyName(prop.Name);
            JsonSerializer.Serialize(context.JsonWriter, propVal);
        }

        jsonWriter.WriteEndObject();
    }
}