namespace Fx
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field, Inherited = false)]
    internal sealed class IntrinsicAttribute : Attribute
    {
    }

    [Intrinsic]
    public unsafe ref struct BetterSpan<T> where T : allows ref struct
    {
        private readonly T* data;

        internal BetterSpan(T* data, int length)
        {
            this.data = data;
            Length = length;
        }

        internal BetterSpan(Span<byte> memory, int length)
        {
            if (memory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>() * length)
            {
                throw new Exception("TODO");
            }

            fixed (byte* pointer = memory)
            {
                this.data = (T*)pointer;
            }

            Length = length;
        }

        public T this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                return data[index];
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                data[index] = value;
            }
        }

        public int Length { get; }

        public static implicit operator T*(BetterSpan<T> betterSpan)
        {
            return betterSpan.data;
        }
    }
}
