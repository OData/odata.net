using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Serializer;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class ODataJsonListWriter<TCollection, TElement, TCustomState> : ODataResourceSetBaseJsonWriter<TCollection, TElement, TCustomState>
    where TCollection : IList<TElement>
{
    public ODataJsonListWriter(ODataTypeInfo<TCollection, TCustomState>? typeInfo = null)
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
