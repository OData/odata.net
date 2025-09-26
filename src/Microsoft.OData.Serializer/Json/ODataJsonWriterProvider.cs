using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Adapters;
using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json.State;
using Microsoft.OData.Serializer.Json.Writers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

internal class ODataJsonWriterProvider<TCustomState>(ODataSerializerOptions<TCustomState> options) : IODataWriterProvider<ODataWriterState<TCustomState>>
{
    //private static readonly Type ObjectType = typeof(object);
    private static readonly ODataJsonBoolWriter<TCustomState> boolWriter = new();
    private static readonly ODataJsonInt32Writer<TCustomState> int32Writer = new();
    private static readonly ODataJsonStringWriter<TCustomState> stringWriter = new();
    private static readonly ODataJsonDateTimeWriter<TCustomState> dateTimeWriter = new();
    private static readonly ODataJsonDateTimeOffsetWriter<TCustomState> dateTimeOffsetWriter = new();
    private static readonly ODataJsonDoubleWriter<TCustomState> doubleWriter = new();
    private static readonly ODataJsonDecimalWriter<TCustomState> decimalWriter = new();
    private static readonly ODataJsonByteArrayWriter<TCustomState> byteArrayWriter = new();
    private static readonly ODataJsonUriWriter<TCustomState> uriWriter = new();
    private static readonly ODataJsonPipeReaderBinaryWriter<TCustomState> pipeReaderWriter = new();
    private static readonly ODataJsonBufferedReaderBinaryWriter<TCustomState> bufferedReaderBinaryWriter = new();
    private static readonly ODataJsonObjectWriter<TCustomState> objectWriter = new();



    private static Dictionary<Type, IODataWriter<ODataWriterState<TCustomState>>> simpleWriters = InitPrimitiveWriters();
    private static List<ODataWriterFactory<TCustomState>> defaultFactories = InitDefaultFactories();

    private ConcurrentDictionary<Type, IODataWriter<ODataWriterState<TCustomState>>> writersCache = new();

    public IODataWriter<T, ODataWriterState<TCustomState>> GetWriter<T>(IEdmModel? model)
    {
        //return (IODataWriter<T, ODataJsonWriterState>)writersCache.GetOrAdd(typeof(T), this.GetWriterNoCache<T>());
        // TODO: use GetOrAdd() instead, would require refactoring GetWriterNoCache
        // to be non-generic
        if (!writersCache.TryGetValue(typeof(T), out var writer))
        {
            writer = GetWriterNoCache<T>(model);
            writersCache.TryAdd(typeof(T), writer);
            return (IODataWriter<T, ODataWriterState<TCustomState>>)writer;
        }

        return (IODataWriter<T, ODataWriterState<TCustomState>>)writer;
    }

    public IODataWriter<ODataWriterState<TCustomState>> GetWriter(Type type, IEdmModel? model)
    {
        if (type == typeof(object))
        {
            return GetWriter<object>(model);
        }

        if (writersCache.TryGetValue(type, out var writer))
        {
            return writer;
        }

        var getWriterMethod = GetType()
            .GetMethod(
                nameof(GetWriter),
                genericParameterCount: 1,
                [typeof(IEdmModel)])
            ?.MakeGenericMethod([type]);

        Debug.Assert(getWriterMethod != null);

        var generatedWriter = getWriterMethod.Invoke(this, [model]);
        Debug.Assert(generatedWriter != null);

        return (IODataWriter<ODataWriterState<TCustomState>>)generatedWriter;
    }

    private IODataWriter<T, ODataWriterState<TCustomState>> GetWriterNoCache<T>(IEdmModel? model)
    {
        var type = typeof(T);
        if (simpleWriters.TryGetValue(type, out var writer))
        {
            return (IODataWriter<T, ODataWriterState<TCustomState>>)writer;
        }

        foreach (var factory in defaultFactories)
        {
            if (factory.CanWrite(type))
            {
                return (IODataWriter<T, ODataWriterState<TCustomState>>)factory.CreateWriter(type, options);
            }
        }

        ODataTypeInfo<T, TCustomState>? typeInfo = options.TryGetResourceInfo<T>();
        if (typeInfo != null)
        {
            return new ODataResourceJsonWriter<T, TCustomState>(typeInfo);
        }

        // TODO: automatic generation of type infos should not be tightly coupled to
        // the core writer, it should be a pluggable extension on top of the core writer.
        // However, currently it depends on internal details of ODataTypeInfo and ODataPropertyInfo,
        // e.g. the internal ODataPropertyInfo<,>.WriteValueWithCustomWriterAsync property.
        // To promote an extensible design, we should either refactor the implementation such
        // that these internal properties are not needed, or not stored in the core serializer components
        // like ODataTypeInfo/ODataPropertyInfo, or it we should consider whether it makes sense
        // to make them public such that extensions we write do not have special access than
        // extensions written by library consumers.
        // Attempts to generate a type info on the fly.
        // TODO: This currently only works with POCOs.
        typeInfo = ODataTypeInfoFactory<TCustomState>.CreateTypeInfo<T>(model, options.TypeMapper);
        if (typeInfo != null)
        {
            return new ODataResourceJsonWriter<T, TCustomState>(typeInfo);
        }

        throw new Exception($"Could not find a suitable writer for {type.FullName}");
    }

    private static Dictionary<Type, IODataWriter<ODataWriterState<TCustomState>>> InitPrimitiveWriters()
    {
        const int NumSimpleWriters = 12; // Update this when adding more writers. Keeps the dict size exact.
        Dictionary<Type, IODataWriter<ODataWriterState<TCustomState>>> writers = new(NumSimpleWriters);

        Add(boolWriter);
        Add(int32Writer);
        Add(stringWriter);
        Add(dateTimeWriter);
        Add(dateTimeOffsetWriter);
        Add(doubleWriter);
        Add(decimalWriter);
        Add(byteArrayWriter);
        Add(uriWriter);
        Add(pipeReaderWriter);
        Add(bufferedReaderBinaryWriter);
        Add(objectWriter);

        Debug.Assert(NumSimpleWriters == writers.Count);

        return writers;

        void Add(IODataWriter<ODataWriterState<TCustomState>> writer)
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
