namespace GeneratorV3.OldToNewConverters
{
    public sealed class RuleNameConverter
    {
        private RuleNameConverter()
        {
        }

        public static RuleNameConverter Instance { get; } = new RuleNameConverter();

        public GeneratorV3.Abnf._rulename Convert(AbnfParser.CstNodes.RuleName ruleName)
        {
        }
    }
}
