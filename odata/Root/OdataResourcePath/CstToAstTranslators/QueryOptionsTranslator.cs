namespace Root.OdataResourcePath.CstToAstTranslators
{
    using Root;

    public sealed class QueryOptionsTranslator :
        ConcreteSyntaxTreeNodes.QueryOptions.Visitor<
            AbstractSyntaxTreeNodes.QueryOptions,
            Void>
    {
        private QueryOptionsTranslator()
        {
        }

        public static QueryOptionsTranslator Default { get; } = new QueryOptionsTranslator();
    }
}
