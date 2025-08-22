using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonPipeReaderBinaryWriter<TCustomState> : ODataJsonWriter<PipeReader, TCustomState>
{
    public override bool Write(PipeReader value, ODataJsonWriterState<TCustomState> state)
    {
        state.Stack.Push();
        if (state.Stack.Current.ResourceProgress < State.ResourceWriteProgress.Value)
        {
            state.JsonWriter.Flush(); // Commit pending bytes to the buffer writer before we start writing to it.
            state.BufferWriter.Write([JsonConstants.DoubleQuote]);
            state.Stack.Current.ResourceProgress = State.ResourceWriteProgress.Value;
            state.Stack.Current.PipeReader = value;
        }

        // See: https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines

        int chunkSize = Math.Min(LargeBinaryStringWriter<TCustomState>.ChunkSize, (int)Math.Floor(state.FreeBufferCapacity * 3 / 4.0));

        bool success = value.TryRead(out ReadResult readResult);
        if (!success)
        {
            state.Stack.Pop(false);
            return false;
        }

        bool isFinalChunk = readResult.IsCompleted;
        if (readResult.IsCanceled)
        {
            state.Stack.Pop(false);
            return false;
        }

        // the read buffer is a sequence of memory segments. So we'll have to consume it in a loop.
        // We could also read just one segment at a time and return, but would be less efficient.
        // It's also possible that the sequence contains more data than we can fit in a single
        // chunk. So we may need to terminate before we reach the end of the sequence.

        // naive version, read only one segment for simplicity
        var data = readResult.Buffer.FirstSpan;
        var dataToRead = data[0..Math.Min(data.Length, chunkSize)];

        // TODO: how do we ensure the destination is never too small?
        var destLength = Base64.GetMaxEncodedToUtf8Length(dataToRead.Length);
        Span<byte> destination = state.BufferWriter.GetSpan(destLength);

        bool isFinalBlock = isFinalChunk && readResult.Buffer.Length == dataToRead.Length;
        OperationStatus encodingResult = Base64.EncodeToUtf8(
            dataToRead,
            destination,
            out int bytesConsumed,
            out int bytesWritten,
            isFinalBlock: isFinalBlock);
        Debug.Assert(encodingResult == OperationStatus.Done || encodingResult == OperationStatus.NeedMoreData);

        // Since the pipe reader is stateful, we don't need to track in the state how much we've read, or trailing bytes.
        // We tell the reader how much we consumed and how much we examined so that it knows whether to fetch more data.
        value.AdvanceTo(readResult.Buffer.GetPosition(bytesConsumed), readResult.Buffer.GetPosition(dataToRead.Length));

        if (isFinalBlock)
        {
            // Should we dispose the reader? We should have a flag for this.
            Debug.Assert(bytesConsumed == dataToRead.Length, "All bytes should be consumed in the final block.");
            state.Stack.Pop(true);
            return true;
        }


        state.Stack.Pop(false);
        return false;
    }
}
