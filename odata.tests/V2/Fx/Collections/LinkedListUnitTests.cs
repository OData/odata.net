namespace V2.Fx.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices.Marshalling;
    using Microsoft.VisualBasic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA2014 // Do not use stackalloc in loops

    public static class AllocationPlayground
    {
        public static void AllocateNodes()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize * 3]; // allocate memory for 3 nodes, we intend to use that much
            list.Append(42, memory);
            list.TryAppend(67);
            list.TryAppend(96);
            if (!list.TryAppend(11))
            {
                memory = stackalloc byte[list.MemorySize * 5]; // we need more memory...
                list.Append(11, memory);
            }

            for (int i = 0; i < 10; ++i)
            {
                if (!list.TryAppend(i))
                {
                    memory = stackalloc byte[list.MemorySize * 5];
                    list.Append(i, memory);
                }
            }

            memory = stackalloc byte[list.MemorySize * 10];
            list.Append(-1, memory);
            memory = stackalloc byte[list.MemorySize * 10];
            list.Append(-2, memory);
            //// TODO what happens to the memory here? do we just waste 9 nodes of the first allocation?
        }

        public static bool TryAppend<T>(this LinkedList<T> list, T value)
        {
            return true;
        }

        public static void AllocateNodes2()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            list.TryAppend2(42); //// TODO should return false

            ByteSpan memory = stackalloc byte[list.MemorySize * 5];
            list.TryAllocate2(memory); //// TODO should return true, but what if it returns false?

            list.TryAppend2(42); //// TODO should return true, but what if it returns false?
        }

        public static bool TryAppend2<T>(this LinkedList<T> list, T value)
        {
            return true;
        }

        public static bool TryAllocate2<T>(this LinkedList<T> list, ByteSpan memory)
        {
            return true;
        }

        public static void AllocateNodes3()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize * 5];
            var appender = list.Allocate3(memory);

            appender.TryAppend(42);

            memory = stackalloc byte[list.MemorySize * 2];
            appender = list.Allocate3(memory); //// TODO what happens to the unused memory from the existing appender?
        }

        public static LinkedListAppender<T> Allocate3<T>(this LinkedList<T> list, ByteSpan memory)
        {
            return new LinkedListAppender<T>();
        }

        public ref struct LinkedListAppender<T> where T : allows ref struct
        {
            public bool TryAppend(T value)
            {
                return true;
            }
        }

        public static void AllocateNodes4()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize * 2];

            list.TryAppend4(42); // should return false
            list.TryAppend4(42, memory); // should return true
            memory = stackalloc byte[list.MemorySize * 2];
            list.TryAppend4(67, memory); // should return false
            list.TryAppend4(96, memory); // should return true;

            var list2 = new LinkedList<int>(stackalloc byte[0]);

            ByteSpan memory2 = stackalloc byte[list2.MemorySize * 3];
            for (int i = 0; i < 10; ++i)
            {
                if (list2.TryAppend4(i, memory2))
                {
                    memory2 = stackalloc byte[list2.MemorySize * 3];
                    // if the loop were to end right now, we would have 5 nodes worth of memory unused
                }
            }

            var list3 = new LinkedList<int>(stackalloc byte[0]);

            for (int i = 0; i < 10; ++i)
            {
                if (!list3.TryAppend4(i))
                {
                    ByteSpan memory3 = stackalloc byte[list3.MemorySize * 3];
                    list3.TryAppend4(i, memory3);
                    // if the loop were to end right now, we would have 2 nodes worth of memory unused; this is the cost of doing business with this kind of pre-allocation though
                    // but what if this call returns `false`?
                }
            }

            var list4 = new LinkedList<int>(stackalloc byte[0]);
            ByteSpan memory4 = stackalloc byte[list4.MemorySize * 3];
            for (int i = 0; i < 10; ++i)
            {
                if (!list4.TryAppend4(i))
                {
                    if (list4.TryAppend4(i, memory4))
                    {
                        // only if `memory4` actually got used do we allocate some more
                        memory4 = stackalloc byte[list4.MemorySize * 3];
                    }
                }
            }
        }

        public static bool TryAppend4<T>(this LinkedList<T> list, T value, ByteSpan memory)
        {
            // true if `memory` was used, false if we had enough existing memory
            return true;
        }

        public static bool TryAppend4<T>(this LinkedList<T> list, T value)
        {
            // `true` if we had memory available for the operation, false otherwise (and therefore the append didn't happen)
            return true;
        }
    }

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
            var list = new LinkedList<Wrapper<int>>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(new Wrapper<int>(-1), memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[list.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 11), list);

            for (int i = 10; i < 20; ++i)
            {
                memory = stackalloc byte[list.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 21), list);
        }

        [TestMethod]
        public void Append()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(-1, memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[list.MemorySize]; //// TODO memorysize shouldn't be static; this is probably easy if you allow an empty list
                list.Append(i, memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 11), list);

            for (int i = 10; i < 20; ++i)
            {
                memory = stackalloc byte[list.MemorySize];
                list.Append(i, memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 21), list);
        }

        [TestMethod]
        public void EnumerateReturnedList()
        {
            var list = new LinkedList<Wrapper<int>>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(new Wrapper<int>(-1), memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[list.MemorySize];
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
            var list = new LinkedList<Wrapper<int>>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(new Wrapper<int>(-1), memory);

            AssertEnumerable(new[] { -1 }, list);

            memory = stackalloc byte[list.MemorySize];
            var newList = Foo(list, memory);

            AssertEnumerable(new[] { -1, 42 }, list); //// TODO if `list` is passed by value, shouldn't this still only have 1 element
            AssertEnumerable(new[] { -1, 42 }, newList);

            Span<byte> zeroed = stackalloc byte[100];
            zeroed.Clear();

            AssertEnumerable(new[] { -1, 42 }, list); //// TODO if `list` is passed by value, shouldn't this still only have 1 element
            AssertEnumerable(new[] { -1, 42 }, newList);

            //// TODO the by-value vs by-ref thing aside, what happens if you append more elements
        }

        private static LinkedList<Wrapper<int>> Foo(LinkedList<Wrapper<int>> list, ByteSpan memory)
        {
            list.Append(new Wrapper<int>(42), memory);
            return list;
        }

        [TestMethod]
        public void UpdateValue()
        {
            //// TODO this test is to demonstrate the behavior `LinkedList<T>.Enumerator.Current` returning by ref; that isn't necessarily the correct behavior

            var list = new LinkedList<Wrapper<int>>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(new Wrapper<int>(-1), memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[list.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }

            var enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            enumerator.MoveNext();
            enumerator.Current = new Wrapper<int>(42);

            AssertEnumerable(new[] { -1, 42, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void AppendToEmptyList()
        {
            var list = new LinkedList<Wrapper<int>>(stackalloc byte[0]);

            for (int i = 0; i < 10; ++i)
            {
                ByteSpan memory = stackalloc byte[list.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }

            AssertEnumerable(Enumerable.Range(0, 10), list);
        }

        [TestMethod]
        public void EmptyList()
        {
            var list = new LinkedList<Wrapper<int>>(stackalloc byte[0]);

            AssertEnumerable(Enumerable.Empty<int>(), list);
        }

        [TestMethod]
        public void DefaultList()
        {
            var list = new LinkedList<Wrapper<int>>();

            AssertEnumerable(Enumerable.Empty<int>(), list);
        }

        [TestMethod]
        public void AppendSpan()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            Span<int> ints = stackalloc int[] { 1, 2, 3, 4 };
            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(SpanEx.FromSpan(ints), memory);

            AssertEnumerable(new[] { 1, 2, 3, 4 }, list);
        }

        [TestMethod]
        public void AppendSingle()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(42, memory);

            AssertEnumerable(new[] { 42 }, list);
        }

        [TestMethod]
        public void StitchStackAndHeap()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            Span<int> stack = stackalloc int[] { 1, 2, 3, 4 };
            Span<int> heap = new[] { 5, 6, 7, 8, 9 };

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(SpanEx.FromSpan(stack), memory);

            memory = stackalloc byte[list.MemorySize];
            list.Append(SpanEx.FromSpan(heap), memory);

            AssertEnumerable(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, list);
        }

        [TestMethod]
        public void AppendStackHeapSingle()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            Span<int> stack = stackalloc int[] { 1, 2, 3, 4 };
            Span<int> heap = new[] { 5, 6, 7, 8, 9 };

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(SpanEx.FromSpan(stack), memory);

            memory = stackalloc byte[list.MemorySize];
            list.Append(SpanEx.FromSpan(heap), memory);

            memory = stackalloc byte[list.MemorySize];
            list.Append(42, memory);

            AssertEnumerable(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 42 }, list);
        }

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
