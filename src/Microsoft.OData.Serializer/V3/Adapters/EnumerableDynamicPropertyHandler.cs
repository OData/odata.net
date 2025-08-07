using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class EnumerableDynamicPropertyHandler<TValue, TCustomState> : IDynamicPropertiesHandler<TCustomState>
{
    public void WriteDynamicProperties(object dynamicProperties, IDynamicPropertyWriter<TCustomState> writer, ODataJsonWriterState<TCustomState> state)
    {
        // TODO: support resumability in dynamic properties
        // We can store the enumerator in the state and use it to resume for where it left off.
        var enumerable = dynamicProperties as IEnumerable<KeyValuePair<string, TValue>>;
        Debug.Assert(enumerable != null, "Dynamic properties should be of type IEnumerable<KeyValuePair<string, TValue>>");

        foreach (var kvp in enumerable)
        {
            writer.WriteDynamicProperty(kvp.Key.AsSpan(), kvp.Value, state);
        }
    }
}
