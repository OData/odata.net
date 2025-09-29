using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonGuidWriter<TCustomState> : ODataWriter<Guid, ODataWriterState<TCustomState>>
{
    public override bool Write(Guid value, ODataWriterState<TCustomState> state)
    {
        state.JsonWriter.WriteStringValue(value);
        return true;
    }
}
