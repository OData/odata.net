namespace Microsoft.OData.Core.NewWriter2;

internal interface IMetadataValueProvider<TContext, TState, TProperty> :
    ICollectionCounterProvider<TContext, TState, TProperty>,
    INextLinkHandlerProvider<TContext, TState, TProperty>,
    IEtagHandlerProvider<TContext, TState>
{
}
