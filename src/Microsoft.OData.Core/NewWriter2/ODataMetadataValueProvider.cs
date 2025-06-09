using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataMetadataValueProvider : IMetadataValueProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    // TODO should these be concurrent dicts? But we expect them to be set at config time. Before serialization starts.
    Dictionary<Type, object> counters = new();
    Dictionary<Type, object> nextLinkRetrievers = new();

    // TODO: this approach will require us to create a new counter for each type T in generic types like IEnumerable<T>,
    // We should consider a factory pattern for such use cases. But we'll we create factories all across the library?
    public void MapCounter<TValue>(Func<TValue, ODataJsonWriterContext, ODataJsonWriterStack, long?> countFunc)
    {
        // TODO: double allocation, not ideal
        counters[typeof(TValue)] = new CollectionCounter<TValue>(countFunc);
    }

    public void MapNextLinkRetriever<TValue>(Func<TValue, ODataJsonWriterStack, ODataJsonWriterContext, Uri> nextLinkFunc)
    {
        nextLinkRetrievers[typeof(TValue)] = new NextLinkRetriever<TValue>(nextLinkFunc);
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
            // TODO: should we default to throwing an exception here, or should or no-op?
            throw new InvalidOperationException($"No counter registered for type {typeof(TValue).FullName}");
        }
    }

    public INextLinkRetriever<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetNextLinkRetriever<TValue>(
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        if (nextLinkRetrievers.TryGetValue(typeof(TValue), out var nextLinkObj))
        {
            return (INextLinkRetriever<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)nextLinkObj;
        }
        else
        {
            throw new InvalidOperationException($"No next link retriever registered for type {typeof(TValue).FullName}");
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


    class NextLinkRetriever<TValue>(Func<TValue, ODataJsonWriterStack, ODataJsonWriterContext, Uri> nextLinkFunc)
        : INextLinkRetriever<ODataJsonWriterContext, ODataJsonWriterStack, TValue>
    {
        public bool HasNextLinkValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context, out Uri nextLink)
        {
            nextLink = nextLinkFunc(value, state, context);
            if (nextLink != null)
            {
                return true;
            }

            return false;
        }

        public void WriteNextLinkValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
        {
            // TODO: for this implementation of NextLinkRetriever, we assume HasNextLink always returns the nextLink
            // and therefore this method should not be called. But we implement it just in case.
            // Or should we throw an exception instead? Or have some strategy pattern that doesn't require this to be called when not needed?
            bool hasNextLink = HasNextLinkValue(value, state, context, out var nextLink);
            Debug.Assert(hasNextLink == true, "WriteNextLinkValue should only be called if HasNextLinkValue returned true.");
            Debug.Assert(nextLink != null);
            context.JsonWriter.WriteStringValue(nextLink.AbsoluteUri);
        }
    }
}

