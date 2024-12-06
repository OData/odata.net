namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;

    public abstract class Cnl
    {
        private Cnl()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Cnl node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(Comment node, TContext context);
            protected internal abstract TResult Accept(Newline node, TContext context);
        }

        public sealed class Comment : Cnl
        {
            public Comment(CstNodes.Comment value)
            {
                Value = value;
            }

            public CstNodes.Comment Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class Newline : Cnl
        {
            public Newline(Crlf crlf)
            {
                Crlf = crlf;
            }

            public Crlf Crlf { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
