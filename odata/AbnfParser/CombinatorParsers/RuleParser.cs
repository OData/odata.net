namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class RuleParser
    {
        public static Parser<Rule> Instance { get; } =
            from ruleName in RuleNameParser.Instance
            from definedAs in DefinedAsParser.Instance
            from elements in ElementsParser.Instance
            from cnl in CnlParser.Instance
            select new Rule(ruleName, definedAs, elements, cnl);
    }
}
