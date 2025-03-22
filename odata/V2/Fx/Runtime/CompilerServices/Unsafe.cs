namespace V2.Fx.Runtime.CompilerServices
{
    using System;

    using OldUnsafe = System.Runtime.CompilerServices.Unsafe;
    
    public static class Unsafe
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the <see cref="ByteSpan.Length"/> or <paramref name="destination"/> is not the same as the
        /// <see langword="sizeof"/> <paramref name="source"/>
        /// </exception>
        public static unsafe void Copy<T>(ByteSpan destination, in T source) where T : allows ref struct
        {
            var size = OldUnsafe.SizeOf<T>();
            if (destination.Length != size)
            {
                var message = $"The length of '{nameof(destination)}' must be the same as the size of '{nameof(source)}'. The length of '{nameof(destination)}' was '{destination.Length}'. The size of '{nameof(source)}' was '{size}'.";
                throw new ArgumentOutOfRangeException(message, (Exception?)null);
            }

            fixed (byte* pointer = destination)
            {
                System.Runtime.CompilerServices.Unsafe.Copy(pointer, in source);
            }
        }

        public static unsafe void* AsPointer<T>(in T value) where T : allows ref struct
        {
            ref T reference = ref OldUnsafe.AsRef(in value);

            return OldUnsafe.AsPointer(ref reference);
        }
    }
}
