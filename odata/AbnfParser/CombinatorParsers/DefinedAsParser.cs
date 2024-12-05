namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class DefinedAsParser
    {
        public static Parser<DefinedAs.Declaration> Declaration { get; }
    }
}
