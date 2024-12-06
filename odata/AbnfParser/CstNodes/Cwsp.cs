namespace AbnfParser.CstNodes
{
    using AbnfParser.CstNodes.Core;

    public abstract class Cwsp
    {
        private Cwsp()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Cwsp node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(WspOnly node, TContext context);
            protected internal abstract TResult Accept(CnlAndWsp node, TContext context);
        }

        public sealed class WspOnly : Cwsp
        {
            public WspOnly(Wsp wsp)
            {
                Wsp = wsp;
            }

            public Wsp Wsp { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class CnlAndWsp : Cwsp
        {
            public CnlAndWsp(Cnl cnl, Wsp wsp)
            {
                Cnl = cnl;
                Wsp = wsp;
            }

            public Cnl Cnl { get; }
            public Wsp Wsp { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
