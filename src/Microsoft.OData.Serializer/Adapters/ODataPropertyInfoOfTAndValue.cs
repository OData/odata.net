
using System.Diagnostics;

namespace Microsoft.OData.Serializer;

// TODO: I'd want to create a default version of this with DefaultState,
// i.e. ODataPropertyInfo<TDeclaringType, TValue> : ODataPropertyInfo<TDeclaringType, TValue, DefaultState>
// But that would conflict with ODataPropertyInfo<TDeclaringType, TCustomState> definition and cause a compiler error.

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public sealed class ODataPropertyInfo<TDeclaringType, TValue, TCustomState> : ODataPropertyInfo<TDeclaringType, TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    public Func<TDeclaringType, ODataWriterState<TCustomState>, TValue>? GetValue { get; init; }

    public Action<TDeclaringType, TValue, ODataReaderState<TCustomState>>? SetValue { get; init; }

    internal protected override bool WritePropertyValue(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        if (GetValue != null)
        {
            return WritePropertyValue(resource, GetValue(resource, state), state);
        }
        else
        {
            return base.WritePropertyValue(resource, state);
        }
    }

    protected internal override bool ReadPropertyValue(TDeclaringType resource, ODataReaderState<TCustomState> state)
    {
        if (SetValue != null)
        {
            bool success = state.ReadValue(out TValue value);
            Debug.Assert(success, "Resumability is not yet supported, so we can't handle ReadValue returning false. Add support for it then remove this assertion.");
            SetValue(resource, value, state);
            return success;
        }

        return base.ReadPropertyValue(resource, state);
    }
}
