namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class NumValParser
    {
        public static Parser<NumVal.BinVal> BinVal { get; } =
            from value in BinValParser.Instance
            select new NumVal.BinVal(value);

        public static Parser<NumVal.DecVal> DecVal { get; } =
            from value in DecValParser.Instance
            select new NumVal.DecVal(value);

        public static Parser<NumVal.HexVal> HexVal { get; } =
            from value in HexValParser.Instance
            select new NumVal.HexVal(value);

        public static Parser<NumVal> Instance { get; } =
            BinVal
            .Or<NumVal>(DecVal)
            .Or(HexVal);
    }
}
