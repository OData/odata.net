using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataResourceTypeInfo<T>
{
    public Type ClrType { get; set; } = typeof(T);

    public required IReadOnlyList<ODataPropertyInfo<T>> Properties { get; init; }
}
