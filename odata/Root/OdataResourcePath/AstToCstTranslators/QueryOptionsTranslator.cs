namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class QueryOptionsTranslator :
        AbstractSyntaxTree.QueryOptions.Visitor<
            ConcreteSyntaxTree.QueryOptions,
            Void>
    {
        private QueryOptionsTranslator()
        {
        }

        public static QueryOptionsTranslator Default { get; } = new QueryOptionsTranslator();
    }
}
