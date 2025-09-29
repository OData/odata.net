using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer;

internal class ODataJsonInt64Writer<TCustomState> : ODataWriter<long, ODataWriterState<TCustomState>>
{
    public override bool Write(long value, ODataWriterState<TCustomState> state)
    {
        // TODO: might need to write as string
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
