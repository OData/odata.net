using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

// TODO: internal because this is not yet supported by the ODataTypeInfoFactory.
internal abstract class ODataPropertyValueWriter<TDeclaringType, TCustomState>
{
    public abstract bool WriteValue(
        TDeclaringType resource,
        IValueWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
