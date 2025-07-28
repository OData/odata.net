using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Core;
using Microsoft.OData.Serializer.V3.Json.Writers;
using System;
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


    internal static Dictionary<Type, IODataWriter> simpleWriters = InitPrimitiveWriters();

    public IODataWriter<T, ODataJsonWriterState> GetWriter<T>()
    {
        // TODO: should cache result
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

        throw new Exception($"Could not find a suitable writer for {type.FullName}");
    }

    private static Dictionary<Type, IODataWriter> InitPrimitiveWriters()
    {
        const int NumSimpleWriters = 3; // Update this when adding more writers. Keeps the dict size exact.
        Dictionary<Type, IODataWriter> writers = new(NumSimpleWriters);

        Add(boolWriter);
        Add(int32Writer);
        Add(stringWriter);

        Debug.Assert(NumSimpleWriters <= writers.Count);

        return writers;

        void Add(IODataWriter writer)
        {
            writers.Add(writer.Type!, writer);
        }
    }
}
