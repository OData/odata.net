namespace V2.Fx
{
    using System;

    using V2.Fx.Runtime.InteropServices;

    public static class SpanEx
    {
        public static SpanEx<byte> FromMemory(ByteSpan memory)
        {
            return SpanEx<byte>.Create(memory);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memory"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the length of <paramref name="memory"/> is not the same as the product of <paramref name="length"/> and the
        /// <see langword="sizeof"/> <typeparamref name="T"/>
        /// </exception>
        public static SpanEx<T> FromMemory<T>(ByteSpan memory, int length) where T : allows ref struct
        {
            return SpanEx<T>.Create(memory, length);
        }

        public static SpanEx<T> FromInstance<T>(scoped ref T value) where T : allows ref struct
        {
            return MemoryMarshal.CreateSpan(ref value, 1);
        }
    }
}
