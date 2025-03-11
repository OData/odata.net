namespace odata.tests
{
    using System.Runtime.CompilerServices;

    using Fx;
    using Fx.Collections;

#pragma warning disable CA2014 // Do not use stackalloc in loops
    [TestClass]
    public sealed class LinkedListNodeTests
    {
        //// TODO what about a mutable type

        private readonly ref struct Wrapper<T> where T : allows ref struct
        {
            public Wrapper(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        [TestMethod]
        public void CreateListAndEnumerateWithRefStruct()
        {
            var list = new LinkedListNode<Wrapper<int>>(BetterSpan.FromInstance(new Wrapper<int>(-1)));
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<Wrapper<int>>>()];
                var wrapper = new Wrapper<int>(i);
                list = list.Append(BetterSpan.FromInstance(wrapper), memory);

            }
        }
    }
#pragma warning restore CA2014 // Do not use stackalloc in loops
}
