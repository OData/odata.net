using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal class EnumerableCustomAnnotationsHandler<TValue, TCustomState> : ICustomAnnotationsHandler<TCustomState>
{
    public async ValueTask WriteAnnotations(object annotations, IAnnotationWriter<TCustomState> writer, ODataJsonWriterState<TCustomState> state)
    {
        
        var enumerable = (IEnumerable<KeyValuePair<string, TValue>>)annotations;
        
        foreach (var kvp in enumerable)
        {
            await writer.WriteAnnotation(kvp.Key, kvp.Value, state);
        }
    }
}
