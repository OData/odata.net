
using System.Globalization;

namespace Microsoft.OData.Serializer;

internal class ODataJsonDecimalWriter<TCustomState> : ODataJsonWriter<decimal, TCustomState>
{
    public override bool Write(decimal value, ODataWriterState<TCustomState> state)
    {
        if (state.Options.Ieee754Compatible)
        {
            state.JsonWriter.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            state.JsonWriter.WriteNumberValue(value);
        }

        return true;
    }
}
