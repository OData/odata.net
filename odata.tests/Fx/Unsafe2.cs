namespace Fx
{
    using System.Runtime.CompilerServices;

    public static class Unsafe2
    {
        public static unsafe void Copy<T>(Span<byte> destination, in T source) where T : allows ref struct
        {
            fixed (byte* pointer = destination)
            {
                Unsafe.Copy(pointer, in source);
            }
        }
    }
}
