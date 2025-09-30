namespace Microsoft.OData.Serializer;

internal interface IOpenPropertiesHandlerResolver<TCustomState>
{
    IOpenPropertiesHandler<TCustomState>? Resolve(Type dynamicPropertiesContainerType);
}
