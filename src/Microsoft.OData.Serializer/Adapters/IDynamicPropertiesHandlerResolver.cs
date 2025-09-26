namespace Microsoft.OData.Serializer.Adapters;

internal interface IDynamicPropertiesHandlerResolver<TCustomState>
{
    IDynamicPropertiesHandler<TCustomState>? Resolve(Type dynamicPropertiesContainerType);
}
