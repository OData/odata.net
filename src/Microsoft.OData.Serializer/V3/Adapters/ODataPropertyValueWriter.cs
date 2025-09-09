using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public abstract class ODataPropertyValueWriter<TDeclaringType, TCustomState>
{
    public abstract bool WriteValue(
        TDeclaringType resource,
        IValueWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
