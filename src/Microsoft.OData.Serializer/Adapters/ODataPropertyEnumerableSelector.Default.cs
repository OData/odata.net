using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

public class ODataPropertyEnumerableSelector<TDeclaringType, TProperty>
    : ODataPropertyEnumerableSelector<TDeclaringType, TProperty, DefaultState>
{
}
