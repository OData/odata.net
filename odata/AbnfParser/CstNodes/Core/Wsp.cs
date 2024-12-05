namespace AbnfParser.CstNodes.Core
{
    public abstract class Wsp
    {
        private Wsp()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Wsp node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(Space node, TContext context);
            protected internal abstract TResult Accept(Tab node, TContext context);
        }

        public sealed class Space : Wsp
        {
            public Space(Sp sp)
            {
                Sp = sp;
            }

            public Sp Sp { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class Tab : Wsp
        {
            public Tab(Htab htab)
            {
                Htab = htab;
            }

            public Htab Htab { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
