using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Microsoft.OData.Serializer;

public static class ODataSerializer
{
    private static readonly JsonWriterOptions DefaultJsonWriterOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        SkipValidation = true, // Important because we bypass the writer in some cases
    };

    public static ValueTask WriteAsync<T>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions options)
    {
        return WriteAsync<T, DefaultState>(value, stream, uri, model, options);
    }

    public static ValueTask WriteAsync<T, TCustomState>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions<TCustomState> options)
    {
        return WriteAsync(value, stream, uri, model, options, default);
    }

    public static async ValueTask WriteAsync<T, TCustomState>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions<TCustomState> options, TCustomState customState)
    {
        // this is rough structure of what we expect the writer to do
        // based on payload kind, determine the appropriate state, context and underlying writer.

        // init state
        using var bufferWriter = new PooledByteBufferWriter(options.BufferSize);
        //using var bufferWriter = new PooledByteSegmentBufferWriter(options.BufferSize);
        var jsonWriter = new Utf8JsonWriter(bufferWriter, DefaultJsonWriterOptions); // TODO: make this configurable in options
        var writerProvider = new ODataJsonWriterProvider<TCustomState>(options);
        var payloadKind = GetPayloadKindFromUri(uri);
        var state = new ODataWriterState<TCustomState>(options, writerProvider, jsonWriter, bufferWriter)
        {
            ODataUri = uri,
            EdmModel = model,
            PayloadKind = payloadKind,
            JavaScriptEncoder = DefaultJsonWriterOptions.Encoder,
            OutputStream = stream,
        };

        state.SetCustomSate(in customState);
        // get writer
        var writer = writerProvider.GetWriter<T>(model); // should we pass the value as well?

        try
        {

            bool isDone = false;
            while (!isDone)
            {
                isDone = writer.Write(value, state);


                if (state.ShouldFlush())
                {
                    jsonWriter.Flush();
                    //await bufferWriter.WriteToStreamAsync(stream, cancellationToken: default).ConfigureAwait(false);
                    await stream.WriteAsync(bufferWriter.WrittenMemory, cancellationToken: default).ConfigureAwait(false);
                    bufferWriter.Clear();
                }

                if (state.Stack.HasSuspendedFrames() && state.Stack.LastSuspendedFrame.PendingTask.HasValue)
                {
                    await state.Stack.LastSuspendedFrame.PendingTask.Value;
                }

                // We might need to dispose the pipe reader, where should we do that? Here?
                // How do we know we need to dispose?
            }

            if (jsonWriter.BytesPending > 0)
            {
                // write any remaining data
                jsonWriter.Flush();
                //await bufferWriter.WriteToStreamAsync(stream, cancellationToken: default).ConfigureAwait(false);
                await stream.WriteAsync(bufferWriter.WrittenMemory, cancellationToken: default).ConfigureAwait(false);
            }

            //bufferWriter.Dispose();
            await stream.FlushAsync().ConfigureAwait(false);
        }
        finally
        {
            if (state.DisposableObjects != null)
            {
                foreach (var resource in state.DisposableObjects)
                {
                    if (resource is IAsyncDisposable asyncDisposable)
                    {
                        await asyncDisposable.DisposeAsync();
                    }
                }
            }
        }



        // TODO: brainstorming re-entrancy support
        //bool isDone = false;
        //do
        //{
        //    // write value
        //    isDone = writer.Write(value, state);
        //    // might also need to fetch more data from value (e.g. if source is a stream or IAsyncEnumerable)

        //    if (!isDone)
        //    {
        //        if (typeof(IAsyncSource).IsAssignableFrom(typeof(T));
        //        {
        //            IAsyncSource source = (IAsyncSource)value;
        //            if (source.NeedsMoreData)
        //            {
        //                await source.AdvanceAsync();
        //            }
        //        }

        //        if (state.ShouldFlush())
        //        {
        //            // if not done, we might need to flush the stream or do some async operation
        //            // to continue writing later.
        //            await stream.FlushAsync();
        //        } 
        //    }
        //} while (!isDone);
    }

    private static ODataPayloadKind GetPayloadKindFromUri(ODataUri uri)
    {
        var lastSegment = uri.Path.LastSegment;
        return lastSegment switch
        {
            EntitySetSegment => ODataPayloadKind.ResourceSet,
            NavigationPropertySegment navSeg =>
                navSeg.EdmType.TypeKind == EdmTypeKind.Collection ? ODataPayloadKind.ResourceSet : ODataPayloadKind.Resource,
            KeySegment => ODataPayloadKind.Resource,
            SingletonSegment => ODataPayloadKind.Resource,
            _ => ODataPayloadKind.Unsupported
        };
    }
}
