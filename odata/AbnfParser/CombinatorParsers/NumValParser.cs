namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class NumValParser
    {
        public static Parser<NumVal.BinVal> BinVal { get; } =
            from percent in x25Parser.Instance
            from value in BinValParser.Instance
            select new NumVal.BinVal(percent, value);

        public static Parser<NumVal.DecVal> DecVal { get; } =
            from percent in x25Parser.Instance
            from value in DecValParser.Instance
            select new NumVal.DecVal(percent, value);

        public static Parser<NumVal.HexVal> HexVal { get; } =
            from percent in x25Parser.Instance
            from value in HexValParser.Instance
            select new NumVal.HexVal(percent, value);

        public static Parser<NumVal> Instance { get; } =
            BinVal
            .Or<NumVal>(DecVal)
            .Or(HexVal);
    }
}
