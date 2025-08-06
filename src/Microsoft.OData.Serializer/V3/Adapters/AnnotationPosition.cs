using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public enum AnnotationPosition
{
    /// <summary>
    /// The library determines the position of the annotation.
    /// </summary>
    Auto,
    /// <summary>
    /// The annotation is written before the value.
    /// </summary>
    PreValue,
    /// <summary>
    /// The annotation is written after the value.
    /// </summary>
    PostValue,
}
