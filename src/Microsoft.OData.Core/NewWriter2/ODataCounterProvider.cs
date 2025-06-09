using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataCounterProvider : ICollectionCounterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    Dictionary<Type, object> counters = new();

    // TODO: this approach will require us to create a new counter for each type T in generic types like IEnumerable<T>,
    // We should consider a factory pattern for such use cases. But we'll we create factories all across the library?
    public void MapCounter<TValue>(Func<TValue, ODataJsonWriterContext, ODataJsonWriterStack, long?> countFunc)
    {
        // TODO: double allocation, not ideal
        counters[typeof(TValue)] = new CollectionCounter<TValue>(countFunc);
    }

    public ICollectionCounter<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetCounter<TValue>(
        ODataJsonWriterContext context,
        ODataJsonWriterStack state)
    {
        if (counters.TryGetValue(typeof(TValue), out var counterObj))
        {
            return (ICollectionCounter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)counterObj;
        }
        else
        {
            throw new InvalidOperationException($"No counter registered for type {typeof(TValue).FullName}");
        }
    }

    class CollectionCounter<TValue>(Func<TValue, ODataJsonWriterContext, ODataJsonWriterStack, long?> countFunc)
        : ICollectionCounter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>
    {
        public bool HasCountValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context, out long? count)
        {
            count = default;
            long? result = countFunc(value, context, state);
            if (result.HasValue)
            {
                count = result.Value;
                return true;
            }

            return false;
        }

        public void WriteCountValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
        {
            // TODO: for this implementation of CollectionProvider, we assume HasCountValue always returns the count
            // and therefore this method should not be called. But we implement it just in case.
            // Or should we throw an exception instead? Or have some strategy pattern that doesn't require this to be called when not needed?
            bool hasCount = HasCountValue(value, state, context, out var count);
            Debug.Assert(hasCount);
            Debug.Assert(count.HasValue);
            context.JsonWriter.WriteNumberValue(count.Value);
        }
    }

}

