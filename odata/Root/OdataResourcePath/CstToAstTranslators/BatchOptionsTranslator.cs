namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

    public sealed class BatchOptionsTranslator : 
        ConcreteSyntaxTree.BatchOptions.Visitor<
            AbstractSyntaxTree.BatchOptions, 
            Void>
    {
        private BatchOptionsTranslator()
        {
        }

        public static BatchOptionsTranslator Default { get; } = new BatchOptionsTranslator();
    }
}
