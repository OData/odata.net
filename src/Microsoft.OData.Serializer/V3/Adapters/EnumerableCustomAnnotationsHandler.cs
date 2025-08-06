using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal class EnumerableCustomAnnotationsHandler<TValue, TCustomState> : ICustomAnnotationsHandler<TCustomState>
{
    public bool CanHandle(object annotations)
    {
        // TODO: we should have separate handlers for Dictionary<string, TValue>, IDictionary, IReadOnlyDictionary, etc.
        // might help with perf (e.g. non-boxing enumerable for concrete Dictionary)
        // If we had to check for duplicates, we can omit such checks when dealing IDictionary or IReadOnlyDictionary
        // but not IEnumerable<KeyValuePair<string, TValue>>.
        var type = annotations.GetType();
        return type.IsAssignableTo(typeof(IEnumerable<KeyValuePair<string, TValue>>));
    }

    public async ValueTask WriteAnnotations(object annotations, IAnnotationWriter<TCustomState> writer, ODataJsonWriterState<TCustomState> state)
    {
        
        var enumerable = (IEnumerable<KeyValuePair<string, TValue>>)annotations;
        
        foreach (var kvp in enumerable)
        {
            await writer.WriteAnnotation(kvp.Key, kvp.Value, state);
        }
    }
}
