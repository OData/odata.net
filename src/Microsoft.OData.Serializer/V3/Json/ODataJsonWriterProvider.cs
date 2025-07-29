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

internal class ODataJsonWriterProvider(ODataSerializerOptions options) : IODataWriterProvider<ODataJsonWriterState>
{
    private static readonly ODataJsonBoolWriter boolWriter = new();
    private static readonly ODataJsonInt32Writer int32Writer = new();
    private static readonly ODataJsonStringWriter stringWriter = new();
    private static readonly ODataJsonDateTimeWriter dateTimeWriter = new();
    private static readonly ODataJsonDecimalWriter decimalWriter = new();


    private static Dictionary<Type, IODataWriter> simpleWriters = InitPrimitiveWriters();
    private static List<ODataWriterFactory> defaultFactories = InitDefaultFactories();

    private ConcurrentDictionary<Type, IODataWriter> writersCache = new();

    public IODataWriter<T, ODataJsonWriterState> GetWriter<T>()
    {
        return (IODataWriter<T, ODataJsonWriterState>)writersCache.GetOrAdd(typeof(T), this.GetWriterNoCache<T>());
    }

    private IODataWriter<T, ODataJsonWriterState> GetWriterNoCache<T>()
    {
        var type = typeof(T);
        if (simpleWriters.TryGetValue(type, out var writer))
        {
            return (IODataWriter<T, ODataJsonWriterState>)writer;
        }


        ODataResourceTypeInfo<T>? typeInfo = options.TryGetResourceInfo<T>();
        if (typeInfo != null)
        {
            return new ODataResourceJsonWriter<T>(typeInfo);
        }

        foreach (var factory in defaultFactories)
        {
            if (factory.CanWrite(type))
            {
                return (IODataWriter<T, ODataJsonWriterState>)factory.CreateWriter(type);
            }
        }

        throw new Exception($"Could not find a suitable writer for {type.FullName}");
    }

    private static Dictionary<Type, IODataWriter> InitPrimitiveWriters()
    {
        const int NumSimpleWriters = 3; // Update this when adding more writers. Keeps the dict size exact.
        Dictionary<Type, IODataWriter> writers = new(NumSimpleWriters);

        Add(boolWriter);
        Add(int32Writer);
        Add(stringWriter);
        Add(dateTimeWriter);
        Add(decimalWriter);

        Debug.Assert(NumSimpleWriters <= writers.Count);

        return writers;

        void Add(IODataWriter writer)
        {
            writers.Add(writer.Type!, writer);
        }
    }

    private static List<ODataWriterFactory> InitDefaultFactories()
    {
        return [
            new ODataJsonEnumWriterFactory(),
            new ODataJsonEnumerableWriterFactory()
        ];
    }
}
