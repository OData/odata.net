using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

public sealed class ODataJsonWriterState
{
    private ODataSerializerOptions options;
    private ODataJsonWriterProvider writers;

    public ODataUri ODataUri { get; set; }

    public ODataPayloadKind PayloadKind { get; set; }
    public ODataMetadataLevel MetadataLevel { get; set; } = ODataMetadataLevel.Minimal;

    internal WriteStack Stack { get; } = new WriteStack();

    internal ODataJsonWriterState(ODataSerializerOptions options, ODataJsonWriterProvider writers, Utf8JsonWriter jsonWriter)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.writers = writers ?? throw new ArgumentNullException(nameof(writers));
        this.JsonWriter = jsonWriter;
    }

    internal ODataSerializerOptions Options => options;
    internal Utf8JsonWriter JsonWriter { get; init; }

    internal bool ShouldFlush()
    {
        return JsonWriter.BytesPending > 0.9 * this.Options.BufferSize;
    }

    public bool IsTopLevel()
    {
        return this.Stack.IsTopLevel();
    }

    public ValueTask WriteValue<T>(T value)
    {
        var writer = writers.GetWriter<T>();
        return writer.Write(value, this);
    }

    public Adapters.ODataPropertyInfo? CurrentPropertyInfo()
    {
        return this.Stack.Current.PropertyInfo;
    }

    public Adapters.ODataPropertyInfo? ParentPropertyInfo()
    {
        if (this.IsTopLevel())
        {
            return null;
        }

        return this.Stack.Parent.PropertyInfo;
    }
}
