namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class DecValParser
    {
        public static Parser<DecVal.DecsOnly> DecsOnly { get; } =
            from d in x64Parser.Instance
            from digits in DigitParser.Instance.AtLeastOnce()
            select new DecVal.DecsOnly(d, digits);

        public static Parser<DecVal.ConcatenatedDecs> ConcatenatedDecs { get; } = ConcatenatedDecsParser.Instance;

        public static class ConcatenatedDecsParser
        {
            public static Parser<DecVal.ConcatenatedDecs> Instance { get; } =
                from d in x64Parser.Instance
                from digits in DigitParser.Instance.AtLeastOnce()
                from inners in InnerParser.Instance.AtLeastOnce()
                select new DecVal.ConcatenatedDecs(d, digits, inners);

            public static class InnerParser
            {
                public static Parser<DecVal.ConcatenatedDecs.Inner> Instance { get; } =
                    from dot in x2EParser.Instance
                    from digits in DigitParser.Instance.AtLeastOnce()
                    select new DecVal.ConcatenatedDecs.Inner(dot, digits);
            }
        }

        public static Parser<DecVal.Range> Range { get; } = RangeParser.Instance;

        public static class RangeParser
        {
            public static Parser<DecVal.Range> Instance { get; } =
                from d in x64Parser.Instance
                from digits in DigitParser.Instance.AtLeastOnce()
                from inners in InnerParser.Instance.AtLeastOnce()
                select new DecVal.Range(d, digits, inners);

            public static class InnerParser
            {
                public static Parser<DecVal.Range.Inner> Instance { get; } =
                    from dash in x2DParser.Instance
                    from digits in DigitParser.Instance.AtLeastOnce()
                    select new DecVal.Range.Inner(dash, digits);
            }
        }

        public static Parser<DecVal> Instance { get; } =
            DecsOnly
            .Or<DecVal>(ConcatenatedDecs)
            .Or(Range);
    }
}
