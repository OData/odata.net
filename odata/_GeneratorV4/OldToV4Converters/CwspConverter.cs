namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;

    public sealed class CwspConverter : AbnfParser.CstNodes.Cwsp.Visitor<_cⲻwsp, Root.Void>
    {
        private CwspConverter()
        {
        }

        public static CwspConverter Instance { get; } = new CwspConverter();

        protected internal override _cⲻwsp Accept(Cwsp.WspOnly node, Root.Void context)
        {
            return new _cⲻwsp._WSP(
                WspConverter.Instance.Visit(node.Wsp, context));
        }

        protected internal override _cⲻwsp Accept(Cwsp.CnlAndWsp node, Root.Void context)
        {
            return new _cⲻwsp._Ⲥcⲻnl_WSPↃ(
                new Inners._Ⲥcⲻnl_WSPↃ(
                    new Inners._cⲻnl_WSP(
                        CnlConverter.Instance.Visit(node.Cnl, context),
                        WspConverter.Instance.Visit(node.Wsp, context))));
        }
    }
}
