namespace AbnfParser.CombinatorParsers.Core
{
    using AbnfParser.CstNodes.Core;
    using Sprache;

    public static class WspParser
    {
        public static Parser<Wsp.Space> Space { get; } =
            from sp in SpParser.Instance
            select new Wsp.Space(sp);

        public static Parser<Wsp.Tab> Tab { get; } =
            from htab in HtabParser.Instance
            select new Wsp.Tab(htab);

        public static Parser<Wsp> Instance { get; } =
            Space
            .Or<Wsp>(Tab);
    }
}
