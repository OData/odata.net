using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

using System.Collections;
using System.Diagnostics;

public class ODataElementEnumeratorSelector<TResource, TPropertyEnumerator, TElement, TCustomState>
    : ODataElementSelector<TResource, TCustomState>
    where TPropertyEnumerator : IEnumerator<TElement>
{
    public required Func<TResource, ODataWriterState<TCustomState>, TPropertyEnumerator>
        GetElementsEnumerator
    { get; set; }

    public Func<TResource, TElement, IValueWriter<TCustomState>, ODataWriterState<TCustomState>, bool>?
        WriteElement
    { get; set; }

    internal override bool WriteElements(TResource resource, ODataWriterState<TCustomState> state)
    {
        bool isResuming = state.Stack.Current.CurrentEnumerator != null;

        if (!isResuming)
        {
            // This fast path allows to write all properties without boxing if everything fits in the buffer.

            // We get the enumerator without boxing.
            TPropertyEnumerator enumerator = GetElementsEnumerator(resource, state);
            if (!enumerator.MoveNext())
            {
                // If the collection is empty, we can return immediately.
                return true;
            }

            do
            {
                if (state.ShouldFlush())
                {
                    // We box here to store the enumerator.
                    state.Stack.Current.CurrentEnumerator = enumerator;
                    return false;
                }

                var propertyItem = enumerator.Current;
                bool success = WriteElementImplementation(resource, propertyItem, state);
                if (!success)
                {
                    // Store the enumerator for resuming later. At this point, the enumerator is boxed.
                    state.Stack.Current.CurrentEnumerator = enumerator;
                    return false;
                }

                state.Stack.EndProperty();
            }
            while (enumerator.MoveNext());

            // if we got here, then we wrote all properties without flushing, and without boxing.
            return true;
        }

        // We get the boxed enumerator from the state
        IEnumerator<TElement> boxedEnumerator = (state.Stack.Current.CurrentEnumerator as IEnumerator<TElement>)!;
        Debug.Assert(boxedEnumerator != null, "CurrentEnumerator should be of type IEnumerator<TProperty>.");

        do
        {
            if (state.ShouldFlush())
            {
                return false;
            }

            var propertyItem = boxedEnumerator.Current;

            bool success = WriteElementImplementation(resource, propertyItem, state);
            if (!success)
            {
                return false;
            }

            state.Stack.EndProperty();
        } while (boxedEnumerator.MoveNext());

        state.Stack.EndCollectionElement();
        return true;
    }

    protected virtual bool WriteElementImplementation(TResource resource, TElement element, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(WriteElement != null, "WriteProperty should not be null if this method is called.");

        return WriteElement(resource, element, DefaultValueWriter<TCustomState>.Instance, state);
    }
}
