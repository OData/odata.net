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
                list = list.Append(new Wrapper<int>(i), memory);
            }
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

        private LinkedListNode<int> Test8()
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                Unsafe2.Copy(memory, in list);

                var nextValue = BetterSpan.FromInstance(i);
                var previousNode = BetterSpan.FromMemory<LinkedListNode<int>>(memory, 1);
                list = new LinkedListNode<int>(nextValue, previousNode);
            }

            //// THIS IS A GOOD THING
            return list;
        }

        private LinkedListNode<int> Test9()
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                list = list.Append(BetterSpan.FromInstance(i), memory);
            }

            //// THIS IS A GOOD THING
            return list;
        }

        private void Test10(LinkedListNode<int> list)
        {
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
            
            //// THIS IS A GOOD THING
            list = list.Append(BetterSpan.FromInstance(42), memory);
        }

        private LinkedListNode<int> Test11(LinkedListNode<int> list)
        {
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];

            //// THIS IS A GOOD THING
            return list.Append(BetterSpan.FromInstance(42), memory);
        }

        private LinkedListNode<int> Test12(LinkedListNode<int> list)
        {
            return list;
        }

        private LinkedListNode<int> Test13()
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                list = list.Append(i, memory);
            }

            //// THIS IS A GOOD THING
            return list;
        }

        private LinkedListNode<int> Test14()
        {
            var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));

            //// TODO this should be allowed
            return list;
        }
    }
#pragma warning restore CA2014 // Do not use stackalloc in loops
}
