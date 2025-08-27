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

    public void WriteDynamicProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);
        bool complete = state.WriteValue(value);
        if (!complete)
        {
            throw new InvalidOperationException("Resumable dynamic property writes are not yet supported.");
        }
    }

    public void WriteDynamicProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WritePropertyName(name);
        bool complete = state.WriteValue(value);
        if (!complete)
        {
            throw new InvalidOperationException("Resumable dynamic property writes are not yet supported.");
        }
    }
}
