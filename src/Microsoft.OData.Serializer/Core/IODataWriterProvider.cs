using Microsoft.OData.Edm;

namespace Microsoft.OData.Serializer;

public interface IODataWriterProvider<TWriteState, TReadState>
{
    public IODataWriter<T, TWriteState, TReadState> GetWriter<T>(IEdmModel? model); // TODO should we pass the state here?
}
