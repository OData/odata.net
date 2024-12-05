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
        public void Generate()
        {
            var start = 0x21;
            var end = 0x7E;
            var elementName = "Vchar";

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
            GenerateAccepts(builder, start, end);
            builder.AppendLine("}");
            builder.AppendLine();
            GenerateClasses(builder, elementName, start, end);


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
                builder.AppendLine($"public {className}(Core.{className} value)");
                builder.AppendLine("{");
                builder.AppendLine("Value = value;");
                builder.AppendLine("}");
                builder.AppendLine();
                builder.AppendLine($"public Core.{className} Value {{ get; }}");
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
            var coreRulesText = File.ReadAllText(@"C:\github\odata.net\odata\AbnfParser\core.abnf");
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(coreRulesText);
        }

        [TestMethod]
        public void AbnfRules()
        {
            var coreRulesText = File.ReadAllText(@"C:\github\odata.net\odata\AbnfParser\core.abnf");
            var abnfRulesText = File.ReadAllText(@"C:\github\odata.net\odata\AbnfParser\abnf.abnf");
            var fullRulesText = string.Join(Environment.NewLine, coreRulesText, abnfRulesText);
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(fullRulesText);
        }

        [TestMethod]
        public void OdataRules()
        {
            var odataRulesText = File.ReadAllText(@"C:\github\odata.net\odata\odata.abnf");
            var cst = AbnfParser.CombinatorParsers.RuleListParser.Instance.Parse(odataRulesText);
        }
    }
}
