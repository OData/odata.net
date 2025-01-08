namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;

    public sealed class CnlConverter : AbnfParser.CstNodes.Cnl.Visitor<_cⲻnl, Root.Void>
    {
        private CnlConverter()
        {
        }

        public static CnlConverter Instance { get; } = new CnlConverter();

        protected internal override _cⲻnl Accept(Cnl.Comment node, Root.Void context)
        {
            return new _cⲻnl._comment(
                CommentConverter.Instance.Convert(node.Value));
        }

        protected internal override _cⲻnl Accept(Cnl.Newline node, Root.Void context)
        {
            return new _cⲻnl._CRLF(
                CrLfConverter.Instance.Convert(node.Crlf));
        }
    }
}
