using CombinatorParsingV3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
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
        public void DynamicList()
        {
            ListPlaygroundAgain.DoWork();
        }
    }

    public static class ListPlaygroundAgain
    {
        public readonly ref struct RefStructNullable<TValue, TState> where TValue : allows ref struct where TState : allows ref struct
        {
            private readonly bool hasValue;

            private readonly Func<TState, TValue> generator;

            public readonly TState state;

            public RefStructNullable(Func<TState, TValue> generator, TState state)
            {
                this.generator = generator;
                this.state = state;

                this.hasValue = true;
            }

            public bool TryGetValue([MaybeNullWhen(false)] out TValue value)
            {
                if (this.hasValue)
                {
                    value = this.generator(this.state);
                    return true;
                }

                value = default;
                return false;
            }
        }

        public ref struct List<TValue> where TValue : allows ref struct
        {
            private RefStructNullable<TValue, Tval> first;

            public List()
            {
                this.first = new RefStructNullable<ListNode<TValue, Wrapper>, Wrapper>();
            }

            public void Append(TValue value)
            {
                AppendImpl(ref this.first, value);
            }

            private static void AppendImpl(ref RefStructNullable<ListNode<TValue, Wrapper>, Wrapper> potentialNode, TValue value)
            {
                if (!potentialNode.TryGetValue(out var node))
                {
                    potentialNode = new RefStructNullable<ListNode<TValue, Wrapper>, Wrapper>(Create, new Wrapper(value));
                }
                else
                {
                    potentialNode = new RefStructNullable<ListNode<TValue, Wrapper>, Wrapper>(
                        wrapper =>
                        {
                            var newNode = new ListNode<TValue, Wrapper>(wrapper.Value);

                            return newNode;
                        },
                        new Wrapper(node.Value));
                }
            }
        }

        public ref struct ListNode2<TValue> where TValue : allows ref struct
        {
            public ListNode2(TValue value)
            {
                Value = value;

                this.Next = new RefStructNullable<ListNode2<TValue>, Func<ListNode2<TValue>>();
            }

            public TValue Value { get; }

            public RefStructNullable<ListNode2<TValue>, (ListNode2<TValue>, Func<ListNode2<TValue>, ListNode2<TValue>)> Next;

            public void Append(TValue value)
            {
                this.Next = new RefStructNullable<ListNode2<TValue>, Func<ListNode2<TValue>>(_ => new ListNode2<TValue>(_), );
            }
        }

        public static ListNode2<TValue> Get<TValue>(ListNode2<TValue> node)
        {
            node.Next.TryGetValue(out var foo);
            return foo;
        }

        public ref struct ListNode<TValue, TState> where TValue : allows ref struct where TState : allows ref struct
        {
            public ListNode(TValue value)
            {
                Value = value;

                this.Next = new RefStructNullable<ListNode<TValue, TState>, TState>();
            }

            public TValue Value { get; }

            public RefStructNullable<ListNode<TValue, TState>, TState> Next;

            public void Append(Func<TState, ListNode<TValue, TState>> generator, TState state)
            {
                this.Next = new RefStructNullable<ListNode<TValue, TState>, TState>(generator, state);
            }

            public void Prepend(TValue value)
            {
                var prefix = new ListNode<TValue, TState>(value);
                prefix.Next = new RefStructNullable<ListNode<TValue, TState>, TState>()
            }
        }

        public static class Factory<TState> where TState : allows ref struct
        {
            public static ListNode<TValue, TState> Create<TValue>(TValue value) where TValue : allows ref struct
            {
                return new ListNode<TValue, TState>(value);
            }
        }

        public static void DoWork()
        {
            var firstNode = new ListNode<PretendNode, PretendNode>(new PretendNode(-1));
            var secondNode = new ListNode<PretendNode, PretendNode>(new PretendNode(0));

            firstNode.Next = new RefStructNullable<ListNode<PretendNode, PretendNode>, PretendNode>(
                _ =>
                {
                    var newNode = new ListNode<PretendNode, PretendNode>(_);
                    newNode.Next = new RefStructNullable<ListNode<PretendNode, PretendNode>, PretendNode>(
                        _2 =>
                        {
                            var newNode2 = new ListNode<PretendNode, PretendNode>(_2);
                            return newNode2;
                        },
                        new PretendNode(2));
                    return newNode;
                },
                new PretendNode(1));



            var list = new List<PretendNode, Closure<PretendNode>>();
            for (int i = 0; i < 10; ++i)
            {
                list.Append(Create, new Closure<PretendNode>(i));
            }

            foreach (var element in list)
            {
                Console.WriteLine(element.Value);
            }

            var listNode = Factory<Closure<PretendNode>>.Create(new PretendNode(-1));
            for (int i = 0; i < 10; ++i)
            {
                //// TODO why does this work, but just providing the values doesn't?
                //// to answer this question, the answer is: just providing the values works fine, but we aren't trying to make it work where the elements are `int`, we are trying to make it work where the elements are a ref struct
                //// TODO but the question still stands: why do you need the `closure` part? why can't you just provide the `pretendnode` part?




                ///// TODO YOU ARE NOT DYNAMICALLY MAKING THE LIST BIGGER, YOU ARE SECONDING THE SECOND ELEMENT A BUNCH OF TIMES!
                var nextNode = new RefStructNullable<ListNode<PretendNode, Closure<PretendNode>>, Closure<PretendNode>>(Create, new Closure<PretendNode>(i));
                listNode.Append(Create, new Closure<PretendNode>(i));
            }

            foreach (var element in listNode.ToEnumerable())
            {
                Console.WriteLine(element.Value);
            }

            /*var listNode2 = new ListNode<PretendNode, PretendNode>(new PretendNode(-1));
            for (int i = 0; i < 10; ++i)
            {
                listnode
            }*/
        }

        public static ListNodeEnumerable<TValue, TState> ToEnumerable<TValue, TState>(this ListNode<TValue, TState> listNode) where TValue : allows ref struct where TState : allows ref struct
        {
            return new ListNodeEnumerable<TValue, TState>(listNode);
        }

        public ref struct ListNodeEnumerable<TValue, TState> where TValue : allows ref struct where TState : allows ref struct
        {
            private readonly ListNode<TValue, TState> listNode;

            public ListNodeEnumerable(ListNode<TValue, TState> listNode)
            {
                this.listNode = listNode;
            }

            public ListNodeEnumerator GetEnumerator()
            {
                return new ListNodeEnumerator(this.listNode);
            }

            public ref struct ListNodeEnumerator
            {
                private ListNode<TValue, TState> listNode;

                private bool moved;

                public ListNodeEnumerator(ListNode<TValue, TState> listNode)
                {
                    this.listNode = listNode;

                    this.moved = false;
                }

                public TValue Current
                {
                    get
                    {
                        if (!this.moved)
                        {
                            throw new Exception("TODO");
                        }

                        return this.listNode.Value;
                    }
                }

                public bool MoveNext()
                {
                    if (!this.moved)
                    {
                        this.moved = true;
                        return true;
                    }

                    if (this.listNode.Next.TryGetValue(out var next))
                    {
                        this.listNode = next;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static ListNode<PretendNode, Closure<PretendNode>> Create(Closure<PretendNode> closure)
        {
            return closure.Create();
        }

        public readonly ref struct Closure<TValue> where TValue : allows ref struct
        {
            public Closure(int value)
            {
                Value = value;
            }

            public int Value { get; }

            public ListNode<PretendNode, Closure<PretendNode>> Create()
            {
                return Factory<Closure<PretendNode>>.Create(new PretendNode(this.Value));
            }
        }

        public readonly ref struct PretendNode
        {
            public PretendNode(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }
    }
}
