namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;
    using System.Collections.Generic;

    public abstract class HexVal
    {
        private HexVal()
        {
        }

        public sealed class HexOnly : HexVal
        {
            public HexOnly(x78 x, IEnumerable<HexDig> hexDigs)
            {
                X = x;
                HexDigs = hexDigs; //// TODO assert one or more
            }

            public x78 X { get; }
            public IEnumerable<HexDig> HexDigs { get; }
        }

        public sealed class ConcatenatedHex : HexVal
        {
            public ConcatenatedHex(x78 x, IEnumerable<HexDig> hexDigs, IEnumerable<Inner> inners)
            {
                X = x;
                HexDigs = hexDigs; //// TODO assert one or more
                Inners = inners; //// TODO assert one or more
            }

            public x78 X { get; }
            public IEnumerable<HexDig> HexDigs { get; }
            public IEnumerable<Inner> Inners { get; }

            public sealed class Inner
            {
                public Inner(x2E dot, IEnumerable<HexDig> hexDigs)
                {
                    Dot = dot;
                    HexDigs = hexDigs; //// TODO assert one or more
                }

                public x2E Dot { get; }
                public IEnumerable<HexDig> HexDigs { get; }
            }
        }

        public sealed class Range : HexVal
        {
            public Range(x78 x, IEnumerable<HexDig> hexDigs, IEnumerable<Inner> inners)
            {
                X = x;
                HexDigs = hexDigs; //// TODO assert one or more
                Inners = inners; //// TODO assert one or more
            }

            public x78 X { get; }
            public IEnumerable<HexDig> HexDigs { get; }
            public IEnumerable<Inner> Inners { get; }

            public sealed class Inner
            {
                public Inner(x2D dash, IEnumerable<HexDig> hexDigs)
                {
                    Dash = dash;
                    HexDigs = hexDigs; //// TODO assert one or more
                }

                public x2D Dash { get; }
                public IEnumerable<HexDig> HexDigs { get; }
            }
        }
    }
}
