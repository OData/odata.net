namespace Microsoft.OData.Serializer.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface IMetadataValueProvider<TContext, TState, TProperty> :
    ICollectionCounterProvider<TContext, TState, TProperty>,
    INextLinkHandlerProvider<TContext, TState, TProperty>,
    IEtagHandlerProvider<TContext, TState>
{
}
