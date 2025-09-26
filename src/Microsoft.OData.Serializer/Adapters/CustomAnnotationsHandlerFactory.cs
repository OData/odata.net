namespace Microsoft.OData.Serializer.Adapters;

internal abstract class CustomAnnotationsHandlerFactory<TCustomState>
{
    public abstract bool CanHandle(Type type);

    public abstract ICustomAnnotationsHandler<TCustomState> CreateHandler(Type type);
}
