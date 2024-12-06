namespace Root.OdataResourcePath.ConcreteSyntaxTreeNodes
{
    // TODO this is just a stub for now
    public abstract class QueryOptions
    {
        private QueryOptions()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(QueryOptions node, TContext context)
            {
                return node.Dispatch(this, context);
            }
        }
    }
}
