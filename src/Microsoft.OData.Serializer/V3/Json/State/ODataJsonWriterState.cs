using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

public sealed class ODataJsonWriterState<TCustomState>
{
    private ODataSerializerOptions<TCustomState> options;
    private ODataJsonWriterProvider<TCustomState> writers;

    public ODataUri ODataUri { get; set; }

    public ODataPayloadKind PayloadKind { get; set; }
    public ODataMetadataLevel MetadataLevel { get; set; } = ODataMetadataLevel.Minimal;

    internal WriteStack<TCustomState> Stack { get; } = new WriteStack<TCustomState>();

    internal ODataJsonWriterState(ODataSerializerOptions<TCustomState> options, ODataJsonWriterProvider<TCustomState> writers, Utf8JsonWriter jsonWriter)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.writers = writers ?? throw new ArgumentNullException(nameof(writers));
        this.JsonWriter = jsonWriter;
    }

    internal ODataSerializerOptions<TCustomState> Options => options;
    internal Utf8JsonWriter JsonWriter { get; init; }

    internal bool ShouldFlush()
    {
        return JsonWriter.BytesPending > 0.9 * this.Options.BufferSize;
    }

    internal async ValueTask FlushIfNecessaryAsync()
    {
        if (ShouldFlush())
        {
            await JsonWriter.FlushAsync().ConfigureAwait(false);
        }
    }

    public bool IsTopLevel()
    {
        return this.Stack.IsTopLevel();
    }

    // TODO: Make this internal and instead of expose writer interfaces to public hooks
    // so that we can still the flow of how and where things are written.
    internal ValueTask WriteValue<T>(T value)
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

    public ref TCustomState CurrentCustomState()
    {
        return ref this.Stack.CurrentCustomState;
    }

    internal ICustomAnnotationsHandler<TCustomState> GetCustomAnnotationsHandler(Type annotationsType)
    {
        var handler = options.CustomAnnotationsHandlerResolver.Resolve(annotationsType)
            ?? throw new InvalidOperationException($"No custom annotations handler found for type {annotationsType.FullName}");

        return handler;
    }

    internal IDynamicPropertiesHandler<TCustomState> GetDynamicPropertiesHandler(Type dynamicPropertiesType)
    {
        var handler = options.DynamicPropertiesHandlerResolver.Resolve(dynamicPropertiesType)
            ?? throw new InvalidOperationException($"No dynamic properties handler found for type {dynamicPropertiesType.FullName}");
        return handler;
    }
}
