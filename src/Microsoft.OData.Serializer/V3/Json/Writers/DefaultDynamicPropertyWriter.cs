using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class DefaultDynamicPropertyWriter<TCustomState> : IDynamicPropertyWriter<TCustomState>
{
    internal static readonly DefaultDynamicPropertyWriter<TCustomState> Instance = new();

    public ValueTask WriteDynamicProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);
        return state.WriteValue(value);
    }

    public ValueTask WriteDynamicProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);
        return state.WriteValue(value);
    }
}
