using System;
using V2.Fx;
using V2.Fx.Collections;

namespace V3
{
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
                        // if the loop were to end right now, we would have 5 nodes worth of memory unused
                    }
                }
            }

            var list5 = new LinkedList<int>(stackalloc byte[0]);
            ByteSpan memory5 = stackalloc byte[0];
            var reallocate = true;
            for (int i = 0; i < 10; ++i)
            {
                if (!list5.TryAppend4(i))
                {
                    if (reallocate)
                    {
                        memory5 = stackalloc byte[list.MemorySize * 3];
                    }

                    reallocate = list5.TryAppend4(i, memory5);
                    // if the loop were to end right now, we would have 2 nodes worth of memory unused; this is the cost of doing business with this kind of pre-allocation though
                }
            }
        }

        public static bool TryAppend4<T>(this LinkedList<T> list, T value, ByteSpan memory)
        {
            //// TODO you *might* want to consider if someone invents a stack-based arraypool and that pool has arrays that are not aligned with your node sizes; in that case, you would (currently) be disallowed the use of those buffer, so maybe it makes sense to be able to stitch multiple buffers together if they have some "dangling" memory

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
    public sealed class SpanExPoolTests
    {
        [TestMethod]
        public void Test()
        {
            var numberOfSpans = 2;
            var spanLength = 100;
            ByteSpan memory = stackalloc byte[numberOfSpans * SpanExPool<int>.SizePerSpan(spanLength)];
            var pool = new SpanExPool<int>(memory, spanLength);

            Assert.IsTrue(pool.TryRent(out var first));
            Assert.IsTrue(pool.TryRent(out var second));
            Assert.IsFalse(pool.TryRent(out var third));

            first.Span[0] = 5;
            first.Span[99] = 42;

            for (int i = 0; i < first.Span.Length; ++i)
            {
                Console.WriteLine(first.Span[i]);
            }

            first.Dispose();

            Assert.IsTrue(pool.TryRent(out var fourth));

            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            for (int i = 0; i < fourth.Span.Length; ++i)
            {
                Console.WriteLine(first.Span[i]);
            }
        }
    }

    public ref struct SpanExPool<T> where T : allows ref struct
    {
        private readonly SpanEx<Pointer> array;

        private int next;

        public SpanExPool(ByteSpan totalMemory, int spanLength)
        {
            // you have an array (spanex<spanex<t>>) of pointers (spanex<T>) to elements (T)

            var pointerSize = System.Runtime.CompilerServices.Unsafe.SizeOf<Pointer>();
            var individualElementSize = System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
            var elementsSize = individualElementSize * spanLength;

            var overheadSize = pointerSize + elementsSize;
            if (totalMemory.Length % overheadSize != 0)
            {
                throw new System.Exception("TODO");
            }

            var spanCount = totalMemory.Length / overheadSize;

            var pointersLength = pointerSize * spanCount;
            var pointersMemory = totalMemory.Slice(0, pointersLength);
            var spansMemory = totalMemory.Slice(pointersLength, totalMemory.Length - pointersLength); //// TODO off by 1?

            this.array = SpanEx.FromMemory<Pointer>(pointersMemory, spanCount);
            for (int i = 0; i < this.array.Length; ++i)
            {
                this.array[i] = new Pointer()
                {
                    IsInUse = false,
                    Span = SpanEx.FromMemory<T>(spansMemory.Slice(i * elementsSize, elementsSize), spanLength),
                };
            }

            this.next = 0;
        }

        public static int SizePerSpan(int spanLength)
        {
            var pointerSize = System.Runtime.CompilerServices.Unsafe.SizeOf<Pointer>();
            var individualElementSize = System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
            var elementsSize = individualElementSize * spanLength;

            return pointerSize + elementsSize;
        }

        public bool TryRent(out Disposable rented)
        {
            //// TODO start with `this.next`
            for (int i = 0; i < this.array.Length; ++i)
            {
                ref Pointer pointer = ref this.array[i];
                if (!pointer.IsInUse)
                {
                    pointer.IsInUse = true;
                    rented = new Disposable(SpanEx.FromInstance(ref pointer));
                    return true;
                }
            }

            rented = default;
            return false;
        }

        public ref struct Disposable
        {
            private SpanEx<Pointer> pointer;

            private bool disposed;

            internal Disposable(SpanEx<Pointer> pointer) //// TODO can you make this private? or public in a way that's always useful?
            {
                this.pointer = pointer;
            }

            public SpanEx<T> Span
            {
                get
                {
                    //// TODO it'd be nice to return this by ref
                    //// TODO it'd be better to not even return a spanex<T> but instead for `disposable` to *look* like a `spanex<T>`
                    return this.pointer[0].Span;
                }
            }

            public void Dispose()
            {
                //// TODO this should go in spanex...
                for (int i = 0; i < this.pointer[0].Span.Length; ++i)
                {
                    this.pointer[0].Span[i] = default;
                }

                this.pointer[0].IsInUse = false;
            }
        }

        internal ref struct Pointer //// TODO can you make this private?
        {
            public bool IsInUse;

            public SpanEx<T> Span;
        }
    }

#pragma warning restore CA2014 // Do not use stackalloc in loops
}
