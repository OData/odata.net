using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3.Json;
using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3;

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

    public static async ValueTask WriteAsync<T, TCustomState>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions<TCustomState> options)
    {
        // this is rough structure of what we expect the writer to do
        // based on payload kind, determine the appropirate state, context and underlying writer.

        // init state
        using var bufferWriter = new PooledByteBufferWriter(options.BufferSize);
        //using var bufferWriter = new PooledByteSegmentBufferWriter(options.BufferSize);
        var jsonWriter = new Utf8JsonWriter(bufferWriter, DefaultJsonWriterOptions); // TODO: make this configurable in options
        var writerProvider = new ODataJsonWriterProvider<TCustomState>(options);
        var state = new ODataJsonWriterState<TCustomState>(options, writerProvider, jsonWriter)
        {
            ODataUri = uri,
            PayloadKind = ODataPayloadKind.ResourceSet,
            JavaScriptEncoder = DefaultJsonWriterOptions.Encoder,
            BufferWriter = bufferWriter,
        };
        // get writer
        var writer = writerProvider.GetWriter<T>(); // should we pass the value as well?

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


            if (state.Stack.HasSuspendedFrames())
            {
                var suspendedPipeReader = state.Stack.LastSuspendedFrame.PipeReader;
                if (suspendedPipeReader != null)
                {
                    // We should prob. check if we need more data before calling this.
                    // It's possible that we reached here not because we need more data,
                    // but because we needed to clear the buffer.
                    await suspendedPipeReader.ReadAsync(cancellationToken: default);
                }
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
}
