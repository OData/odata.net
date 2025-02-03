namespace __GeneratedOdataV3.CstNodes.Inners
{
    public abstract class _ʺx30ʺⳆʺx31ʺ
    {
        private _ʺx30ʺⳆʺx31ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx30ʺⳆʺx31ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx30ʺⳆʺx31ʺ._ʺx30ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx30ʺⳆʺx31ʺ._ʺx31ʺ node, TContext context);
        }
        
        public sealed class _ʺx30ʺ : _ʺx30ʺⳆʺx31ʺ
        {
            public _ʺx30ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1)
            {
                this._ʺx30ʺ_1 = _ʺx30ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx30ʺ _ʺx30ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx31ʺ : _ʺx30ʺⳆʺx31ʺ
        {
            public _ʺx31ʺ(__GeneratedOdataV3.CstNodes.Inners._ʺx31ʺ _ʺx31ʺ_1)
            {
                this._ʺx31ʺ_1 = _ʺx31ʺ_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx31ʺ _ʺx31ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
