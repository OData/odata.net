namespace Microsoft.OData.Serializer.Core;

public interface IResourceWriterProvider<TContext, TState>
{
    IODataWriter<TContext, TState, TValue> GetResourceWriter<TValue>(TContext context, TState state);
}
