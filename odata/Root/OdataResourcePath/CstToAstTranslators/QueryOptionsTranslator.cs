namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

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
