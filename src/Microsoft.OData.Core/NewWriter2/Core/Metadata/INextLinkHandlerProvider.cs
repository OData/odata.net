namespace Microsoft.OData.Core.NewWriter2;

internal interface INextLinkHandlerProvider<TContext, TState>
{
    INextLinkHandler<TContext, TState, TValue> GetNextLinkHandler<TValue>(TState state, TContext context);
}
