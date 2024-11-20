namespace Root.OdataResourcePath.ConcreteSyntaxTreeNodes
{
    // TODO this is just a stub for now
    public abstract class BatchOptions
    {
        private BatchOptions()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(BatchOptions node, TContext context)
            {
                return node.Dispatch(this, context);
            }
        }
    }
}
