using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.Serializer;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class ODataJsonEnumerableWriter<TCollection, TElement, TCustomState> : ODataResourceSetBaseJsonWriter<TCollection, TElement, TCustomState>
    where TCollection : IEnumerable<TElement>
{
    public ODataJsonEnumerableWriter(ODataTypeInfo<TCollection, TCustomState>? typeInfo = null)
        : base(typeInfo)
    {
    }

    protected override bool WriteElements(TCollection value, ODataWriterState<TCustomState> state)
    {
        IEnumerator<TElement> enumerator;
        if (state.Stack.Current.CurrentEnumerator == null)
        {
            enumerator = value.GetEnumerator();
            state.Stack.Current.CurrentEnumerator = enumerator;
            if (!enumerator.MoveNext())
            {
                // If the collection is empty, we can return true immediately.
                return true;
            }
        }
        else
        {
            // Retrieve the enumerator from the state so we can resume writing from where we left off.
            enumerator = state.Stack.Current.CurrentEnumerator as IEnumerator<TElement>;
            Debug.Assert(enumerator != null, "CurrentEnumerator should be of type IEnumerator<TElement>. Possible bug in state management and collection resume operation.");
        }

        do
        {
            if (state.ShouldFlush())
            {
                return false;
            }

            var item = enumerator.Current;
            if (!state.WriteValue(item))
            {
                return false;
            }

        } while (enumerator.MoveNext());


        return true;
    }
}
