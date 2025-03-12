using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Fx
{
    public unsafe ref struct BetterSpan2<T> where T : allows ref struct
    {
        private Container data;

        private readonly int length;

        private readonly bool isSingle;

        internal BetterSpan2(Span<byte> memory, int length)
        {
            if (memory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>() * length)
            {
                throw new Exception("TODO");
            }

            this.data = Unsafe.As<Span<byte>, Container>(ref memory);
            this.length = length;
        }

        internal BetterSpan2(T value)
        {
            this.data = Unsafe.As<T, Container>(ref value);
            this.isSingle = true;
            this.length = 1;
        }

        public T this[int index]
        {
            get
            {
                if (this.isSingle)
                {
                    if (index != 0)
                    {
                        throw new ArgumentOutOfRangeException("TODO");
                    }

                    return Unsafe.As<Container, T>(ref this.data);
                }
                else
                {
                    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
                    ArgumentOutOfRangeException.ThrowIfNegative(index);

                    var span = Unsafe.As<Container, Span<byte>>(ref this.data);
                    fixed (byte* pointer = span)
                    {
                        T* typedPointer = (T*)pointer;
                        return typedPointer[index];
                    }
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

        private ref struct Container
        {
            private int first;

            private int second;
        }
    }
}
