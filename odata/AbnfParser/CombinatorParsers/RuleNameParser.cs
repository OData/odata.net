namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class RuleNameParser
    {
        public static Parser<RuleName> Instance { get; } =
            from alpha in AlphaParser.Instance
            from inners in Parse.Many(InnerParser.Instance)
            select new RuleName(alpha, inners);

        public static class InnerParser
        {
            public static Parser<RuleName.Inner.AlphaInner> AlphaInner { get; } =
                from alpha in AlphaParser.Instance
                select new RuleName.Inner.AlphaInner(alpha);

            public static Parser<RuleName.Inner.DigitInner> DigitInner { get; } =
                from digit in DigitParser.Instance
                select new RuleName.Inner.DigitInner(digit);

            public static Parser<RuleName.Inner.DashInner> DashInner { get; } =
                from dash in x2DParser.Instance
                select new RuleName.Inner.DashInner(dash);

            public static Parser<RuleName.Inner> Instance { get; } =
                AlphaInner
                .Or<RuleName.Inner>(DigitInner)
                .Or(DashInner);
        }
    }
}
