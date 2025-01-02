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
                    new Abnf.Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃ(
                        InnerConverter.Instance.Visit(inner, default))));
        }

        private sealed class InnerConverter : AbnfParser.CstNodes.RuleName.Inner.Visitor<GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆʺx2Dʺ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(RuleName.Inner.AlphaInner node, Void context)
            {
                return new GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA(
                    AlphaConverter.Instance.Visit(node.Alpha, context));
            }

            protected internal override GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(RuleName.Inner.DigitInner node, Void context)
            {
                return new GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT(
                    DigitConverter.Instance.Visit(node.Digit, context));
            }

            protected internal override GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(RuleName.Inner.DashInner node, Void context)
            {
                return new GeneratorV3.Abnf.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ(
                    new Abnf.Inners._ʺx2Dʺ(x2DConverter.Instance.Convert(node.Dash)));
            }
        }
    }
}
