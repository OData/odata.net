namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class SpParser
    {
        public static Parser<Sp> Instance { get; } =
            from value in x20Parser.Instance
            select new Sp(value);
    }
}
