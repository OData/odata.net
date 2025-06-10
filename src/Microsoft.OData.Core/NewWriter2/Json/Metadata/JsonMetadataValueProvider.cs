using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class JsonMetadataValueProvider : IMetadataValueProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    // TODO should these be concurrent dicts? But we expect them to be set at config time. Before serialization starts.
    Dictionary<Type, object> counters = new();
    Dictionary<Type, object> nextLinkRetrievers = new();
    Dictionary<Type, object> etagHandlers = new();

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

    public void MapEtagHandler<TValue>(Func<TValue, ODataJsonWriterStack, ODataJsonWriterContext, string> etagFunc)
    {
        etagHandlers[typeof(TValue)] = new EtagHandler<TValue>(etagFunc);
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
            return NoOpCollectionCounter<TValue>.Instance;
            // TODO: should we default to throwing an exception here, or should or no-op?
            //throw new InvalidOperationException($"No counter registered for type {typeof(TValue).FullName}");
        }
    }

    public INextLinkHandler<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetNextLinkRetriever<TValue>(
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        if (nextLinkRetrievers.TryGetValue(typeof(TValue), out var nextLinkObj))
        {
            return (INextLinkHandler<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)nextLinkObj;
        }
        else
        {
            return NoOpNextLinkRetriever<TValue>.Instance;
            // throw new InvalidOperationException($"No next link retriever registered for type {typeof(TValue).FullName}");
        }
    }

    public IEtagHandler<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetEtagHandler<TValue>(
        ODataJsonWriterStack state,
        ODataJsonWriterContext context
        )
    {
        if (etagHandlers.TryGetValue(typeof(TValue), out var etagObj))
        {
            return (IEtagHandler<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)etagObj;
        }
        else
        {
            return NoOpEtagHandler<TValue>.Instance;
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

    class NoOpCollectionCounter<TValue> : ICollectionCounter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>
    {
        public static readonly NoOpCollectionCounter<TValue> Instance = new();
        public bool HasCountValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context, out long? count)
        {
            count = null;
            return false;
        }
        public void WriteCountValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
        {
        }
    }


    class NextLinkRetriever<TValue>(Func<TValue, ODataJsonWriterStack, ODataJsonWriterContext, Uri> nextLinkFunc)
        : INextLinkHandler<ODataJsonWriterContext, ODataJsonWriterStack, TValue>
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

    class NoOpNextLinkRetriever<TValue> : INextLinkHandler<ODataJsonWriterContext, ODataJsonWriterStack, TValue>
    {
        public static readonly NoOpNextLinkRetriever<TValue> Instance = new();
        public bool HasNextLinkValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context, out Uri nextLink)
        {
            nextLink = null;
            return false;
        }
        public void WriteNextLinkValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
        {
        }
    }

    class EtagHandler<TValue>(Func<TValue, ODataJsonWriterStack, ODataJsonWriterContext, string> etagFunc)
        : IEtagHandler<ODataJsonWriterContext, ODataJsonWriterStack, TValue>
    {
        public bool HasEtagValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context, out string etagValue)
        {
            etagValue = etagFunc(value, state, context);
            return !string.IsNullOrEmpty(etagValue);
        }
        public void WriteEtagValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
        {
            bool hasEtag = HasEtagValue(value, state, context, out var etagValue);
            Debug.Assert(hasEtag);
            Debug.Assert(etagValue != null);
            context.JsonWriter.WriteStringValue(etagValue);
        }
    }

    class NoOpEtagHandler<TValue> : IEtagHandler<ODataJsonWriterContext, ODataJsonWriterStack, TValue>
    {
        public static readonly NoOpEtagHandler<TValue> Instance = new();

        public bool HasEtagValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context, out string etagValue)
        {
            etagValue = null;
            return false;
        }
        public void WriteEtagValue(TValue value, ODataJsonWriterStack state, ODataJsonWriterContext context)
        {
            // No operation for no-op handler
        }
    }
}

