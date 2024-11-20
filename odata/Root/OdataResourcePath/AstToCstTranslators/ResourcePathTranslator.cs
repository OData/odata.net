namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class ResourcePathTranslator :
        AbstractSyntaxTreeNodes.ResourcePath.Visitor<
            ConcreteSyntaxTreeNodes.ResourcePath,
            Void>
    {
        private ResourcePathTranslator()
        {
        }

        public static ResourcePathTranslator Default { get; } = new ResourcePathTranslator();
    }
}
