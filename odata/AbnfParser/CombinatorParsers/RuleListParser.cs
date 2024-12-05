namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CstNodes;
    using Sprache;

    public static class RuleListParser
    {
        public static Parser<RuleList> Instance { get; } =
            from inners in InnerParser.Instance.Many()
            select new RuleList(inners);

        public static class InnerParser
        {
            public static Parser<RuleList.Inner.RuleInner> RuleInner { get; } =
                from rule in RuleParser.Instance
                select new RuleList.Inner.RuleInner(rule);

            public static Parser<RuleList.Inner.CommentInner> CommentInner { get; } =
                from cwsps in CwspParser.Instance.Many()
                from cnl in CnlParser.Instance
                select new RuleList.Inner.CommentInner(cwsps, cnl);

            public static Parser<RuleList.Inner> Instance { get; } =
                RuleInner
                .Or<RuleList.Inner>(CommentInner);
        }
    }
}
