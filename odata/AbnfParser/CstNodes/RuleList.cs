namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;
    using System.Collections.Generic;

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc5234
    /// </summary>
    public sealed class RuleList
    {
        public RuleList(IEnumerable<Inner> inners)
        {
            //// TODO assert one or more elements
            Inners = inners;
        }

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

                protected internal abstract TResult Accept(RuleInner node, TContext context);
                protected internal abstract TResult Accept(CommentInner node, TContext context);
            }

            public sealed class RuleInner : Inner
            {
                public RuleInner(Rule rule)
                {
                    Rule = rule;
                }

                public Rule Rule { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class CommentInner : Inner
            {
                public CommentInner(IEnumerable<Cwsp> cwsps, Cnl cnl)
                {
                    Cwsps = cwsps;
                    Cnl = cnl;
                }

                public IEnumerable<Cwsp> Cwsps { get; }
                public Cnl Cnl { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
}
