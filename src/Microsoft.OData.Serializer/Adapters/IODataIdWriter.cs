
namespace Microsoft.OData.Serializer;

public interface IODataIdWriter<TCustomState>
{
    void WriteId(ReadOnlySpan<char> id, ODataWriterState<TCustomState> state);
    void WriteId(ReadOnlySpan<byte> id, ODataWriterState<TCustomState> state);
    void WriteId(Uri id, ODataWriterState<TCustomState> state);
}
