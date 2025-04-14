namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public abstract class DecVal
    {
        private DecVal()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(DecVal node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(DecsOnly node, TContext context);
            protected internal abstract TResult Accept(ConcatenatedDecs node, TContext context);
            protected internal abstract TResult Accept(Range node, TContext context);
        }

        public sealed class DecsOnly : DecVal
        {
            public DecsOnly(x64 d, IEnumerable<Digit> digits)
            {
                D = d;
                Digits = digits; //// TODO assert one or more
            }

            public x64 D { get; }
            public IEnumerable<Digit> Digits { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class ConcatenatedDecs : DecVal
        {
            public ConcatenatedDecs(x64 d, IEnumerable<Digit> digits, IEnumerable<Inner> inners)
            {
                D = d;
                Digits = digits; //// TODO assert one or more
                Inners = inners; //// TODO assert one or more
            }

            public x64 D { get; }
            public IEnumerable<Digit> Digits { get; }
            public IEnumerable<Inner> Inners { get; }

            public sealed class Inner
            {
                public Inner(x2E dot, IEnumerable<Digit> digits)
                {
                    Dot = dot;
                    Digits = digits; //// TODO assert one or more
                }

                public x2E Dot { get; }
                public IEnumerable<Digit> Digits { get; }
            }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class Range : DecVal
        {
            public Range(x64 d, IEnumerable<Digit> digits, IEnumerable<Inner> inners)
            {
                D = d;
                Digits = digits; //// TODO assert one or more
                Inners = inners; //// TODO assert one or more
            }

            public x64 D { get; }
            public IEnumerable<Digit> Digits { get; }
            public IEnumerable<Inner> Inners { get; }

            public sealed class Inner
            {
                public Inner(x2D dash, IEnumerable<Digit> digits)
                {
                    Dash = dash;
                    Digits = digits; //// TODO assert one or more
                }

                public x2D Dash { get; }
                public IEnumerable<Digit> Digits { get; }
            }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
