namespace _GeneratorV5.OldToGeneratedCstConverters
{
    public sealed class RuleConverter
    {
        private RuleConverter()
        {
        }

        public static RuleConverter Instance { get; } = new RuleConverter();

        public __Generated.CstNodes.Rules._rule Convert(AbnfParser.CstNodes.Rule rule)
        {
            return new __Generated.CstNodes.Rules._rule(
                RuleNameConverter.Instance.Convert(rule.RuleName),
                DefinedAsConverter.Instance.Visit(rule.DefinedAs, default),
                ElementsConverter.Instance.Convert(rule.Elements),
                CnlConverter.Instance.Visit(rule.Cnl, default));
        }
    }
}
