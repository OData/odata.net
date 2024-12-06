using AbnfParser.CstNodes.Core;
using System.Collections.Generic;

namespace AbnfParser.CstNodes
{
    public sealed class RuleName
    {
        public RuleName(Alpha alpha, IEnumerable<Inner> inners)
        {
            Alpha = alpha;
            Inners = inners;
        }

        public Alpha Alpha { get; }
        public IEnumerable<Inner> Inners { get; }

        public abstract class Inner
        {
            private Inner()
            {
            }

            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(Inner node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(AlphaInner node, TContext context);
                protected internal abstract TResult Accept(DigitInner node, TContext context);
                protected internal abstract TResult Accept(DashInner node, TContext context);
            }

            public sealed class AlphaInner : Inner
            {
                public AlphaInner(Alpha alpha)
                {
                    Alpha = alpha;
                }

                public Alpha Alpha { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class DigitInner : Inner
            {
                public DigitInner(Digit digit)
                {
                    Digit = digit;
                }

                public Digit Digit { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class DashInner : Inner
            {
                public DashInner(x2D dash)
                {
                    Dash = dash;
                }

                public x2D Dash { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
}
