using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public enum ODataIgnoreCondition
{
    /// <summary>
    /// Always ignores the property, regardless of its value.
    /// </summary>
    Always,
    /// <summary>
    /// Ignores the property when its value is null. This applies to reference types and nullable value types.
    /// It does not apply to non-nullable value types, as they cannot be null.
    /// </summary>
    WhenWritingNull
}
