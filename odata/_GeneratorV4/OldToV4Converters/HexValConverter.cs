namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using Root;

    public sealed class HexValConverter : AbnfParser.CstNodes.HexVal.Visitor<_hexⲻval, Root.Void>
    {
        private HexValConverter()
        {
        }

        public static HexValConverter Instance { get; } = new HexValConverter();

        protected internal override _hexⲻval Accept(HexVal.HexOnly node, Void context)
        {
            return new _hexⲻval(
                new Inners._ʺx78ʺ(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                null);
        }

        protected internal override _hexⲻval Accept(HexVal.ConcatenatedHex node, Void context)
        {
            return new _hexⲻval(
                new Inners._ʺx78ʺ(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                new Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ(
                    node.Inners.Select(inner =>
                        new Inners._Ⲥʺx2Eʺ_1ЖHEXDIGↃ(
                            new Inners._ʺx2Eʺ_1ЖHEXDIG(
                                new Inners._ʺx2Eʺ(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.HexDigs.Select(hexDig =>
                                    HexDigConverter.Instance.Visit(hexDig, context)))))));
        }

        protected internal override _hexⲻval Accept(HexVal.Range node, Void context)
        {
            return new _hexⲻval(
                new Inners._ʺx78ʺ(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                new Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ(
                    new Inners._Ⲥʺx2Dʺ_1ЖHEXDIGↃ(
                        new Inners._ʺx2Dʺ_1ЖHEXDIG(
                            new Inners._ʺx2Dʺ(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().HexDigs.Select(hexDig =>
                                HexDigConverter.Instance.Visit(hexDig, context))))));
        }
    }
}
