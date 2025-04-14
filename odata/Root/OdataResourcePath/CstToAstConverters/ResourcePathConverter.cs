namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;

    public sealed class ResourcePathConverter :
        ConcreteSyntaxTreeNodes.ResourcePath.Visitor<
            AbstractSyntaxTreeNodes.ResourcePath,
            Void>
    {
        private ResourcePathConverter()
        {
        }

        public static ResourcePathConverter Default { get; } = new ResourcePathConverter();
    }
}
