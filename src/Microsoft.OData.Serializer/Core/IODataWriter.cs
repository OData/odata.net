namespace Microsoft.OData.Serializer.Core;

public interface IODataWriter
{
    Type? Type { get; }
    bool CanWrite(Type type);
}
