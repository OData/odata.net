namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class WspConverter : AbnfParser.CstNodes.Core.Wsp.Visitor<__Generated.CstNodes.Rules._WSP, Root.Void>
    {
        private WspConverter()
        {
        }

        public static WspConverter Instance { get; } = new WspConverter();

        protected internal override __Generated.CstNodes.Rules._WSP Accept(AbnfParser.CstNodes.Core.Wsp.Space node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._WSP._SP(
                new __Generated.CstNodes.Rules._SP(
                    new __Generated.CstNodes.Inners._Ⰳx20(
                        __Generated.CstNodes.Inners._2.Instance,
                        __Generated.CstNodes.Inners._0.Instance)));
        }

        protected internal override __Generated.CstNodes.Rules._WSP Accept(AbnfParser.CstNodes.Core.Wsp.Tab node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._WSP._HTAB(
                new __Generated.CstNodes.Rules._HTAB(
                    new __Generated.CstNodes.Inners._Ⰳx09(
                        __Generated.CstNodes.Inners._0.Instance,
                        __Generated.CstNodes.Inners._9.Instance)));
        }
    }
}
