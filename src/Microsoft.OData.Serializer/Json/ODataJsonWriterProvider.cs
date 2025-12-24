using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Json.Writers;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.OData.Serializer;

internal class ODataJsonWriterProvider<TCustomState>(ODataSerializerOptions<TCustomState> options) : IODataWriterProvider<ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>
{
    //private static readonly Type ObjectType = typeof(object);
    private static readonly ODataJsonBoolWriter<TCustomState> boolWriter = new();
    private static readonly ODataJsonNullableBoolWriter<TCustomState> nullableBoolWriter = new();
    private static readonly ODataJsonByteWriter<TCustomState> byteWriter = new();
    private static readonly ODataJsonNullableByteWriter<TCustomState> nullableByteWriter = new();
    private static readonly ODataJsonSByteWriter<TCustomState> sbyteWriter = new();
    private static readonly ODataJsonNullableSByteWriter<TCustomState> nullableSbyteWriter = new();
    private static readonly ODataJsonShortWriter<TCustomState> shortWriter = new();
    private static readonly ODataJsonNullableShortWriter<TCustomState> nullableShortWriter = new();
    private static readonly ODataJsonUshortWriter<TCustomState> ushortWriter = new();
    private static readonly ODataJsonNullableUshortWriter<TCustomState> nullableUshortWriter = new();
    private static readonly ODataJsonInt32Writer<TCustomState> int32Writer = new();
    private static readonly ODataJsonNullableInt32Writer<TCustomState> nullableInt32Writer = new();
    private static readonly ODataJsonUintWriter<TCustomState> uint32Writer = new();
    private static readonly ODataJsonNullableUintWriter<TCustomState> nullableUint32Writer = new();
    private static readonly ODataJsonInt64Writer<TCustomState> int64Writer = new();
    private static readonly ODataJsonNullableInt64Writer<TCustomState> nullableInt64Writer = new();
    private static readonly ODataJsonUlongWriter<TCustomState> uint64Writer = new();
    private static readonly ODataJsonNullableUlongWriter<TCustomState> nullableUint64Writer = new();
    private static readonly ODataJsonStringWriter<TCustomState> stringWriter = new();
    private static readonly ODataJsonGuidWriter<TCustomState> guidWriter = new();
    private static readonly ODataJsonNullableGuidWriter<TCustomState> nullableGuidWriter = new();
    private static readonly ODataJsonDateTimeWriter<TCustomState> dateTimeWriter = new();
    private static readonly ODataJsonNullableDateTimeWriter<TCustomState> nullableDateTimeWriter = new();
    private static readonly ODataJsonDateTimeOffsetWriter<TCustomState> dateTimeOffsetWriter = new();
    private static readonly ODataJsonNullableDateTimeOffsetWriter<TCustomState> nullableDateTimeOffsetWriter = new();
    private static readonly ODataJsonFloatWriter<TCustomState> floatWriter = new();
    private static readonly ODataJsonNullableFloatWriter<TCustomState> nullableFloatWriter = new();
    private static readonly ODataJsonDoubleWriter<TCustomState> doubleWriter = new();
    private static readonly ODataJsonNullableDoubleWriter<TCustomState> nullableDoubleWriter = new();
    private static readonly ODataJsonDecimalWriter<TCustomState> decimalWriter = new();
    private static readonly ODataJsonNullableDecimalWriter<TCustomState> nullableDecimalWriter = new();
    private static readonly ODataJsonByteArrayWriter<TCustomState> byteArrayWriter = new();
    private static readonly ODataJsonUriWriter<TCustomState> uriWriter = new();
    private static readonly ODataJsonObjectWriter<TCustomState> objectWriter = new();



    private static Dictionary<Type, IODataWriter<ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>> simpleWriters = InitPrimitiveWriters();
    private static List<ODataWriterFactory<TCustomState>> defaultFactories = InitDefaultFactories();

    private ConcurrentDictionary<Type, IODataWriter<ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>> writersCache = new();

    public IODataWriter<T, ODataWriterState<TCustomState>, ODataReaderState<TCustomState>> GetWriter<T>(IEdmModel? model)
    {
        //return (IODataWriter<T, ODataJsonWriterState>)writersCache.GetOrAdd(typeof(T), this.GetWriterNoCache<T>());
        // TODO: use GetOrAdd() instead, would require refactoring GetWriterNoCache
        // to be non-generic
        if (!writersCache.TryGetValue(typeof(T), out var writer))
        {
            writer = GetWriterNoCache<T>(model);
            writersCache.TryAdd(typeof(T), writer);
            return (IODataWriter<T, ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>)writer;
        }

        return (IODataWriter<T, ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>)writer;
    }

    public IODataWriter<ODataWriterState<TCustomState>, ODataReaderState<TCustomState>> GetWriter(Type type, IEdmModel? model)
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

