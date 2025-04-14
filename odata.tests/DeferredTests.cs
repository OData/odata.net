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
    public sealed class InstrumentedStringInput : ITokenStream<char>
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

        public ITokenStream<char>? Next()
        {
            var newIndex = this.index + 1;
            if (newIndex >= this.input.Length)
            {
                return null;
            }

            return new InstrumentedStringInput(this.input, this.indexes, newIndex);
        }
    }

    [TestClass]
    public sealed class DeferredTestsV2
    {
        [TestMethod]
        public void Test1()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = GeneratedOdataUri.Create(input);

            var segOutput = odataUri._segment_1.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.RealizedValue;
            }

            var realUri = odataUri.Parse();
        }

        [TestMethod]
        public void Test2()
        {
            var url = "B/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = GeneratedOdataUri.Create(input);

            Assert.ThrowsException<InvalidDataException>(() => odataUri.Parse());
        }

        [TestMethod]
        public void DeferredTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = GeneratedOdataUri.Create(input);

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri._segment_1._1._slash_1.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var questionMark = odataUri._questionMark_1.Realize();

            //// TODO it would actually be expected that this assertion would fail because of some amount of backtracking, but it fails because the actual count is 343, which seems way too high
            //// Assert.AreEqual(10, indexes.Count);

            Assert.AreEqual(9, indexes.Max());
        }

        [TestMethod]
        public void SupportCCharacter()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = GeneratedOdataUri.Create(input);

            var segOutput = odataUri._segment_1.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.RealizedValue;
            }

            var realUri = odataUri.Parse();
        }

        [TestMethod]
        public void Test3()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAB";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = GeneratedOdataUri.Create(input);

            var segOutput = odataUri._segment_1.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.RealizedValue;
            }

            Assert.ThrowsException<InvalidOperationException>(() => odataUri.Parse());
        }

        [TestMethod]
        public void Test4()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = GeneratedOdataUri.Create(input);

            var segOutput = odataUri._segment_1.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.RealizedValue;
            }

            var realUri = odataUri.Parse();

            Assert.IsTrue(realUri._queryOption_1.Node.Element.Value.TryGetValue(out var queryOption));

            var name = queryOption._optionName_1._alphaNumeric_1;

            Assert.IsTrue(name._1.Realize().RealizedValue is __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>.Realized._ʺx41ʺ);

            var secondCharacterNode = name.Node;
            Assert.IsTrue(secondCharacterNode.Element.Value.TryGetValue(out var secondCharacter));
            Assert.IsTrue(secondCharacter is __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>.Realized._ʺx41ʺ);

            var thirdCharacterNode = secondCharacterNode.Next;
            Assert.IsTrue(thirdCharacterNode.Element.Value.TryGetValue(out var thirdCharacter));
            Assert.IsTrue(thirdCharacter is __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>.Realized._ʺx41ʺ);

            var fourthCharacterNode = thirdCharacterNode.Next;
            Assert.IsTrue(fourthCharacterNode.Element.Value.TryGetValue(out var fourthCharacter));
            Assert.IsTrue(fourthCharacter is __GeneratedPartialV1.Deferred.CstNodes.Rules._alphaNumeric<ParseMode.Realized>.Realized._ʺx41ʺ);

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

            var odataUri = GeneratedOdataUri.Create(input);

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri._segment_1._1._slash_1.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var secondSegment = odataUri._segment_1.Node.Element.Realize();
            Assert.AreEqual(5, indexes.Max());

            var thirdSegment = odataUri._segment_1.Node.Next.Realize();

            Assert.AreEqual(9, indexes.Max());
        }

        [TestMethod]
        public void DeferredList2()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = GeneratedOdataUri.Create(input);

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri._segment_1._1._slash_1.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var secondAndThirdSegments = odataUri._segment_1.Node.Realize();

            Assert.AreEqual(9, indexes.Max());
        }

        [TestMethod]
        public void DeferredList3()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = GeneratedOdataUri.Create(input);

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri._segment_1._1._slash_1.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var segments = odataUri._segment_1.Realize();

            Assert.AreEqual(9, indexes.Max());
        }

        [TestMethod]
        public void TranscribeTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var deferredOdataUri = GeneratedOdataUri.Create(input);

            var odataUri = deferredOdataUri.Parse();

            var stringBuilder = new StringBuilder();
            CombinatorParsingV3.TranscribersUsingGeneratedNodes.OdataUriTranscriber.Instance.Transcribe(odataUri, stringBuilder);

            Assert.AreEqual(url, stringBuilder.ToString());
        }
    }

    public static class GeneratedOdataUri
    {
        public static __GeneratedPartialV1.Deferred.CstNodes.Rules._odataUri<ParseMode.Deferred> Create(ITokenStream<char> input)
        {
            return __GeneratedPartialV1.Deferred.CstNodes.Rules._odataUri.Create(Future.Create(() => new RealizationResult<char>(true, input)));
        }
    }

    [TestClass]
    public sealed class DeferredTests
    {
        [TestMethod]
        public void Test1()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.RealizedValue;
            }

            var realUri = odataUri.Parse();
        }

        [TestMethod]
        public void Test2()
        {
            var url = "B/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

            Assert.ThrowsException<InvalidDataException>(() => odataUri.Parse());
        }

        [TestMethod]
        public void DeferredTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

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

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.RealizedValue;
            }

            var realUri = odataUri.Parse();
        }

        [TestMethod]
        public void Test3()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAB";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.RealizedValue;
            }

            Assert.ThrowsException<InvalidOperationException>(() => odataUri.Parse());
        }

        [TestMethod]
        public void Test4()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.RealizedValue;
            }

            var realUri = odataUri.Parse();

            Assert.IsTrue(realUri.QueryOptions.Node.Element.Value.TryGetValue(out var queryOption));

            var name = queryOption.Name.Characters;

            Assert.IsTrue(name._1.Realize().RealizedValue is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.Realized.A);

            var secondCharacterNode = name.Node;
            Assert.IsTrue(secondCharacterNode.Element.Value.TryGetValue(out var secondCharacter));
            Assert.IsTrue(secondCharacter is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.Realized.A);

            var thirdCharacterNode = secondCharacterNode.Next;
            Assert.IsTrue(thirdCharacterNode.Element.Value.TryGetValue(out var thirdCharacter));
            Assert.IsTrue(thirdCharacter is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.Realized.A);

            var fourthCharacterNode = thirdCharacterNode.Next;
            Assert.IsTrue(fourthCharacterNode.Element.Value.TryGetValue(out var fourthCharacter));
            Assert.IsTrue(fourthCharacter is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.Realized.A);

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

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

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

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

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

            var odataUri = V3ParserPlayground.OdataUri.Create(input);

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri.Segments._1.Slash.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var segments = odataUri.Segments.Realize();

            Assert.AreEqual(9, indexes.Max());
        }

        [TestMethod]
        public void TranscribeTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var deferredOdataUri = V3ParserPlayground.OdataUri.Create(input);

            var odataUri = deferredOdataUri.Parse();

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.OdataUriTranscriber.Instance.Transcribe(odataUri, stringBuilder);

            Assert.AreEqual(url, stringBuilder.ToString());
        }

        [TestMethod]
        public void WriteTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var deferredOdataUri = V3ParserPlayground.OdataUri.Create(input);

            var odataUri = deferredOdataUri.Parse();

            var rewritten = Rewrite(odataUri);
            var realized = rewritten.Realize().RealizedValue;

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.OdataUriTranscriber.Instance.Transcribe(realized, stringBuilder);

            Assert.AreEqual(url.Replace('C', 'D').Replace('A', 'C').Replace('D', 'A'), stringBuilder.ToString());
        }

        [TestMethod]
        public void DeferredWriteTest()
        {
            //// TODO what you really want is to rewrite from deferred to deferred, not realized to deferred

            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var input = new CombinatorParsingV3.CharacterTokenStream(url);

            var deferredOdataUri = V3ParserPlayground.OdataUri.Create(input);

            var odataUri = deferredOdataUri.Parse();

            var rewritten = Rewrite(odataUri);

            var segmentOutput = rewritten.Segments._1.Realize();

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.SegmentTranscriber.Instance.Transcribe(segmentOutput.RealizedValue, stringBuilder);

            Assert.AreEqual("/CC", stringBuilder.ToString());

            var questionMarkOutput = rewritten.QuestionMark.Realize();

            rewritten.Realize();
        }

        private static V3ParserPlayground.OdataUri<ParseMode.Deferred> Rewrite(V3ParserPlayground.OdataUri<ParseMode.Realized> originalUri)
        {
            return V3ParserPlayground.OdataUriFromRealizedRewriter.Instance.Transcribe(originalUri, new StringBuilder());
        }

        [TestMethod]
        public void WriteFromDeferredToDeferred()
        {
            //// TODO what you really want is to rewrite from deferred to deferred, not realized to deferred

            var url = "/AA/A/AAA?AAAA=AAAAAC";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var deferredOdataUri = V3ParserPlayground.OdataUri.Create(input);

            var rewritten = Rewrite2(deferredOdataUri);

            Assert.AreEqual(0, indexes.Count);

            var segmentOutput = rewritten.Segments._1.Realize();

            Assert.AreEqual(3, indexes.Max());

            var stringBuilder = new StringBuilder();
            V3ParserPlayground.SegmentTranscriber.Instance.Transcribe(segmentOutput.RealizedValue, stringBuilder);

            Assert.AreEqual(3, indexes.Max());

            Assert.AreEqual("/CC", stringBuilder.ToString());

            var questionMarkOutput = rewritten.QuestionMark.Realize();

            Assert.AreEqual(9, indexes.Max());

            var realized = rewritten.Realize().RealizedValue;

            Assert.AreEqual(url.Length - 1, indexes.Max());

            var secondStringBuilder = new StringBuilder();
            V3ParserPlayground.OdataUriTranscriber.Instance.Transcribe(realized, secondStringBuilder);

            Assert.AreEqual(url.Replace('C', 'D').Replace('A', 'C').Replace('D', 'A'), secondStringBuilder.ToString());
        }

        private static V3ParserPlayground.OdataUri<ParseMode.Deferred> Rewrite2(V3ParserPlayground.OdataUri<ParseMode.Deferred> originalUri)
        {
            return V3ParserPlayground.Rewriter2.OdataUriRewriter.Instance.Transcribe(originalUri, new StringBuilder());
        }
    }
}
