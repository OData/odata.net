namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public abstract class DefinedAs
    {
        private DefinedAs()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(DefinedAs node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(Declaration node, TContext context);
            protected internal abstract TResult Accept(Incremental node, TContext context);
        }

        public sealed class Declaration : DefinedAs
        {
            public Declaration(IEnumerable<Cwsp> prefixCwsps, x3D equals, IEnumerable<Cwsp> suffixCwsps)
            {
                PrefixCwsps = prefixCwsps;
                Equals = equals;
                SuffixCwsps = suffixCwsps;
            }

            public IEnumerable<Cwsp> PrefixCwsps { get; }
            public x3D Equals { get; }
            public IEnumerable<Cwsp> SuffixCwsps { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class Incremental : DefinedAs
        {
            public Incremental(IEnumerable<Cwsp> prefixCwsps, x3D equals, x2F slash, IEnumerable<Cwsp> suffixCwsps)
            {
                PrefixCwsps = prefixCwsps;
                Equals = equals;
                Slash = slash;
                SuffixCwsps = suffixCwsps;
            }

            public IEnumerable<Cwsp> PrefixCwsps { get; }
            public x3D Equals { get; }
            public x2F Slash { get; }
            public IEnumerable<Cwsp> SuffixCwsps { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
