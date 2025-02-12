namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _qcharⲻunescaped
    {
        private _qcharⲻunescaped()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_qcharⲻunescaped node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_qcharⲻunescaped._unreserved node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._pctⲻencodedⲻunescaped node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._otherⲻdelims node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._ʺx3Aʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._ʺx40ʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._ʺx2Fʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._ʺx3Fʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._ʺx24ʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._ʺx27ʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻunescaped._ʺx3Dʺ node, TContext context);
        }
        
        public sealed class _unreserved : _qcharⲻunescaped
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
        
        public sealed class _pctⲻencodedⲻunescaped : _qcharⲻunescaped
        {
            public _pctⲻencodedⲻunescaped(__GeneratedOdataV2.CstNodes.Rules._pctⲻencodedⲻunescaped _pctⲻencodedⲻunescaped_1)
            {
                this._pctⲻencodedⲻunescaped_1 = _pctⲻencodedⲻunescaped_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._pctⲻencodedⲻunescaped _pctⲻencodedⲻunescaped_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _otherⲻdelims : _qcharⲻunescaped
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
        
        public sealed class _ʺx3Aʺ : _qcharⲻunescaped
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
        
        public sealed class _ʺx40ʺ : _qcharⲻunescaped
        {
            private _ʺx40ʺ()
            {
                this._ʺx40ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx40ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx40ʺ _ʺx40ʺ_1 { get; }
            public static _ʺx40ʺ Instance { get; } = new _ʺx40ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ : _qcharⲻunescaped
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
        
        public sealed class _ʺx3Fʺ : _qcharⲻunescaped
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
        
        public sealed class _ʺx24ʺ : _qcharⲻunescaped
        {
            private _ʺx24ʺ()
            {
                this._ʺx24ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx24ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx24ʺ _ʺx24ʺ_1 { get; }
            public static _ʺx24ʺ Instance { get; } = new _ʺx24ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx27ʺ : _qcharⲻunescaped
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
        
        public sealed class _ʺx3Dʺ : _qcharⲻunescaped
        {
            private _ʺx3Dʺ()
            {
                this._ʺx3Dʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx3Dʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx3Dʺ _ʺx3Dʺ_1 { get; }
            public static _ʺx3Dʺ Instance { get; } = new _ʺx3Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
