namespace odata.tests
{
    using _GeneratorV4;
    using AbnfParser.CstNodes.Core;
    using AbnfParserGenerator;
    using GeneratorV3;
    using Root;
    using Root.OdataResourcePath.CombinatorParsers;
    using Root.OdataResourcePath.Transcribers;
    using Sprache;
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using static AbnfParser.CstNodes.RuleList.Inner;

    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void OdataTest1()
        {
            var url = "https://graph.microsoft.com/v1.0/$metadata";
            var urlCst = __GeneratedOdata.Parsers.Rules._odataUriParser.Instance.Parse(url);
            var stringBuilder = new StringBuilder();
            __GeneratedOdata.Trancsribers.Rules._odataUriTranscriber.Instance.Transcribe(urlCst, stringBuilder);
            var transcribed = stringBuilder.ToString();
            Assert.AreEqual(url, transcribed);
        }

        [TestMethod]
        public void OdataTest2()
        {
            var url = "users/myid/calendar/events?$filter=id eq 'thisisatest'";
            var urlCst = __GeneratedOdata.Parsers.Rules._odataRelativeUriParser.Instance.Parse(url);
            var stringBuilder = new StringBuilder();
            __GeneratedOdata.Trancsribers.Rules._odataRelativeUriTranscriber.Instance.Transcribe(urlCst, stringBuilder);
            var transcribed = stringBuilder.ToString();
            Assert.AreEqual(url, transcribed);
        }

        /*[TestMethod]
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
                    var model = CsdlReader.Parse(xmlReader);

                    var original = "users/myid/calendar/events?$filter=id%20eq%20%27thisisatest%27";
                    var odataUri = new Microsoft.OData.UriParser.ODataUriParser(
                        model, 
                        new Uri(original, UriKind.Relative))
                        .ParseUri();
                    var uri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash).ToString();

                    Assert.AreEqual(original, uri);
                }
            }
        }*/

        [TestMethod]
        public void GenerateOdataWithLatest()
        {
            var fullRulesPath = @"C:\msgithub\odata.net\odata\odata.abnf";
            var fullRulesText = File.ReadAllText(fullRulesPath);

            GenerateParserTypes(
                fullRulesText,
                true,
                "__GeneratedOdata.CstNodes.Rules",
                "__GeneratedOdata.CstNodes.Inners",
                @"C:\msgithub\odata.net\odata\__GeneratedOdata\CstNodes\Rules",
                @"C:\msgithub\odata.net\odata\__GeneratedOdata\CstNodes\Inners",
                "__GeneratedOdata.Trancsribers.Rules",
                "__GeneratedOdata.Trancsribers.Inners",
                @"C:\msgithub\odata.net\odata\__GeneratedOdata\Transcribers\Rules",
                @"C:\msgithub\odata.net\odata\__GeneratedOdata\Transcribers\Inners",
                "__GeneratedOdata.Parsers.Rules",
                "__GeneratedOdata.Parsers.Inners",
                @"C:\msgithub\odata.net\odata\__GeneratedOdata\Parsers\Rules",
                @"C:\msgithub\odata.net\odata\__GeneratedOdata\Parsers\Inners");
        }

