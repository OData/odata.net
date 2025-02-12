namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _subⲻdelims
    {
        private _subⲻdelims()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_subⲻdelims node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_subⲻdelims._ʺx24ʺ node, TContext context);
            protected internal abstract TResult Accept(_subⲻdelims._ʺx26ʺ node, TContext context);
            protected internal abstract TResult Accept(_subⲻdelims._ʺx27ʺ node, TContext context);
            protected internal abstract TResult Accept(_subⲻdelims._ʺx3Dʺ node, TContext context);
            protected internal abstract TResult Accept(_subⲻdelims._otherⲻdelims node, TContext context);
        }
        
        public sealed class _ʺx24ʺ : _subⲻdelims
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
        
        public sealed class _ʺx26ʺ : _subⲻdelims
        {
            private _ʺx26ʺ()
            {
                this._ʺx26ʺ_1 = __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ.Instance;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx26ʺ _ʺx26ʺ_1 { get; }
            public static _ʺx26ʺ Instance { get; } = new _ʺx26ʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx27ʺ : _subⲻdelims
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
        
        public sealed class _ʺx3Dʺ : _subⲻdelims
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
        
        public sealed class _otherⲻdelims : _subⲻdelims
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
    }
    
}
