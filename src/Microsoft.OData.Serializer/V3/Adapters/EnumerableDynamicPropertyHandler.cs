using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal class EnumerableDynamicPropertyHandler<TValue, TCustomState> : IDynamicPropertiesHandler<TCustomState>
{
    public async ValueTask WriteDynamicProperties(object dynamicProperties, IDynamicPropertyWriter<TCustomState> writer, ODataJsonWriterState<TCustomState> state)
    {
        var enumerable = dynamicProperties as IEnumerable<KeyValuePair<string, TValue>>;
        Debug.Assert(enumerable != null, "Dynamic properties should be of type IEnumerable<KeyValuePair<string, TValue>>");

        foreach (var kvp in enumerable)
        {
            await writer.WriteDynamicProperty(kvp.Key.AsSpan(), kvp.Value, state);
        }
    }
}
