namespace V2.Fx.Runtime.CompilerServices
{
    using System;
    
    public static class Unsafe
    {
        public static unsafe void Copy<T>(DifferentMemory destination, in T source) where T : allows ref struct
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

        public static unsafe void* AsPointer<T>(in T value) where T : allows ref struct
        {
            fixed (void* pointer = &value)
            {
                return pointer;
            }
        }
    }
}
