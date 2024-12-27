namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;

    public sealed class CwspConverter : AbnfParser.CstNodes.Cwsp.Visitor<GeneratorV3.Abnf._cⲻwsp, Root.Void>
    {
        private CwspConverter()
        {
        }

        public static CwspConverter Instance { get; } = new CwspConverter();

        protected internal override _cⲻwsp Accept(Cwsp.WspOnly node, Root.Void context)
        {
        }

        protected internal override _cⲻwsp Accept(Cwsp.CnlAndWsp node, Root.Void context)
        {
        }
    }
}