        return (IODataWriter<ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>)generatedWriter;
    }

    private IODataWriter<T, ODataWriterState<TCustomState>, ODataReaderState<TCustomState>> GetWriterNoCache<T>(IEdmModel? model)
    {
        var type = typeof(T);

        // We currently don't support overriding the serialization of primitive types,
        // so we check the built-in simple writers first.
        if (simpleWriters.TryGetValue(type, out var writer))
        {
            return (IODataWriter<T, ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>)writer;
        }

        // Manually registered ODataTypeInfo have precedence over built-in writer factories.
        if (TryGetWriterFromRegisteredTypeInfo<T>(type, out var typeInfo, out IODataWriter<T, ODataWriterState<TCustomState>> odataTypeInfoWriter))
        {
            return odataTypeInfoWriter;
        }

        foreach (var factory in defaultFactories)
        {
            if (factory.CanWrite(type))
            {
                return (IODataWriter<T, ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>)factory.CreateWriter(type, options);
            }
        }

        if (TryCreateWriterFromType<T>(model, type, out IODataWriter<T, ODataWriterState<TCustomState>> createdWriter))
        {
            return createdWriter;
        }

        throw new Exception(typeInfo == null ?
            $"Could not find a suitable writer for {type.FullName}"
            : $"Unable to determine the OData value kind for type '{type.FullName}'. Set the GetValueKind property directly on the ODataTypeInfo to explicitly specify the value kind.");
    }

    private bool TryGetWriterFromRegisteredTypeInfo<T>(Type type, out ODataTypeInfo<T, TCustomState>? typeInfo, [NotNullWhen(true)] out IODataWriter<T, ODataWriterState<TCustomState>>? writer)
    {
        // type is same as T, but we want to avoid calleding typeof(T) each time.
        Debug.Assert(type == typeof(T));
        writer = null;
        typeInfo = options.TryGetResourceInfo<T>();
        if (typeInfo == null)
        {
            return false;
        }

        // the type info could represet a resource/entity writer, a collection writer, or both.
        // we use heuristics to determine the right kind unless the user specifies it explicitly

        if (typeInfo.GetValueKind != null)
        {
            writer = new ODataJsonHybridValueKindWriter<T, TCustomState>(typeInfo);
            return true;
        }

        var couldBeResource = typeInfo.Properties != null || typeInfo.PropertySelector != null || typeInfo.WriteProperties != null || typeInfo.GetOpenProperties != null;
        var couldBeCollection = typeInfo.ElementSelector != null;
        if (couldBeResource && !couldBeCollection)
        {
            // we have properties but no element selector, so it's a resource writer
            writer = new ODataResourceJsonWriter<T, TCustomState>(typeInfo);
            return true;
        }

        if (couldBeCollection && !couldBeResource)
        {
            writer = new ODataJsonResourceSetWithElementSelectorWriter<T, TCustomState>(typeInfo);
            return true;
        }


        return false;
    }

    private bool TryCreateWriterFromType<T>(IEdmModel? model, Type type, [NotNullWhen(true)] out IODataWriter<T, ODataWriterState<TCustomState>>? writer)
    {
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

        // type is same as T, but we want to avoid calleding typeof(T) each time.
        Debug.Assert(type == typeof(T));
        writer = null;

        var typeInfo = ODataTypeInfoFactory<TCustomState>.CreateTypeInfo<T>(model, options.TypeMapper);
        if (typeInfo != null)
        {
            writer = new ODataResourceJsonWriter<T, TCustomState>(typeInfo);
            return true;
        }

        return false;
    }

    private static Dictionary<Type, IODataWriter<ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>> InitPrimitiveWriters()
    {
        const int NumSimpleWriters = 34; // Update this when adding more writers. Keeps the dict size exact.
        Dictionary<Type, IODataWriter<ODataWriterState<TCustomState>, ODataReaderState<TCustomState>>> writers = new(NumSimpleWriters);

        Add(boolWriter);
        Add(nullableBoolWriter);
        Add(int32Writer);
        Add(nullableInt32Writer);
        Add(int64Writer);
        Add(nullableInt64Writer);
        Add(stringWriter);
        Add(guidWriter);
        Add(nullableGuidWriter);
        Add(dateTimeWriter);
        Add(nullableDateTimeWriter);
        Add(dateTimeOffsetWriter);
        Add(nullableDateTimeOffsetWriter);
        Add(decimalWriter);
        Add(nullableDecimalWriter);
        Add(byteWriter);
        Add(nullableByteWriter);
        Add(sbyteWriter);
        Add(nullableSbyteWriter);
        Add(shortWriter);
        Add(nullableShortWriter);
        Add(ushortWriter);
        Add(nullableUshortWriter);
        Add(uint32Writer);
        Add(nullableUint32Writer);
        Add(uint64Writer);
        Add(nullableUint64Writer);
        Add(floatWriter);
        Add(nullableFloatWriter);
        Add(doubleWriter);
        Add(nullableDoubleWriter);
        Add(byteArrayWriter);
        Add(uriWriter);
        Add(objectWriter);

        Debug.Assert(NumSimpleWriters == writers.Count);

        return writers;

        void Add(IODataWriter<ODataWriterState<TCustomState>, ODataReaderState<TCustomState>> writer)
        {
            writers.Add(writer.Type!, writer);
        }
    }

    private static List<ODataWriterFactory<TCustomState>> InitDefaultFactories()
    {
        return [
            new ODataJsonEnumWriterFactory<TCustomState>(),
            new ODataJsonNullableWriterFactory<TCustomState>(),
            new ODataJsonEnumerableWriterFactory<TCustomState>()
        ];
    }
}
