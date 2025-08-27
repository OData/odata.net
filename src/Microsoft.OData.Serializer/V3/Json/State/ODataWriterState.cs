using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

public sealed class ODataWriterState<TCustomState>
{
    private ODataSerializerOptions<TCustomState> options;
    private ODataJsonWriterProvider<TCustomState> writers;
    private TCustomState customState;

    public ODataUri ODataUri { get; set; }

    public ODataPayloadKind PayloadKind { get; set; }
    public ODataMetadataLevel MetadataLevel { get; set; } = ODataMetadataLevel.Minimal;

    internal WriteStack<TCustomState> Stack { get; } = new WriteStack<TCustomState>();

    internal ODataWriterState(ODataSerializerOptions<TCustomState> options, ODataJsonWriterProvider<TCustomState> writers, Utf8JsonWriter jsonWriter)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.writers = writers ?? throw new ArgumentNullException(nameof(writers));
        this.JsonWriter = jsonWriter;
    }

    internal ODataSerializerOptions<TCustomState> Options => options;
    internal Utf8JsonWriter JsonWriter { get; init; }

    internal PooledByteBufferWriter BufferWriter { get; init; }

    internal Stream OutputStream { get; init; }

    internal JavaScriptEncoder JavaScriptEncoder { get; init; }

    internal HashSet<object>? _disposableObjects;

    // Most-significant bit tells us if we're in the middle of writing base64 segments
    // Remainder is the length of the trailing bytes
    private int trailingBase64BytesLength;
    internal TrailingBase64BytesBuffer trailingBase64BytesBuffer;

    public ref TCustomState CustomState
    {
        get => ref customState;
    }

    /// <summary>
    /// If we're not already in a base64 segment scope, start one. Returns true if this call started a new scope, false otherwise.
    /// </summary>
    /// <returns>Return true if this call has started a new scope or false if already in the middle of an ongoing scope.</returns>
    internal bool TryStartBase64SegmentScope()
    {
        // if msb od trailingBase64BytesLength is 1, keep it as 1, if it's 0, set it to 1 and return whether it was 0
        if (trailingBase64BytesLength < 0)
        {
            return false;
        }

        trailingBase64BytesLength = -1;
        return true;
    }

    internal void EndBase64SegmentScope()
    {
        trailingBase64BytesLength = 0;
    }

    internal void SetTrailingBase64Bytes(ReadOnlySpan<byte> bytes)
    {
        Debug.Assert(bytes.Length < 3, "Base64 encoding should not leave 3 or more trailing bytes.");
        bytes.CopyTo(trailingBase64BytesBuffer);

        // Overwrite the current length while preserving the msb
        trailingBase64BytesLength = -1 - bytes.Length;// (trailingBase64BytesLength & (1 << 31)) | bytes.Length;
    }

    internal void ClearTrailingBase64Bytes()
    {
        trailingBase64BytesLength = -1;
    }

    internal Span<byte> GetTrailingBase64BytesBuffer()
    {
        return trailingBase64BytesBuffer[..trailingBase64BytesLength];
    }

    internal int GetTrailingBase64BytesLength()
    {
        //return trailingBase64BytesLength & ~(1 << 31);
        return -(trailingBase64BytesLength + 1);
    }

    internal void SetCustomSate(in TCustomState state)
    {
        customState = state;
    }

    internal bool ShouldFlush()
    {
        return JsonWriter.BytesPending + BufferWriter.WrittenCount > 0.9 * this.BufferWriter.Capacity;
    }

    internal async ValueTask FlushAsync()
    {
        JsonWriter.Flush();
        //await bufferWriter.WriteToStreamAsync(stream, cancellationToken: default).ConfigureAwait(false);
        await OutputStream.WriteAsync(BufferWriter.WrittenMemory, cancellationToken: default).ConfigureAwait(false);
        BufferWriter.Clear();
    }

    internal int UsedBufferSize => JsonWriter.BytesPending + BufferWriter.WrittenCount;

    internal int FreeBufferCapacity => this.BufferWriter.Capacity - this.UsedBufferSize;

    public HashSet<object>? DisposableObjects => _disposableObjects;

    public bool IsTopLevel()
    {
        return this.Stack.IsTopLevel();
    }

    // TODO: Make this internal and instead of expose writer interfaces to public hooks
    // so that we can still the flow of how and where things are written.
    internal bool WriteValue<T>(T value)
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

    public void RegisterForDispose(object obj)
    {
        if (_disposableObjects == null)
        {
            _disposableObjects = new HashSet<object>();
        }

        _disposableObjects.Add(obj);
    }
}
