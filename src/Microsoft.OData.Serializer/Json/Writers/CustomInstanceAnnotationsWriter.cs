
namespace Microsoft.OData.Serializer;

internal class CustomInstanceAnnotationWriter<TCustomState> : IAnnotationWriter<TCustomState>
{
    public static readonly CustomInstanceAnnotationWriter<TCustomState> Instance = new();

    public void WriteAnnotation<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state)
    {
        // TODO: prefix annotation withs @ and ensure name includes . but does not start with odata.
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomAnnotationName(jsonWriter, name);
        bool completed = state.WriteValue(value);
        if (!completed)
        {
            throw new InvalidOperationException("Resumable annotation writes are not supported.");
        }
    }

    public void WriteAnnotation<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomAnnotationName(jsonWriter, name);
        bool completed = state.WriteValue(value);
        if (!completed)
        {
            throw new InvalidOperationException("Resumable annotation writes are not supported.");
        }
    }
}
