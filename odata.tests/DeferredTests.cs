using __GeneratedOdata.Trancsribers.Rules;
using CombinatorParsingV3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace odata.tests
{
    [TestClass]
    public sealed class DeferredTests
    {
        [TestMethod]
        public void Test1()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.Parsed;
            }

            var realUri = odataUri.Parse();
        }

        [TestMethod]
        public void Test2()
        {
            var url = "B/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            Assert.ThrowsException<InvalidDataException>(() => odataUri.Parse());
        }

        [TestMethod]
        public void DeferredTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri.Segments._1.Slash.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var questionMark = odataUri.QuestionMark.Realize();

            //// TODO it would actually be expected that this assertion would fail because of some amount of backtracking, but it fails because the actual count is 343, which seems way too high
            //// Assert.AreEqual(10, indexes.Count);
            
            Assert.AreEqual(9, indexes.Max());
        }

        [TestMethod]
        public void SupportCCharacter()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.Parsed;
            }

            var realUri = odataUri.Parse();
        }

        [TestMethod]
        public void Test3()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAB";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.Parsed;
            }

            Assert.ThrowsException<InvalidOperationException>(() => odataUri.Parse());
        }

        [TestMethod]
        public void Test4()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.Parsed;
            }

            var realUri = odataUri.Parse();

            Assert.IsTrue(realUri.QueryOptions.Node.Element.Value.TryGetValue(out var queryOption));
            
            var name = queryOption.Name.Characters;

            Assert.IsTrue(name._1.Realize().Parsed is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);
            
            var secondCharacterNode = name.Node;
            Assert.IsTrue(secondCharacterNode.Element.Value.TryGetValue(out var secondCharacter));
            Assert.IsTrue(secondCharacter is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);

            var thirdCharacterNode = secondCharacterNode.Next;
            Assert.IsTrue(thirdCharacterNode.Element.Value.TryGetValue(out var thirdCharacter));
            Assert.IsTrue(thirdCharacter is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);

            var fourthCharacterNode = thirdCharacterNode.Next;
            Assert.IsTrue(fourthCharacterNode.Element.Value.TryGetValue(out var fourthCharacter));
            Assert.IsTrue(fourthCharacter is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);

            var potentialFifthCharacterNode = fourthCharacterNode.Next;
            Assert.IsFalse(potentialFifthCharacterNode.Element.Value.TryGetValue(out var potentialFifthCharacter));

            // NOTE: it *is* a bit strange that `Next` will always give you back something, and the caller has to check if there's a value on that element; however, this is necessary because:
            // 1. in deferred mode, we don't know how many elements there will be, and we have no way to communicate this to the caller because we don't know the answer; if the caller wants to realize the sixth node, only at that time can we notice that there are only 4; to make this clear, we would need to have a different type for `manynode` when it's realized vs when it's deferred
            // 2. if we use parsemode anywhere in the AST, we have to use it everywhere; in this case, for example, if `manynode` had a deferred type and a realized type, then in `segment`, for example, `characters` would need to be `atleastone<alphanumericholder, alphanumeric<parsemode.realized>, tmode>` for deferred and `atleastone<alphanumeric<parsemode.realized>, alphanumeric<parsemode.realized>, tmode>` for realized; this would require `segment` itself to also have two variants, one for realized and one for deferred, and this would propagate through the entire AST
            var potentialSixthCharacterNode = potentialFifthCharacterNode.Next;
            Assert.IsFalse(potentialSixthCharacterNode.Element.Value.TryGetValue(out var potentialSixCharacter));
        }

        [TestMethod]
        public void DeferredList()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri.Segments._1.Slash.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var secondSegment = odataUri.Segments.Node.Element.Realize();
            Assert.AreEqual(5, indexes.Max());

            var thirdSegment = odataUri.Segments.Node.Next.Realize();

            Assert.AreEqual(9, indexes.Max());
        }

        [TestMethod]
        public void DeferredList2()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri.Segments._1.Slash.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var secondAndThirdSegments = odataUri.Segments.Node.Realize();

            Assert.AreEqual(9, indexes.Max());
        }

        [TestMethod]
        public void DeferredList3()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri.Segments._1.Slash.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var segments = odataUri.Segments.Realize();

            Assert.AreEqual(9, indexes.Max());
        }

        private sealed class InstrumentedStringInput : IInput<char>
        {
            private readonly string input;
            private readonly List<int> indexes;
            private readonly int index;

            public InstrumentedStringInput(string input, List<int> indexes)
                : this(input, indexes, 0)
            {
            }

            private InstrumentedStringInput(string input, List<int> indexes, int index)
            {
                this.input = input;
                this.indexes = indexes;
                this.index = index;
            }

            public char Current
            {
                get
                {
                    this.indexes.Add(this.index);

                    return this.input[this.index];
                }
            }

            public IInput<char> Next()
            {
                var newIndex = this.index + 1;
                if (newIndex >= this.input.Length)
                {
                    return null;
                }

                return new InstrumentedStringInput(this.input, this.indexes, newIndex);
            }
        }

        [TestMethod]
        public void TranscribeTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.StringInput(url);

            var deferredOdataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(Func.Close(DeferredOutput.Create(input)));

            var odataUri = deferredOdataUri.Parse();

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.OdataUriTranscriber.Instance.Transcribe(odataUri, stringBuilder);

            Assert.AreEqual(url, stringBuilder.ToString());
        }

        [TestMethod]
        public unsafe void LinkedListTest()
        {
            /*scoped*/
            var list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(-1));
            for (int i = 0; i < 10; ++i)
            {
                var data = stackalloc int[sizeof(LinkedListNode<Wrapper<int>>)];
                Unsafe.Copy(data, ref list);

                LinkedListNode<Wrapper<int>>* pointer = (LinkedListNode<Wrapper<int>>*)data;
                list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(i), pointer);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }

            for (int i = 0; i < 10; ++i)
            {
                var data = stackalloc int[sizeof(LinkedListNode<Wrapper<int>>)];
                Unsafe.Copy(data, ref list);

                LinkedListNode<Wrapper<int>>* pointer = (LinkedListNode<Wrapper<int>>*)data;
                list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(i), pointer);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }


            var second = new LinkedListNode<int>(11);
            for (int i = 12; i < 22; ++i)
            {
                var data = stackalloc int[sizeof(LinkedListNode<int>)];
                Unsafe.Copy(data, ref second);

                LinkedListNode<int>* pointer = (LinkedListNode<int>*)data;
                second = new LinkedListNode<int>(i, pointer);
            }

            /*var concated = ConcatFunc<int>()(list, second);
            foreach (var element in concated)
            {
                Console.WriteLine(element);
            }*/
        }

        [TestMethod]
        public unsafe void LinkedListTest2()
        {
            var list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(-1));
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[sizeof(LinkedListNode<Wrapper<int>>)];
                var wrapper = new Wrapper<int>(i);
                list = list.Append(ref wrapper, memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }
        }

        [TestMethod]
        public void LinkedListTest2Point7()
        {
            var list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(-1));
            for (int i = 0; i < 10; ++i)
            {
                var size = Unsafe.SizeOf<LinkedListNode<Wrapper<int>>>();
                Span<byte> memory = stackalloc byte[size];
                var wrapper = new Wrapper<int>(i);
                unsafe
                {
                    list = list.Append(ref wrapper, memory);
                }
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }
        }

        [TestMethod]
        public LinkedListNode<Wrapper<int>> LinkedListTest2Point6()
        {
            var list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(-1));
            for (int i = 0; i < 10; ++i)
            {
                var size = Unsafe.SizeOf<LinkedListNode<Wrapper<int>>>();
                Span<byte> memory = stackalloc byte[size];
                var wrapper = new Wrapper<int>(i);
                unsafe
                {
                    list = list.Append(ref wrapper, memory);
                }
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }

            return list; //// TODO i shouldn't be able to return this list
        }

        public static Span<int> GetSpan()
        {
            var span = new Span<int>();
            return span;
        }

        /*public static Span<int> GetSpan2()
        {
            Span<int> span = stackalloc int[100];
            return span;
        }

        public static SomethingThatUsesSpan GetSomethingThatUsesSpan()
        {
            Span<int> span = stackalloc int[100];
            var thing = new SomethingThatUsesSpan(span);

            return thing;
        }*/

        public static SomethingThatUsesSpan GetSomethingThatUsesSpan2()
        {
            Span<int> span = stackalloc int[100];
            var thing = new SomethingThatUsesSpan();

            return thing;
        }

        public static SomethingThatUsesSpan GetSomethingThatUsesSpan23()
        {
            Span<int> span = new Span<int>();
            var thing = new SomethingThatUsesSpan(span);

            return thing;
        }

        public readonly ref struct SomethingThatUsesSpan
        {
            private readonly Span<int> span;

            public SomethingThatUsesSpan(Span<int> span)
            {
                this.span = span;
            }

            public SomethingThatUsesSpan()
            {
            }
        }

        [TestMethod]
        public unsafe void LinkedListTest2Point5()
        {
            var list = new LinkedListNode<int>(-1);
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[sizeof(LinkedListNode<int>)];
                list = list.Append(ref i, memory); //// TODO i don't think you need `ref` here; span is very small
            }

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }
        }

        [TestMethod]
        public unsafe void LinkedListTest2Point8()
        {
            var list = new LinkedListNode<int>(-1);
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[sizeof(LinkedListNode<int>)];
                list = list.Append(ref i, memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }

            SomethingElse();

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }
        }

        public unsafe void SomethingElse()
        {
            var memory = stackalloc byte[1000];

            for (int i = 0; i < 1000; ++i)
            {
                memory[i] = 0;
            }
        }

        [TestMethod]
        public unsafe void LinkedListTest2Point9()
        {
            var list = new LinkedListNode<int>(-1);
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[sizeof(LinkedListNode<int>)];
                list = list.Append(ref i, memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }

            DoEnumeration(list);
        }

        public void DoEnumeration(LinkedListNode<int> list)
        {
            foreach (var element in list)
            {
                Console.WriteLine(element);
            }
        }

        [TestMethod]
        public unsafe void LinkedListTest2Point10()
        {
            var list = new LinkedListNode<int>(-1);
            for (int i = 0; i < 10; ++i)
            {
                Span<byte> memory = stackalloc byte[sizeof(LinkedListNode<int>)];
                list = list.Append(ref i, memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }

            AddMore(list);

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }
        }

        public unsafe void AddMore(LinkedListNode<int> list)
        {
            for (int i = 10; i < 20; ++i)
            {
                Span<byte> memory = stackalloc byte[sizeof(LinkedListNode<int>)];
                list = list.Append(ref i, memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }
        }

        [TestMethod]
        public unsafe void LinkedListTest3()
        {
            var list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(-1));
            for (int i = 0; i < 10; ++i)
            {
                var memory = stackalloc int[sizeof(LinkedListNode<Wrapper<int>>)];
                list = list.Append(new Wrapper<int>(i), memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }
        }

        [TestMethod]
        public unsafe void LinkedListTest4()
        {
            var list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(-1));
            for (int i = 0; i < 10; ++i)
            {
                var memory = list.Allocate()();
                var wrapper = new Wrapper<int>(i);
                list = list.Append(ref wrapper, memory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }
        }

        /*public static LinkedListNode<Wrapper<int>> GetSpan()
        {
            //// TODO this can compile if y o make the method `unsafe`
            Span<char> foo = stackalloc char[10];

            return new LinkedListNode<Wrapper<int>>(foo);
        }*/

        public static unsafe LinkedListNode<Wrapper<int>> GetData()
        {
            var list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(-1));
            for (int i = 0; i < 10; ++i)
            {
                var data = stackalloc int[sizeof(LinkedListNode<Wrapper<int>>)];
                Unsafe.Copy(data, ref list);

                LinkedListNode<Wrapper<int>>* pointer = (LinkedListNode<Wrapper<int>>*)data;
                list = new LinkedListNode<Wrapper<int>>(new Wrapper<int>(i), pointer);
            }

            return list;
        }

        public readonly ref struct Wrapper<T> where T : allows ref struct
        {
            public Wrapper(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }

        public static Func<LinkedListNode<TValue>, LinkedListNode<TValue>, LinkedListNode<TValue>> ConcatFunc<TValue>()
        {
            return Concat;
        }

        public unsafe static LinkedListNode<TValue> Concat<TValue>(LinkedListNode<TValue> first, LinkedListNode<TValue> second)
        {
            var firstEnumerator = first.GetEnumerator();
            if (!firstEnumerator.MoveNext())
            {
                throw new Exception("TODO");
            }

            var result = new LinkedListNode<TValue>(firstEnumerator.Current);
            while (firstEnumerator.MoveNext())
            {
                var data = stackalloc int[sizeof(LinkedListNode<TValue>)];
                Unsafe.Copy(data, ref result);

                LinkedListNode<TValue>* pointer = (LinkedListNode<TValue>*)data;
                result = new LinkedListNode<TValue>(firstEnumerator.Current, pointer);
            }

            /*var secondEnumerator = second.GetEnumerator();
            while (secondEnumerator.MoveNext())
            {
                var data = stackalloc int[sizeof(LinkedListNode<TValue>)];
                Unsafe.Copy(data, ref result);

                LinkedListNode<TValue>* pointer = (LinkedListNode<TValue>*)data;
                result = new LinkedListNode<TValue>(secondEnumerator.Current, pointer);
            }*/

            return result;
        }

        /*public static Span<int> Allocate<TValue>(LinkedListNode<TValue> node)
        {
            var size = Unsafe.SizeOf<LinkedListNode<TValue>>();
            Span<int> foo = stackalloc int[size];
            return foo;
        }*/

        [TestMethod]
        public unsafe void SpansTest()
        {
            //// TODO have a span for ref structs
            //// TODO have an equivalent for stackallocs with non-primitives (that probably returns the new span)

            var value = -1;
            Span<byte> valueSpan = new Span<byte>(&value, sizeof(int));

            var list = new LinkedListNode2<int>(valueSpan);
            for (int i = 0; i < 5; ++i)
            {
                Span<byte> nextMemory = stackalloc byte[sizeof(LinkedListNode2<int>)];
                Span<byte> valueMemory = stackalloc byte[sizeof(int)];
                fixed (byte* valuePointer = valueMemory)
                {
                    Unsafe.Copy(valuePointer, ref i);
                }

                list = list.Append(valueMemory, nextMemory);
            }

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }

            Span<int> arraySpan = new[] { 1, 2, 3, 4 };
            Span<byte> nextMemory2 = stackalloc byte[sizeof(LinkedListNode2<int>)];
            list = Append(list, arraySpan, nextMemory2);

            foreach (var element in list)
            {
                Console.WriteLine(element);
            }
        }

        public static unsafe LinkedListNode2<TValue> Append<TValue>(LinkedListNode2<TValue> list, Span<TValue> values, Span<byte> memory)
        {
            fixed (TValue* pointer = values)
            {
                var byteValues = new Span<byte>(pointer, values.Length * sizeof(TValue));
                return list.Append(byteValues, memory);
            }
        }

        public readonly unsafe ref struct LinkedListNode2<TValue> where TValue : allows ref struct
        {
            private readonly LinkedListNode2<TValue>* previous;

            private readonly TValue* values;

            private readonly int valueCount;

            public LinkedListNode2(Span<byte> values)
                : this(values, null)
            {
            }

            private LinkedListNode2(Span<byte> values, LinkedListNode2<TValue>* previous)
            {
                if (values.Length % sizeof(TValue) != 0)
                {
                    throw new Exception("TODO");
                }

                fixed (byte* pointer = values)
                {
                    this.values = (TValue*)pointer;
                }

                this.valueCount = values.Length / sizeof(TValue);

                this.previous = previous;
            }

            public LinkedListNode2<TValue> Append(Span<byte> values, Span<byte> memory)
            {
                if (values.Length % sizeof(TValue) != 0)
                {
                    throw new Exception("TODO");
                }

                //// TODO can the node contain multiple values at once?
                //// TODO this is size in bytes, but span length is number of ints (which are 4 bytes each)
                var size = Unsafe.SizeOf<LinkedListNode2<TValue>>();
                if (memory.Length != size) //// TODO do we want to allow for overallocation? i don't think it makes sense to do this, but maybe there's a reason
                {
                    throw new Exception("TODO not enough memory allocated");
                }

                fixed (byte* data = memory)
                {
                    var self = this;
                    Unsafe.Copy(data, ref self);

                    LinkedListNode2<TValue>* pointer = (LinkedListNode2<TValue>*)data;
                    return new LinkedListNode2<TValue>(values, pointer);
                }
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private LinkedListNode2<TValue> node;

                private int index = 0;

                private bool hasMoved = false;

                public Enumerator(LinkedListNode2<TValue> node)
                {
                    this.node = node;
                }

                public TValue Current
                {
                    get
                    {
                        if (!hasMoved)
                        {
                            throw new Exception("TODO");
                        }

                        return node.values[index];
                    }
                }

                public bool MoveNext()
                {
                    if (!hasMoved)
                    {
                        this.hasMoved = true;
                        return true;
                    }

                    if (this.index < this.node.valueCount - 1)
                    {
                        ++this.index;
                        return true;
                    }

                    if (this.node.previous == null)
                    {
                        return false;
                    }

                    this.node = *node.previous;
                    this.index = 0;
                    return true;
                }
            }
        }

        public readonly unsafe ref struct LinkedListNode<TValue> where TValue : allows ref struct
        {
            private readonly LinkedListNode<TValue>* previous;
            ////private readonly Span<char> span;

            public LinkedListNode(TValue value)
            {
                this.Value = value;
            }

            /*public LinkedListNode(Span<char> span)
            {
                this.span = span;
            }*/

            public LinkedListNode(TValue value, LinkedListNode<TValue>* previous)
            {
                this.Value = value;
                this.previous = previous;
            }

            public TValue Value { get; }

            public Func<Span<byte>> Allocate()
            {
                return () =>
                {
                    Span<byte> memory = stackalloc byte[sizeof(LinkedListNode<TValue>)];

                    return memory;
                };
            }

            public LinkedListNode<TValue> Append(ref TValue value, Span<byte> memory)
            {
                //// TODO can the node contain multiple values at once?
                //// TODO this is size in bytes, but span length is number of ints (which are 4 bytes each)
                var size = Unsafe.SizeOf<LinkedListNode<TValue>>();
                if (memory.Length != size) //// TODO do we want to allow for overallocation? i don't think it makes sense to do this, but maybe there's a reason
                {
                    throw new Exception("TODO not enough memory allocated");
                }

                fixed (byte* data = memory)
                {
                    var self = this;
                    Unsafe.Copy(data, ref self);

                    LinkedListNode<TValue>* pointer = (LinkedListNode<TValue>*)data;
                    return new LinkedListNode<TValue>(value, pointer);
                }
            }

            public LinkedListNode<TValue> Append(TValue value, int* memory)
            {
                int* data = memory;
                var self = this;
                Unsafe.Copy(data, ref self);

                LinkedListNode<TValue>* pointer = (LinkedListNode<TValue>*)data;
                return new LinkedListNode<TValue>(value, pointer);
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            public ref struct Enumerator
            {
                private LinkedListNode<TValue> node;

                private bool moved = false;

                public Enumerator(LinkedListNode<TValue> node)
                {
                    this.node = node;
                }

                public TValue Current
                {
                    get
                    {
                        if (!this.moved)
                        {
                            throw new InvalidOperationException("TODO");
                        }

                        return node.Value;
                    }
                }

                public bool MoveNext()
                {
                    if (!this.moved)
                    {
                        this.moved = true;
                        return true;
                    }


                    if (node.previous == null)
                    {
                        return false;
                    }

                    node = *node.previous;
                    return true;
                }
            }
        }

        /*public static void Foo()
        {
            var thing = Bar();
        }

        public static LinkedListNode<string> Bar()
        {
            var first = new LinkedListNode<string>("Asdf");
            var second = new LinkedListNode<string>("qwer");
            first.Append(second);

            return first; // second is now popped off the stack, so the pointer that first has to second is no longer valid
        }*/
    }

    /*#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        public static class BetterSpan
        {
            public static unsafe BetterSpan<T> Create<T>(Span<T> values)
            {
                fixed (T* pointer = &values[0])
                {
                    return new BetterSpan<T>(pointer, values.Length);
                }
            }

            public static unsafe BetterSpan<T> FromInstance<T>(T value)
            {
                var valuePointer = &value;
                Span<byte> span = new Span<byte>(valuePointer, Unsafe.SizeOf<T>());

                return BetterSpan.FromAllocated<T>(span, 1);
            }

            public static unsafe BetterSpan<T> FromAllocated<T>(Span<byte> memory, int length) where T : allows ref struct
            {
                return new BetterSpan<T>(memory, length);
            }

            public static unsafe void Copy<T>(Span<byte> destination, ref T source) where T : allows ref struct
            {
                fixed (byte* pointer = destination)
                {
                    System.Runtime.CompilerServices.Unsafe.Copy(pointer, ref source);
                }
            }
        }

        public unsafe ref struct BetterSpan<T> where T : allows ref struct
        {
            private readonly T* data;

            internal BetterSpan(T* data, int length)
            {
                this.data = data;
                this.Length = length;
            }

            internal BetterSpan(Span<byte> memory, int length)
            {
                if (memory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<T>() * length)
                {
                    throw new Exception("TODO");
                }

                this.data = (T*)&memory;
                this.Length = length;
            }

            public T this[int index]
            {
                get
                {
                    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Length);
                    ArgumentOutOfRangeException.ThrowIfNegative(index);

                    return this.data[index];
                }
                set
                {
                    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Length);
                    ArgumentOutOfRangeException.ThrowIfNegative(index);

                    this.data[index] = value;
                }
            }

            public int Length { get; }

            public static implicit operator T*(BetterSpan<T> betterSpan)
            {
                return betterSpan.data;
            }
        }

        public static class LinkedListNodeExtensions
        {
            public static unsafe LinkedListNode<T> Append<T>(this LinkedListNode<T> source, T value, Span<byte> previousMemory)
            {
                return source.Append(BetterSpan.FromInstance(value), previousMemory);
            }
        }

        public readonly ref struct LinkedListNode<T> where T : allows ref struct
        {
            private readonly BetterSpan<LinkedListNode<T>> previous;

            private readonly BetterSpan<T> values;

            public LinkedListNode(BetterSpan<T> values)
                : this(values, default)
            {
            }

            private LinkedListNode(BetterSpan<T> values, BetterSpan<LinkedListNode<T>> previous)
            {
                this.values = values;
                this.previous = previous;
            }

            public LinkedListNode<T> Append(BetterSpan<T> values, Span<byte> previousMemory)
            {
                if (previousMemory.Length != System.Runtime.CompilerServices.Unsafe.SizeOf<LinkedListNode<T>>())
                {
                    throw new Exception("TODO");
                }

                var self = this;
                BetterSpan.Copy(previousMemory, ref self);

                return new LinkedListNode<T>(values, BetterSpan.FromAllocated<LinkedListNode<T>>(previousMemory, 1));
            }

            public ref struct Enumerator
            {
                private LinkedListNode<T> node;

                private int index;

                public Enumerator(LinkedListNode<T> node)
                {
                    this.node = node;

                    this.index = -1;
                }

                public T Current
                {
                    get
                    {
                        if (this.index < 0)
                        {
                            throw new Exception("TODO");
                        }

                        return node.values[this.index];
                    }
                }

                public bool MoveNext()
                {
                    if (this.index < 0)
                    {
                        if (this.node.values.Length > 0)
                        {
                            this.index = 0;
                            return true;
                        }

                        if (node.previous.Length == 0)
                        {
                            return false;
                        }

                        this.node = node.previous[0];
                        return this.MoveNext();
                    }

                    if (this.index < this.node.values.Length - 1)
                    {
                        ++this.index;
                        return true;
                    }

                    if (node.previous.Length == 0)
                    {
                        return false;
                    }

                    this.node = node.previous[0];
                    return this.MoveNext();
                }
            }
        }
    #pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type*/
}
