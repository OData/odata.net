using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableGuidWriter<TCustomState> : ODataWriter<Guid?, ODataWriterState<TCustomState>>
{
    public override bool Write(Guid? value, ODataWriterState<TCustomState> state)
    {
        if (!value.HasValue)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

        state.JsonWriter.WriteStringValue(value.Value);
        return true;
    }
}
