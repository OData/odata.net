namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ
    {
        private _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ node, TContext context);
        }
        
        public sealed class _ALPHA : _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ
        {
            public _ALPHA(__GeneratedOdataV2.CstNodes.Rules._ALPHA _ALPHA_1)
            {
                this._ALPHA_1 = _ALPHA_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._ALPHA _ALPHA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _DIGIT : _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ
        {
            public _DIGIT(__GeneratedOdataV2.CstNodes.Rules._DIGIT _DIGIT_1)
            {
                this._DIGIT_1 = _DIGIT_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._DIGIT _DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Bʺ : _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ
        {
            private _ʺx2Bʺ()
            {
                this._ʺx2Bʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx2Bʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Bʺ _ʺx2Bʺ_1 { get; }
            public static _ʺx2Bʺ Instance { get; } = new _ʺx2Bʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Dʺ : _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ
        {
            private _ʺx2Dʺ()
            {
                this._ʺx2Dʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx2Dʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1 { get; }
            public static _ʺx2Dʺ Instance { get; } = new _ʺx2Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Eʺ : _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ
        {
            private _ʺx2Eʺ()
            {
                this._ʺx2Eʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx2Eʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            public static _ʺx2Eʺ Instance { get; } = new _ʺx2Eʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
