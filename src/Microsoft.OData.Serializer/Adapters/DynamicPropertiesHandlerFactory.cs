namespace Microsoft.OData.Serializer.Adapters;

internal abstract class DynamicPropertiesHandlerFactory<TCustomState>
{
    public abstract bool CanHandle(Type type);

    public abstract IDynamicPropertiesHandler<TCustomState> CreateHandler(Type type);
}
