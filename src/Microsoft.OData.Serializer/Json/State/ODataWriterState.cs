using Microsoft.OData.Edm;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Microsoft.OData.Serializer;

public sealed class ODataWriterState<TCustomState>
{
    private ODataSerializerOptions<TCustomState> options;
    private ODataJsonWriterProvider<TCustomState> writers;
    private TCustomState customState;

    public ODataUri ODataUri { get; set; }
    public IEdmModel EdmModel { get; set; } // Should this be set on the state or options?

    public ODataPayloadKind PayloadKind { get; set; }
    public ODataMetadataLevel MetadataLevel { get; set; } = ODataMetadataLevel.Minimal;

    internal WriteStack<TCustomState> Stack { get; } = new WriteStack<TCustomState>();

    internal ODataWriterState(ODataSerializerOptions<TCustomState> options, ODataJsonWriterProvider<TCustomState> writers, Utf8JsonWriter jsonWriter)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.writers = writers ?? throw new ArgumentNullException(nameof(writers));
        JsonWriter = jsonWriter;
    }

    internal ODataSerializerOptions<TCustomState> Options => options;
    internal Utf8JsonWriter JsonWriter { get; init; }

    internal PooledByteBufferWriter BufferWriter { get; init; }

    internal Stream OutputStream { get; init; }

    internal JavaScriptEncoder JavaScriptEncoder { get; init; }

    internal HashSet<object>? _disposableObjects;

    // Stores both the length of trailing bytes and whether we're in a segmented value scope
    private int trailingSegmentedBytesLength;
    internal InlineByteBuffer3 _partialTrailingBytesBuffer;

    internal Span<byte> PartialTrailingBytesBuffer => _partialTrailingBytesBuffer;
    internal Span<char> PartialTrailingCharsBuffer => MemoryMarshal.Cast<byte, char>(_partialTrailingBytesBuffer);

    public ref TCustomState CustomState
    {
        get => ref customState;
    }

    /// <summary>
    /// If we're not already in a base64 segment scope, start one. Returns true if this call started a new scope, false otherwise.
    /// </summary>
    /// <returns>Return true if this call has started a new scope or false if already in the middle of an ongoing scope.</returns>
    internal bool TryStartSegmentedValueScope()
    {
        // if msb od trailingBase64BytesLength is 1, keep it as 1, if it's 0, set it to 1 and return whether it was 0
        if (trailingSegmentedBytesLength < 0)
        {
            return false;
        }

        trailingSegmentedBytesLength = -1;
        return true;
    }

    internal void EndSegmentedValueScope()
    {
        trailingSegmentedBytesLength = 0;
    }

    internal void SetTrailingBase64Bytes(ReadOnlySpan<byte> bytes)
    {
        Debug.Assert(bytes.Length < 3, "Base64 encoding should not leave 3 or more trailing bytes.");
        bytes.CopyTo(PartialTrailingBytesBuffer);

        trailingSegmentedBytesLength = -1 - bytes.Length;
    }

    internal void SetTrailingChars(ReadOnlySpan<char> chars)
    {
        Debug.Assert(chars.Length < 2, "String escaping should not leave 2 or more trailing chars.");
        chars.CopyTo(PartialTrailingCharsBuffer);

        trailingSegmentedBytesLength = -1 - chars.Length;
    }

    internal void ClearTrailingPartialData()
    {
        trailingSegmentedBytesLength = -1;
    }

    internal int GetTrailingPartialDataLength()
    {
        //return trailingBase64BytesLength & ~(1 << 31);
        return -(trailingSegmentedBytesLength + 1);
    }

    internal void SetCustomSate(in TCustomState state)
    {
        customState = state;
    }

    internal bool ShouldFlush()
    {
        return JsonWriter.BytesPending + BufferWriter.WrittenCount > 0.9 * BufferWriter.Capacity;
    }

    internal async ValueTask FlushAsync()
    {
        JsonWriter.Flush();
        //await bufferWriter.WriteToStreamAsync(stream, cancellationToken: default).ConfigureAwait(false);
        await OutputStream.WriteAsync(BufferWriter.WrittenMemory, cancellationToken: default).ConfigureAwait(false);
        BufferWriter.Clear();
    }

    internal int UsedBufferSize => JsonWriter.BytesPending + BufferWriter.WrittenCount;

    internal int FreeBufferCapacity => BufferWriter.Capacity - UsedBufferSize;

    public HashSet<object>? DisposableObjects => _disposableObjects;

    public bool IsTopLevel()
    {
        return Stack.IsTopLevel();
    }

    internal IODataWriter<ODataWriterState<TCustomState>> GetWriter(Type type)
    {
        return writers.GetWriter(type, EdmModel);
    }

    // TODO: Make this internal and instead of exposing writer interfaces to public hooks
    // so that we can still control the flow of how and where things are written.
    internal bool WriteValue<T>(T value)
    {
        var writer = writers.GetWriter<T>(EdmModel);
        return writer.Write(value, this);
    }

    public ODataPropertyInfo? CurrentPropertyInfo()
    {
        return Stack.Current.PropertyInfo;
    }


    public ODataPropertyInfo? ParentPropertyInfo()
    {
        if (IsTopLevel())
        {
            return null;
        }

        return Stack.Parent.PropertyInfo;
    }

    internal ICustomAnnotationsHandler<TCustomState> GetCustomAnnotationsHandler(Type annotationsType)
    {
        var handler = options.CustomAnnotationsHandlerResolver.Resolve(annotationsType)
            ?? throw new InvalidOperationException($"No custom annotations handler found for type {annotationsType.FullName}");

        return handler;
    }

    internal IOpenPropertiesHandler<TCustomState> GetOpenPropertiesHandler(Type dynamicPropertiesType)
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

    [InlineArray(3)]
    internal struct InlineByteBuffer3
    {
        byte _byte;
    }
}
