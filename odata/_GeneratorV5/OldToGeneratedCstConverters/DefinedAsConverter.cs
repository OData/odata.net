namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using System.Linq;

    public sealed class DefinedAsConverter : AbnfParser.CstNodes.DefinedAs.Visitor<__Generated.CstNodes.Rules._definedⲻas, Root.Void>
    {
        private DefinedAsConverter()
        {
        }

        public static DefinedAsConverter Instance { get; } = new DefinedAsConverter();

        protected internal override __Generated.CstNodes.Rules._definedⲻas Accept(AbnfParser.CstNodes.DefinedAs.Declaration node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._definedⲻas(
                node.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)),
                new __Generated.CstNodes.Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ(
                    new __Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dʺ(
                        new __Generated.CstNodes.Inners._ʺx3Dʺ(
                            x3DConverter.Instance.Convert(node.Equals)))),
                node.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)));
        }

        protected internal override __Generated.CstNodes.Rules._definedⲻas Accept(AbnfParser.CstNodes.DefinedAs.Incremental node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._definedⲻas(
                node.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)),
                new __Generated.CstNodes.Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ(
                    new __Generated.CstNodes.Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dx2Fʺ(
                        new __Generated.CstNodes.Inners._ʺx3Dx2Fʺ(
                            x3DConverter.Instance.Convert(node.Equals),
                            x2FConverter.Instance.Convert(node.Slash)))),
                node.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)));
        }
    }
}
