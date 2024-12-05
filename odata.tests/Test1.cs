namespace odata.tests
{
    using AbnfParser.CstNodes.Core;
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
            var fullRulesText = string.Join(Environment.NewLine, abnfRulesText, coreRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);

            var stringBuilder = new StringBuilder();
            AbnfParser.Transcribers.RuleListTranscriber.Instance.Transcribe(cst, stringBuilder);
            var transcribedText = stringBuilder.ToString();
            Assert.AreEqual(fullRulesText, transcribedText);
        }

        [TestMethod]
        public void OdataRules()
        {
            var odataRulesText = File.ReadAllText(@"C:\github\odata.net\odata\odata.abnf");
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(odataRulesText);
        }
    }
}
