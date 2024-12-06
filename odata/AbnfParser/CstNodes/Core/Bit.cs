namespace AbnfParser.CstNodes.Core
{
    public abstract class Bit
    {
        private Bit()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Bit node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(Zero node, TContext context);
            protected internal abstract TResult Accept(One node, TContext context);
        }

        public sealed class Zero : Bit
        {
            public Zero(x30 value)
            {
                Value = value;
            }

            public x30 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class One : Bit
        {
            public One(x31 value)
            {
                Value = value;
            }

            public x31 Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
