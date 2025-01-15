namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using _GeneratorV5.ManualParsers.Rules;
    using System.Linq;

    public sealed class RuleListConverter
    {
        private RuleListConverter()
        {
        }

        public static RuleListConverter Instance { get; } = new RuleListConverter();

        public __Generated.CstNodes.Rules._rulelist Convert(AbnfParser.CstNodes.RuleList ruleList)
        {
            return new __Generated.CstNodes.Rules._rulelist(ruleList
                .Inners
                .Select(inner => 
                    InnerConverter.Instance.Visit(inner, default)).Convert2());
        }

        public sealed class InnerConverter : AbnfParser.CstNodes.RuleList.Inner.Visitor<__Generated.CstNodes.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override __Generated.CstNodes.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ Accept(AbnfParser.CstNodes.RuleList.Inner.RuleInner node, Root.Void context)
            {
                return new __Generated.CstNodes.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ(
                    new __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule(
                        RuleConverter.Instance.Convert(node.Rule)));
            }

            protected internal override __Generated.CstNodes.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ Accept(AbnfParser.CstNodes.RuleList.Inner.CommentInner node, Root.Void context)
            {
                return new __Generated.CstNodes.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ(
                    new __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ(
                        new __Generated.CstNodes.Inners._ⲤЖcⲻwsp_cⲻnlↃ(
                            new __Generated.CstNodes.Inners._Жcⲻwsp_cⲻnl(
                                node.Cwsps.Select(cwsp => CwspConverter.Instance.Visit(cwsp, default)).Convert(),
                                CnlConverter.Instance.Visit(node.Cnl, default)))));
            }
        }
    }
}
