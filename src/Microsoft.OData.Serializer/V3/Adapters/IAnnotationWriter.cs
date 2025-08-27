using Microsoft.OData.Serializer.V3.Json;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface IAnnotationWriter<TCustomState>
{
    void WriteAnnotation<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state);
    void WriteAnnotation<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state);
}
