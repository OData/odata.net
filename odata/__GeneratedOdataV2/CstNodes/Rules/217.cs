namespace __GeneratedOdataV2.CstNodes.Rules
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
            public _SP(__GeneratedOdataV2.CstNodes.Rules._SP _SP_1)
            {
                this._SP_1 = _SP_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Rules._SP _SP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Aʺ : _qcharⲻJSONⲻspecial
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
        
        public sealed class _ʺx7Bʺ : _qcharⲻJSONⲻspecial
        {
            public _ʺx7Bʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx7Bʺ _ʺx7Bʺ_1)
            {
                this._ʺx7Bʺ_1 = _ʺx7Bʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx7Bʺ _ʺx7Bʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx7Dʺ : _qcharⲻJSONⲻspecial
        {
            public _ʺx7Dʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx7Dʺ _ʺx7Dʺ_1)
            {
                this._ʺx7Dʺ_1 = _ʺx7Dʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx7Dʺ _ʺx7Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx5Bʺ : _qcharⲻJSONⲻspecial
        {
            public _ʺx5Bʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx5Bʺ _ʺx5Bʺ_1)
            {
                this._ʺx5Bʺ_1 = _ʺx5Bʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx5Bʺ _ʺx5Bʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx5Dʺ : _qcharⲻJSONⲻspecial
        {
            public _ʺx5Dʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx5Dʺ _ʺx5Dʺ_1)
            {
                this._ʺx5Dʺ_1 = _ʺx5Dʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx5Dʺ _ʺx5Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
