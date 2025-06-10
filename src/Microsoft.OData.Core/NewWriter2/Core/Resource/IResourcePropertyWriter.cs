using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal interface IResourcePropertyWriter<TResource, TProperty, TState, TContext>
{
    ValueTask WriteProperty(TResource resource, TProperty property, TState state, TContext context);
}
