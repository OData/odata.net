using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

public interface IEtagWriter<TCustomState>
{
    void WriteEtag(ReadOnlySpan<char> etag, ODataWriterState<TCustomState> state);
    void WriteEtag(ReadOnlySpan<byte> etag, ODataWriterState<TCustomState> state);
}
