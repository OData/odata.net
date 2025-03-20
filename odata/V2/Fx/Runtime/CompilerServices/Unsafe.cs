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
            
            //// TODO if you keep the below method, you should delegate this method to it
        }

        public static unsafe void Copy<T>(DifferentMemory destination, in T source, int offset) where T : allows ref struct
        {
            var index = offset * System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
            if (offset + System.Runtime.CompilerServices.Unsafe.SizeOf<T>() >= destination.Length)
            {
                throw new Exception("TODO");
            }

            fixed (byte* pointer = destination)
            {
                var indexedPointer = pointer + index;

                System.Runtime.CompilerServices.Unsafe.Copy(indexedPointer, in source);
            }
        }

        public static unsafe void* AsPointer<T>(in T value) where T : allows ref struct
        {
            fixed (void* pointer = BetterReadOnlySpan.FromInstance(value))
            {
                return pointer;
            }
        }
    }
}
