namespace odata.tests
{
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
            var start = 0x3F;
            var end = 0xFE;

            var builder = new StringBuilder();
            for (int i = start; i <= end; ++i)
            {
                var className = $"x{i:X2}";
                /*builder.AppendLine($"public static Parser<ProseVal.{className}> {className} {{ get; }} =");
                builder.AppendLine($"\tfrom lessThan in x3CParser.Instance");
                builder.AppendLine($"\tfrom value in {className}Parser.Instance");
                builder.AppendLine($"\tfrom greaterThan in x3EParser.Instance");
                builder.AppendLine($"\tselect new ProseVal.{className}(lessThan, value, greaterThan);");
                builder.AppendLine();*/

                builder.AppendLine($".Or({className})");
            }

            File.WriteAllText(@"C:\Users\gdebruin\code.txt", builder.ToString());
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
