namespace V2.Fx.Runtime.CompilerServices
{
    using System;
    
    public static class Unsafe
    {
        public static unsafe void Copy<T>(Span<byte> destination, in T source) where T : allows ref struct
        {
            if (destination.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>())
            {
                throw new Exception("TODO");
            }

            fixed (byte* pointer = destination)
            {
                System.Runtime.CompilerServices.Unsafe.Copy(pointer, in source);
            }
        }
    }
}
