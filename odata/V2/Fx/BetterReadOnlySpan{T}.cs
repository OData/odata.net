namespace V2.Fx
{
    using System;
    using System.Runtime.CompilerServices;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// You looked into if you could have something like `BetterReadOnlySpan<byte> = stackalloc byte[10];` and the answer is no.
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
            //// TODO should this be a cast in `differentmemory`?

            return new BetterReadOnlySpan<byte>(memory, memory.Length);
        }

        public static BetterReadOnlySpan<T> Create(DifferentMemory memory, int length)
        {
            return new BetterReadOnlySpan<T>(memory, length);
        }

        public static unsafe BetterReadOnlySpan<T> Create(scoped in T instance)
        {
            void* pointer = Fx.Runtime.CompilerServices.Unsafe.AsPointer(instance);

            {
                var span = new ReadOnlySpan<byte>(pointer, Unsafe.SizeOf<T>());

                return Create(span, 1);
            }
        }

        public unsafe T this[int index]
        {
            get
            {
                //// TODO return `ref T` (like `Get`) and remove `Get`
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                fixed (byte* pointer = data)
                {
                    T* typedPointer = (T*)pointer;
                    return typedPointer[index];
                }
            }
        }

        public unsafe ref T Get(int index)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
            ArgumentOutOfRangeException.ThrowIfNegative(index);

            fixed (byte* pointer = data)
            {
                T* typedPointer = (T*)pointer;
                return ref typedPointer[index];
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
        }

        public ref byte GetPinnableReference()
        {
            return ref this.data.GetPinnableReference();
        }
    }

    public ref struct DifferentMemory //// TODO can't be readonly because of the pinnedreference for some reason
    {
        private readonly ReadOnlySpan<byte> memory;

        private ref byte pinnedReference;

        private unsafe DifferentMemory(ReadOnlySpan<byte> memory)
        {
            this.memory = memory;
            fixed (byte* pointer = memory)
            {
                this.pinnedReference = ref *pointer;
            }
        }

        public static DifferentMemory Create(ReadOnlySpan<byte> span)
        {
            return new DifferentMemory(span);
        }

        public static DifferentMemory Create(BetterReadOnlySpan<byte> span)
        {
            return new DifferentMemory(span.data.memory);
        }

        public unsafe byte this[int index]
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

        public ref byte GetPinnableReference()
        {
            return ref this.pinnedReference;
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
