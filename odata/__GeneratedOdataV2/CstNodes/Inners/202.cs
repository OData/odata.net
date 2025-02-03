namespace __GeneratedOdataV2.CstNodes.Inners
{
    public abstract class _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded
    {
        private _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ node, TContext context);
            protected internal abstract TResult Accept(_ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded node, TContext context);
        }
        
        public sealed class _ALPHA : _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded
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
        
        public sealed class _DIGIT : _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded
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
        
        public sealed class _COMMA : _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded
        {
            public _COMMA(__GeneratedOdataV2.CstNodes.Rules._COMMA _COMMA_1)
            {
                this._COMMA_1 = _COMMA_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._COMMA _COMMA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Eʺ : _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded
        {
            public _ʺx2Eʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1)
            {
                this._ʺx2Eʺ_1 = _ʺx2Eʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Eʺ _ʺx2Eʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pctⲻencoded : _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded
        {
            public _pctⲻencoded(__GeneratedOdataV2.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1)
            {
                this._pctⲻencoded_1 = _pctⲻencoded_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
