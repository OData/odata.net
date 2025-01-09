namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class CnlConverter : AbnfParser.CstNodes.Cnl.Visitor<__Generated.CstNodes.Rules._cⲻnl, Root.Void>
    {
        private CnlConverter()
        {
        }

        public static CnlConverter Instance { get; } = new CnlConverter();

        protected internal override __Generated.CstNodes.Rules._cⲻnl Accept(AbnfParser.CstNodes.Cnl.Comment node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._cⲻnl._comment(
                CommentConverter.Instance.Convert(node.Value));
        }

        protected internal override _cⲻnl Accept(AbnfParser.CstNodes.Cnl.Newline node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._cⲻnl._CRLF(
                CrLfConverter.Instance.Convert(node.Crlf));
        }
    }
}
