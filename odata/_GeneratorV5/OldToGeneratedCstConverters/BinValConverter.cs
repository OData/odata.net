namespace _GeneratorV5.OldToGeneratedCstConverters
{
    using _GeneratorV5.ManualParsers.Rules;
    using System.Linq;

    public sealed class BinValConverter : AbnfParser.CstNodes.BinVal.Visitor<__Generated.CstNodes.Rules._binⲻval, Root.Void>
    {
        private BinValConverter()
        {
        }

        public static BinValConverter Instance { get; } = new BinValConverter();

        protected internal override __Generated.CstNodes.Rules._binⲻval Accept(AbnfParser.CstNodes.BinVal.BitsOnly node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._binⲻval(
                new __Generated.CstNodes.Inners._ʺx62ʺ(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)).Convert2(),
                null);
        }

        protected internal override __Generated.CstNodes.Rules._binⲻval Accept(AbnfParser.CstNodes.BinVal.ConcatenatedBits node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._binⲻval(
                new __Generated.CstNodes.Inners._ʺx62ʺ(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)).Convert2(),
                new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._1ЖⲤʺx2Eʺ_1ЖBITↃ(
                    node.Inners.Select(inner =>
                        new __Generated.CstNodes.Inners._Ⲥʺx2Eʺ_1ЖBITↃ(
                            new __Generated.CstNodes.Inners._ʺx2Eʺ_1ЖBIT(
                                new __Generated.CstNodes.Inners._ʺx2Eʺ(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.Bits.Select(bit =>
                                    BitConverter.Instance.Visit(bit, context)).Convert2()))).Convert2()));
        }

        protected internal override __Generated.CstNodes.Rules._binⲻval Accept(AbnfParser.CstNodes.BinVal.Range node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._binⲻval(
                new __Generated.CstNodes.Inners._ʺx62ʺ(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)).Convert2(),
                new __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._Ⲥʺx2Dʺ_1ЖBITↃ(
                    new __Generated.CstNodes.Inners._Ⲥʺx2Dʺ_1ЖBITↃ(
                        new __Generated.CstNodes.Inners._ʺx2Dʺ_1ЖBIT(
                            new __Generated.CstNodes.Inners._ʺx2Dʺ(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().Bits.Select(bit =>
                                BitConverter.Instance.Visit(bit, context)).Convert2()))));
        }
    }
}
