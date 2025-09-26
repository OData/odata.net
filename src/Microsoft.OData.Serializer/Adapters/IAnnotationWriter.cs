using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

public interface IAnnotationWriter<TCustomState>
{
    void WriteAnnotation<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state);
    void WriteAnnotation<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state);
}
