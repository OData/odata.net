using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

// TODO: internal because this is not yet supported by the ODataTypeInfoFactory.
internal abstract class ODataAsyncPropertyWriter<TDeclaringType> : ODataAsyncPropertyWriter<TDeclaringType, DefaultState>
{
}