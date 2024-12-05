namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class DefinedAsParser
    {
        public static Parser<DefinedAs.Declaration> Declaration { get; } =
            from prefixCwsps in Parse.Many(CwspParser.Instance)
            from @equals in x3DParser.Instance
            from suffixCwsps in Parse.Many(CwspParser.Instance)
            select new DefinedAs.Declaration(prefixCwsps, @equals, suffixCwsps);

        public static Parser<DefinedAs.Incremental> Incremental { get; } =
            from prefixCwsps in Parse.Many(CwspParser.Instance)
            from @equals in x3DParser.Instance
            from slash in x2FParser.Instance
            from suffixCwsps in Parse.Many(CwspParser.Instance)
            select new DefinedAs.Incremental(prefixCwsps, @equals, slash, suffixCwsps);

        public static Parser<DefinedAs> Instance { get; } =
            Declaration
            .Or<DefinedAs>(Incremental);
    }
}
