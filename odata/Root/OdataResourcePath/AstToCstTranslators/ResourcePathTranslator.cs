namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class ResourcePathTranslator :
        AbstractSyntaxTree.ResourcePath.Visitor<
            ConcreteSyntaxTree.ResourcePath,
            Void>
    {
        private ResourcePathTranslator()
        {
        }

        public static ResourcePathTranslator Default { get; } = new ResourcePathTranslator();
    }
}
