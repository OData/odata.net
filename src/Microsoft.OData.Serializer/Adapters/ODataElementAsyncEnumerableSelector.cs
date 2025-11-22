using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public class ODataElementAsyncEnumerableSelector<TResource, TElement, TCustomState>
    : ODataElementSelector<TResource, TCustomState>
{
    public required Func<TResource, ODataWriterState<TCustomState>, IAsyncEnumerable<TElement>> GetElements { get; set; }

    public Func<TResource, TElement, IValueWriter<TCustomState>, ODataWriterState<TCustomState>, bool>? WriteElement { get; set; }

    internal override bool WriteElements(TResource resource, ODataWriterState<TCustomState> state)
    {
        IAsyncEnumerator<TElement> enumerator;
        if (state.Stack.Current.CurrentEnumerator == null)
        {
            var elements = GetElements(resource, state);
            enumerator = elements.GetAsyncEnumerator();
            state.Stack.Current.CurrentEnumerator = enumerator;
            // TODO: This may create too many tasks allocation,
            // consider optimizing by avoiding AsTask() if value task already completed successfully
            Task<bool> moveNextTask = enumerator.MoveNextAsync().AsTask();
            state.Stack.Current.PendingTaskWithValue = moveNextTask;

            // The task will be awaited at the serializer root.
            // TODO: we are returning to the root for each iteration,
            // we should avoid the overhead of the return to the root if the
            // task is already completed successfully.
            return false;
        }

        // Retrieve the enumerator from the state so we can resume writing from where we left off.
        enumerator = (state.Stack.Current.CurrentEnumerator as IAsyncEnumerator<TElement>)!;
        Debug.Assert(enumerator != null, "CurrentEnumerator should be of type IEnumerator<TProperty>. Possible bug in state management and property resume operation.");
        bool didMoveNextSucceed = true;
        var pendingTask = state.Stack.Current.PendingTaskWithValue;
        if (pendingTask != null)
        {
            Task<bool> moveNextTask = pendingTask as Task<bool>;
            Debug.Assert(moveNextTask != null, "PendingTaskWithValue should be Task<bool> when resuming an async enumerable.");
            Debug.Assert(moveNextTask.IsCompletedSuccessfully, "PendingTaskWithValue should be completed successfully when resuming an async enumerable.");
            didMoveNextSucceed = moveNextTask.GetAwaiter().GetResult();
            state.Stack.Current.PendingTaskWithValue = null;
        }

        if (!didMoveNextSucceed)
        {
            // we reached the end of the collection;
            return true;
        }

        var item = enumerator.Current;
        if (!WriteElementImplementation(resource, item, state))
        {
            return false;
        }

        state.Stack.EndProperty();

        state.Stack.Current.PendingTaskWithValue = enumerator.MoveNextAsync().AsTask();
        return false;
    }

    protected virtual bool WriteElementImplementation(TResource resource, TElement element, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(WriteElement != null, "WriteProperty should not be null if this method is called.");

        return WriteElement(resource, element, DefaultValueWriter<TCustomState>.Instance, state);
    }
}
