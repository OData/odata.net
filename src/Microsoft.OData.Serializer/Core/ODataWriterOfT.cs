namespace Microsoft.OData.Serializer.Core;

public abstract class ODataWriter<T, TState> : IODataWriter<T, TState>
{
    public Type? Type { get; } = typeof(T);

    public virtual bool CanWrite(Type type)
    {
        return type == Type;
    }

    public abstract bool Write(T value, TState state);

    public bool WriteObject(object value, TState state)
    {
        return Write((T)value, state);
    }
}
