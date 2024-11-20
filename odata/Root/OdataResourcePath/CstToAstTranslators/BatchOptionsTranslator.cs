namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class BatchOptionsTranslator : 
        ConcreteSyntaxTreeNodes.BatchOptions.Visitor<
            AbstractSyntaxTreeNodes.BatchOptions, 
            Void>
    {
        private BatchOptionsTranslator()
        {
        }

        public static BatchOptionsTranslator Default { get; } = new BatchOptionsTranslator();
    }
}