        [TestMethod]
        public void GenerateAbnfWithLatest()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);

            GenerateParserTypes(
                fullRulesText,
                false,
                "__Generated.CstNodes.Rules",
                "__Generated.CstNodes.Inners",
                @"C:\msgithub\odata.net\odata\__Generated\CstNodes\Rules",
                @"C:\msgithub\odata.net\odata\__Generated\CstNodes\Inners",
                "__Generated.Trancsribers.Rules",
                "__Generated.Trancsribers.Inners",
                @"C:\msgithub\odata.net\odata\__Generated\Transcribers\Rules",
                @"C:\msgithub\odata.net\odata\__Generated\Transcribers\Inners",
                "__Generated.Parsers.Rules",
                "__Generated.Parsers.Inners",
                @"C:\msgithub\odata.net\odata\__Generated\Parsers\Rules",
                @"C:\msgithub\odata.net\odata\__Generated\Parsers\Inners");
        }

        [TestMethod]
        public void GenerateAbnfWithLatest_2()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);

            GenerateParserTypes(
                fullRulesText,
                false,
                "__GeneratedV2.CstNodes.Rules",
                "__GeneratedV2.CstNodes.Inners",
                @"C:\msgithub\odata.net\odata\__GeneratedV2\CstNodes\Rules",
                @"C:\msgithub\odata.net\odata\__GeneratedV2\CstNodes\Inners",
                "__Generated.Trancsribers.Rules",
                "__Generated.Trancsribers.Inners",
                @"C:\msgithub\odata.net\odata\__Generated\Transcribers\Rules",
                @"C:\msgithub\odata.net\odata\__Generated\Transcribers\Inners",
                "__Generated.Parsers.Rules",
                "__Generated.Parsers.Inners",
                @"C:\msgithub\odata.net\odata\__Generated\Parsers\Rules",
                @"C:\msgithub\odata.net\odata\__Generated\Parsers\Inners");
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
            string innerParsersDirectory
            )
        {
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);
            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var generatedCstNodes = new _GeneratorV5.CstNodesGenerator(ruleCstNodesNamespace, innerCstNodesNamespace).Generate(newCst);

            Directory.CreateDirectory(ruleCstNodesDirectory);
            TranscribeNamespace(generatedCstNodes.RuleCstNodes, ruleCstNodesDirectory, useNumericFileNames);
            Directory.CreateDirectory(innerCstNodesDirectory);
            TranscribeNamespace(generatedCstNodes.InnerCstNodes, innerCstNodesDirectory, useNumericFileNames);

            /*var generatedTranscribers = new _GeneratorV5.TranscribersGenerator(ruleTranscribersNamespace, innerTranscribersNamespace).Generate(generatedCstNodes);

            //// TODO transcriber generator should return namespaces
            Directory.CreateDirectory(ruleTranscribersDirectory);
            TranscribeNamespace(
                new Namespace(
                    ruleTranscribersNamespace,
                    generatedTranscribers.Rules),
                ruleTranscribersDirectory,
                useNumericFileNames);
            Directory.CreateDirectory(innerTranscibersDirectory);
            TranscribeNamespace(
                new Namespace(
                    innerTranscribersNamespace,
                    generatedTranscribers.Inners),
                innerTranscibersDirectory,
                useNumericFileNames);

            var generatedParsers = new _GeneratorV5.ParsersGenerator(ruleParsersNamespace, innerParsersNamespace).Generate(generatedCstNodes);

            Directory.CreateDirectory(ruleParsersDirectory);
            TranscribeNamespace(generatedParsers.RuleParsers, ruleParsersDirectory, useNumericFileNames);
            Directory.CreateDirectory(innerParsersDirectory);
            TranscribeNamespace(generatedParsers.InnerParsers, innerParsersDirectory, useNumericFileNames);*/
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
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(coreRulesText);

            //// TODO if the ABNF is missing a trailing newline, the last rule will be dropped

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(coreRulesText, transcribedText);
        }

        [TestMethod]
        public void CoreRulesV5_2()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var cst = __Generated.Parsers.Rules._rulelistParser.Instance.Parse(coreRulesText);

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
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(fullRulesText, transcribedText);
        }

        [TestMethod]
        public void AbnfRulesV5_2()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = __Generated.Parsers.Rules._rulelistParser.Instance.TryParse(fullRulesText);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(cst.Value, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(fullRulesText, transcribedText);
        }

        [TestMethod]
        public void OdataRulesV5()
        {
            var odataRulesPath = @"C:\msgithub\odata.net\odata\odata.abnf";
            var odataRulesText = File.ReadAllText(odataRulesPath);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(odataRulesText);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(odataRulesText, transcribedText);
        }

        [TestMethod]
        public void OdataRulesV5_2()
        {
            var odataRulesPath = @"C:\msgithub\odata.net\odata\odata.abnf";
            var odataRulesText = File.ReadAllText(odataRulesPath);
            var cst = __Generated.Parsers.Rules._rulelistParser.Instance.Parse(odataRulesText);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(cst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(odataRulesText, transcribedText);
        }

        [TestMethod]
        public void TestRulesV5()
        {
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(TestAbnf);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(TestAbnf, transcribedText);
        }

        [TestMethod]
        public void TestRulesV5_2()
        {
            var cst = __Generated.Parsers.Rules._rulelistParser.Instance.Parse(TestAbnf);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(cst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(TestAbnf, transcribedText);
        }

        [TestMethod]
        public void TestRules2V5()
        {
            var abnf = File.ReadAllText(@"C:\msgithub\odata.net\odata\GeneratorV3\test.abnf");
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(abnf);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            __Generated.Trancsribers.Rules._rulelistTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(TestAbnf, transcribedText);
        }

        [TestMethod]
        public void TestRules2V5_2()
        {
            var abnf = File.ReadAllText(@"C:\msgithub\odata.net\odata\GeneratorV3\test.abnf");
            var cst = __Generated.Parsers.Rules._rulelistParser.Instance.Parse(abnf);

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
