namespace Microsoft.OData.Serializer;

public interface IODataWriter<TState> : IODataWriter
{
    // Named this WriteObject to avoid conflict with the generic Write<T> in IODataWriter<T, TState>
    bool WriteObject(object value, TState state);
}
