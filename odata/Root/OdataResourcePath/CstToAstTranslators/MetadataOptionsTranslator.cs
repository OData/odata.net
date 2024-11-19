namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class MetadataOptionsTranslator :
        ConcreteSyntaxTree.MetadataOptions.Visitor<
            AbstractSyntaxTree.MetadataOptions,
            Void>
    {
        private MetadataOptionsTranslator()
        {
        }

        public static MetadataOptionsTranslator Default { get; } = new MetadataOptionsTranslator();
    }
}
