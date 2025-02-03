namespace odata.tests
{
    using _GeneratorV4;
    using AbnfParserGenerator;
    using CombinatorParsingV2;
    using GeneratorV3;
    using Microsoft.OData.UriParser;
    using Root;
    using Root.OdataResourcePath.CombinatorParsers;
    using Root.OdataResourcePath.Transcribers;
    using Sprache;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Xml;

    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        /*public void StackPointer()
        {
            AssertCast<Foo>(ParseFoo);

            //// TODO does *this* end up with the memory allocated in `stackpointer` instead of `parsefoo`?
            var topLevel = new TopLevel<Foo>();
            //// TODO see what it looks like when you need to allocate more memory
            var foos = ParseFoo((TopLevel<Foo> first, int second) => first.Generate(second), topLevel, () => new Foo());
            foreach (var foo in foos)
            {

            }
        }*/

        private static void AssertCast<T>(Parse<T> parse) where T : struct
        {
        }

        public struct Foo
        {
        }

        public ref struct StructList<T> : IEnumerable<T>
        {
            private readonly Span<T> span;

            private readonly int count;

            public StructList(Span<T> span, int count)
            {
                this.span = span;
                this.count = count;
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public ref struct Enumerator : IEnumerator<T>
            {
                private readonly StructList<T> values;

                private int index;

                public Enumerator(StructList<T> values)
                {
                    this.values = values;

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

                        return this.values.span[this.index];
                    }
                }

                object IEnumerator.Current => throw new NotImplementedException();

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    ++this.index;
                    return this.index <= this.values.count;
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }
            }
        }

        public static StructList<Foo> ParseFoo(Func<TopLevel<Foo>, int, Span<Foo>> allocate, in TopLevel<Foo> topLevel, Func<Foo?> doParse)
        {
            var count = 10;
            var span = allocate(topLevel, count);

            for (int i = 0; i < count; ++i)
            {
                var foo = doParse();
                if (foo == null)
                {
                    break;
                }

                span[i] = foo.Value;
            }

            return new StructList<Foo>(span, count);
        }

        public delegate StructList<T> Parse<T>(Func<TopLevel<T>, int, Span<T>> allocate, in TopLevel<T> topLevel, Func<T?> doParse) where T : struct;

        public unsafe struct TopLevel<T>
        {
            public TopLevel()
            {
            }

            private T* Values { get; set; }

            public Span<T> Generate(int count)
            {
                var memory = stackalloc T*[count];
                Span<T> span = new Span<T>(memory, count);

                fixed (T* temp = span)
                {
                    this.Values = temp;
                }

                return span;
            }
        }

        [TestMethod]
        public void Asdf()
        {
            ulong foo = ulong.MaxValue;

            Assert.AreEqual(0ul, Interlocked.Increment(ref foo));
            Assert.AreEqual(0ul, foo);
        }

        [TestMethod]
        public void OdataTest1()
        {
            var url = "https://graph.microsoft.com/v1.0/$metadata";
            if (!__GeneratedOdata.Parsers.Rules._odataUriParser.Instance.TryParse(url, out var urlCst))
            {
                throw new Exception("TODO");
            }

            var stringBuilder = new StringBuilder();
            __GeneratedOdata.Trancsribers.Rules._odataUriTranscriber.Instance.Transcribe(urlCst, stringBuilder);
            var transcribed = stringBuilder.ToString();
            Assert.AreEqual(url, transcribed);
        }

        [TestMethod]
        public void EntityTest()
        {
            var url = "$entity?$id=asdf";
            if (!__GeneratedOdata.Parsers.Rules._odataRelativeUriParser.Instance.TryParse(url, out var urlCst))
            {
                throw new Exception("TODO");
            }

            var stringBuilder = new StringBuilder();
            __GeneratedOdata.Trancsribers.Rules._odataRelativeUriTranscriber.Instance.Transcribe(urlCst, stringBuilder);
            var transcribed = stringBuilder.ToString();
            Assert.AreEqual(url, transcribed);

            var csdl =
"""
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="microsoft.graph" xmlns="http://docs.oasis-open.org/odata/ns/edm">
        <EntityType Name="user" OpenType="true">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
          <NavigationProperty Name="calendar" Type="microsoft.graph.calendar" ContainsTarget="true" />
        </EntityType>
        <EntityType Name="calendar">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
          <NavigationProperty Name="events" Type="Collection(microsoft.graph.event)" ContainsTarget="true" />
        </EntityType>
        <EntityType Name="event">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
        </EntityType>
        <EntityContainer Name="GraphService">
          <EntitySet Name="users" EntityType="microsoft.graph.user" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
""";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csdl)))
            {
                using (var xmlReader = XmlReader.Create(stream))
                {
                    var model = Microsoft.OData.Edm.Csdl.CsdlReader.Parse(xmlReader);

                    var odataUri = new Microsoft.OData.UriParser.ODataUriParser(
                        model,
                        new Uri(url, UriKind.Relative))
                        .ParseUri();
                }
            }
        }

        [TestMethod]
        public void OdataTest2()
        {
            var url = "users/myid/calendar/events?$filter=id eq 'thisisatest'";
            if (!__GeneratedOdata.Parsers.Rules._odataRelativeUriParser.Instance.TryParse(url, out var urlCst))
            {
                throw new Exception("TODO");
            }

            var stringBuilder = new StringBuilder();
            __GeneratedOdata.Trancsribers.Rules._odataRelativeUriTranscriber.Instance.Transcribe(urlCst, stringBuilder);
            var transcribed = stringBuilder.ToString();
            Assert.AreEqual(url, transcribed);
        }

        private static void Perf2Generator(int iterations)
        {
            var url = "users/myid/calendar/events?$filter=id eq 'thisisatest'";
            var parser = __GeneratedOdata.Parsers.Rules._odataRelativeUriParser.Instance;
            var transcriber = __GeneratedOdata.Trancsribers.Rules._odataRelativeUriTranscriber.Instance;
            for (int i = 0; i < iterations; ++i)
            {
                if (!parser.TryParse(url, out var urlCst))
                {
                    throw new Exception("TODO");
                }

                var stringBuilder = new StringBuilder();
                transcriber.Transcribe(urlCst, stringBuilder);
                var transcribed = stringBuilder.ToString();
                Assert.AreEqual(url, transcribed);
            }
        }

        [TestMethod]
        public void Perf2()
        {
            var iterations = 10000;
            var stopwatch = Stopwatch.StartNew();
            Perf2Generator(iterations);
            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = Stopwatch.StartNew();
            Perf1Odata(iterations);
            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = Stopwatch.StartNew();
            Perf2Generator(iterations);
            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = Stopwatch.StartNew();
            Perf1Odata(iterations);
            Console.WriteLine(stopwatch.ElapsedTicks);
        }

        private static void Perf1Generator(int iterations)
        {
            var url = "users/myid/calendar/events?$filter=id eq 'thisisatest'";
            var parser = __GeneratedOdata.Parsers.Rules._odataRelativeUriParser.Instance;
            var transcriber = __GeneratedOdata.Trancsribers.Rules._odataRelativeUriTranscriber.Instance;
            for (int i = 0; i < iterations; ++i)
            {
                if (!parser.TryParse(url, out var urlCst))
                {
                    throw new Exception("TODO");
                }

                var stringBuilder = new StringBuilder();
                transcriber.Transcribe(urlCst, stringBuilder);
                var transcribed = stringBuilder.ToString();
                Assert.AreEqual(url, transcribed);
            }
        }

        /*
    public static class _customNameParser
    {
        ////public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._customName> Instance { get; } = from _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1 in __GeneratedOdataV2.Parsers.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARParser.Instance
////from _ⲤqcharⲻnoⲻAMPⲻEQↃ_1 in Inners._ⲤqcharⲻnoⲻAMPⲻEQↃParser.Instance.Many()
////select new __GeneratedOdataV2.CstNodes.Rules._customName(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1, _ⲤqcharⲻnoⲻAMPⲻEQↃ_1);
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._customName> Instance { get; } = Parser.Instance;

        private sealed class Parser : IParser<char, __GeneratedOdataV2.CstNodes.Rules._customName>
        {
            private Parser()
            {
            }

            public static Parser Instance { get; } = new Parser();

            public IOutput<char, _customName> Parse(IInput<char>? input)
            {
                //// from _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1 in __GeneratedOdataV2.Parsers.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARParser.Instance
                var _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1 = __GeneratedOdataV2.Parsers.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARParser.Instance.Parse(input);
                if (!_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1.Success)
                {
                    //// TODO this bang can hopefully be removed...
                    return Output.Create(false, default(_customName)!, input);
                }

                //// from _ⲤqcharⲻnoⲻAMPⲻEQↃ_1 in Inners._ⲤqcharⲻnoⲻAMPⲻEQↃParser.Instance.Many()
                var _ⲤqcharⲻnoⲻAMPⲻEQↃ_1 = Inners._ⲤqcharⲻnoⲻAMPⲻEQↃParser.Instance.Many().Parse(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1.Remainder); //// TODO can you save the "many" parser instance?
                if (!_ⲤqcharⲻnoⲻAMPⲻEQↃ_1.Success)
                {
                    //// TODO this bang can hopefully be removed...
                    return Output.Create(false, default(_customName)!, input);
                }

                //// select new __GeneratedOdataV2.CstNodes.Rules._customName(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1, _ⲤqcharⲻnoⲻAMPⲻEQↃ_1);
                return Output.Create(
                    true,
                    new __GeneratedOdataV2.CstNodes.Rules._customName(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1.Parsed, _ⲤqcharⲻnoⲻAMPⲻEQↃ_1.Parsed),
                    _ⲤqcharⲻnoⲻAMPⲻEQↃ_1.Remainder);
            }
        }
    }
        */

        [TestMethod]
        public void Perf1()
        {
            //// TODO update generator to remove selectmany
            //// TODO update generator to remove all selects
            //// TODO does transcription matter?
            //// TODO pull structs from other branch for v3 combinator parser; //// TODO test performance
            //// TODO pull ref struct from other branch for v4 combinator parser //// TODO test performance
            //// TODO use structs for the node types? //// TODO test performance

            //// TODO perf improvments that worked:
            //// implementing a charparser in the parsing library
            //// removing selectmany calls in the generated code
            //// producing more singletons in the generated code

            var iterations = 10000;
            var stopwatch = Stopwatch.StartNew();
            Perf1Generator(iterations);
            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = Stopwatch.StartNew();
            Perf1Odata(iterations);
            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = Stopwatch.StartNew();
            Perf1Generator(iterations);
            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = Stopwatch.StartNew();
            Perf1Odata(iterations);
            Console.WriteLine(stopwatch.ElapsedTicks);
        }

        private static void Perf1Odata(int iterations)
        {
            var csdl =
"""
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="microsoft.graph" xmlns="http://docs.oasis-open.org/odata/ns/edm">
        <EntityType Name="user" OpenType="true">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
          <NavigationProperty Name="calendar" Type="microsoft.graph.calendar" ContainsTarget="true" />
        </EntityType>
        <EntityType Name="calendar">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
          <NavigationProperty Name="events" Type="Collection(microsoft.graph.event)" ContainsTarget="true" />
        </EntityType>
        <EntityType Name="event">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
        </EntityType>
        <EntityContainer Name="GraphService">
          <EntitySet Name="users" EntityType="microsoft.graph.user" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
""";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csdl)))
            {
                using (var xmlReader = XmlReader.Create(stream))
                {
                    var model = Microsoft.OData.Edm.Csdl.CsdlReader.Parse(xmlReader);

                    var original = "users/myid/calendar/events?$filter=id%20eq%20%27thisisatest%27";

                    for (int i = 0; i < iterations; ++i)
                    {
                        var odataUri = new Microsoft.OData.UriParser.ODataUriParser(
                            model,
                            new Uri(original, UriKind.Relative))
                            .ParseUri();
                        /*Assert.IsTrue(odataUri.Filter.Expression.Kind == Microsoft.OData.UriParser.QueryNodeKind.BinaryOperator);
                        if (!(odataUri.Filter.Expression is BinaryOperatorNode binaryNode))
                        {
                            throw new AssertFailedException();
                        }

                        Assert.IsTrue(binaryNode.Left.Kind == QueryNodeKind.Convert);
                        if (!(binaryNode.Left is ConvertNode convert))
                        {
                            throw new AssertFailedException();
                        }

                        Assert.IsTrue(convert.Source.Kind == QueryNodeKind.SingleValuePropertyAccess);

                        Assert.IsTrue(binaryNode.Right.Kind == QueryNodeKind.Constant);*/
                        
                        var uri = Microsoft.OData.ODataUriExtensions.BuildUri(odataUri, Microsoft.OData.ODataUrlKeyDelimiter.Slash).ToString();

                        Assert.AreEqual(original, uri);
                    }
                }
            }
        }

        [TestMethod]
        public void OdataTest3()
        {
            var csdl =
"""
<?xml version="1.0" encoding="utf-8"?>
<edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
  <edmx:DataServices>
    <Schema Namespace="microsoft.graph" xmlns="http://docs.oasis-open.org/odata/ns/edm">
        <EntityType Name="user" OpenType="true">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
          <NavigationProperty Name="calendar" Type="microsoft.graph.calendar" ContainsTarget="true" />
        </EntityType>
        <EntityType Name="calendar">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
          <NavigationProperty Name="events" Type="Collection(microsoft.graph.event)" ContainsTarget="true" />
        </EntityType>
        <EntityType Name="event">
          <Key>
            <PropertyRef Name="id" />
          </Key>
          <Property Name="id" Type="Edm.String" Nullable="false" />
        </EntityType>
        <EntityContainer Name="GraphService">
          <EntitySet Name="users" EntityType="microsoft.graph.user" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>
""";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csdl)))
            {
                using (var xmlReader = XmlReader.Create(stream))
                {
                    var model = Microsoft.OData.Edm.Csdl.CsdlReader.Parse(xmlReader);

                    var original = "users/myid/calendar/events?$filter=id%20eq%20%27thisisatest%27";
                    var odataUri = new Microsoft.OData.UriParser.ODataUriParser(
                        model, 
                        new Uri(original, UriKind.Relative))
                        .ParseUri();
                    var uri = Microsoft.OData.ODataUriExtensions.BuildUri(odataUri, Microsoft.OData.ODataUrlKeyDelimiter.Slash).ToString();

                    Assert.AreEqual(original, uri);
                }
            }
        }

        [TestMethod]
        public void GenerateOdataWithLatest()
        {
            var fullRulesPath = @"C:\github\odata.net\odata\odata.abnf";
            var fullRulesText = File.ReadAllText(fullRulesPath);

            GenerateParserTypes(
                fullRulesText,
                true,
                "__GeneratedOdata.CstNodes.Rules",
                "__GeneratedOdata.CstNodes.Inners",
                @"C:\github\odata.net\odata\__GeneratedOdata\CstNodes\Rules",
                @"C:\github\odata.net\odata\__GeneratedOdata\CstNodes\Inners",
                "__GeneratedOdata.Trancsribers.Rules",
                "__GeneratedOdata.Trancsribers.Inners",
                @"C:\github\odata.net\odata\__GeneratedOdata\Transcribers\Rules",
                @"C:\github\odata.net\odata\__GeneratedOdata\Transcribers\Inners",
                "__GeneratedOdata.Parsers.Rules",
                "__GeneratedOdata.Parsers.Inners",
                @"C:\github\odata.net\odata\__GeneratedOdata\Parsers\Rules",
                @"C:\github\odata.net\odata\__GeneratedOdata\Parsers\Inners");
        }

        [TestMethod]
        public void V2GenerateOdataWithLatest()
        {
            var fullRulesPath = @"C:\github\odata.net\odata\odata.abnf";
            var fullRulesText = File.ReadAllText(fullRulesPath);

            GenerateParserTypes(
                fullRulesText,
                true,
                "__GeneratedOdataV2.CstNodes.Rules",
                "__GeneratedOdataV2.CstNodes.Inners",
                @"C:\github\odata.net\odata\__GeneratedOdataV2\CstNodes\Rules",
                @"C:\github\odata.net\odata\__GeneratedOdataV2\CstNodes\Inners",
                "__GeneratedOdataV2.Trancsribers.Rules",
                "__GeneratedOdataV2.Trancsribers.Inners",
                @"C:\github\odata.net\odata\__GeneratedOdataV2\Transcribers\Rules",
                @"C:\github\odata.net\odata\__GeneratedOdataV2\Transcribers\Inners",
                "__GeneratedOdataV2.Parsers.Rules",
                "__GeneratedOdataV2.Parsers.Inners",
                @"C:\github\odata.net\odata\__GeneratedOdataV2\Parsers\Rules",
                @"C:\github\odata.net\odata\__GeneratedOdataV2\Parsers\Inners",
                true);
        }

        [TestMethod]
        public void GenerateAbnfWithLatest()
        {
            var coreRulesPath = @"C:\github\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\github\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);

            GenerateParserTypes(
                fullRulesText,
                false,
                "__Generated.CstNodes.Rules",
                "__Generated.CstNodes.Inners",
                @"C:\github\odata.net\odata\__Generated\CstNodes\Rules",
                @"C:\github\odata.net\odata\__Generated\CstNodes\Inners",
                "__Generated.Trancsribers.Rules",
                "__Generated.Trancsribers.Inners",
                @"C:\github\odata.net\odata\__Generated\Transcribers\Rules",
                @"C:\github\odata.net\odata\__Generated\Transcribers\Inners",
                "__Generated.Parsers.Rules",
                "__Generated.Parsers.Inners",
                @"C:\github\odata.net\odata\__Generated\Parsers\Rules",
                @"C:\github\odata.net\odata\__Generated\Parsers\Inners");
        }

        private static void GenerateParserTypes(
            string fullRulesText,
            bool useNumericFileNames,
            string ruleCstNodesNamespace,
            string innerCstNodesNamespace,
            string ruleCstNodesDirectory,
            string innerCstNodesDirectory,
            string ruleTranscribersNamespace,
            string innerTranscribersNamespace,
            string ruleTranscribersDirectory,
            string innerTranscibersDirectory,
            string ruleParsersNamespace,
            string innerParsersNamespace,
            string ruleParsersDirectory,
            string innerParsersDirectory,
            bool optimizeSingletons = false
            )
        {
            if (!__Generated.Parsers.Rules._rulelistParser.Instance.TryParse(fullRulesText, out var cst))
            {
                throw new Exception("TODO");
            }

            var generatedCstNodes = new _GeneratorV5.CstNodesGenerator(ruleCstNodesNamespace, innerCstNodesNamespace).Generate(cst);

            if (optimizeSingletons)
            {
                generatedCstNodes = new _GeneratorV6.CstNodesOptimizer().Optimize(generatedCstNodes);
            }

            Directory.CreateDirectory(ruleCstNodesDirectory);
            TranscribeNamespace(generatedCstNodes.RuleCstNodes, ruleCstNodesDirectory, useNumericFileNames);
            Directory.CreateDirectory(innerCstNodesDirectory);
            TranscribeNamespace(generatedCstNodes.InnerCstNodes, innerCstNodesDirectory, useNumericFileNames);

            var generatedTranscribers = new _GeneratorV5.TranscribersGenerator(ruleTranscribersNamespace, innerTranscribersNamespace).Generate(generatedCstNodes);

            //// TODO transcriber generator should return namespaces
            Directory.CreateDirectory(ruleTranscribersDirectory);
            TranscribeNamespace(
                new Namespace(
                    ruleTranscribersNamespace,
                    generatedTranscribers.Rules.ToList()),
                ruleTranscribersDirectory,
                useNumericFileNames);
            Directory.CreateDirectory(innerTranscibersDirectory);
            TranscribeNamespace(
                new Namespace(
                    innerTranscribersNamespace,
                    generatedTranscribers.Inners.ToList()),
                innerTranscibersDirectory,
                useNumericFileNames);

            var generatedParsers = new _GeneratorV5.ParsersGenerator(ruleParsersNamespace, innerParsersNamespace).Generate(generatedCstNodes);

            generatedParsers.RuleParsers.Classes.ToList();
            generatedParsers.InnerParsers.Classes.ToList();

            Directory.CreateDirectory(ruleParsersDirectory);
            TranscribeNamespace(generatedParsers.RuleParsers, ruleParsersDirectory, useNumericFileNames);
            Directory.CreateDirectory(innerParsersDirectory);
            TranscribeNamespace(generatedParsers.InnerParsers, innerParsersDirectory, useNumericFileNames);
        }

        private static void TranscribeNamespace(Namespace @namespace, string folderPath, bool useNumericFileNames)
        {
            int i = 0;
            foreach (var @class in @namespace.Classes)
            {
                string filePath;
                if (useNumericFileNames)
                {
                    filePath = Path.Combine(folderPath, $"{i.ToString()}.cs");
                    ++i;
                }
                else
                {
                    filePath = Path.Combine(folderPath, $"{@class.Name}.cs");
                }

                var classTranscriber = new ClassTranscriber();

                var stringBuilder = new StringBuilder();
                var builder = new Builder(stringBuilder, "    ");
                builder.AppendLine($"namespace {@namespace.Name}");
                builder.AppendLine("{");
                builder.Indent();
                var usings = false;
                foreach (var usingDeclaration in @namespace.UsingDeclarations)
                {
                    builder.AppendLine($"using {usingDeclaration};");
                    usings = true;
                }

                if (usings)
                {
                    builder.AppendLine();
                }

                classTranscriber.Transcribe(@class, builder);
                builder.AppendLine();

                builder.Unindent();
                builder.AppendLine("}");

                var csharp = stringBuilder.ToString();

                File.WriteAllText(filePath, csharp);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var url = "$metadata";
            var cst = OdataRelativeUriParser.Instance.Parse(url);

            var ast = Root.OdataResourcePath.CstToAstConverters.OdataRelativeUriConverter
                .Default
                .Visit(cst, default);

            var newCst = Root.OdataResourcePath.AstToCstConverters.OdataRelativeUriConverter
                .Default
                .Visit(ast, default);

            var stringBuilder = new StringBuilder();
            OdataRelativeUriTranscriber.Default.Visit(newCst, stringBuilder);

            Assert.AreEqual(url, stringBuilder.ToString());
        }

        [TestMethod]
        public void CoreRulesV5()
        {
            var coreRulesPath = @"C:\github\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            if (!__Generated.Parsers.Rules._rulelistParser.Instance.TryParse(coreRulesText, out var cst))
            {
                throw new Exception("TODO");
            }

            //// TODO if the ABNF is missing a trailing newline, the last rule will be dropped

            ////var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(cst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(coreRulesText, transcribedText);
        }

        [TestMethod]
        public void AbnfRulesV5()
        {
            var coreRulesPath = @"C:\github\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\github\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            if (!__Generated.Parsers.Rules._rulelistParser.Instance.TryParse(fullRulesText, out var cst))
            {
                throw new Exception("TODO");
            }

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(cst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(fullRulesText, transcribedText);
        }

        [TestMethod]
        public void OdataRulesV5()
        {
            var odataRulesPath = @"C:\github\odata.net\odata\odata.abnf";
            var odataRulesText = File.ReadAllText(odataRulesPath);
            if (!__Generated.Parsers.Rules._rulelistParser.Instance.TryParse(odataRulesText, out var cst))
            {
                throw new Exception("TODO");
            }

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(cst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(odataRulesText, transcribedText);
        }

        [TestMethod]
        public void TestRulesV5()
        {
            if (!__Generated.Parsers.Rules._rulelistParser.Instance.TryParse(TestAbnf, out var cst))
            {
                throw new Exception("TODO");
            }

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(cst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(TestAbnf, transcribedText);
        }

        [TestMethod]
        public void TestRules2V5()
        {
            var abnf = File.ReadAllText(@"C:\github\odata.net\odata\GeneratorV3\test.abnf");
            if (!__Generated.Parsers.Rules._rulelistParser.Instance.TryParse(abnf, out var cst))
            {
                throw new Exception("TODO");
            }

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(cst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(TestAbnf, transcribedText);
        }

        private static string TestAbnf =
"""
first-rule = second-rule
second-rule = first-rule 
            / first-rule first-rule 
            / first-rule *(first-rule [first-rule]) *[first-rule first-rule] *1first-rule 1*first-rule 
            / (first-rule / (first-rule [first-rule]))
            / first-rule (first-rule [first-rule]) [first-rule first-rule]

""";

        private sealed class Builder
        {
            private readonly StringBuilder builder;
            private readonly string indent;
            private string currentIndent;
            private bool isNewLine;

            public Builder(StringBuilder builder, string indent)
            {
                this.builder = builder;
                this.indent = indent;

                this.currentIndent = string.Empty;
                this.isNewLine = true;
            }

            private void AppendIndent()
            {
                if (this.isNewLine)
                {
                    this.builder.Append(this.currentIndent);
                }
            }

            public Builder Append(string value)
            {
                this.AppendIndent();
                this.isNewLine = false;
                this.builder.Append(value);
                return this;
            }

            public Builder AppendLine()
            {
                this.AppendIndent();
                this.isNewLine = true;
                this.builder.AppendLine();
                return this;
            }

            public Builder AppendLine(string value)
            {
                this.AppendIndent();
                this.isNewLine = true;
                this.builder.AppendLine(value);
                return this;
            }

            public Builder AppendJoin(string separator, IEnumerable<string> values)
            {
                this.AppendIndent();
                this.isNewLine = false;
                this.builder.AppendJoin(separator, values);
                return this;
            }

            public Builder Indent()
            {
                this.currentIndent += this.indent;
                return this;
            }

            public Builder Unindent()
            {
                this.currentIndent = this.currentIndent.Substring(this.indent.Length);
                return this;
            }

            public Builder AppendJoin<TElement>(string separator, IEnumerable<TElement> values, Action<TElement, Builder> selector)
            {
                this.AppendIndent();
                this.isNewLine = false;
                using (var valuesEnumerator = values.GetEnumerator())
                {
                    if (!valuesEnumerator.MoveNext())
                    {
                        return this;
                    }

                    selector(valuesEnumerator.Current, this);
                    while (valuesEnumerator.MoveNext())
                    {
                        this.Append(separator);
                        selector(valuesEnumerator.Current, this);
                    }

                    return this;
                }
            }
        }

        private sealed class ClassTranscriber
        {
            public void Transcribe(AbnfParserGenerator.Class @class, StringBuilder stringBuilder, string indent)
            {
                Transcribe(@class, new Builder(stringBuilder, indent));
            }

            private void Transcribe(AbnfParserGenerator.AccessModifier accessModifier, Builder builder)
            {
                if (accessModifier.HasFlag(AbnfParserGenerator.AccessModifier.Private))
                {
                    builder.Append($"private ");
                }

                if (accessModifier.HasFlag(AbnfParserGenerator.AccessModifier.Protected))
                {
                    builder.Append($"protected ");
                }

                if (accessModifier.HasFlag(AbnfParserGenerator.AccessModifier.Internal))
                {
                    builder.Append($"internal ");
                }

                if (accessModifier.HasFlag(AbnfParserGenerator.AccessModifier.Public))
                {
                    builder.Append($"public ");
                }
            }

            public void Transcribe(AbnfParserGenerator.Class @class, Builder builder)
            {
                Transcribe(@class.AccessModifier, builder);
                if (@class.ClassModifier == ClassModifier.Abstract)
                {
                    builder.Append("abstract ");
                }
                else if (@class.ClassModifier == ClassModifier.Sealed)
                {
                    builder.Append("sealed ");
                }
                else if (@class.ClassModifier == ClassModifier.Static)
                {
                    builder.Append("static ");
                }

                builder.Append("class ").Append(@class.Name);
                var genericTypeParameters = string.Join(", ", @class.GenericTypeParameters);
                if (!string.IsNullOrEmpty(genericTypeParameters))
                {
                    builder.Append("<").Append(genericTypeParameters).Append(">");
                }

                var baseType = @class.BaseType;
                if (baseType != null)
                {
                    builder.Append(" : ").Append(baseType);
                }

                builder.AppendLine();
                builder.AppendLine("{");
                builder.Indent();
                var needsNewLine = false;
                if (@class.Constructors.Any()) //// TODO you shouldn't have to do this check if you join properly
                {
                    if (needsNewLine)
                    {
                        builder.AppendLine();
                    }

                    needsNewLine = true;
                    foreach (var constructor in @class.Constructors)
                    {
                        Transcribe(constructor.AccessModifier, builder);
                        builder.Append(@class.Name).Append("(");
                        builder.AppendJoin(", ", constructor.Parameters, (parameter, b) => b.Append(parameter.Type).Append(" ").Append(parameter.Name));
                        builder.AppendLine(")");
                        builder.AppendLine("{");
                        builder.Indent();
                        foreach (var bodyLine in constructor.Body)
                        {
                            builder.AppendLine(bodyLine);
                        }

                        builder.Unindent();
                        builder.AppendLine("}");
                    }
                }

                if (@class.Properties.Any())
                {
                    if (needsNewLine)
                    {
                        builder.AppendLine();
                    }

                    needsNewLine = true;
                    foreach (var property in @class.Properties)
                    {
                        Transcribe(property.AccessModifier, builder);
                        if (property.IsStatic)
                        {
                            builder.Append("static ");
                        }

                        builder.Append(property.Type).Append(" ");
                        builder.Append(property.Name).Append(" ");
                        var needsBrace = true;
                        if (property.HasGet)
                        {
                            if (needsBrace)
                            {
                                builder.Append("{ ");
                            }

                            needsBrace = false;
                            builder.Append("get; ");
                        }

                        if (property.HasSet)
                        {
                            if (needsBrace)
                            {
                                builder.Append("{ ");
                            }

                            needsBrace = false;
                            builder.Append("set; ");
                        }

                        if (!needsBrace)
                        {
                            builder.Append("}");
                        }

                        if (property.Initializer != null)
                        {
                            builder.Append($" = {property.Initializer}");
                        }

                        builder.AppendLine();
                    }
                }

                if (@class.Methods.Any())
                {
                    if (needsNewLine)
                    {
                        builder.AppendLine();
                    }

                    needsNewLine = true;
                    var methodNewLine = false;
                    foreach (var method in @class.Methods)
                    {
                        if (methodNewLine)
                        {
                            builder.AppendLine();
                        }

                        Transcribe(method.AccessModifier, builder);
                        if (method.ClassModifier == ClassModifier.Abstract)
                        {
                            builder.Append("abstract ");
                        }
                        else if (method.ClassModifier == ClassModifier.Sealed)
                        {
                            builder.Append("sealed ");
                        }
                        else if (method.ClassModifier == ClassModifier.Static)
                        {
                            builder.Append("static ");
                        }

                        if (method.IsOverride)
                        {
                            builder.Append("override ");
                        }

                        builder.Append(method.ReturnType).Append(" ");
                        builder.Append(method.Name);
                        var methodGenericTypeParameters = string.Join(", ", method.GenericTypeParameters);
                        if (!string.IsNullOrEmpty(methodGenericTypeParameters))
                        {
                            builder.Append("<").Append(methodGenericTypeParameters).Append(">");
                        }

                        builder.Append("(");
                        builder.AppendJoin(", ", method.Parameters, (parameter, b) => b.Append(parameter.Type).Append(" ").Append(parameter.Name));
                        builder.Append(")");
                        if (method.Body == null)
                        {
                            builder.AppendLine(";");
                            methodNewLine = false;
                        }
                        else
                        {
                            builder.AppendLine();
                            builder.AppendLine("{");
                            builder.Indent();
                            builder.AppendLine(method.Body);
                            builder.Unindent();
                            builder.AppendLine("}");
                            methodNewLine = true;
                        }
                    }
                }

                foreach (var nestedClass in @class.NestedClasses)
                {
                    if (needsNewLine)
                    {
                        builder.AppendLine();
                    }

                    needsNewLine = true;
                    Transcribe(nestedClass, builder);
                }

                builder.Unindent();
                builder.AppendLine("}");
            }
        }
    }
}
