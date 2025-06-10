﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal interface IMetadataWriter<TContext, TState, TValue>
{
    
    ValueTask WriteContextUrlAsync(TValue value, TState state, TContext context);
    ValueTask WriteCountPropertyAsync(TValue value, TState state, TContext context);
    ValueTask WriteNextLinkPropertyAsync(TValue value, TState state, TContext context);

    ValueTask WriteEtagPropertyAsync(TValue value, TState state, TContext context);
}
