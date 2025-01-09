namespace _GeneratorV5.OldToGeneratedCstConverters
{
    public sealed class CwspConverter : AbnfParser.CstNodes.Cwsp.Visitor<__Generated.CstNodes.Rules._cⲻwsp, Root.Void>
    {
        private CwspConverter()
        {
        }

        public static CwspConverter Instance { get; } = new CwspConverter();

        protected internal override __Generated.CstNodes.Rules._cⲻwsp Accept(AbnfParser.CstNodes.Cwsp.WspOnly node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._cⲻwsp._WSP(
                WspConverter.Instance.Visit(node.Wsp, context));
        }

        protected internal override __Generated.CstNodes.Rules._cⲻwsp Accept(AbnfParser.CstNodes.Cwsp.CnlAndWsp node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._cⲻwsp._Ⲥcⲻnl_WSPↃ(
                new __Generated.CstNodes.Inners._Ⲥcⲻnl_WSPↃ(
                    new __Generated.CstNodes.Inners._cⲻnl_WSP(
                        CnlConverter.Instance.Visit(node.Cnl, context),
                        WspConverter.Instance.Visit(node.Wsp, context))));
        }
    }
}
