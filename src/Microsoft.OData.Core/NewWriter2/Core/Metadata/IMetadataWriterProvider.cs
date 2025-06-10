using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal interface IMetadataWriterProvider<TContext, TState>
{
    IMetadataWriter<TContext, TState, TValue> GetMetadataWriter<TValue>(TContext context, TState state);
}
