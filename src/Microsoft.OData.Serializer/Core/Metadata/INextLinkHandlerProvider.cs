namespace Microsoft.OData.Serializer.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface INextLinkHandlerProvider<TContext, TState, TProperty>
{
    INextLinkHandler<TContext, TState, TValue, TProperty> GetNextLinkHandler<TValue>(TState state, TContext context);
}
