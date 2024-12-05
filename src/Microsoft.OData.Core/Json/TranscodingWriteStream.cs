//---------------------------------------------------------------------
// <copyright file="TranscodingWriteStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// A stream that allows converting from one encoding to another when writing to a stream.
    /// </summary>
    /// <remarks>
    /// This is adapted from the .NET runtime's TranscodingStream available in .NET 5+
    /// see: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Text/TranscodingStream.cs
    /// </remarks>
    internal sealed class TranscodingWriteStream : Stream
    {
        // We optimistically assume 1 byte ~ 1 char during transcoding. This is a good rule of thumb
        // but isn't always appropriate: transcoding between single-byte and multi-byte encodings
        // will violate this, as will any invalid data fixups performed by the transcoder itself.
        // To account for these unknowns we have a minimum scratch buffer size we use during the
        // transcoding process. This should be generous enough to account for even the largest
        // fallback mechanism we're likely to see in the real world.

        private const int MinWriteRentedArraySize = 4 * 1024;
        private const int MaxWriteRentedArraySize = 1024 * 1024;

        private static ValueTask CompletedValueTask => default;

        private readonly Encoding _innerEncoding;
        private readonly Encoding _thisEncoding;
        private Stream _innerStream; // null if the wrapper has been disposed
        private readonly bool _leaveOpen;

        // Fields used for writing bytes [this] -> chars -> bytes [inner]
        // Lazily initialized the first time we need to write
        private Encoder _innerEncoder;
        private Decoder _thisDecoder;

        internal TranscodingWriteStream(Stream targetStream, Encoding targetEncoding, Encoding inputEncoding, bool leaveOpen = false)
        {
            Debug.Assert(targetStream != null);
            Debug.Assert(targetEncoding != null);
            Debug.Assert(inputEncoding != null);

            _innerStream = targetStream;
            _leaveOpen = leaveOpen;

            _innerEncoding = targetEncoding;
            _thisEncoding = inputEncoding;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length
        {
            get
            {
                Debug.Assert(false, "Should never get here.");
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                Debug.Assert(false, "Should never get here.");
                throw new NotSupportedException();
            }

            set
            {
                Debug.Assert(false, "Should never get here.");
                throw new NotSupportedException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            Debug.Assert(disposing, "This type isn't finalizable.");

            if (_innerStream is null)
            {
                return; // dispose called multiple times, ignore
            }

            // First, flush any pending data to the inner stream.

            ArraySegment<byte> pendingData = FinalFlushWriteBuffers();
            if (pendingData.Count != 0)
            {
                _innerStream.Write(pendingData);
            }

            // Mark our object as disposed

            Stream innerStream = _innerStream;
            _innerStream = null!;

            // And dispose the inner stream if needed

            if (!_leaveOpen)
            {
                innerStream.Dispose();
            }
        }

        public override ValueTask DisposeAsync()
        {
            if (_innerStream is null)
            {
                return default; // dispose called multiple times, ignore
            }

            // First, get any pending data destined for the inner stream.

            ArraySegment<byte> pendingData = FinalFlushWriteBuffers();

            if (pendingData.Count == 0)
            {
                // Fast path: just dispose of the object graph.
                // No need to write anything to the stream first.

                Stream innerStream = _innerStream;
                _innerStream = null!;

                return (_leaveOpen)
                    ? default /* no work to do */
                    : innerStream.DisposeAsync();
            }

            // Slower path; need to perform an async write followed by an async dispose.

            return DisposeAsyncCore(pendingData);

            async ValueTask DisposeAsyncCore(ArraySegment<byte> pendingData)
            {
                Debug.Assert(pendingData.Count != 0);

                Stream innerStream = _innerStream;
                _innerStream = null!;

                await innerStream.WriteAsync(pendingData.AsMemory()).ConfigureAwait(false);

                if (!_leaveOpen)
                {
                    await innerStream.DisposeAsync().ConfigureAwait(false);
                }
            }
        }

        // Sets up the data structures that are necessary before any write operation takes place,
        // throwing if the object is in a state where writes are not possible.
        private void EnsurePreWriteConditions()
        {
            ThrowIfDisposed();
            if (_innerEncoder is null)
            {
                InitializeWriteDataStructures();
            }

            void InitializeWriteDataStructures()
            {
                _innerEncoder = _innerEncoding.GetEncoder();
                _thisDecoder = _thisEncoding.GetDecoder();
            }
        }

        // returns any pending data that needs to be flushed to the inner stream before disposal
        private ArraySegment<byte> FinalFlushWriteBuffers()
        {
            // If this stream was never used for writing, no-op.

            if (_thisDecoder is null || _innerEncoder is null)
            {
                return default;
            }

            // convert bytes [this] -> chars
            // Having leftover data in our buffers should be very rare since it should only
            // occur if the end of the stream contains an incomplete multi-byte sequence.
            // Let's not bother complicating this logic with array pool rentals or allocation-
            // avoiding loops.
            char[] chars = Array.Empty<char>();
            int charCount = _thisDecoder.GetCharCount(Array.Empty<byte>(), 0, 0, flush: true);

            if (charCount > 0)
            {
                chars = new char[charCount];
                charCount = _thisDecoder.GetChars(Array.Empty<byte>(), 0, 0, chars, 0, flush: true);
            }

            // convert chars -> bytes [inner]
            // It's possible that _innerEncoder might need to perform some end-of-text fixup
            // (due to flush: true), even if _thisDecoder didn't need to do so.

            byte[] bytes = Array.Empty<byte>();
            int byteCount = _innerEncoder.GetByteCount(chars, 0, charCount, flush: true);

            if (byteCount > 0)
            {
                bytes = new byte[byteCount];
                byteCount = _innerEncoder.GetBytes(chars, 0, charCount, bytes, 0, flush: true);
            }

            return new ArraySegment<byte>(bytes, 0, byteCount);
        }

        public override void Flush()
        {
            // Don't pass flush: true to our inner decoder + encoder here, since it could cause data
            // corruption if a flush occurs mid-stream. Wait until the stream is being closed.

            ThrowIfDisposed();
            _innerStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            // Don't pass flush: true to our inner decoder + encoder here, since it could cause data
            // corruption if a flush occurs mid-stream. Wait until the stream is being closed.

            ThrowIfDisposed();
            return _innerStream.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int Read(Span<byte> buffer)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        private void ThrowIfDisposed()
        {
            if (_innerStream is null)
            {
                throw new ObjectDisposedException(nameof(TranscodingWriteStream));
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArguments(buffer, offset, count);

            Write(new ReadOnlySpan<byte>(buffer, offset, count));
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            EnsurePreWriteConditions();

            if (buffer.IsEmpty)
            {
                return;
            }

            int rentalLength = Math.Clamp(buffer.Length, MinWriteRentedArraySize, MaxWriteRentedArraySize);

            char[] scratchChars = ArrayPool<char>.Shared.Rent(rentalLength);
            byte[] scratchBytes = ArrayPool<byte>.Shared.Rent(rentalLength);

            try
            {
                bool decoderFinished, encoderFinished;
                do
                {
                    // convert bytes [this] -> chars

                    _thisDecoder.Convert(
                        bytes: buffer,
                        chars: scratchChars,
                        flush: false,
                        out int bytesConsumed,
                        out int charsWritten,
                        out decoderFinished);

                    buffer = buffer.Slice(bytesConsumed);

                    // convert chars -> bytes [inner]

                    Span<char> decodedChars = scratchChars.AsSpan(0, charsWritten);

                    do
                    {
                        _innerEncoder.Convert(
                            chars: decodedChars,
                            bytes: scratchBytes,
                            flush: false,
                            out int charsConsumed,
                            out int bytesWritten,
                            out encoderFinished);

                        decodedChars = decodedChars.Slice(charsConsumed);

                        // It's more likely that the inner stream provides an optimized implementation of
                        // Write(byte[], ...) over Write(ROS<byte>), so we'll prefer the byte[]-based overloads.

                        _innerStream.Write(scratchBytes, 0, bytesWritten);
                    } while (!encoderFinished);
                } while (!decoderFinished);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(scratchChars);
                ArrayPool<byte>.Shared.Return(scratchBytes);
            }
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateBufferArguments(buffer, offset, count);

            return WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            EnsurePreWriteConditions();

            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask(Task.FromCanceled(cancellationToken));
            }

            if (buffer.IsEmpty)
            {
                return CompletedValueTask;
            }

            return WriteAsyncCore(buffer, cancellationToken);

            async ValueTask WriteAsyncCore(ReadOnlyMemory<byte> remainingOuterEncodedBytes, CancellationToken cancellationToken)
            {
                int rentalLength = Math.Clamp(remainingOuterEncodedBytes.Length, MinWriteRentedArraySize, MaxWriteRentedArraySize);

                char[] scratchChars = ArrayPool<char>.Shared.Rent(rentalLength);
                byte[] scratchBytes = ArrayPool<byte>.Shared.Rent(rentalLength);

                try
                {
                    bool decoderFinished, encoderFinished;
                    do
                    {
                        // convert bytes [this] -> chars

                        _thisDecoder.Convert(
                            bytes: remainingOuterEncodedBytes.Span,
                            chars: scratchChars,
                            flush: false,
                            out int bytesConsumed,
                            out int charsWritten,
                            out decoderFinished);

                        remainingOuterEncodedBytes = remainingOuterEncodedBytes.Slice(bytesConsumed);

                        // convert chars -> bytes [inner]

                        ArraySegment<char> decodedChars = new ArraySegment<char>(scratchChars, 0, charsWritten);

                        do
                        {
                            _innerEncoder.Convert(
                                chars: decodedChars,
                                bytes: scratchBytes,
                                flush: false,
                                out int charsConsumed,
                                out int bytesWritten,
                                out encoderFinished);

                            decodedChars = decodedChars.Slice(charsConsumed);
                            await _innerStream.WriteAsync(new ReadOnlyMemory<byte>(scratchBytes, 0, bytesWritten), cancellationToken).ConfigureAwait(false);
                        } while (!encoderFinished);
                    } while (!decoderFinished);
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(scratchChars);
                    ArrayPool<byte>.Shared.Return(scratchBytes);
                }
            }
        }

        public override void WriteByte(byte value)
            => Write(new ReadOnlySpan<byte>(new byte[] { value }));

        private static new void ValidateBufferArguments(byte[] buffer, int offset, int size)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if ((uint)offset > (uint)buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if ((uint)size > (uint)(buffer.Length - offset))
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }
        }
    }
}
