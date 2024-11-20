namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class ResourcePathTranslator :
        ConcreteSyntaxTreeNodes.ResourcePath.Visitor<
            AbstractSyntaxTreeNodes.ResourcePath,
            Void>
    {
        private ResourcePathTranslator()
        {
        }

        public static ResourcePathTranslator Default { get; } = new ResourcePathTranslator();
    }
}
