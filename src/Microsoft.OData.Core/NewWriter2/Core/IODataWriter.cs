using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal interface IODataWriter<TContext, TState, TValue>
{
    ValueTask WriteAsync(TValue value, TState state, TContext context);
}
