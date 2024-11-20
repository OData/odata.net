namespace Root.OdataResourcePath.AstToCstConverters
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
