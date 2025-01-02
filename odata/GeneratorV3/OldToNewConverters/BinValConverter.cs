namespace GeneratorV3.OldToNewConverters
{
    using System.Linq;

    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using GeneratorV3.OldToNewConverters.Core;
    using Root;

    public sealed class BinValConverter : AbnfParser.CstNodes.BinVal.Visitor<GeneratorV3.Abnf._binⲻval, Root.Void>
    {
        private BinValConverter()
        {
        }

        public static BinValConverter Instance { get; } = new BinValConverter();

        protected internal override _binⲻval Accept(BinVal.BitsOnly node, Void context)
        {
            return new _binⲻval(
                new Inners._doublequotex62doublequote(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)),
                null);
        }

        protected internal override _binⲻval Accept(BinVal.ConcatenatedBits node, Void context)
        {
            return new _binⲻval(
                new Inners._doublequotex62doublequote(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)),
                new Inners._1asteriskopendoublequotex2Edoublequote_1asteriskBITↃⳆopendoublequotex2Ddoublequote_1asteriskBITↃ._1asteriskopendoublequotex2Edoublequote_1asteriskBITↃ(
                    node.Inners.Select(inner =>
                        new Inners._opendoublequotex2Edoublequote_1asteriskBITↃ(
                            new Inners._doublequotex2Edoublequote_1asteriskBIT(
                                new Inners._doublequotex2Edoublequote(
                                    x2EConverter.Instance.Convert(inner.Dot)),
                                inner.Bits.Select(bit =>
                                    BitConverter.Instance.Visit(bit, context)))))));
        }

        protected internal override _binⲻval Accept(BinVal.Range node, Void context)
        {
            return new _binⲻval(
                new Inners._doublequotex62doublequote(
                    x62Converter.Instance.Convert(node.B)),
                node.Bits.Select(bit =>
                    BitConverter.Instance.Visit(bit, context)),
                new Inners._1asteriskopendoublequotex2Edoublequote_1asteriskBITↃⳆopendoublequotex2Ddoublequote_1asteriskBITↃ._opendoublequotex2Ddoublequote_1asteriskBITↃ(
                    new Inners._opendoublequotex2Ddoublequote_1asteriskBITↃ(
                        new Inners._doublequotex2Ddoublequote_1asteriskBIT(
                            new Inners._doublequotex2Ddoublequote(
                                x2DConverter.Instance.Convert(node.Inners.First().Dash)),
                            node.Inners.First().Bits.Select(bit =>
                                BitConverter.Instance.Visit(bit, context))))));
        }
    }
}
