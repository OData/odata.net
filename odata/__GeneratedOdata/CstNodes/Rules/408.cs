namespace __GeneratedOdata.CstNodes.Rules
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
            public _ʺx24ʺ(__GeneratedOdata.CstNodes.Inners._ʺx24ʺ _ʺx24ʺ_1)
            {
                this._ʺx24ʺ_1 = _ʺx24ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx24ʺ _ʺx24ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx26ʺ : _subⲻdelims
        {
            public _ʺx26ʺ(__GeneratedOdata.CstNodes.Inners._ʺx26ʺ _ʺx26ʺ_1)
            {
                this._ʺx26ʺ_1 = _ʺx26ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx26ʺ _ʺx26ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx27ʺ : _subⲻdelims
        {
            public _ʺx27ʺ(__GeneratedOdata.CstNodes.Inners._ʺx27ʺ _ʺx27ʺ_1)
            {
                this._ʺx27ʺ_1 = _ʺx27ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx27ʺ _ʺx27ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Dʺ : _subⲻdelims
        {
            public _ʺx3Dʺ(__GeneratedOdata.CstNodes.Inners._ʺx3Dʺ _ʺx3Dʺ_1)
            {
                this._ʺx3Dʺ_1 = _ʺx3Dʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx3Dʺ _ʺx3Dʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _otherⲻdelims : _subⲻdelims
        {
            public _otherⲻdelims(__GeneratedOdata.CstNodes.Rules._otherⲻdelims _otherⲻdelims_1)
            {
                this._otherⲻdelims_1 = _otherⲻdelims_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._otherⲻdelims _otherⲻdelims_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
