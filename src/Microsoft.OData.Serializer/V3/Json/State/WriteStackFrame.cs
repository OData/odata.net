using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.State;

internal struct WriteStackFrame<TCustomState>
{
    public ODataResourceTypeInfo? ResourceTypeInfo { get; set; }
    public Adapters.ODataPropertyInfo? PropertyInfo { get; set; }

    public TCustomState CustomState;
}
