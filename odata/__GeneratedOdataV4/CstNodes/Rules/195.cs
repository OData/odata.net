namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _arrayOrObject
    {
        private _arrayOrObject()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_arrayOrObject node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_arrayOrObject._complexColInUri node, TContext context);
            protected internal abstract TResult Accept(_arrayOrObject._complexInUri node, TContext context);
            protected internal abstract TResult Accept(_arrayOrObject._rootExprCol node, TContext context);
            protected internal abstract TResult Accept(_arrayOrObject._primitiveColInUri node, TContext context);
        }
        
        public sealed class _complexColInUri : _arrayOrObject
        {
            public _complexColInUri(__GeneratedOdataV4.CstNodes.Rules._complexColInUri _complexColInUri_1)
            {
                this._complexColInUri_1 = _complexColInUri_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._complexColInUri _complexColInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexInUri : _arrayOrObject
        {
            public _complexInUri(__GeneratedOdataV4.CstNodes.Rules._complexInUri _complexInUri_1)
            {
                this._complexInUri_1 = _complexInUri_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._complexInUri _complexInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _rootExprCol : _arrayOrObject
        {
            public _rootExprCol(__GeneratedOdataV4.CstNodes.Rules._rootExprCol _rootExprCol_1)
            {
                this._rootExprCol_1 = _rootExprCol_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._rootExprCol _rootExprCol_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveColInUri : _arrayOrObject
        {
            public _primitiveColInUri(__GeneratedOdataV4.CstNodes.Rules._primitiveColInUri _primitiveColInUri_1)
            {
                this._primitiveColInUri_1 = _primitiveColInUri_1;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._primitiveColInUri _primitiveColInUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
