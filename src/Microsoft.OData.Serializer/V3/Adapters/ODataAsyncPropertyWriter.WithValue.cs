using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1005:Avoid excessive parameters on generic types",
    Justification = "Design decision to support generic custom state which could be struct.")]
public abstract class ODataAsyncPropertyWriter<TDeclaringType, TValue, TCustomState>
{
    public abstract ValueTask WriteValueAsync(
        TDeclaringType resource,
        TValue propertyValue,
        IStreamValueWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
