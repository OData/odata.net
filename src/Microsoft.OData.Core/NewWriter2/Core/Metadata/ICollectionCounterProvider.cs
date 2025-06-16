using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal interface ICollectionCounterProvider<TContext, TState, TProperty>
{
    ICollectionCounter<TContext, TState, TValue, TProperty> GetCounter<TValue>(TContext context, TState state);
}
