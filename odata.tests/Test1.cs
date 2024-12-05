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
            var start = 0x01;
            var end = 0x7F;

            var builder = new StringBuilder();
            for (int i = start; i <= end; ++i)
            {
                var className = $"x{i:X2}";
                builder.AppendLine($"public sealed class {className}");
                builder.AppendLine("{");
                builder.AppendLine($"private {className}()");
                builder.AppendLine("{");
                builder.AppendLine("}");
                builder.AppendLine();
                builder.AppendLine($"public static {className} Instance {{ get; }} = new {className}();");
                builder.AppendLine("}");
                builder.AppendLine();
            }

            File.WriteAllText(@"C:\Users\gdebruin\code.txt", builder.ToString());
        }
    }
}
