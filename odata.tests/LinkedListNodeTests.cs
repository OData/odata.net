namespace odata.tests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices.Marshalling;
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
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<Wrapper<int>>>()];
            var list2 = list.Append(new Wrapper<int>(0), memory);
            for (int i = 1; i < 10; ++i)
            {
                Span<byte> memory2 = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<Wrapper<int>>>()];
                list2 = list2.Append(new Wrapper<int>(i), memory2);
            }

            //// TODO these are still backwards...
            var expected = 9;
            foreach (var element in list2)
            {
                Assert.AreEqual(expected, element.Value);
                --expected;
            }

            Assert.AreNotEqual(9, expected);
        }

        [TestMethod]
        public void CreateListAndEnumerateWithRefStruct3()
        {
            var originalWrapper = new Wrapper<int>(-1);
            var betterSpan = BetterSpan.FromInstance(originalWrapper);
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode3<Wrapper<int>>>()];
            var list = new LinkedListNode3<Wrapper<int>>(betterSpan, BetterSpan<LinkedListNode3<Wrapper<int>>>.CreateEmpty(memory));
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> linkMemory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<Wrapper<int>>>()];
                Span<byte> valueMemory = stackalloc byte[Unsafe.SizeOf<Wrapper<int>>()];
                Unsafe2.Copy(valueMemory, new Wrapper<int>(i));
                list = list.Append(BetterSpan.FromMemory<Wrapper<int>>(valueMemory, 1), linkMemory);
            }

            //// TODO these are still backwards...
            var expected = 9;
            foreach (var element in list)
            {
                Assert.AreEqual(expected, element.Value);
                --expected;
            }

            Assert.AreNotEqual(9, expected);
        }

        [TestMethod]
        public void CreateListAndEnumerateWithStruct3()
        {
            var originalWrapper = -1;
            var betterSpan = BetterSpan.FromInstance(originalWrapper);
            Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode3<Wrapper<int>>>()];
            var list = new LinkedListNode3<int>(betterSpan, BetterSpan<LinkedListNode3<int>>.CreateEmpty(memory));
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> linkMemory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<Wrapper<int>>>()];
                Span<byte> valueMemory = stackalloc byte[Unsafe.SizeOf<Wrapper<int>>()];
                Unsafe2.Copy(valueMemory, i);
                list = list.Append(BetterSpan.FromMemory<int>(valueMemory, 1), linkMemory);
            }

            Span<byte> linkMemory2 = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<Wrapper<int>>>()];
            var newValue = BetterSpan.FromSpan(new Span<int>(new[] { 10 }));
            list = list.Append(newValue, linkMemory2);

            //// TODO these are still backwards...
            var expected = 10;
            foreach (var element in list)
            {
                Assert.AreEqual(expected, element);
                --expected;
            }

            Assert.AreNotEqual(9, expected);
        }

        /*[TestMethod]
        public void CreateListAndEnumerateWithRefStruct4()
        {
            var originalWrapper = new Wrapper<int>(-1);
            var betterSpan = BetterSpan.FromInstance3(originalWrapper);
            var list = new LinkedListNode4<Wrapper<int>>(betterSpan, new BetterSpan2<LinkedListNode4<Wrapper<int>>>());
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> linkMemory = stackalloc byte[Unsafe.SizeOf<LinkedListNode4<Wrapper<int>>>()];
                var wrapper = new Wrapper<int>(i);
                list = list.Append(new BetterSpan2<Wrapper<int>>(wrapper), linkMemory);

                BetterSpan2<byte> testing = stackalloc byte[10];
                Span<byte> testing2 = stackalloc byte[10];
            }

            //// TODO these are still backwards...
            var expected = 9;
            foreach (var element in list)
            {
                Assert.AreEqual(expected, element.Value);
                --expected;
            }

            Assert.AreNotEqual(9, expected);
        }*/

        [TestMethod]
        public void Stackframes()
        {
            var container = new Container<int>(42);
            Span<Container<int>> span = new Span<Container<int>>(ref container);
            container = new Container<int>(67);

            Assert.AreEqual(67, span[0].Value);
        }

        public struct Container<T>
        {
            public Container(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        [TestMethod]
        public void Stackframes2()
        {
            var list = new NewNode<int>(-1);
            Span<byte> linkMemory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<Wrapper<int>>>()];
            var list2 = list.Append(0, linkMemory);
            for (int i = 0; i < 10; ++i)
            {
                linkMemory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<Wrapper<int>>>()];
                list2 = list2.Append(i, linkMemory);
            }

            var expected = 9;
            foreach (var element in list2)
            {
                Assert.AreEqual(expected, element);
                --expected;
            }

            Assert.AreNotEqual(9, expected);
        }

        public readonly ref struct NewSpan<T> where T : allows ref struct
        {
            private readonly Span<byte> memory;
            private readonly int length;

            public NewSpan(Span<byte> memory, int length)
            {
                this.memory = memory;
                this.length = length;
            }

            public T this[int index]
            {
                get
                {
                    var slice = this.memory.Slice(index * Unsafe.SizeOf<T>());
                    return Unsafe.As<Span<byte>, T>(ref slice);
                }
            }

            public int Length
            {
                get
                {
                    return this.length;
                }
            }
        }

        public readonly ref struct NewNode<T>
        {
            private readonly NewSpan<NewNode<T>> previous;

            private readonly T value;

            public NewNode(T value)
                : this(value, default)
            {
            }

            private NewNode(T value, NewSpan<NewNode<T>> previous)
            {
                this.value = value;
                this.previous = previous;
            }

            public NewNode<T> Append(T value, Span<byte> linkMemory)
            {
                var self = this;
                Unsafe2.Copy(linkMemory, self);

                var span = new NewSpan<NewNode<T>>(linkMemory, 1);
                return new NewNode<T>(value, span);
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private NewNode<T> node;

                private bool hasMoved;

                public Enumerator(NewNode<T> node)
                {
                    this.node = node;

                    this.hasMoved = false;
                }

                public T Current
                {
                    get
                    {
                        if (!this.hasMoved)
                        {
                            throw new Exception("TODO");
                        }

                        return node.value;
                    }
                }

                public bool MoveNext()
                {
                    if (!this.hasMoved)
                    {
                        this.hasMoved = true;
                        return true;
                    }

                    if (this.node.previous.Length == 0)
                    {
                        return false;
                    }


                    this.node = node.previous[0];
                    return true;
                }
            }
        }

        [TestMethod]
        public void Stackframes3()
        {
            var list = new LinkedListNode5<Wrapper<int>>(new Wrapper<int>(-1));
            for (int i = 0; i < 10; ++i)
            {
                list = list.Append(new Wrapper<int>(i));
            }

            var expected = 9;
            foreach (var element in list)
            {
                Assert.AreEqual(expected, element.Value);
                --expected;
            }

            Assert.AreNotEqual(9, expected);
        }

        [TestMethod]
        public void LinkedList7()
        {
            Span<byte> memory = stackalloc byte[LinkedList7<Wrapper<int>>.MemorySize];
            var list = new LinkedList7<Wrapper<int>>(new Wrapper<int>(-1), memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[LinkedList7<Wrapper<int>>.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
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

        /*public static class V2
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
                var list = new LinkedListNode2<int>(42);
                Unsafe2.Copy(memory, in list);
                return list;
            }

            private static BetterSpan<LinkedListNode2<int>> Test5()
            {
                var list = new LinkedListNode2<int>(42);
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                Unsafe2.Copy(memory, in list);

                var nextValue = BetterSpan.FromInstance(67);
                var previousNode = BetterSpan.FromMemory<LinkedListNode2<int>>(memory, 1);

                //// THIS IS A GOOD THING
                return previousNode;
            }

            private static LinkedListNode2<int> Test6()
            {
                var list = new LinkedListNode2<int>(42);
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                Unsafe2.Copy(memory, in list);

                var nextValue = 67;
                var previousNode = BetterSpan.FromMemory<LinkedListNode2<int>>(memory, 1);
                list = new LinkedListNode2<int>(nextValue, previousNode);

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test7()
            {
                var list = new LinkedListNode2<int>(42);
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                list = list.Append(67, memory);

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test8()
            {
                var list = new LinkedListNode2<int>(42);
                for (int i = 0; i < 10; ++i)
                {
                    Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                    Unsafe2.Copy(memory, in list);

                    var nextValue = i;
                    var previousNode = BetterSpan.FromMemory<LinkedListNode2<int>>(memory, 1);
                    list = new LinkedListNode2<int>(nextValue, previousNode);
                }

                //// THIS IS A GOOD THING
                return list;
            }

            private static LinkedListNode2<int> Test9()
            {
                //// TODO look at this!

                var list = new LinkedListNode2<int>(42);
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                LinkedListNode2<int> list2 = list.Append(0, memory);
                for (int i = 1; i < 10; ++i)
                {
                    Span<byte> memory2 = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];
                    list2 = list2.Append(i, memory2);
                }

                //// THIS IS A GOOD THING
                return list2;
            }

            private static void Test10(LinkedListNode2<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];

                //// THIS IS A GOOD THING
                list = list.Append(42, memory);
            }

            private static LinkedListNode2<int> Test11(LinkedListNode2<int> list)
            {
                Span<byte> memory = stackalloc byte[Unsafe.SizeOf<LinkedListNode2<int>>()];

                //// THIS IS A GOOD THING
                return list.Append(42, memory);
            }

            private static LinkedListNode2<int> Test12(LinkedListNode2<int> list)
            {
                return list;
            }

            private static LinkedListNode2<int> Test13()
            {
                var list = new LinkedListNode2<int>(42);
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
                var list = new LinkedListNode2<int>(42);

                //// TODO this should be allowed
                //// this doesn't work currently because `linkedlistnode2.previous` will get initialized to its default, and since it has a `T*` field, it might have a pointer to somewhere in this stackframe, and so when we return `list`, the first node in the list (in this case, the only node) will have a reference to a pointer to the stackframe that has already been popped off; of course, *we* know that the pointer in this case will be `0`, but the compiler doesn't have a way to know that
                //// TODO i don't think the above is true, i think it's because the constructor takes in a `in` parameter, so they are receiving `42` *by reference to its location in the stackframe*, so when we return, that referenced value is gone
                return list;
            }
        }*/
    }
#pragma warning restore CA2014 // Do not use stackalloc in loops
}
