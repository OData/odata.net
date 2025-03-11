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

        private LinkedListNode<Wrapper<int>> CreateListAndEnumerateWithRefStruct2()
        {
            var list = new LinkedListNode<Wrapper<int>>(BetterSpan.FromInstance(new Wrapper<int>(-1)));
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<Wrapper<int>>>()];
            var wrapper = new Wrapper<int>(0);
            list = list.Append(BetterSpan.FromInstance(wrapper), memory);
            return list;
        }

        private void Test()
        {
            Span<byte> span = stackalloc byte[4];
            var betterspan = BetterSpan.FromMemory<string>(span, 1);
        }

        private BetterSpan<string> Test2()
        {
            Span<byte> span = stackalloc byte[4];
            var betterspan = BetterSpan.FromMemory<string>(span, 1);

            //// THIS IS A GOOD THING
            return betterspan;
        }

        private LinkedListNode<int> Test3()
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
            return list;
        }

        private LinkedListNode<int> Test4(Span<byte> memory)
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
            Unsafe2.Copy(memory, in list);
            return list;
        }

        private BetterSpan<LinkedListNode<int>> Test5()
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
            Unsafe2.Copy(memory, in list);

            var nextValue = BetterSpan.FromInstance(67);
            var previousNode = BetterSpan.FromMemory<LinkedListNode<int>>(memory, 1);

            //// THIS IS A GOOD THING
            return previousNode;
        }

        private LinkedListNode<int> Test6()
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
            Unsafe2.Copy(memory, in list);

            var nextValue = BetterSpan.FromInstance(67);
            var previousNode = BetterSpan.FromMemory<LinkedListNode<int>>(memory, 1);
            list = new LinkedListNode<int>(nextValue, previousNode);

            //// THIS IS A GOOD THING
            return list;
        }

        private LinkedListNode<int> Test7()
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
            list = list.Append(BetterSpan.FromInstance(67), memory);

            //// THIS IS A GOOD THING
            return list;
        }
    }
#pragma warning restore CA2014 // Do not use stackalloc in loops
}
