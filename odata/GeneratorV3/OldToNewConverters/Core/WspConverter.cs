namespace GeneratorV3.OldToNewConverters.Core
{
    public sealed class WspConverter : AbnfParser.CstNodes.Core.Wsp.Visitor<GeneratorV3.Abnf._WSP, Root.Void>
    {
        private WspConverter()
        {
        }

        public static WspConverter Instance { get; } = new WspConverter();

        protected internal override GeneratorV3.Abnf._WSP Accept(AbnfParser.CstNodes.Core.Wsp.Space node, Root.Void context)
        {
        }

        protected internal override GeneratorV3.Abnf._WSP Accept(AbnfParser.CstNodes.Core.Wsp.Tab node, Root.Void context)
        {
        }
    }
}
