
namespace Microsoft.OData.Serializer;

internal interface IOpenPropertiesHandler<TCustomState>
{
    // TODO: support resumability for dynamic properties
    void WriteOpenProperties<TResource>(
        TResource resource,
        object dynamicProperties,
        Func<TResource, string, ODataWriterState<TCustomState>, object?>? getPropertyPreValueAnnotations,
        Func<TResource, string, ODataWriterState<TCustomState>, object?>? getPropertyPostValueAnnotations,
        IOpenPropertyHandler<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
