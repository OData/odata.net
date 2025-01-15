namespace __GeneratedV2.CstNodes.Inners
{
    public abstract class _ALPHAⳆDIGITⳆʺx2Dʺ
    {
        private _ALPHAⳆDIGITⳆʺx2Dʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ALPHAⳆDIGITⳆʺx2Dʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆʺx2Dʺ._ALPHA node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆʺx2Dʺ._DIGIT node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆʺx2Dʺ._ʺx2Dʺ node, TContext context);
        }
        
        public sealed class _ALPHA : _ALPHAⳆDIGITⳆʺx2Dʺ
        {
            public _ALPHA(__GeneratedV2.CstNodes.Rules._ALPHA _ALPHA_1)
            {
                this._ALPHA_1 = _ALPHA_1;
            }
            
            public __GeneratedV2.CstNodes.Rules._ALPHA _ALPHA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _DIGIT : _ALPHAⳆDIGITⳆʺx2Dʺ
        {
            public _DIGIT(__GeneratedV2.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedV2.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Dʺ : _ALPHAⳆDIGITⳆʺx2Dʺ
        {
            public _ʺx2Dʺ(__GeneratedV2.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1)
            {
                this._ʺx2Dʺ_1 = _ʺx2Dʺ_1;
            }
            
            public __GeneratedV2.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
