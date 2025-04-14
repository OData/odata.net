namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class CrParser
    {
        public static Parser<Cr> Instance { get; } =
            from value in x0DParser.Instance
            select new Cr(value);
    }
}
