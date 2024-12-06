namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class ElementsParser
    {
        public static Parser<Elements> Instance { get; } =
            from alternation in AlternationParser.Instance
            from cwsps in CwspParser.Instance.Many()
            select new Elements(alternation, cwsps);
    }
}
