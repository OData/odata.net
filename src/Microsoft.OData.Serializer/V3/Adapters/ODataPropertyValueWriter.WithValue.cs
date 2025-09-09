using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json;

namespace Microsoft.OData.Serializer.V3.Adapters;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public abstract class ODataPropertyValueWriter<TDeclaringType, TValue, TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    public abstract bool WriteValue(
        TDeclaringType resource,
        TValue value,
        IValueWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
