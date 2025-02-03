namespace __GeneratedOdataV2.CstNodes.Rules
{
    public abstract class _unreserved
    {
        private _unreserved()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_unreserved node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_unreserved._ALPHA node, TContext context);
            protected internal abstract TResult Accept(_unreserved._DIGIT node, TContext context);
            protected internal abstract TResult Accept(_unreserved._ʺx2Dʺ node, TContext context);
            protected internal abstract TResult Accept(_unreserved._ʺx2Eʺ node, TContext context);
            protected internal abstract TResult Accept(_unreserved._ʺx5Fʺ node, TContext context);
            protected internal abstract TResult Accept(_unreserved._ʺx7Eʺ node, TContext context);
        }
        
        public sealed class _ALPHA : _unreserved
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
        
        public sealed class _DIGIT : _unreserved
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
        
        public sealed class _ʺx2Dʺ : _unreserved
        {
            public _ʺx2Dʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1)
            {
                this._ʺx2Dʺ_1 = _ʺx2Dʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx2Dʺ _ʺx2Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx2Eʺ : _unreserved
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
        
        public sealed class _ʺx5Fʺ : _unreserved
        {
            public _ʺx5Fʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx5Fʺ _ʺx5Fʺ_1)
            {
                this._ʺx5Fʺ_1 = _ʺx5Fʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx5Fʺ _ʺx5Fʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx7Eʺ : _unreserved
        {
            public _ʺx7Eʺ(__GeneratedOdataV2.CstNodes.Inners._ʺx7Eʺ _ʺx7Eʺ_1)
            {
                this._ʺx7Eʺ_1 = _ʺx7Eʺ_1;
            }
            
            public __GeneratedOdataV2.CstNodes.Inners._ʺx7Eʺ _ʺx7Eʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
