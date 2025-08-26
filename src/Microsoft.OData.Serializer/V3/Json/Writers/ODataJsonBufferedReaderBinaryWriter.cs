using Microsoft.OData.Serializer.V3.IO;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonBufferedReaderBinaryWriter<TCustomState> :
    ODataJsonWriter<IBufferedReader<byte>, TCustomState>
{
    
    public override bool Write(
        IBufferedReader<byte> value,
        ODataJsonWriterState<TCustomState> state)
    {
        state.Stack.Push();
        if (state.Stack.Current.ResourceProgress < State.ResourceWriteProgress.Value)
        {
            state.JsonWriter.Flush(); // Commit pending bytes to the buffer writer before we start writing to it.
            state.BufferWriter.Write([JsonConstants.DoubleQuote]);
            state.Stack.Current.ResourceProgress = State.ResourceWriteProgress.Value;
            state.Stack.Current.StreamingValueSource = value;
        }

        // See: https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines

        int chunkSize = Math.Min(LargeBinaryStringWriter<TCustomState>.ChunkSize, (int)Math.Floor(state.FreeBufferCapacity * 3 / 4.0));

        bool success = value.TryRead(out BufferedReadResult<byte> readResult);
        if (!success)
        {
            state.Stack.Pop(false);
            return false;
        }

        bool isFinalChunk = readResult.IsCompleted;
        //if (readResult.IsCanceled)
        //{
        //    state.Stack.Pop(false);
        //    return false;
        //}

        Span<byte> trailingBytesBuffer = stackalloc byte[3];
        int numTrailingBytes = 0;
        int examined = 0;

        int totalConsumed = 0;
        int totalWritten = 0;

        var currSeqPos = readResult.Buffer.Start;
        bool reachedEnd = !readResult.Buffer.TryGet(ref currSeqPos, out ReadOnlyMemory<byte> currentSegment, advance: true);
        Debug.Assert(!reachedEnd, "First segment should always be available since we already checked TryRead result.");

        do
        {
            if (numTrailingBytes > 0)
            {
                // Instead of allocating a new segment, let's move some bytes from this segment
                // into the temp buffer. We need at least 3 bytes to encode without trailing.
                // State 1: trailingBytes.Length + segment.Length >= 3, so we can encode without trailing
                if (numTrailingBytes + currentSegment.Length >= 3)
                {
                    int bytesNeeded = 3 - numTrailingBytes;
                    currentSegment.Span.Slice(0, bytesNeeded).CopyTo(trailingBytesBuffer.Slice(numTrailingBytes));
                    var destination = state.BufferWriter.GetSpan(Base64.GetMaxEncodedToUtf8Length(3));

                    OperationStatus encodingResult = Base64.EncodeToUtf8(
                        trailingBytesBuffer,
                        destination,
                        out int bytesConsumed,
                        out int bytesWritten
                        );
                    Debug.Assert(encodingResult == OperationStatus.Done, "Encoding 3 bytes should always succeed.");
                    Debug.Assert(bytesConsumed == 3, "All 3 bytes should be consumed.");
                    state.BufferWriter.Advance(bytesWritten);

                    examined += bytesNeeded;
                    totalConsumed += bytesConsumed;
                    totalWritten += bytesWritten;
                    currentSegment = currentSegment[bytesNeeded..];
                    numTrailingBytes = 0;
                    continue;
                }

                // If we got here, it means the current segment doesn't have enough bytes. This could
                // occur if either we are on the last segment, or the segment is just too small (< 3 bytes)
                // the latter is highly unlikely, but we should handle it anyway.
                bool isLastSegment = readResult.Buffer.Length == totalConsumed + numTrailingBytes + currentSegment.Length;
                if (isLastSegment && isFinalChunk)
                {
                    currentSegment.Span.CopyTo(trailingBytesBuffer.Slice(numTrailingBytes));
                    var toWrite = trailingBytesBuffer.Slice(0, numTrailingBytes + currentSegment.Length);
                    var destination = state.BufferWriter.GetSpan(Base64.GetMaxEncodedToUtf8Length(toWrite.Length));

                    OperationStatus encodingResult = Base64.EncodeToUtf8(
                        toWrite,
                        destination,
                        out int bytesConsumed,
                        out int bytesWritten,
                        isFinalBlock: true
                        );
                    Debug.Assert(encodingResult == OperationStatus.Done, "Final block should always succeed.");
                    Debug.Assert(bytesConsumed == toWrite.Length, "All 3 bytes should be consumed.");
                    state.BufferWriter.Advance(bytesWritten);
                    totalConsumed += bytesConsumed;
                    totalWritten += bytesWritten;

                    numTrailingBytes = 0;
                    totalConsumed += currentSegment.Length;
                    examined += currentSegment.Length;
                    // Move to the next segment
                    reachedEnd = true;
                    continue;
                }
                else if (isLastSegment)
                {
                    // last segment but not final chunk and we don't have enough bytes to encode? We should fetch more data
                    examined += currentSegment.Length;
                    reachedEnd = true;
                    continue;
                }
                else
                {
                    // not enough bytes in this segment, but also not the last segment. We should continue accumulating bytes
                    currentSegment.Span.CopyTo(trailingBytesBuffer.Slice(numTrailingBytes));
                    numTrailingBytes += currentSegment.Length;
                    examined += currentSegment.Length;
                    reachedEnd = !readResult.Buffer.TryGet(ref currSeqPos, out currentSegment, advance: true);
                    Debug.Assert(!reachedEnd, "Next segment should always be available since we already checked ifLastSegment.");
                    continue;
                }
            }
            else
            {
                // if we get here, we have no trailing bytes
                int remainingBeforeFlush = chunkSize - totalConsumed;
                if (remainingBeforeFlush <= 0)
                {
                    // we've reached the chunk size limit, we should break and flush
                    reachedEnd = true;
                    continue;
                }

                var sizeOfDataToRead = Math.Min(currentSegment.Length, remainingBeforeFlush);
                var destLength = Base64.GetMaxEncodedToUtf8Length(sizeOfDataToRead);

                var dataToRead = currentSegment.Span.Slice(0, sizeOfDataToRead);

                // TODO: how do we ensure the destination is never too small?
                Span<byte> destination = state.BufferWriter.GetSpan(destLength);

                bool isFinalBlock = isFinalChunk && readResult.Buffer.Length == totalConsumed + sizeOfDataToRead;


                OperationStatus encodingResult = Base64.EncodeToUtf8(
                    dataToRead,
                    destination,
                    out int bytesConsumed,
                    out int bytesWritten,
                    isFinalBlock: isFinalBlock);
                Debug.Assert(encodingResult == OperationStatus.Done || encodingResult == OperationStatus.NeedMoreData);
                state.BufferWriter.Advance(bytesWritten);

                totalConsumed += bytesConsumed;
                totalWritten += bytesWritten;
                examined += sizeOfDataToRead;

                numTrailingBytes = sizeOfDataToRead - bytesConsumed;

                Debug.Assert(numTrailingBytes < 3, "There should never be more than 3 trailing bytes in Base64 encoding.");
                currentSegment[^numTrailingBytes..].Span.CopyTo(trailingBytesBuffer);

                reachedEnd = isFinalBlock || !readResult.Buffer.TryGet(ref currSeqPos, out currentSegment, advance: true);
            }


        } while (!reachedEnd);


        value.AdvanceTo(readResult.Buffer.GetPosition(totalConsumed), readResult.Buffer.GetPosition(examined));

        if (isFinalChunk && totalConsumed == readResult.Buffer.Length)
        {
            state.BufferWriter.Write([JsonConstants.DoubleQuote]);
            state.Stack.Pop(true);
            return true;
        }


        state.Stack.Pop(false);
        return false;
    }
}
