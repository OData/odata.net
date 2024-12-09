namespace odata.tests
{
    using AbnfParser.CstNodes.Core;
    using AbnfParserGenerator.CstNodesGenerator;
    using Root;
    using Root.OdataResourcePath.CombinatorParsers;
    using Root.OdataResourcePath.Transcribers;
    using Sprache;
    using System.Text;

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
            var coreRulesPath = @"C:\github\odata.net\odata\AbnfParser\core.abnf";
            var coreRulesText = File.ReadAllText(coreRulesPath);
            var abnfRulesPath = @"C:\github\odata.net\odata\AbnfParser\abnf.abnf";
            var abnfRulesText = File.ReadAllText(abnfRulesPath);
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var classes = RuleListGenerator.Instance.Generate(cst, default);

            var builder = new StringBuilder();
            new ClassTranscriber().Transcribe(classes.Value.ElementAt(16), builder, "  ");
            var csharp = builder.ToString();
        }

        private sealed class ClassTranscriber
        {
            private sealed class Builder
            {
                private readonly StringBuilder builder;
                private readonly string indent;
                private string currentIndent;

                public Builder(StringBuilder builder, string indent)
                {
                    this.builder = builder;
                    this.indent = indent;

                    this.currentIndent = string.Empty;
                }

                public Builder Append(string value)
                {
                    this.builder.Append(value);
                    return this;
                }

                public Builder AppendLine()
                {
                    this.builder.AppendLine().Append(this.currentIndent);
                    return this;
                }

                public Builder AppendLine(string value)
                {
                    this.builder.AppendLine(value).Append(this.currentIndent);
                    return this;
                }

                public Builder AppendJoin(string separator, IEnumerable<string> values)
                {
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

            public void Transcribe(Class @class, StringBuilder builder, string indent)
            {
                Transcribe(@class, new Builder(builder, indent));
            }

            private void Transcribe(Class @class, Builder builder)
            {
                builder.Append("public ");
                if (@class.IsAbstract != null)
                {
                    if (@class.IsAbstract.Value)
                    {
                        builder.Append("abstract ");
                    }
                    else
                    {
                        builder.Append("sealed ");
                    }
                }
                
                builder.Append("class ").Append(@class.Name);
                var genericTypeParameters = string.Join(",", @class.GenericTypeParameters);
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
                        builder.Append(constructor.AccessModifier.ToString().ToLower()).Append(" ").Append(@class.Name).Append("(");
                        builder.AppendJoin(", ", constructor.Parameters, (parameter, b) => b.Append(parameter.Type).Append(" ").Append(parameter.Name));
                        builder.AppendLine(")");
                        builder.AppendLine("{");
                        if (!string.IsNullOrEmpty(constructor.Body))
                        {
                            builder.Indent();
                            builder.AppendLine(constructor.Body);
                            builder.Unindent();
                        }

                        builder.AppendLine("}");
                    }
                }

                if (@class.Methods.Any())
                {
                    if (needsNewLine)
                    {
                        builder.AppendLine();
                    }

                    needsNewLine = true;
                    foreach (var method in @class.Methods)
                    {
                        builder.Append(method.AccessModifier.ToString().ToLower()).Append(" ");
                        if (method.IsAbstract != null)
                        {
                            if (method.IsAbstract.Value)
                            {
                                builder.Append("abstract ");
                            }
                            else
                            {
                                builder.Append("sealed ");
                            }
                        }

                        if (method.IsOverride)
                        {
                            builder.Append("override ");
                        }
                    }
                }




                var constructorParameters = @class.ConstructorParameters;
                if (constructorParameters != null)
                {
                    builder.Append($"public {@class.Name}(");
                    builder.AppendJoin(
                        ",", 
                        constructorParameters
                            .Value
                            .Select(constructorParameter => $"{constructorParameter.Type} {constructorParameter.Name}"));
                    builder.AppendLine(")");
                    builder.AppendLine("{");
                    //// TODO set values
                    builder.AppendLine("}");
                }

                foreach (var property in @class.Properties)
                {
                    if (property.Type == null)
                    {
                        /// TODO throw new Exception("TODO");
                    }

                    //// TODO null propogation
                    builder.AppendLine($"public {property.Type?.Name} {property.Name.ToString()} {{ get; }}");
                }

                foreach (var nestedClass in @class.NestedClasses)
                {
                    new ClassTranscriber().Transcribe(nestedClass, builder);
                }

                builder.Unindent();
                builder.AppendLine("}");
            }
        }
    }
}
