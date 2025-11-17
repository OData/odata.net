
namespace Microsoft.OData.Serializer;

public abstract class ODataPropertySelector<TResource, TCustomState>
{
    internal ODataPropertySelector() // internal constructor to prevent external subclassing
    {
    }

    internal abstract bool WriteProperties(TResource resource, ODataWriterState<TCustomState> state);
}
