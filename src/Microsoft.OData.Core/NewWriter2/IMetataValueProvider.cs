namespace Microsoft.OData.Core.NewWriter2;

internal interface IMetadataValueProvider<TContext, TState> :
    ICollectionCounterProvider<TContext, TState>,
    INextLinkRetrieverProvider<TContext, TState>
{
}
