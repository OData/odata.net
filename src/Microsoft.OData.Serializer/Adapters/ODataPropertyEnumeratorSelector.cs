using Microsoft.OData.Serializer.Json.State;
using Microsoft.OData.Serializer.Json.Writers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

public class ODataPropertyEnumeratorSelector<TResource, TPropertyEnumerator, TProperty, TCustomState>
    : ODataPropertySelector<TResource, TCustomState>
    where TPropertyEnumerator : IEnumerator<TProperty>
{
    public required Func<TResource, ODataWriterState<TCustomState>, TPropertyEnumerator>
        GetPropertiesEnumerator { get; set; }

    public Func<TResource, TProperty, IPropertyWriter<TCustomState>, ODataWriterState<TCustomState>, bool>?
        WriteProperty { get; set; }

    internal override bool WriteProperties(TResource resource, ODataWriterState<TCustomState> state)
    {
        bool isResuming = state.Stack.Current.CurrentEnumerator != null;

        if (!isResuming)
        {
            // This fast path allows to write all properties without boxing if everything fits in the buffer.

            // We get the enumerator without boxing.
            TPropertyEnumerator enumerator = GetPropertiesEnumerator(resource, state);
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
                bool success = WritePropertyImplementation(resource, propertyItem, state);
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
        IEnumerator<TProperty> boxedEnumerator = (state.Stack.Current.CurrentEnumerator as IEnumerator<TProperty>)!;
        Debug.Assert(boxedEnumerator != null, "CurrentEnumerator should be of type IEnumerator<TProperty>.");

        do
        {
            if (state.ShouldFlush())
            {
                return false;
            }

            var propertyItem = boxedEnumerator.Current;

            bool success = WritePropertyImplementation(resource, propertyItem, state);
            if (!success)
            {
                return false;
            }

            state.Stack.EndProperty();
        } while (boxedEnumerator.MoveNext());

        state.Stack.EndCollectionElement();
        return true;
    }

    protected virtual bool WritePropertyImplementation(TResource resource, TProperty propertyItem, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(WriteProperty != null, "WriteProperty should not be null if this method is called.");

        return WriteProperty(resource, propertyItem, DefaultPropertyWriter<TCustomState>.Instance, state);
    }
}
