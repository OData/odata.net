namespace AbnfParser.CstNodes
{
    public abstract class Element
    {
        private Element()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Element node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(RuleName node, TContext context);
            protected internal abstract TResult Accept(Group node, TContext context);
            protected internal abstract TResult Accept(Option node, TContext context);
            protected internal abstract TResult Accept(CharVal node, TContext context);
            protected internal abstract TResult Accept(NumVal node, TContext context);
            protected internal abstract TResult Accept(ProseVal node, TContext context);
        }

        public sealed class RuleName : Element
        {
            public RuleName(CstNodes.RuleName value)
            {
                Value = value;
            }

            public CstNodes.RuleName Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class Group : Element
        {
            public Group(CstNodes.Group value)
            {
                Value = value;
            }

            public CstNodes.Group Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class Option : Element
        {
            public Option(CstNodes.Option value)
            {
                Value = value;
            }

            public CstNodes.Option Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class CharVal : Element
        {
            public CharVal(CstNodes.CharVal value)
            {
                Value = value;
            }

            public CstNodes.CharVal Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class NumVal : Element
        {
            public NumVal(CstNodes.NumVal value)
            {
                Value = value;
            }

            public CstNodes.NumVal Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class ProseVal : Element
        {
            public ProseVal(CstNodes.ProseVal value)
            {
                Value = value;
            }

            public CstNodes.ProseVal Value { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
