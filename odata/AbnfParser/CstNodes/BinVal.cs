namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;
    using System.Collections.Generic;

    public abstract class BinVal
    {
        private BinVal()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(BinVal node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(BitsOnly node, TContext context);
            protected internal abstract TResult Accept(ConcatenatedBits node, TContext context);
            protected internal abstract TResult Accept(Range node, TContext context);
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

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
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

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
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

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
