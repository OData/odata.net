using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal interface IMetadataWriterProvider<TContext, TState, TProperty>
{
    IMetadataWriter<TContext, TState, TValue, TProperty> GetMetadataWriter<TValue>(TContext context, TState state);
}
