namespace Microsoft.OData.Serializer;

public abstract class ODataWriterFactory<TCustomState> : IODataWriter
{
    public Type? Type => null;

    public abstract bool CanWrite(Type type);

    public abstract IODataWriter CreateWriter(Type type, ODataSerializerOptions<TCustomState> options);
}
