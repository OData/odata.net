using Microsoft.OData.Serializer.Adapters;
using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class DefaultDynamicPropertyWriter<TCustomState> : IDynamicPropertyWriter<TCustomState>
{
    internal static readonly DefaultDynamicPropertyWriter<TCustomState> Instance = new();

    public void WriteDynamicProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);

        // TODO: consider supporting resumability in dynamic properties
        while (!state.WriteValue(value)) { } // Write to completion
    }

    public void WriteDynamicProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);
        while (!state.WriteValue(value)) { } // Write to completion
    }
}
