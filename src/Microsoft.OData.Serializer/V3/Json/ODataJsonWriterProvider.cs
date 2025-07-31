using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Core;
using Microsoft.OData.Serializer.V3.Json.Writers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

internal class ODataJsonWriterProvider<TCustomState>(ODataSerializerOptions<TCustomState> options) : IODataWriterProvider<ODataJsonWriterState<TCustomState>>
{
    private static readonly ODataJsonBoolWriter<TCustomState> boolWriter = new();
    private static readonly ODataJsonInt32Writer<TCustomState> int32Writer = new();
    private static readonly ODataJsonStringWriter<TCustomState> stringWriter = new();
    private static readonly ODataJsonDateTimeWriter<TCustomState> dateTimeWriter = new();
    private static readonly ODataJsonDecimalWriter<TCustomState> decimalWriter = new();
    private static readonly ODataJsonByteArrayWriter<TCustomState> byteArrayWriter = new();
    private static readonly ODataJsonUriWriter<TCustomState> uriWriter = new();


    private static Dictionary<Type, IODataWriter> simpleWriters = InitPrimitiveWriters();
    private static List<ODataWriterFactory<TCustomState>> defaultFactories = InitDefaultFactories();

    private ConcurrentDictionary<Type, IODataWriter> writersCache = new();

    public IODataWriter<T, ODataJsonWriterState<TCustomState>> GetWriter<T>()
    {
        //return (IODataWriter<T, ODataJsonWriterState>)writersCache.GetOrAdd(typeof(T), this.GetWriterNoCache<T>());
        // TODO: use GetOrAdd() instead, would require refactoring GetWriterNoCache
        // to be non-generic
        if (!writersCache.TryGetValue(typeof(T), out var writer))
        {
            writer = this.GetWriterNoCache<T>();
            writersCache.TryAdd(typeof(T), writer);
            return (IODataWriter<T, ODataJsonWriterState<TCustomState>>)writer;
        }

        return (IODataWriter<T, ODataJsonWriterState<TCustomState>>)writer;
    }

    private IODataWriter<T, ODataJsonWriterState<TCustomState>> GetWriterNoCache<T>()
    {
        var type = typeof(T);
        if (simpleWriters.TryGetValue(type, out var writer))
        {
            return (IODataWriter<T, ODataJsonWriterState<TCustomState>>)writer;
        }

        foreach (var factory in defaultFactories)
        {
            if (factory.CanWrite(type))
            {
                return (IODataWriter<T, ODataJsonWriterState<TCustomState>>)factory.CreateWriter(type, options);
            }
        }

        // TODO: we could consider generating a custom writer for the poco
        // but we'd need to map it to an OData type.
        ODataResourceTypeInfo<T, TCustomState>? typeInfo = options.TryGetResourceInfo<T>();
        if (typeInfo != null)
        {
            return new ODataResourceJsonWriter<T, TCustomState>(typeInfo);
        }

        throw new Exception($"Could not find a suitable writer for {type.FullName}");
    }

    private static Dictionary<Type, IODataWriter> InitPrimitiveWriters()
    {
        const int NumSimpleWriters = 6; // Update this when adding more writers. Keeps the dict size exact.
        Dictionary<Type, IODataWriter> writers = new(NumSimpleWriters);

        Add(boolWriter);
        Add(int32Writer);
        Add(stringWriter);
        Add(dateTimeWriter);
        Add(decimalWriter);
        Add(byteArrayWriter);
        Add(uriWriter);

        Debug.Assert(NumSimpleWriters <= writers.Count);

        return writers;

        void Add(IODataWriter writer)
        {
            writers.Add(writer.Type!, writer);
        }
    }

    private static List<ODataWriterFactory<TCustomState>> InitDefaultFactories()
    {
        return [
            new ODataJsonEnumWriterFactory<TCustomState>(),
            new ODataJsonEnumerableWriterFactory<TCustomState>()
        ];
    }
}
