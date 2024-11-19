namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class MetadataOptionsTranslator :
        AbstractSyntaxTree.MetadataOptions.Visitor<
            ConcreteSyntaxTree.MetadataOptions,
            Void>
    {
        private MetadataOptionsTranslator()
        {
        }

        public static MetadataOptionsTranslator Default { get; } = new MetadataOptionsTranslator();
    }
}
