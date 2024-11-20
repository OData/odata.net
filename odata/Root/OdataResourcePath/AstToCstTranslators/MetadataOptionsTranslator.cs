namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class MetadataOptionsTranslator :
        AbstractSyntaxTreeNodes.MetadataOptions.Visitor<
            ConcreteSyntaxTreeNodes.MetadataOptions,
            Void>
    {
        private MetadataOptionsTranslator()
        {
        }

        public static MetadataOptionsTranslator Default { get; } = new MetadataOptionsTranslator();
    }
}
