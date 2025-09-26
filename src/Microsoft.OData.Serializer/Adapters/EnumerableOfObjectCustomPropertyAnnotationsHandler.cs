using Microsoft.OData.Serializer.Json;
using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

internal class EnumerableOfObjectCustomPropertyAnnotationsHandler<TCustomState> : ICustomPropertyAnnotationsHandler<TCustomState>
{
    public static readonly EnumerableOfObjectCustomPropertyAnnotationsHandler<TCustomState> Instance = new();
    public void WriteCustomPropertyAnnotations(ReadOnlySpan<char> propertyName, object annotations, ODataWriterState<TCustomState> state)
    {
        var enumerable = annotations as IEnumerable<KeyValuePair<string, object>>;
        if (enumerable == null)
        {
            return;
        }

        foreach (var kvp in enumerable)
        {
            var jsonWriter = state.JsonWriter;
            JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, propertyName, kvp.Key);

            // TODO: Consider supporting resumable writes for custom property annotations.
            while (!state.WriteValue(kvp.Value))
            {
                // Write to completion
            }
        }

    }
}
