namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _pchar
    {
        private _pchar()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_pchar node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_pchar._unreserved node, TContext context);
            protected internal abstract TResult Accept(_pchar._pctⲻencoded node, TContext context);
            protected internal abstract TResult Accept(_pchar._subⲻdelims node, TContext context);
            protected internal abstract TResult Accept(_pchar._ʺx3Aʺ node, TContext context);
            protected internal abstract TResult Accept(_pchar._ʺx40ʺ node, TContext context);
        }
        
        public sealed class _unreserved : _pchar
        {
            public _unreserved(__GeneratedOdata.CstNodes.Rules._unreserved _unreserved_1)
            {
                this._unreserved_1 = _unreserved_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._unreserved _unreserved_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _pctⲻencoded : _pchar
        {
            public _pctⲻencoded(__GeneratedOdata.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1)
            {
                this._pctⲻencoded_1 = _pctⲻencoded_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._pctⲻencoded _pctⲻencoded_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _subⲻdelims : _pchar
        {
            public _subⲻdelims(__GeneratedOdata.CstNodes.Rules._subⲻdelims _subⲻdelims_1)
            {
                this._subⲻdelims_1 = _subⲻdelims_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._subⲻdelims _subⲻdelims_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx3Aʺ : _pchar
        {
            public _ʺx3Aʺ(__GeneratedOdata.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1)
            {
                this._ʺx3Aʺ_1 = _ʺx3Aʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx3Aʺ _ʺx3Aʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx40ʺ : _pchar
        {
            public _ʺx40ʺ(__GeneratedOdata.CstNodes.Inners._ʺx40ʺ _ʺx40ʺ_1)
            {
                this._ʺx40ʺ_1 = _ʺx40ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx40ʺ _ʺx40ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}