namespace Microsoft.OData.Serializer;

public abstract class ODataWriter<T, TWriteState, TReadState> : IODataWriter<T, TWriteState, TReadState>
{
    public Type? Type { get; } = typeof(T);

    public virtual bool CanWrite(Type type)
    {
        return type == Type;
    }

    public abstract bool Write(T value, TWriteState state);

    public bool WriteObject(object value, TWriteState state)
    {
        return Write((T)value, state);
    }

    public virtual bool Read(TReadState state, out T value)
    {
        throw new NotImplementedException();
    }
}
