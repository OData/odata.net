
using System.Xml;

namespace Microsoft.OData.Serializer;

internal class ODataJsonDateTimeOffsetWriter<TCustomState> : ODataJsonWriter<DateTimeOffset, TCustomState>
{
    public override bool Write(DateTimeOffset value, ODataWriterState<TCustomState> state)
    {
        if (value.Offset == TimeSpan.Zero)
        {
            // The default legacy OData JSON writer uses the format yyyy-MM-ddThh:mm:ss.fffffffZ when the offset is 0
            // Utf8JsonWriter uses the format yyyy-MM-ddThh:mm:ss.fffffff+00:00
            // While both formats are valid IS0 8601, we decided to keep the output
            // consistent between the two writers to avoid breaking any client that may
            // have dependency on the original format.

            // This uses the existing OData format. But it's expensive.
            // TODO: Allocating a new string is quite expensive though,
            // we should optimize this or have a setting that allows us to fall back to Utf8JsonWriter's native DateTimeOffset format.
            var dateTimeOffsetString = XmlConvert.ToString(value);
            state.JsonWriter.WriteStringValue(dateTimeOffsetString);
            return true;
        }
        
        state.JsonWriter.WriteStringValue(value);
        return true;
    }
}
