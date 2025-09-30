namespace Microsoft.OData.Serializer;

public interface IODataWriter
{
    Type? Type { get; }
    bool CanWrite(Type type);
}
