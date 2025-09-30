namespace Microsoft.OData.Serializer;

internal abstract class OpenPropertiesHandlerFactory<TCustomState>
{
    public abstract bool CanHandle(Type type);

    public abstract IOpenPropertiesHandler<TCustomState> CreateHandler(Type type);
}
