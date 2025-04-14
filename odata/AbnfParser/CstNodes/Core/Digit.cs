namespace AbnfParser.CstNodes.Core
{
    public abstract class Digit
    {
        private Digit()
        {
        }
        public abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Digit node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(x30 node, TContext context);
            protected internal abstract TResult Accept(x31 node, TContext context);
            protected internal abstract TResult Accept(x32 node, TContext context);
            protected internal abstract TResult Accept(x33 node, TContext context);
            protected internal abstract TResult Accept(x34 node, TContext context);
            protected internal abstract TResult Accept(x35 node, TContext context);
            protected internal abstract TResult Accept(x36 node, TContext context);
            protected internal abstract TResult Accept(x37 node, TContext context);
            protected internal abstract TResult Accept(x38 node, TContext context);
            protected internal abstract TResult Accept(x39 node, TContext context);
        }

        public sealed class x30 : Digit
        {
            public x30(Core.x30 value)
            {
                Value = value;
            }

            public Core.x30 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x31 : Digit
        {
            public x31(Core.x31 value)
            {
                Value = value;
            }

            public Core.x31 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x32 : Digit
        {
            public x32(Core.x32 value)
            {
                Value = value;
            }

            public Core.x32 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x33 : Digit
        {
            public x33(Core.x33 value)
            {
                Value = value;
            }

            public Core.x33 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x34 : Digit
        {
            public x34(Core.x34 value)
            {
                Value = value;
            }

            public Core.x34 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x35 : Digit
        {
            public x35(Core.x35 value)
            {
                Value = value;
            }

            public Core.x35 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x36 : Digit
        {
            public x36(Core.x36 value)
            {
                Value = value;
            }

            public Core.x36 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x37 : Digit
        {
            public x37(Core.x37 value)
            {
                Value = value;
            }

            public Core.x37 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x38 : Digit
        {
            public x38(Core.x38 value)
            {
                Value = value;
            }

            public Core.x38 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }

        public sealed class x39 : Digit
        {
            public x39(Core.x39 value)
            {
                Value = value;
            }

            public Core.x39 Value { get; }

            public sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

        }
    }
}
