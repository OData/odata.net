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
    using static AbnfParser.CstNodes.RuleList.Inner;

    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void GeneratorWithLatest()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);

            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);
            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var ruleCstNodesNamespace = "__Generated.CstNodes.Rules";
            var innerCstNodesNamespace = "__Generated.CstNodes.Inners";
            var generatedCstNodes = new _GeneratorV5.CstNodesGenerator(ruleCstNodesNamespace, innerCstNodesNamespace).Generate(newCst);

            TranscribeNamespace(generatedCstNodes.RuleCstNodes, @"C:\msgithub\odata.net\odata\__Generated\CstNodes\Rules");
            TranscribeNamespace(generatedCstNodes.InnerCstNodes, @"C:\msgithub\odata.net\odata\__Generated\CstNodes\Inners");

            var ruleTranscribersNamespace = "__Generated.Trancsribers.Rules";
            var innerTranscribersNamespace = "__Generated.Trancsribers.Inners";
            var generatedTranscribers = new _GeneratorV5.TranscribersGenerator(ruleTranscribersNamespace, innerTranscribersNamespace).Generate(generatedCstNodes);

            TranscribeNamespace(
                new Namespace(
                    ruleTranscribersNamespace,
                    generatedTranscribers.Rules),
                @"C:\msgithub\odata.net\odata\__Generated\Transcribers\Rules");
            TranscribeNamespace(
                new Namespace(
                    innerTranscribersNamespace,
                    generatedTranscribers.Inners),
                @"C:\msgithub\odata.net\odata\__Generated\Transcribers\Inners");

            //// TODO finish this
        }

        private static void TranscribeNamespace(Namespace @namespace, string folderPath)
        {
            foreach (var @class in @namespace.Classes)
            {
                var filePath = Path.Combine(folderPath, $"{@class.Name}.cs");
                var classTranscriber = new ClassTranscriber();

                var stringBuilder = new StringBuilder();
                var builder = new Builder(stringBuilder, "    ");
                builder.AppendLine($"namespace {@namespace.Name}");
                builder.AppendLine("{");
                builder.Indent();
                classTranscriber.Transcribe(@class, builder);
                builder.AppendLine();

                builder.Unindent();
                builder.AppendLine("}");

                var csharp = stringBuilder.ToString();

                File.WriteAllText(filePath, csharp);
            }
        }

        [TestMethod]
        public void TranscribersV5()
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
        public void CoreRules()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(coreRulesText);

            //// TODO if the ABNF is missing a trailing newline, the last rule will be dropped

            var newCst = GeneratorV3.OldToNewConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            GeneratorV3.SpikeTranscribers.Rules.RuleListTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(coreRulesText, transcribedText);
        }

        [TestMethod]
        public void AbnfRules()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var newCst = GeneratorV3.OldToNewConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            GeneratorV3.SpikeTranscribers.Rules.RuleListTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(fullRulesText, transcribedText);
        }

        [TestMethod]
        public void OdataRules()
        {
            var odataRulesPath = @"C:\msgithub\odata.net\odata\odata.abnf";
            var odataRulesText = File.ReadAllText(odataRulesPath);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(odataRulesText);

            var newCst = GeneratorV3.OldToNewConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            GeneratorV3.SpikeTranscribers.Rules.RuleListTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            File.WriteAllText(odataRulesPath, transcribedText);
            Assert.AreEqual(odataRulesText, transcribedText);
        }

        [TestMethod]
        public void TestGeneratorV3()
        {
            var abnf = File.ReadAllText(@"C:\msgithub\odata.net\odata\GeneratorV3\test.abnf");
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(abnf);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var @namespace = "GeneratorV3";
            var classes = new _GeneratorV4.Generator(@namespace).Generate(newCst);

            var classTranscriber = new ClassTranscriber();

            var stringBuilder = new StringBuilder();
            var builder = new Builder(stringBuilder, "    ");
            builder.AppendLine($"namespace {@namespace}");
            builder.AppendLine("{");
            builder.Indent();
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine();

            foreach (var @class in classes)
            {
                classTranscriber.Transcribe(@class, builder);
                builder.AppendLine();
            }

            builder.Unindent();
            builder.AppendLine("}");

            var csharp = stringBuilder.ToString();

            var resultFilePath = @"C:\msgithub\odata.net\odata\GeneratorV3\TestNodes2.result";
            File.WriteAllText(resultFilePath, csharp);

            var expectedFilePath = @"C:\msgithub\odata.net\odata\GeneratorV3\TestNodes.cs";
            var expected = File.ReadAllText(expectedFilePath);

            Assert.AreEqual(expected, csharp);
        }

        [TestMethod]
        public void TestCodeGeneratorV4()
        {
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(TestAbnf);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var @namespace = "TestRules";
            var classes = new _GeneratorV4.Generator(@namespace).Generate(newCst);

            var classTranscriber = new ClassTranscriber();
            var builder = new StringBuilder();
            foreach (var @class in classes)
            {
                classTranscriber.Transcribe(@class, builder, "    ");
                builder.AppendLine();
            }

            var csharp = builder.ToString();

            var filePath = @"C:\msgithub\odata.net\odata.tests\testclasses.txt";
            var expected = File.ReadAllText(filePath);

            File.WriteAllText(filePath, csharp);

            Assert.AreEqual(expected, csharp);
        }

        [TestMethod]
        public void GenerateForCoreV4()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var @namespace = "GeneratorV3.Core";
            var classes = new _GeneratorV4.Generator(@namespace).Generate(newCst);

            var classTranscriber = new ClassTranscriber();

            var stringBuilder = new StringBuilder();
            var builder = new Builder(stringBuilder, "    ");
            builder.AppendLine($"namespace {@namespace}");
            builder.AppendLine("{");
            builder.Indent();
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine();

            foreach (var @class in classes)
            {
                classTranscriber.Transcribe(@class, builder);
                builder.AppendLine();
            }

            builder.Unindent();
            builder.AppendLine("}");

            var csharp = stringBuilder.ToString();

            var expectedFilePath = @"C:\msgithub\odata.net\odata\GeneratorV3\Core.cs";
            var expected = File.ReadAllText(expectedFilePath);
            Assert.AreEqual(expected, csharp);

            File.WriteAllText(expectedFilePath, csharp);
        }

        [TestMethod]
        public void GenerateForAbnfV4()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var @namespace = "GeneratorV3.Abnf";
            var classes = new _GeneratorV4.Generator(@namespace).Generate(newCst);

            var classTranscriber = new ClassTranscriber();

            var stringBuilder = new StringBuilder();
            var builder = new Builder(stringBuilder, "    ");
            builder.AppendLine($"namespace {@namespace}");
            builder.AppendLine("{");
            builder.Indent();
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine();

            foreach (var @class in classes)
            {
                classTranscriber.Transcribe(@class, builder);
                builder.AppendLine();
            }

            builder.Unindent();
            builder.AppendLine("}");

            var csharp = stringBuilder.ToString();

            var expectedFilePath = @"C:\msgithub\odata.net\odata\GeneratorV3\abnf.cs";
            var expected = File.ReadAllText(expectedFilePath);
            Assert.AreEqual(expected, csharp);

            File.WriteAllText(expectedFilePath, csharp);
        }

        [TestMethod]
        public void GenerateTranscribersForAbnfV4()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var @namespace = "Test.CstNodes";
            var cstNodes = new _GeneratorV4.Generator(@namespace).Generate(newCst);

            var rulesNamespace = "Test.Transcribers.Rules";
            var innersNamespace = "Test.Transcribers.Inners";
            var transcribers = new TranscribersGenerator(rulesNamespace, innersNamespace).Generate(cstNodes);
            var danglingRules = transcribers
                .Rules
                .Where(
                    rule => rule
                        .Methods
                        .Where(method => method.Name == "Transcribe")
                        .Where(method => string.IsNullOrEmpty(method.Body))
                        .Any())
                .ToList();
            Assert.IsFalse(danglingRules.Any());

            var rulesFilePath = @"C:\msgithub\odata.net\odata\GeneratorV3\GeneratedTranscribers\Rules.cs";
            var expectedRulesText = File.ReadAllText(rulesFilePath);
            var innersFilePath = @"C:\msgithub\odata.net\odata\GeneratorV3\GeneratedTranscribers\Inners.cs";
            var expectedInnersText = File.ReadAllText(innersFilePath);

            TranscribeClasses(rulesNamespace, rulesFilePath, transcribers.Rules);
            TranscribeClasses(innersNamespace, innersFilePath, transcribers.Inners);

            var actualRulesText = File.ReadAllText(rulesFilePath);
            Assert.AreEqual(expectedRulesText, actualRulesText);

            var actualInnersText = File.ReadAllText(innersFilePath);
            Assert.AreEqual(expectedInnersText, actualInnersText);
        }

        private static void TranscribeClasses(string @namespace, string filePath, IEnumerable<Class> classes)
        {
            var classTranscriber = new ClassTranscriber();

            var stringBuilder = new StringBuilder();
            var builder = new Builder(stringBuilder, "    ");
            builder.AppendLine($"namespace {@namespace}");
            builder.AppendLine("{");
            builder.Indent();
            builder.AppendLine("using System.Text;");
            builder.AppendLine();
            builder.AppendLine("using GeneratorV3;");
            builder.AppendLine("using GeneratorV3.Abnf;");
            builder.AppendLine();

            foreach (var @class in classes)
            {
                classTranscriber.Transcribe(@class, builder);
                builder.AppendLine();
            }

            builder.Unindent();
            builder.AppendLine("}");

            var csharp = stringBuilder.ToString();

            File.WriteAllText(filePath, csharp);
        }

        [TestMethod]
        public void GenerateForOdataV4()
        {
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\odata.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(abnfRulesText);

            var newCst = _GeneratorV5.OldToGeneratedCstConverters.RuleListConverter.Instance.Convert(cst);

            var @namespace = "GeneratorV3.Odata";
            var classes = new _GeneratorV4.Generator(@namespace).Generate(newCst);

            var classTranscriber = new ClassTranscriber();

            var stringBuilder = new StringBuilder();
            var builder = new Builder(stringBuilder, "    ");
            builder.AppendLine($"namespace {@namespace}");
            builder.AppendLine("{");
            builder.Indent();
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine();

            foreach (var @class in classes)
            {
                classTranscriber.Transcribe(@class, builder);
                builder.AppendLine();
            }

            builder.Unindent();
            builder.AppendLine("}");

            var csharp = stringBuilder.ToString();

            var expectedFilePath = @"C:\msgithub\odata.net\odata\odata.cs";
            var expected = File.ReadAllText(expectedFilePath);
            Assert.AreEqual(expected, csharp);

            File.WriteAllText(expectedFilePath, csharp);
        }

        [TestMethod]
        public void AbnfRulesWithNewNodes()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var newCst = GeneratorV3.OldToNewConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            GeneratorV3.SpikeTranscribers.Rules.RuleListTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(fullRulesText, transcribedText);
        }

        [TestMethod]
        public void AbnfRulesWithGeneratedTranscribers()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var newCst = GeneratorV3.OldToNewConverters.RuleListConverter.Instance.Convert(cst);

            var stringBuilder = new StringBuilder();

            Test.Transcribers.Rules._rulelistTranscriber.Instance.Transcribe(newCst, stringBuilder);

            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(fullRulesText, transcribedText);
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
