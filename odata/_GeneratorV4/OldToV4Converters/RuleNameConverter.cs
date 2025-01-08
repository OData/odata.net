namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using Root;

    public sealed class RuleNameConverter
    {
        private RuleNameConverter()
        {
        }

        public static RuleNameConverter Instance { get; } = new RuleNameConverter();

        public _rulename Convert(AbnfParser.CstNodes.RuleName ruleName)
        {
            return new _rulename(
                AlphaConverter.Instance.Visit(ruleName.Alpha, default),
                ruleName.Inners.Select(inner =>
                    new Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃ(
                        InnerConverter.Instance.Visit(inner, default))));
        }

        private sealed class InnerConverter : AbnfParser.CstNodes.RuleName.Inner.Visitor<Inners._ALPHAⳆDIGITⳆʺx2Dʺ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(RuleName.Inner.AlphaInner node, Void context)
            {
                return new Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA(
                    AlphaConverter.Instance.Visit(node.Alpha, context));
            }

            protected internal override Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(RuleName.Inner.DigitInner node, Void context)
            {
                return new Inners._ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT(
                    DigitConverter.Instance.Visit(node.Digit, context));
            }

            protected internal override Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(RuleName.Inner.DashInner node, Void context)
            {
                return new Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ(
                    new Inners._ʺx2Dʺ(x2DConverter.Instance.Convert(node.Dash)));
            }
        }
    }
}
