using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Json;

public abstract class ODataJsonWriter<T, TCustomState> : ODataWriter<T, ODataWriterState<TCustomState>>
{
}
