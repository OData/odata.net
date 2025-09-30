
namespace Microsoft.OData.Serializer;

public interface INextLinkWriter<TCustomState>
{
    void WriteNextLink(ReadOnlySpan<char> nextLink, ODataWriterState<TCustomState> state);
    void WriteNextLink(ReadOnlySpan<byte> nextLink, ODataWriterState<TCustomState> state);
    void WriteNextLink(Uri nextLink, ODataWriterState<TCustomState> state);
}
