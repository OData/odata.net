using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableByteWriter<TCustomState> : ODataJsonWriter<byte?, TCustomState>
{
    public override bool Write(byte? value, ODataWriterState<TCustomState> state)
    {
        if (value.HasValue)
        {
            state.JsonWriter.WriteNumberValue(value.Value);
            return true;
        }

        state.JsonWriter.WriteNullValue();
        return true;
    }
}
