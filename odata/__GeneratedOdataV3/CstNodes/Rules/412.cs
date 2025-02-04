namespace __GeneratedOdataV3.CstNodes.Rules
{
    public abstract class _qcharⲻnoⲻAMP
    {
        private _qcharⲻnoⲻAMP()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_qcharⲻnoⲻAMP node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._unreserved node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._pctⲻencoded node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._otherⲻdelims node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._ʺx3Aʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._ʺx40ʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._ʺx2Fʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._ʺx3Fʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._ʺx24ʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._ʺx27ʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻnoⲻAMP._ʺx3Dʺ node, TContext context);
        }
        
        public sealed class _unreserved : _qcharⲻnoⲻAMP
        {
            public _unreserved(__GeneratedOdataV3.CstNodes.Rules._unreserved _unreserved_1)
            {
                this._unreserved_1 = _unreserved_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._unreserved _unreserved_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pctⲻencoded : _qcharⲻnoⲻAMP
        {
            public _pctⲻencoded(__GeneratedOdataV3.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1)
            {
                this._pctⲻencoded_1 = _pctⲻencoded_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _otherⲻdelims : _qcharⲻnoⲻAMP
        {
            public _otherⲻdelims(__GeneratedOdataV3.CstNodes.Rules._otherⲻdelims _otherⲻdelims_1)
            {
                this._otherⲻdelims_1 = _otherⲻdelims_1;
            }
            
            public __GeneratedOdataV3.CstNodes.Rules._otherⲻdelims _otherⲻdelims_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Aʺ : _qcharⲻnoⲻAMP
        {
            private _ʺx3Aʺ()
            {
                this._ʺx3Aʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            public static _ʺx3Aʺ Instance { get; } = new _ʺx3Aʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx40ʺ : _qcharⲻnoⲻAMP
        {
            private _ʺx40ʺ()
            {
                this._ʺx40ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx40ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx40ʺ _ʺx40ʺ_1 { get; }
            public static _ʺx40ʺ Instance { get; } = new _ʺx40ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Fʺ : _qcharⲻnoⲻAMP
        {
            private _ʺx2Fʺ()
            {
                this._ʺx2Fʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ _ʺx2Fʺ_1 { get; }
            public static _ʺx2Fʺ Instance { get; } = new _ʺx2Fʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Fʺ : _qcharⲻnoⲻAMP
        {
            private _ʺx3Fʺ()
            {
                this._ʺx3Fʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ _ʺx3Fʺ_1 { get; }
            public static _ʺx3Fʺ Instance { get; } = new _ʺx3Fʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24ʺ : _qcharⲻnoⲻAMP
        {
            private _ʺx24ʺ()
            {
                this._ʺx24ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx24ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx24ʺ _ʺx24ʺ_1 { get; }
            public static _ʺx24ʺ Instance { get; } = new _ʺx24ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx27ʺ : _qcharⲻnoⲻAMP
        {
            private _ʺx27ʺ()
            {
                this._ʺx27ʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx27ʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx27ʺ _ʺx27ʺ_1 { get; }
            public static _ʺx27ʺ Instance { get; } = new _ʺx27ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Dʺ : _qcharⲻnoⲻAMP
        {
            private _ʺx3Dʺ()
            {
                this._ʺx3Dʺ_1 = __GeneratedOdataV3.CstNodes.Inners._ʺx3Dʺ.Instance;
            }
            
            public __GeneratedOdataV3.CstNodes.Inners._ʺx3Dʺ _ʺx3Dʺ_1 { get; }
            public static _ʺx3Dʺ Instance { get; } = new _ʺx3Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
