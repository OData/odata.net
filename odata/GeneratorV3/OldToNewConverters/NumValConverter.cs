namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class NumValConverter : AbnfParser.CstNodes.NumVal.Visitor<GeneratorV3.Abnf._numⲻval, Root.Void>
    {
        private NumValConverter()
        {
        }

        public static NumValConverter Instance { get; } = new NumValConverter();

        protected internal override _numⲻval Accept(NumVal.BinVal node, Void context)
        {
        }

        protected internal override _numⲻval Accept(NumVal.DecVal node, Void context)
        {
        }

        protected internal override _numⲻval Accept(NumVal.HexVal node, Void context)
        {
        }
    }
}
