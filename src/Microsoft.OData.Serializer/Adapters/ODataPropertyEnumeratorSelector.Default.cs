using Microsoft.OData.Serializer.Json.State;

namespace Microsoft.OData.Serializer.Adapters;

public class ODataPropertyEnumeratorSelector<TDeclaringType, TPropertyEnumerator, TProperty> :
    ODataPropertyEnumeratorSelector<TDeclaringType, TPropertyEnumerator, TProperty, DefaultState>
    where TPropertyEnumerator : IEnumerator<TProperty>
{
}
