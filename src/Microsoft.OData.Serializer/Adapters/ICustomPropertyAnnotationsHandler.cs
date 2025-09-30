
namespace Microsoft.OData.Serializer;

internal interface ICustomPropertyAnnotationsHandler<TCustomState>
{
    void WriteCustomPropertyAnnotations(
        ReadOnlySpan<char> propertyName,
        object annotations,
        ODataWriterState<TCustomState> state);
}
