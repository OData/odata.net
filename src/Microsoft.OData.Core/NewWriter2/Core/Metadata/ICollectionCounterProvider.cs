using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface ICollectionCounterProvider<TContext, TState, TProperty>
{
    ICollectionCounter<TContext, TState, TValue, TProperty> GetCounter<TValue>(TContext context, TState state);
}
