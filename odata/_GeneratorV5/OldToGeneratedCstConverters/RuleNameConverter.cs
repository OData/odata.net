namespace _GeneratorV4.OldToGeneratedCstConverters
{
    using System.Linq;

    public sealed class RuleNameConverter
    {
        private RuleNameConverter()
        {
        }

        public static RuleNameConverter Instance { get; } = new RuleNameConverter();

        public __Generated.CstNodes.Rules._rulename Convert(AbnfParser.CstNodes.RuleName ruleName)
        {
            return new __Generated.CstNodes.Rules._rulename(
                AlphaConverter.Instance.Visit(ruleName.Alpha, default),
                ruleName.Inners.Select(inner =>
                    new __Generated.CstNodes.Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃ(
                        InnerConverter.Instance.Visit(inner, default))));
        }

        private sealed class InnerConverter : AbnfParser.CstNodes.RuleName.Inner.Visitor<__Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(AbnfParser.CstNodes.RuleName.Inner.AlphaInner node, Root.Void context)
            {
                return new __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA(
                    AlphaConverter.Instance.Visit(node.Alpha, context));
            }

            protected internal override __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(AbnfParser.CstNodes.RuleName.Inner.DigitInner node, Root.Void context)
            {
                return new __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT(
                    DigitConverter.Instance.Visit(node.Digit, context));
            }

            protected internal override __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ Accept(AbnfParser.CstNodes.RuleName.Inner.DashInner node, Root.Void context)
            {
                return new __Generated.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ(
                    new __Generated.CstNodes.Inners._ʺx2Dʺ(x2DConverter.Instance.Convert(node.Dash)));
            }
        }
    }
}
