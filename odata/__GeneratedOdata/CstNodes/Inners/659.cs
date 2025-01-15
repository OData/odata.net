namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _IPv6addressⳆIPvFuture
    {
        private _IPv6addressⳆIPvFuture()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_IPv6addressⳆIPvFuture node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_IPv6addressⳆIPvFuture._IPv6address node, TContext context);
            protected internal abstract TResult Accept(_IPv6addressⳆIPvFuture._IPvFuture node, TContext context);
        }
        
        public sealed class _IPv6address : _IPv6addressⳆIPvFuture
        {
            public _IPv6address(__GeneratedOdata.CstNodes.Rules._IPv6address _IPv6address_1)
            {
                this._IPv6address_1 = _IPv6address_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._IPv6address _IPv6address_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _IPvFuture : _IPv6addressⳆIPvFuture
        {
            public _IPvFuture(__GeneratedOdata.CstNodes.Rules._IPvFuture _IPvFuture_1)
            {
                this._IPvFuture_1 = _IPvFuture_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._IPvFuture _IPvFuture_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
