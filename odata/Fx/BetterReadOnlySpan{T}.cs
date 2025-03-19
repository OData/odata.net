namespace Fx
{
    using System;

    public readonly ref struct BetterReadOnlySpan<T> where T : allows ref struct
    {
        private readonly Span<byte> data;

        private readonly int length;

        internal BetterReadOnlySpan(Span<byte> memory, int length)
        {
            if (memory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>() * length)
            {
                throw new Exception("TODO");
            }

            this.data = memory;
            this.length = length;
        }

        internal BetterReadOnlySpan(Span<byte> memory, int length, bool unused)
        {
            this.data = memory;
            this.length = length;
        }

        private BetterReadOnlySpan(Span<byte> memory)
        {
            this.data = memory;
            this.length = 0;
        }

        public unsafe T this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                fixed (byte* pointer = this.data)
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

            fixed (byte* pointer = this.data)
            {
                T* typedPointer = (T*)pointer;
                return ref typedPointer[index];
            }
        }

        public int Length
        {
            get
            {
                return this.length;
            }
        }

        /*public static implicit operator T*(BetterSpan<T> betterSpan)
        {
            return betterSpan.data;
        }*/
    }
}
