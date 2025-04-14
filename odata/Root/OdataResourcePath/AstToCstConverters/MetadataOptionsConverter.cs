namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class MetadataOptionsConverter :
        AbstractSyntaxTreeNodes.MetadataOptions.Visitor<
            ConcreteSyntaxTreeNodes.MetadataOptions,
            Void>
    {
        private MetadataOptionsConverter()
        {
        }

        public static MetadataOptionsConverter Default { get; } = new MetadataOptionsConverter();
    }
}
