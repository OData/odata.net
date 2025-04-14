namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class RepetitionParser
    {
        public static Parser<Repetition.ElementOnly> ElementOnly { get; } =
            from element in ElementParser.Instance
            select new Repetition.ElementOnly(element);

        public static Parser<Repetition.RepeatAndElement> RepeatAndElement { get; } =
            from repeat in RepeatParser.Instance
            from element in ElementParser.Instance
            select new Repetition.RepeatAndElement(repeat, element);

        public static Parser<Repetition> Instance { get; } =
            ElementOnly
            .Or<Repetition>(RepeatAndElement);
    }
}
