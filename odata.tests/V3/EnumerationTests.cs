namespace V3
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    using V2.Fx;
    using V2.Fx.Collections;

    [TestClass]
    public sealed class EnumerationTests
    {
        //// TODO iequalitycomparer for ref structs? maybe use reflection for this?
        
        private readonly ref struct Wrapper<T> where T : allows ref struct
        {
            public Wrapper(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        private sealed class Comparer : IEqualityComparer<Wrapper<int>>
        {
            private Comparer()
            {
            }

            public static Comparer Instance { get; } = new Comparer();

            public bool Equals(Wrapper<int> x, Wrapper<int> y)
            {
                return x.Value == y.Value;
            }

            public int GetHashCode([DisallowNull] Wrapper<int> obj)
            {
                return obj.Value.GetHashCode();
            }
        }

        public static unsafe void Copy<T>(ByteSpan destination, in T source, int offset) where T : allows ref struct
        {
            //// TODO if you keep this method, you should have the overload without `offset` delegate to it
            
            var index = offset * System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
            if (offset + System.Runtime.CompilerServices.Unsafe.SizeOf<T>() >= destination.Length)
            {
                throw new Exception("TODO");
            }

            fixed (byte* pointer = destination)
            {
                var indexedPointer = pointer + index;

                System.Runtime.CompilerServices.Unsafe.Copy(indexedPointer, in source);
            }
        }

        private static ReadOnlyArray<TValue> ToArray2<TEnumerable, TEnumerator, TValue>(TEnumerable enumerable, ByteSpan memory, Params<TEnumerable, TValue, TEnumerator> typeParameters) where TEnumerable : IBetterReadOnlyCollection<TEnumerable, TValue, TEnumerator>, allows ref struct where TValue : allows ref struct where TEnumerator : IEnumerator<TValue>, allows ref struct
        {
            return ToArray<TEnumerable, TEnumerator, TValue>(enumerable, memory);
        }

        private static ReadOnlyArray<TValue> ToArray3<TEnumerable, TEnumerator, TValue>(Params<TEnumerable, TValue, TEnumerable> enumerable, ByteSpan memory) where TEnumerable : IBetterReadOnlyCollection<TEnumerable, TValue, TEnumerator>, allows ref struct where TValue : allows ref struct where TEnumerator : IEnumerator<TValue>, allows ref struct
        {
            return ToArray<TEnumerable, TEnumerator, TValue>(enumerable.Self, memory);
        }

        private static ReadOnlyArray<TValue> ToArray<TEnumerable, TEnumerator, TValue>(TEnumerable enumerable, ByteSpan memory) where TEnumerable : IBetterReadOnlyCollection<TEnumerable, TValue, TEnumerator>, allows ref struct where TValue : allows ref struct where TEnumerator : IEnumerator<TValue>, allows ref struct
        {
            if (memory.Length != enumerable.Count * Unsafe.SizeOf<TValue>())
            {
                throw new Exception("TODO");
            }

            var index = 0;
            foreach (var element in enumerable)
            {
                Copy<TValue>(memory, element, index);

                ++index;
            }

            return new ReadOnlyArray<TValue>(memory, enumerable.Count);
        }

        private static void AssertEnumerable2<TValue, TEnumerable, TEnumerator>(TEnumerable expected, TEnumerable actual, IEqualityComparer<TValue> comparer, Params<TEnumerable, TValue, TEnumerator> typeParameters) where TEnumerable : IBetterReadOnlyCollection<TEnumerable, TValue, TEnumerator>, allows ref struct where TValue : allows ref struct where TEnumerator : IEnumerator<TValue>, allows ref struct
        {
            AssertEnumerable<TValue, TEnumerable, TEnumerator>(expected, actual, comparer);
        }

        private static void AssertEnumerable3<TValue, TEnumerable, TEnumerator>(Params<TEnumerable, TValue, TEnumerator> expected, Params<TEnumerable, TValue, TEnumerator> actual, IEqualityComparer<TValue> comparer) where TEnumerable : IBetterReadOnlyCollection<TEnumerable, TValue, TEnumerator>, allows ref struct where TValue : allows ref struct where TEnumerator : IEnumerator<TValue>, allows ref struct
        {
            AssertEnumerable<TValue, TEnumerable, TEnumerator>(expected.Self, actual.Self, comparer);
        }

        private static void AssertEnumerable<TValue, TEnumerable, TEnumerator>(TEnumerable expected, TEnumerable actual, IEqualityComparer<TValue> comparer) where TEnumerable : IBetterReadOnlyCollection<TEnumerable, TValue, TEnumerator>, allows ref struct where TValue : allows ref struct where TEnumerator : IEnumerator<TValue>, allows ref struct
        {
            //// TODO why does this work?
            ByteSpan memory = stackalloc byte[expected.Count * Unsafe.SizeOf<TValue>()];
            var expectedArray = ToArray2(expected, memory, expected.TypeParameters);

            var index = 0;
            foreach (var element in actual)
            {
                Assert.IsTrue(comparer.Equals(element, expectedArray[index]));

                ++index;
            }
        }

        [TestMethod]
        public void Enumeration()
        {
            Span<byte> memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            var list = new LinkedList<Wrapper<int>>(new Wrapper<int>(-1), memory);

            for (int i = 0; i < 10; ++i)
            {
                memory = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
                list.Append(new Wrapper<int>(i), memory);
            }


            Span<byte> memory2 = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
            var list2 = new LinkedList<Wrapper<int>>(new Wrapper<int>(-1), memory2);

            for (int i = 0; i < 10; ++i)
            {
                memory2 = stackalloc byte[LinkedList<Wrapper<int>>.MemorySize];
                list2.Append(new Wrapper<int>(i), memory2);
            }

            //// TODO fix the type inference
            AssertEnumerable3(list.TypeParameters, list2.TypeParameters, Comparer.Instance);
        }

        /*[TestMethod]
        public void Cast()
        {
            Span<byte> memory = stackalloc byte[LinkedList<Dog>.MemorySize];
            var list = new LinkedList<Dog>(new Dog(), memory);

            DoCast(list);

            var array = new Dog[0];
            DoCast2(array);

            var genericList = new System.Collections.Generic.List<Dog>();
            DoCast3(genericList);

            var thing = new Thing<Dog>();
            DoCast4(thing);
        }

        public static void DoCast(LinkedList<Animal> animals)
        {

        }

        public static void DoCast2(Animal[] animals)
        {
        }

        public static void DoCast3(System.Collections.Generic.List<Animal> animals)
        {
        }

        public static void DoCast4(Thing<Animal> thing)
        {
        }

        public interface IThing<out T>
        {
        }

        public sealed class Thing<T> : IThing<T>
        {
        }

        public class Animal
        {
        }

        public class Dog : Animal
        {
        }*/

        public ref struct LinkedList<T> : IBetterReadOnlyCollection<LinkedList<T>, T, LinkedList<T>.Enumerator> where T : allows ref struct
        {
            private SpanEx<LinkedListNode> first;

            private SpanEx<LinkedListNode> current;

            private int count;

            private bool hasValues;

            public LinkedList()
            {
                this.count = 0;
                this.hasValues = false;
            }

            public LinkedList(T value, ByteSpan memory) //// TODO can you use betterspan instead of span? how about readonlyspan?
            {
                //// TODO do you still want this constructor now that empty lists are a thing?
                this.SetFirstValue(value, memory);
            }

            public int ArraySize
            {
                get
                {
                    return this.Count * System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
                }
            }

            private void SetFirstValue(T value, ByteSpan memory) //// TODO can you use betterspan instead of span? how about readonlyspan?
            {
                var firstNode = new LinkedListNode(value);
                V2.Fx.Runtime.InteropServices.MemoryMarshal.Write(memory, firstNode);

                this.first = SpanEx.FromMemory<LinkedListNode>(memory, 1);
                this.current = this.first;

                this.count = 1;
                this.hasValues = true;
            }

            public void Append(T value, ByteSpan memory)
            {
                if (!this.hasValues)
                {
                    this.SetFirstValue(value, memory);
                }
                else
                {
                    var nextNode = new LinkedListNode(value);
                    V2.Fx.Runtime.InteropServices.MemoryMarshal.Write(memory, nextNode);

                    var next = SpanEx.FromMemory<LinkedListNode>(memory, 1);

                    this.current[0].Next = next;

                    this.current = next;

                    ++this.count;
                }
            }

            public static int MemorySize { get; } = System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode>();

            public int Count
            {
                get
                {
                    return this.count;
                }
            }

            public Params<LinkedList<T>, T, Enumerator> TypeParameters
            {
                get
                {
                    return new Params<LinkedList<T>, T, Enumerator>(this);
                }
            }

            internal ref struct LinkedListNode //// TODO can you make this private
            {
                public readonly T Value;

                public SpanEx<LinkedListNode> Next;

                public LinkedListNode(T value)
                {
                    this.Value = value;
                }
            }

            public Enumerator GetEnumerator()
            {
                if (this.hasValues)
                {
                    return new Enumerator(this.first);
                }
                else
                {
                    return new Enumerator();
                }
            }

            public ref struct Enumerator : IEnumerator<T>
            {
                private SpanEx<LinkedListNode> node;

                private bool hasMoved;

                private readonly bool hasValues;

                internal Enumerator(SpanEx<LinkedListNode> node)
                {
                    this.node = node;

                    this.hasMoved = false;
                    this.hasValues = true;
                }

                public T Current //// TODO cna you make this return `ref t`?
                {
                    get
                    {
                        if (!this.hasValues || !this.hasMoved)
                        {
                            throw new Exception("TODO");
                        }

                        return this.node[0].Value;
                    }
                }

                object IEnumerator.Current => throw new NotImplementedException();

                public bool MoveNext()
                {
                    if (!this.hasValues)
                    {
                        return false;
                    }

                    if (!this.hasMoved)
                    {
                        this.hasMoved = true;
                        return true;
                    }

                    var nextNode = this.node[0].Next;
                    if (nextNode.Length != 0)
                    {
                        this.node = nextNode;
                        return true;
                    }

                    return false;
                }

                public void Reset()
                {
                    ////throw new NotImplementedException();
                }

                public void Dispose()
                {
                    ////throw new NotImplementedException();
                }
            }
        }

        public interface IBetterReadOnlyCollection<TSelf, TValue, TEnumerator> where TSelf : allows ref struct where TValue : allows ref struct where TEnumerator : IEnumerator<TValue>, allows ref struct
        {
            int Count { get; }

            TEnumerator GetEnumerator();

            Params<TSelf, TValue, TEnumerator> TypeParameters { get; } //// TODO this requires removing covariance...but if this is really only used for ref structs (why else would it have the enumerator?) then you can't leverage the covariance anyway
        }

        public readonly ref struct Params<TSelf, T1, T2> where TSelf : allows ref struct where T1 : allows ref struct where T2 : allows ref struct
        {
            public Params(TSelf self)
            {
                this.Self = self;
            }

            public TSelf Self { get; }
        }

        public interface IReadOnlyArray<out T> where T : allows ref struct
        {
            T this[int index] { get; }

            int Count { get; }
        }

        public ref struct ReadOnlyArray<T> : IReadOnlyArray<T> where T : allows ref struct
        {
            private readonly ByteSpan memory;
            private readonly int length;

            public ReadOnlyArray(ByteSpan memory, int length)
            {
                this.memory = memory;
                this.length = length;
            }

            public unsafe T this[int index]
            {
                get
                {
                    fixed (byte* pointer = memory)
                    {
                        byte* indexed = pointer + index * System.Runtime.CompilerServices.Unsafe.SizeOf<T>();

                        return System.Runtime.CompilerServices.Unsafe.AsRef<T>(indexed);
                    }
                }
            }

            public int Count
            {
                get
                {
                    return this.Count;
                }
            }
        }
    }
}
