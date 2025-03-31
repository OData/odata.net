namespace V2.Fx.Collections
{
    using System;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LinkedList2UnitTests
    {

        private static int Finalized = 0;

        public class CollectedTest
        {
            ~CollectedTest()
            {
                Interlocked.Increment(ref Finalized);
            }
        }

        public struct StructCollectedTest
        {
            public StructCollectedTest(CollectedTest value, string something)
            {
                this.Value = value;
                Something = something;
                this.AnotherValue = 1;
            }

            public string Something { get; }

            public int AnotherValue;

            public CollectedTest Value { get; }
        }

        [TestMethod]
        public void Test()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finalized = 0;

            var list = new LinkedList2<StructCollectedTest>(stackalloc byte[0]);

            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[list.MemorySize];
                list.Append(new StructCollectedTest(new CollectedTest(), i.ToString()), memory);
            }
            
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(0, Finalized);
        }
    }
}
