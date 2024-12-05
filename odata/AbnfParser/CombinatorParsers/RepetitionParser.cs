namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class RepetitionParser
    {
        public static Parser<Repetition.ElementOnly> ElementOnly { get; } =
            from element in ElementParser.Instance
            select new Repetition.ElementOnly(element);
    }
}
