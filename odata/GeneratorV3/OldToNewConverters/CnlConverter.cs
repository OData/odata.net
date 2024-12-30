namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;

    public sealed class CnlConverter : AbnfParser.CstNodes.Cnl.Visitor<GeneratorV3.Abnf._cⲻnl, Root.Void>
    {
        private CnlConverter()
        {
        }

        public static CnlConverter Instance { get; } = new CnlConverter();

        protected internal override _cⲻnl Accept(Cnl.Comment node, Root.Void context)
        {
            return new GeneratorV3.Abnf._cⲻnl._comment(
                CommentConverter.Instance.Convert(node.Value));
        }

        protected internal override _cⲻnl Accept(Cnl.Newline node, Root.Void context)
        {
            return new GeneratorV3.Abnf._cⲻnl._CRLF(
                CrLfConverter.Instance.Convert(node.Crlf));
        }
    }
}
