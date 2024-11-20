namespace Root.OdataResourcePath.AbstractSyntaxTreeNodes
{
    // TODO this is just a stub for now
    public abstract class MetadataOptions
    {
        private MetadataOptions()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(MetadataOptions node, TContext context)
            {
                return node.Dispatch(this, context);
            }
        }
    }
}