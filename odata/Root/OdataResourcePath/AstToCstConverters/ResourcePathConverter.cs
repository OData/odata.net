namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class ResourcePathConverter :
        AbstractSyntaxTreeNodes.ResourcePath.Visitor<
            ConcreteSyntaxTreeNodes.ResourcePath,
            Void>
    {
        private ResourcePathConverter()
        {
        }

        public static ResourcePathConverter Default { get; } = new ResourcePathConverter();
    }
}
