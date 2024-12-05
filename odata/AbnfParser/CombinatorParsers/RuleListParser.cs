namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class RuleListParser
    {
        public static class Inner
        {
            public static Parser<RuleList.Inner.RuleInner> RuleInner { get; }
        }
    }
}
