﻿namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class RuleConverter
    {
        private RuleConverter()
        {
        }

        public static RuleConverter Instance { get; } = new RuleConverter();

        public _rule Convert(AbnfParser.CstNodes.Rule rule)
        {
            return new _rule(
                RuleNameConverter.Instance.Convert(rule.RuleName),
                DefinedAsConverter.Instance.Visit(rule.DefinedAs, default),
                ElementsConverter.Instance.Convert(rule.Elements),
                CnlConverter.Instance.Visit(rule.Cnl, default));
        }
    }
}