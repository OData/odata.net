
using System.Globalization;

namespace Microsoft.OData.Serializer;

internal class ODataJsonNullableDecimalWriter<TCustomState> : ODataJsonWriter<decimal?, TCustomState>
{
    public override bool Write(decimal? value, ODataWriterState<TCustomState> state)
    {
        if (!value.HasValue)
        {
            state.JsonWriter.WriteNullValue();
            return true;
        }

        if (state.Options.Ieee754Compatible)
        {
            state.JsonWriter.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            state.JsonWriter.WriteNumberValue(value.Value);
        }

        return true;
    }
}
