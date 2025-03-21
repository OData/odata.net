namespace V2.Fx
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// You looked into if you could have something like `BetterReadOnlySpan<byte> = stackalloc byte[10];`. You *can* by having an implicit operator taking in `Span<byte>`. However, the return type of that operator must be `BetterReadOnlySpan<T>`, so you will need to assert something about the length of the input `span`, and having assertions in implicit operators is a bad idea. Note that `differentmemory` can be used instead for this purpose.
    /// </remarks>
    public readonly ref struct BetterReadOnlySpan<T> where T : allows ref struct //// TODO is there other span stuff that you should add in here?
    {
        internal readonly DifferentMemory data; //// TODO can you make this private?

        private readonly int length;

        private BetterReadOnlySpan(DifferentMemory memory, int length)
        {
            if (memory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>() * length)
            {
                throw new Exception("TODO");
            }

            this.data = memory;
            this.length = length;
        }

        //// TODO since these factory methods are public, you need to add the same tests as you added to betterreadonlyspan

        public static BetterReadOnlySpan<byte> Create(DifferentMemory memory)
        {
            return new BetterReadOnlySpan<byte>(memory, memory.Length);
        }

        public static BetterReadOnlySpan<T> Create(DifferentMemory memory, int length)
        {
            return new BetterReadOnlySpan<T>(memory, length);
        }

        public ref T this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                var elementSize = System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
                var slice = this.data.Slice(index * elementSize, elementSize);
                return ref Fx.Runtime.InteropServices.MemoryMarshal.AsRef<T>(slice);
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
        }

        public ref readonly byte GetPinnableReference()
        {
            return ref this.data.GetPinnableReference();
        }
    }

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
            return new DifferentMemory(span.data.memory);
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
