namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public sealed class Comment
    {
        public Comment(x3B semicolon, IEnumerable<Inner> inners, Crlf crlf)
        {
            Semicolon = semicolon;
            Inners = inners;
            Crlf = crlf;
        }

        public x3B Semicolon { get; }
        public IEnumerable<Inner> Inners { get; }
        public Crlf Crlf { get; }

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

                protected internal abstract TResult Accept(InnerWsp node, TContext context);
                protected internal abstract TResult Accept(InnerVchar node, TContext context);
            }

            public sealed class InnerWsp : Inner
            {
                public InnerWsp(Wsp wsp)
                {
                    Wsp = wsp;
                }

                public Wsp Wsp { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class InnerVchar : Inner
            {
                public InnerVchar(Vchar vchar)
                {
                    Vchar = vchar;
                }

                public Vchar Vchar { get; }

                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
}
