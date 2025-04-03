namespace V2.Fx.Collections
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class LinkedList2UnitTests
    {

        private static int Finalized = 0;

        public class CollectedTest
        {
            public CollectedTest(int value)
            {
                Value = value;
            }

            public CollectedTest()
            {
                this.Value = 0;
            }

            ~CollectedTest()
            {
                Interlocked.Increment(ref Finalized);
            }

            public int Value { get; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StructCollectedTest
        {
            public StructCollectedTest(CollectedTest value, string something)
            {
                /*this.Value = value;
                Something = something;
                this.AnotherValue = 1;*/
            }

            ////public string Something { get; }

            public int AnotherValue;

            ////public int AnotherValue2;

            public object Object;

            public object Object2;

            ////public CollectedTest Value { get; set; }
        }

        public struct StructCollectedTest2<T>
        {
            public StructCollectedTest2(CollectedTest value, string something)
            {
                /*this.Value = value;
                Something = something;
                this.AnotherValue = 1;*/
            }

            ////public string Something { get; }

            public int AnotherValue;

            ////public int AnotherValue2;

            public T Object;

            public T Object2;

            ////public CollectedTest Value { get; set; }
        }

        //// https://blog.tchatzigiannakis.com/changing-an-objects-type-at-runtime-in-c-sharp/

        //// https://blog.adamfurmanek.pl/2016/05/07/custom-memory-allocation-in-c-part-3/

        [TestMethod]
        public void Test()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Finalized = 0;

            StructCollectedTest test2;

            var list = new LinkedList2<StructCollectedTest>(stackalloc byte[0]);

            for (int i = 0; i < 10; ++i)
            {
                var finalizable = new CollectedTest(67);
                if (i == 5)
                {
                    for (int j = 0; j < 1; ++j)
                    {
                        unsafe
                        {
                            var bytePointer = stackalloc byte[24];
                            bytePointer[16] = 42;

                            CollectedTest* finalizablePointer = &finalizable;
                            var asBytes = (byte*)finalizablePointer;
                            for (int k = 0; k < 8; ++k)
                            {
                                bytePointer[k] = asBytes[k];
                            }

                            var testPointer = (StructCollectedTest*)bytePointer;
                            test2 = *testPointer;
                        }


                        /*unsafe
                        {
                            var size = sizeof(StructCollectedTest);
                        }

                        Span<byte> memory = stackalloc byte[Unsafe.SizeOf<StructCollectedTest>()];

                        var test = new StructCollectedTest(new CollectedTest(), i.ToString());
                        V2.Fx.Runtime.InteropServices.MemoryMarshal.Write(memory, test);
                        var bytes = Unsafe.ReadUnaligned<StructCollectedTest>(ref MemoryMarshal.GetReference(memory));*/
                    }
                }
            }
            
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(0, Finalized);
        }
    }
}
