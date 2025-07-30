using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Adapters;

public abstract class ODataResourceTypeInfo
{
    public required virtual Type Type { get; init; }
}
