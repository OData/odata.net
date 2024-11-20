namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class BatchOptionsTranslator : 
        AbstractSyntaxTreeNodes.BatchOptions.Visitor<
            ConcreteSyntaxTreeNodes.BatchOptions, 
            Void>
    {
        private BatchOptionsTranslator()
        {
        }

        public static BatchOptionsTranslator Default { get; } = new BatchOptionsTranslator();
    }
}
