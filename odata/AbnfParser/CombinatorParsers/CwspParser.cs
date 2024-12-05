namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class CwspParser
    {
        public static Parser<Cwsp.WspOnly> WspOnly { get; }
    }
}
