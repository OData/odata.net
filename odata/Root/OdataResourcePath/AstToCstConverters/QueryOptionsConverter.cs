namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class QueryOptionsConverter :
        AbstractSyntaxTreeNodes.QueryOptions.Visitor<
            ConcreteSyntaxTreeNodes.QueryOptions,
            Void>
    {
        private QueryOptionsConverter()
        {
        }

        public static QueryOptionsConverter Default { get; } = new QueryOptionsConverter();
    }
}
