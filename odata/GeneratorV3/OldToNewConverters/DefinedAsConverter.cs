namespace GeneratorV3.OldToNewConverters
{
    using AbnfParser.CstNodes;
    using GeneratorV3.Abnf;

    public sealed class DefinedAsConverter : AbnfParser.CstNodes.DefinedAs.Visitor<GeneratorV3.Abnf._definedⲻas, Root.Void>
    {
        private DefinedAsConverter()
        {
        }

        public static DefinedAsConverter Instance { get; } = new DefinedAsConverter();

        protected internal override _definedⲻas Accept(DefinedAs.Declaration node, Root.Void context)
        {
        }

        protected internal override _definedⲻas Accept(DefinedAs.Incremental node, Root.Void context)
        {
        }
    }
}
