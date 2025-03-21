namespace V2.Fx
{
    using System;

    [TestClass]
    public sealed class BetterReadOnlySpanUnitTests
    {
        [TestMethod]
        public void FromInstance()
        {
            var span = BetterReadOnlySpan.FromInstance(42);

            Span<byte> zeroed = stackalloc byte[100];
            zeroed.Clear();

            Assert.AreEqual(42, span[0]);
        }


        public sealed class Test1
        {
            public Test1(ReadOnlySpan<byte> span)
            {
            }

            public static implicit operator Test1(Span<byte> span)
            {
                return new Test1(span);
            }
        }

        public sealed class Test2
        {
            public static implicit operator Test2(Test1 test)
            {
                return new Test2();
            }
        }

        [TestMethod]
        public void Test()
        {
            Test1 test1 = stackalloc byte[10];

            BetterReadOnlySpan<byte> span = stackalloc byte[10];
            BetterReadOnlySpan<Test1> span2 = stackalloc byte[10];
        }
    }
}
