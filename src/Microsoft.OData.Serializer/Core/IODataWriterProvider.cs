using Microsoft.OData.Edm;

namespace Microsoft.OData.Serializer.Core;

public interface IODataWriterProvider<TState>
{
    public IODataWriter<T, TState> GetWriter<T>(IEdmModel? model); // TODO should we pass the state here?
}
