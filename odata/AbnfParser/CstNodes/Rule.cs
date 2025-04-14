namespace AbnfParser.CstNodes
{
    public sealed class Rule
    {
        public Rule(RuleName ruleName, DefinedAs definedAs, Elements elements, Cnl cnl)
        {
            RuleName = ruleName;
            DefinedAs = definedAs;
            Elements = elements;
            Cnl = cnl;
        }

        public RuleName RuleName { get; }
        public DefinedAs DefinedAs { get; }
        public Elements Elements { get; }
        public Cnl Cnl { get; }
    }
}
