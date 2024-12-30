namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class DecValConverter : AbnfParser.CstNodes.DecVal.Visitor<GeneratorV3.Abnf._decⲻval, Root.Void>
    {
        private DecValConverter()
        {
        }

        public static DecValConverter Instance { get; } = new DecValConverter();

        protected internal override _decⲻval Accept(DecVal.DecsOnly node, Void context)
        {
        }

        protected internal override _decⲻval Accept(DecVal.ConcatenatedDecs node, Void context)
        {
        }

        protected internal override _decⲻval Accept(DecVal.Range node, Void context)
        {
        }
    }
}
