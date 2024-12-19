namespace odata.tests
{
    using AbnfParser.CstNodes.Core;
    using AbnfParserGenerator.CstNodesGenerator;
    using Root;
    using Root.OdataResourcePath.CombinatorParsers;
    using Root.OdataResourcePath.Transcribers;
    using Sprache;
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
                builder.AppendLine().AppendLine();
            }

            var csharp = builder.ToString();

            Assert.AreEqual(TestAbnfCstClasses, csharp);

            //// TODO does the natural language classnames even make sense? would it make more sense to just make the class names the ABNF but replacing the symbol syntax with class friendly symbols?
            //// TODO you are entirely skipping out on incremental definitions, by the way
        }

        private static string TestAbnfCstClasses =
"""
public sealed class rulewithnameFIRST_RULE
{
    public rulewithnameFIRST_RULE(rulewithnameSECOND_RULE rulewithnameSECOND_RULE1)
    {
        this.rulewithnameSECOND_RULE1 = rulewithnameSECOND_RULE1;
    }

    public rulewithnameSECOND_RULE rulewithnameSECOND_RULE1 { get; }
}

public abstract class rulewithnameSECOND_RULE
{
    private rulewithnameSECOND_RULE()
    {
    }

    protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

    public abstract class Visitor<TResult, TContext>
    {
        public TResult Visit(rulewithnameSECOND_RULE node, TContext context)
        {
            return node.Dispatch(this, context);
        }

        protected internal abstract TResult Accept(FIRST_RULE node, TContext context);
    }

    public sealed class groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ
    {
        public groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ(FIRST_RULE FIRST_RULE1, FIRST_RULE FIRST_RULE2)
        {
            this.FIRST_RULE1 = FIRST_RULE1;
            this.FIRST_RULE2 = FIRST_RULE2;
        }

        public FIRST_RULE FIRST_RULE1 { get; }
        public FIRST_RULE FIRST_RULE2 { get; }
    }

    public sealed class groupingofᴖFIRST_RULEfollowedbyanynumberofgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanynumberofanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖfollowedbyatmostONEFIRST_RULEfollowedbyatleastONEFIRST_RULEᴖ
    {
        public groupingofᴖFIRST_RULEfollowedbyanynumberofgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanynumberofanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖfollowedbyatmostONEFIRST_RULEfollowedbyatleastONEFIRST_RULEᴖ(
            FIRST_RULE FIRST_RULE1,
            IEnumerable<groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ> groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1,
            IEnumerable<groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ> groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1,
            IEnumerable<FIRST_RULE> atmostONEFIRST_RULE1,
            IEnumerable<FIRST_RULE> atleastONEFIRST_RULE1)
        {
            this.FIRST_RULE1 = FIRST_RULE1;
            this.groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 = groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1;
            this.groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1 = groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1;
            this.atmostONEFIRST_RULE = atmostONEFIRST_RULE1;
            this.atleastONEFIRST_RULE = atleastONEFIRST_RULE1;
        }

        public FIRST_RULE FIRST_RULE1 { get; }
        public IEnumerable<groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ> groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 { get; }
        public IEnumerable<groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ> groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1 { get; }
        public IEnumerable<FIRST_RULE> atmostONEFIRST_RULE { get; }
        public IEnumerable<FIRST_RULE> atleastONEFIRST_RULE { get; }

        public sealed class groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ
        {
            public groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ(FIRST_RULEfollowedbyanoptionalFIRST_RULE FIRST_RULEfollowedbyanoptionalFIRST_RULE1)
            {
                this.FIRST_RULEfollowedbyanoptionalFIRST_RULE1 = FIRST_RULEfollowedbyanoptionalFIRST_RULE1;
            }

            public FIRST_RULEfollowedbyanoptionalFIRST_RULE FIRST_RULEfollowedbyanoptionalFIRST_RULE1 { get; }

            public abstract class FIRST_RULEfollowedbyanoptionalFIRST_RULE
            {
                private FIRST_RULEfollowedbyanoptionalFIRST_RULE()
                {
                }

                protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

                public abstract class Visitor<TResult, TContext>
                {
                    public TResult Visit(FIRST_RULEfollowedbyanoptionalFIRST_RULE node, TContext context)
                    {
                        return node.Dispatch(this, context);
                    }

                    protected internal abstract TResult Accept(FIRST_RULEfollowedbyoneFIRST_RULE node, TContext context);

                    protected internal abstract TResult Accept(FIRST_RULEfollowedbynoFIRST_RULE node, TContext context);
                }

                public sealed class FIRST_RULEfollowedbyoneFIRST_RULE : FIRST_RULEfollowedbyanoptionalFIRST_RULE
                {
                    public FIRST_RULEfollowedbyoneFIRST_RULE(FIRST_RULE FIRST_RULE1, FIRST_RULE FIRST_RULE2)
                    {
                        this.FIRST_RULE1 = FIRST_RULE1;
                        this.FIRST_RULE2 = FIRST_RULE2;
                    }

                    public FIRST_RULE FIRST_RULE1 { get; }
                    public FIRST_RULE FIRST_RULE2 { get; }

                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }

                public sealed class FIRST_RULEfollowedbynoFIRST_RULE : FIRST_RULEfollowedbyanoptionalFIRST_RULE
                {
                    public FIRST_RULEfollowedbynoFIRST_RULE(FIRST_RULE FIRST_RULE1)
                    {
                        this.FIRST_RULE1 = FIRST_RULE1;
                    }

                    public FIRST_RULE FIRST_RULE1 { get; }

                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }
            }
        }

        public sealed class groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ
        {
            public groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ(FIRST_RULE FIRST_RULE1, FIRST_RULE FIRST_RULE2)
            {
                this.FIRST_RULE1 = FIRST_RULE1;
                this.FIRST_RULE2 = FIRST_RULE2;
            }

            public FIRST_RULE FIRST_RULE1 { get; }
            public FIRST_RULE FIRST_RULE2 { get; }
        }
    }

    public sealed class groupingofᴖFIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖᴖ
    {
        public groupingofᴖFIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖᴖ(FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1)
        {
            this.FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 = FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1;
        }

        public FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 { get; }

        public abstract class FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ
        {
            private FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ()
            {
            }

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(FIRST_RULE node, TContext context);

                protected internal abstract TResult Accept(FIRST_RULEfollowedbyanoptionalFIRST_RULE node, TContext context);
            }

            public sealed class groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ
            {
                public groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ(FIRST_RULEfollowedbyanoptionalFIRST_RULE FIRST_RULEfollowedbyanoptionalFIRST_RULE1)
                {
                    this.FIRST_RULEfollowedbyanoptionalFIRST_RULE1 = FIRST_RULEfollowedbyanoptionalFIRST_RULE1;
                }

                public FIRST_RULEfollowedbyanoptionalFIRST_RULE FIRST_RULEfollowedbyanoptionalFIRST_RULE1 { get; }

                public abstract class FIRST_RULEfollowedbyanoptionalFIRST_RULE
                {
                    private FIRST_RULEfollowedbyanoptionalFIRST_RULE()
                    {
                    }

                    protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

                    public abstract class Visitor<TResult, TContext>
                    {
                        public TResult Visit(FIRST_RULEfollowedbyanoptionalFIRST_RULE node, TContext context)
                        {
                            return node.Dispatch(this, context);
                        }

                        protected internal abstract TResult Accept(FIRST_RULEfollowedbyoneFIRST_RULE node, TContext context);

                        protected internal abstract TResult Accept(FIRST_RULEfollowedbynoFIRST_RULE node, TContext context);
                    }

                    public sealed class FIRST_RULEfollowedbyoneFIRST_RULE : FIRST_RULEfollowedbyanoptionalFIRST_RULE
                    {
                        public FIRST_RULEfollowedbyoneFIRST_RULE(FIRST_RULE FIRST_RULE1, FIRST_RULE FIRST_RULE2)
                        {
                            this.FIRST_RULE1 = FIRST_RULE1;
                            this.FIRST_RULE2 = FIRST_RULE2;
                        }

                        public FIRST_RULE FIRST_RULE1 { get; }
                        public FIRST_RULE FIRST_RULE2 { get; }

                        protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                        {
                            return visitor.Accept(this, context);
                        }
                    }

                    public sealed class FIRST_RULEfollowedbynoFIRST_RULE : FIRST_RULEfollowedbyanoptionalFIRST_RULE
                    {
                        public FIRST_RULEfollowedbynoFIRST_RULE(FIRST_RULE FIRST_RULE1)
                        {
                            this.FIRST_RULE1 = FIRST_RULE1;
                        }

                        public FIRST_RULE FIRST_RULE1 { get; }

                        protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                        {
                            return visitor.Accept(this, context);
                        }
                    }
                }
            }

            public sealed class FIRST_RULE : FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ
            {
                public FIRST_RULE(FIRST_RULE FIRST_RULE1)
                {
                    this.FIRST_RULE1 = FIRST_RULE1;
                }

                public FIRST_RULE FIRST_RULE1 { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class FIRST_RULEfollowedbyanoptionalFIRST_RULE : FIRST_RULEorgroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ
            {
                public FIRST_RULEfollowedbyanoptionalFIRST_RULE(groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1)
                {
                    this.groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 = groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1;
                }

                public groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }

    //// TODO these grouping classes that immediately take in a DU could just directly be a DU
    public sealed class groupingofᴖFIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖᴖ
    {
        public groupingofᴖFIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖᴖ(
            FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1)
        {
            this.FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1 = FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1;
        }

        public FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1 { get; }

        public abstract class FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ
        {
            private FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ()
            {
            }

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyonegroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ node, TContext context);
                protected internal abstract TResult Accept(FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbynogroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ node, TContext context);
            }

            public sealed class groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ
            {
                public groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ(FIRST_RULEfollowedbyanoptionalFIRST_RULE FIRST_RULEfollowedbyanoptionalFIRST_RULE1)
                {
                    this.FIRST_RULEfollowedbyanoptionalFIRST_RULE1 = FIRST_RULEfollowedbyanoptionalFIRST_RULE1;
                }

                public FIRST_RULEfollowedbyanoptionalFIRST_RULE FIRST_RULEfollowedbyanoptionalFIRST_RULE1 { get; }

                public abstract class FIRST_RULEfollowedbyanoptionalFIRST_RULE
                {
                    private FIRST_RULEfollowedbyanoptionalFIRST_RULE()
                    {
                    }

                    protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

                    public abstract class Visitor<TResult, TContext>
                    {
                        public TResult Visit(FIRST_RULEfollowedbyanoptionalFIRST_RULE node, TContext context)
                        {
                            return node.Dispatch(this, context);
                        }

                        protected internal abstract TResult Accept(FIRST_RULEfollowedbyoneFIRST_RULE node, TContext context);
                        protected internal abstract TResult Accept(FIRST_RULEfollwedbynoFIRST_RULE node, TContext context);
                    }

                    public sealed class FIRST_RULEfollowedbyoneFIRST_RULE : FIRST_RULEfollowedbyanoptionalFIRST_RULE
                    {
                        public FIRST_RULEfollowedbyoneFIRST_RULE(FIRST_RULE FIRST_RULE1, FIRST_RULE FIRST_RULE2)
                        {
                            this.FIRST_RULE1 = FIRST_RULE1;
                            this.FIRST_RULE2 = FIRST_RULE2;
                        }

                        public FIRST_RULE FIRST_RULE1 { get; }
                        public FIRST_RULE FIRST_RULE2 { get; }

                        protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                        {
                            return visitor.Accept(this, context);
                        }
                    }

                    public sealed class FIRST_RULEfollwedbynoFIRST_RULE : FIRST_RULEfollowedbyanoptionalFIRST_RULE
                    {
                        public FIRST_RULEfollwedbynoFIRST_RULE(FIRST_RULE FIRST_RULE1, FIRST_RULE FIRST_RULE2)
                        {
                            this.FIRST_RULE1 = FIRST_RULE1;
                            this.FIRST_RULE2 = FIRST_RULE2;
                        }

                        public FIRST_RULE FIRST_RULE1 { get; }
                        public FIRST_RULE FIRST_RULE2 { get; }

                        protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                        {
                            return visitor.Accept(this, context);
                        }
                    }
                }
            }

            public sealed class groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ
            {
                public groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ(FIRST_RULE FIRST_RULE1, FIRST_RULE FIRST_RULE2)
                {
                    this.FIRST_RULE1 = FIRST_RULE1;
                    this.FIRST_RULE2 = FIRST_RULE2;
                }

                public FIRST_RULE FIRST_RULE1 { get; }
                public FIRST_RULE FIRST_RULE2 { get; }
            }

            public sealed class FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyonegroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ : FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ
            {
                public FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyonegroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ(
                    FIRST_RULE FIRST_RULE1,
                    groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1,
                    groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1)
                {
                    this.FIRST_RULE1 = FIRST_RULE1;
                    this.groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 = groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1;
                    this.groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1 = groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1;
                }

                public FIRST_RULE FIRST_RULE1 { get; }
                public groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 { get; }
                public groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ groupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ1 { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbynogroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ : FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbyanoptionalgroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ
            {
                public FIRST_RULEfollowedbygroupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖfollowedbynogroupingofᴖFIRST_RULEfollowedbyFIRST_RULEᴖ(
                    FIRST_RULE FIRST_RULE1,
                    groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1)
                {
                    this.FIRST_RULE1 = FIRST_RULE1;
                    this.groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 = groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1;
                }

                public FIRST_RULE FIRST_RULE1 { get; }
                public groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ groupingofᴖFIRST_RULEfollowedbyanoptionalFIRST_RULEᴖ1 { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }

    public sealed class FIRST_RULE : rulewithnameSECOND_RULE
    {
        public FIRST_RULE(rulewithnameFIRST_RULE _rulewithnameFIRST_RULE1)
        {
            this.rulewithnameFIRST_RULE1 = _rulewithnameFIRST_RULE1;
        }

        public rulewithnameFIRST_RULE rulewithnameFIRST_RULE1 { get; }

        protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
        {
            return visitor.Accept(this, context);
        }
    }

    public sealed class FIRST_RULEfollowedbyFIRST_RULE
    {
    }

    //// TODO you didn't actually finish this...
}
""";

        private static string TestAbnf =
"""
first-rule = second-rule
second-rule = first-rule 
            / first-rule first-rule 
            / first-rule *(first-rule [first-rule]) *[first-rule first-rule] *1first-rule 1*first-rule 
            / (first-rule / (first-rule [first-rule]))
            / first-rule (first-rule [first-rule]) [first-rule first-rule]

""";

        private sealed class ClassTranscriber
        {
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

            public void Transcribe(AbnfParserGenerator.Class @class, StringBuilder builder, string indent)
            {
                Transcribe(@class, new Builder(builder, indent));
            }

            private void Transcribe(AbnfParserGenerator.Class @class, Builder builder)
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
                        }
                        else
                        {
                            builder.AppendLine();
                            builder.AppendLine("{");
                            builder.Indent();
                            builder.AppendLine(method.Body);
                            builder.Unindent();
                            builder.AppendLine("}");
                        }
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
                        builder.Append(property.AccessModifier.ToString().ToLower()).Append(" ");
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
