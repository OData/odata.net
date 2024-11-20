namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class BatchOptionsConverter : 
        AbstractSyntaxTreeNodes.BatchOptions.Visitor<
            ConcreteSyntaxTreeNodes.BatchOptions, 
            Void>
    {
        private BatchOptionsConverter()
        {
        }

        public static BatchOptionsConverter Default { get; } = new BatchOptionsConverter();
    }
}
