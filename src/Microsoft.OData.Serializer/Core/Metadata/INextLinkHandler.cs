using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface INextLinkHandler<TContext, TState, TValue, TProperty>
{
    // TODO: This coupling to Uri type is not ideal and should be changed. This may lead
    // to expensive, unnecessary allocations of the Uri type in the common case
    // where instead a lighter weight type would be sufficient.
    // Should this be generic on the return type as well? How would know what type it was registered with?
    bool HasNextLinkValue(TValue value, TState state, TContext context, out Uri nextLink);
    void WriteNextLinkValue(TValue value, TState state, TContext context);

    bool HasNestedNextLinkValue(TValue value, TProperty resourceProperty, TState state, TContext context, out Uri nextLink);
    void WriteNestedNextLinkValue(TValue value, TProperty resourceProperty, TState state, TContext context);
}
