namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class HexDigParser
    {
        public static Parser<HexDig.Digit> Digit { get; } =
            from value in DigitParser.Instance
            select new HexDig.Digit(value);

        public static Parser<HexDig.A> A { get; } =
            from value in x41Parser.Instance
            select new HexDig.A(value);

        public static Parser<HexDig.B> B { get; } =
            from value in x42Parser.Instance
            select new HexDig.B(value);

        public static Parser<HexDig.C> C { get; } =
            from value in x43Parser.Instance
            select new HexDig.C(value);

        public static Parser<HexDig.D> D { get; } =
            from value in x44Parser.Instance
            select new HexDig.D(value);

        public static Parser<HexDig.E> E { get; } =
            from value in x45Parser.Instance
            select new HexDig.E(value);

        public static Parser<HexDig.F> F { get; } =
            from value in x46Parser.Instance
            select new HexDig.F(value);

        public static Parser<HexDig> Instance { get; } =
            Digit
            .Or<HexDig>(A)
            .Or(B)
            .Or(C)
            .Or(D)
            .Or(E)
            .Or(F);
    }
}
