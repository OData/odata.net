using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

public interface ICountWriter<TCustomState>
{
    void WriteCount(long count, ODataWriterState<TCustomState> state);
}
