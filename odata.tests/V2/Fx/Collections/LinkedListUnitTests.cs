namespace V2.Fx.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
