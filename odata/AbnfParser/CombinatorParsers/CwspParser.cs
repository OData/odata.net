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

        public static Parser<Cwsp.CnlAndWsp> CnlAndWsp { get; } =
            from cnl in CnlParser.Instance
            from wsp in WspParser.Instance
            select new Cwsp.CnlAndWsp(cnl, wsp);

        public static Parser<Cwsp> Instance { get; } =
            WspOnly
            .Or<Cwsp>(CnlAndWsp);
    }
}
