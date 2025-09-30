
namespace Microsoft.OData.Serializer;

public abstract class ODataPropertySelector<TResource, TCustomState>
{
    internal abstract bool WriteProperties(TResource resource, ODataWriterState<TCustomState> state);
}
