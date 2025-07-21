using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Core;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public interface IODataWriter<TContext, TState, TValue>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    ValueTask WriteAsync(TValue value, TState state, TContext context);
}
