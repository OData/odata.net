using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

// TODO: internal because this is not yet supported by the ODataTypeInfoFactory.
#pragma warning disable CA1005 // Avoid excessive parameters on generic types
internal abstract class ODataPropertyValueWriter<TDeclaringType, TValue, TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    public abstract bool WriteValue(
        TDeclaringType resource,
        TValue value,
        IValueWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
