namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class BatchOptionsTranslator : 
        AbstractSyntaxTree.BatchOptions.Visitor<
            ConcreteSyntaxTree.BatchOptions, 
            Void>
    {
        private BatchOptionsTranslator()
        {
        }

        public static BatchOptionsTranslator Default { get; } = new BatchOptionsTranslator();
    }
}
