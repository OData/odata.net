using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface IResourcePropertyWriterProvider<TContext, TState, TProperty>
{
    IResourcePropertyWriter<TResource, TProperty, TState, TContext> GetPropertyWriter<TResource>(
        TState state,
        TContext context);
}
