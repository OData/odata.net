using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal interface IMetadataWriter<TContext, TState, TValue>
{
    
    ValueTask WriteContextUrlAsync(TContext context, TState state, TValue value);
}
