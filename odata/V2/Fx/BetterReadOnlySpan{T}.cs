namespace V2.Fx
{
    using System;

    public readonly ref struct BetterReadOnlySpan<T> where T : allows ref struct //// TODO is there other span stuff that you should add in here?
    {
        private readonly Span<byte> data;

        private readonly int length;

        internal BetterReadOnlySpan(Span<byte> memory, int length)
        {
            if (memory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>() * length)
            {
                throw new Exception("TODO");
            }

            data = memory;
            this.length = length;
        }

        internal BetterReadOnlySpan(Span<byte> memory, int length, bool unused)
        {
            data = memory;
            this.length = length;
        }

        public unsafe T this[int index]
        {
            get
            {
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
