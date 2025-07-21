using Microsoft.OData.Serializer.Core;
using System.Collections.Concurrent;

namespace Microsoft.OData.Serializer.Json;

public class ResourceJsonWriterProvider : IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
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

        if (TryGetCollectionOfT(type, typeof(IReadOnlyList<>), out Type listOfT, out Type elementType))
        {
            var writerType = typeof(ListResourceSetJsonWriter<,>).MakeGenericType(type, elementType);
            var writer = (IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue>)Activator.CreateInstance(writerType);
            _writers[type] = writer;
            return writer;
        }

        if (TryGetEnumerableOfT(type, out Type enumerableOfT, out Type enumerableELementType))
        {
            var writerType = typeof(EnumerableResourceSetJsonWriter<,>).MakeGenericType(type, enumerableELementType);
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
        return TryGetCollectionOfT(type, typeof(IEnumerable<>), out enumerableOfT, out elementType);
    }

    private static bool TryGetCollectionOfT(Type sourceType, Type collectionType, out Type collectionOfT, out Type elementType)
    {
        collectionOfT = null;
        elementType = null;
        if (!sourceType.IsGenericType)
        {
            return false;
        }

        if (sourceType.GenericTypeArguments.Length != 1)
        {
            return false;
        }

        elementType = sourceType.GenericTypeArguments[0];

        collectionOfT = collectionType.MakeGenericType([elementType]);
        return sourceType.IsAssignableTo(collectionOfT);
    }
}
