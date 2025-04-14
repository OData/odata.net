using AbnfParser.CstNodes.Core;

namespace AbnfParser.CstNodes
{
    public abstract class Repetition
    {
        private Repetition()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Repetition node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(ElementOnly node, TContext context);
            protected internal abstract TResult Accept(RepeatAndElement node, TContext context);
        }

        public sealed class ElementOnly : Repetition
        {
            public ElementOnly(Element element)
            {
                Element = element;
            }

            public Element Element { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class RepeatAndElement : Repetition
        {
            public RepeatAndElement(Repeat repeat, Element element)
            {
                Repeat = repeat;
                Element = element;
            }

            public Repeat Repeat { get; }
            public Element Element { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
