namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _dummyStartRule
    {
        private _dummyStartRule()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_dummyStartRule node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_dummyStartRule._odataUri node, TContext context);
            protected internal abstract TResult Accept(_dummyStartRule._header node, TContext context);
            protected internal abstract TResult Accept(_dummyStartRule._primitiveValue node, TContext context);
        }
        
        public sealed class _odataUri : _dummyStartRule
        {
            public _odataUri(__GeneratedOdataV3.CstNodes.Rules._odataUri _odataUri_1)
            {
                this._odataUri_1 = _odataUri_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._odataUri _odataUri_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _header : _dummyStartRule
        {
            public _header(__GeneratedOdataV3.CstNodes.Rules._header _header_1)
            {
                this._header_1 = _header_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._header _header_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _primitiveValue : _dummyStartRule
        {
            public _primitiveValue(__GeneratedOdataV3.CstNodes.Rules._primitiveValue _primitiveValue_1)
            {
                this._primitiveValue_1 = _primitiveValue_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._primitiveValue _primitiveValue_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
