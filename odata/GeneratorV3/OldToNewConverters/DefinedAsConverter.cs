namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using GeneratorV3.OldToNewConverters.Core;

    public sealed class DefinedAsConverter : AbnfParser.CstNodes.DefinedAs.Visitor<GeneratorV3.Abnf._definedⲻas, Root.Void>
    {
        private DefinedAsConverter()
        {
        }

        public static DefinedAsConverter Instance { get; } = new DefinedAsConverter();

        protected internal override _definedⲻas Accept(DefinedAs.Declaration node, Root.Void context)
        {
            return new _definedⲻas(
                node.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)),
                new Inners._Ⲥdoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ(
                    new Inners._doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote._doublequotex3Ddoublequote(
                        new Inners._doublequotex3Ddoublequote(
                            x3DConverter.Instance.Convert(node.Equals)))),
                node.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)));
        }

        protected internal override _definedⲻas Accept(DefinedAs.Incremental node, Root.Void context)
        {
            return new _definedⲻas(
                node.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)),
                new Inners._Ⲥdoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ(
                    new Inners._doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote._doublequotex3Dx2Fdoublequote(
                        new Inners._doublequotex3Dx2Fdoublequote(
                            x3DConverter.Instance.Convert(node.Equals),
                            x2FConverter.Instance.Convert(node.Slash)))),
                node.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)));
        }
    }
}
