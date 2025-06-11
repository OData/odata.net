using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal interface IPropertyValueWriterProvider<TContext, TState, TProperty>
{
    IPropertyValueWriter<TContext, TState, TResource, TProperty> GetPropertyValueWriter<TResource>(
        TResource resource,
        TProperty property,
        TState state,
        TContext context
    );
}
