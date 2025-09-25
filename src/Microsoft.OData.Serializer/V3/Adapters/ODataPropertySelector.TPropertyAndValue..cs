using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public class ODataPropertySelector<TResource, TProperty, TValue, TCustomState>
    : ODataPropertySelector<TResource, TProperty, TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    public Func<TResource, TProperty, ODataWriterState<TCustomState>, TValue>?
        GetPropertyValue {get; set; }
}
