using Microsoft.OData.Serializer.V3.Json;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface IAnnotationWriter<TCustomState>
{
    ValueTask WriteAnnotation<TValue>(ReadOnlySpan<char> name, TValue value, ODataJsonWriterState<TCustomState> state);
    ValueTask WriteAnnotation<TValue>(ReadOnlySpan<byte> name, TValue value, ODataJsonWriterState<TCustomState> state);
}
