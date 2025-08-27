using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class ODataJsonEnumWriter<T, TCustomState> : ODataJsonWriter<T, TCustomState>
    where T : Enum
{
    public override bool Write(T value, ODataWriterState<TCustomState> state)
    {
        // TODO: optimize this instead of calling value.ToString()
        // What if the enum value does not 
        state.JsonWriter.WriteStringValue(value.ToString());
        return true;
    }
}
