namespace odata.tests
{
    using System;
    using System.Runtime.CompilerServices;

    using Fx;
    using Fx.Collections;
    using Newtonsoft.Json.Linq;

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
            var originalWrapper = new Wrapper<int>(-1);
            var orignalValue = BetterSpan.FromInstance2(originalWrapper);
            var list = new LinkedListNode<Wrapper<int>>(orignalValue);
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<Wrapper<int>>>()];
                var wrapper = new Wrapper<int>(i);
                var value = BetterSpan.FromInstance(wrapper);
                list = list.Append(value, memory);

                //// TODO why doesn't this work
                //// list = list.Append(new Wrapper<int>(i), memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }
        }

        [TestMethod]
        public void CreateListAndEnumerateWithRefStruct2()
        {
            var originalWrapper = new Wrapper<int>(-1);
            var list = new LinkedListNode2<Wrapper<int>>(originalWrapper);
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<Wrapper<int>>>()];
                var wrapper = new Wrapper<int>(i);
                list = list.Append(wrapper, memory);

                //// TODO why doesn't this work
                //// list = list.Append(new Wrapper<int>(i), memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }
        }

        /*public static class V1
        {
            private static void Test()
            {
                Span<byte> span = stackalloc byte[4];
                var betterspan = BetterSpan.FromMemory<string>(span, 1);
            }

            private static BetterSpan<string> Test2()
            {
                Span<byte> span = stackalloc byte[4];
                var betterspan = BetterSpan.FromMemory<string>(span, 1);

                //// THIS IS A GOOD THING
                return betterspan;
            }

            private static LinkedListNode<int> Test3()
            {
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                return list;
            }

            private static LinkedListNode<int> Test4(Span<byte> memory)
            {
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                Unsafe2.Copy(memory, in list);
                return list;
            }

            private static BetterSpan<LinkedListNode<int>> Test5()
            {
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                Unsafe2.Copy(memory, in list);

                var nextValue = BetterSpan.FromInstance(67);
                var previousNode = BetterSpan.FromMemory<LinkedListNode<int>>(memory, 1);

                //// THIS IS A GOOD THING
                return previousNode;
            }

            private static LinkedListNode<int> Test6()
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

            private static LinkedListNode<int> Test7()
            {
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];
                list = list.Append(BetterSpan.FromInstance(67), memory);

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode<int> Test8()
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

            private static LinkedListNode<int> Test9()
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

            private static void Test10(LinkedListNode<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];

                //// THIS IS A GOOD THING
                list = list.Append(BetterSpan.FromInstance(42), memory);
            }

            private static LinkedListNode<int> Test11(LinkedListNode<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode<int>>()];

                //// THIS IS A GOOD THING
                return list.Append(BetterSpan.FromInstance(42), memory);
            }

            private static LinkedListNode<int> Test12(LinkedListNode<int> list)
            {
                return list;
            }

            private static LinkedListNode<int> Test13()
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

            private static LinkedListNode<int> Test14()
            {
                var list = new LinkedListNode<int>(BetterSpan.FromInstance(42));

                //// TODO this should be allowed
                return list;
            }

            private static LinkedListNode<int> Test15()
            {
                var betterSpan = BetterSpan.FromSpan(new Span<int>(new[] { 1, 2, 3, 4 }));

                var list = new LinkedListNode<int>(betterSpan);

                //// TODO this should be allowed
                return list;
            }
        }*/

        public static class V2
        {
            private static void Test()
            {
                Span<byte> span = stackalloc byte[4];
                var betterspan = BetterSpan.FromMemory<string>(span, 1);
            }

            private static BetterSpan<string> Test2()
            {
                Span<byte> span = stackalloc byte[4];
                var betterspan = BetterSpan.FromMemory<string>(span, 1);

                //// THIS IS A GOOD THING
                return betterspan;
            }

            private static LinkedListNode2<int> Test3()
            {
                var list = new LinkedListNode2<int>(42);
                return list;
            }

            private static LinkedListNode2<int> Test4(Span<byte> memory)
            {
                var list = new LinkedListNode2<int>(BetterSpan.FromInstance(42));
                Unsafe2.Copy(memory, in list);
                return list;
            }

            private static BetterSpan<LinkedListNode2<int>> Test5()
            {
                var list = new LinkedListNode2<int>(BetterSpan.FromInstance(42));
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                Unsafe2.Copy(memory, in list);

                var nextValue = BetterSpan.FromInstance(67);
                var previousNode = BetterSpan.FromMemory<LinkedListNode2<int>>(memory, 1);

                //// THIS IS A GOOD THING
                return previousNode;
            }

            private static LinkedListNode2<int> Test6()
            {
                var list = new LinkedListNode2<int>(BetterSpan.FromInstance(42));
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                Unsafe2.Copy(memory, in list);

                var nextValue = BetterSpan.FromInstance(67);
                var previousNode = BetterSpan.FromMemory<LinkedListNode2<int>>(memory, 1);
                list = new LinkedListNode2<int>(nextValue, previousNode);

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test7()
            {
                var list = new LinkedListNode2<int>(BetterSpan.FromInstance(42));
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                list = list.Append(BetterSpan.FromInstance(67), memory);

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test8()
            {
                var list = new LinkedListNode2<int>(BetterSpan.FromInstance(42));
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                    Unsafe2.Copy(memory, in list);

                    var nextValue = BetterSpan.FromInstance(i);
                    var previousNode = BetterSpan.FromMemory<LinkedListNode2<int>>(memory, 1);
                    list = new LinkedListNode2<int>(nextValue, previousNode);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test9()
            {
                var list = new LinkedListNode2<int>(BetterSpan.FromInstance(42));
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                    list = list.Append(BetterSpan.FromInstance(i), memory);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static void Test10(LinkedListNode2<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];

                //// THIS IS A GOOD THING
                list = list.Append(BetterSpan.FromInstance(42), memory);
            }

            private static LinkedListNode2<int> Test11(LinkedListNode2<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];

                //// THIS IS A GOOD THING
                return list.Append(BetterSpan.FromInstance(42), memory);
            }

            private static LinkedListNode2<int> Test12(LinkedListNode2<int> list)
            {
                return list;
            }

            private static LinkedListNode2<int> Test13()
            {
                var list = new LinkedListNode2<int>(BetterSpan.FromInstance(42));
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                    list = list.Append(i, memory);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test14()
            {
                var list = new LinkedListNode2<int>(BetterSpan.FromInstance(42));

                //// TODO this should be allowed
                return list;
            }

            private static LinkedListNode2<int> Test15()
            {
                var betterSpan = BetterSpan.FromSpan(new Span<int>(new[] { 1, 2, 3, 4 }));

                var list = new LinkedListNode2<int>(betterSpan);

                //// TODO this should be allowed
                return list;
            }
        }
    }
#pragma warning restore CA2014 // Do not use stackalloc in loops
}
