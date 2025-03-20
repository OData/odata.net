namespace V2.Fx
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// You looked into if you could have something like `BetterReadOnlySpan<byte> = stackalloc byte[10];` and the answer is no.
    /// </remarks>
    public readonly ref struct BetterReadOnlySpan<T> where T : allows ref struct //// TODO is there other span stuff that you should add in here?
    {
        private readonly ReadOnlySpan<byte> data;

        private readonly int length;

        private BetterReadOnlySpan(ReadOnlySpan<byte> memory, int length)
        {
            if (memory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>() * length)
            {
                throw new Exception("TODO");
            }

            data = memory;
            this.length = length;
        }

        public static BetterReadOnlySpan<byte> Create(ReadOnlySpan<byte> span)
        {
            return new BetterReadOnlySpan<byte>(span, span.Length);
        }

        public static BetterReadOnlySpan<T> Create(BetterReadOnlySpan<byte> span, int length)
        {
            return new BetterReadOnlySpan<T>(span.data, length);
        }

        public static unsafe BetterReadOnlySpan<T> Create(in T instance)
        {
            fixed (T* pointer = &instance)
            {
                var span = new ReadOnlySpan<byte>(pointer, Unsafe.SizeOf<T>());

                return BetterReadOnlySpan.FromMemory<T>(Create(span), 1);
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
    }
}
