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
        }

        protected internal override _BIT Accept(Bit.One node, Void context)
        {
        }
    }
}
