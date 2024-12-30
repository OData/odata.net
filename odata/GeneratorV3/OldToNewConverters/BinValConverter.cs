namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class BinValConverter : AbnfParser.CstNodes.BinVal.Visitor<GeneratorV3.Abnf._binⲻval, Root.Void>
    {
        private BinValConverter()
        {
        }

        public static BinValConverter Instance { get; } = new BinValConverter();

        protected internal override _binⲻval Accept(BinVal.BitsOnly node, Void context)
        {
        }

        protected internal override _binⲻval Accept(BinVal.ConcatenatedBits node, Void context)
        {
        }

        protected internal override _binⲻval Accept(BinVal.Range node, Void context)
        {
        }
    }
}
