
namespace Microsoft.OData.Serializer;

public interface IValueWriter<TCustomState>
{
    bool WriteValue<T>(T value, ODataWriterState<TCustomState> state);
}
