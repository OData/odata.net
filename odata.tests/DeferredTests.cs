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
using static CombinatorParsingV3.ParseMode;

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

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

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

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

            Assert.ThrowsException<InvalidDataException>(() => odataUri.Parse());
        }

        [TestMethod]
        public void DeferredTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

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

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

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

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

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

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

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

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

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

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

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

            var odataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

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

            var deferredOdataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

            var odataUri = deferredOdataUri.Parse();

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.OdataUriTranscriber.Instance.Transcribe(odataUri, stringBuilder);

            Assert.AreEqual(url, stringBuilder.ToString());
        }

        [TestMethod]
        public void WriteTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.StringInput(url);

            var deferredOdataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

            var odataUri = deferredOdataUri.Parse();

            var rewritten = Rewrite(odataUri);
            var realized = rewritten.Realize().Parsed;

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.OdataUriTranscriber.Instance.Transcribe(realized, stringBuilder);

            Assert.AreEqual(url.Replace('C', 'D').Replace('A', 'C').Replace('D', 'A'), stringBuilder.ToString());
        }

        [TestMethod]
        public void DeferredWriteTest()
        {
            //// TODO what you really want is to rewrite from deferred to deferred, not realized to deferred

            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.StringInput(url);

            var deferredOdataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

            var odataUri = deferredOdataUri.Parse();

            var rewritten = Rewrite(odataUri);

            var segmentOutput = rewritten.Segments._1.Realize();

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.SegmentTranscriber.Instance.Transcribe(segmentOutput.Parsed, stringBuilder);

            Assert.AreEqual("/CC", stringBuilder.ToString());

            var questionMarkOutput = rewritten.QuestionMark.Realize();

            rewritten.Realize();
        }

        private static V3ParserPlayground.OdataUri<ParseMode.Deferred> Rewrite(V3ParserPlayground.OdataUri<ParseMode.Realized> originalUri)
        {
            return V3ParserPlayground.OdataUriFromRealizedRewriter.Instance.Transcribe(originalUri, new StringBuilder());
        }

        [TestMethod]
        public void DeferredWriteTest2()
        {
            //// TODO what you really want is to rewrite from deferred to deferred, not realized to deferred

            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.StringInput(url);

            var deferredOdataUri = V3ParserPlayground.OdataUri.Create(Func.Close(DeferredOutput.Create(input)).ToFuture());

            var rewritten = Rewrite2(deferredOdataUri~);

            var segmentOutput = rewritten.Segments._1.Realize();

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.SegmentTranscriber.Instance.Transcribe(segmentOutput.Parsed, stringBuilder);

            Assert.AreEqual("/CC", stringBuilder.ToString());

            var questionMarkOutput = rewritten.QuestionMark.Realize();

            rewritten.Realize();
        }

        private static V3ParserPlayground.OdataUri<ParseMode.Deferred> Rewrite2(V3ParserPlayground.OdataUri<ParseMode.Deferred> originalUri)
        {
            return V3ParserPlayground.Rewriter2.OdataUriRewriter.Instance.Transcribe(originalUri.Parse(), new StringBuilder());
        }
    }
}
