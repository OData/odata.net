namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class NumValParser
    {
        public static Parser<NumVal.BinVal> BinVal { get; }
    }
}
