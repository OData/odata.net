using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonDateTimeOffsetWriter<TCustomState> : ODataJsonWriter<DateTimeOffset, TCustomState>
{
    public override bool Write(DateTimeOffset value, ODataWriterState<TCustomState> state)
    {
#pragma warning disable CA1305 // Specify IFormatProvider
        state.JsonWriter.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ")); // Ensure OData format is applied
#pragma warning restore CA1305 // Specify IFormatProvider
        return true;
    }
}
