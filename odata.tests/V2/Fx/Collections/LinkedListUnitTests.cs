namespace V2.Fx.Collections
{
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

            var expectedValue = -1;
            foreach (var element in list)
            {
                Assert.AreEqual(expectedValue, element.Value);
                ++expectedValue;
            }

            for (int i = 10; i < 20; ++i)
            {
                memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }

            expectedValue = -1;
            foreach (var element in list)
            {
                Assert.AreEqual(expectedValue, element.Value);
                ++expectedValue;
            }
        }

        private static void AssertEnumerable<T>(IEnumerable<T> expected, LinkedList<Wrapper<T>> actual) //// TODO add allows ref struct constraint
        {
            AssertEnumerable(expected, actual, wrapper => wrapper.Value);
        }

        private static void AssertEnumerable<TValue, TWrapper>(IEnumerable<TValue> expected, LinkedList<TWrapper> actual, Func<TWrapper, TValue> selector) where TWrapper : allows ref struct //// TODO add allows ref struct constraint to TValue
        {
            using (var expectedEnumerator = expected.GetEnumerator())
            {
                var expectedMoved = expectedEnumerator.MoveNext();

                var actualHasElements = false;
                foreach (var element in actual)
                {
                    actualHasElements = true;
                    var value = selector(element);
                    Assert.AreEqual(expectedEnumerator.Current, value);
                }

                //// TODO what if there's a different number of elements?

                Assert.AreEqual(expectedMoved, actualHasElements);
            }
        }
    }
#pragma warning restore CA2014 // Do not use stackalloc in loops
}
