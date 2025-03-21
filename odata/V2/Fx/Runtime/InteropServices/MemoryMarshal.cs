namespace V2.Fx.Runtime.InteropServices
{
    using System;
    using System.Runtime.CompilerServices;

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

        public static unsafe BetterReadOnlySpan<T> CreateReadOnlySpan<T>(scoped in T reference) where T : allows ref struct //// TODO the actual memorymarshal uses `scoped ref readonly`; it also takes a length parameter
        {
            var pointer = Fx.Runtime.CompilerServices.Unsafe.AsPointer(reference);
            return BetterReadOnlySpan.FromMemory<T>(
                new Span<byte>(
                    pointer,
                    Unsafe.SizeOf<T>()),
                1);
        }
    }
}
