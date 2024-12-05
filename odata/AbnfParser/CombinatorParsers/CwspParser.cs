namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class CwspParser
    {
        public static Parser<Cwsp.WspOnly> WspOnly { get; } =
            from wsp in WspParser.Instance
            select new Cwsp.WspOnly(wsp);

        public static Parser<Cwsp.CnlAndWsp> CnlAndWsp { get; }
    }
}
