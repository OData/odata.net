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
internal class EnumerableDynamicPropertyHandler<TDynamicValue, TCustomState> : IDynamicPropertiesHandler<TCustomState>
{
    public void WriteDynamicProperties<TResource>(
        TResource resource,
        object dynamicProperties,
        Func<TResource, string, ODataWriterState<TCustomState>, object?>? getPropertyPreValueAnnotations,
        Func<TResource, string, ODataWriterState<TCustomState>, object?>? getPropertyPostValueAnnotations,
        IDynamicPropertyWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state)
    {
        // TODO: support resumability in dynamic properties
        // We can store the enumerator in the state and use it to resume for where it left off.
        var enumerable = dynamicProperties as IEnumerable<KeyValuePair<string, TDynamicValue>>;
        Debug.Assert(enumerable != null, "Dynamic properties should be of type IEnumerable<KeyValuePair<string, TValue>>");

        bool noAnnotations = getPropertyPreValueAnnotations == null && getPropertyPostValueAnnotations == null;

        if (noAnnotations)
        {
            // avoid extra checks in the loop if there are no annotations
            foreach (var kvp in enumerable)
            {
                writer.WriteDynamicProperty(kvp.Key.AsSpan(), kvp.Value, state);
            }
        }
        else
        {
            foreach (var kvp in enumerable)
            {
                // write pre annotations
                if (getPropertyPreValueAnnotations != null)
                {
                    var preAnnotations = getPropertyPreValueAnnotations(resource, kvp.Key, state);
                    if (preAnnotations != null)
                    {
                        // TODO: we should use a resolve to get the annotations writer for the type of preAnnotations,
                        // but for convenience and expediency, we will just use the default one, expecting the type
                        // to be compatible with IEnumerable<KeyValuePair<string, object>>
                        EnumerableOfObjectCustomPropertyAnnotationsHandler<TCustomState>.Instance.WriteCustomPropertyAnnotations(
                            kvp.Key,
                            preAnnotations,
                            state);
                    }
                }
                writer.WriteDynamicProperty(kvp.Key.AsSpan(), kvp.Value, state);
                // write post annotations
                if (getPropertyPostValueAnnotations != null)
                {
                    var postAnnotations = getPropertyPostValueAnnotations(resource, kvp.Key, state);
                    if (postAnnotations != null)
                    {
                        EnumerableOfObjectCustomPropertyAnnotationsHandler<TCustomState>.Instance.WriteCustomPropertyAnnotations(
                            kvp.Key,
                            postAnnotations,
                            state);
                    }
                }
            }
        }
    }
}
