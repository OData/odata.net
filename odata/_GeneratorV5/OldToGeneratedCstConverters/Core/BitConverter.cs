namespace _GeneratorV4.OldToGeneratedCstConverters
{
    public sealed class BitConverter : AbnfParser.CstNodes.Core.Bit.Visitor<__Generated.CstNodes.Rules._BIT, Root.Void>
    {
        private BitConverter()
        {
        }

        public static BitConverter Instance { get; } = new BitConverter();

        protected internal override __Generated.CstNodes.Rules._BIT Accept(AbnfParser.CstNodes.Core.Bit.Zero node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._BIT._ʺx30ʺ(
                new __Generated.CstNodes.Inners._ʺx30ʺ(
                    x30Converter.Instance.Convert(node.Value)));
        }

        protected internal override __Generated.CstNodes.Rules._BIT Accept(AbnfParser.CstNodes.Core.Bit.One node, Root.Void context)
        {
            return new __Generated.CstNodes.Rules._BIT._ʺx31ʺ(
                new __Generated.CstNodes.Inners._ʺx31ʺ(
                    x31Converter.Instance.Convert(node.Value)));
        }
    }
}
