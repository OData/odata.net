namespace __GeneratedOdataV4.CstNodes.Rules
{
    public abstract class _qcharⲻJSONⲻspecial
    {
        private _qcharⲻJSONⲻspecial()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_qcharⲻJSONⲻspecial node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_qcharⲻJSONⲻspecial._SP node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻJSONⲻspecial._ʺx3Aʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻJSONⲻspecial._ʺx7Bʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻJSONⲻspecial._ʺx7Dʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻJSONⲻspecial._ʺx5Bʺ node, TContext context);
            protected internal abstract TResult Accept(_qcharⲻJSONⲻspecial._ʺx5Dʺ node, TContext context);
        }
        
        public sealed class _SP : _qcharⲻJSONⲻspecial
        {
            private _SP()
            {
                this._SP_1 = __GeneratedOdataV4.CstNodes.Rules._SP.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Rules._SP _SP_1 { get; }
            public static _SP Instance { get; } = new _SP();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Aʺ : _qcharⲻJSONⲻspecial
        {
            private _ʺx3Aʺ()
            {
                this._ʺx3Aʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            public static _ʺx3Aʺ Instance { get; } = new _ʺx3Aʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx7Bʺ : _qcharⲻJSONⲻspecial
        {
            private _ʺx7Bʺ()
            {
                this._ʺx7Bʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx7Bʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx7Bʺ _ʺx7Bʺ_1 { get; }
            public static _ʺx7Bʺ Instance { get; } = new _ʺx7Bʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx7Dʺ : _qcharⲻJSONⲻspecial
        {
            private _ʺx7Dʺ()
            {
                this._ʺx7Dʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx7Dʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx7Dʺ _ʺx7Dʺ_1 { get; }
            public static _ʺx7Dʺ Instance { get; } = new _ʺx7Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx5Bʺ : _qcharⲻJSONⲻspecial
        {
            private _ʺx5Bʺ()
            {
                this._ʺx5Bʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx5Bʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx5Bʺ _ʺx5Bʺ_1 { get; }
            public static _ʺx5Bʺ Instance { get; } = new _ʺx5Bʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx5Dʺ : _qcharⲻJSONⲻspecial
        {
            private _ʺx5Dʺ()
            {
                this._ʺx5Dʺ_1 = __GeneratedOdataV4.CstNodes.Inners._ʺx5Dʺ.Instance;
            }
            
            public __GeneratedOdataV4.CstNodes.Inners._ʺx5Dʺ _ʺx5Dʺ_1 { get; }
            public static _ʺx5Dʺ Instance { get; } = new _ʺx5Dʺ();
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
