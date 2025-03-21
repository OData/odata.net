namespace V2.Fx
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The purpose of this type is to be, in all ways possible, a `span<byte>` that can be created from either a `span` or a `betterspan`
    /// </remarks>
    public readonly ref struct ByteSpan //// TODO is there other span stuff that you should add in here?
    {
        private readonly Span<byte> memory;

        private ByteSpan(Span<byte> memory)
        {
            this.memory = memory;
        }

        public static ByteSpan Create(Span<byte> span)
        {
            return new ByteSpan(span);
        }

        public static ByteSpan Create(BetterReadOnlySpan<byte> span)
        {
            return new ByteSpan(MemoryMarshal.CreateSpan(ref span.GetPinnableReference(), span.Length));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the sum of <paramref name="startIndex"/> and <paramref name="length"/> is greater than <see cref="ByteSpan.Length"/></exception>
        public ByteSpan Slice(int startIndex, int length)
        {
            if (startIndex + length > this.Length)
            {
                var message = $"The slice of memory exceeds the length of the memory that is being sliced. The provided '{nameof(startIndex)}' was '{startIndex}'. The provided '{nameof(length)}' was '{length}'. The length of the memory being sliced was '{this.Length}'.";
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            return Create(this.memory.Slice(startIndex, length));
        }

        public ref byte this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Length)
                {
                    var message = $"'{nameof(index)}' must be a non-negative value less than the length of the span. The provided '{nameof(index)}' was '{index}'. The length of the span was '{this.Length}'.";
                    throw new IndexOutOfRangeException(message);
                }

                return ref this.memory[index];
            }
        }

        public int Length
        {
            get
            {
                return this.memory.Length;
            }
        }

        //// TODO you can add this here (and other places) if you really want to get parity with the .net version [EditorBrowsable(EditorBrowsableState.Never)]
        public ref byte GetPinnableReference()
        {
            ref byte pointer = ref MemoryMarshal.AsRef<byte>(memory);
            return ref pointer;
        }
        
        public static implicit operator ByteSpan(Span<byte> span)
        {
            return ByteSpan.Create(span);
        }
    }
}
