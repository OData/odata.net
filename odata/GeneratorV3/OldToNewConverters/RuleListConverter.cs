﻿namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class RuleListConverter
    {
        private RuleListConverter()
        {
        }

        public static RuleListConverter Instance { get; } = new RuleListConverter();

        public GeneratorV3.Abnf._rulelist Convert(AbnfParser.CstNodes.RuleList ruleList)
        {
            return new _rulelist(ruleList
                .Inners
                .Select(inner => 
                    InnerConverter.Instance.Visit(inner, default)));
        }

        public sealed class InnerConverter : AbnfParser.CstNodes.RuleList.Inner.Visitor<Abnf.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ, Root.Void>
        {
            private InnerConverter()
            {
            }

            public static InnerConverter Instance { get; } = new InnerConverter();

            protected internal override Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ Accept(RuleList.Inner.RuleInner node, Void context)
            {
                return new Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ(
                    new Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule(
                        RuleConverter.Instance.Convert(node.Rule)));
            }

            protected internal override Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ Accept(RuleList.Inner.CommentInner node, Void context)
            {
                return new Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ(
                    new Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ(
                        new Inners._ⲤЖcⲻwsp_cⲻnlↃ(
                            new Inners._Жcⲻwsp_cⲻnl(
                                node.Cwsps.Select(cwsp => CwspConverter.Instance.Visit(cwsp, default)),
                                CnlConverter.Instance.Visit(node.Cnl, default)))));
            }
        }
    }
}