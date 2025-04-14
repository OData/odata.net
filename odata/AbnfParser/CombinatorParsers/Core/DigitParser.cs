namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class DigitParser
    {
        public static Parser<Digit.x30> x30 { get; } =
            from value in x30Parser.Instance
            select new Digit.x30(value);

        public static Parser<Digit.x31> x31 { get; } =
            from value in x31Parser.Instance
            select new Digit.x31(value);

        public static Parser<Digit.x32> x32 { get; } =
            from value in x32Parser.Instance
            select new Digit.x32(value);

        public static Parser<Digit.x33> x33 { get; } =
            from value in x33Parser.Instance
            select new Digit.x33(value);

        public static Parser<Digit.x34> x34 { get; } =
            from value in x34Parser.Instance
            select new Digit.x34(value);

        public static Parser<Digit.x35> x35 { get; } =
            from value in x35Parser.Instance
            select new Digit.x35(value);

        public static Parser<Digit.x36> x36 { get; } =
            from value in x36Parser.Instance
            select new Digit.x36(value);

        public static Parser<Digit.x37> x37 { get; } =
            from value in x37Parser.Instance
            select new Digit.x37(value);

        public static Parser<Digit.x38> x38 { get; } =
            from value in x38Parser.Instance
            select new Digit.x38(value);

        public static Parser<Digit.x39> x39 { get; } =
            from value in x39Parser.Instance
            select new Digit.x39(value);

        public static Parser<Digit> Instance { get; } =
            x30
            .Or<Digit>(x31)
            .Or(x32)
            .Or(x33)
            .Or(x34)
            .Or(x35)
            .Or(x36)
            .Or(x37)
            .Or(x38)
            .Or(x39);
    }
}
