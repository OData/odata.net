using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

// TODO: I'd want to create a default version of this with DefaultState,
// i.e. ODataPropertyInfo<TDeclaringType, TValue> : ODataPropertyInfo<TDeclaringType, TValue, DefaultState>
// But that would conflict with ODataPropertyInfo<TDeclaringType, TCustomState> definition and cause a compiler error.

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public class ODataPropertyInfo<TDeclaringType, TValue, TCustomState> : ODataPropertyInfo<TDeclaringType, TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    public Func<TDeclaringType, ODataWriterState<TCustomState>, TValue>? GetValue { get; init; }

    internal protected override bool WritePropertyValue(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        if (this.GetValue != null)
        {
            return this.WritePropertyValue(resource, this.GetValue(resource, state), state);
        }
        else
        {
            return base.WritePropertyValue(resource, state);
        }
    }
}
