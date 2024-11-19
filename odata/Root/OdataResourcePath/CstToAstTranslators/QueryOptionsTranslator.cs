namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

    public sealed class QueryOptionsTranslator :
        ConcreteSyntaxTree.QueryOptions.Visitor<
            AbstractSyntaxTree.QueryOptions,
            Void>
    {
        private QueryOptionsTranslator()
        {
        }

        public static QueryOptionsTranslator Default { get; } = new QueryOptionsTranslator();
    }
}
