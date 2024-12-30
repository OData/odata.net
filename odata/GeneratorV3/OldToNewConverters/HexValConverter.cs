namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class HexValConverter : AbnfParser.CstNodes.HexVal.Visitor<GeneratorV3.Abnf._hexⲻval, Root.Void>
    {
        private HexValConverter()
        {
        }

        public static HexValConverter Instance { get; } = new HexValConverter();

        protected internal override _hexⲻval Accept(HexVal.HexOnly node, Void context)
        {
        }

        protected internal override _hexⲻval Accept(HexVal.ConcatenatedHex node, Void context)
        {
        }

        protected internal override _hexⲻval Accept(HexVal.Range node, Void context)
        {
        }
    }
}
