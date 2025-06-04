using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal interface ICollectionCounter<TContext, TState, TValue>
{
    bool TryGetCount(TValue value, TContext context, TState state, out long count);
}
