using Microsoft.OData.Serializer.Adapters;
using Microsoft.OData.Serializer.Json.State;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Serializer.Json.Writers;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class ODataJsonReadOnlyListWriter<TCollection, TElement, TCustomState> : ODataResourceSetBaseJsonWriter<TCollection, TElement, TCustomState>
    where TCollection : IReadOnlyList<TElement>
{
    public ODataJsonReadOnlyListWriter(ODataTypeInfo<TCollection, TCustomState>? typeInfo = null)
        : base(typeInfo)
    {
    }

    protected override bool WriteElements(TCollection value, ODataWriterState<TCustomState> state)
    {
        var propIndex = state.Stack.Current.EnumeratorIndex;
        for (int i = propIndex; i < value.Count; i++)
        {
            state.Stack.Current.EnumeratorIndex = i;
            bool success = state.WriteValue(value[i]);
            if (!success)
            {
                // If the write was not complete, we need to return false to indicate that we need to resume later.
                return false;
            }

            if (state.ShouldFlush())
            {
                state.Stack.Current.EnumeratorIndex = i + 1;
                return false;
            }
        }

        return true;
    }
}
