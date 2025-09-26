
namespace Microsoft.OData.Serializer;

public class ODataPropertyEnumeratorSelector<TDeclaringType, TPropertyEnumerator, TProperty> :
    ODataPropertyEnumeratorSelector<TDeclaringType, TPropertyEnumerator, TProperty, DefaultState>
    where TPropertyEnumerator : IEnumerator<TProperty>
{
}
