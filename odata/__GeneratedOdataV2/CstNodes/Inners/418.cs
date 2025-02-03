namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx2DʺⳆʺx2Bʺ
    {
        private _ʺx2DʺⳆʺx2Bʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx2DʺⳆʺx2Bʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx2DʺⳆʺx2Bʺ._ʺx2Dʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx2DʺⳆʺx2Bʺ._ʺx2Bʺ node, TContext context);
        }
        
        public sealed class _ʺx2Dʺ : _ʺx2DʺⳆʺx2Bʺ
        {
            public _ʺx2Dʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1)
            {
                this._ʺx2Dʺ_1 = _ʺx2Dʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Bʺ : _ʺx2DʺⳆʺx2Bʺ
        {
            public _ʺx2Bʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx2Bʺ _ʺx2Bʺ_1)
            {
                this._ʺx2Bʺ_1 = _ʺx2Bʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Bʺ _ʺx2Bʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
