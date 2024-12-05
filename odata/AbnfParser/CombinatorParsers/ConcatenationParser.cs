namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class ConcatenationParser
    {
        public static Parser<Concatenation> Instance { get; } =
            from repetition in RepetitionParser.Instance
            from inners in InnerParser.Instance.Many()
            select new Concatenation(repetition, inners);

        public static class InnerParser
        {
            public static Parser<Concatenation.Inner> Instance { get; } =
                from cwsps in CwspParser.Instance.Many()
                from repetition in RepetitionParser.Instance
                select new Concatenation.Inner(cwsps, repetition);
        }
    }
}
