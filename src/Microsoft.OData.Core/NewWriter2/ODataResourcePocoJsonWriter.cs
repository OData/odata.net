using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataResourcePocoJsonWriter<T> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T>
{
    public ValueTask WriteAsync(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        // TODO: implement field selection, annotations, etc.
        JsonSerializer.Serialize(context.JsonWriter, value, context.JsonSerializerOptions);
        return ValueTask.CompletedTask;
    }
}