namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;
    using GeneratorV3.OldToNewConverters.Core;
    using Root;

    public sealed class RuleNameConverter
    {
        private RuleNameConverter()
        {
        }

        public static RuleNameConverter Instance { get; } = new RuleNameConverter();

        public GeneratorV3.Abnf._rulename Convert(AbnfParser.CstNodes.RuleName ruleName)
        {
            return new Abnf._rulename(
                AlphaConverter.Instance.Visit(ruleName.Alpha, default),
                ruleName.Inners.Select(inner =>
                    new Abnf.Inners._openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ(
                        InnerConverter.Instance.Visit(inner, default))));
        }

        private sealed class InnerConverter : AbnfParser.CstNodes.RuleName.Inner.Visitor<GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆdoublequotex2Ddoublequote, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆdoublequotex2Ddoublequote Accept(RuleName.Inner.AlphaInner node, Void context)
            {
                return new GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆdoublequotex2Ddoublequote._ALPHA(
                    AlphaConverter.Instance.Visit(node.Alpha, context));
            }

            protected internal override GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆdoublequotex2Ddoublequote Accept(RuleName.Inner.DigitInner node, Void context)
            {
                return new GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆdoublequotex2Ddoublequote._DIGIT(
                    DigitConverter.Instance.Visit(node.Digit, context));
            }

            protected internal override GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆdoublequotex2Ddoublequote Accept(RuleName.Inner.DashInner node, Void context)
            {
                return new GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆdoublequotex2Ddoublequote._doublequotex2Ddoublequote(
                    new Abnf.Inners._doublequotex2Ddoublequote(x2DConverter.Instance.Convert(node.Dash)));
            }
        }
    }
}
