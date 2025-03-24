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

        public static SpanEx<T> FromInstance<T>(scoped ref T value) where T : allows ref struct //// TODO is `scoped` ok here? //// TODO is there a way to do this without `in`? //// TODO removing either `scoped` or `in` might cause a test to fail, so you should have a memory integrity issue in mind that removing them fixes
        {
            //// TODO when you change this to not be readonly, what should the expectation be of mutating a single instance span? should the original T get updated to? (i.e. should this be `ref` or `in`?) (see what happens when you create a span from the pointer to an instance and then update span[0]) (also check memorymarshal.createspan(ref instance), you may need to change your memorymarshal.createspan implementation)
            return MemoryMarshal.CreateSpan(ref value, 1);
        }
    }
}
