using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json.Writers;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class ODataJsonNullableOfTWriter<T, TCustomState> : ODataJsonWriter<Nullable<T>, TCustomState>
    where T : struct
{
    public override bool Write(Nullable<T> value, ODataWriterState<TCustomState> state)
    {
        if (!value.HasValue)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

        return state.WriteValue(value.Value);
    }
}
