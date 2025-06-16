namespace Microsoft.OData.Core.NewWriter2;

internal interface INextLinkHandlerProvider<TContext, TState, TProperty>
{
    INextLinkHandler<TContext, TState, TValue, TProperty> GetNextLinkHandler<TValue>(TState state, TContext context);
}
