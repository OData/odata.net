using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonDecimalWriter<TCustomState> : ODataJsonWriter<decimal, TCustomState>
{
    public override bool Write(decimal value, ODataJsonWriterState<TCustomState> state)
    {
        // TODO: support write as string
        state.JsonWriter.WriteNumberValue(value);
        return true;
    }
}
