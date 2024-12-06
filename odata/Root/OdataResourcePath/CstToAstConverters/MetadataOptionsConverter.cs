namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;

    public sealed class MetadataOptionsConverter :
        ConcreteSyntaxTreeNodes.MetadataOptions.Visitor<
            AbstractSyntaxTreeNodes.MetadataOptions,
            Void>
    {
        private MetadataOptionsConverter()
        {
        }

        public static MetadataOptionsConverter Default { get; } = new MetadataOptionsConverter();
    }
}
