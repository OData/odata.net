namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _primitiveKeyPropertyⳆkeyPropertyAlias
    {
        private _primitiveKeyPropertyⳆkeyPropertyAlias()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_primitiveKeyPropertyⳆkeyPropertyAlias node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty node, TContext context);
            protected internal abstract TResult Accept(_primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias node, TContext context);
        }
        
        public sealed class _primitiveKeyProperty : _primitiveKeyPropertyⳆkeyPropertyAlias
        {
            public _primitiveKeyProperty(__GeneratedOdata.CstNodes.Rules._primitiveKeyProperty _primitiveKeyProperty_1)
            {
                this._primitiveKeyProperty_1 = _primitiveKeyProperty_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveKeyProperty _primitiveKeyProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _keyPropertyAlias : _primitiveKeyPropertyⳆkeyPropertyAlias
        {
            public _keyPropertyAlias(__GeneratedOdata.CstNodes.Rules._keyPropertyAlias _keyPropertyAlias_1)
            {
                this._keyPropertyAlias_1 = _keyPropertyAlias_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._keyPropertyAlias _keyPropertyAlias_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
