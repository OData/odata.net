namespace Microsoft.OData.Core.NewWriter2;

public interface IResourceWriterProvider<TContext, TState>
{
    IODataWriter<TContext, TState, TValue> GetResourceWriter<TValue>(TContext context, TState state);
}
