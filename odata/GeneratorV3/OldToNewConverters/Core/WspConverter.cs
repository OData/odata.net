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
            return new Abnf._WSP._SP(
                new Abnf._SP(
                    new Abnf.Inners._Ⰳx20(
                        Abnf.Inners._2.Instance,
                        Abnf.Inners._0.Instance)));
        }

        protected internal override GeneratorV3.Abnf._WSP Accept(AbnfParser.CstNodes.Core.Wsp.Tab node, Root.Void context)
        {
            return new Abnf._WSP._HTAB(
                new Abnf._HTAB(
                    new Abnf.Inners._Ⰳx09(
                        Abnf.Inners._0.Instance,
                        Abnf.Inners._9.Instance)));
        }
    }
}
