using System.Runtime.CompilerServices;

namespace V2.Fx.Runtime.InteropServices
{
    public static class MemoryMarshal
    {
        public static unsafe ref T AsRef<T>(DifferentMemory memory) where T : allows ref struct
        {
            if (memory.Length != Unsafe.SizeOf<T>())
            {
                throw new System.Exception("TODO");
            }

            fixed (byte* pointer = memory)
            {
                return ref *(T*)pointer;
            }
        }
    }
}
