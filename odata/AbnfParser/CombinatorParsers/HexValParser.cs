namespace AbnfParser.CombinatorParsers
{
    using AbnfParser.CombinatorParsers.Core;
    using AbnfParser.CstNodes;
    using Sprache;

    public static class HexValParser
    {
        public static Parser<HexVal.HexOnly> HexOnly { get; } =
            from x in x78Parser.Instance
            from hexDigs in HexDigParser.Instance.AtLeastOnce()
            select new HexVal.HexOnly(x, hexDigs);

        public static Parser<HexVal.ConcatenatedHex> ConcatenatedHex { get; } = ConcatenatedHexParser.Instance;

        public static class ConcatenatedHexParser
        {
            public static Parser<HexVal.ConcatenatedHex> Instance { get; } =
                from x in x78Parser.Instance
                from hexDigs in HexDigParser.Instance.AtLeastOnce()
                from inners in InnerParser.Instance.AtLeastOnce()
                select new HexVal.ConcatenatedHex(x, hexDigs, inners);

            public static class InnerParser
            {
                public static Parser<HexVal.ConcatenatedHex.Inner> Instance { get; } =
                    from dot in x2EParser.Instance
                    from hexDigs in HexDigParser.Instance.AtLeastOnce()
                    select new HexVal.ConcatenatedHex.Inner(dot, hexDigs);
            }
        }

        public static Parser<HexVal.Range> Range { get; } = RangeParser.Instance;

        public static class RangeParser
        {
            public static Parser<HexVal.Range> Instance { get; } =
                from x in x78Parser.Instance
                from hexDigs in HexDigParser.Instance.AtLeastOnce()
                from inners in InnerParser.Instance.AtLeastOnce()
                select new HexVal.Range(x, hexDigs, inners);

            public static class InnerParser
            {
                public static Parser<HexVal.Range.Inner> Instance { get; } =
                    from dash in x2DParser.Instance
                    from hexDigs in HexDigParser.Instance.AtLeastOnce()
                    select new HexVal.Range.Inner(dash, hexDigs);
            }
        }

        public static Parser<HexVal> Instance { get; } = //// TODO you originally wrote this as hexonly.or(concatenated).or(range); how can you know to do it in this order instead?
            ConcatenatedHex
            .Or<HexVal>(Range)
            .Or(HexOnly);
    }
}
