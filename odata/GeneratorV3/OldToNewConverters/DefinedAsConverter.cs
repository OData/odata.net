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
                new Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ(
                    new Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dʺ(
                        new Inners._ʺx3Dʺ(
                            x3DConverter.Instance.Convert(node.Equals)))),
                node.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)));
        }

        protected internal override _definedⲻas Accept(DefinedAs.Incremental node, Root.Void context)
        {
            return new _definedⲻas(
                node.PrefixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)),
                new Inners._Ⲥʺx3DʺⳆʺx3Dx2FʺↃ(
                    new Inners._ʺx3DʺⳆʺx3Dx2Fʺ._ʺx3Dx2Fʺ(
                        new Inners._ʺx3Dx2Fʺ(
                            x3DConverter.Instance.Convert(node.Equals),
                            x2FConverter.Instance.Convert(node.Slash)))),
                node.SuffixCwsps.Select(cwsp =>
                    CwspConverter.Instance.Visit(cwsp, context)));
        }
    }
}
