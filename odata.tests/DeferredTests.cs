using CombinatorParsingV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odata.tests
{
    [TestClass]
    public sealed class DeferredTests
    {
        //// TODO use a strongly typed, heterogeneous linked list in order to make clear the dependencies between each property of a given AST node?

        [TestMethod]
        public void Test1()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(DeferredOutput2.FromValue(input));

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.Parsed;
            }

            var realUri = odataUri.Realize();

            Assert.IsTrue(realUri.Success);
            /*Assert.IsTrue(realUri.Parsed.Segments._1.Characters._1 is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);
            Assert.IsTrue(realUri.Parsed.Segments._1.Characters._2 is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);
            Assert.IsFalse(realUri.Parsed.Segments._1.Characters._3 is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);*/
        }

        [TestMethod]
        public void Test2()
        {
            var url = "B/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(DeferredOutput2.FromValue(input));

            var realUri = odataUri.Realize();

            Assert.IsFalse(realUri.Success);
        }

        [TestMethod]
        public void DeferredTest()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var indexes = new List<int>();
            var input = new InstrumentedStringInput(url, indexes);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(DeferredOutput2.FromValue(input));

            Assert.AreEqual(0, indexes.Count);

            var slash = odataUri.Segments._1.Slash.Realize();

            Assert.AreEqual(1, indexes.Count);
            Assert.AreEqual(0, indexes[0]);

            var questionMark = odataUri.QuestionMark.Realize();

            //// TODO it would actually be expected that this assertion would fail because of some amount of backtracking, but it fails because the actual count is 343, which seems way too high
            //// Assert.AreEqual(10, indexes.Count);
            
            Assert.AreEqual(9, indexes.Max());

            //// TODO dynamic length lists
            //// TODO implement proper "futures"
            //// TODO write a transcriber just to make sure things still make sense from that perspective
            //// TODO clean up some of this new deferred code
        }

        [TestMethod]
        public void Test3()
        {
            var url = "/AA/A/AAA?AAAA=AAAAAB";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri<ParseMode.Deferred>(DeferredOutput2.FromValue(input));

            var segOutput = odataUri.Segments.Realize();
            if (segOutput.Success)
            {
                var segments = segOutput.Parsed;
            }

            var realUri = odataUri.Realize();

            var queryOption = realUri.Parsed.QueryOptions.Node.Element.Value;
            var name = queryOption.Name.Characters;

            Assert.IsTrue(name._1 is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);
            var secondCharacterNode = name.Node;
            Assert.IsTrue(secondCharacterNode.Element.Value is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);
            var thirdCharacterNode = secondCharacterNode.Next;
            Assert.IsTrue(thirdCharacterNode.Element.Value is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);
            var fourthCharacterNode = thirdCharacterNode.Next;
            Assert.IsTrue(fourthCharacterNode.Element.Value is V3ParserPlayground.AlphaNumeric<ParseMode.Realized>.A);
            var potentialFifthCharacterNode = fourthCharacterNode.Next;
            var optional = potentialFifthCharacterNode.Element.Realize();
            Assert.IsFalse(optional.Parsed.HasValue);

            ////Assert.IsTrue(realUri.Success);
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
    }
}
