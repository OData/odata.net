using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

public interface IValueWriter<TCustomState>
{
    bool WriteValue<T>(T value, ODataWriterState<TCustomState> state);
}
