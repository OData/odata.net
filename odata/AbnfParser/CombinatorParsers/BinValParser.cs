namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class BinValParser
    {
        public static Parser<BinVal.BitsOnly> BitsOnly { get; } =
            from b in x62Parser.Instance
            from bits in BitParser.Instance.AtLeastOnce()
            select new BinVal.BitsOnly(b, bits);

        public static Parser<BinVal.ConcatenatedBits> ConcatenatedBits { get; } = ConcatenatedBitsParser.Instance;

        public static class ConcatenatedBitsParser
        {
            public static Parser<BinVal.ConcatenatedBits> Instance { get; } =
                from b in x62Parser.Instance
                from bits in BitParser.Instance.AtLeastOnce()
                from inners in InnerParser.Instance.AtLeastOnce()
                select new BinVal.ConcatenatedBits(b, bits, inners);

            public static class InnerParser
            {
                public static Parser<BinVal.ConcatenatedBits.Inner> Instance { get; } =
                    from dot in x2EParser.Instance
                    from bits in BitParser.Instance.AtLeastOnce()
                    select new BinVal.ConcatenatedBits.Inner(dot, bits);
            }
        }

        public static Parser<BinVal.Range> Range { get; } = RangeParser.Instance;

        public static class RangeParser
        {
            public static Parser<BinVal.Range> Instance { get; } =
                from b in x62Parser.Instance
                from bits in BitParser.Instance.AtLeastOnce()
                from inners in InnerParser.Instance.AtLeastOnce()
                select new BinVal.Range(b, bits, inners);

            public static class InnerParser
            {
                public static Parser<BinVal.Range.Inner> Instance { get; } =
                    from dash in x2DParser.Instance
                    from bits in BitParser.Instance.AtLeastOnce()
                    select new BinVal.Range.Inner(dash, bits);
            }
        }

        public static Parser<BinVal> Instance { get; } =
            BitsOnly
            .Or<BinVal>(ConcatenatedBits)
            .Or(Range);
    }
}
