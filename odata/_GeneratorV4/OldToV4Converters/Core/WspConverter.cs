namespace _GeneratorV4.OldToV4Converters
{
    using _GeneratorV4.Abnf.CstNodes;

    public sealed class WspConverter : AbnfParser.CstNodes.Core.Wsp.Visitor<_WSP, Root.Void>
    {
        private WspConverter()
        {
        }

        public static WspConverter Instance { get; } = new WspConverter();

        protected internal override _WSP Accept(AbnfParser.CstNodes.Core.Wsp.Space node, Root.Void context)
        {
            return new _WSP._SP(
                new _SP(
                    new Inners._Ⰳx20(
                        Inners._2.Instance,
                        Inners._0.Instance)));
        }

        protected internal override _WSP Accept(AbnfParser.CstNodes.Core.Wsp.Tab node, Root.Void context)
        {
            return new _WSP._HTAB(
                new _HTAB(
                    new Inners._Ⰳx09(
                        Inners._0.Instance,
                        Inners._9.Instance)));
        }
    }
}
