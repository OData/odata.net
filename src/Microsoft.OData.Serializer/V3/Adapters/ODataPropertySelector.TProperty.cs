using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public class ODataPropertySelector<TResource, TProperty, TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
    : ODataPropertySelector<TResource, TCustomState>
{
    public Func<TResource, ODataWriterState<TCustomState>, IEnumerable<TProperty>>?
        GetProperties { get; set; }

    public Func<TResource, TProperty, IPropertyWriter<TResource, TCustomState>, ODataWriterState<TCustomState>, bool>?
        WriteProperty { get; set; }

    public Func<TResource, TProperty, ODataWriterState<TCustomState>, string>?
        GetPropertyName { get; set; }
}
