namespace V2.Fx
{
    using System;
    using System.Runtime.CompilerServices;

    using Fx.Runtime.InteropServices;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// The purpose of this type is to be equivalent to <see cref="System.Span{T}"/> but allows for <typeparamref name="T"/> to be
    /// a <see langword="ref"/> <see langword="struct"/>
    /// 
    /// You looked into if you could have something like `<see cref="SpanEx{byte}"/> foo = stackalloc byte[10];`. You *can* by having an implicit
    /// operator taking in `<see cref="Span{byte}"/>`. However, the return type of that operator must be <see cref="SpanEx{T}"/>, so you will need to
    /// assert something about the length of the input `<see cref="Span{byte}"/>`, and having assertions in implicit operators is a bad idea. Note that
    /// <see cref="ByteSpan"/> can be used instead for this purpose.
    /// </remarks>
    public readonly ref struct SpanEx<T> where T : allows ref struct //// TODO is there other span stuff that you should add in here?
    {
        private readonly ByteSpan data;

        private readonly int length;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="length"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the length of <paramref name="memory"/> is not the same as the product of <paramref name="length"/> and the
        /// <see langword="sizeof"/> <typeparamref name="T"/>
        /// </exception>
        private SpanEx(ByteSpan memory, int length)
        {
            var elementSize = Unsafe.SizeOf<T>();
            if (memory.Length != elementSize * length)
            {
                var message = $"The number of bytes in '{nameof(memory)}' must exactly fit the number of elements in the '{nameof(SpanEx)}'. The number of bytes provide was '{memory.Length}'. The number of requested elements was '{length}'. The type of each element was '{typeof(T).FullName}'; its size was '{elementSize}'.";
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            this.data = memory;
            this.length = length;
        }

        public static SpanEx<byte> Create(ByteSpan memory)
        {
            return new SpanEx<byte>(memory, memory.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the length of <paramref name="memory"/> is not the same as the product of <paramref name="length"/> and the
        /// <see langword="sizeof"/> <typeparamref name="T"/>
        /// </exception>
        public static SpanEx<T> Create(ByteSpan memory, int length)
        {
            return new SpanEx<T>(memory, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="index"/> is negative or greater than or equal to <see cref="Length"/>
        /// </exception>
        public ref T this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Length);
                ArgumentOutOfRangeException.ThrowIfNegative(index);

                var elementSize = Unsafe.SizeOf<T>();
                var slice = this.data.Slice(index * elementSize, elementSize);
                return ref MemoryMarshal.AsRef<T>(slice);
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

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref struct Enumerator
        {
            private readonly SpanEx<T> span;

            private int index;

            public Enumerator(SpanEx<T> span)
            {
                this.span = span;

                this.index = -1;
            }

            public T Current
            {
                get
                {
                    if (this.index < 0)
                    {
                        throw new Exception("TODO");
                    }

                    return this.span[this.index];
                }
            }

            public bool MoveNext()
            {
                ++this.index;
                return this.index < this.span.length;
            }
        }
    }
}
