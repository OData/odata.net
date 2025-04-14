namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class LfParser
    {
        public static Parser<Lf> Instance { get; } =
            from value in x0AParser.Instance
            select new Lf(value);
    }
}
