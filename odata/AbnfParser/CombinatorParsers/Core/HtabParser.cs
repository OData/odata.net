namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class HtabParser
    {
        public static Parser<Htab> Instance { get; } =
            from value in x09Parser.Instance
            select new Htab(value);
    }
}
