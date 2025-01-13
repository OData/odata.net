namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _keyPredicate
    {
        private _keyPredicate()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_keyPredicate node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_keyPredicate._simpleKey node, TContext context);
            protected internal abstract TResult Accept(_keyPredicate._compoundKey node, TContext context);
            protected internal abstract TResult Accept(_keyPredicate._keyPathSegments node, TContext context);
        }
        
        public sealed class _simpleKey : _keyPredicate
        {
            public _simpleKey(__GeneratedOdata.CstNodes.Rules._simpleKey _simpleKey_1)
            {
                this._simpleKey_1 = _simpleKey_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._simpleKey _simpleKey_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _compoundKey : _keyPredicate
        {
            public _compoundKey(__GeneratedOdata.CstNodes.Rules._compoundKey _compoundKey_1)
            {
                this._compoundKey_1 = _compoundKey_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._compoundKey _compoundKey_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _keyPathSegments : _keyPredicate
        {
            public _keyPathSegments(__GeneratedOdata.CstNodes.Rules._keyPathSegments _keyPathSegments_1)
            {
                this._keyPathSegments_1 = _keyPathSegments_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._keyPathSegments _keyPathSegments_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
