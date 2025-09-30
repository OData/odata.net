
namespace Microsoft.OData.Serializer;

public interface ICountWriter<TCustomState>
{
    void WriteCount(long count, ODataWriterState<TCustomState> state);
}
