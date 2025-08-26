using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3.IO;
using Microsoft.OData.Serializer.V3.Json;
using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
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

    public static ValueTask WriteAsync<T, TCustomState>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions<TCustomState> options)
    {
        return WriteAsync<T, TCustomState>(value, stream, uri, model, options, default);
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
        var state = new ODataJsonWriterState<TCustomState>(options, writerProvider, jsonWriter)
        {
            ODataUri = uri,
            PayloadKind = ODataPayloadKind.ResourceSet,
            JavaScriptEncoder = DefaultJsonWriterOptions.Encoder,
            BufferWriter = bufferWriter,
        };

        state.SetCustomSate(in customState);
        // get writer
        var writer = writerProvider.GetWriter<T>(); // should we pass the value as well?

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


                if (state.Stack.HasSuspendedFrames())
                {
                    ResourceCleanupState cleanupState = state.Stack.LastSuspendedFrame.CleanUpState;

                    // TODO: ideally we should not be checking the progress value outside
                    // the writer that set it since each writer might have a different
                    // interpretation of the progress value. Another code smell?
                    if (cleanupState == ResourceCleanupState.Cleanup)
                    {
                        object? suspendedValueStream = state.Stack.LastSuspendedFrame.StreamingValueSource;
                        if (suspendedValueStream is PipeReader suspendedPipeReader)
                        {
                            await suspendedPipeReader.CompleteAsync();
                            Debug.Assert(state.DisposableObjects != null, "We should have tracked the object for disposal.");
                            state.DisposableObjects.Remove(suspendedValueStream!);
                        }
                        else if (suspendedValueStream is IAsyncDisposable asyncDisposable)
                        {
                            await asyncDisposable.DisposeAsync();
                            Debug.Assert(state.DisposableObjects != null, "We should have tracked the object for disposal.");
                            state.DisposableObjects.Remove(suspendedValueStream!);
                        }

                        state.Stack.LastSuspendedFrame.CleanUpState = ResourceCleanupState.CleanupComplete;
                    }
                    else
                    {
                        object? suspendedValueStream = state.Stack.LastSuspendedFrame.StreamingValueSource;

                        // This is really messy
                        if (suspendedValueStream is PipeReader suspendedPipeReader)
                        {
                            // We should prob. check if we need more data before calling this.
                            // It's possible that we reached here not because we need more data,
                            // but because we needed to clear the buffer.
                            var result = await suspendedPipeReader.ReadAsync(cancellationToken: default);

                            // TODO: check for buffer cancellation.
                            // We call to advance to allow the continuation to call TryRead() again.
                            // Since we've advanced by 0 bytes, the next TryRead() will return the same buffer again.
                            suspendedPipeReader.AdvanceTo(result.Buffer.GetPosition(0));
                        }
                        // TODO: Have a common non-generic interface for IBufferedReader to avoid
                        // creating multiple branches for each supported type
                        else if (suspendedValueStream is IBufferedReader<byte> suspendedByteReader)
                        {
                            var result = await suspendedByteReader.ReadAsync();
                            suspendedByteReader.AdvanceTo(result.Buffer.GetPosition(0));
                        }
                        else if (suspendedValueStream is IBufferedReader<char> suspendedCharReader)
                        {
                            var result = await suspendedCharReader.ReadAsync();
                            suspendedCharReader.AdvanceTo(result.Buffer.GetPosition(0));
                        }
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
        }
        finally
        {
            if (state.DisposableObjects != null)
            {
                foreach (var resource in state.DisposableObjects)
                {
                    if (resource is PipeReader pipeReader)
                    {
                        await pipeReader.CompleteAsync();
                    }
                    else if (resource is IAsyncDisposable asyncDisposable)
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
}
