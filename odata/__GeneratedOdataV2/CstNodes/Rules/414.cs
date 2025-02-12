namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR
    {
        private _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ node, TContext context);
        }
        
        public sealed class _unreserved : _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR
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
        
        public sealed class _pctⲻencoded : _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR
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
        
        public sealed class _otherⲻdelims : _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR
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
        
        public sealed class _ʺx3Aʺ : _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR
        {
            private _ʺx3Aʺ()
            {
                this._ʺx3Aʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            public static _ʺx3Aʺ Instance { get; } = new _ʺx3Aʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ : _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR
        {
            private _ʺx2Fʺ()
            {
                this._ʺx2Fʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public static _ʺx2Fʺ Instance { get; } = new _ʺx2Fʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Fʺ : _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR
        {
            private _ʺx3Fʺ()
            {
                this._ʺx3Fʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx3Fʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx3Fʺ _ʺx3Fʺ_1 { get; }
            public static _ʺx3Fʺ Instance { get; } = new _ʺx3Fʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx27ʺ : _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR
        {
            private _ʺx27ʺ()
            {
                this._ʺx27ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx27ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx27ʺ _ʺx27ʺ_1 { get; }
            public static _ʺx27ʺ Instance { get; } = new _ʺx27ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
