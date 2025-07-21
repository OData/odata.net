using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1005:Avoid excessive parameters on generic types", Justification = "<Pending>")]
public interface IResourcePropertyWriter<TResource, TProperty, TState, TContext>
{
    ValueTask WriteProperty(TResource resource, TProperty resourceProperty, TState state, TContext context);
}
