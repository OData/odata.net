namespace V2.Fx.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
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

        /*private readonly ref struct Wrapper2<T>
        {
            public Wrapper2(ref T value)
            {
                Value = ref value;
            }

            public readonly ref T Value;
        }

        //// TODO add the memory integrity tests

        class HeapVal
        {
            public HeapVal(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private LinkedList<Wrapper2<int>> ByRefGCCounter()
        {
            var list = new LinkedList<Wrapper2<int>>(stackalloc byte[0]);

            var value = -1;
            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(new Wrapper2<int>(ref value), memory);

            return list;
        }

        private HeapVal something = new HeapVal(42);

        private LinkedList<Wrapper<HeapVal>> ByRefGCCounter2()
        {
            var list = new LinkedList<Wrapper<HeapVal>>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(new Wrapper<HeapVal>(something), memory);

            return list;
        }

        private LinkedList<Wrapper<HeapVal>> ByRefGCCounter3(HeapVal something3)
        {
            var list = new LinkedList<Wrapper<HeapVal>>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(new Wrapper<HeapVal>(something3), memory);

            return list;
        }*/

        /*private LinkedList<HeapVal> ByRefGCCounter4()
        {
            var list = new LinkedList<HeapVal>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(new HeapVal(), memory);

            GC.Collect();

            return list;
        }*/

        public struct Tester()
        {
            public int Value;
        }

        [TestMethod]
        public void Pointers()
        {
            //// TODO send the correct version of this to the team
            Tester a = new Tester();
            a.Value = 42;
            Tester b = a;

            PointersImpl(ref b);

            Assert.AreEqual(100, a.Value);
        }

        public void PointersImpl(ref Tester tester)
        {
            tester.Value = 100;
        }

        private static int Finalized = 0;

        public class CollectedTest
        {
            ~CollectedTest()
            {
                Interlocked.Increment(ref Finalized);
            }
        }

        public readonly struct StructCollectedTest
        {
            public StructCollectedTest(CollectedTest value, string something)
            {
                this.Value = value;
                Something = something;
                this.AnotherValue = 1;
            }
            public string Something { get; }

            public int AnotherValue { get; }

            public CollectedTest Value { get; }
        }

        [TestMethod]
        public unsafe void SpanWithReferences()
        {
            var bytes = stackalloc byte[0];
            Span<StructCollectedTest> span = new Span<StructCollectedTest>(bytes, 0);
        }

        [TestMethod]
        public unsafe void SpanWithReferences2()
        {
            var bytes = stackalloc byte[0];
            Span<CollectedTest> span = new Span<CollectedTest>(bytes, 0);
        }

        class NoReferences()
        {
        }

        [TestMethod]
        public unsafe void SpanWithReferences3()
        {
            var bytes = stackalloc byte[0];
            Span<NoReferences> span = new Span<NoReferences>(bytes, 0);
        }

        [TestMethod]
        public void SpanWithReferences4()
        {
            Span<NoReferences> span = new Span<NoReferences>(new NoReferences[0]);
        }

        [TestMethod]
        public void SpanWithReferences5()
        {
            var span = new Span<CollectedTest>(new CollectedTest[0]);
        }

        [TestMethod]
        public void SpanWithReferences6()
        {
            var span = new Span<StructCollectedTest>(new StructCollectedTest[0]);
        }

        [TestMethod]
        public unsafe void SpanWithReferences7()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finalized = 0;

            Span<byte> bytes = stackalloc byte[System.Runtime.CompilerServices.Unsafe.SizeOf<Span<StructCollectedTest>>()];
            Span<StructCollectedTest> copy = new Span<StructCollectedTest>();

            for (int i = 0; i < 10; ++i)
            {
                var array = new StructCollectedTest[1];
                array[0] = new StructCollectedTest(new CollectedTest(), "Asdf");
                var span = new Span<StructCollectedTest>(array);
                if (i == 0)
                {
                    V2.Fx.Runtime.InteropServices.MemoryMarshal.Write(bytes, span); //// TODO this would normally throw...
                    copy = System.Runtime.CompilerServices.Unsafe.As<Span<byte>, Span<StructCollectedTest>>(ref bytes);
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(9, Finalized);
        }

        [TestMethod]
        public void GarbageCollectionWithStruct()
        {
            //// TODO for some reason, if `value` *is* a ref struct, the handle still will exist and the garbage collector won't its references; this even goes so far as keeping the data on the stack or something
            var thing = new StructCollectedTest(new CollectedTest(), "asdf");
            var @object = System.Runtime.CompilerServices.Unsafe.As<StructCollectedTest, object>(ref thing);

            GarbageCollectionWithStructHelper();
        }

        private static void GarbageCollectionWithStructHelper()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finalized = 0;

            var list = new LinkedList<StructCollectedTest>(stackalloc byte[0]);

            for (int j = 0; j < 10; ++j)
            {
                var @struct = new StructCollectedTest(new CollectedTest(), "asdf");

                ByteSpan memory = stackalloc byte[list.MemorySize];
                list.Append(@struct, memory);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(0, Finalized);
        }

        [TestMethod]
        public void GarbageCollectedWithArray()
        {
            GarbageCollectedWithArrayHelper(false);
            GarbageCollectedWithArrayHelper(true);
        }

        private static void GarbageCollectedWithArrayHelper(bool addToList)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finalized = 0;

            var list = new LinkedList<CollectedTest>(stackalloc byte[0]);

            for (int j = 0; j < 10; ++j)
            {
                Span<CollectedTest> span = new CollectedTest[10];
                for (int i = 0; i < span.Length; ++i)
                {
                    span[i] = new CollectedTest();
                }

                if (addToList)
                {
                    ByteSpan memory = stackalloc byte[list.MemorySize];
                    list.Append(SpanEx.FromSpan(span), memory);
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(0, Finalized);
        }

        [TestMethod]
        public void GarbageCollected()
        {
            GarbageCollectedHelper(true);
            GarbageCollectedHelper(false);
        }

        private static void GarbageCollectedHelper(bool addToList)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finalized = 0;

            var list = new LinkedList<CollectedTest>(stackalloc byte[0]);

            for (int i = 0; i < 10; ++i)
            {
                var foo = new CollectedTest();
                if (addToList)
                {
                    ByteSpan memory = stackalloc byte[list.MemorySize];
                    list.Append(foo, memory);
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (addToList)
            {
                Assert.AreEqual(0, Finalized);
            }
            else
            {
                Assert.AreEqual(9, Finalized);
            }

            list.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (addToList)
            {
                Assert.AreEqual(9, Finalized);
            }
            else
            {
                Assert.AreEqual(9, Finalized);
            }
        }

        /*[TestMethod]
        public void FinalizeTest()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finalized = 0;

            FinalizeTestDelegate();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(1, Finalized);
        }

        private static void FinalizeTestDelegate()
        {
            var foo = new CollectedTest();
        }*/

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
                memory = stackalloc byte[list.MemorySize];
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

        /*[TestMethod]
        public void Append2()
        {
            var list = new LinkedList<int>(stackalloc byte[0]);

            ByteSpan memory = stackalloc byte[list.MemorySize];
            list.Append(-1, memory);

            for (int i = 0; i < 2; ++i)
            {
                memory = stackalloc byte[list.MemorySize];
                list.Append(i, memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 11), list);

            DelegateCall(list);
        }

        private static void DelegateCall(LinkedList<int> list2)
        {
            //// TODO for some reason this isn't compiling, fix it
            for (int i = 10; i < 2; ++i)
            {
                ByteSpan memory = stackalloc byte[list2.MemorySize];
                list2.Append(i, memory);
            }

            AssertEnumerable(Enumerable.Range(-1, 21), list2);
        }*/

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
