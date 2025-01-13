namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _function
    {
        private _function()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_function node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_function._entityFunction node, TContext context);
            protected internal abstract TResult Accept(_function._entityColFunction node, TContext context);
            protected internal abstract TResult Accept(_function._complexFunction node, TContext context);
            protected internal abstract TResult Accept(_function._complexColFunction node, TContext context);
            protected internal abstract TResult Accept(_function._primitiveFunction node, TContext context);
            protected internal abstract TResult Accept(_function._primitiveColFunction node, TContext context);
        }
        
        public sealed class _entityFunction : _function
        {
            public _entityFunction(__GeneratedOdata.CstNodes.Rules._entityFunction _entityFunction_1)
            {
                this._entityFunction_1 = _entityFunction_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._entityFunction _entityFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _entityColFunction : _function
        {
            public _entityColFunction(__GeneratedOdata.CstNodes.Rules._entityColFunction _entityColFunction_1)
            {
                this._entityColFunction_1 = _entityColFunction_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._entityColFunction _entityColFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexFunction : _function
        {
            public _complexFunction(__GeneratedOdata.CstNodes.Rules._complexFunction _complexFunction_1)
            {
                this._complexFunction_1 = _complexFunction_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._complexFunction _complexFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _complexColFunction : _function
        {
            public _complexColFunction(__GeneratedOdata.CstNodes.Rules._complexColFunction _complexColFunction_1)
            {
                this._complexColFunction_1 = _complexColFunction_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._complexColFunction _complexColFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveFunction : _function
        {
            public _primitiveFunction(__GeneratedOdata.CstNodes.Rules._primitiveFunction _primitiveFunction_1)
            {
                this._primitiveFunction_1 = _primitiveFunction_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveFunction _primitiveFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveColFunction : _function
        {
            public _primitiveColFunction(__GeneratedOdata.CstNodes.Rules._primitiveColFunction _primitiveColFunction_1)
            {
                this._primitiveColFunction_1 = _primitiveColFunction_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._primitiveColFunction _primitiveColFunction_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
