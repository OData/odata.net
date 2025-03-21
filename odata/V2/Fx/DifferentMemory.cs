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
    public readonly ref struct DifferentMemory //// TODO is there other span stuff that you should add in here?
    {
        private readonly Span<byte> memory;

        private DifferentMemory(Span<byte> memory)
        {
            this.memory = memory;
        }

        public static DifferentMemory Create(Span<byte> span)
        {
            return new DifferentMemory(span);
        }

        public static DifferentMemory Create(BetterReadOnlySpan<byte> span)
        {
            return new DifferentMemory(MemoryMarshal.CreateSpan(ref span.GetPinnableReference(), span.Length));
        }

        public DifferentMemory Slice(int startIndex, int length)
        {
            return Create(this.memory.Slice(startIndex, length));
        }

        public ref byte this[int index]
        {
            get
            {
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
        
        public static implicit operator DifferentMemory(Span<byte> span)
        {
            return DifferentMemory.Create(span);
        }
    }
}
