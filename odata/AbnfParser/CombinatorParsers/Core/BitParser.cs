namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class BitParser
    {
        public static Parser<Bit.Zero> Zero { get; } =
            from value in x30Parser.Instance
            select new Bit.Zero(value);

        public static Parser<Bit.One> One { get; } =
            from value in x31Parser.Instance
            select new Bit.One(value);

        public static Parser<Bit> Instance { get; } =
            Zero
            .Or<Bit>(One);
    }
}
