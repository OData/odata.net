namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using GeneratorV3.OldToNewConverters.Core;
    using Root;

    public sealed class HexValConverter : AbnfParser.CstNodes.HexVal.Visitor<GeneratorV3.Abnf._hexⲻval, Root.Void>
    {
        private HexValConverter()
        {
        }

        public static HexValConverter Instance { get; } = new HexValConverter();

        protected internal override _hexⲻval Accept(HexVal.HexOnly node, Void context)
        {
            return new _hexⲻval(
                new Inners._doublequotex78doublequote(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                null);
        }

        protected internal override _hexⲻval Accept(HexVal.ConcatenatedHex node, Void context)
        {
            return new _hexⲻval(
                new Inners._doublequotex78doublequote(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                new Inners._ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ._ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ(
                    node.Inners.Select(inner =>
                        new Inners._opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ(
                            new Inners._doublequotex2Edoublequote_ONEasteriskHEXDIG(
                                new Inners._doublequotex2Edoublequote(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.HexDigs.Select(hexDig =>
                                    HexDigConverter.Instance.Visit(hexDig, context)))))));
        }

        protected internal override _hexⲻval Accept(HexVal.Range node, Void context)
        {
            return new _hexⲻval(
                new Inners._doublequotex78doublequote(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                new Inners._ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ._opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ(
                    new Inners._opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ(
                        new Inners._doublequotex2Ddoublequote_ONEasteriskHEXDIG(
                            new Inners._doublequotex2Ddoublequote(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().HexDigs.Select(hexDig =>
                                HexDigConverter.Instance.Visit(hexDig, context))))));
        }
    }
}
