namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;

    public sealed class QueryOptionsConverter :
        ConcreteSyntaxTreeNodes.QueryOptions.Visitor<
            AbstractSyntaxTreeNodes.QueryOptions,
            Void>
    {
        private QueryOptionsConverter()
        {
        }

        public static QueryOptionsConverter Default { get; } = new QueryOptionsConverter();
    }
}
