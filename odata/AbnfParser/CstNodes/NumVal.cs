using AbnfParser.CstNodes.Core;

namespace AbnfParser.CstNodes
{
    public abstract class NumVal
    {
        private NumVal()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(NumVal node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(BinVal node, TContext context);
            protected internal abstract TResult Accept(DecVal node, TContext context);
            protected internal abstract TResult Accept(HexVal node, TContext context);
        }

        public sealed class BinVal : NumVal
        {
            public BinVal(x25 percent, CstNodes.BinVal value)
            {
                Percent = percent;
                Value = value;
            }

            public x25 Percent { get; }
            public CstNodes.BinVal Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class DecVal : NumVal
        {
            public DecVal(x25 percent, CstNodes.DecVal value)
            {
                Percent = percent;
                Value = value;
            }

            public x25 Percent { get; }
            public CstNodes.DecVal Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class HexVal : NumVal
        {
            public HexVal(x25 percent, CstNodes.HexVal value)
            {
                Percent = percent;
                Value = value;
            }

            public x25 Percent { get; }
            public CstNodes.HexVal Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
