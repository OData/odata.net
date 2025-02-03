namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _host
    {
        private _host()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_host node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_host._IPⲻliteral node, TContext context);
            protected internal abstract TResult Accept(_host._IPv4address node, TContext context);
            protected internal abstract TResult Accept(_host._regⲻname node, TContext context);
        }
        
        public sealed class _IPⲻliteral : _host
        {
            public _IPⲻliteral(__GeneratedOdataV3.CstNodes.Rules._IPⲻliteral _IPⲻliteral_1)
            {
                this._IPⲻliteral_1 = _IPⲻliteral_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._IPⲻliteral _IPⲻliteral_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _IPv4address : _host
        {
            public _IPv4address(__GeneratedOdataV3.CstNodes.Rules._IPv4address _IPv4address_1)
            {
                this._IPv4address_1 = _IPv4address_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._IPv4address _IPv4address_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _regⲻname : _host
        {
            public _regⲻname(__GeneratedOdataV3.CstNodes.Rules._regⲻname _regⲻname_1)
            {
                this._regⲻname_1 = _regⲻname_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._regⲻname _regⲻname_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
