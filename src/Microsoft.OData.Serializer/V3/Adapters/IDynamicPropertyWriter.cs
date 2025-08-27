using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal interface IDynamicPropertyWriter<TCustomState>
{
    // TODO: support resumability for dynamic properties
    void WriteDynamicProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state);

    void WriteDynamicProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state);
}
