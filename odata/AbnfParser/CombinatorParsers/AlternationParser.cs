namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class AlternationParser
    {
        public static Parser<Alternation> Instance { get; } =
            from concatenation in ConcatenationParser.Instance
            from inners in InnerParser.Instance.Many()
            select new Alternation(concatenation, inners);

        public static class InnerParser
        {
            public static Parser<Alternation.Inner> Instance { get; } =
                from prefixCwsps in CwspParser.Instance.Many()
                from slash in x2FParser.Instance
                from suffixCwsps in CwspParser.Instance.Many()
                from concatenation in ConcatenationParser.Instance
                select new Alternation.Inner(prefixCwsps, slash, suffixCwsps, concatenation);
        }
    }
}
