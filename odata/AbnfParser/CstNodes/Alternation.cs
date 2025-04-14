namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public sealed class Alternation
    {
        public Alternation(Concatenation concatenation, IEnumerable<Inner> inners)
        {
            Concatenation = concatenation;
            Inners = inners;
        }

        public Concatenation Concatenation { get; }
        public IEnumerable<Inner> Inners { get; }

        public sealed class Inner
        {
            public Inner(IEnumerable<Cwsp> prefixCwsps, x2F slash, IEnumerable<Cwsp> suffixCwsps, Concatenation concatenation)
            {
                PrefixCwsps = prefixCwsps;
                Slash = slash;
                SuffixCwsps = suffixCwsps;
                Concatenation = concatenation;
            }

            public IEnumerable<Cwsp> PrefixCwsps { get; }
            public x2F Slash { get; }
            public IEnumerable<Cwsp> SuffixCwsps { get; }
            public Concatenation Concatenation { get; }
        }
    }
}
