using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.OData.Serializer;

internal class ODataJsonInt64Writer<TCustomState> : ODataWriter<long, ODataWriterState<TCustomState>>
{
    public override bool Write(long value, ODataWriterState<TCustomState> state)
    {
        if (state.Options.Ieee754Compatible)
        {
            state.JsonWriter.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            state.JsonWriter.WriteNumberValue(value);
        }
        
        return true;
    }
}
