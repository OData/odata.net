namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;
    using Root;

    public sealed class RepeatConverter : AbnfParser.CstNodes.Repeat.Visitor<GeneratorV3.Abnf._repeat, Root.Void>
    {
        private RepeatConverter()
        {
        }

        public static RepeatConverter Instance { get; } = new RepeatConverter();

        protected internal override _repeat Accept(Repeat.Count node, Void context)
        {
        }

        protected internal override _repeat Accept(Repeat.Range node, Void context)
        {
        }
    }
}
