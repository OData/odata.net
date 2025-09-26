using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

public interface IODataIdWriter<TCustomState>
{
    void WriteId(ReadOnlySpan<char> id, ODataWriterState<TCustomState> state);
    void WriteId(ReadOnlySpan<byte> id, ODataWriterState<TCustomState> state);
    void WriteId(Uri id, ODataWriterState<TCustomState> state);
}
