namespace __Generated.CstNodes.Rules
{
    public abstract class _cⲻnl
    {
        private _cⲻnl()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_cⲻnl node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_cⲻnl._comment node, TContext context);
            protected internal abstract TResult Accept(_cⲻnl._CRLF node, TContext context);
        }
        
        public sealed class _comment : _cⲻnl
        {
            public _comment(__Generated.CstNodes.Rules._comment _comment_1)
            {
                this._comment_1 = _comment_1;
            }
            
            public __Generated.CstNodes.Rules._comment _comment_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _CRLF : _cⲻnl
        {
            public _CRLF(__Generated.CstNodes.Rules._CRLF _CRLF_1)
            {
                this._CRLF_1 = _CRLF_1;
            }
            
            public __Generated.CstNodes.Rules._CRLF _CRLF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
