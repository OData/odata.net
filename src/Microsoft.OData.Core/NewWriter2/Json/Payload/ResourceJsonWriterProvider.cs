using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class ResourceJsonWriterProvider : IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
{
    private readonly ConcurrentDictionary<Type, object> _writers = new();

    public void AddValueWriter<T>(IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T> writer)
    {
        var type = typeof(T);
       _writers[type] = writer ?? throw new ArgumentNullException(nameof(writer), "Writer cannot be null.");
    }

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

        if (type == typeof(int))
        {
            var writer = new IntJsonwriter();
            _writers[type] = writer;
            return (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)writer;
        }

        if (type == typeof(bool))
        {
            var writer = new BoolJsonWriter();
            _writers[type] = writer;
            return (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)writer;
        }

        if (type == typeof(string))
        {
            var writer = new StringJsonWriter();
            _writers[type] = writer;
            return (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)writer;
        }

        if (type == typeof(decimal))
        {
            var writer = new DecimalJsonWriter();
            _writers[type] = writer;
            return (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)writer;
        }

        if (type == typeof(byte[]))
        {    
            var writer = new ByteArrayJsonWriter();
            _writers[type] = writer;
            return (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)writer;
        }

        if (type.IsEnum)
        {
            var writerType = typeof(EnumJsonWriter<>).MakeGenericType(type);
            var writer = (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)Activator.CreateInstance(writerType);
            _writers[type] = writer;
            return writer;
        }

        if (type == typeof(DateTime))
        {
            var writer = new DateTimeJsonWriter();
            _writers[type] = writer;
            return (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)writer;
        }

        if (type == typeof(DateTimeOffset))
        {
            var writer = new DateTimeOffsetJsonWriter();
            _writers[type] = writer;
            return (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)writer;
        }

        var pocoResourceWriter = new ODataResourceHandlerBasedJsonWriter<TValue>();
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
