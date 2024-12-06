namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class RepeatParser
    {
        public static Parser<Repeat.Count> Count { get; } =
            from digits in DigitParser.Instance.AtLeastOnce()
            select new Repeat.Count(digits);

        public static Parser<Repeat.Range> Range { get; } =
            from prefixDigits in DigitParser.Instance.Many()
            from asterisk in x2AParser.Instance
            from suffixDigits in DigitParser.Instance.Many()
            select new Repeat.Range(prefixDigits, asterisk, suffixDigits);

        public static Parser<Repeat> Instance { get; } =
            Range
            .Or<Repeat>(Count); //// TODO reversing range and count breaks `rulelist = 1*( rule / (*c-wsp c-nl) )`
    }
}
