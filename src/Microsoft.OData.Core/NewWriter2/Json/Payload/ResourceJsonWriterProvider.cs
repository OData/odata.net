using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class ResourceJsonWriterProvider : IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    private readonly ConcurrentDictionary<Type, object> _writers = new();
    public IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue> GetResourceWriter<TValue>(ODataJsonWriterContext context, ODataJsonWriterStack state)
    {
        if (_writers.TryGetValue(typeof(TValue), out var writerObj))
        {
            return (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)writerObj;
        }

        var type = typeof(TValue);
        if (TryGetEnumerableOfT(type, out Type enumerableOfT, out Type elementType))
        {
            var writerType = typeof(EnumerableResourceSetJsonWriter<,>).MakeGenericType(type, elementType);
            var writer = (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)Activator.CreateInstance(writerType);
            _writers[type] = writer;
            return writer;
        }

        var pocoResourceWriter = new PocoResourceJsonWriter<TValue>();
        _writers[type] = pocoResourceWriter;
        return pocoResourceWriter;
    }

    private static bool TryGetEnumerableOfT(Type type, out Type enumerableOfT, out Type elementType)
    {
        enumerableOfT = null;
        elementType = null;
        if (!type.IsGenericType)
        {
            return false;
        }

        if (type.GenericTypeArguments.Length != 1)
        {
            return false;
        }

        elementType = type.GenericTypeArguments[0];

        enumerableOfT = typeof(IEnumerable<>).MakeGenericType([elementType]);
        return type.IsAssignableTo(enumerableOfT);
    }
}
