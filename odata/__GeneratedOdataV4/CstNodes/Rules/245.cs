namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _primitiveProperty
    {
        private _primitiveProperty()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_primitiveProperty node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_primitiveProperty._primitiveKeyProperty node, TContext context);
            protected internal abstract TResult Accept(_primitiveProperty._primitiveNonKeyProperty node, TContext context);
        }
        
        public sealed class _primitiveKeyProperty : _primitiveProperty
        {
            public _primitiveKeyProperty(__GeneratedOdataV4.CstNodes.Rules._primitiveKeyProperty _primitiveKeyProperty_1)
            {
                this._primitiveKeyProperty_1 = _primitiveKeyProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._primitiveKeyProperty _primitiveKeyProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveNonKeyProperty : _primitiveProperty
        {
            public _primitiveNonKeyProperty(__GeneratedOdataV4.CstNodes.Rules._primitiveNonKeyProperty _primitiveNonKeyProperty_1)
            {
                this._primitiveNonKeyProperty_1 = _primitiveNonKeyProperty_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._primitiveNonKeyProperty _primitiveNonKeyProperty_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
