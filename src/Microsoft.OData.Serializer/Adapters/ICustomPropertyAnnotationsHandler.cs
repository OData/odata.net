using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

internal interface ICustomPropertyAnnotationsHandler<TCustomState>
{
    void WriteCustomPropertyAnnotations(
        ReadOnlySpan<char> propertyName,
        object annotations,
        ODataWriterState<TCustomState> state);
}
