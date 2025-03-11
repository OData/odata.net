using System.Diagnostics.CodeAnalysis;

namespace Fx
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field, Inherited = false)]
    internal sealed class IntrinsicAttribute : Attribute
    {
    }

    public readonly unsafe ref struct BetterSpan<T> where T : allows ref struct
    {
        private readonly Span<byte> data;

        ////private readonly T* data;

        private readonly int length;

        internal BetterSpan(Span<byte> memory, int length)
        {
            if (memory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>() * length)
            {
                throw new Exception("TODO");
            }

            /*fixed (byte* pointer = memory)
            {
                this.data = (T*)pointer;
            }*/

            this.data = memory;
            this.length = length;
        }

        private BetterSpan(Span<byte> memory)
        {
            if (memory.Length != 0)
            {
                throw new Exception("TODO");
            }

            this.data = null;
            this.length = 0;
        }

        public static BetterSpan<T> Empty => new BetterSpan<T>(Span<byte>.Empty);

        public T this[int index]
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
            /*set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                data[index] = value;
            }*/
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
