namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;

    public sealed class BatchOptionsConverter : 
        ConcreteSyntaxTreeNodes.BatchOptions.Visitor<
            AbstractSyntaxTreeNodes.BatchOptions, 
            Void>
    {
        private BatchOptionsConverter()
        {
        }

        public static BatchOptionsConverter Default { get; } = new BatchOptionsConverter();
    }
}
