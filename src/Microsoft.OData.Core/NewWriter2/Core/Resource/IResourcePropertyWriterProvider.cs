using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2.Core.Resource;

internal interface IResourcePropertyWriterProvider<TContext, TState, TProperty>
{
    IResourcePropertyWriter<TResource, TProperty, TState, TContext> GetPropertyWriter<TResource>(
        TState state,
        TContext context);
}
