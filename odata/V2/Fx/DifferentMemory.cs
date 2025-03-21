namespace V2.Fx
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The purpose of this type is to be, in all ways possible, a readonlyspan<byte> that can be created from either a span or a betterspan
    /// </remarks>
    public readonly ref struct DifferentMemory
    {
        private readonly ReadOnlySpan<byte> memory;

        private DifferentMemory(ReadOnlySpan<byte> memory)
        {
            this.memory = memory;
        }

        public static DifferentMemory Create(ReadOnlySpan<byte> span)
        {
            return new DifferentMemory(span);
        }

        public static DifferentMemory Create(BetterReadOnlySpan<byte> span)
        {
            return new DifferentMemory(MemoryMarshal.CreateReadOnlySpan(in span.GetPinnableReference(), span.Length));
        }

        public DifferentMemory Slice(int startIndex, int length)
        {
            return Create(this.memory.Slice(startIndex, length));
        }

        public byte this[int index]
        {
            get
            {
                return this.memory[index];
            }
        }

        public int Length
        {
            get
            {
                return this.memory.Length;
            }
        }

        //// TODO [EditorBrowsable(EditorBrowsableState.Never)]
        public ref readonly byte GetPinnableReference()
        {
            ref readonly byte pointer = ref MemoryMarshal.AsRef<byte>(memory);
            return ref pointer;
        }
        
        public static implicit operator DifferentMemory(Span<byte> span)
        {
            return DifferentMemory.Create(span);
        }

        public static implicit operator DifferentMemory(ReadOnlySpan<byte> span)
        {
            return DifferentMemory.Create(span);
        }
    }
}
