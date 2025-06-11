namespace Microsoft.OData.Core.NewWriter2;

internal interface IResourceWriterProvider<TContext, TState>
{
    IODataWriter<TContext, TState, TValue> GetResourceWriter<TValue>(TContext context, TState state);
}
