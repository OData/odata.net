namespace Root.OdataResourcePath.AbstractSyntaxTreeNodes
{
    // TODO this is just a stub for now
    public abstract class QualifiedEntityTypeName
    {
        private QualifiedEntityTypeName()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(QualifiedEntityTypeName node, TContext context)
            {
                return node.Dispatch(this, context);
            }
        }
    }
}
