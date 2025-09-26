using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

public abstract class ODataPropertySelector<TResource, TCustomState>
{
    internal abstract bool WriteProperties(TResource resource, ODataWriterState<TCustomState> state);
}
