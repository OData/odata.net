using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface IPropertyValueWriterProvider<TContext, TState, TProperty>
{
    IPropertyValueWriter<TContext, TState, TResource, TProperty> GetPropertyValueWriter<TResource>(
        TResource resource,
        TProperty resourceProperty,
        TState state,
        TContext context
    );
}
