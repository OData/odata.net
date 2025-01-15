namespace __GeneratedOdata.CstNodes.Inners
{
    public abstract class _ʺx7DʺⳆʺx25x37x44ʺ
    {
        private _ʺx7DʺⳆʺx25x37x44ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx7DʺⳆʺx25x37x44ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx7DʺⳆʺx25x37x44ʺ._ʺx7Dʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx7DʺⳆʺx25x37x44ʺ._ʺx25x37x44ʺ node, TContext context);
        }
        
        public sealed class _ʺx7Dʺ : _ʺx7DʺⳆʺx25x37x44ʺ
        {
            public _ʺx7Dʺ(__GeneratedOdata.CstNodes.Inners._ʺx7Dʺ _ʺx7Dʺ_1)
            {
                this._ʺx7Dʺ_1 = _ʺx7Dʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx7Dʺ _ʺx7Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx25x37x44ʺ : _ʺx7DʺⳆʺx25x37x44ʺ
        {
            public _ʺx25x37x44ʺ(__GeneratedOdata.CstNodes.Inners._ʺx25x37x44ʺ _ʺx25x37x44ʺ_1)
            {
                this._ʺx25x37x44ʺ_1 = _ʺx25x37x44ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx25x37x44ʺ _ʺx25x37x44ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
