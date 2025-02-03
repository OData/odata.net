namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ
    {
        private _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx24x73x6Bx69x70ʺ node, TContext context);
            protected internal abstract TResult Accept(_ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ._ʺx73x6Bx69x70ʺ node, TContext context);
        }
        
        public sealed class _ʺx24x73x6Bx69x70ʺ : _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ
        {
            public _ʺx24x73x6Bx69x70ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx24x73x6Bx69x70ʺ _ʺx24x73x6Bx69x70ʺ_1)
            {
                this._ʺx24x73x6Bx69x70ʺ_1 = _ʺx24x73x6Bx69x70ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24x73x6Bx69x70ʺ _ʺx24x73x6Bx69x70ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx73x6Bx69x70ʺ : _ʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺ
        {
            public _ʺx73x6Bx69x70ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx73x6Bx69x70ʺ _ʺx73x6Bx69x70ʺ_1)
            {
                this._ʺx73x6Bx69x70ʺ_1 = _ʺx73x6Bx69x70ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx73x6Bx69x70ʺ _ʺx73x6Bx69x70ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
