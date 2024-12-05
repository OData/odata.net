namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public abstract class Repeat
    {
        private Repeat()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(Repeat node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(Count node, TContext context);
            protected internal abstract TResult Accept(Range node, TContext context);
        }

        public sealed class Count : Repeat
        {
            public Count(IEnumerable<Digit> digits)
            {
                //// TODO assert one or more
                Digits = digits;
            }

            public IEnumerable<Digit> Digits { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class Range : Repeat
        {
            public Range(IEnumerable<Digit> prefixDigits, x2A asterisk, IEnumerable<Digit> suffixDigits)
            {
                PrefixDigits = prefixDigits;
                Asterisk = asterisk;
                SuffixDigits = suffixDigits;
            }

            public IEnumerable<Digit> PrefixDigits { get; }
            public x2A Asterisk { get; }
            public IEnumerable<Digit> SuffixDigits { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
