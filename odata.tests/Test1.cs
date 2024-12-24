namespace odata.tests
{
    using AbnfParser.CstNodes.Core;
    using AbnfParserGenerator;
    using AbnfParserGenerator.CstNodesGenerator;
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
        public void GenerateAlphaTranscribers()
        {
            var range = (0x01, 0x7E);

            var builder = new StringBuilder();
            for (int i = range.Item1; i <= range.Item2; ++i)
            {
                var className = $"x{i:X2}";
                builder.AppendLine($"public sealed class {className}Transcriber");
                builder.AppendLine("{");
                builder.AppendLine($"private {className}Transcriber()");
                builder.AppendLine("{");
                builder.AppendLine("}");
                builder.AppendLine();
                builder.AppendLine($"public static {className}Transcriber Instance {{ get; }} = new {className}Transcriber();");
                builder.AppendLine();
                builder.AppendLine($"public Void Transcribe({className} node, StringBuilder context)");
                builder.AppendLine("{");
                builder.AppendLine($"context.Append((char)0{className});");
                builder.AppendLine("return default;");
                builder.AppendLine("}");
                builder.AppendLine("}");
            }

            File.WriteAllText(@"C:\Users\gdebruin\code.txt", builder.ToString());
        }

        [TestMethod]
        public void GenerateTranscriber()
        {
            var ranges = new[]
            {
                (0x20, 0x3D),
                (0x3F, 0x7E),
            };
            var elementName = "ProseVal";

            var builder = new StringBuilder();
            builder.AppendLine("using System.Text;");
            builder.AppendLine();
            builder.AppendLine("using AbnfParser.CstNodes.Core;");
            builder.AppendLine("using Root;");
            builder.AppendLine();
            builder.AppendLine($"public sealed class {elementName}Transcriber : {elementName}.Visitor<Void, StringBuilder>");
            builder.AppendLine("{");
            builder.AppendLine($"private {elementName}Transcriber()");
            builder.AppendLine("{");
            builder.AppendLine("}");
            builder.AppendLine();
            builder.AppendLine($"public static {elementName}Transcriber Instance {{ get; }} = new {elementName}Transcriber();");
            builder.AppendLine();
            foreach (var range in ranges)
            {
                GenerateAccepts(builder, elementName, range.Item1, range.Item2);
            }

            builder.AppendLine("}");

            File.WriteAllText(@"C:\Users\gdebruin\code.txt", builder.ToString());
        }

        private void GenerateAccepts(StringBuilder builder, string elementName, int start, int end)
        {
            for (int i = start; i <= end; ++i)
            {
                var className = $"x{i:X2}";
                builder.AppendLine($"protected internal override Void Accept({elementName}.{className} node, StringBuilder context)");
                builder.AppendLine("{");
                builder.AppendLine($"return {className}Transcriber.Instance.Transcribe(node.Value, context);");
                builder.AppendLine("}");
                builder.AppendLine();
            }
        }

        [TestMethod]
        public void Generate()
        {
            var ranges = new[]
            {
                (0x41, 0x5A),
                (0x61, 0x7A),
            };
            var elementName = "ProseVal";

            var builder = new StringBuilder();
            builder.AppendLine("public abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);");
            builder.AppendLine();
            builder.AppendLine("public abstract class Visitor<TResult, TContext>");
            builder.AppendLine("{");
            builder.AppendLine($"public TResult Visit({elementName} node, TContext context)");
            builder.AppendLine("{");
            builder.AppendLine("return node.Dispatch(this, context);");
            builder.AppendLine("}");
            builder.AppendLine();
            foreach (var range in ranges)
            {
                GenerateAccepts(builder, range.Item1, range.Item2);
            }

            builder.AppendLine("}");
            builder.AppendLine();
            foreach (var range in ranges)
            {
                GenerateClasses(builder, elementName, range.Item1, range.Item2);
            }


            File.WriteAllText(@"C:\Users\gdebruin\code.txt", builder.ToString());
        }

        private void GenerateAccepts(StringBuilder builder, int start, int end)
        {
            for (int i = start; i <= end; ++i)
            {
                var className = $"x{i:X2}";
                builder.AppendLine($"protected internal abstract TResult Accept({className} node, TContext context);");
            }
        }

        private void GenerateClasses(StringBuilder builder, string elementName, int start, int end)
        {
            for (int i = start; i <= end; ++i)
            {
                var className = $"x{i:X2}";
                builder.AppendLine($"public sealed class {className} : {elementName}");
                builder.AppendLine("{");
                builder.AppendLine($"public {className}(Core.x3C lessThan, Core.{className} value, Core.x3E greaterThan)");
                builder.AppendLine("{");
                builder.AppendLine("LessThan = lessThan;");
                builder.AppendLine("Value = value;");
                builder.AppendLine("GreaterThan = greaterThan;");
                builder.AppendLine("}");
                builder.AppendLine();
                builder.AppendLine($"public Core.x3C LessThan {{ get; }}");
                builder.AppendLine($"public Core.{className} Value {{ get; }}");
                builder.AppendLine($"public Core.x3E GreaterThan {{ get; }}");
                builder.AppendLine();
                builder.AppendLine($"public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)");
                builder.AppendLine("{");
                builder.AppendLine("return visitor.Accept(this, context);");
                builder.AppendLine("}");
                builder.AppendLine("}");
                builder.AppendLine();
            }
        }

        [TestMethod]
        public void CoreRules()
        {
            var coreRulesPath = @"C:\github\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(coreRulesText);

            //// TODO if the ABNF is missing a trailing newline, the last rule will be dropped

            var stringBuilder = new StringBuilder();
            AbnfParser.Transcribers.RuleListTranscriber.Instance.Transcribe(cst, stringBuilder);
            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(coreRulesText, transcribedText);
        }

        [TestMethod]
        public void AbnfRules()
        {
            var coreRulesPath = @"C:\github\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\github\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var stringBuilder = new StringBuilder();
            AbnfParser.Transcribers.RuleListTranscriber.Instance.Transcribe(cst, stringBuilder);
            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(fullRulesText, transcribedText);
        }

        [TestMethod]
        public void OdataRules()
        {
            var odataRulesPath = @"C:\github\odata.net\odata\odata.abnf";
            var odataRulesText = File.ReadAllText(odataRulesPath);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(odataRulesText);

            var stringBuilder = new StringBuilder();
            AbnfParser.Transcribers.RuleListTranscriber.Instance.Transcribe(cst, stringBuilder);
            var transcribedText = stringBuilder.ToString();

            File.WriteAllText(odataRulesPath, transcribedText);
            Assert.AreEqual(odataRulesText, transcribedText);
        }

        [TestMethod]
        public void Helper()
        {
            var ranges = new[]
            {
                (0x41, 0x5A),
                (0x61, 0x7A),
            };

            var builder = new StringBuilder();
            foreach (var range in ranges)
            {
                for (int i = range.Item1; i <= range.Item2; ++i)
                {
                    var className = $"x{i:X2}";
                    builder.AppendLine($"protected internal sealed override Void Accept(Alpha.{className} node, StringBuilder context)");
                    builder.AppendLine("{");
                    builder.AppendLine($"context.Append((char)0{className});");
                    builder.AppendLine("return default;");
                    builder.AppendLine("}");
                    builder.AppendLine();
                }
            }

            File.WriteAllText(@"C:\Users\gdebruin\code.txt", builder.ToString());
        }

        [TestMethod]
        public void CodeGenerator()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var classes = RuleListGenerator.Instance.Generate(cst, default);

            var builder = new StringBuilder();
            ////new ClassTranscriber().Transcribe(classes.Value.ElementAt(26), builder, "  ");
            var csharp = builder.ToString();
        }

        [TestMethod]
        public void TestCodeGenerator()
        {
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(TestAbnf);

            var classes = AbnfParserGenerator.Generator.Instance.Generate(cst, default);

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

            //// TODO does the natural language classnames even make sense? would it make more sense to just make the class names the ABNF but replacing the symbol syntax with class friendly symbols?
            //// TODO i don't really like using _ for spaces *and* for the property name conflict resolution
            //// TODO make "optionals" not be nullable
            //// TODO you are entirely skipping out on incremental definitions, by the way
            //// TODO make sure to flesh out the code quality checks for the generated code
            //// TODO it could happen that someojne has first-rule = first-rule / second-rule in which case the du name first-rule with conflict with one of its elements
        }

        [TestMethod]
        public void TestGeneratorV3()
        {
            var abnf = File.ReadAllText(@"C:\msgithub\odata.net\odata\GeneratorV3\test.abnf");
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(abnf);

            var classes = GeneratorV3.Generator.Intance.Generate(cst, default);

            var classTranscriber = new ClassTranscriber();

            var stringBuilder = new StringBuilder();
            var builder = new Builder(stringBuilder, "    ");
            builder.AppendLine("namespace GeneratorV3"); //// TODO
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
        public void GenerateForCore()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var classes = GeneratorV3.Generator.Intance.Generate(cst, default);

            var classTranscriber = new ClassTranscriber();

            var stringBuilder = new StringBuilder();
            var builder = new Builder(stringBuilder, "    ");
            builder.AppendLine("namespace GeneratorV3");
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
        }

        [TestMethod]
        public void GenerateForAbnf()
        {
            var coreRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\msgithub\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var classes = GeneratorV3.Generator.Intance.Generate(cst, default);

            var classTranscriber = new ClassTranscriber();

            var stringBuilder = new StringBuilder();
            var builder = new Builder(stringBuilder, "    ");
            builder.AppendLine("namespace GeneratorV3");
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
