namespace _GeneratorV4.OldToV4Converters
{
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParser.CstNodes;
    using Root;

    public sealed class BinValConverter : AbnfParser.CstNodes.BinVal.Visitor<_binⲻval, Root.Void>
    {
        private BinValConverter()
        {
        }

        public static BinValConverter Instance { get; } = new BinValConverter();

        protected internal override _binⲻval Accept(BinVal.BitsOnly node, Void context)
        {
            return new _binⲻval(
                new Inners._ʺx62ʺ(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)),
                null);
        }

        protected internal override _binⲻval Accept(BinVal.ConcatenatedBits node, Void context)
        {
            return new _binⲻval(
                new Inners._ʺx62ʺ(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)),
                new Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._1ЖⲤʺx2Eʺ_1ЖBITↃ(
                    node.Inners.Select(inner =>
                        new Inners._Ⲥʺx2Eʺ_1ЖBITↃ(
                            new Inners._ʺx2Eʺ_1ЖBIT(
                                new Inners._ʺx2Eʺ(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.Bits.Select(bit =>
                                    BitConverter.Instance.Visit(bit, context)))))));
        }

        protected internal override _binⲻval Accept(BinVal.Range node, Void context)
        {
            return new _binⲻval(
                new Inners._ʺx62ʺ(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)),
                new Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._Ⲥʺx2Dʺ_1ЖBITↃ(
                    new Inners._Ⲥʺx2Dʺ_1ЖBITↃ(
                        new Inners._ʺx2Dʺ_1ЖBIT(
                            new Inners._ʺx2Dʺ(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().Bits.Select(bit =>
                                BitConverter.Instance.Visit(bit, context))))));
        }
    }
}
