
namespace Microsoft.OData.Serializer;

public interface IEtagWriter<TCustomState>
{
    void WriteEtag(ReadOnlySpan<char> etag, ODataWriterState<TCustomState> state);
    void WriteEtag(ReadOnlySpan<byte> etag, ODataWriterState<TCustomState> state);
}
