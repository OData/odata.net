using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

internal interface IDynamicPropertyWriter<TCustomState>
{
    // TODO: support resumability for dynamic properties
    void WriteDynamicProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state);

    void WriteDynamicProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state);
}
