namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using GeneratorV3.OldToNewConverters.Core;

    public sealed class CwspConverter : AbnfParser.CstNodes.Cwsp.Visitor<GeneratorV3.Abnf._cⲻwsp, Root.Void>
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
