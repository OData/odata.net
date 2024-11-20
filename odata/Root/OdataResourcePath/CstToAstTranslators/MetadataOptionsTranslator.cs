namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class MetadataOptionsTranslator :
        ConcreteSyntaxTreeNodes.MetadataOptions.Visitor<
            AbstractSyntaxTreeNodes.MetadataOptions,
            Void>
    {
        private MetadataOptionsTranslator()
        {
        }

        public static MetadataOptionsTranslator Default { get; } = new MetadataOptionsTranslator();
    }
}
