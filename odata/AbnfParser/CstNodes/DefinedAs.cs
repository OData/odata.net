namespace AbnfParser.CstNodes
{
    using System.Collections.Generic;

    using AbnfParser.CstNodes.Core;

    public abstract class DefinedAs
    {
        private DefinedAs()
        {
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
        }
    }
}
