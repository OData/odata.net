using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal interface IEtagHandler<TContext, TState, TValue>
{
    public bool HasEtagValue(TValue value, TState state, TContext context, out string? etagValue);
    public void WriteEtagValue(TValue value, TState state, TContext context);

}
