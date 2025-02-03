namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _pcharⲻnoⲻSQUOTE
    {
        private _pcharⲻnoⲻSQUOTE()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_pcharⲻnoⲻSQUOTE node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_pcharⲻnoⲻSQUOTE._unreserved node, TContext context);
            protected internal abstract TResult Accept(_pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE node, TContext context);
            protected internal abstract TResult Accept(_pcharⲻnoⲻSQUOTE._otherⲻdelims node, TContext context);
            protected internal abstract TResult Accept(_pcharⲻnoⲻSQUOTE._ʺx24ʺ node, TContext context);
            protected internal abstract TResult Accept(_pcharⲻnoⲻSQUOTE._ʺx26ʺ node, TContext context);
            protected internal abstract TResult Accept(_pcharⲻnoⲻSQUOTE._ʺx3Dʺ node, TContext context);
            protected internal abstract TResult Accept(_pcharⲻnoⲻSQUOTE._ʺx3Aʺ node, TContext context);
            protected internal abstract TResult Accept(_pcharⲻnoⲻSQUOTE._ʺx40ʺ node, TContext context);
        }
        
        public sealed class _unreserved : _pcharⲻnoⲻSQUOTE
        {
            public _unreserved(__GeneratedOdataV2.CstNodes.Rules._unreserved _unreserved_1)
            {
                this._unreserved_1 = _unreserved_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._unreserved _unreserved_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pctⲻencodedⲻnoⲻSQUOTE : _pcharⲻnoⲻSQUOTE
        {
            public _pctⲻencodedⲻnoⲻSQUOTE(__GeneratedOdataV2.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE _pctⲻencodedⲻnoⲻSQUOTE_1)
            {
                this._pctⲻencodedⲻnoⲻSQUOTE_1 = _pctⲻencodedⲻnoⲻSQUOTE_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE _pctⲻencodedⲻnoⲻSQUOTE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _otherⲻdelims : _pcharⲻnoⲻSQUOTE
        {
            public _otherⲻdelims(__GeneratedOdataV2.CstNodes.Rules._otherⲻdelims _otherⲻdelims_1)
            {
                this._otherⲻdelims_1 = _otherⲻdelims_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._otherⲻdelims _otherⲻdelims_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24ʺ : _pcharⲻnoⲻSQUOTE
        {
            public _ʺx24ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx24ʺ _ʺx24ʺ_1)
            {
                this._ʺx24ʺ_1 = _ʺx24ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24ʺ _ʺx24ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx26ʺ : _pcharⲻnoⲻSQUOTE
        {
            public _ʺx26ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ _ʺx26ʺ_1)
            {
                this._ʺx26ʺ_1 = _ʺx26ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ _ʺx26ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Dʺ : _pcharⲻnoⲻSQUOTE
        {
            public _ʺx3Dʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx3Dʺ _ʺx3Dʺ_1)
            {
                this._ʺx3Dʺ_1 = _ʺx3Dʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx3Dʺ _ʺx3Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Aʺ : _pcharⲻnoⲻSQUOTE
        {
            public _ʺx3Aʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1)
            {
                this._ʺx3Aʺ_1 = _ʺx3Aʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx40ʺ : _pcharⲻnoⲻSQUOTE
        {
            public _ʺx40ʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx40ʺ _ʺx40ʺ_1)
            {
                this._ʺx40ʺ_1 = _ʺx40ʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx40ʺ _ʺx40ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
