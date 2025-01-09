namespace _GeneratorV4.OldToGeneratedCstConverters
{
    using System.Linq;

    public sealed class HexValConverter : AbnfParser.CstNodes.HexVal.Visitor<__Generated.CstNodes.Rules._hexⲻval, Root.Void>
    {
        private HexValConverter()
        {
        }

        public static HexValConverter Instance { get; } = new HexValConverter();

        protected internal override __Generated.CstNodes.Rules._hexⲻval Accept(AbnfParser.CstNodes.HexVal.HexOnly node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._hexⲻval(
                new __Generated.CstNodes.Inners._ʺx78ʺ(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                null);
        }

        protected internal override __Generated.CstNodes.Rules._hexⲻval Accept(AbnfParser.CstNodes.HexVal.ConcatenatedHex node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._hexⲻval(
                new __Generated.CstNodes.Inners._ʺx78ʺ(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ(
                    node.Inners.Select(inner =>
                        new __Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖHEXDIGↃ(
                            new __Generated.CstNodes.Inners._ʺx2Eʺ_1ЖHEXDIG(
                                new __Generated.CstNodes.Inners._ʺx2Eʺ(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.HexDigs.Select(hexDig =>
                                    HexDigConverter.Instance.Visit(hexDig, context)))))));
        }

        protected internal override __Generated.CstNodes.Rules._hexⲻval Accept(AbnfParser.CstNodes.HexVal.Range node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._hexⲻval(
                new __Generated.CstNodes.Inners._ʺx78ʺ(
                    x78Converter.Instance.Convert(node.X)),
                node.HexDigs.Select(hexDig =>
                    HexDigConverter.Instance.Visit(hexDig, context)),
                new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ(
                    new __Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖHEXDIGↃ(
                        new __Generated.CstNodes.Inners._ʺx2Dʺ_1ЖHEXDIG(
                            new __Generated.CstNodes.Inners._ʺx2Dʺ(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().HexDigs.Select(hexDig =>
                                HexDigConverter.Instance.Visit(hexDig, context))))));
        }
    }
}
