namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class QueryOptionsTranslator :
        AbstractSyntaxTreeNodes.QueryOptions.Visitor<
            ConcreteSyntaxTreeNodes.QueryOptions,
            Void>
    {
        private QueryOptionsTranslator()
        {
        }

        public static QueryOptionsTranslator Default { get; } = new QueryOptionsTranslator();
    }
}
