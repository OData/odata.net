namespace Microsoft.OData.Serializer;

internal interface IDynamicPropertiesHandlerResolver<TCustomState>
{
    IDynamicPropertiesHandler<TCustomState>? Resolve(Type dynamicPropertiesContainerType);
}
