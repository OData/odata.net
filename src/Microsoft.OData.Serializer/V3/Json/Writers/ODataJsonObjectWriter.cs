using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal sealed class ODataJsonObjectWriter<TCustomState> : ODataJsonWriter<object, TCustomState>
{
    private static readonly Type ObjectType = typeof(object);

    public override bool Write(object value, ODataWriterState<TCustomState> state)
    {
        if (value is null)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

        var runtimeType = value.GetType();
        if (runtimeType == ObjectType)
        {
            // Write empty object
            state.JsonWriter.WriteStartObject();
            state.JsonWriter.WriteEndObject();
            return true;
        }

        var writer = state.GetWriter(runtimeType);
        return writer.WriteObject(value, state);
    }
}
