using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal interface INextLinkRetrieverProvider<TContext, TState>
{
    INextLinkHandler<TContext, TState, TValue> GetNextLinkRetriever<TValue>(TState state, TContext context);
}
