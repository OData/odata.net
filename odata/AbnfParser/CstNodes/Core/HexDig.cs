namespace AbnfParser.CstNodes.Core
{
    public abstract class HexDig
    {
        private HexDig()
        {
        }

        public abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(HexDig node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(Digit node, TContext context);
            protected internal abstract TResult Accept(A node, TContext context);
            protected internal abstract TResult Accept(B node, TContext context);
            protected internal abstract TResult Accept(C node, TContext context);
            protected internal abstract TResult Accept(D node, TContext context);
            protected internal abstract TResult Accept(E node, TContext context);
            protected internal abstract TResult Accept(F node, TContext context);
        }

        public sealed class Digit : HexDig
        {
            public Digit(Core.Digit value)
            {
                Value = value;
            }

            public Core.Digit Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class A : HexDig
        {
            public A(x41 value)
            {
                Value = value;
            }

            public x41 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class B : HexDig
        {
            public B(x42 value)
            {
                Value = value;
            }

            public x42 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class C : HexDig
        {
            public C(x43 value)
            {
                Value = value;
            }

            public x43 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class D : HexDig
        {
            public D(x44 value)
            {
                Value = value;
            }

            public x44 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class E : HexDig
        {
            public E(x45 value)
            {
                Value = value;
            }

            public x45 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class F : HexDig
        {
            public F(x46 value)
            {
                Value = value;
            }

            public x46 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
