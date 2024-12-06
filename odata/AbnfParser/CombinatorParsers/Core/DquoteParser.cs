namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class DquoteParser
    {
        public static Parser<Dquote> Instance { get; } =
            from value in x22Parser.Instance
            select new Dquote(value);
    }
}
