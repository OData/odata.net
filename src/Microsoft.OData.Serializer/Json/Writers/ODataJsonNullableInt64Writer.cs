using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.OData.Serializer;

internal class ODataJsonNullableInt64Writer<TCustomState> : ODataWriter<long?, ODataWriterState<TCustomState>>
{
    public override bool Write(long? value, ODataWriterState<TCustomState> state)
    {
        if (!value.HasValue)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

        if (state.Options.Ieee754Compatible)
        {
            state.JsonWriter.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            state.JsonWriter.WriteNumberValue(value.Value);
        }

        return true;
    }
}
