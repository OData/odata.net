using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal interface ICollectionCounterProvider<TContext, TState>
{
    ICollectionCounter<TContext, TState, TValue> GetCounter<TValue>(TContext context, TState state);
}
