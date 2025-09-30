using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Serializer;

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
