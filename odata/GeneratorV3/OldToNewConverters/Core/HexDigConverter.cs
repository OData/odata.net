namespace GeneratorV3.OldToNewConverters.Core
{
    using AbnfParser.CstNodes.Core;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class HexDigConverter : AbnfParser.CstNodes.Core.HexDig.Visitor<GeneratorV3.Abnf._HEXDIG, Root.Void>
    {
        private HexDigConverter()
        {
        }

        public static HexDigConverter Instance { get; } = new HexDigConverter();

        protected internal override _HEXDIG Accept(HexDig.Digit node, Void context)
        {
        }

        protected internal override _HEXDIG Accept(HexDig.A node, Void context)
        {
        }

        protected internal override _HEXDIG Accept(HexDig.B node, Void context)
        {
        }

        protected internal override _HEXDIG Accept(HexDig.C node, Void context)
        {
        }

        protected internal override _HEXDIG Accept(HexDig.D node, Void context)
        {
        }

        protected internal override _HEXDIG Accept(HexDig.E node, Void context)
        {
        }

        protected internal override _HEXDIG Accept(HexDig.F node, Void context)
        {
        }
    }
}
