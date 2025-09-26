using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class EnumerableCustomAnnotationsHandler<TValue, TCustomState> : ICustomAnnotationsHandler<TCustomState>
{
    public void WriteAnnotations(object annotations, IAnnotationWriter<TCustomState> writer, ODataWriterState<TCustomState> state)
    {
        
        var enumerable = (IEnumerable<KeyValuePair<string, TValue>>)annotations;
        
        foreach (var kvp in enumerable)
        {
            writer.WriteAnnotation(kvp.Key, kvp.Value, state);
        }
    }
}
