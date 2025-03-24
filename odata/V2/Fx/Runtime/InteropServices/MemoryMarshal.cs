namespace V2.Fx.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The purpose of this type is to be equivalent to <see cref="System.Runtime.InteropServices.MemoryMarshal"/> but using
    /// <see cref="ByteSpan"/> instead of <see cref="Span{byte}"/> and using <see cref="SpanEx{T}"/> instead of
    /// <see cref="Span{T}"/>
    /// </remarks>
    public static class MemoryMarshal
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memory"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <see cref="ByteSpan.Length"/> of <paramref name="memory"/> is not the same as the
        /// <see langword="sizeof"/> <typeparamref name="T"/>
        /// </exception>
        /// <remarks>
        /// <see cref="System.Runtime.InteropServices.MemoryMarshal.AsRef{T}(Span{byte})"/> specifically makes an assertion
        /// *against* <typeparamref name="T"/> returning <see langword="true"/> for
        /// <see cref="RuntimeHelpers.IsReferenceOrContainsReferences{T}"/> because it doesn't want to allow for situations like
        /// this:
        /// 
        /// ```
        /// public readonly ref struct Foo
        /// {
        ///   public int Value { get; }
        /// }
        /// ...
        /// public Foo DoWork()
        /// {
        ///   Span&lt;byte> bytes = stackalloc[10];
        ///   return Memory.AsRef&lt;Foo>(bytes);
        /// }
        /// ```
        /// 
        /// This would allow the bytes allocated in `bytes` to be referenced outside of the declaration scope. *This* method,
        /// however, is intentionally allowing for <see langword="ref"/> <see langword="struct"/>s and so we need to leave that
        /// check out. It is up to the caller to not leak this scope.
        /// </remarks>
        public static ref T AsRef<T>(ByteSpan memory) where T : allows ref struct
        {
            var typeSize = Unsafe.SizeOf<T>();
            if (memory.Length != typeSize)
            {
                var message = $"The length of '{nameof(memory)}' must be the same as the size of the type to get a reference to. The length of '{nameof(memory)}' was '{memory.Length}'. The type was '{typeof(T).FullName}'; its size was '{typeSize}'.";
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            return ref Unsafe.As<byte, T>(ref GetReference(memory));
        }
        
        public static ref byte GetReference(ByteSpan span)
        {
            return ref span.GetPinnableReference();
        }

        public static unsafe SpanEx<T> CreateSpan<T>(scoped ref T reference, int length) where T : allows ref struct
        {
            var pointer = Unsafe.AsPointer(ref reference);
            return SpanEx.FromMemory<T>(
                new Span<byte>(
                    pointer,
                    length * Unsafe.SizeOf<T>()),
                length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <see cref="ByteSpan.Length"/> or <paramref name="destination"/> is not the same as the
        /// <see langword="sizeof"/> <typeparamref name="T"/>
        /// </exception>
        public static unsafe void Write<T>(ByteSpan destination, in T value) where T : allows ref struct
        {
            var typeSize = Unsafe.SizeOf<T>();
            if (destination.Length != typeSize)
            {
                var message = $"The length of '{nameof(destination)}' must be the same as the size of the type of '{nameof(value)}'. The length of '{nameof(destination)}' was '{destination.Length}'. The type of '{nameof(value)}' was '{typeof(T).FullName}'; its size was '{typeSize}'.";
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            fixed (byte* pointer = destination)
            {
                Unsafe.Copy(pointer, in value);
            }
        }
    }
}
