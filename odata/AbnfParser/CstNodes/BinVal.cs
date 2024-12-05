namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;
    using System.Collections.Generic;

    public abstract class BinVal
    {
        private BinVal()
        {
        }

        public sealed class BitsOnly : BinVal
        {
            public BitsOnly(x62 b, IEnumerable<Bit> bits)
            {
                B = b;
                Bits = bits; //// TODO assert one or more
            }

            public x62 B { get; }
            public IEnumerable<Bit> Bits { get; }
        }

        public sealed class ConcatenatedBits : BinVal
        {
            public ConcatenatedBits(x62 b, IEnumerable<Bit> bits, IEnumerable<Inner> inners)
            {
                B = b;
                Bits = bits; //// TODO assert one or more
                Inners = inners; //// TODO assert one or more
            }

            public x62 B { get; }
            public IEnumerable<Bit> Bits { get; }
            public IEnumerable<Inner> Inners { get; }

            public sealed class Inner
            {
                public Inner(x2E dot, IEnumerable<Bit> bits)
                {
                    Dot = dot;
                    Bits = bits; //// TODO assert one or more
                }

                public x2E Dot { get; }
                public IEnumerable<Bit> Bits { get; }
            }
        }

        public sealed class Range : BinVal
        {
            public Range(x62 b, IEnumerable<Bit> bits, IEnumerable<Inner> inners)
            {
                B = b;
                Bits = bits; //// TODO assert one or more
                Inners = inners; //// TODO assert one or more
            }

            public x62 B { get; }
            public IEnumerable<Bit> Bits { get; }
            public IEnumerable<Inner> Inners { get; }

            public sealed class Inner
            {
                public Inner(x2D dash, IEnumerable<Bit> bits)
                {
                    Dash = dash;
                    Bits = bits; //// TODO assert one or more
                }

                public x2D Dash { get; }
                public IEnumerable<Bit> Bits { get; }
            }
        }
    }
}
