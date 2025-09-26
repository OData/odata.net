using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

internal interface IDynamicPropertiesHandler<TCustomState>
{
    // TODO: support resumability for dynamic properties
    void WriteDynamicProperties<TResource>(
        TResource resource,
        object dynamicProperties,
        Func<TResource, string, ODataWriterState<TCustomState>, object?>? getPropertyPreValueAnnotations,
        Func<TResource, string, ODataWriterState<TCustomState>, object?>? getPropertyPostValueAnnotations,
        IDynamicPropertyWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
