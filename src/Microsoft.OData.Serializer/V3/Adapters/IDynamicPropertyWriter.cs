using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal interface IDynamicPropertyWriter<TCustomState>
{
    ValueTask WriteDynamicProperty<TValue>(ReadOnlySpan<char> name, TValue value, ODataJsonWriterState<TCustomState> state);

    ValueTask WriteDynamicProperty<TValue>(ReadOnlySpan<byte> name, TValue value, ODataJsonWriterState<TCustomState> state);
}
