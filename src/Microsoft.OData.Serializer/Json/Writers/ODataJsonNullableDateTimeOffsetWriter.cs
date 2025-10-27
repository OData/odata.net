
namespace Microsoft.OData.Serializer;

internal class ODataJsonNullableDateTimeOffsetWriter<TCustomState> : ODataJsonWriter<DateTimeOffset?, TCustomState>
{
    public override bool Write(DateTimeOffset? value, ODataWriterState<TCustomState> state)
    {
        if (!value.HasValue)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

#pragma warning disable CA1305 // Specify IFormatProvider
        state.JsonWriter.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")); // Ensure OData format is applied
#pragma warning restore CA1305 // Specify IFormatProvider
        return true;
    }
}
