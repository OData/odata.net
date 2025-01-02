namespace GeneratorV3.OldToNewConverters.Core
{
    using AbnfParser.CstNodes.Core;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class BitConverter : AbnfParser.CstNodes.Core.Bit.Visitor<GeneratorV3.Abnf._BIT, Root.Void>
    {
        private BitConverter()
        {
        }

        public static BitConverter Instance { get; } = new BitConverter();

        protected internal override _BIT Accept(Bit.Zero node, Void context)
        {
            return new _BIT._ʺx30ʺ(
                new Inners._ʺx30ʺ(
                    x30Converter.Instance.Convert(node.Value)));
        }

        protected internal override _BIT Accept(Bit.One node, Void context)
        {
            return new _BIT._ʺx31ʺ(
                new Inners._ʺx31ʺ(
                    x31Converter.Instance.Convert(node.Value)));
        }
    }
}
