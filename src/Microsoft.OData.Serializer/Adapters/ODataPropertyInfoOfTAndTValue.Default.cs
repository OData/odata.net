namespace Microsoft.OData.Serializer.Adapters;

// TODO: I'd want to create a default version of this with DefaultState,
// i.e. ODataPropertyInfo<TDeclaringType, TValue> : ODataPropertyInfo<TDeclaringType, TValue, DefaultState>
// But that would conflict with ODataPropertyInfo<TDeclaringType, TCustomState> definition and cause a compiler error.

//public class ODataPropertyInfo<TDeclaringType, TValue> : ODataPropertyInfo<TDeclaringType, TValue, DefaultState>
//{
//}
