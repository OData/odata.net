namespace V2.Fx.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;

    public static class MemoryMarshal
    {
        public static unsafe ref T AsRef<T>(ByteSpan memory) where T : allows ref struct
        {
            if (memory.Length != Unsafe.SizeOf<T>())
            {
                throw new System.Exception("TODO");
            }

            fixed (byte* pointer = memory)
            {
                return ref Unsafe.AsRef<T>(pointer);
            }
        }

        public static unsafe BetterReadOnlySpan<T> CreateSpan<T>(scoped in T reference) where T : allows ref struct //// TODO the actual memorymarshal uses `scoped ref`; it also takes a length parameter
        {
            var pointer = Fx.Runtime.CompilerServices.Unsafe.AsPointer(reference);
            return BetterReadOnlySpan.FromMemory<T>(
                new Span<byte>(
                    pointer,
                    Unsafe.SizeOf<T>()),
                1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <see cref="ByteSpan.Length"/> or <paramref name="destination"/> is not the same as the
        /// <see langword="sizeof"/> <paramref name="source"/>
        /// </exception>
        public static unsafe void Write<T>(ByteSpan destination, in T value) where T : allows ref struct
        {
            var size = Unsafe.SizeOf<T>();
            if (destination.Length != size)
            {
                var message = $"The length of '{nameof(destination)}' must be the same as the size of '{nameof(value)}'. The length of '{nameof(destination)}' was '{destination.Length}'. The size of '{nameof(value)}' was '{size}'.";
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            fixed (byte* pointer = destination)
            {
                Unsafe.Copy(pointer, in value);
            }
        }
    }
}
