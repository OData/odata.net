namespace V2.Fx.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualBasic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA2014 // Do not use stackalloc in loops
    [TestClass]
    public sealed class LinkedListUnitTests
    {
        private readonly ref struct Wrapper<T> where T : allows ref struct
        {
            public Wrapper(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        [TestMethod]
        public void AppendRefStruct()
        {
            Span<byte> memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            var list = new LinkedList<Wrapper<int>>(new Wrapper<int>(-1), memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 11), list);

            for (int i = 10; i < 20; ++i)
            {
                memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 21), list);
        }

        [TestMethod]
        public void Append()
        {
            Span<byte> memory = stackalloc byte[LinkedList<int>.MemorySize];
            var list = new LinkedList<int>(-1, memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[LinkedList<int>.MemorySize]; //// TODO memorysize shouldn't be static; this is probably easy if you allow an empty list
                list.Append(i, memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 11), list);

            for (int i = 10; i < 20; ++i)
            {
                memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
                list.Append(i, memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 21), list);
        }

        [TestMethod]
        public void EnumerateReturnedList()
        {
            Span<byte> memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            var list = new LinkedList<Wrapper<int>>(new Wrapper<int>(-1), memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 11), list);

            var newList = PassListThrough(list);
            AssertEnumerable(Enumerable.Range(-1, 11), newList);
        }

        private static LinkedList<Wrapper<int>> PassListThrough(LinkedList<Wrapper<int>> list)
        {
            return list;
        }

        [TestMethod]
        public void AddElementToListInCallee()
        {
            Span<byte> memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            var list = new LinkedList<Wrapper<int>>(new Wrapper<int>(-1), memory);

            AssertEnumerable(new[] { -1 }, list);

            memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            var newList = Foo(list, memory);

            AssertEnumerable(new[] { -1, 42 }, list); //// TODO if `list` is passed by value, shouldn't this still only have 1 element
            AssertEnumerable(new[] { -1, 42 }, newList);

            Span<byte> zeroed = stackalloc byte[100];
            zeroed.Clear();

            AssertEnumerable(new[] { -1, 42 }, list); //// TODO if `list` is passed by value, shouldn't this still only have 1 element
            AssertEnumerable(new[] { -1, 42 }, newList);

            //// TODO the by-value vs by-ref thing aside, what happens if you append more elements
        }

        private static LinkedList<Wrapper<int>> Foo(LinkedList<Wrapper<int>> list, Span<byte> memory)
        {
            list.Append(new Wrapper<int>(42), memory);
            return list;
        }

        /*[TestMethod]
        public void EmptyList()
        {
            var list = new LinkedList<Wrapper<int>>();

            AssertEnumerable(Enumerable.Empty<int>(), list);
        }

        [TestMethod]
        public void AppendToEmptyList()
        {
            //// TODO get these working
            var list2 = new LinkedList<Wrapper<int>>();
            Span<byte> memory2 = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            list2.Append(new Wrapper<int>(67), memory2);

            LinkedList<Wrapper<int>> list3 = default;
            Span<byte> memory3 = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            list3.Append(new Wrapper<int>(67), memory3);

            Span<byte> memory4 = stackalloc byte[0];
            var list4 = new LinkedList<Wrapper<int>>(memory4);
            memory4 = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            list4.Append(new Wrapper<int>(98), memory4);

            Span<byte> memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            var list = new LinkedList<Wrapper<int>>(memory);

            memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            list.Append(new Wrapper<int>(42), memory);

            AssertEnumerable(new[] { 42 }, list);
        }

        private static LinkedList<T> Create<T>() where T : allows ref struct
        {
            Span<byte> memory4 = stackalloc byte[0];
            LinkedList<T> list4 = new LinkedList<T>(memory4);
            return list4;
        }*/

        private static void AssertEnumerable<T>(IEnumerable<T> expected, LinkedList<Wrapper<T>> actual) //// TODO add allows ref struct constraint
        {
            AssertEnumerable(expected, actual, wrapper => wrapper.Value);
        }

        private static void AssertEnumerable<T>(IEnumerable<T> expected, LinkedList<T> actual)
        {
            AssertEnumerable(expected, actual, _ => _);
        }

        private static void AssertEnumerable<TValue, TWrapper>(IEnumerable<TValue> expected, LinkedList<TWrapper> actual, Func<TWrapper, TValue> selector) where TWrapper : allows ref struct //// TODO add allows ref struct constraint to TValue
        {
            using (var expectedEnumerator = expected.GetEnumerator())
            {
                if (!expectedEnumerator.MoveNext())
                {
                    foreach (var actualElement in actual)
                    {
                        Assert.Fail();
                    }
                }
                else
                {
                    var expectedHasAnotherElement = true;
                    foreach (var actualElement in actual)
                    {
                        Assert.IsTrue(expectedHasAnotherElement);

                        var value = selector(actualElement);
                        Assert.AreEqual(expectedEnumerator.Current, value);
                        expectedHasAnotherElement = expectedEnumerator.MoveNext();
                    }

                    Assert.IsFalse(expectedHasAnotherElement);
                }
            }
        }
    }
#pragma warning restore CA2014 // Do not use stackalloc in loops
}
